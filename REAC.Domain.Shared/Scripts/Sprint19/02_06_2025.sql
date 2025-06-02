GO
/****** Object:  StoredProcedure [dbo].[GetReviewersReformersArbitratorsReport]    Script Date: 6/1/2025 4:02:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- تقرير المدقق والمصلح والمحكم
create PROCEDURE [dbo].[GetReviewersReformersArbitratorsReport]
    @PersonnelTypeId NVARCHAR(MAX) = NULL,             -- Filter by type: 'Reviewer ==> 1', 'Reformer ==> 2', 'Arbitrator ==> 3', or NULL for all
    @EmployeeIds NVARCHAR(MAX) = NULL,              -- Filter by specific employee/citizen IDs
    @ReformersIds NVARCHAR(MAX) = NULL,              -- Filter by specific employee/citizen IDs
    @ArbitratorsIds NVARCHAR(MAX) = NULL,              -- Filter by specific employee/citizen IDs
    @PersonnelStatusId NVARCHAR(50) = NULL,           -- Filter by personnel status
    @ExperienceYears INT = NULL,                    -- Filter by years of experience (Reformers/Arbitrators only)
    @ArbitratorClassIds NVARCHAR(MAX) = NULL,                    -- Filter by @ArbitratorClass (Arbitrators only)
    @Skip INT = 0,                                  -- Number of records to skip (for pagination)
    @Take INT = NULL                                -- Number of records to take (NULL = all records)
AS
BEGIN
    -- Prevent row count messages from being returned to improve performance
    SET NOCOUNT ON;
    
    -- Validate pagination parameters
    SET @Skip = ISNULL(@Skip, 0);
    IF @Skip < 0 SET @Skip = 0;
    

    -- =============================================
    -- CTE 1: AllReviewers - Get ALL reviewer employees
    -- =============================================
    
	WITH AllReviewers AS (
        SELECT DISTINCT
            E.Id AS PersonnelId,
            E.EmployeeName AS PersonnelName,
            N'مدقق' AS PersonnelType,
			1 As PersonnelTypeId,
            CASE WHEN E.IsActive = 1 THEN N'معتمد' ELSE N'موقوف' END AS PersonnelStatus,
			0 AS ExperienceYears -- Reviewers don't have experience years in the original logic
        FROM SysEmployee E
        INNER JOIN SysDepartment D ON E.DepartmentId = D.Id
        WHERE 
            -- Get ALL employees from departments with DepartmentType = 1 (reviewer departments)
            D.DepartmentType = 1
            
            -- Apply reviewer-specific filters
            AND (@EmployeeIds IS NULL OR E.Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')))
            AND (@PersonnelStatusId IS NULL OR E.IsActive IN  (SELECT value FROM STRING_SPLIT(@PersonnelStatusId, ',')))
                 
    ),


    -- =============================================
    -- CTE 2: AllReformers - Get ALL reformer citizens
    -- =============================================
   
    
	 AllReformers AS (
        SELECT DISTINCT
            C.Id AS PersonnelId,
            C.FullName AS PersonnelName,
            N'مصلح' AS PersonnelType,
			2 As PersonnelTypeId,
            CASE WHEN c.ReformerStatusId = 1 THEN N'معتمد' ELSE N'موقوف' END AS PersonnelStatus,
			0 AS ExperienceYears -- Reviewers don't have experience years in the original logic
        FROM Citizen C
        LEFT JOIN CitizenRequest CR ON C.Id = CR.Id
        WHERE 
            -- Get ALL citizens who are reformers (based on IsReformer flag)
            C.IsReformer = 1
            
            -- Apply reformer-specific filters
            AND (@ReformersIds IS NULL OR C.Id IN (SELECT value FROM STRING_SPLIT(@ReformersIds, ',')))
            AND (@PersonnelStatusId IS NULL OR C.ReformerStatusId IN  (SELECT value FROM STRING_SPLIT(@PersonnelStatusId, ',')))
                 
            AND (@ExperienceYears IS NULL OR ISNULL(CR.ExperienceYears, 0) >= @ExperienceYears)
    ),


    -- =============================================
    -- CTE 3: AllArbitrators - Get ALL arbitrator citizens
    -- =============================================
   
	 AllArbitrators AS (
        SELECT DISTINCT
            C.Id AS PersonnelId,
            C.FullName AS PersonnelName,
		    N'محكم' AS PersonnelType,
			3 As PersonnelTypeId,
            CASE WHEN C.ArbitratorStatusId = 1 THEN N'معتمد' ELSE N'موقوف' END AS ArbitratorStatus,
            ISNULL(CR.ExperienceYears, 0) AS ExperienceYears

        FROM Citizen C
        LEFT JOIN CitizenRequest CR ON C.Id = CR.Id
		LEFT JOIN SysArbitratorClass AC ON CR.ArbitratorClassId = AC.Id
        WHERE 
            -- Get ALL citizens who are arbitrators (based on IsArbitrator flag)
            C.IsArbitrator = 1
            
            -- Apply arbitrator-specific filters
            AND (@ArbitratorsIds IS NULL OR C.Id IN (SELECT value FROM STRING_SPLIT(@ArbitratorsIds, ',')))
            AND (@PersonnelStatusId IS NULL OR C.ArbitratorStatusId IN  (SELECT value FROM STRING_SPLIT(@PersonnelStatusId, ',')))
            AND (@ArbitratorClassIds IS NULL OR AC.Id IN  (SELECT value FROM STRING_SPLIT(@ArbitratorClassIds, ',')))
            AND (@ExperienceYears IS NULL OR ISNULL(CR.ExperienceYears, 0) >= @ExperienceYears)

    ),
    
    -- =============================================
    -- CTE 4: AllPersonnel - Union all personnel types
    -- =============================================
    AllPersonnel AS (
        SELECT * FROM AllReviewers
        UNION ALL
        SELECT * FROM AllReformers
        UNION ALL
        SELECT * FROM AllArbitrators
    ),
    
    -- =============================================
    -- CTE 5: ReviewerStats - Calculate litigation statistics for reviewers
    -- =============================================
    ReviewerStats AS (
        SELECT 
            R.PersonnelId,
            R.PersonnelName,
            R.PersonnelType,
			R.PersonnelTypeId,
            R.PersonnelStatus,
            R.ExperienceYears,
            
            COUNT(L.Id) AS TotalLitigationCount,
            SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
            SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
            SUM(CASE 
                WHEN L.LitigationRequestStage = 1                     
                     AND L.IsReminderForReviewerManagerSent = 1       
                     AND L.IsReminderForReviewerSent = 1              
                     --AND L.CreatedOn >= @FromDate                     
                     --AND L.CreatedOn <= @ToDate                       
                THEN 1 ELSE 0 
            END) AS DueDateCount,
            
            -- Additional fields for arbitrators (will be 0 for reviewers)
            0 AS ChairmanTotalCount,
            0 AS ChairmanNotFinishedCount,
            0 AS ChairmanFinishedCount,
            0 AS ChairmanDueDateCount,
            0 AS ArbitratorTotalCount,
            0 AS ArbitratorNotFinishedCount,
            0 AS ArbitratorFinishedCount,
            0 AS ArbitratorDueDateCount
            
        FROM AllPersonnel R
        LEFT JOIN (
            SELECT WFP.*,
                   ROW_NUMBER() OVER (PARTITION BY WFP.RequestId ORDER BY WFP.CreatedOn DESC) AS rn
            FROM WorkFlowProcessingLog WFP
            WHERE WFP.EmployeeId IS NOT NULL
        ) W ON W.EmployeeId = R.PersonnelId AND W.rn = 1 AND R.PersonnelType = N'مدقق'
        LEFT JOIN LitigationRequest L ON L.Id = W.RequestId
                                      AND L.LitigationRequestStage = 1
                                      --AND L.SentOn >= @FromDate 
                                      --AND L.SentOn <= @ToDate
                                      AND R.PersonnelType = N'مدقق'
        WHERE R.PersonnelType = N'مدقق'
        GROUP BY R.PersonnelId, R.PersonnelName, R.PersonnelType, R.PersonnelStatus, R.ExperienceYears, R.PersonnelTypeId
    ),
    
    -- =============================================
    -- CTE 6: ReformerStats - Calculate litigation statistics for reformers
    -- =============================================
    ReformerStats AS (
        SELECT 
            R.PersonnelId,
            R.PersonnelName,
            R.PersonnelType,
			R.PersonnelTypeId,
            R.PersonnelStatus,
            R.ExperienceYears,
            
            COUNT(L.Id) AS TotalLitigationCount,
            SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
            SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
            SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 THEN 1 ELSE 0 END) AS DueDateCount,
            
            -- Additional fields for arbitrators (will be 0 for reformers)
            0 AS ChairmanTotalCount,
            0 AS ChairmanNotFinishedCount,
            0 AS ChairmanFinishedCount,
            0 AS ChairmanDueDateCount,
            0 AS ArbitratorTotalCount,
            0 AS ArbitratorNotFinishedCount,
            0 AS ArbitratorFinishedCount,
            0 AS ArbitratorDueDateCount
            
        FROM AllPersonnel R
        LEFT JOIN (
            SELECT L.*,
                   ROW_NUMBER() OVER (PARTITION BY L.Id ORDER BY L.CreatedOn DESC) AS rn
            FROM LitigationRequest L
            WHERE L.ReformerCitizenId IS NOT NULL
              AND L.LitigationRequestStage = 2
        ) L ON L.ReformerCitizenId = R.PersonnelId AND L.rn = 1 AND R.PersonnelType = N'مصلح'
        WHERE R.PersonnelType = N'مصلح'
        GROUP BY R.PersonnelId, R.PersonnelName, R.PersonnelType, R.PersonnelStatus, R.ExperienceYears , R.PersonnelTypeId
    ),
    
    -- =============================================
    -- CTE 7: ArbitratorStats - Calculate litigation statistics for arbitrators
    -- =============================================
   
    ArbitratorStats AS (
        SELECT 
            A.PersonnelId,
            A.PersonnelName,
            A.PersonnelType,
			A.PersonnelTypeId,
            A.ArbitratorStatus,
            A.ExperienceYears,
            
                    -- Combined totals (chairman + regular arbitrator roles)
            (COUNT(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId THEN 1 END) + 
             COUNT(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId THEN 1 END)) AS TotalLitigationCount,
            (SUM(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId AND L.IsFinished = 0 THEN 1 ELSE 0 END) + 
             SUM(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId AND L2.IsFinished = 0 THEN 1 ELSE 0 END)) AS NotFinishedCount,
            (SUM(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId AND L.IsFinished = 1 THEN 1 ELSE 0 END) + 
             SUM(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId AND L2.IsFinished = 1 THEN 1 ELSE 0 END)) AS FinishedCount,
            (SUM(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId AND L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 THEN 1 ELSE 0 END) + 
             SUM(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId AND L2.LitigationRequestStage = 3 AND L2.ArbitrationPeriod = 0 THEN 1 ELSE 0 END)) AS DueDateCount,
            
            -- Separate statistics for chairman role
            COUNT(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId THEN 1 END) AS ChairmanTotalCount,
            SUM(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS ChairmanNotFinishedCount,
            SUM(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS ChairmanFinishedCount,
            SUM(CASE WHEN L.ArbitratorCitizenId = A.PersonnelId AND L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 THEN 1 ELSE 0 END) AS ChairmanDueDateCount,
            
            -- Separate statistics for regular arbitrator role
            COUNT(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId THEN 1 END) AS ArbitratorTotalCount,
            SUM(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId AND L2.IsFinished = 0 THEN 1 ELSE 0 END) AS ArbitratorNotFinishedCount,
            SUM(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId AND L2.IsFinished = 1 THEN 1 ELSE 0 END) AS ArbitratorFinishedCount,
            SUM(CASE WHEN LRA.ArbitratorCitizenId = A.PersonnelId AND L2.LitigationRequestStage = 3 AND L2.ArbitrationPeriod = 0 THEN 1 ELSE 0 END) AS ArbitratorDueDateCount
                        
        FROM AllArbitrators A
        
        -- Left join with litigation requests where this person is chairman
        LEFT JOIN (
            SELECT L.*,
                   ROW_NUMBER() OVER (PARTITION BY L.Id ORDER BY L.CreatedOn DESC) AS rn
            FROM LitigationRequest L
            WHERE L.ArbitratorCitizenId IS NOT NULL
              AND L.LitigationRequestStage = 3
        ) L ON L.ArbitratorCitizenId = A.PersonnelId AND L.rn = 1
        
        -- Left join with litigation requests where this person is a regular arbitrator
        LEFT JOIN LitigationRequestArbitrator LRA ON LRA.ArbitratorCitizenId = A.PersonnelId
        LEFT JOIN LitigationRequest L2 ON L2.Id = LRA.LitigationRequestId 
                                       AND L2.LitigationRequestStage = 3
        
        GROUP BY A.PersonnelId, A.PersonnelName, A.ArbitratorStatus, A.ExperienceYears,A.PersonnelType, A.PersonnelTypeId
    ),
    -- =============================================
    -- CTE 8: UnifiedStats - Union all statistics
    -- =============================================
    UnifiedStats AS (
        SELECT * FROM ReviewerStats
        UNION ALL
        SELECT * FROM ReformerStats
        UNION ALL
        SELECT * FROM ArbitratorStats
        UNION ALL
        -- Include personnel with no statistics (zero counts)
        SELECT 
            AP.PersonnelId,
            AP.PersonnelName,
            AP.PersonnelType,
			AP.PersonnelTypeId,
            AP.PersonnelStatus,
            AP.ExperienceYears,
            0 AS TotalLitigationCount,
            0 AS NotFinishedCount,
            0 AS FinishedCount,
            0 AS DueDateCount,
            0 AS ChairmanTotalCount,
            0 AS ChairmanNotFinishedCount,
            0 AS ChairmanFinishedCount,
            0 AS ChairmanDueDateCount,
            0 AS ArbitratorTotalCount,
            0 AS ArbitratorNotFinishedCount,
            0 AS ArbitratorFinishedCount,
            0 AS ArbitratorDueDateCount
        FROM AllPersonnel AP
        WHERE NOT EXISTS (
            SELECT 1 FROM ReviewerStats RS WHERE RS.PersonnelId = AP.PersonnelId
            UNION
            SELECT 1 FROM ReformerStats RF WHERE RF.PersonnelId = AP.PersonnelId
            UNION
            SELECT 1 FROM ArbitratorStats AR WHERE AR.PersonnelId = AP.PersonnelId
        )

    )
    
    -- =============================================
    -- MAIN QUERY: Generate final unified personnel report
    -- =============================================
    SELECT
        PersonnelName,
        PersonnelType,
		PersonnelTypeId,
        PersonnelStatus,
        ExperienceYears,
        
        -- Core count metrics
        ISNULL(TotalLitigationCount, 0) AS TotalLitigationCount,
        ISNULL(NotFinishedCount, 0) AS NotFinishedCount,
        ISNULL(FinishedCount, 0) AS FinishedCount,
        ISNULL(DueDateCount, 0) AS DueDateCount,
        
        -- Arbitrator-specific metrics (will be 0 for non-arbitrators)
        ISNULL(ChairmanTotalCount, 0) AS ChairmanTotalCount,
        ISNULL(ChairmanNotFinishedCount, 0) AS ChairmanNotFinishedCount,
        ISNULL(ChairmanFinishedCount, 0) AS ChairmanFinishedCount,
        ISNULL(ChairmanDueDateCount, 0) AS ChairmanDueDateCount,
        ISNULL(ArbitratorTotalCount, 0) AS ArbitratorTotalCount,
        ISNULL(ArbitratorNotFinishedCount, 0) AS ArbitratorNotFinishedCount,
        ISNULL(ArbitratorFinishedCount, 0) AS ArbitratorFinishedCount,
        ISNULL(ArbitratorDueDateCount, 0) AS ArbitratorDueDateCount,
        
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
        ,
		COUNT(*) OVER() AS TotalCount
    FROM UnifiedStats
	Where 
	(@PersonnelTypeId IS NULL OR PersonnelTypeId in (SELECT value FROM STRING_SPLIT(@PersonnelTypeId, ',')))

    ORDER BY PersonnelType, PersonnelName
    
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
