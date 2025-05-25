
            --تقرير قائمة الدعاوى في مرحلة التدقيق

Alter PROCEDURE [dbo].[GetListOfLitigationReviewReport]
    @RequestCode NVARCHAR(20) = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL,
    @AreaIds NVARCHAR(MAX) = NULL,
    @CitiesIds NVARCHAR(MAX) = NULL,
    @LitigationRequestCategoryIds NVARCHAR(MAX) = NULL,
    @LitigationRequestStatusIds NVARCHAR(MAX) = NULL,
    @LitigationRequestAmountId NVARCHAR(MAX) = NULL, -- Changed to UNIQUEIDENTIFIER
    @EmployeeIds NVARCHAR(MAX) = NULL,
    @SortDirection NVARCHAR(4) = NULL, -- 'ASC' or 'DESC'
    @Skip INT = NULL,
    @Take INT = NULL
AS
BEGIN
    --SET NOCOUNT ON;

    -- Handle default skip if @SKIP is NULL
    SET @Skip = ISNULL(@Skip, 0);

    -- Handle default take if @TAKE is NULL
    SET @Take = ISNULL(@Take, 10);

    -- Basic validation for @SortDirection to prevent injection
    IF @SortDirection IS NULL OR (@SortDirection <> 'ASC' AND @SortDirection <> 'DESC')
    BEGIN
        SET @SortDirection = 'DESC'; -- Default to DESC if invalid or null
    END

    -- Handle default dates if @FromDate is NULL
    SET @FromDate = ISNULL(@FromDate, '1970-01-01 00:00:00.000');

    -- Handle default dates if @ToDate is NULL
    SET @ToDate = ISNULL(@ToDate, GETDATE());

    

    -- ====================================================================
    -- First RESULT SET: PAGINATED DATA
    -- This is your original query with pagination.
    -- ====================================================================
    SELECT
        slra.LitigationAmount,
        L.CreatedOn AS CreatedOn,
        L.RequestCode AS RequestCode,
        L.ContractAddress AS ContractAddress,
        A.AreaName AS AreaName,
        C.CityName AS CityName,
        Slrc.CategoryName,
        E.EmployeeName AS EmployeeName,
        S.RequestStatus AS RequestStatus,
        CASE
            WHEN F.ArabicFacilityName IS NOT NULL THEN F.ArabicFacilityName
            ELSE Cit.FullName
        END AS FirstParty,
        CASE
            WHEN SF.ArabicFacilityName IS NOT NULL THEN SF.ArabicFacilityName
            ELSE SecCit.FullName
        END AS SecondParty
    FROM
        LitigationRequestContract LC
    INNER JOIN LitigationRequest L ON L.ContractId = LC.Id
    -- Only First Workflow Processing Log
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
    INNER JOIN SysLitigationRequestStatus S ON S.Id = L.ExternalStatusId
    LEFT JOIN SysEmployee E ON W.EmployeeId = E.Id
    LEFT JOIN Facility F ON F.Id = LC.FirstPartyFacilityId
    LEFT JOIN Facility SF ON SF.Id = LC.SecondPartyFacilityId
    LEFT JOIN Citizen Cit ON Cit.Id = LC.FirstPartyCitizenId
    LEFT JOIN Citizen SecCit ON SecCit.Id = LC.SecondPartyCitizenId
    LEFT JOIN SysArea A ON L.AreaId = A.Id
    LEFT JOIN SysCity C ON L.CityId = C.Id -- Corrected join for CityId
    LEFT JOIN SysLitigationRequestCategory Slrc ON Slrc.Id = L.LitigationRequestCategoryId
    INNER JOIN SysLitigationRequestAmount slra ON slra.Id = L.LitigationRequestAmountId
    WHERE
        (L.LitigationRequestStage = 1) -- مرحلة التدقيق
        AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
        AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
        AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
        AND (@LitigationRequestStatusIds IS NULL OR L.ExternalStatusId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestStatusIds, ',')))
        AND (L.SentOn >= @FromDate)
        AND (L.SentOn <= @ToDate)
        AND (@LitigationRequestAmountId IS NULL OR L.LitigationRequestAmountId = @LitigationRequestAmountId) -- Comparison now between UNIQUEIDENTIFIER types
        AND (@RequestCode IS NULL OR L.RequestCode LIKE @RequestCode)
        AND (@EmployeeIds IS NULL OR E.Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')))
    ORDER BY
        CASE WHEN @SortDirection = 'ASC' THEN L.CreatedOn END ASC,
        CASE WHEN @SortDirection = 'DESC' THEN L.CreatedOn END DESC
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY;


	-- ====================================================================
    -- Second RESULT SET: TOTAL COUNT
    -- This query calculates the total number of records matching the criteria
    -- before any pagination (OFFSET/FETCH) is applied.
    -- ====================================================================
    SELECT COUNT(L.Id) AS TotalCount
    FROM LitigationRequestContract LC
    INNER JOIN LitigationRequest L ON L.ContractId = LC.Id
    -- Only First Workflow Processing Log
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
    INNER JOIN SysLitigationRequestStatus S ON S.Id = L.ExternalStatusId
    LEFT JOIN SysEmployee E ON W.EmployeeId = E.Id
    LEFT JOIN Facility F ON F.Id = LC.FirstPartyFacilityId
    LEFT JOIN Facility SF ON SF.Id = LC.SecondPartyFacilityId
    LEFT JOIN Citizen Cit ON Cit.Id = LC.FirstPartyCitizenId
    LEFT JOIN Citizen SecCit ON SecCit.Id = LC.SecondPartyCitizenId
    LEFT JOIN SysArea A ON L.AreaId = A.Id
    LEFT JOIN SysCity C ON L.CityId = C.Id
    LEFT JOIN SysLitigationRequestCategory Slrc ON Slrc.Id = L.LitigationRequestCategoryId
    INNER JOIN SysLitigationRequestAmount slra ON slra.Id = L.LitigationRequestAmountId
    WHERE
        (L.LitigationRequestStage = 1) -- مرحلة التدقيق
        AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
        AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
        AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
        AND (@LitigationRequestStatusIds IS NULL OR L.ExternalStatusId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestStatusIds, ',')))
        AND (L.SentOn >= @FromDate)
        AND (L.SentOn <= @ToDate)
        AND (@LitigationRequestAmountId IS NULL OR L.LitigationRequestAmountId = @LitigationRequestAmountId) 
        AND (@RequestCode IS NULL OR L.RequestCode LIKE @RequestCode)
        AND (@EmployeeIds IS NULL OR E.Id IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ',')));
END;


------------------------------------------------------------------

Alter PROCEDURE [dbo].[GetLitigationStatisticsReviewReport] 
    @EmployeeIds NVARCHAR(MAX) = NULL,
    @LitigationRequestStageId INT = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL,
    @AreaIds NVARCHAR(MAX) = NULL,
    @CitiesIds NVARCHAR(MAX) = NULL,
	@LitigationRequestAmountId NVARCHAR(MAX) = NULL,
    @LitigationRequestCategoryIds NVARCHAR(MAX) = NULL

AS
BEGIN
    SET NOCOUNT ON;
    SET @FromDate = ISNULL(@FromDate, '1900-01-01');
    SET @ToDate = ISNULL(@ToDate, GETDATE());

    ;WITH BaseLitigations AS (
        SELECT 
            L.Id,
            L.IsFinished,
            L.LitigationRequestStage,
            L.ReconciliationPeriod,
            L.ArbitrationPeriod,
            L.StartReconciliationDate,
            L.StartArbitrationDate,
            L.SentOn,
            L.CreatedOn,
            L.AreaId,
            L.CityId,
            L.LitigationRequestCategoryId,
            L.Fees,
            L.ContractId,
			L.IsReminderForReviewerManagerSent,
			L.IsReminderForReviewerSent
        FROM LitigationRequest L
              WHERE 
            (
                @EmployeeIds IS NULL OR 
                EXISTS (
                    SELECT 1 FROM WorkFlowProcessingLog WF 
                    WHERE WF.RequestId = L.Id AND WF.EmployeeId IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ','))
                )
            )
            AND (@LitigationRequestStageId IS NULL OR L.LitigationRequestStage = @LitigationRequestStageId)
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
		  AND (@LitigationRequestAmountId IS NULL OR L.LitigationRequestAmountId = @LitigationRequestAmountId)
          AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
    )
    SELECT
        COUNT(1) AS TotalLitigationCount,
        SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
        SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
        SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateReconciliationCount,
        SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateArbitrationCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) AS RealEstateBrokerageCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) AS WafiCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) AS OtherServicesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) AS RentCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) AS PropertiesCount,
        -- Ratios
        CAST(SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS NotFinishedRatio,
        CAST(SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS FinishedRatio,
		 CAST(SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END)* 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS DueDateReconciliationRatio,
        CAST(SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END)* 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS DueDateArbitrationRatio,
        CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS RealEstateBrokerageRatio,
        CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS WafiRatio,
        CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS OtherServicesRatio,
        CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS RentRatio,
        CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)) AS PropertiesRatio,
        -- Merged Review DueDate count
        SUM(CASE 
            WHEN L.LitigationRequestStage = 1 
             AND L.IsReminderForReviewerManagerSent = 1 
             AND L.IsReminderForReviewerSent = 1 
             AND L.CreatedOn >= @FromDate 
             AND L.CreatedOn <= @ToDate 
            THEN 1 ELSE 0 
        END) AS TotalReviewDueDateCount,
		-- Merged Review DueDate ratio
		CAST(
    SUM(CASE 
        WHEN L.LitigationRequestStage = 1 
         AND L.IsReminderForReviewerManagerSent = 1 
         AND L.IsReminderForReviewerSent = 1 
         AND L.CreatedOn >= @FromDate 
         AND L.CreatedOn <= @ToDate 
        THEN 1 ELSE 0 
    END) * 100.0 / NULLIF(COUNT(1),0) 
AS DECIMAL(5,2)) AS TotalReviewDueDateRatio

    FROM BaseLitigations L
    LEFT JOIN LitigationRequestContract C ON C.Id = L.ContractId;
END
GO