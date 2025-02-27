
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'206d0d2e-4d1c-491a-b217-3c54ecb46151', N'SLA.ArbitratorRegisterationFeesPeriod', N'72')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'bc995140-71d9-4d01-a861-3ba453c21f9c', N'SLA.ArbitratorRegisterationFeesFirstNotificationPeriod', N'48')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'acdd0157-4331-4db0-a7c1-ce65a9098011', N'SLA.ArbitratorRegisterationFeesNotification', N'برجاء دفع رسوم التسجيل كمحكم قبل انتهاء المدة المحددة لذلك')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'09a843c4-ebd3-402a-b8cf-2f76146bbee7', N'SLA.ArbitratorMembershipFeesPeriod', N'72')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'25ed311f-2181-4d70-919a-9b95544f9839', N'SLA.ArbitratorMembershipFeesFirstNotificationPeriod', N'48')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'f2cea285-5f77-4e4e-b883-aa8b9ceee83f', N'SLA.ArbitratorMembershipFeesNotification', N'برجاء دفع رسوم العضوية كمحكم قبل انتهاء المدة المحددة لذلك')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'6c2a28e3-4ec2-4541-af24-aed002239e3c', N'SLA.ArbitratorMembershipRenewalFeesNotification', N'برجاء دفع رسوم تجديد العضوية كمحكم قبل انتهاء المدة المحددة لذلك')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'0cc61c3e-a97b-4a9e-8ca3-9a01f5715199', N'Configurable.ArbitratorMembershipFees', N'250')

INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'57b2d3b0-06bc-48b3-b14d-cf1c87066849', N'LitigationRequest.ModificationRequest', N'تم أضافة طلب تعديل علي الدعوى رقم {LitigationCode}')

-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'0cc61c3e-a97b-4a9e-8ca3-9a01f5715199', N'Configurable.ArbitratorRegisterationFees.A', N'100')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'0cc61c3e-a97b-4a9e-8ca3-9a01f5715199', N'Configurable.ArbitratorRegisterationFees.B', N'200')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'0cc61c3e-a97b-4a9e-8ca3-9a01f5715199', N'Configurable.ArbitratorRegisterationFees.C', N'300')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'0cc61c3e-a97b-4a9e-8ca3-9a01f5715199', N'Configurable.ArbitratorMembershipFees.A', N'400')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'0cc61c3e-a97b-4a9e-8ca3-9a01f5715199', N'Configurable.ArbitratorMembershipFees.B', N'500')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (N'0cc61c3e-a97b-4a9e-8ca3-9a01f5715199', N'Configurable.ArbitratorMembershipFees.C', N'600')

------------------------------------------------------------------------------------------------------------------------------------------------------------------

GO
/****** Object:  StoredProcedure [dbo].[GetLitigationStatisticsForAllStages]    Script Date: 2/26/2025 11:19:21 AM ******/
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
