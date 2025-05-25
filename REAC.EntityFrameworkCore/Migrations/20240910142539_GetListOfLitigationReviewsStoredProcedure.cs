using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class GetListOfLitigationReviewsStoredProcedure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*
             * تقرير قائمة الدعاوى في مرحلة التدقيق
             */
            migrationBuilder.Sql($@"   
            
            --تقرير قائمة الدعاوى في مرحلة التدقيق

Create PROCEDURE [dbo].[GetListOfLitigationReviewReport]
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
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Drop PROCEDURE [dbo].[GetListOfLitigationReviewReport]");
        }
    }
}
