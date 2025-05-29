using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ReviewersReportStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"Create PROCEDURE [dbo].[GetReviewersReport]
    @EmployeeIds NVARCHAR(MAX) = NULL,              -- Filter by specific employee IDs
    @FromDate DATETIME = NULL,                      -- Start date for filtering
    @ToDate DATETIME = NULL,                        -- End date for filtering
    @ReviewerName NVARCHAR(200) = NULL,             -- Filter by reviewer name (partial match)
    @ReviewerStatus NVARCHAR(50) = NULL,            -- Filter by reviewer status
    @Skip INT = 0,                                  -- Number of records to skip (for pagination)
    @Take INT = NULL                                -- Number of records to take (NULL = all records)
AS
BEGIN
    -- Prevent row count messages from being returned to improve performance
    SET NOCOUNT ON;
    
    -- Validate pagination parameters
    SET @Skip = ISNULL(@Skip, 0);
    IF @Skip < 0 SET @Skip = 0;
    
    -- Set default date range if not provided
    -- Default from date: earliest possible date to include all historical records
    SET @FromDate = ISNULL(@FromDate, '1900-01-01');
    -- Default to date: current date and time to include all records up to now
    SET @ToDate = ISNULL(@ToDate, GETDATE());

    -- =============================================
    -- CTE 1: LatestWorkFlowLog
    -- Purpose: Get only the most recent WorkFlowProcessingLog entry for each LitigationRequest
    -- This prevents duplicate litigation requests in the final result
    -- =============================================
    ;WITH LatestWorkFlowLog AS (
        SELECT 
            WF.RequestId,                           -- The litigation request ID
            WF.EmployeeId,                          -- The employee assigned to this workflow step
            -- Use ROW_NUMBER to rank workflow entries by recency for each request
            -- Most recent entry gets rn=1, older entries get higher numbers
            ROW_NUMBER() OVER (
                PARTITION BY WF.RequestId           -- Group by each litigation request
                ORDER BY WF.CreatedOn DESC,         -- Order by creation date (newest first)
                         WF.Id DESC                 -- Use ID as tiebreaker for same timestamps
            ) AS rn
        FROM WorkFlowProcessingLog WF
    ),
    -- =============================================
    -- CTE 2: BaseLitigations
    -- Purpose: Main query that joins litigation requests with reviewer information
    -- and applies all the filtering criteria specified in parameters
    -- =============================================
    BaseLitigations AS (
        SELECT 
            -- Core litigation request fields
            L.Id,                                   -- Unique litigation request identifier
            L.IsFinished,                           -- Boolean flag indicating if case is completed
            L.LitigationRequestStage,               -- Current stage of the litigation process
            L.ReconciliationPeriod,                 -- Days remaining in reconciliation period
            L.ArbitrationPeriod,                    -- Days remaining in arbitration period
            L.StartReconciliationDate,              -- When reconciliation phase started
            L.StartArbitrationDate,                 -- When arbitration phase started
            L.SentOn,                              -- Date when litigation request was sent
            L.CreatedOn,                           -- Date when litigation request was created
            L.AreaId,                              -- Geographic area identifier
            L.CityId,                              -- City identifier
            L.LitigationRequestCategoryId,         -- Category/type of litigation
            L.Fees,                                -- Associated fees for the litigation
            L.ContractId,                          -- Related contract identifier
            L.IsReminderForReviewerManagerSent,    -- Flag for manager reminder status
            L.IsReminderForReviewerSent,           -- Flag for reviewer reminder status
            
            -- Reviewer information from employee table
            E.EmployeeName AS ReviewerName,         -- Name of assigned reviewer
            -- Convert boolean IsActive to descriptive text
            CASE WHEN E.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS ReviewerStatus
        FROM LitigationRequest L
        
        -- Join with latest workflow log (only one record per litigation request)
        LEFT JOIN LatestWorkFlowLog WF ON L.Id = WF.RequestId AND WF.rn = 1
        
        -- Join with employee table to get reviewer details
        LEFT JOIN SysEmployee E ON WF.EmployeeId = E.Id
        
        -- =============================================
        -- WHERE clause: Apply remaining filtering criteria
        -- =============================================
        WHERE
            -- Employee ID filtering (simplified - only checks workflow processing log employee)
            (@EmployeeIds IS NULL OR WF.EmployeeId IN (
                SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')
            ))
            
            -- Filter by date range (SentOn date must be within specified range)
            AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
            
            -- Filter by reviewer name (partial match, case-insensitive)
            AND (@ReviewerName IS NULL OR E.EmployeeName LIKE '%' + @ReviewerName + '%')
            
            -- Filter by reviewer status with proper boolean conversion
            AND (@ReviewerStatus IS NULL OR 
                 (@ReviewerStatus = 'Active' AND E.IsActive = 1) OR 
                 (@ReviewerStatus = 'Inactive' AND E.IsActive = 0))
    )
    -- =============================================
    -- MAIN QUERY: Generate aggregated report by reviewer
    -- Purpose: Group litigation data by reviewer and calculate key statistics
    -- =============================================
    SELECT
        -- Reviewer identification with fallback for unassigned cases
        ISNULL(ReviewerName, 'Unassigned') AS ReviewerName,
        ISNULL(ReviewerStatus, 'Unknown') AS ReviewerStatus,
        
        -- Core count metrics
        COUNT(1) AS TotalLitigationCount,                   -- Total cases assigned to this reviewer
        
        -- Completion status counts
        SUM(CASE WHEN LitigationRequestStage = 1 THEN 1 ELSE 0 END) AS NotFinishedCount,  -- Cases in stage 1 (not finished)
        SUM(CASE WHEN LitigationRequestStage != 1 THEN 1 ELSE 0 END) AS FinishedCount,     -- Cases not in stage 1 (finished)
        

        -- Review stage due date violations (when both reminder flags are set)
        SUM(CASE 
            WHEN LitigationRequestStage = 1                     -- Review stage
                 AND IsReminderForReviewerManagerSent = 1       -- Manager reminder sent
                 AND IsReminderForReviewerSent = 1              -- Reviewer reminder sent
                 AND CreatedOn >= @FromDate                     -- Within date range
                 AND CreatedOn <= @ToDate                       -- Within date range
            THEN 1 ELSE 0 
        END) AS TotalReviewDueDateCount,        -- Overdue review cases (both reminders sent)
        
        -- Performance ratios (percentages)
        -- Calculate percentage of cases that are not finished (stage 1)
        CAST(
            SUM(CASE WHEN LitigationRequestStage = 1 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                 -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS NotFinishedRatio,                  -- Percentage of pending cases (stage 1)
        
        -- Calculate percentage of cases that are finished (not stage 1)
        CAST(
            SUM(CASE WHEN LitigationRequestStage != 1 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                 -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS FinishedRatio,                     -- Percentage of completed cases (not stage 1)
        
        -- Calculate percentage of review cases that are overdue (both reminders sent)
        CAST(
            SUM(CASE 
                WHEN LitigationRequestStage = 1                     -- Review stage
                     AND IsReminderForReviewerManagerSent = 1       -- Manager reminder sent
                     AND IsReminderForReviewerSent = 1              -- Reviewer reminder sent
                     AND CreatedOn >= @FromDate                     -- Within date range
                     AND CreatedOn <= @ToDate                       -- Within date range
                THEN 1 ELSE 0 
            END) * 100.0 / NULLIF(COUNT(1), 0)  -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS TotalReviewDueDateRatio            -- Percentage of overdue review cases
        
    FROM BaseLitigations
    WHERE ReviewerName IS NOT NULL             -- Exclude records without assigned reviewers
    GROUP BY ReviewerName, ReviewerStatus      -- Group by reviewer to get per-reviewer statistics
    ORDER BY ReviewerName                      -- Sort results alphabetically by reviewer name
    
    -- =============================================
    -- PAGINATION: Apply OFFSET and FETCH NEXT for pagination
    -- =============================================
    OFFSET @Skip ROWS                          -- Skip the specified number of rows
    FETCH NEXT 
        CASE 
            WHEN @Take IS NULL THEN 2147483647 -- If @Take is NULL, fetch maximum possible rows
            ELSE @Take 
        END ROWS ONLY                          -- Fetch only the specified number of rows
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetReviewersReportStoredProcedure]");
        }
    }
}
