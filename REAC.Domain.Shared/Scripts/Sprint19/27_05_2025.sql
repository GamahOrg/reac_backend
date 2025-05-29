
-- Procedure: GetReviewersReport
-- Description: Generates a comprehensive report of litigation reviewers and their case statistics
-- Parameters:
--   @EmployeeIds: Comma-separated list of employee IDs to filter by
--   @LitigationRequestStageId: Stage ID to filter litigation requests (1=Review, 2=Reconciliation, 3=Arbitration)
--   @FromDate/@ToDate: Date range filter based on SentOn date
--   @ReviewerName: Partial name match filter for reviewer names
--   @ReviewerStatus: Filter by reviewer status ('Active' or 'Inactive')
ALTER PROCEDURE [dbo].[GetReviewersReport]
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
           INNER JOIN
    (
        SELECT
            WFP.*,
            ROW_NUMBER() OVER (PARTITION BY WFP.RequestId ORDER BY WFP.CreatedOn DESC) AS rn
        FROM
            WorkFlowProcessingLog AS WFP
        WHERE
            WFP.EmployeeId IS NOT NULL
    ) AS W ON W.RequestId = L.Id AND W.rn = 1
        
        -- Join with employee table to get reviewer details
        LEFT JOIN SysEmployee E ON w.EmployeeId = E.Id
        
        -- =============================================
        -- WHERE clause: Apply remaining filtering criteria
        -- =============================================
        WHERE
		(L.LitigationRequestStage = 1)
		And
            -- Employee ID filtering (simplified - only checks workflow processing log employee)
            (@EmployeeIds IS NULL OR w.EmployeeId IN (
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
        SUM(CASE WHEN IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,  -- Cases in stage 1 (not finished)
        SUM(CASE WHEN IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,     -- Cases not in stage 1 (finished)
        

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
            SUM(CASE WHEN  IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                 -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS NotFinishedRatio,                  -- Percentage of pending cases (stage 1)
        
        -- Calculate percentage of cases that are finished (not stage 1)
        CAST(
            SUM(CASE WHEN  IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / 
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
END
GO
-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------
Alter PROCEDURE [dbo].[GetReformersReport]
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
            (L.LitigationRequestStage = 2)
            And
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
                 (@ReformerStatus = 1 AND C.ReformerStatusId = 1) OR 
                 (@ReformerStatus = 0 AND C.ReformerStatusId = 0))
            
            -- Filter by years of experience
            AND (@ExperienceYears IS NULL OR CR.ExperienceYears >= @ExperienceYears)
    )
    -- =============================================
    -- MAIN QUERY: Generate aggregated report by reformer
    -- Purpose: Group litigation data by reformer and calculate key statistics
    -- =============================================
    SELECT
        -- Reformer identification with fallback for unassigned cases
        ISNULL(ReformerName, 'غير مسند') AS ReformerName,                    -- اسم المصلح
        ISNULL(ReformerStatus, 'غير معروف') AS ReformerStatus,              -- حالة المصلح
        ISNULL(ExperienceYears, 0) AS ExperienceYears,                      -- عدد سنوات الخبرة
        
        -- Core count metrics
        COUNT(1) AS TotalLitigationCount,                                   -- عدد الدعاوى الإجمالية تم إسنادها للمصلح
        
        -- Current cases (cases still in reconciliation stage 2)
        SUM(CASE 
            WHEN LitigationRequestStage = 2 
            THEN 1 ELSE 0 
        END) AS CurrentCasesCount,                                          -- عدد الدعاوى الحالية قيد الدراسة المسندة للمصلح
        
        -- Delayed cases (cases in stage 2 that have exceeded reconciliation period)
        SUM(CASE 
            WHEN LitigationRequestStage = 2                                 -- في مرحلة المصالحة
                 AND ReconciliationPeriod = 0                               -- انتهت المدة المقررة
            THEN 1 ELSE 0 
        END) AS DelayedCasesCount,                                          -- عدد الدعاوى المتأخرة قيد الدراسة وتجاوزت المدة المقررة
        
        -- Completed cases (cases that moved beyond stage 2)
        SUM(CASE 
            WHEN LitigationRequestStage > 2                                 -- تجاوزت مرحلة المصالحة
            THEN 1 ELSE 0 
        END) AS CompletedCasesCount,                                        -- عدد الدعاوى المنتهية المسندة للمصلح وتم الانتهاء منها
        
        -- Performance ratios (percentages)
        -- Calculate percentage of current cases
        CAST(
            SUM(CASE WHEN LitigationRequestStage = 2 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS CurrentCasesRatio,                                             -- نسبة الدعاوى الحالية
        
        -- Calculate percentage of delayed cases
        CAST(
            SUM(CASE 
                WHEN LitigationRequestStage = 2 AND ReconciliationPeriod = 0 
                THEN 1 ELSE 0 
            END) * 100.0 / NULLIF(COUNT(1), 0)                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS DelayedCasesRatio,                                             -- نسبة الدعاوى المتأخرة
        
        -- Calculate percentage of completed cases
        CAST(
            SUM(CASE WHEN LitigationRequestStage > 2 THEN 1 ELSE 0 END) * 100.0 / 
            NULLIF(COUNT(1), 0)                                             -- Avoid division by zero
            AS DECIMAL(5,2)
        ) AS CompletedCasesRatio                                            -- نسبة الدعاوى المنتهية
        
    FROM BaseReformerLitigations
    WHERE ReformerName IS NOT NULL                                          -- Exclude records without assigned reformers
    GROUP BY ReformerName, ReformerStatus, ExperienceYears                  -- Group by reformer to get per-reformer statistics
    ORDER BY ReformerName                                                   -- Sort results alphabetically by reformer name
END
GO
------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------
ALTER PROCEDURE [dbo].[GetArbitratorsReport]
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
                 (@ArbitratorStatus = 1 AND C.ArbitratorStatusId = 1) OR 
                 (@ArbitratorStatus = 0  AND C.ArbitratorStatusId = 0))
            
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
END