using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SP_GetLitigationStatisticsDetailsForAllStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
--تقرير  تفاصيل اعداد الدعاوى فى جميع المراحل                 
             Create PROCEDURE [dbo].[GetLitigationStatisticsDetailsForAllStages] 
            @FromDate DATETIME = NULL,
            @ToDate DATETIME = NULL
    AS
    BEGIN
															-- مرحلة التدقيق
															--الدعاوي قيد الدراسة
			
			-- عدد الدعاوي قيد الدراسة وساطة عقارية
			DECLARE @TotalReviewNotFinishedRealEstateBrokerageCount INT;
			-- عدد  قيد الدراسةالدعاوي وافي
            DECLARE @TotalReviewNotFinishedWafiCount INT;
			-- عدد  قيد الدراسةالدعاوي أخري
			DECLARE @TotalReviewNotFinishedOtherServicesCount INT;
			-- عدد  قيد الدراسةالدعاوي ملاك
			DECLARE @TotalReviewNotFinishedPropertiesCount INT;
			-- عدد  قيد الدراسةالدعاوي ايجار
			DECLARE @TotalReviewNotFinishedRentCount INT;

															-- الدعاوي المتأخرة
			-- عدد الدعاوي المتأخرة وساطة عقارية
			DECLARE @TotalReviewDueDateRealEstateBrokerageCount INT;
			-- عدد الدعاوي المتأخرة وافي
            DECLARE @TotalReviewDueDateWafiCount INT;
			-- عدد الدعاوي المتأخرة أخري
			DECLARE @TotalReviewDueDateOtherServicesCount INT;
			-- عدد الدعاوي المتأخرة ملاك
			DECLARE @TotalReviewDueDatePropertiesCount INT;
			-- عدد الدعاوي المتأخرة ايجار
			DECLARE @TotalReviewDueDateRentCount INT;

															-- الدعاوي المنتهية
			-- عدد الدعاوي المنتهية وساطة عقارية
			DECLARE @TotalReviewFinishedRealEstateBrokerageCount INT;
			-- عدد الدعاوي المنتهية وافي
            DECLARE @TotalReviewFinishedWafiCount INT;
			-- عدد الدعاوي المنتهية أخري
			DECLARE @TotalReviewFinishedOtherServicesCount INT;
			-- عدد الدعاوي المنتهية ملاك
			DECLARE @TotalReviewFinishedPropertiesCount INT;
			-- عدد الدعاوي المنتهية ايجار
			DECLARE @TotalReviewFinishedRentCount INT;
------------------------------------------------------------------------------------------------
														-- مرحلة الصلح
													--الدعاوي قيد الدراسة
			
			-- عدد الدعاوي قيد الدراسة وساطة عقارية
			DECLARE @TotalReconciliationNotFinishedRealEstateBrokerageCount INT;
			-- عدد  قيد الدراسةالدعاوي وافي
            DECLARE @TotalReconciliationNotFinishedWafiCount INT;
			-- عدد  قيد الدراسةالدعاوي أخري
			DECLARE @TotalReconciliationNotFinishedOtherServicesCount INT;
			-- عدد  قيد الدراسةالدعاوي ملاك
			DECLARE @TotalReconciliationNotFinishedPropertiesCount INT;
			-- عدد  قيد الدراسةالدعاوي ايجار
			DECLARE @TotalReconciliationNotFinishedRentCount INT;

															-- الدعاوي المتأخرة
			-- عدد الدعاوي المتأخرة وساطة عقارية
			DECLARE @TotalReconciliationDueDateRealEstateBrokerageCount INT;
			-- عدد الدعاوي المتأخرة وافي
            DECLARE @TotalReconciliationDueDateWafiCount INT;
			-- عدد الدعاوي المتأخرة أخري
			DECLARE @TotalReconciliationDueDateOtherServicesCount INT;
			-- عدد الدعاوي المتأخرة ملاك
			DECLARE @TotalReconciliationDueDatePropertiesCount INT;
			-- عدد الدعاوي المتأخرة ايجار
			DECLARE @TotalReconciliationDueDateRentCount INT;

															-- الدعاوي المنتهية
			-- عدد الدعاوي المنتهية وساطة عقارية
			DECLARE @TotalReconciliationFinishedRealEstateBrokerageCount INT;
			-- عدد الدعاوي المنتهية وافي
            DECLARE @TotalReconciliationFinishedWafiCount INT;
			-- عدد الدعاوي المنتهية أخري
			DECLARE @TotalReconciliationFinishedOtherServicesCount INT;
			-- عدد الدعاوي المنتهية ملاك
			DECLARE @TotalReconciliationFinishedPropertiesCount INT;
			-- عدد الدعاوي المنتهية ايجار
			DECLARE @TotalReconciliationFinishedRentCount INT;
---------------------------------------------------------------------------------------------------
																	-- مرحلة التحكيم
																	--الدعاوي قيد الدراسة
			
			-- عدد الدعاوي قيد الدراسة وساطة عقارية
			DECLARE @TotalArbitrationNotFinishedRealEstateBrokerageCount INT;
			-- عدد  قيد الدراسةالدعاوي وافي
            DECLARE @TotalArbitrationNotFinishedWafiCount INT;
			-- عدد  قيد الدراسةالدعاوي أخري
			DECLARE @TotalArbitrationNotFinishedOtherServicesCount INT;
			-- عدد  قيد الدراسةالدعاوي ملاك
			DECLARE @TotalArbitrationNotFinishedPropertiesCount INT;
			-- عدد  قيد الدراسةالدعاوي ايجار
			DECLARE @TotalArbitrationNotFinishedRentCount INT;

															-- الدعاوي المتأخرة
			-- عدد الدعاوي المتأخرة وساطة عقارية
			DECLARE @TotalArbitrationDueDateRealEstateBrokerageCount INT;
			-- عدد الدعاوي المتأخرة وافي
            DECLARE @TotalArbitrationDueDateWafiCount INT;
			-- عدد الدعاوي المتأخرة أخري
			DECLARE @TotalArbitrationDueDateOtherServicesCount INT;
			-- عدد الدعاوي المتأخرة ملاك
			DECLARE @TotalArbitrationDueDatePropertiesCount INT;
			-- عدد الدعاوي المتأخرة ايجار
			DECLARE @TotalArbitrationDueDateRentCount INT;

															-- الدعاوي المنتهية
			-- عدد الدعاوي المنتهية وساطة عقارية
			DECLARE @TotalArbitrationFinishedRealEstateBrokerageCount INT;
			-- عدد الدعاوي المنتهية وافي
            DECLARE @TotalArbitrationFinishedWafiCount INT;
			-- عدد الدعاوي المنتهية أخري
			DECLARE @TotalArbitrationFinishedOtherServicesCount INT;
			-- عدد الدعاوي المنتهية ملاك
			DECLARE @TotalArbitrationFinishedPropertiesCount INT;
			-- عدد الدعاوي المنتهية ايجار
			DECLARE @TotalArbitrationFinishedRentCount INT;
            
			SET @ToDate = ISNULL(@ToDate, GETDATE());
			
------------------------------------------------------------------------------------------------------
-- Review Process
    
    -- Calculate not finished RealEstateBrokerage review count
        SELECT @TotalReviewNotFinishedRealEstateBrokerageCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 66
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Wafi review count
        SELECT @TotalReviewNotFinishedWafiCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 87
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished OtherServices review count
        SELECT @TotalReviewNotFinishedOtherServicesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 78
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Properties review count
        SELECT @TotalReviewNotFinishedPropertiesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 79
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Rent review count
        SELECT @TotalReviewNotFinishedRentCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 82
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)


-- Calculate  finished RealEstateBrokerage review count
        SELECT @TotalReviewFinishedRealEstateBrokerageCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 66
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate  finished Wafi review count
        SELECT @TotalReviewFinishedWafiCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 87
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished OtherServices review count
        SELECT @TotalReviewFinishedOtherServicesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 78
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished Properties review count
        SELECT @TotalReviewFinishedPropertiesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 79
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished Rent review count
        SELECT @TotalReviewFinishedRentCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 82
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)


	-- Calculate DueDate review RealEstateBrokerage count
        SELECT @TotalReviewDueDateRealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.IsReminderForReviewerManagerSent = 1
		   AND C.LitigationRequestContractTypeId = 66
		  AND L.IsReminderForReviewerSent = 1
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate review Wafi count
        SELECT @TotalReviewDueDateWafiCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.IsReminderForReviewerManagerSent = 1
		   AND C.LitigationRequestContractTypeId = 87
		  AND L.IsReminderForReviewerSent = 1
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate review OtherServices count
        SELECT @TotalReviewDueDateOtherServicesCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.IsReminderForReviewerManagerSent = 1
		   AND C.LitigationRequestContractTypeId = 78
		  AND L.IsReminderForReviewerSent = 1
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate review Properties count
        SELECT @TotalReviewFinishedPropertiesCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.IsReminderForReviewerManagerSent = 1
		   AND C.LitigationRequestContractTypeId = 79
		  AND L.IsReminderForReviewerSent = 1
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate review Rent count
        SELECT @TotalReviewDueDateRentCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.IsReminderForReviewerManagerSent = 1
		   AND C.LitigationRequestContractTypeId = 82
		  AND L.IsReminderForReviewerSent = 1
          AND L.LitigationRequestStage = 1
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

---------------------------------------------------------------------------------------------------------------------

-- Reconciliation Process
    
		  -- Calculate not finished RealEstateBrokerage Reconciliation count
        SELECT @TotalReconciliationNotFinishedRealEstateBrokerageCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 66
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Wafi Reconciliation count
        SELECT @TotalReconciliationNotFinishedWafiCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 87
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished OtherServices Reconciliation count
        SELECT @TotalReconciliationNotFinishedOtherServicesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 78
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Properties Reconciliation count
        SELECT @TotalReconciliationNotFinishedPropertiesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 79
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Rent Reconciliation count
        SELECT @TotalReconciliationNotFinishedRentCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 82
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)


		-- Calculate  finished RealEstateBrokerage Reconciliation count
        SELECT @TotalReconciliationFinishedRealEstateBrokerageCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 1
		   AND C.LitigationRequestContractTypeId = 66
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate  finished Wafi Reconciliation count
        SELECT @TotalReconciliationFinishedWafiCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 87
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished OtherServices Reconciliation count
        SELECT @TotalReconciliationFinishedOtherServicesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 78
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished Properties Reconciliation count
        SELECT @TotalReconciliationFinishedPropertiesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 79
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished Rent Reconciliation count
        SELECT @TotalReconciliationFinishedRentCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 2
		   AND C.LitigationRequestContractTypeId = 82
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)


			-- Calculate DueDate Reconciliation RealEstateBrokerage count
        SELECT @TotalReconciliationDueDateRealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ReconciliationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 66
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Reconciliation Wafi count
        SELECT @TotalReconciliationDueDateWafiCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ReconciliationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 87
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Reconciliation OtherServices count
        SELECT @TotalReconciliationDueDateOtherServicesCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ReconciliationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 78
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Reconciliation Properties count
        SELECT @TotalReconciliationFinishedPropertiesCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ReconciliationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 79
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Reconciliation Rent count
        SELECT @TotalReconciliationDueDateRentCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ReconciliationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 82
          AND L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

-----------------------------------------------------------------------------------------------------------------------
-- Arbitration Process
    
		  -- Calculate not finished RealEstateBrokerage Arbitration count
        SELECT @TotalArbitrationNotFinishedRealEstateBrokerageCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 66
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Wafi Arbitration count
        SELECT @TotalArbitrationNotFinishedWafiCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 87
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished OtherServices Arbitration count
        SELECT @TotalArbitrationNotFinishedOtherServicesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 78
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Properties Arbitration count
        SELECT @TotalArbitrationNotFinishedPropertiesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 79
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate not finished Rent Arbitration count
        SELECT @TotalArbitrationNotFinishedRentCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 82
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)


		-- Calculate  finished RealEstateBrokerage Arbitration count
        SELECT @TotalArbitrationFinishedRealEstateBrokerageCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 66
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate  finished Wafi Arbitration count
        SELECT @TotalArbitrationFinishedWafiCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 87
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished OtherServices Arbitration count
        SELECT @TotalArbitrationFinishedOtherServicesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 78
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished Properties Arbitration count
        SELECT @TotalArbitrationFinishedPropertiesCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 79
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate finished Rent Arbitration count
        SELECT @TotalArbitrationFinishedRentCount = COUNT(*)  
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
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.LitigationRequestStage = 3
		   AND C.LitigationRequestContractTypeId = 82
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)


			-- Calculate DueDate Arbitration RealEstateBrokerage count
        SELECT @TotalArbitrationDueDateRealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ArbitrationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 66
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Arbitration Wafi count
        SELECT @TotalArbitrationDueDateWafiCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ArbitrationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 87
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Arbitration OtherServices count
        SELECT @TotalArbitrationDueDateOtherServicesCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ArbitrationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 78
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Arbitration Properties count
        SELECT @TotalArbitrationFinishedPropertiesCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ArbitrationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 79
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)

		  -- Calculate DueDate Arbitration Rent count
        SELECT @TotalArbitrationDueDateRentCount = COUNT(*)  
        FROM LitigationRequest L
		INNER JOIN dbo.LitigationRequestContract C on L.ContractId = C.Id
        WHERE 
           L.ArbitrationPeriod = 0
		   AND C.LitigationRequestContractTypeId = 82
          AND L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)



    -- Review Output counts 
            SELECT 
			@TotalReviewNotFinishedRealEstateBrokerageCount AS TotalReviewNotFinishedRealEstateBrokerageCount,
            @TotalReviewNotFinishedWafiCount AS TotalReviewNotFinishedWafiCount,
			@TotalReviewNotFinishedOtherServicesCount AS TotalReviewNotFinishedOtherServicesCount,
			@TotalReviewNotFinishedPropertiesCount AS TotalReviewNotFinishedPropertiesCount,
			@TotalReviewNotFinishedRentCount AS TotalReviewNotFinishedRentCount,

			@TotalReviewDueDateRealEstateBrokerageCount AS TotalReviewDueDateRealEstateBrokerageCount,
            @TotalReviewDueDateWafiCount AS TotalReviewDueDateWafiCount,
			@TotalReviewDueDateOtherServicesCount AS TotalReviewDueDateOtherServicesCount,
			@TotalReviewDueDatePropertiesCount AS TotalReviewDueDatePropertiesCount,
			@TotalReviewDueDateRentCount AS TotalReviewDueDateRentCount,

			@TotalReviewFinishedRealEstateBrokerageCount AS TotalReviewFinishedRealEstateBrokerageCount,
            @TotalReviewFinishedWafiCount AS TotalReviewFinishedWafiCount,
			@TotalReviewFinishedOtherServicesCount AS TotalReviewFinishedOtherServicesCount,
			@TotalReviewFinishedPropertiesCount AS TotalReviewFinishedPropertiesCount,
			@TotalReviewFinishedRentCount AS TotalReviewFinishedRentCount,


	-- Reconciliation Output counts  
			@TotalReconciliationNotFinishedRealEstateBrokerageCount AS TotalReconciliationNotFinishedRealEstateBrokerageCount,
            @TotalReconciliationNotFinishedWafiCount AS TotalReconciliationNotFinishedWafiCount,
			@TotalReconciliationNotFinishedOtherServicesCount AS TotalReconciliationNotFinishedOtherServicesCount,
			@TotalReconciliationNotFinishedPropertiesCount AS TotalReconciliationNotFinishedPropertiesCount,
			@TotalReconciliationNotFinishedRentCount AS TotalReconciliationNotFinishedRentCount,

			@TotalReconciliationDueDateRealEstateBrokerageCount AS TotalReconciliationDueDateRealEstateBrokerageCount,
            @TotalReconciliationDueDateWafiCount AS TotalReconciliationDueDateWafiCount,
			@TotalReconciliationDueDateOtherServicesCount AS TotalReconciliationDueDateOtherServicesCount,
			@TotalReconciliationDueDatePropertiesCount AS TotalReconciliationDueDatePropertiesCount,
			@TotalReconciliationDueDateRentCount AS TotalReconciliationDueDateRentCount,

			@TotalReconciliationFinishedRealEstateBrokerageCount AS TotalReconciliationFinishedRealEstateBrokerageCount,
            @TotalReconciliationFinishedWafiCount AS TotalReconciliationFinishedWafiCount,
			@TotalReconciliationFinishedOtherServicesCount AS TotalReconciliationFinishedOtherServicesCount,
			@TotalReconciliationFinishedPropertiesCount AS TotalReconciliationFinishedPropertiesCount,
			@TotalReconciliationFinishedRentCount AS TotalReconciliationFinishedRentCount,



	-- Arbitration Output counts  
			@TotalArbitrationNotFinishedRealEstateBrokerageCount AS TotalArbitrationNotFinishedRealEstateBrokerageCount,
            @TotalArbitrationNotFinishedWafiCount AS TotalArbitrationNotFinishedWafiCount,
			@TotalArbitrationNotFinishedOtherServicesCount AS TotalArbitrationNotFinishedOtherServicesCount,
			@TotalArbitrationNotFinishedPropertiesCount AS TotalArbitrationNotFinishedPropertiesCount,
			@TotalArbitrationNotFinishedRentCount AS TotalArbitrationNotFinishedRentCount,

			@TotalArbitrationDueDateRealEstateBrokerageCount AS TotalArbitrationDueDateRealEstateBrokerageCount,
            @TotalArbitrationDueDateWafiCount AS TotalArbitrationDueDateWafiCount,
			@TotalArbitrationDueDateOtherServicesCount AS TotalArbitrationDueDateOtherServicesCount,
			@TotalArbitrationDueDatePropertiesCount AS TotalArbitrationDueDatePropertiesCount,
			@TotalArbitrationDueDateRentCount AS TotalArbitrationDueDateRentCount,

			@TotalArbitrationFinishedRealEstateBrokerageCount AS TotalArbitrationFinishedRealEstateBrokerageCount,
            @TotalArbitrationFinishedWafiCount AS TotalArbitrationFinishedWafiCount,
			@TotalArbitrationFinishedOtherServicesCount AS TotalArbitrationFinishedOtherServicesCount,
			@TotalArbitrationFinishedPropertiesCount AS TotalArbitrationFinishedPropertiesCount,
			@TotalArbitrationFinishedRentCount AS TotalArbitrationFinishedRentCount

        END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetLitigationStatisticsDetailsForAllStages]");

        }
    }
}
