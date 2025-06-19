GO
/** Object:  StoredProcedure [dbo].[GetLitigationStatisticsForAllStages]    Script Date: 6/11/2025 3:40:03 PM **/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--تقرير اعداد الدعاوى فى جميع المراحل                 
             ALTER PROCEDURE [dbo].[GetLitigationStatisticsForAllStages] 
            @FromDate DATETIME = NULL,
            @ToDate DATETIME = NULL
    AS
    BEGIN
	-- Handle default dates if @FromDate is NULL
    SET @FromDate = ISNULL(@FromDate, '1970-01-01 00:00:00.000');

    -- Handle default dates if @ToDate is NULL
    SET @ToDate = ISNULL(@ToDate, GETDATE());
															
															-- مرحلة التدقيق
			-- عدد الدعاوي الكلية
			DECLARE @TotalReviewAllCount INT;
			--عدد الدعاوى الغير منتهية
			DECLARE @TotalReviewNotFinishedCount INT;
			--عدد الدعاوى المنتهية
			DECLARE @TotalReviewFinishedCount INT;
			-- عدد الدعاوي المتأخرة
			DECLARE @TotalReviewDueDateCount INT;
			-- عدد الدعاوي وساطة عقارية
			DECLARE @TotalReviewRealEstateBrokerageCount INT;
			-- عدد الدعاوي وافي
            DECLARE @TotalReviewWafiCount INT;
			-- عدد الدعاوي أخري
			DECLARE @TotalReviewOtherServicesCount INT;
			-- عدد الدعاوي ملاك
			DECLARE @TotalReviewPropertiesCount INT;
			-- عدد الدعاوي ايجار
			DECLARE @TotalReviewRentCount INT;
------------------------------------------------------------------------------------------------
														-- مرحلة الصلح
			-- عدد الدعاوي الكلية
			DECLARE @TotalReconciliationAllCount INT;
			--عدد الدعاوى الغير منتهية
			DECLARE @TotalReconciliationNotFinishedCount INT;
			--عدد الدعاوى المنتهية
			DECLARE @TotalReconciliationFinishedCount INT;
			-- عدد الدعاوي المتأخرة
			DECLARE @TotalReconciliationDueDateCount INT;
			-- عدد الدعاوي وساطة عقارية
			DECLARE @TotalReconciliationRealEstateBrokerageCount INT;
			-- عدد الدعاوي وافي
            DECLARE @TotalReconciliationWafiCount INT;
			-- عدد الدعاوي أخري
			DECLARE @TotalReconciliationOtherServicesCount INT;
			-- عدد الدعاوي ملاك
			DECLARE @TotalReconciliationPropertiesCount INT;
			-- عدد الدعاوي ايجار
			DECLARE @TotalReconciliationRentCount INT;

			-- عدد الدعاوي المنتهيه في حالة تم الصلح
			DECLARE @ReconciliationWasMadeCount INT;
			--عدد الدعاوى المنتهيه في حالة تم التنازل عن الحق
			DECLARE @WaiverOfRightCount INT;
			--عدد الدعاوى المنتهيه في حالة تم التنازل عن الدعوي
			DECLARE @WaiverOfTheLawSuitCount INT;
			-- عدد الدعاوي المنتهيه في حالة لم تم الصلح
			DECLARE @LackOfReconciliationCount INT;
			-- عدد الدعاوي المنتهيه في حالة تم الصلح في منصة تراضي
			DECLARE @TheReconciliationOnTradyPatformCount INT;

---------------------------------------------------------------------------------------------------
																	-- مرحلة التحكيم
			-- عدد الدعاوي الكلية
			DECLARE @TotalArbitrationAllCount INT;
			--عدد الدعاوى الغير منتهية
			DECLARE @TotalArbitrationNotFinishedCount INT;
			--عدد الدعاوى المنتهية
			DECLARE @TotalArbitrationFinishedCount INT;
			-- عدد الدعاوي المتأخرة
			DECLARE @TotalArbitrationDueDateCount INT;
			-- عدد الدعاوي وساطة عقارية
			DECLARE @TotalArbitrationRealEstateBrokerageCount INT;
			-- عدد الدعاوي وافي
            DECLARE @TotalArbitrationWafiCount INT;
			-- عدد الدعاوي أخري
			DECLARE @TotalArbitrationOtherServicesCount INT;
			-- عدد الدعاوي ملاك
			DECLARE @TotalArbitrationPropertiesCount INT;
			-- عدد الدعاوي ايجار
			DECLARE @TotalArbitrationRentCount INT;
            SET @ToDate = ISNULL(@ToDate, GETDATE());
------------------------------------------------------------------------------------------------------
-- 



-- Review Process
    -- Calculate Total Review All Count
		SELECT @TotalReviewAllCount = COUNT(*)  
        FROM LitigationRequest L 
        WHERE
		 L.LitigationRequestStage = 1
		AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
    -- Calculate not finished review count
        SELECT @TotalReviewNotFinishedCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN (
		    SELECT w2.*
		    FROM WorkFlowProcessingLog w2
		    INNER JOIN (
		        SELECT RequestId, MAX(CreatedOn) AS LatestCreatedOn
		        FROM WorkFlowProcessingLog
		        GROUP BY RequestId
		    ) AS latest ON w2.RequestId = latest.RequestId AND w2.CreatedOn = latest.LatestCreatedOn AND w2.IsFinished = 0
		) AS w ON L.Id = w.RequestId
        WHERE 
           L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate finished review count
        SELECT @TotalReviewFinishedCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN (
		    SELECT w2.*
		    FROM WorkFlowProcessingLog w2
		    INNER JOIN (
		        SELECT RequestId, MAX(CreatedOn) AS LatestCreatedOn
		        FROM WorkFlowProcessingLog
		        GROUP BY RequestId
		    ) AS latest ON w2.RequestId = latest.RequestId AND w2.CreatedOn = latest.LatestCreatedOn AND w2.IsFinished = 1
		) AS w ON L.Id = w.RequestId
        WHERE L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate DueDate review count
        SELECT @TotalReviewDueDateCount = COUNT(*)  
        FROM LitigationRequest L
        WHERE 
           L.IsReminderForReviewerManagerSent = 1
		  AND L.IsReminderForReviewerSent = 1
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
   -- Calculate RealEstateBrokerage review count
        SELECT @TotalReviewRealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 66
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Wafi review count
        SELECT @TotalReviewWafiCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 87
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate OtherServices review count
        SELECT @TotalReviewOtherServicesCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 78
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Properties review count
        SELECT @TotalReviewPropertiesCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 79
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Rent review count
        SELECT @TotalReviewRentCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 82
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
---------------------------------------------------------------------------------------------------------------
-- Reconciliation Process
    -- Calculate Total Review All Count
		SELECT @TotalReconciliationAllCount = COUNT(*)  
        FROM LitigationRequest L 
        WHERE
		 L.LitigationRequestStage = 2
		AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
    -- Calculate not finished review count
        SELECT @TotalReconciliationNotFinishedCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN (
		    SELECT w2.*
		    FROM WorkFlowProcessingLog w2
		    INNER JOIN (
		        SELECT RequestId, MAX(CreatedOn) AS LatestCreatedOn
		        FROM WorkFlowProcessingLog
		        GROUP BY RequestId
		    ) AS latest ON w2.RequestId = latest.RequestId AND w2.CreatedOn = latest.LatestCreatedOn AND w2.IsFinished = 0
		) AS w ON L.Id = w.RequestId
        WHERE 
           L.LitigationRequestStage = 2 And L.IsFinished = 0
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate finished review count
  
       SELECT @TotalReconciliationFinishedCount =  Count(*)
	   FROM LitigationRequest L
	   where ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) 
	   or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3))
	   AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	   -- Calculate finished with ReconciliationWasMade count
  
       SELECT @ReconciliationWasMadeCount =  Count(*)
	   FROM LitigationRequest L
	   where (L.IsFinished = 1 And L.LitigationRequestCategoryId = 1 AND L.InternalStatusId= 18)
	   AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

	   -- Calculate finished with ReconciliationWasMade count
  
       SELECT @WaiverOfRightCount =  Count(*)
	   FROM LitigationRequest L
	   where (L.IsFinished = 1 And L.LitigationRequestCategoryId = 1 AND L.InternalStatusId= 7)
	   AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	   -- Calculate finished with ReconciliationWasMade count
  
       SELECT @WaiverOfTheLawSuitCount =  Count(*)
	   FROM LitigationRequest L
	   where (L.IsFinished = 1 And L.LitigationRequestCategoryId = 1 AND L.InternalStatusId= 8)
	   AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	   -- Calculate finished with ReconciliationWasMade count
  
       SELECT @LackOfReconciliationCount =  Count(*)
	   FROM LitigationRequest L
	   where (L.IsFinished = 1 And L.LitigationRequestCategoryId = 1 AND L.InternalStatusId= 9)
	   AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	   -- Calculate finished with ReconciliationWasMade count
  
       SELECT @TheReconciliationOnTradyPatformCount =  Count(*)
	   FROM LitigationRequest L
	   where (L.IsFinished = 1 And L.LitigationRequestCategoryId = 1 AND L.InternalStatusId= 25)
	   AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate DueDate review count
        SELECT @TotalReconciliationDueDateCount = COUNT(*)  
        FROM LitigationRequest L
        WHERE 
           L.ReconciliationPeriod = 0
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
   -- Calculate RealEstateBrokerage review count
        SELECT @TotalReconciliationRealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 66
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Wafi review count
        SELECT @TotalReconciliationWafiCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 87
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate OtherServices review count
        SELECT @TotalReconciliationOtherServicesCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 78
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Properties review count
        SELECT @TotalReconciliationPropertiesCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 79
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Rent review count
        SELECT @TotalReconciliationRentCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 82
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
-----------------------------------------------------------------------------------------------------------
-- Arbitration Process
    -- Calculate Total Review All Count
		SELECT @TotalArbitrationAllCount = COUNT(*)  
        FROM LitigationRequest L 
        WHERE
		 L.LitigationRequestStage = 3
		AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
    -- Calculate not finished  count
        SELECT @TotalArbitrationNotFinishedCount = COUNT(*)  
        FROM LitigationRequest L
        WHERE 
           L.IsFinished = 0 AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate finished review count
  
  SELECT @TotalArbitrationFinishedCount =  Count(*)
	   FROM LitigationRequest L
	   where L.IsFinished = 1 AND L.LitigationRequestCategoryId = 3 AND
	   (L.InternalStatusId = 20 or ExternalStatusId = 20)
	   AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate DueDate review count
        SELECT @TotalArbitrationDueDateCount = COUNT(*)  
        FROM LitigationRequest L
        WHERE 
           L.ReconciliationPeriod = 0
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
   -- Calculate RealEstateBrokerage review count
        SELECT @TotalArbitrationRealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 66
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Wafi review count
        SELECT @TotalArbitrationWafiCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 87
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate OtherServices review count
        SELECT @TotalArbitrationOtherServicesCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 78
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Properties review count
        SELECT @TotalArbitrationPropertiesCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 79
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
	-- Calculate Rent review count
        SELECT @TotalArbitrationRentCount = COUNT(*)  
        FROM LitigationRequest L
        INNER JOIN LitigationRequestContract c ON l.ContractId = c.Id
        WHERE c.LitigationRequestContractTypeId = 82
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
    -- Review Output counts 
            SELECT 
			@TotalReviewAllCount AS TotalReviewAllCount,
			@TotalReviewNotFinishedCount AS TotalReviewNotFinishedCount,
			@TotalReviewFinishedCount AS TotalReviewFinishedCount,
			@TotalReviewDueDateCount AS TotalReviewDueDateCount,
			@TotalReviewRealEstateBrokerageCount AS TotalReviewRealEstateBrokerageCount,
            @TotalReviewWafiCount AS TotalReviewWafiCount,
			@TotalReviewOtherServicesCount AS TotalReviewOtherServicesCount,
			@TotalReviewPropertiesCount AS TotalReviewPropertiesCount,
			@TotalReviewRentCount AS TotalReviewRentCount,
			------------------------------------------------------
			@TotalReconciliationAllCount AS TotalReconciliationAllCount,
			@TotalReconciliationNotFinishedCount AS TotalReconciliationNotFinishedCount,
			@TotalReconciliationFinishedCount AS TotalReconciliationFinishedCount,

			@ReconciliationWasMadeCount AS ReconciliationWasMadeCount,
			@WaiverOfRightCount AS WaiverOfRightCount,
			@WaiverOfTheLawSuitCount AS WaiverOfTheLawSuitCount,
			@LackOfReconciliationCount AS LackOfReconciliationCount,
			@TheReconciliationOnTradyPatformCount AS TheReconciliationOnTradyPatformCount,


			@TotalReconciliationDueDateCount AS TotalReconciliationDueDateCount,
			@TotalReconciliationRealEstateBrokerageCount AS TotalReconciliationRealEstateBrokerageCount,
            @TotalReconciliationWafiCount AS TotalReconciliationWafiCount,
			@TotalReconciliationOtherServicesCount AS TotalReconciliationOtherServicesCount,
			@TotalReconciliationPropertiesCount AS TotalReconciliationPropertiesCount,
			@TotalReconciliationRentCount AS TotalReconciliationRentCount,
			--------------------------------------------------------
			@TotalArbitrationAllCount AS TotalArbitrationAllCount,
			@TotalArbitrationNotFinishedCount AS TotalArbitrationNotFinishedCount,
			@TotalArbitrationFinishedCount AS TotalArbitrationFinishedCount,
			@TotalArbitrationDueDateCount AS TotalArbitrationDueDateCount,
			@TotalArbitrationRealEstateBrokerageCount AS TotalArbitrationRealEstateBrokerageCount,
            @TotalArbitrationWafiCount AS TotalArbitrationWafiCount,
			@TotalArbitrationOtherServicesCount AS TotalArbitrationOtherServicesCount,
			@TotalArbitrationPropertiesCount AS TotalArbitrationPropertiesCount,
			@TotalArbitrationRentCount AS TotalArbitrationRentCount
        END

        -------------------------------------------------------------------------------
        ------------------------------------------------------------------------------
                     GO
        alter PROCEDURE [dbo].[GetReconciliationDashbourdReport] 
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
            L.ReformerCitizenId,
			L.InternalStatusId
        FROM LitigationRequest L
        WHERE
            (L.LitigationRequestStage = 2 or (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) 
            AND (
                @ReformersId IS NULL
                OR L.ReformerCitizenId = @ReformersId  -- This comparison now works correctly
            )
            AND (L.SentOn >= @FromDate AND L.SentOn <= @ToDate)
    )
    SELECT
        COUNT(1) AS TotalLitigationCount,
        SUM(CASE WHEN (L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) AS NotFinishedCount,
        SUM(CASE WHEN (L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)   THEN 1 ELSE 0 END) AS FinishedCount,
        SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateReconciliationCount,
        SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) AS DueDateArbitrationCount,
        
			-- get status counts  for finished 
		   SUM(CASE WHEN ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) And L.InternalStatusId= 18 THEN 1 ELSE 0 END) AS ReconciliationWasMadeCount,
		   SUM(CASE WHEN ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) And L.InternalStatusId= 7 THEN 1 ELSE 0 END) AS WaiverOfRightCount,
		   SUM(CASE WHEN ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) And L.InternalStatusId= 8 THEN 1 ELSE 0 END) AS WaiverOfTheLawsuitCount,
		   SUM(CASE WHEN (((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) AND (L.InternalStatusId= 9)) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3))  THEN 1 ELSE 0 END) AS LackOfReconciliationCount,
		   SUM(CASE WHEN ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) And L.InternalStatusId= 25 THEN 1 ELSE 0 END) AS TheReconciliationOnTradyPatformCount,

        -- Real Estate Brokerage (66) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) AS RealEstateBrokerageCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) AS RealEstateBrokerageFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND  (L.IsFinished = 0 And L.LitigationRequestStage = 2)  THEN 1 ELSE 0 END) AS RealEstateBrokerageNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS RealEstateBrokerageDueDateCount,
        
        -- Wafi (87) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) AS WafiCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) AS WafiFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND(L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) AS WafiNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS WafiDueDateCount,
        
        -- Other Services (78) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) AS OtherServicesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) AS OtherServicesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND(L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) AS OtherServicesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS OtherServicesDueDateCount,
        
        -- Rent (82) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) AS RentCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) AS RentFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) AS RentNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS RentDueDateCount,
        
        -- Properties (79) breakdown
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) AS PropertiesCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) AS PropertiesFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND(L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) AS PropertiesNotFinishedCount,
        SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) AS PropertiesDueDateCount,
        
        -- Overall Ratios
        ISNULL(CAST(SUM(CASE WHEN (L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS NotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS FinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 2 AND L.ReconciliationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS DueDateReconciliationRatio,
        ISNULL(CAST(SUM(CASE WHEN L.LitigationRequestStage = 3 AND L.ArbitrationPeriod = 0 AND L.IsFinished = 0 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS DueDateArbitrationRatio,
        
        -- Real Estate Brokerage Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 66 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RealEstateBrokerageDueDateRatio,
        
        -- Wafi Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS WafiRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 87 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS WafiDueDateRatio,
        
        -- Other Services Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS OtherServicesRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND(L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 78 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS OtherServicesDueDateRatio,
        
        -- Rent Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS RentRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND(L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentNotFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 AND (L.ReconciliationPeriod = 0 AND L.IsFinished = 0) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 82 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS RentDueDateRatio,
        
        -- Properties Ratios
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END) * 100.0 / NULLIF(COUNT(1), 0) AS DECIMAL(5,2)), 0) AS PropertiesRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND ((L.IsFinished = 1 And L.LitigationRequestCategoryId = 1) or  (L.LitigationRequestCategoryId = 3 And L.LitigationRequestStage=3)) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesFinishedRatio,
        ISNULL(CAST(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 AND (L.IsFinished = 0 And L.LitigationRequestStage = 2) THEN 1 ELSE 0 END) * 100.0 / NULLIF(SUM(CASE WHEN C.LitigationRequestContractTypeId = 79 THEN 1 ELSE 0 END), 0) AS DECIMAL(5,2)), 0) AS PropertiesNotFinishedRatio,
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


--------------------------------------------------------------------------------
-------------------------------------------------------------------------------

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- مدير النظام لعدد مقدمين الخدمة
Create PROCEDURE [dbo].[GetReviewrsReformersArbitratorsStatisticsReport]
    

AS
BEGIN
    SET NOCOUNT ON;
    
    WITH Reviewers AS (
        SELECT
            E.Id,
			E.IsActive,
			NULL AS ReformerStatusId,
			NULL AS ArbitratorStatusId
        FROM SysEmployee E
		LEFT JOIN SysDepartment sd
		on E.DepartmentId = sd.Id
        WHERE
            (
				sd.DepartmentType = 1
			)
		),
		ReformersAndArbitrators AS (
		Select
		c.Id,
        NULL AS IsActive,
		c.ReformerStatusId AS ReformerStatusId,
		c.ArbitratorStatusId AS ArbitratorStatusId
		FROM Citizen c
		WHERE 
		(
		c.IsReformer = 1 OR c.IsArbitrator = 1
		)

		),

		AllPersonnel AS (
        SELECT * FROM Reviewers
        UNION ALL
        SELECT * FROM ReformersAndArbitrators
        
    )


    SELECT
	
        SUM(CASE WHEN allPersonnel.IsActive = 1 THEN 1 ELSE 0 END) AS CertifiedReviewers,
        SUM(CASE WHEN allPersonnel.IsActive = 0 THEN 1 ELSE 0 END) AS StoppedReviewers,

		SUM(CASE WHEN allPersonnel.ReformerStatusId = 1 THEN 1 ELSE 0 END) AS CertifiedReformers,
        SUM(CASE WHEN allPersonnel.ReformerStatusId = 0 THEN 1 ELSE 0 END) AS StoppedReformers,

		SUM(CASE WHEN allPersonnel.ArbitratorStatusId = 1 THEN 1 ELSE 0 END) AS CertifiedArbitrators,
        SUM(CASE WHEN allPersonnel.ArbitratorStatusId = 0 THEN 1 ELSE 0 END) AS StoppedArbitrators
      
    FROM AllPersonnel allPersonnel
    
END