GO
/****** Object:  StoredProcedure [dbo].[GetLitigationStatisticsGeneralReport]    Script Date: 2/23/2025 1:31:46 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
                --تقرير اعداد الدعاوى                
             ALTER PROCEDURE [dbo].[GetLitigationStatisticsGeneralReport] 
            @EmployeeId NVARCHAR(50) = NULL,
            @LitigationRequestStageId INT = NULL,--(فى اى مرحلة فيها الدعوى (تدقيق/صلح/صلح وتحكيم
            @FromDate DATETIME = NULL,
            @ToDate DATETIME = NULL,
            @AreaIds NVARCHAR(MAX) = NULL,
            @CitiesIds NVARCHAR(MAX) = NULL,
            @FromFees DECIMAL(10, 3) = NULL, 
            @ToFees DECIMAL(10, 3) = NULL,
            @LitigationRequestCategoryIds NVARCHAR(MAX) = NULL --(نوع الدعوى المقدمة(تدقيق/صلح/صلح وتحكيم     
    AS
    BEGIN
            -- Declare necessary variables
            DECLARE @TotalLitigation INT;--عدد الدعاوى الكلية
            DECLARE @NotFinishedCount INT;--عدد الدعاوى الغير منتهية
            DECLARE @FinishedCount INT;--عدد الدعاوى المنتهية
			DECLARE @DueDateReconciliationCount INT;--عدد الدعاوى المتأخره في مرحلة الصلح 
			DECLARE @DueDateArbitrationCount INT;--عدد الدعاوى المتأخره في مرحلة التحكيم
            DECLARE @RealEstateBrokerageCount INT;--عدد الدعاوى من نوع وساطة عقارية
            DECLARE @WafiCount INT;--عدد الدعاوى من نوع وافى
            DECLARE @OtherServicesCount INT;--عدد الدعاوى من نوع اخرى
            DECLARE @RentCount INT;--عدد الدعاوى من نوع ايجار
            DECLARE @PropertiesCount INT;--عدد الدعاوى من نوع املاك
             --النسب المئوية للمتغيرات السابقة   
            DECLARE @NotFinishedCountRatio DECIMAL(5, 2);
            DECLARE @FinishedCountRatio DECIMAL(5, 2);
            DECLARE @RealEstateBrokerageCountRatio DECIMAL(5, 2);
            DECLARE @WafiCountRatio DECIMAL(5, 2);
            DECLARE @OtherServicesCountRatio DECIMAL(5, 2);
            DECLARE @RentCountRatio DECIMAL(5, 2);
            DECLARE @PropertiesCountRatio DECIMAL(5, 2);
            -- Handle default dates if @ToDate is NULL
            SET @ToDate = ISNULL(@ToDate, GETDATE());
    -- Calculate total litigation count with filters
        SELECT @TotalLitigation = COUNT(*)  
        FROM LitigationRequest L 
        WHERE 
         -- AND (@LitigationRequestStageId IS NULL OR L.LitigationRequestStage = @LitigationRequestStageId)
           (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
    -- Calculate not finished count
        SELECT @NotFinishedCount = COUNT(*)  
        FROM LitigationRequest L 
        WHERE 
           L.IsFinished = 0
         -- AND (@LitigationRequestStageId IS NULL OR L.LitigationRequestStage = @LitigationRequestStageId)
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
    -- Calculate finished count
        SELECT @FinishedCount = COUNT(*)  
        FROM LitigationRequest L 
        WHERE 
           L.IsFinished = 1
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
	-- Calculate due date Reconciliation count
        SELECT @DueDateReconciliationCount = COUNT(*)  
        FROM LitigationRequest L 
        WHERE 
           L.IsFinished = 0 AND L.LitigationRequestStage = 2 And L.ReconciliationPeriod = 0
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.StartReconciliationDate >= @FromDate)
          AND (L.StartReconciliationDate <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
	-- Calculate due date Arbitration count
        SELECT @DueDateArbitrationCount = COUNT(*)  
        FROM LitigationRequest L 
        WHERE 
           L.IsFinished = 0 AND L.LitigationRequestStage = 3 And L.ArbitrationPeriod = 0
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.StartArbitrationDate >= @FromDate)
          AND (L.StartArbitrationDate <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
    -- Calculate RealEstateBrokerageCount(وساطة)
        SELECT @RealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequestContract C 
        INNER JOIN LitigationRequest L ON C.Id = L.ContractId
        WHERE 
           C.LitigationRequestContractTypeId = 66
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
    -- Similar pattern for WafiCount (وافى)
        SELECT @WafiCount = COUNT(*)  
        FROM LitigationRequestContract C 
        INNER JOIN LitigationRequest L ON C.Id = L.ContractId
        WHERE
           C.LitigationRequestContractTypeId = 87
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
 -- Similar pattern for  OtherServicesCount(اخرى)
        SELECT @OtherServicesCount = COUNT(*)  
        FROM LitigationRequestContract C 
        INNER JOIN LitigationRequest L ON C.Id = L.ContractId
        WHERE 
           C.LitigationRequestContractTypeId = 78
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
 -- Similar pattern for  Rent(ايجار)
        SELECT @RentCount = COUNT(*)  
        FROM LitigationRequestContract C 
        INNER JOIN LitigationRequest L ON C.Id = L.ContractId
        WHERE 
           C.LitigationRequestContractTypeId = 82
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
-- Similar pattern for  Properties (املاك)
        SELECT @PropertiesCount = COUNT(*)  
        FROM LitigationRequestContract C 
        INNER JOIN LitigationRequest L ON C.Id = L.ContractId
        WHERE 
           C.LitigationRequestContractTypeId = 79
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
    -- Calculate ratios if total litigation is greater than zero
    IF @TotalLitigation > 0
        BEGIN
            SELECT @NotFinishedCountRatio = (@NotFinishedCount * 100.0) / @TotalLitigation;
            SELECT @FinishedCountRatio = (@FinishedCount * 100.0) / @TotalLitigation;
            SELECT @RealEstateBrokerageCountRatio = (@RealEstateBrokerageCount * 100.0) / @TotalLitigation;
            SELECT @WafiCountRatio = (@WafiCount * 100.0) / @TotalLitigation;
            SELECT @OtherServicesCountRatio = (@OtherServicesCount * 100.0) / @TotalLitigation;
            SELECT @RentCountRatio = (@RentCount * 100.0) / @TotalLitigation;
            SELECT @PropertiesCountRatio = (@PropertiesCount * 100.0) / @TotalLitigation;
        END
    -- Output counts and ratios
            SELECT 
                @TotalLitigation AS TotalLitigationCount,
                @NotFinishedCount AS NotFinishedCount,
                @FinishedCount AS FinishedCount,
                @DueDateReconciliationCount AS DueDateReconciliationCount,
                @DueDateArbitrationCount AS DueDateArbitrationCount,
                @RealEstateBrokerageCount AS RealEstateBrokerageCount,
                @WafiCount AS WafiCount,
                @OtherServicesCount AS OtherServicesCount,
                @RentCount AS RentCount,
                @PropertiesCount AS PropertiesCount,
                @NotFinishedCountRatio AS NotFinishedRatio,
                @FinishedCountRatio AS FinishedRatio,
                @RealEstateBrokerageCountRatio AS RealEstateBrokerageRatio,
                @WafiCountRatio AS WafiRatio,
                @OtherServicesCountRatio AS OtherServicesRatio,
                @RentCountRatio AS RentRatio,
                @PropertiesCountRatio AS PropertiesRatio;
        END
