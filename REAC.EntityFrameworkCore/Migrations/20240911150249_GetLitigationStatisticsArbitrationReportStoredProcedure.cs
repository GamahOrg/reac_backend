using Microsoft.EntityFrameworkCore.Migrations;
using REAC.Domain.Shared.Enum;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class GetLitigationStatisticsArbitrationReportStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
                --تقرير اعداد الدعاوى فى مرحلة التحكيم               
             CREATE PROCEDURE [dbo].[GetLitigationStatisticsArbitrationReport] 
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
            SET @LitigationRequestCategoryIds=ISNULL(@LitigationRequestCategoryIds, '{(int)LitigationRequestCategory.Arbitration},{(int)LitigationRequestCategory.ReconciliationAndArbitration}');--تحكيم او صلح وتحكيم
    -- Calculate total litigation count with filters
        SELECT @TotalLitigation = COUNT(*)  
        FROM LitigationRequest L        
        WHERE 
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
          AND (L.LitigationRequestStage = '{(int)LitigationRequestStage.Arbitration}')--الدعاوى اللى دخلت التحكيم بالفعل
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);

    -- Calculate finished count
        SELECT @FinishedCount = COUNT(*)  
        FROM LitigationRequest L 
        INNER JOIN LitigationRequest Re ON L.Id = Re.LitigationRequestId
        WHERE 
           L.IsFinished = 1
          AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
          AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
          AND (@LitigationRequestCategoryIds IS NULL OR L.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
          AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
          AND (L.SentOn <= @ToDate)
          AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees);
          
    -- Calculate RealEstateBrokerageCount(وساطة)
        SELECT @RealEstateBrokerageCount = COUNT(*)  
        FROM LitigationRequestContract C 
        INNER JOIN LitigationRequest L ON C.Id = L.ContractId
        WHERE 
           C.LitigationRequestContractTypeId = {(int)LitigationRequestContractType.RealEstateBrokerage}
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
           C.LitigationRequestContractTypeId = {(int)LitigationRequestContractType.Wafi}
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
           C.LitigationRequestContractTypeId = {(int)LitigationRequestContractType.OtherServices}
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
           C.LitigationRequestContractTypeId = {(int)LitigationRequestContractType.Rent}
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
           C.LitigationRequestContractTypeId = {(int)LitigationRequestContractType.Properties}
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
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetLitigationStatisticsArbitrationReport]");

        }
    }
}
