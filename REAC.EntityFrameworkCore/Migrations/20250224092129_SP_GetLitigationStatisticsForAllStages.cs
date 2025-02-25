using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class SP_GetLitigationStatisticsForAllStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"--تقرير اعداد الدعاوى فى جميع المراحل                 
             Create PROCEDURE [dbo].[GetLitigationStatisticsForAllStages] 
            
            @FromDate DATETIME = NULL,
            @ToDate DATETIME = NULL
   
    AS
    BEGIN
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
           L.LitigationRequestStage = 2
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
    
	-- Calculate finished review count
        SELECT @TotalReconciliationFinishedCount = COUNT(*)  
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
        WHERE L.LitigationRequestStage = 2
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

    -- Calculate not finished review count
        SELECT @TotalArbitrationNotFinishedCount = COUNT(*)  
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
           L.LitigationRequestStage = 3
          AND (L.CreatedOn >= @FromDate)
          AND (l.CreatedOn <= @ToDate)
    
	-- Calculate finished review count
        SELECT @TotalArbitrationFinishedCount = COUNT(*)  
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
        WHERE L.LitigationRequestStage = 3
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
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetLitigationStatisticsForAllStages]");

        }
    }
}
