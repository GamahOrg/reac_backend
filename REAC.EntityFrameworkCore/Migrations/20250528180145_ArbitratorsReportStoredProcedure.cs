using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ArbitratorsReportStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"ALTER PROCEDURE [dbo].[GetArbitratorsReport]
    @EmployeeIds NVARCHAR(MAX) = NULL,              -- Filter by specific arbitrator employee IDs
    @FromDate DATETIME = NULL,                      -- Start date for filtering
    @ToDate DATETIME = NULL,                        -- End date for filtering
    @ArbitratorName NVARCHAR(200) = NULL,           -- Filter by arbitrator name (partial match)
    @ArbitratorStatus NVARCHAR(50) = NULL,          -- Filter by arbitrator status (Active/Inactive)
    @ExperienceYears INT = NULL,                    -- Filter by years of experience
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
    -- CTE 1: LatestArbitratorAssignment
    -- Purpose: Get the most recent arbitrator assignment for each LitigationRequest
    -- This ensures we get the current arbitrator for each case
    -- =============================================
    ;WITH LatestArbitratorAssignment AS (
        SELECT 
            L.Id AS RequestId,                      -- The litigation request ID
            L.ArbitratorCitizenId,                  -- The arbitrator citizen ID assigned to this case
            -- Use ROW_NUMBER to rank assignments by recency for each request
            -- Most recent assignment gets rn=1, older assignments get higher numbers
            ROW_NUMBER() OVER (
                PARTITION BY L.Id                   -- Group by each litigation request
                ORDER BY L.CreatedOn DESC,          -- Order by creation date (newest first)
                         L.Id DESC                  -- Use ID as tiebreaker for same timestamps
            ) AS rn
        FROM LitigationRequest L
        WHERE L.ArbitratorCitizenId IS NOT NULL     -- Only include cases with assigned arbitrators
    ),
    -- =============================================
    -- CTE 2: BaseArbitratorLitigations
    -- Purpose: Main query that joins litigation requests with arbitrator information
    -- and applies all the filtering criteria specified in parameters
    -- =============================================
    BaseArbitratorLitigations AS (
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
            L.ArbitratorCitizenId,                 -- Arbitrator citizen ID assigned to this case
            
            -- Arbitrator information from citizen and citizen request tables
            C.FullName AS ArbitratorName,           -- Name of assigned arbitrator
            CR.ExperienceYears,                     -- Years of experience of the arbitrator
            -- Convert boolean ArbitratorStatusId to descriptive text
            CASE WHEN C.ArbitratorStatusId = 1 THEN 'Active' ELSE 'Inactive' END AS ArbitratorStatus
        FROM LitigationRequest L
        
        -- Join with latest arbitrator assignment (only one record per litigation request)
        INNER JOIN
        (
            SELECT
                L2.*,
                ROW_NUMBER() OVER (PARTITION BY L2.Id ORDER BY L2.CreatedOn DESC) AS rn
            FROM
                LitigationRequest AS L2
            WHERE
                L2.ArbitratorCitizenId IS NOT NULL
        ) AS LAA ON LAA.Id = L.Id AND LAA.rn = 1
        
        -- Join with citizen table to get arbitrator details
        LEFT JOIN Citizen C ON L.ArbitratorCitizenId = C.Id
        
        -- Join with citizen request table to get experience information
        LEFT JOIN CitizenRequest CR ON L.ArbitratorCitizenId = CR.Id
        
        -- =============================================
        -- WHERE clause: Apply remaining filtering criteria
        -- =============================================
        WHERE
            (L.LitigationRequestStage = 3)          -- Only arbitration stage cases
            AND
            -- Employee ID filtering (arbitrator citizen ID)
            (@EmployeeIds IS NULL OR L.ArbitratorCitizenId IN (
                SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')
            ))
            
            -- Filter by date range (SentOn date must be within specified range)
            AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
            
            -- Filter by arbitrator name (partial match, case-insensitive)
            AND (@ArbitratorName IS NULL OR C.FullName LIKE '%' + @ArbitratorName + '%')
            
            -- Filter by arbitrator status with proper boolean conversion
            AND (@ArbitratorStatus IS NULL OR 
                 (@ArbitratorStatus = 'Active' AND C.ArbitratorStatusId = 1) OR 
                 (@ArbitratorStatus = 'Inactive' AND C.ArbitratorStatusId = 0))
            
            -- Filter by years of experience
            AND (@ExperienceYears IS NULL OR CR.ExperienceYears >= @ExperienceYears)
    )
    -- =============================================
    -- MAIN QUERY: Generate aggregated report by arbitrator
    -- Purpose: Group litigation data by arbitrator and calculate key statistics
    -- =============================================
    SELECT
        -- Arbitrator identification with fallback for unassigned cases
        ISNULL(ArbitratorName, 'Unassigned') AS ArbitratorName,
        ISNULL(ArbitratorStatus, 'Unknown') AS ArbitratorStatus,
        ISNULL(ExperienceYears, 0) AS ExperienceYears,
        
        -- Core count metrics
        COUNT(1) AS TotalLitigationCount,                   -- Total cases assigned to this arbitrator
        
        -- Completion status counts (following same pattern as reviewers/reformers)
        SUM(CASE WHEN IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,    -- Cases not finished
        SUM(CASE WHEN IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,       -- Cases finished
        
        
        -- Delayed cases (cases in stage 3 that have exceeded arbitration period)
        SUM(CASE 
            WHEN LitigationRequestStage = 3                 -- In arbitration stage
                 AND ArbitrationPeriod = 0                  -- Period has expired
            THEN 1 ELSE 0 
        END) AS DueDateCount,                          -- Overdue arbitration cases
        
        -- Performance ratios (percentages)
        -- Calculate percentage of cases that are not finished
        CAST(
            SUM(CASE WHEN IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS NotFinishedRatio,                              -- Percentage of pending cases
        
        -- Calculate percentage of cases that are finished
        CAST(
            SUM(CASE WHEN IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS FinishedRatio,                                 -- Percentage of completed cases
                                -- Percentage of current arbitration cases
        
        -- Calculate percentage of delayed cases
        CAST(
            SUM(CASE 
                WHEN LitigationRequestStage = 3 AND ArbitrationPeriod = 0 
                THEN 1 ELSE 0 
            END) * 100.0 / NULLIF(COUNT(1), 0)              -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS DueDateRatio                              -- Percentage of overdue arbitration cases
        
    FROM BaseArbitratorLitigations
    WHERE ArbitratorName IS NOT NULL                        -- Exclude records without assigned arbitrators
    GROUP BY ArbitratorName, ArbitratorStatus, ExperienceYears  -- Group by arbitrator to get per-arbitrator statistics
    ORDER BY ArbitratorName                                 -- Sort results alphabetically by arbitrator name
    
    -- =============================================
    -- PAGINATION: Apply OFFSET and FETCH NEXT for pagination
    -- =============================================
    OFFSET @Skip ROWS                                       -- Skip the specified number of rows
    FETCH NEXT 
        CASE 
            WHEN @Take IS NULL THEN 2147483647              -- If @Take is NULL, fetch maximum possible rows
            ELSE @Take 
        END ROWS ONLY                                       -- Fetch only the specified number of rows
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetArbitratorsReport]");
        }
    }
}
