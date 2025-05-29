using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class ReformersReportStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"create PROCEDURE [dbo].[GetReformersReport]
    @EmployeeIds NVARCHAR(MAX) = NULL,              -- Filter by specific reformer employee IDs
    @FromDate DATETIME = NULL,                      -- Start date for filtering
    @ToDate DATETIME = NULL,                        -- End date for filtering
    @ReformerName NVARCHAR(200) = NULL,             -- Filter by reformer name (partial match)
    @ReformerStatus NVARCHAR(50) = NULL,            -- Filter by reformer status (Active/Inactive)
    @ExperienceYears INT = NULL                     -- Filter by years of experience
AS
BEGIN
    -- Prevent row count messages from being returned to improve performance
    SET NOCOUNT ON;
    
    -- Set default date range if not provided
    -- Default from date: earliest possible date to include all historical records
    SET @FromDate = ISNULL(@FromDate, '1900-01-01');
    -- Default to date: current date and time to include all records up to now
    SET @ToDate = ISNULL(@ToDate, GETDATE());

    -- =============================================
    -- CTE: BaseReformerLitigations
    -- Purpose: Main query that gets litigation requests assigned to reformers
    -- and applies all the filtering criteria specified in parameters
    -- =============================================
    ;WITH BaseReformerLitigations AS (
        SELECT 
            -- Core litigation request fields
            L.Id,                                   -- Unique litigation request identifier
            L.LitigationRequestStage,               -- Current stage of the litigation process
            L.ReconciliationPeriod,                 -- Days remaining in reconciliation period
            L.StartReconciliationDate,              -- When reconciliation phase started
            L.SentOn,                              -- Date when litigation request was sent
            L.CreatedOn,                           -- Date when litigation request was created
            L.ReformerCitizenId,                   -- Reformer citizen ID assigned to this case
            
            -- Reformer information from employee table
            C.FullName AS ReformerName,         -- Name of assigned reformer
            CR.ExperienceYears,                      -- Years of experience of the reformer
            -- Convert boolean IsActive to descriptive text
            CASE WHEN C.ReformerStatusId = 1 THEN 'Active' ELSE 'Inactive' END AS ReformerStatus
        FROM LitigationRequest L
        LEFT JOIN CitizenRequest CR ON L.ReformerCitizenId = CR.Id
        -- Join with employee table to get reformer details
        LEFT JOIN Citizen C ON L.ReformerCitizenId = C.Id
        
        -- =============================================
        -- WHERE clause: Apply filtering criteria for reformers
        -- =============================================
        WHERE
            -- Filter by stage 2 (Reconciliation) and specific reformer employee IDs
            (@EmployeeIds IS NULL OR 
             (L.LitigationRequestStage = 2 AND L.ReformerCitizenId IN (
                SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')
             )))
            
            -- Filter by date range (SentOn date must be within specified range)
            AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
            
            -- Filter by reformer name (partial match, case-insensitive)
            AND (@ReformerName IS NULL OR C.FullName LIKE '%' + @ReformerName + '%')
            
            -- Filter by reformer status with proper boolean conversion
            AND (@ReformerStatus IS NULL OR 
                 (@ReformerStatus = 'Active' AND C.ReformerStatusId = 1) OR 
                 (@ReformerStatus = 'Inactive' AND C.ReformerStatusId = 0))
            
            -- Filter by years of experience
            AND (@ExperienceYears IS NULL OR CR.ExperienceYears >= @ExperienceYears)
    )
    -- =============================================
    -- MAIN QUERY: Generate aggregated report by reformer
    -- Purpose: Group litigation data by reformer and calculate key statistics
    -- =============================================
    SELECT
        -- Reformer identification with fallback for unassigned cases
        ISNULL(ReformerName, '??? ????') AS ReformerName,                   
        ISNULL(ReformerStatus, '??? ?????') AS ReformerStatus,              
        ISNULL(ExperienceYears, 0) AS ExperienceYears,                      
        
        -- Core count metrics
        COUNT(1) AS TotalLitigationCount,                                  
        
        -- Current cases (cases still in reconciliation stage 2)
        SUM(CASE 
            WHEN LitigationRequestStage = 2 
            THEN 1 ELSE 0 
        END) AS CurrentCasesCount,                                         
        
        -- Delayed cases (cases in stage 2 that have exceeded reconciliation period)
        SUM(CASE 
            WHEN LitigationRequestStage = 2                                
                 AND ReconciliationPeriod = 0                             
            THEN 1 ELSE 0 
        END) AS DelayedCasesCount,                                        
        
        -- Completed cases (cases that moved beyond stage 2)
        SUM(CASE 
            WHEN LitigationRequestStage > 2                               
            THEN 1 ELSE 0 
        END) AS CompletedCasesCount,                                        
        
        -- Performance ratios (percentages)
        -- Calculate percentage of current cases
        CAST(
            SUM(CASE WHEN LitigationRequestStage = 2 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS CurrentCasesRatio,                                             
        
        -- Calculate percentage of delayed cases
        CAST(
            SUM(CASE 
                WHEN LitigationRequestStage = 2 AND ReconciliationPeriod = 0 
                THEN 1 ELSE 0 
            END) * 100.0 / NULLIF(COUNT(1), 0)                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS DelayedCasesRatio,                                             
        
        -- Calculate percentage of completed cases
        CAST(
            SUM(CASE WHEN LitigationRequestStage > 2 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS CompletedCasesRatio                                            
        
    FROM BaseReformerLitigations
    WHERE ReformerName IS NOT NULL                                          -- Exclude records without assigned reformers
    GROUP BY ReformerName, ReformerStatus, ExperienceYears                  -- Group by reformer to get per-reformer statistics
    ORDER BY ReformerName                                                   -- Sort results alphabetically by reformer name
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetReformersReport]");
        }
    }
}
