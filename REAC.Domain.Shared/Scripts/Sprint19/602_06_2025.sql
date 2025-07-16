

create PROCEDURE [dbo].[GetReviewDashbourdReport] 
    @EmployeeIds NVARCHAR(MAX) = NULL,
    @LitigationRequestStageId INT = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
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
			  (L.LitigationRequestStage = 1)
	AND (
    @EmployeeIds IS NULL
    OR (
        (L.LitigationRequestStage = 1 AND EXISTS (
            SELECT 1 
            FROM WorkFlowProcessingLog WF 
            WHERE WF.RequestId = L.Id 
              AND WF.EmployeeId IN (SELECT value FROM STRING_SPLIT(@EmployeeIds, ','))
        ))
       
    )
)
             AND(@LitigationRequestStageId IS NULL OR L.LitigationRequestStage = @LitigationRequestStageId)
              AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
    )
    SELECT
        COUNT(1) AS TotalLitigationCount,
        SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
        SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
        SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateReconciliationCount,
        SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateArbitrationCount,
        
        -- Real Estate Brokerage (66) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) AS RealEstateBrokerageCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS RealEstateBrokerageFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS RealEstateBrokerageNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.LitigationRequestStage = 1 
             AND L.IsReminderForReviewerManagerSent = 1 
             AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) AS RealEstateBrokerageDueDateCount,
        
        -- Wafi (87) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) AS WafiCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS WafiFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS WafiNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.LitigationRequestStage = 1 
             AND L.IsReminderForReviewerManagerSent = 1 
             AND L.IsReminderForReviewerSent = 1)  THEN 1 ELSE 0 END) AS WafiDueDateCount,
        
        -- Other Services (78) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) AS OtherServicesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS OtherServicesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS OtherServicesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.LitigationRequestStage = 1 AND L.IsReminderForReviewerManagerSent = 1 AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) AS OtherServicesDueDateCount,
        
        -- Rent (82) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) AS RentCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS RentFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS RentNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.LitigationRequestStage = 1 
             AND L.IsReminderForReviewerManagerSent = 1 
             AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) AS RentDueDateCount,
        
        -- Properties (79) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) AS PropertiesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS PropertiesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS PropertiesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.LitigationRequestStage = 1 
             AND L.IsReminderForReviewerManagerSent = 1 
             AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) AS PropertiesDueDateCount,
        
        -- Overall Ratios
         ISNULL(CAST(SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS NotFinishedRatio,
         ISNULL(CAST(SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS FinishedRatio,
		 ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END)* 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS DueDateReconciliationRatio,
         ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END)* 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS DueDateArbitrationRatio,
        
        -- Real Estate Brokerage Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS RealEstateBrokerageRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS RealEstateBrokerageFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS RealEstateBrokerageNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.LitigationRequestStage = 1 
             AND L.IsReminderForReviewerManagerSent = 1 
             AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS RealEstateBrokerageDueDateRatio,
        
        -- Wafi Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS WafiRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS WafiFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS WafiNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.LitigationRequestStage = 1 AND L.IsReminderForReviewerManagerSent = 1 AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS WafiDueDateRatio,
        
        -- Other Services Ratios
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS OtherServicesRatio,
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS OtherServicesFinishedRatio,
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS OtherServicesNotFinishedRatio,
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.LitigationRequestStage = 1 AND L.IsReminderForReviewerManagerSent = 1 AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS OtherServicesDueDateRatio,
        
        -- Rent Ratios
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS RentRatio,
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS RentFinishedRatio,
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS RentNotFinishedRatio,
         ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.LitigationRequestStage = 1 AND L.IsReminderForReviewerManagerSent = 1 AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS RentDueDateRatio,
        
        -- Properties Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1),0) AS DECIMAL(5,2)),0) AS PropertiesRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS PropertiesFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS PropertiesNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.LitigationRequestStage = 1 AND L.IsReminderForReviewerManagerSent = 1 AND L.IsReminderForReviewerSent = 1) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END),0) AS DECIMAL(5,2)),0) AS PropertiesDueDateRatio,
        
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

-------------------------------
GO
CREATE PROCEDURE [dbo].[GetReconciliationDashbourdReport] 
    @ReformersId UNIQUEIDENTIFIER = NULL,  -- Changed from INT to UNIQUEIDENTIFIER
    @LitigationRequestStageId INT = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
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
            L.IsReminderForReviewerSent,
            L.ReformerCitizenId
        FROM LitigationRequest L
        WHERE
            (L.LitigationRequestStage = 2)
            AND (
                @ReformersId IS NULL
                OR L.ReformerCitizenId = @ReformersId  -- This comparison now works correctly
            )
            AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
    )
    SELECT
        COUNT(1) AS TotalLitigationCount,
        SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
        SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
        SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateReconciliationCount,
        SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateArbitrationCount,
        
        -- Real Estate Brokerage (66) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) AS RealEstateBrokerageCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS RealEstateBrokerageFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS RealEstateBrokerageNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS RealEstateBrokerageDueDateCount,
        
        -- Wafi (87) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) AS WafiCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS WafiFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS WafiNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS WafiDueDateCount,
        
        -- Other Services (78) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) AS OtherServicesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS OtherServicesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS OtherServicesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS OtherServicesDueDateCount,
        
        -- Rent (82) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) AS RentCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS RentFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS RentNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS RentDueDateCount,
        
        -- Properties (79) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) AS PropertiesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS PropertiesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS PropertiesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS PropertiesDueDateCount,
        
        -- Overall Ratios
        ISNULL(CAST(SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS NotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS FinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS DueDateReconciliationRatio,
        ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS DueDateArbitrationRatio,
        
        -- Real Estate Brokerage Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageDueDateRatio,
        
        -- Wafi Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS WafiRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiDueDateRatio,
        
        -- Other Services Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS OtherServicesRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesDueDateRatio,
        
        -- Rent Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS RentRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentDueDateRatio,
        
        -- Properties Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS PropertiesRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesDueDateRatio,
        
        -- Merged Review DueDate count (now for Reformer stage)
        SUM(CASE 
            WHEN L.LitigationRequestStage = 2 
             AND L.ReformerCitizenId = @ReformersId  -- Fixed: Now correctly compares GUID with GUID
             AND L.CreatedOn >= @FromDate 
             AND L.CreatedOn <= @ToDate 
            THEN 1 ELSE 0 
        END) AS TotalReviewDueDateCount,
        
        -- Merged Review DueDate ratio (now for Reformer stage)
        CAST(
            SUM(CASE 
                WHEN L.LitigationRequestStage = 2 
                 AND L.ReformerCitizenId = @ReformersId  -- Fixed: Now correctly compares GUID with GUID
                 AND L.CreatedOn >= @FromDate 
                 AND L.CreatedOn <= @ToDate 
                THEN 1 ELSE 0 
            END) * 100.0 / NULLIF(COUNT(1), 0) 
        AS DECIMAL(5,2)) AS TotalReviewDueDateRatio

    FROM BaseLitigations L
    LEFT JOIN LitigationRequestContract C ON L.ContractId = C.Id  -- Added missing table alias and join condition

END

--------------------------------------------------------------------------------------------
-------------------------------------------------------------------------------------------
---arbitrator dashbourd 3/6/2025-----------------------------------------------------------
GO
CREATE PROCEDURE [dbo].[GetArbitrationDashbourdReport] 
    @ArbitratorsIds NVARCHAR(MAX) = NULL,
    @LitigationRequestStageId INT = NULL,
    @FromDate DATETIME = NULL,
    @ToDate DATETIME = NULL
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
            L.IsReminderForReviewerSent,
            L.ReformerCitizenId
        FROM LitigationRequest L
        WHERE
            (L.LitigationRequestStage = 3)
             AND (@ArbitratorsIds IS NULL OR L.Id IN (
              SELECT LRA.LitigationRequestId
              FROM LitigationRequestArbitrator LRA
              WHERE LRA.ArbitratorCitizenId IN (SELECT TRY_CAST(value AS UNIQUEIDENTIFIER) FROM STRING_SPLIT(@ArbitratorsIds, ',') WHERE TRY_CAST(value AS UNIQUEIDENTIFIER) IS NOT NULL)
          ) OR L.ArbitratorCitizenId IN (SELECT value FROM STRING_SPLIT(@ArbitratorsIds, ',') ))
            AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
    )
    SELECT
        COUNT(1) AS TotalLitigationCount,
        SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) AS NotFinishedCount,
        SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) AS FinishedCount,
        SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateReconciliationCount,
        SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateArbitrationCount,
        
        -- Real Estate Brokerage (66) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) AS RealEstateBrokerageCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS RealEstateBrokerageFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS RealEstateBrokerageNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS RealEstateBrokerageDueDateCount,
        
        -- Wafi (87) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) AS WafiCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS WafiFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS WafiNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS WafiDueDateCount,
        
        -- Other Services (78) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) AS OtherServicesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS OtherServicesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS OtherServicesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS OtherServicesDueDateCount,
        
        -- Rent (82) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) AS RentCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS RentFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS RentNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS RentDueDateCount,
        
        -- Properties (79) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) AS PropertiesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 1 THEN 1 ELSE 0 END) AS PropertiesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS PropertiesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS PropertiesDueDateCount,
        
        -- Overall Ratios
        ISNULL(CAST(SUM(CASE WHEN L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS NotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS FinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS DueDateReconciliationRatio,
        ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS DueDateArbitrationRatio,
        
        -- Real Estate Brokerage Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageDueDateRatio,
        
        -- Wafi Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS WafiRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiDueDateRatio,
        
        -- Other Services Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS OtherServicesRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesDueDateRatio,
        
        -- Rent Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS RentRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentDueDateRatio,
        
        -- Properties Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS PropertiesRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 1 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.ArbitrationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesDueDateRatio,
        
        -- Merged Review DueDate count (now for Reformer stage)
        SUM(CASE 
            WHEN L.LitigationRequestStage = 3
             AND L.CreatedOn >= @FromDate 
             AND L.CreatedOn <= @ToDate 
            THEN 1 ELSE 0 
        END) AS TotalReviewDueDateCount,
        
        -- Merged Review DueDate ratio (now for Reformer stage)
        CAST(
            SUM(CASE 
                WHEN L.LitigationRequestStage = 3
                 AND L.CreatedOn >= @FromDate 
                 AND L.CreatedOn <= @ToDate 
                THEN 1 ELSE 0 
            END) * 100.0 / NULLIF(COUNT(1), 0) 
        AS DECIMAL(5,2)) AS TotalReviewDueDateRatio

    FROM BaseLitigations L
    LEFT JOIN LitigationRequestContract C ON L.ContractId = C.Id  -- Added missing table alias and join condition

END