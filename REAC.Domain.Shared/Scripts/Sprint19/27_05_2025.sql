
GO
/****** Object:  StoredProcedure [dbo].[GetReviewersReport]    Script Date: 6/4/2025 1:57:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
    SET @FromDate = ISNULL(@FromDate, '1900-01-01');
    SET @ToDate = ISNULL(@ToDate, GETDATE());

    -- =============================================
    -- CTE 1: AllReviewers - Get ALL reviewer employees
    -- Purpose: Start with ALL reviewers first, regardless of litigation assignment
    -- =============================================
    WITH AllReviewers AS (
        SELECT DISTINCT
            E.Id AS EmployeeId,
            E.EmployeeName AS ReviewerName,
            CASE WHEN E.IsActive = 1 THEN 'Active' ELSE 'Inactive' END AS ReviewerStatus
        FROM SysEmployee E
        INNER JOIN SysDepartment D ON E.DepartmentId = D.Id
        WHERE 
            -- Get ALL employees from departments with DepartmentType = 1 (reviewer departments)
            D.DepartmentType = 1
            
            -- Apply reviewer-specific filters
            AND (@EmployeeIds IS NULL OR E.Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')))
            AND (@ReviewerName IS NULL OR E.EmployeeName LIKE '%' + @ReviewerName + '%')
            AND (@ReviewerStatus IS NULL OR 
                 (@ReviewerStatus = 'Active' AND E.IsActive = 1) OR 
                 (@ReviewerStatus = 'Inactive' AND E.IsActive = 0))
    ),
    
    -- =============================================
    -- CTE 2: ReviewerLitigationStats - Get litigation statistics for each reviewer
    -- Purpose: Calculate statistics for litigation requests assigned to each reviewer
    -- =============================================
    ReviewerLitigationStats AS (
        SELECT 
            R.EmployeeId,
            R.ReviewerName,
            R.ReviewerStatus,
            
            -- Reviewer litigation statistics
            COUNT(L.Id) AS TotalLitigationCount,
            SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
            SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
            
            -- Review stage due date violations (when both reminder flags are set)
            SUM(CASE 
                WHEN L.LitigationRequestStage = 1                     -- Review stage
                     AND L.IsReminderForReviewerManagerSent = 1       -- Manager reminder sent
                     AND L.IsReminderForReviewerSent = 1              -- Reviewer reminder sent
                     AND L.CreatedOn >= @FromDate                     -- Within date range
                     AND L.CreatedOn <= @ToDate                       -- Within date range
                THEN 1 ELSE 0 
            END) AS TotalReviewDueDateCount
            
        FROM AllReviewers R
        
        -- Left join with WorkFlowProcessingLog to get litigation assignments
        LEFT JOIN (
            SELECT WFP.*,
                   ROW_NUMBER() OVER (PARTITION BY WFP.RequestId ORDER BY WFP.CreatedOn DESC) AS rn
            FROM WorkFlowProcessingLog WFP
            WHERE WFP.EmployeeId IS NOT NULL
        ) W ON W.EmployeeId = R.EmployeeId AND W.rn = 1
        
        -- Left join with LitigationRequest to get case details
        LEFT JOIN LitigationRequest L ON L.Id = W.RequestId
                                      AND L.LitigationRequestStage = 1
                                      AND L.SentOn >= @FromDate 
                                      AND L.SentOn <= @ToDate
        
        GROUP BY R.EmployeeId, R.ReviewerName, R.ReviewerStatus
    )
    
    -- =============================================
    -- MAIN QUERY: Generate final reviewer report with statistics
    -- =============================================
    SELECT
        ReviewerName,
        ReviewerStatus,
        
        -- Core count metrics
        ISNULL(TotalLitigationCount, 0) AS TotalLitigationCount,
        ISNULL(NotFinishedCount, 0) AS NotFinishedCount,
        ISNULL(FinishedCount, 0) AS FinishedCount,
        ISNULL(TotalReviewDueDateCount, 0) AS TotalReviewDueDateCount,
        
        -- Performance ratios (percentages)
        CAST(
            ISNULL(NotFinishedCount, 0) * 100.0 / 
            NULLIF(ISNULL(TotalLitigationCount, 0), 0)
            AS DECIMAL(5,2)
        ) AS NotFinishedRatio,
        
        CAST(
            ISNULL(FinishedCount, 0) * 100.0 / 
            NULLIF(ISNULL(TotalLitigationCount, 0), 0)
            AS DECIMAL(5,2)
        ) AS FinishedRatio,
        
        CAST(
            ISNULL(TotalReviewDueDateCount, 0) * 100.0 / 
            NULLIF(ISNULL(TotalLitigationCount, 0), 0)
            AS DECIMAL(5,2)
        ) AS TotalReviewDueDateRatio
		,COUNT(*) OVER() AS TotalCount
        
    FROM ReviewerLitigationStats
    -- Show ALL reviewers, even those with no litigation cases (they'll have zero statistics)
    ORDER BY ReviewerName
    
    -- =============================================
    -- PAGINATION: Apply OFFSET and FETCH NEXT for pagination
    -- =============================================
    OFFSET @Skip ROWS
    FETCH NEXT 
        CASE 
            WHEN @Take IS NULL THEN 2147483647
            ELSE @Take 
        END ROWS ONLY
END

-----------------------------------------------------------------------------------------
-----------------------------------------------------------------------------------------

GO
/****** Object:  StoredProcedure [dbo].[GetReformersReport]    Script Date: 6/4/2025 1:57:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[GetReformersReport]
    @FromDate DATETIME = NULL,                      -- Start date for filtering
    @ToDate DATETIME = NULL,                        -- End date for filtering
    @ReformerName NVARCHAR(200) = NULL,             -- Filter by reformer name (partial match)
    @ReformerStatus INT = NULL,                     -- Filter by reformer status (Active/Inactive)
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
    SET @FromDate = ISNULL(@FromDate, '1900-01-01');
    SET @ToDate = ISNULL(@ToDate, GETDATE());

    -- =============================================
    -- CTE 1: AllReformers - Get ALL reformer citizens
    -- Purpose: Start with ALL reformers first, regardless of litigation assignment
    -- =============================================
    WITH AllReformers AS (
        SELECT DISTINCT
            C.Id AS ReformerCitizenId,
            C.FullName AS ReformerName,
            CASE WHEN C.ReformerStatusId = 1 THEN 'Active' ELSE 'Inactive' END AS ReformerStatus,
            ISNULL(CR.ExperienceYears, 0) AS ExperienceYears
        FROM Citizen C
        LEFT JOIN CitizenRequest CR ON C.Id = CR.Id
        WHERE 
            -- Get ALL citizens who are reformers (based on IsReformer flag)
            C.IsReformer = 1
            
            -- Apply reformer-specific filters
            AND (@ReformerName IS NULL OR C.FullName LIKE '%' + @ReformerName + '%')
            AND (@ReformerStatus IS NULL OR 
                 (@ReformerStatus = 1 AND C.ReformerStatusId = 1) OR 
                 (@ReformerStatus = 0 AND C.ReformerStatusId = 0))
            AND (@ExperienceYears IS NULL OR ISNULL(CR.ExperienceYears, 0) >= @ExperienceYears)
    ),
    
    -- =============================================
    -- CTE 2: ReformerLitigationStats - Get litigation statistics for each reformer
    -- Purpose: Calculate statistics for litigation requests assigned to each reformer
    -- =============================================
    ReformerLitigationStats AS (
        SELECT 
            R.ReformerCitizenId,
            R.ReformerName,
            R.ReformerStatus,
            R.ExperienceYears,
            
            -- Reformer litigation statistics
            COUNT(L.Id) AS TotalLitigationCount,
            SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
            SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
            SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 THEN 1 ELSE 0 END) AS DueDateCount
            
        FROM AllReformers R
        
        -- Left join with litigation requests where this person is the reformer
        LEFT JOIN (
            SELECT L.*,
                   ROW_NUMBER() OVER (PARTITION BY L.Id ORDER BY L.CreatedOn DESC) AS rn
            FROM LitigationRequest L
            WHERE L.ReformerCitizenId IS NOT NULL
              AND L.LitigationRequestStage = 2
              AND L.SentOn >= @FromDate 
              AND L.SentOn <= @ToDate
        ) L ON L.ReformerCitizenId = R.ReformerCitizenId AND L.rn = 1
        
        GROUP BY R.ReformerCitizenId, R.ReformerName, R.ReformerStatus, R.ExperienceYears
    )
    
    -- =============================================
    -- MAIN QUERY: Generate final reformer report with statistics
    -- =============================================
    SELECT
        ReformerName,
        ReformerStatus,
        ExperienceYears,
        
        -- Core count metrics
        ISNULL(TotalLitigationCount, 0) AS TotalLitigationCount,
        ISNULL(NotFinishedCount, 0) AS NotFinishedCount,
        ISNULL(FinishedCount, 0) AS FinishedCount,
        ISNULL(DueDateCount, 0) AS DueDateCount,
        
        -- Performance ratios (percentages)
        CAST(
            ISNULL(NotFinishedCount, 0) * 100.0 / 
            NULLIF(ISNULL(TotalLitigationCount, 0), 0)
            AS DECIMAL(5,2)
        ) AS NotFinishedRatio,
        
        CAST(
            ISNULL(FinishedCount, 0) * 100.0 / 
            NULLIF(ISNULL(TotalLitigationCount, 0), 0)
            AS DECIMAL(5,2)
        ) AS FinishedRatio,
        
        CAST(
            ISNULL(DueDateCount, 0) * 100.0 / 
            NULLIF(ISNULL(TotalLitigationCount, 0), 0)
            AS DECIMAL(5,2)
        ) AS DueDateRatio
		,COUNT(*) OVER() AS TotalCount
        
    FROM ReformerLitigationStats
    -- Show ALL reformers, even those with no litigation cases (they'll have zero statistics)
    ORDER BY ReformerName
    
    -- =============================================
    -- PAGINATION: Apply OFFSET and FETCH NEXT for pagination
    -- =============================================
    OFFSET @Skip ROWS
    FETCH NEXT 
        CASE 
            WHEN @Take IS NULL THEN 2147483647
            ELSE @Take 
        END ROWS ONLY
END

------------------------------------------------------------------------------------------
------------------------------------------------------------------------------------------

GO
/****** Object:  StoredProcedure [dbo].[GetArbitratorsReport]    Script Date: 6/4/2025 1:58:07 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
    SET @FromDate = ISNULL(@FromDate, '1900-01-01');
    SET @ToDate = ISNULL(@ToDate, GETDATE());

    -- =============================================
    -- CTE 1: AllArbitrators - Get ALL arbitrator citizens
    -- Purpose: Start with ALL arbitrators first, regardless of litigation assignment
    -- =============================================
    WITH AllArbitrators AS (
        SELECT DISTINCT
            C.Id AS ArbitratorCitizenId,
            C.FullName AS ArbitratorName,
            CASE WHEN C.ArbitratorStatusId = 1 THEN 'Active' ELSE 'Inactive' END AS ArbitratorStatus,
            ISNULL(CR.ExperienceYears, 0) AS ExperienceYears
        FROM Citizen C
        LEFT JOIN CitizenRequest CR ON C.Id = CR.Id
        WHERE 
            -- Get ALL citizens who are arbitrators (based on IsArbitrator flag)
            C.IsArbitrator = 1
            
            -- Apply arbitrator-specific filters
            AND (@EmployeeIds IS NULL OR C.Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')))
            AND (@ArbitratorName IS NULL OR C.FullName LIKE '%' + @ArbitratorName + '%')
            AND (@ArbitratorStatus IS NULL OR 
                 (@ArbitratorStatus = '1' AND C.ArbitratorStatusId = 1) OR 
                 (@ArbitratorStatus = '0' AND C.ArbitratorStatusId = 0))
            AND (@ExperienceYears IS NULL OR ISNULL(CR.ExperienceYears, 0) >= @ExperienceYears)
    ),
    
    -- =============================================
    -- CTE 2: ArbitratorLitigationStats - Get litigation statistics for each arbitrator
    -- Purpose: Calculate statistics for litigation requests assigned to each arbitrator
    -- =============================================
    ArbitratorLitigationStats AS (
        SELECT 
            A.ArbitratorCitizenId,
            A.ArbitratorName,
            A.ArbitratorStatus,
            A.ExperienceYears,
            
            -- Chairman litigation statistics (where arbitrator is the chairman)
            COUNT(CASE WHEN L.ArbitratorCitizenId = A.ArbitratorCitizenId THEN 1 END) AS ChairmanTotalCount,
            SUM(CASE WHEN L.ArbitratorCitizenId = A.ArbitratorCitizenId AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS ChairmanNotFinishedCount,
            SUM(CASE WHEN L.ArbitratorCitizenId = A.ArbitratorCitizenId AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS ChairmanFinishedCount,
            SUM(CASE WHEN L.ArbitratorCitizenId = A.ArbitratorCitizenId AND L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 THEN 1 ELSE 0 END) AS ChairmanDueDateCount,
            
            -- Regular arbitrator statistics (where arbitrator is a regular arbitrator)
            COUNT(CASE WHEN LRA.ArbitratorCitizenId = A.ArbitratorCitizenId THEN 1 END) AS ArbitratorTotalCount,
            SUM(CASE WHEN LRA.ArbitratorCitizenId = A.ArbitratorCitizenId AND L2.IsFinished = 0 THEN 1 ELSE 0 END) AS ArbitratorNotFinishedCount,
            SUM(CASE WHEN LRA.ArbitratorCitizenId = A.ArbitratorCitizenId AND L2.IsFinished = 1 THEN 1 ELSE 0 END) AS ArbitratorFinishedCount,
            SUM(CASE WHEN LRA.ArbitratorCitizenId = A.ArbitratorCitizenId AND L2.LitigationRequestStage = 3 AND L2.ArbitrationPeriod = 0 THEN 1 ELSE 0 END) AS ArbitratorDueDateCount
            
        FROM AllArbitrators A
        
        -- Left join with litigation requests where this person is chairman
        LEFT JOIN (
            SELECT L.*,
                   ROW_NUMBER() OVER (PARTITION BY L.Id ORDER BY L.CreatedOn DESC) AS rn
            FROM LitigationRequest L
            WHERE L.ArbitratorCitizenId IS NOT NULL
              AND L.LitigationRequestStage = 3
              AND L.SentOn >= @FromDate 
              AND L.SentOn <= @ToDate
        ) L ON L.ArbitratorCitizenId = A.ArbitratorCitizenId AND L.rn = 1
        
        -- Left join with litigation requests where this person is a regular arbitrator
        LEFT JOIN LitigationRequestArbitrator LRA ON LRA.ArbitratorCitizenId = A.ArbitratorCitizenId
        LEFT JOIN LitigationRequest L2 ON L2.Id = LRA.LitigationRequestId 
                                       AND L2.LitigationRequestStage = 3
                                       AND L2.SentOn >= @FromDate 
                                       AND L2.SentOn <= @ToDate
        
        GROUP BY A.ArbitratorCitizenId, A.ArbitratorName, A.ArbitratorStatus, A.ExperienceYears
    )
    
    -- =============================================
    -- MAIN QUERY: Generate final arbitrator report with combined statistics
    -- =============================================
    SELECT
        ArbitratorName,
        ArbitratorStatus,
        ExperienceYears,
        
        -- Combined totals (chairman + regular arbitrator roles)
        (ChairmanTotalCount + ArbitratorTotalCount) AS TotalLitigationCount,
        (ChairmanNotFinishedCount + ArbitratorNotFinishedCount) AS NotFinishedCount,
        (ChairmanFinishedCount + ArbitratorFinishedCount) AS FinishedCount,
        (ChairmanDueDateCount + ArbitratorDueDateCount) AS DueDateCount,
        
        -- Separate statistics for chairman role
        ChairmanTotalCount,
        ChairmanNotFinishedCount,
        ChairmanFinishedCount,
        ChairmanDueDateCount,
        
        -- Separate statistics for regular arbitrator role
        ArbitratorTotalCount,
        ArbitratorNotFinishedCount,
        ArbitratorFinishedCount,
        ArbitratorDueDateCount,
        
        -- Performance ratios for combined statistics
        CAST(
            (ChairmanNotFinishedCount + ArbitratorNotFinishedCount) * 100.0 / 
            NULLIF((ChairmanTotalCount + ArbitratorTotalCount), 0)
            AS DECIMAL(5,2)
        ) AS NotFinishedRatio,
        
        CAST(
            (ChairmanFinishedCount + ArbitratorFinishedCount) * 100.0 / 
            NULLIF((ChairmanTotalCount + ArbitratorTotalCount), 0)
            AS DECIMAL(5,2)
        ) AS FinishedRatio,
        
        CAST(
            (ChairmanDueDateCount + ArbitratorDueDateCount) * 100.0 / 
            NULLIF((ChairmanTotalCount + ArbitratorTotalCount), 0)
            AS DECIMAL(5,2)
        ) AS DueDateRatio
		,COUNT(*) OVER() AS TotalCount
        
    FROM ArbitratorLitigationStats
    -- Show ALL arbitrators, even those with no litigation cases (they'll have zero statistics)
    ORDER BY ArbitratorName
    
    -- =============================================
    -- PAGINATION: Apply OFFSET and FETCH NEXT for pagination
    -- =============================================
    OFFSET @Skip ROWS
    FETCH NEXT 
        CASE 
            WHEN @Take IS NULL THEN 2147483647
            ELSE @Take 
        END ROWS ONLY
END