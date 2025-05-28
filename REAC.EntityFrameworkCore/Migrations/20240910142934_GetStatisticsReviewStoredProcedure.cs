using Microsoft.EntityFrameworkCore.Migrations;
using REAC.Domain.Shared.Enum;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class GetStatisticsReviewStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
          * تقرير اعداد الدعاوى فى مرحلة التدقيق            
          */
            migrationBuilder.Sql($@"
                --تقرير اعداد الدعاوى فى مرحلة التدقيق               
Create PROCEDURE [dbo].[GetLitigationStatisticsReviewReport] 
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

            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetLitigationStatisticsReviewReport]");

        }
    }
}
