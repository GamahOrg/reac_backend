using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class GetListOfLitigationReformerReportStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
             * تقرير قائمة الدعاوى فى مرحلة الصلح
             */

            migrationBuilder.Sql(@"-- تقرير قائمة الدعاوى فى مرحلة الصلح
                 Create PROCEDURE [dbo].[GetListOfLitigationReformerReport] 
                @EmployeeId NVARCHAR(50) = NULL,
                @LitigationRequestStageId INT = NULL,--(فى اى مرحلة فيها الدعوى (تدقيق/صلح/صلح وتحكيم
                @FromDate DATETIME = NULL,
                @ToDate DATETIME = NULL,
                @AreaIds NVARCHAR(MAX) = NULL,
                @CitiesIds NVARCHAR(MAX) = NULL,
                @LitigationRequestCategoryIds NVARCHAR(MAX) = NULL,--(نوع الدعوى المقدمة(تدقيق/صلح/صلح وتحكيم
                @FromFees DECIMAL(10, 3) = NULL, 
                @ToFees DECIMAL(10, 3) = NULL,
                @ReformerNames NVARCHAR(MAX) = NULL
                AS
                BEGIN           
                    -- Handle default dates if @ToDate is NULL
                    SET @ToDate = ISNULL(@ToDate, GETDATE());    
                -- Get Data
                   select L.Id As LitigationRequestId ,LF.RequestOn as CreatedOn,L.RequestCode as RequestCode,
                    L.ContractAddress as ContractAddress,L.Fees as Fees,
                    A.AreaName as AreaName,C.CityName as CityName ,Slrc.CategoryName,ReformerCit.FullName as RefomerName,
                    S.RequestStatus as RequestStatus,
                     CASE 
                            WHEN F.ArabicFacilityName IS NOT NULL THEN F.ArabicFacilityName 
                            ELSE Cit.FullName 
                        END as FirstParty,   
                        CASE 
                            WHEN SF.ArabicFacilityName IS NOT NULL THEN SF.ArabicFacilityName
                            ELSE SecCit.FullName 
                        END as SecondParty
                        from  LitigationRequestContract LC 
                        inner join LitigationRequest L on L.ContractId =LC.Id
                        inner join SysLitigationRequestStatus S on S.Id=L.ExternalStatusId
                        LEFT join LitigationRequestReformerAssigningRequest LF ON LF.LitigationRequestId = L.Id           
                        LEFT join Facility F on F.Id=LC.FirstPartyFacilityId
                        LEFT join Facility SF on SF.Id=LC.SecondPartyFacilityId
                        LEFT join Citizen Cit on Cit.Id=LC.FirstPartyCitizenId
                        LEFT join Citizen SecCit on SecCit.Id=LC.SecondPartyCitizenId
			            LEFT join Citizen ReformerCit on ReformerCit.Id=L.ReformerCitizenId
                        LEFT join SysArea A on L.AreaId=A.Id
                        LEFT join SysCity C on L.CityId=C.Id
                        LEFT join SysLitigationRequestCategory Slrc on Slrc.Id=L.LitigationRequestCategoryId
                    WHERE            
                       (@LitigationRequestStageId IS NULL OR L.LitigationRequestStage = @LitigationRequestStageId)
                      AND (@AreaIds IS NULL OR L.AreaId IN (SELECT value FROM STRING_SPLIT(@AreaIds, ',')))
                      AND (@CitiesIds IS NULL OR L.CityId IN (SELECT value FROM STRING_SPLIT(@CitiesIds, ',')))
                      AND (@LitigationRequestCategoryIds IS NULL OR l.LitigationRequestCategoryId IN (SELECT value FROM STRING_SPLIT(@LitigationRequestCategoryIds, ',')))
                      AND (@FromDate IS NULL OR L.SentOn >= @FromDate)
                      AND (l.SentOn <= @ToDate)
                      AND (@FromFees IS NULL OR L.Fees BETWEEN @FromFees AND @ToFees)
                      AND (@ReformerNames IS NULL OR ReformerCit.FullName IN (SELECT value FROM STRING_SPLIT(@ReformerNames, ',')));
         End
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetListOfLitigationReformerReport]");
        }
    }
}
