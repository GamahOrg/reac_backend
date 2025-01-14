using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class InitDatabaseMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LitigationCompanyRepresentatives",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LitigationCompanyRepresentatives", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OtherCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherCategories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysArbitratorAssigningRequestType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ArbitratorAssigningRequestName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysArbitratorAssigningRequest3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysArbitratorClass",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorClassName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    YearsFrom = table.Column<int>(type: "int", nullable: false),
                    YearsTo = table.Column<int>(type: "int", nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysArbitratorClass_3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysArbitratorCount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysArbitratorCount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysArbitratorNominationListCount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorCount = table.Column<int>(type: "int", nullable: false),
                    ArbitratorNominationListCount = table.Column<int>(type: "int", nullable: false),
                    ArbitratorChairmanNominationListCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysArbitratorNominationListCount", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysArbitratorStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysArbitratorStatus4AA46", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysArea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AreaName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysArea_3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysBank",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysBank_3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysCitizenRequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysCitizenRequestStatus__3214EC0715A02324", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysCitizenRequestType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CitizenRequestTypeName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    BpfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysCitizenRequestType__3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysDelegationRelationshipType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    DelegationRelationshipTypeName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysDelegationRelationshipType3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysDepartment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DistributionMode = table.Column<int>(type: "int", nullable: false),
                    DepartmentType = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysDepar__3214EC07035C6297", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysExemptionReductionCategory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExemptionReductionCategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysExemptionReductionCategory_E19C938", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysFileManager",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    FileType = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysFileM__3214EC07904841A6", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysGender",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysGende__3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysIdentityType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    IdentityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysIdent__3214EC0765CD90E5", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestAmount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationAmount = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Fees = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysLitig__3214EC07298413D2", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "صلح – تحكيم-صلح وتحكيم"),
                    AcknowledgmentContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BpfId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysLitig__3214EC072E49D791", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestContractType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ReconciliationPeriod = table.Column<int>(type: "int", nullable: false),
                    CodingPrefixSymbol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcknowledgmentContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysLitig__3214EC074D10EA1A", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestCorrespondenceDestination",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysLitig__3214EC071298CB03", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestExtendPeriodRequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysLitig__3214EC07708342EA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestOwnerType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    LitigationRequestOwnerType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "مقدم عن نفسه – مفوض"),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysLitig__3214EC070BF201CD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysLitigationRequestStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    RequestStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysLitig__3214EC0715A02324", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysMeetingLocation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MeetingLocationName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysMeetingLocation_3214EC0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysMobileOTP",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ExpiryDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysMobileOTP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysNationalities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NationalityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalityCode = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysNationalities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysNotificationActorType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ActorType = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysNotificationActorType3214EC07F533D1CD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysNotificationPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationPlanName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysNotificationPlan__C0743341C99", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysReformerStatus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysReformerStatus4AA46", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysRequestType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    RequestTypeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysRequestType0000C07F533D1CD", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysSetting",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(225)", maxLength: 225, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysSetti__3214EC07BA9C86EA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShowForApplicant = table.Column<bool>(type: "bit", nullable: false),
                    LogMessage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC070B2A996F", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowNotificationDestinationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC070F04AA46", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowNotificationPlan",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationPlan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC070E19C938", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowNotificationType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC07BAAB3A96", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepEventActionMode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ActionMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlowStepEventActionMode__32143A96", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepEventSecurityLevel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SecurityLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlowStepEventSecurityLevel__32143A96", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepEventSecurityLevelActorType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    SecurityLevelActorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlowStepEventSecurityLevelActorType__3214EC07C94B22C3", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepsActorType",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ActorType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlowStepsActorType__3214EC07C94B22C3", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepStage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: true),
                    StageName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC073B964C3D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepWorkMode",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    WorkMode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC079D121DED", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysCity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysCity_3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKSysAreaCity676101",
                        column: x => x.AreaId,
                        principalTable: "SysArea",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SysEmployee",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    AllowAssign = table.Column<bool>(type: "bit", nullable: false),
                    WorkAsEmployee = table.Column<bool>(type: "bit", nullable: false),
                    AllowReAssign = table.Column<bool>(type: "bit", nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "('false')"),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValueSql: "('true')"),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysEmplo__3214EC0760D0BD3B", x => x.Id);
                    table.ForeignKey(
                        name: "FKSysEmploye454468",
                        column: x => x.DepartmentId,
                        principalTable: "SysDepartment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC07F533D1CD", x => x.Id);
                    table.ForeignKey(
                        name: "FKWorkFlowDepartment11186",
                        column: x => x.DepartmentId,
                        principalTable: "SysDepartment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DelegationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityTypeId = table.Column<int>(type: "int", nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    BirthDate = table.Column<DateTime>(type: "date", nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    SourceCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "date", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    DelegationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DelegationFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Delegati__3214EC074548DE5A", x => x.Id);
                    table.ForeignKey(
                        name: "FKDelegation266019",
                        column: x => x.DelegationFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKDelegation632464",
                        column: x => x.IdentityTypeId,
                        principalTable: "SysIdentityType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SysNotificationPlanItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SysNotificationPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmailSubject = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MailTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmsTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicationNotificationTemplate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SysDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysNotificationPlanItem__0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKSysNotificationPlanItemsSysNotificationPlan76101",
                        column: x => x.SysNotificationPlanId,
                        principalTable: "SysNotificationPlan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SysNotificationPlanItem_SysDepartment_SysDepartmentId",
                        column: x => x.SysDepartmentId,
                        principalTable: "SysDepartment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SysNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestTypeId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationActorTypeId = table.Column<int>(type: "int", nullable: false),
                    ActorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsViewed = table.Column<bool>(type: "bit", nullable: false),
                    ViewedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysNotification214EC07F533D1CD", x => x.Id);
                    table.ForeignKey(
                        name: "FKSysNotificationActorTypeSysNotifications231388",
                        column: x => x.NotificationActorTypeId,
                        principalTable: "SysNotificationActorType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSysRequestTypeSysNotifications231388",
                        column: x => x.RequestTypeId,
                        principalTable: "SysRequestType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Citizen",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    NationalityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GenderId = table.Column<int>(type: "int", nullable: true),
                    IdentityExpiryDate = table.Column<DateTime>(type: "date", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IdentityTypeId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NationalAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PassCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdentityFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Employer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HomeTelephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    WorkTelephone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BuildingNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UnitNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    IsLawyer = table.Column<bool>(type: "bit", nullable: true),
                    IsReformer = table.Column<bool>(type: "bit", nullable: true),
                    ReformerStatusId = table.Column<int>(type: "int", nullable: true),
                    IsArbitrator = table.Column<bool>(type: "bit", nullable: true),
                    ArbitratorStatusId = table.Column<int>(type: "int", nullable: true),
                    LawyerLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LawyerLicenseExpirationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LawLicenseFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDraftAccount = table.Column<bool>(type: "bit", nullable: true),
                    LitigationCompanyRepresentativeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Citizen__3214EC0756CF4E03", x => x.Id);
                    table.ForeignKey(
                        name: "FKCSysArbitratorStatusitizen92459",
                        column: x => x.ArbitratorStatusId,
                        principalTable: "SysArbitratorStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCSysReformerStatusCitizen92459",
                        column: x => x.ReformerStatusId,
                        principalTable: "SysReformerStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizen807894",
                        column: x => x.IdentityTypeId,
                        principalTable: "SysIdentityType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizen92411",
                        column: x => x.NationalityId,
                        principalTable: "SysNationalities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizen92422",
                        column: x => x.CityId,
                        principalTable: "SysCity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizen92455",
                        column: x => x.GenderId,
                        principalTable: "SysGender",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizen92459",
                        column: x => x.AreaId,
                        principalTable: "SysArea",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKIndentityCitizens616003",
                        column: x => x.IdentityFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLawLicenseCitizens807894",
                        column: x => x.LawLicenseFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Citizen_LitigationCompanyRepresentatives_LitigationCompanyRepresentativeId",
                        column: x => x.LitigationCompanyRepresentativeId,
                        principalTable: "LitigationCompanyRepresentatives",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SysVacationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedOn = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysVacationRequestEC07276A4089", x => x.Id);
                    table.ForeignKey(
                        name: "FKSysApprovedByEmployee__9495",
                        column: x => x.ApprovedBy,
                        principalTable: "SysEmployee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSysEmployee__09723",
                        column: x => x.EmployeeId,
                        principalTable: "SysEmployee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowNotification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowNotificationPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowNotificationDestinationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowNotificationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotificationMessage = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC07D57D00A2", x => x.Id);
                    table.ForeignKey(
                        name: "FKWorkFlowEv485259",
                        column: x => x.WorkFlowNotificationDestinationTypeId,
                        principalTable: "WorkFlowNotificationDestinationType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowEv659410",
                        column: x => x.EmployeeId,
                        principalTable: "SysEmployee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowEv746150",
                        column: x => x.DepartmentId,
                        principalTable: "SysDepartment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowEv78956",
                        column: x => x.WorkFlowNotificationTypeId,
                        principalTable: "WorkFlowNotificationType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowEv986122",
                        column: x => x.WorkFlowNotificationPlanId,
                        principalTable: "WorkFlowNotificationPlan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStep",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreationLogMessage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    ShowForApplicant = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    ProcessActorType = table.Column<int>(type: "int", nullable: false),
                    ActorTypeId = table.Column<int>(type: "int", nullable: false),
                    WorkModeId = table.Column<int>(type: "int", nullable: false),
                    StageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InternalStatusId = table.Column<int>(type: "int", nullable: true),
                    ExternalStatusId = table.Column<int>(type: "int", nullable: true),
                    CreationNotificationPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC072AD866D7", x => x.Id);
                    table.ForeignKey(
                        name: "FKWorkFlowSt113423",
                        column: x => x.EmployeeId,
                        principalTable: "SysEmployee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowSt200163",
                        column: x => x.DepartmentId,
                        principalTable: "SysDepartment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowSt287452",
                        column: x => x.ActorTypeId,
                        principalTable: "WorkFlowStepsActorType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowSt486458",
                        column: x => x.WorkFlowId,
                        principalTable: "WorkFlow",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowSt648097",
                        column: x => x.WorkModeId,
                        principalTable: "WorkFlowStepWorkMode",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowSt932171",
                        column: x => x.StageId,
                        principalTable: "WorkFlowStepStage",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowStep_WorkFlowNotificationPlan_CreationNotificationPlanId",
                        column: x => x.CreationNotificationPlanId,
                        principalTable: "WorkFlowNotificationPlan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SysNotificationPlanItemDestination",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SysNotificationPlanItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NotificationDestinationType = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    NotificationActorTypeId = table.Column<int>(type: "int", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    SysDepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysNotificationPlanItemDestination__0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKNotificationDestinationTypeSysNotionPlanItemDestinat7610001",
                        column: x => x.NotificationActorTypeId,
                        principalTable: "SysNotificationActorType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSysSysNotificationPlanItemDestinationsPlan7610001",
                        column: x => x.SysNotificationPlanItemId,
                        principalTable: "SysNotificationPlanItem",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SysNotificationPlanItemDestination_SysDepartment_SysDepartmentId",
                        column: x => x.SysDepartmentId,
                        principalTable: "SysDepartment",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArbitratorStopFreeUpDecision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorStatusId = table.Column<int>(type: "int", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoppingReason = table.Column<int>(type: "int", nullable: true),
                    LateLitigationRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArbitratorStopFreeUpDecision_3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenArbitratorStopFreeUpDecision676101",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FKSysArbitratorStatusArbitratorStopFreeUpDecision676101",
                        column: x => x.ArbitratorStatusId,
                        principalTable: "SysArbitratorStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenBankAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountOwnerName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IBan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenBankAccount_3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenCitizenBankAccounts676101",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FKSysBankCitizenBankAccounts676101",
                        column: x => x.BankId,
                        principalTable: "SysBank",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestTypeId = table.Column<int>(type: "int", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicDegreeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsVolunteerApproval = table.Column<bool>(type: "bit", nullable: false),
                    RequestOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExperienceYears = table.Column<int>(type: "int", nullable: true),
                    ArbitratorClassId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Brief = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    StatusId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequest_3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenCitizenRequest676100000001",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizenRequestStatuses676880",
                        column: x => x.StatusId,
                        principalTable: "SysCitizenRequestStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSysArbitratorClassCitizenRequest676101",
                        column: x => x.ArbitratorClassId,
                        principalTable: "SysArbitratorClass",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSysCitizenRequestTypeCitizenRequest676101",
                        column: x => x.CitizenRequestTypeId,
                        principalTable: "SysCitizenRequestType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSysSysAreaCitizenRequest676101",
                        column: x => x.AreaId,
                        principalTable: "SysArea",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Facility",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CommercialRegisterNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Facility700Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ArabicFacilityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EnglishFacilityName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fax = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    District = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Street = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SubNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    BuildingNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    UnitNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Facility__3214EC07AE06877A", x => x.Id);
                    table.ForeignKey(
                        name: "FKFacility36705",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ReformerStopFreeUpDecision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReformerStatusId = table.Column<int>(type: "int", nullable: false),
                    DecisionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StoppingReason = table.Column<int>(type: "int", nullable: true),
                    LateLitigationRequestNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReformerInactiveDecision_3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenReformerStopFreeUpDecision676101",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FKSysReformerStatusReformerStopFreeUpDecision676101",
                        column: x => x.ReformerStatusId,
                        principalTable: "SysReformerStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SysRegistrationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Mobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    CRANumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Facility700Number = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SourceCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestedOn = table.Column<DateTime>(type: "date", nullable: false),
                    IsRegistrationDone = table.Column<bool>(type: "bit", nullable: false),
                    RegistrationDoneOn = table.Column<DateTime>(type: "date", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__SysRegis__3214EC07CCB73928", x => x.Id);
                    table.ForeignKey(
                        name: "FKSysRegistr415781",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSysRegistr676101",
                        column: x => x.SourceCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepEvent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowStepsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CustomActionType = table.Column<int>(type: "int", nullable: true),
                    EventName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LitigationRequestStage = table.Column<int>(type: "int", nullable: true),
                    LitigationRequestSubStage = table.Column<int>(type: "int", nullable: true),
                    AllowFileUpload = table.Column<bool>(type: "bit", nullable: false),
                    AllowNotes = table.Column<bool>(type: "bit", nullable: false),
                    ShowProcessingLogForApplicant = table.Column<bool>(type: "bit", nullable: false),
                    ShowLogForApplicant = table.Column<bool>(type: "bit", nullable: false),
                    LogMessage = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    EventSecurityLevelId = table.Column<int>(type: "int", nullable: false),
                    IsAdminApprovalRequired = table.Column<bool>(type: "bit", nullable: false),
                    AllowChangeInternalStatus = table.Column<bool>(type: "bit", nullable: false),
                    InternalStatusId = table.Column<int>(type: "int", nullable: true),
                    AllowChangeExternalStatus = table.Column<bool>(type: "bit", nullable: false),
                    ExternalStatusId = table.Column<int>(type: "int", nullable: true),
                    SameEmployee = table.Column<bool>(type: "bit", nullable: false),
                    ActionModeId = table.Column<int>(type: "int", nullable: true),
                    NextWorkFlowStepDestinationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllowCompletionNotificationPlan = table.Column<bool>(type: "bit", nullable: false),
                    CompletionNotificationPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AllowChangeIsAllowEdit = table.Column<bool>(type: "bit", nullable: false),
                    IsAllowEditValue = table.Column<bool>(type: "bit", nullable: true),
                    AllowChangeIsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsActiveValue = table.Column<bool>(type: "bit", nullable: true),
                    AllowChangeIsFinished = table.Column<bool>(type: "bit", nullable: false),
                    IsFinishedValue = table.Column<bool>(type: "bit", nullable: true),
                    WorkFlowNotificationPlanId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Translations = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC07C98BCC1F", x => x.Id);
                    table.ForeignKey(
                        name: "FKWorkFlowEv231388",
                        column: x => x.WorkFlowStepsId,
                        principalTable: "WorkFlowStep",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowEv458686",
                        column: x => x.NextWorkFlowStepDestinationId,
                        principalTable: "WorkFlowStep",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowEv60082",
                        column: x => x.CompletionNotificationPlanId,
                        principalTable: "WorkFlowNotificationPlan",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowStepEventEventSecurityLevel231388",
                        column: x => x.EventSecurityLevelId,
                        principalTable: "WorkFlowStepEventSecurityLevel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowStepEventWorkFlowStepEventActionMode231388",
                        column: x => x.ActionModeId,
                        principalTable: "WorkFlowStepEventActionMode",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WorkFlowStepEvent_WorkFlowNotificationPlan_WorkFlowNotificationPlanId",
                        column: x => x.WorkFlowNotificationPlanId,
                        principalTable: "WorkFlowNotificationPlan",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CitizenRequestAcademicDegreeFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicDegreeFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequestAcademicDegreeFile3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "CitizenRequestAcademicDegreeFiles616003",
                        column: x => x.AcademicDegreeFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizenRequestAcademicDegreeFileCitizenRequest676101",
                        column: x => x.CitizenRequestId,
                        principalTable: "CitizenRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenRequestExperienceCertificate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CertificateName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AcquisitionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IssuingEntityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequestExperienceCertificate3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenRequestExperienceCertificateCitizenRequest676101",
                        column: x => x.CitizenRequestId,
                        principalTable: "CitizenRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenRequestFreelanceWorkDocumentFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FreelanceWorkDocumentFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequestFreelanceWorkDocumentFile3214EC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenRequestCitizenRequestFreelanceWorkDocumentFiles01111676101",
                        column: x => x.FreelanceWorkDocumentFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizenRequestFreelanceWorkDocumentFileCitizenRequest676101",
                        column: x => x.CitizenRequestId,
                        principalTable: "CitizenRequest",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CitizenRequestNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    NoteDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CitizenRequestNote_3214EC07F533D1CD", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenRequestCitizenRequestNotes231388",
                        column: x => x.CitizenRequestId,
                        principalTable: "CitizenRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Delegator",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DelegateNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DelegationRelationshipTypeId = table.Column<int>(type: "int", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DelegatorCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DelegationFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Facility__340286D707D66911", x => x.Id);
                    table.ForeignKey(
                        name: "FKCDelegatorSysDelegationRelationshipTypes2455",
                        column: x => x.DelegationRelationshipTypeId,
                        principalTable: "SysDelegationRelationshipType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKDelegatorCitizenToDelegators18143",
                        column: x => x.DelegatorCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKDelegatorFromDelegators8143",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKDelegatorsFilesSystemss18143",
                        column: x => x.DelegationFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKFacilityDelegators200220",
                        column: x => x.FacilityId,
                        principalTable: "Facility",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestContract",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestContractTypeId = table.Column<int>(type: "int", nullable: false),
                    ContractKey = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    VersionNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    AssociationName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContractFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AssociationRegistrationCertificateFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FoundingLetterFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InternalRegulationsFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstPartyCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstPartyFacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FirstPartyCapacity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecondPartyCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SecondPartyFacilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SecondPartyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondPartyCapacity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SecondPartyIdentityNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecondPartyEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SecondPartyMobile = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    SecondPartyRegistrationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC079F19E3F8", x => x.Id);
                    table.ForeignKey(
                        name: "FKAssociationRegistrationCertificateFile807894",
                        column: x => x.AssociationRegistrationCertificateFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKFoundingLetterFile807894",
                        column: x => x.FoundingLetterFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKInternalRegulationsFile807894",
                        column: x => x.InternalRegulationsFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation253862",
                        column: x => x.FirstPartyCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation263926",
                        column: x => x.FirstPartyFacilityId,
                        principalTable: "Facility",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation28965",
                        column: x => x.SecondPartyCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation945532",
                        column: x => x.ContractFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation986672",
                        column: x => x.SecondPartyFacilityId,
                        principalTable: "Facility",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation989644",
                        column: x => x.LitigationRequestContractTypeId,
                        principalTable: "SysLitigationRequestContractType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKSecondPartyLitigation238798",
                        column: x => x.SecondPartyRegistrationRequestId,
                        principalTable: "SysRegistrationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowStepEventSecurityLevelActor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowStepEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityLevelActorId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlowStepEventSecurityLevelActor__3214EC072AD866D7", x => x.Id);
                    table.ForeignKey(
                        name: "FKWorkFlowStepEventSecurityLevelActorDepartment200163",
                        column: x => x.DepartmentId,
                        principalTable: "SysDepartment",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowStepEventSecurityLevelActorEmployeeReferenceSt113423",
                        column: x => x.EmployeeId,
                        principalTable: "SysEmployee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowStepEventSecurityLevelActorType287452",
                        column: x => x.SecurityLevelActorId,
                        principalTable: "WorkFlowStepEventSecurityLevelActorType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowStepEventSecurityLevelActorsWorkFlowStepEvent486458",
                        column: x => x.WorkFlowStepEventId,
                        principalTable: "WorkFlowStepEvent",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CitizenRequestExperienceCertificateFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenRequestExperienceCertificateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExperienceCertificateFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CitizenRequestExperienceCertificateFileEC0743341C99", x => x.Id);
                    table.ForeignKey(
                        name: "FKCitizenRequestCitizenRequestExperienceCertificateFile000676101",
                        column: x => x.ExperienceCertificateFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKCitizenRequestExperienceCertificateFileCitizenRequestExperienceCertificate676101",
                        column: x => x.CitizenRequestExperienceCertificateId,
                        principalTable: "CitizenRequestExperienceCertificate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    LitigationRequestStage = table.Column<int>(type: "int", nullable: true),
                    LitigationRequestSubStage = table.Column<int>(type: "int", nullable: true),
                    ContractId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AreaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ContractAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApplicantCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DelegatorCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OtherCategories = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsReminderForReviewerSent = table.Column<bool>(type: "bit", nullable: true),
                    IsReformerReminderForReconciliationFirstPeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorCountSelectionResponsePeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorCountSelectionDecisionPeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorNominationListResponsePeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorNominationListDecisionPeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorChairmanNominationListResponsePeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorChairmanNominationListDecisionPeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorAssigningRequestResponseFirstPeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsArbitratorAssigningRequestResponseSecondPeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsLitigationRequestReconciliationReportResponsePeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsLitigationRequestReformerAssigningRequestResponsePeriodSent = table.Column<bool>(type: "bit", nullable: true),
                    IsReminderForReviewerManagerSent = table.Column<bool>(type: "bit", nullable: true),
                    IsReminderForApplicantSent = table.Column<bool>(type: "bit", nullable: true),
                    IsReminderForAgreementRequestForRequestSent = table.Column<bool>(type: "bit", nullable: true),
                    IsAllowEdit = table.Column<bool>(type: "bit", nullable: false),
                    FirstPartyArbitratorsCount = table.Column<int>(type: "int", nullable: true),
                    SecondPartyArbitratorsCount = table.Column<int>(type: "int", nullable: true),
                    ArbitratorsCount = table.Column<int>(type: "int", nullable: true),
                    ReformerCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ArbitratorCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LitigationRequestCategoryId = table.Column<int>(type: "int", nullable: true),
                    LitigationRequestOwnerTypeId = table.Column<int>(type: "int", nullable: false),
                    IsMinutesOfMeetingApprovalRequired = table.Column<bool>(type: "bit", nullable: true),
                    IsPlaintiffArbitratorNominationListDone = table.Column<bool>(type: "bit", nullable: true),
                    IsDefendantArbitratorNominationListDone = table.Column<bool>(type: "bit", nullable: true),
                    IsAgreementRequestRequired = table.Column<bool>(type: "bit", nullable: true),
                    AgreementRequestRequiredOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlaintiffAgreementRequestResponse = table.Column<int>(type: "int", nullable: true),
                    PlaintiffAgreementRequestResponseOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DefendantAgreementRequestResponse = table.Column<int>(type: "int", nullable: true),
                    DefendantAgreementRequestResponseOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MintuesOfMeetingApprovalPlaintiffStatus = table.Column<int>(type: "int", nullable: true),
                    MintuesOfMeetingApprovalDefendantStatus = table.Column<int>(type: "int", nullable: true),
                    MintuesOfMeetingApprovalPlaintiffStatusOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MintuesOfMeetingApprovalDefendantStatusOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InternalStatusId = table.Column<int>(type: "int", nullable: false),
                    ExternalStatusId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinancialLitigation = table.Column<bool>(type: "bit", nullable: true),
                    LitigationRequestAmountId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Fees = table.Column<decimal>(type: "money", nullable: false),
                    ReconciliationPeriod = table.Column<int>(type: "int", nullable: true),
                    ArbitrationPeriod = table.Column<int>(type: "int", nullable: true),
                    StartReconciliationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndReconciliationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartArbitrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndArbitrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsPaid = table.Column<bool>(type: "bit", nullable: false),
                    PaidOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false),
                    SentOn = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    IsFinished = table.Column<bool>(type: "bit", nullable: true),
                    IsExemptionReduction = table.Column<bool>(type: "bit", nullable: true),
                    ArbitratorSelectionDuration = table.Column<int>(type: "int", nullable: true),
                    ExemptionReductionCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExemptionReductionFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PretenderRating = table.Column<int>(type: "int", nullable: true),
                    DefendantRating = table.Column<int>(type: "int", nullable: true),
                    ArbitratorsSelectionDurationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArbitratorsCountDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArbitratorsNominationListCreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArbitratorsNominationListSelectionDoneDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArbitratorsChairmanNominationListCreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ArbitratorsChairmanNominationListSelectionDoneDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    JudgmentDecisionIssuingDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsArbitratorsChairmanDecisionRequired = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC077962DFCA", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitratorCitizenLitigation638578",
                        column: x => x.ReformerCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKExemptionReductionCategory580725",
                        column: x => x.ExemptionReductionCategoryId,
                        principalTable: "SysExemptionReductionCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKExemptionReductionCategoryFilesExemptionReductionFiles01176101",
                        column: x => x.ExemptionReductionFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation216575",
                        column: x => x.ContractId,
                        principalTable: "LitigationRequestContract",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation446010",
                        column: x => x.LitigationRequestCategoryId,
                        principalTable: "SysLitigationRequestCategory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation464519",
                        column: x => x.LitigationRequestAmountId,
                        principalTable: "SysLitigationRequestAmount",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation580725",
                        column: x => x.LitigationRequestOwnerTypeId,
                        principalTable: "SysLitigationRequestOwnerType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation638578",
                        column: x => x.ApplicantCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation676880",
                        column: x => x.ExternalStatusId,
                        principalTable: "SysLitigationRequestStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation6DelegatorLitigationRequests38578",
                        column: x => x.DelegatorCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation891328",
                        column: x => x.InternalStatusId,
                        principalTable: "SysLitigationRequestStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKOtherCategory580725",
                        column: x => x.OtherCategories,
                        principalTable: "OtherCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReformerCitizenLitigation638578",
                        column: x => x.ArbitratorCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LitigationRequest_SysArea_AreaId",
                        column: x => x.AreaId,
                        principalTable: "SysArea",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LitigationRequest_SysCity_CityId",
                        column: x => x.CityId,
                        principalTable: "SysCity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitrationReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JudgmentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Facts = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    Reasons = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    JudgmentDecisionText = table.Column<string>(type: "nvarchar(max)", maxLength: 5000, nullable: false),
                    ModificationRequest = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    JudgmentDecisionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    JudgmentDecisionPlace = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MeetingLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitrationReport_3214EC07F3A8E5CA", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigationLitigationRequestArbitrationReport334620",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKMeetingLocationRequestArbitrationReport33000000004620",
                        column: x => x.MeetingLocationId,
                        principalTable: "SysMeetingLocation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitrator",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitrator07F533D1CD", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitratorCitizenRequestArbitrators231388345000",
                        column: x => x.ArbitratorCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequestLitigationRequestArbitrators231388",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitratorNominationList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlaintiffPriority = table.Column<int>(type: "int", nullable: true),
                    DefendantPriority = table.Column<int>(type: "int", nullable: true),
                    TotalPriority = table.Column<int>(type: "int", nullable: true),
                    IsSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitratorNominationList07F533D1CD", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitratorCitizenRequestArbitrators231388ccc000",
                        column: x => x.ArbitratorCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequestLitigationRequestArbitratorNominationList231388",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitratorRecusalRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestParty = table.Column<int>(type: "int", nullable: false),
                    ApproveRejectStatus = table.Column<int>(type: "int", nullable: true),
                    ResponseOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitratorRecusalRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitratorCitizenLitigationRequestArbitratorRecusalRequests231388ccc000",
                        column: x => x.ArbitratorCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequestLitigationRequestArbitratorRecusalRequests231388ccc000",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitratorsChairmanNominationList",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorChairmanCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalPriority = table.Column<int>(type: "int", nullable: true),
                    IsSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitratorsChairmanNominationList07F533D1CD", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitratorLitigationRequestArbitratorsChairmanNominationLists000",
                        column: x => x.ArbitratorChairmanCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequesLitigationRequestArbitrChairmanNominationList1388",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestCorrespondence",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestCorrespondenceDestinationId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07868BEDDE", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation401380",
                        column: x => x.LitigationRequestCorrespondenceDestinationId,
                        principalTable: "SysLitigationRequestCorrespondenceDestination",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation887015",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestExtendPeriodRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestExtendPeriodRequestTypeId = table.Column<int>(type: "int", nullable: false),
                    RequiredDays = table.Column<int>(type: "int", nullable: false),
                    RequestReason = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LitigationRequestExtendPeriodRequestStatusId = table.Column<int>(type: "int", nullable: true),
                    FinishStatusReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SysEmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07276A4089", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation194598",
                        column: x => x.LitigationRequestExtendPeriodRequestStatusId,
                        principalTable: "SysLitigationRequestExtendPeriodRequestStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation809723",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LitigationRequestExtendPeriodRequest_SysEmployee_SysEmployeeId",
                        column: x => x.SysEmployeeId,
                        principalTable: "SysEmployee",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestGround",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07B596385A", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation62808",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestNote",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07C1F98E04", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation535233",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LitigationRequestNote_Citizen_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestReconciliationReport",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    ReconciliationSessionDetails = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ModificationRequest = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07F3A8E5CA", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigationLitigationRequestReconciliationReport334620",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestReformerAssigningRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Response = table.Column<int>(type: "int", nullable: false),
                    ResponseOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestReformerAssigningRequest4EC07AE06877A", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigationRequestLitigationRequestReformerAssigningRequest705",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKReformerCitizenLitigationRequestReformerAssigningRequest705",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestContent = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC0751C3A7B1", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation674990",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestSession",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SessionDatetime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LitigationRequestSessionRequestTypeId = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SessionUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestSessionC1F98E04", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigationLitigationRequestSessions535233",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowProcessingLog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmployeeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowStepEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceWorkFlowProcessingLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WorkFlowStepId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsWithNotes = table.Column<bool>(type: "bit", nullable: false),
                    ShowForApplicant = table.Column<bool>(type: "bit", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFinished = table.Column<bool>(type: "bit", nullable: false),
                    FinishedOn = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC079C2E52E3", x => x.Id);
                    table.ForeignKey(
                        name: "FKWorkDestination_SourceWorkFlowProcessingLogPr467617",
                        column: x => x.SourceWorkFlowProcessingLogId,
                        principalTable: "WorkFlowProcessingLog",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowPr231724",
                        column: x => x.EmployeeId,
                        principalTable: "SysEmployee",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowPr467617",
                        column: x => x.WorkFlowStepId,
                        principalTable: "WorkFlowStep",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowPr909055",
                        column: x => x.WorkFlowStepEventId,
                        principalTable: "WorkFlowStepEvent",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowPrWorkFlowProcessingLog231724",
                        column: x => x.RequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitrationReportExplanationRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExplanationRequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LitigationRequestArbitrationReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestArbitrationReportExplanationRequestParty = table.Column<int>(type: "int", nullable: false),
                    ExplanationRequests = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Response = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResponseOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitrationReportExplanationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigationRequestArbitrationReportExplanationRequests231388ccc000",
                        column: x => x.LitigationRequestArbitrationReportId,
                        principalTable: "LitigationRequestArbitrationReport",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitratorAssigningRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LitigationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorAssigningRequestTypeId = table.Column<int>(type: "int", nullable: false),
                    LitigationRequestArbitratorNominationListId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Response = table.Column<int>(type: "int", nullable: false),
                    ResponseOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitratorAssigningRequest4EC07AE06877A", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitratorAssigningRequestIdArbitratorAssigningRequest705",
                        column: x => x.ArbitratorAssigningRequestTypeId,
                        principalTable: "SysArbitratorAssigningRequestType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKArbitratorCitizenLitigationRequestArbitratorAssigningRequest705",
                        column: x => x.CitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequestArbitratorNominationListAssigningRequest70502",
                        column: x => x.LitigationRequestArbitratorNominationListId,
                        principalTable: "LitigationRequestArbitratorNominationList",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequestLitigationRequestArbitratorAssigningRequest705",
                        column: x => x.LitigationRequestId,
                        principalTable: "LitigationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitratorRecusalRequestFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestArbitratorRecusalRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecusalRequestFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitratorRecusalRequestFile", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigationRequestArbitratorRecusalRequesArbitratorRecusalRequestFiles2313000",
                        column: x => x.LitigationRequestArbitratorRecusalRequestId,
                        principalTable: "LitigationRequestArbitratorRecusalRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKRecusalRequestFileRequestArbitratorRecusalRequestsFile2313885888000",
                        column: x => x.RecusalRequestFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitratorsChairmanNominationListValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestArbitratorsChairmanNominationListId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorChairmanCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitratorCitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    IsSent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitratorsChairmanNominationListValue07F533D1CD", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitratorLitigationRequestArbitratorsChairmanNominationListArbitratorValues000",
                        column: x => x.ArbitratorCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKArbitratorLitigationRequestArbitratorsChairmanNominationListChairmanValues000",
                        column: x => x.ArbitratorChairmanCitizenId,
                        principalTable: "Citizen",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequesLitigationRequestArbitrChairmanNominationListValues011111112581388",
                        column: x => x.LitigationRequestArbitratorsChairmanNominationListId,
                        principalTable: "LitigationRequestArbitratorsChairmanNominationList",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestCorrespondenceFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestCorrespondenceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07B506B090", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation804409",
                        column: x => x.LitigationRequestCorrespondenceId,
                        principalTable: "LitigationRequestCorrespondence",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation81269",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestExtendPeriodRequestFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestExtendPeriodRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC076F56C6AF", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation662845",
                        column: x => x.LitigationRequestExtendPeriodRequestId,
                        principalTable: "LitigationRequestExtendPeriodRequest",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation788993",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestGroundFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestGroundId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07E98400B7", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation488105",
                        column: x => x.LitigationRequestGroundId,
                        principalTable: "LitigationRequestGround",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation81857",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestNoteFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC070E49794B", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation888272",
                        column: x => x.NoteId,
                        principalTable: "LitigationRequestNote",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation910435",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestMeetingReconciliationItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestReconciliationReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReconciliationItem = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC07A8D6A249", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation688593",
                        column: x => x.LitigationRequestReconciliationReportId,
                        principalTable: "LitigationRequestReconciliationReport",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestRequestFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LitigationRequestRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileManagerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Litigati__3214EC076FBEF9E2", x => x.Id);
                    table.ForeignKey(
                        name: "FKLitigation620435",
                        column: x => x.FileManagerId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigation662656",
                        column: x => x.LitigationRequestRequestId,
                        principalTable: "LitigationRequestRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkFlowProcessingLogFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkFlowProcessingLogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__WorkFlow__3214EC076A5BB1A2", x => x.Id);
                    table.ForeignKey(
                        name: "FKFileManagerCitizenRequestAcademicDegreeFiles616003",
                        column: x => x.FileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKWorkFlowPr653271",
                        column: x => x.WorkFlowProcessingLogId,
                        principalTable: "WorkFlowProcessingLog",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LitigationRequestArbitrationReportExplanationRequestFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArbitrationReportExplanationRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExplanationRequestFileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LitigationRequestArbitrationReportExplanationRequestFile", x => x.Id);
                    table.ForeignKey(
                        name: "FKArbitrationReportExplanationRequestFileRequestFile3800088000",
                        column: x => x.ExplanationRequestFileId,
                        principalTable: "SysFileManager",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FKLitigationRequestArbitrationR2313800088c000",
                        column: x => x.ArbitrationReportExplanationRequestId,
                        principalTable: "LitigationRequestArbitrationReportExplanationRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArbitratorStopFreeUpDecision_ArbitratorStatusId",
                table: "ArbitratorStopFreeUpDecision",
                column: "ArbitratorStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ArbitratorStopFreeUpDecision_CitizenId",
                table: "ArbitratorStopFreeUpDecision",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_ArbitratorStatusId",
                table: "Citizen",
                column: "ArbitratorStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_AreaId",
                table: "Citizen",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_CityId",
                table: "Citizen",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_GenderId",
                table: "Citizen",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_IdentityFileId",
                table: "Citizen",
                column: "IdentityFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_IdentityTypeId",
                table: "Citizen",
                column: "IdentityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_LawLicenseFileId",
                table: "Citizen",
                column: "LawLicenseFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_LitigationCompanyRepresentativeId",
                table: "Citizen",
                column: "LitigationCompanyRepresentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_NationalityId",
                table: "Citizen",
                column: "NationalityId");

            migrationBuilder.CreateIndex(
                name: "IX_Citizen_ReformerStatusId",
                table: "Citizen",
                column: "ReformerStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenBankAccount_BankId",
                table: "CitizenBankAccount",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenBankAccount_CitizenId",
                table: "CitizenBankAccount",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequest_ArbitratorClassId",
                table: "CitizenRequest",
                column: "ArbitratorClassId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequest_AreaId",
                table: "CitizenRequest",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequest_CitizenId",
                table: "CitizenRequest",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequest_CitizenRequestTypeId",
                table: "CitizenRequest",
                column: "CitizenRequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequest_StatusId",
                table: "CitizenRequest",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestAcademicDegreeFile_AcademicDegreeFileId",
                table: "CitizenRequestAcademicDegreeFile",
                column: "AcademicDegreeFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestAcademicDegreeFile_CitizenRequestId",
                table: "CitizenRequestAcademicDegreeFile",
                column: "CitizenRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestExperienceCertificate_CitizenRequestId",
                table: "CitizenRequestExperienceCertificate",
                column: "CitizenRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestExperienceCertificateFile_CitizenRequestExperienceCertificateId",
                table: "CitizenRequestExperienceCertificateFile",
                column: "CitizenRequestExperienceCertificateId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestExperienceCertificateFile_ExperienceCertificateFileId",
                table: "CitizenRequestExperienceCertificateFile",
                column: "ExperienceCertificateFileId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestFreelanceWorkDocumentFile_CitizenRequestId",
                table: "CitizenRequestFreelanceWorkDocumentFile",
                column: "CitizenRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestFreelanceWorkDocumentFile_FreelanceWorkDocumentFileId",
                table: "CitizenRequestFreelanceWorkDocumentFile",
                column: "FreelanceWorkDocumentFileId");

            migrationBuilder.CreateIndex(
                name: "CitizenRequestNote_Id",
                table: "CitizenRequestNote",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CitizenRequestNote_CitizenRequestId",
                table: "CitizenRequestNote",
                column: "CitizenRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_DelegationFileId",
                table: "DelegationRequest",
                column: "DelegationFileId");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationRequest_IdentityTypeId",
                table: "DelegationRequest",
                column: "IdentityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_CitizenId",
                table: "Delegator",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_DelegationFileId",
                table: "Delegator",
                column: "DelegationFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_DelegationRelationshipTypeId",
                table: "Delegator",
                column: "DelegationRelationshipTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_DelegatorCitizenId",
                table: "Delegator",
                column: "DelegatorCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_Delegator_FacilityId",
                table: "Delegator",
                column: "FacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_Facility_CitizenId",
                table: "Facility",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "UQ__Facility__E3FD678860036876",
                table: "Facility",
                column: "Facility700Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ApplicantCitizenId",
                table: "LitigationRequest",
                column: "ApplicantCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ArbitratorCitizenId",
                table: "LitigationRequest",
                column: "ArbitratorCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_AreaId",
                table: "LitigationRequest",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_CityId",
                table: "LitigationRequest",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ContractId",
                table: "LitigationRequest",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_DelegatorCitizenId",
                table: "LitigationRequest",
                column: "DelegatorCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ExemptionReductionCategoryId",
                table: "LitigationRequest",
                column: "ExemptionReductionCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ExemptionReductionFileId",
                table: "LitigationRequest",
                column: "ExemptionReductionFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ExternalStatusId",
                table: "LitigationRequest",
                column: "ExternalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_InternalStatusId",
                table: "LitigationRequest",
                column: "InternalStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_LitigationRequestAmountId",
                table: "LitigationRequest",
                column: "LitigationRequestAmountId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_LitigationRequestCategoryId",
                table: "LitigationRequest",
                column: "LitigationRequestCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_LitigationRequestOwnerTypeId",
                table: "LitigationRequest",
                column: "LitigationRequestOwnerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_OtherCategories",
                table: "LitigationRequest",
                column: "OtherCategories");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequest_ReformerCitizenId",
                table: "LitigationRequest",
                column: "ReformerCitizenId");

            migrationBuilder.CreateIndex(
                name: "UQ__Litigati__CBAB82F68D5B9E0C",
                table: "LitigationRequest",
                column: "RequestCode",
                unique: true,
                filter: "[RequestCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitrationReport_LitigationRequestId",
                table: "LitigationRequestArbitrationReport",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitrationReport_MeetingLocationId",
                table: "LitigationRequestArbitrationReport",
                column: "MeetingLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitrationReportExplanationRequest_LitigationRequestArbitrationReportId",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                column: "LitigationRequestArbitrationReportId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbitrationReportExplanationRequest_Id",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitrationReportExplanationRequestFile_ArbitrationReportExplanationRequestId",
                table: "LitigationRequestArbitrationReportExplanationRequestFile",
                column: "ArbitrationReportExplanationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitrationReportExplanationRequestFile_ExplanationRequestFileId",
                table: "LitigationRequestArbitrationReportExplanationRequestFile",
                column: "ExplanationRequestFileId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbonRequestFile_Id",
                table: "LitigationRequestArbitrationReportExplanationRequestFile",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitrator_ArbitratorCitizenId",
                table: "LitigationRequestArbitrator",
                column: "ArbitratorCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitrator_LitigationRequestId",
                table: "LitigationRequestArbitrator",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbitrator_Id",
                table: "LitigationRequestArbitrator",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorAssigningRequest_ArbitratorAssigningRequestTypeId",
                table: "LitigationRequestArbitratorAssigningRequest",
                column: "ArbitratorAssigningRequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorAssigningRequest_CitizenId",
                table: "LitigationRequestArbitratorAssigningRequest",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorAssigningRequest_LitigationRequestArbitratorNominationListId",
                table: "LitigationRequestArbitratorAssigningRequest",
                column: "LitigationRequestArbitratorNominationListId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorAssigningRequest_LitigationRequestId",
                table: "LitigationRequestArbitratorAssigningRequest",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorNominationList_ArbitratorCitizenId",
                table: "LitigationRequestArbitratorNominationList",
                column: "ArbitratorCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorNominationList_LitigationRequestId",
                table: "LitigationRequestArbitratorNominationList",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbitratorNominationList_Id",
                table: "LitigationRequestArbitratorNominationList",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorRecusalRequest_ArbitratorCitizenId",
                table: "LitigationRequestArbitratorRecusalRequest",
                column: "ArbitratorCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorRecusalRequest_LitigationRequestId",
                table: "LitigationRequestArbitratorRecusalRequest",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbitratorRecusalRequest_Id",
                table: "LitigationRequestArbitratorRecusalRequest",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorRecusalRequestFile_LitigationRequestArbitratorRecusalRequestId",
                table: "LitigationRequestArbitratorRecusalRequestFile",
                column: "LitigationRequestArbitratorRecusalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorRecusalRequestFile_RecusalRequestFileId",
                table: "LitigationRequestArbitratorRecusalRequestFile",
                column: "RecusalRequestFileId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbitratorRecusalRequestFile_Id",
                table: "LitigationRequestArbitratorRecusalRequestFile",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorsChairmanNominationList_ArbitratorChairmanCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                column: "ArbitratorChairmanCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorsChairmanNominationList_LitigationRequestId",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbitratorsChairmanNominationList_Id",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorsChairmanNominationListValue_ArbitratorChairmanCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                column: "ArbitratorChairmanCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorsChairmanNominationListValue_ArbitratorCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                column: "ArbitratorCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestArbitratorsChairmanNominationListValue_LitigationRequestArbitratorsChairmanNominationListId",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                column: "LitigationRequestArbitratorsChairmanNominationListId");

            migrationBuilder.CreateIndex(
                name: "LitigationRequestArbitratorsChairmanNominationListValue_Id",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_AssociationRegistrationCertificateFileId",
                table: "LitigationRequestContract",
                column: "AssociationRegistrationCertificateFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_ContractFileId",
                table: "LitigationRequestContract",
                column: "ContractFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_FirstPartyCitizenId",
                table: "LitigationRequestContract",
                column: "FirstPartyCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_FirstPartyFacilityId",
                table: "LitigationRequestContract",
                column: "FirstPartyFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_FoundingLetterFileId",
                table: "LitigationRequestContract",
                column: "FoundingLetterFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_InternalRegulationsFileId",
                table: "LitigationRequestContract",
                column: "InternalRegulationsFileId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_LitigationRequestContractTypeId",
                table: "LitigationRequestContract",
                column: "LitigationRequestContractTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_SecondPartyCitizenId",
                table: "LitigationRequestContract",
                column: "SecondPartyCitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_SecondPartyFacilityId",
                table: "LitigationRequestContract",
                column: "SecondPartyFacilityId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestContract_SecondPartyRegistrationRequestId",
                table: "LitigationRequestContract",
                column: "SecondPartyRegistrationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondence_LitigationRequestCorrespondenceDestinationId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestCorrespondenceDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondence_LitigationRequestId",
                table: "LitigationRequestCorrespondence",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondenceFile_FileManagerId",
                table: "LitigationRequestCorrespondenceFile",
                column: "FileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestCorrespondenceFile_LitigationRequestCorrespondenceId",
                table: "LitigationRequestCorrespondenceFile",
                column: "LitigationRequestCorrespondenceId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestExtendPeriodRequest_LitigationRequestExtendPeriodRequestStatusId",
                table: "LitigationRequestExtendPeriodRequest",
                column: "LitigationRequestExtendPeriodRequestStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestExtendPeriodRequest_LitigationRequestId",
                table: "LitigationRequestExtendPeriodRequest",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestExtendPeriodRequest_SysEmployeeId",
                table: "LitigationRequestExtendPeriodRequest",
                column: "SysEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestExtendPeriodRequestFile_FileManagerId",
                table: "LitigationRequestExtendPeriodRequestFile",
                column: "FileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestExtendPeriodRequestFile_LitigationRequestExtendPeriodRequestId",
                table: "LitigationRequestExtendPeriodRequestFile",
                column: "LitigationRequestExtendPeriodRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestGround_LitigationRequestId",
                table: "LitigationRequestGround",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestGroundFile_FileManagerId",
                table: "LitigationRequestGroundFile",
                column: "FileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestGroundFile_LitigationRequestGroundId",
                table: "LitigationRequestGroundFile",
                column: "LitigationRequestGroundId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestMeetingReconciliationItem_LitigationRequestReconciliationReportId",
                table: "LitigationRequestMeetingReconciliationItem",
                column: "LitigationRequestReconciliationReportId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestNote_CitizenId",
                table: "LitigationRequestNote",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestNote_LitigationRequestId",
                table: "LitigationRequestNote",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestNoteFile_FileManagerId",
                table: "LitigationRequestNoteFile",
                column: "FileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestNoteFile_NoteId",
                table: "LitigationRequestNoteFile",
                column: "NoteId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestReconciliationReport_LitigationRequestId",
                table: "LitigationRequestReconciliationReport",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestReformerAssigningRequest_CitizenId",
                table: "LitigationRequestReformerAssigningRequest",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestReformerAssigningRequest_LitigationRequestId",
                table: "LitigationRequestReformerAssigningRequest",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestRequest_LitigationRequestId",
                table: "LitigationRequestRequest",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestRequestFile_FileManagerId",
                table: "LitigationRequestRequestFile",
                column: "FileManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestRequestFile_LitigationRequestRequestId",
                table: "LitigationRequestRequestFile",
                column: "LitigationRequestRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_LitigationRequestSession_LitigationRequestId",
                table: "LitigationRequestSession",
                column: "LitigationRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ReformerStopFreeUpDecision_CitizenId",
                table: "ReformerStopFreeUpDecision",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_ReformerStopFreeUpDecision_ReformerStatusId",
                table: "ReformerStopFreeUpDecision",
                column: "ReformerStatusId");

            migrationBuilder.CreateIndex(
                name: "SysArbitratorCount_Id",
                table: "SysArbitratorCount",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "SysArbitratorNominationListCount_Id",
                table: "SysArbitratorNominationListCount",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "SysArbitratorStatus_Id",
                table: "SysArbitratorStatus",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysCity_AreaId",
                table: "SysCity",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_SysEmployee_DepartmentId",
                table: "SysEmployee",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "SysExemptionReductionCategory_Id",
                table: "SysExemptionReductionCategory",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SysLitig__1931ED8347FF283F",
                table: "SysLitigationRequestAmount",
                column: "LitigationAmount",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SysLitig__D4E7DFA8A7F5E85A",
                table: "SysLitigationRequestContractType",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SysLitig__06968C68F7DD93E4",
                table: "SysLitigationRequestCorrespondenceDestination",
                column: "Destination",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SysLitig__05E7698A4ABD62EC",
                table: "SysLitigationRequestExtendPeriodRequestStatus",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysNotification_NotificationActorTypeId",
                table: "SysNotification",
                column: "NotificationActorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysNotification_RequestTypeId",
                table: "SysNotification",
                column: "RequestTypeId");

            migrationBuilder.CreateIndex(
                name: "SysNotification_Id",
                table: "SysNotification",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "SysNotificationActorType_Id",
                table: "SysNotificationActorType",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysNotificationPlanItem_SysDepartmentId",
                table: "SysNotificationPlanItem",
                column: "SysDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SysNotificationPlanItem_SysNotificationPlanId",
                table: "SysNotificationPlanItem",
                column: "SysNotificationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_SysNotificationPlanItemDestination_NotificationActorTypeId",
                table: "SysNotificationPlanItemDestination",
                column: "NotificationActorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SysNotificationPlanItemDestination_SysDepartmentId",
                table: "SysNotificationPlanItemDestination",
                column: "SysDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_SysNotificationPlanItemDestination_SysNotificationPlanItemId",
                table: "SysNotificationPlanItemDestination",
                column: "SysNotificationPlanItemId");

            migrationBuilder.CreateIndex(
                name: "SysReformerStatus_Id",
                table: "SysReformerStatus",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysRegistrationRequest_CitizenId",
                table: "SysRegistrationRequest",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_SysRegistrationRequest_SourceCitizenId",
                table: "SysRegistrationRequest",
                column: "SourceCitizenId");

            migrationBuilder.CreateIndex(
                name: "SysRequestType_Id",
                table: "SysRequestType",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__SysSetti__C41E0289172F62B6",
                table: "SysSetting",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SysVacationRequest_ApprovedBy",
                table: "SysVacationRequest",
                column: "ApprovedBy");

            migrationBuilder.CreateIndex(
                name: "IX_SysVacationRequest_EmployeeId",
                table: "SysVacationRequest",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "SysVacationRequest_Id",
                table: "SysVacationRequest",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlow_DepartmentId",
                table: "WorkFlow",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlow__44466DF02F20F5FC",
                table: "WorkFlow",
                column: "WorkFlowName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlow_Id",
                table: "WorkFlow",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowLog_Id",
                table: "WorkFlowLog",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowNotification_DepartmentId",
                table: "WorkFlowNotification",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowNotification_EmployeeId",
                table: "WorkFlowNotification",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowNotification_WorkFlowNotificationDestinationTypeId",
                table: "WorkFlowNotification",
                column: "WorkFlowNotificationDestinationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowNotification_WorkFlowNotificationPlanId",
                table: "WorkFlowNotification",
                column: "WorkFlowNotificationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowNotification_WorkFlowNotificationTypeId",
                table: "WorkFlowNotification",
                column: "WorkFlowNotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "WorkFlowNotification_Id",
                table: "WorkFlowNotification",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlow__2798CCB6BCC465AF",
                table: "WorkFlowNotificationDestinationType",
                column: "DestinationType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowNotificationDestinationType_Id",
                table: "WorkFlowNotificationDestinationType",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowNotificationPlan_Id",
                table: "WorkFlowNotificationPlan",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlow__591971B00EEC9096",
                table: "WorkFlowNotificationType",
                column: "NotificationType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowNotificationType_Id",
                table: "WorkFlowNotificationType",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowProcessingLog_EmployeeId",
                table: "WorkFlowProcessingLog",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowProcessingLog_RequestId",
                table: "WorkFlowProcessingLog",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowProcessingLog_SourceWorkFlowProcessingLogId",
                table: "WorkFlowProcessingLog",
                column: "SourceWorkFlowProcessingLogId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowProcessingLog_WorkFlowStepEventId",
                table: "WorkFlowProcessingLog",
                column: "WorkFlowStepEventId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowProcessingLog_WorkFlowStepId",
                table: "WorkFlowProcessingLog",
                column: "WorkFlowStepId");

            migrationBuilder.CreateIndex(
                name: "WorkFlowProcessingLog_Id",
                table: "WorkFlowProcessingLog",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowProcessingLogFile_FileId",
                table: "WorkFlowProcessingLogFile",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowProcessingLogFile_WorkFlowProcessingLogId",
                table: "WorkFlowProcessingLogFile",
                column: "WorkFlowProcessingLogId");

            migrationBuilder.CreateIndex(
                name: "WorkFlowProcessingLogFile_Id",
                table: "WorkFlowProcessingLogFile",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStep_ActorTypeId",
                table: "WorkFlowStep",
                column: "ActorTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStep_CreationNotificationPlanId",
                table: "WorkFlowStep",
                column: "CreationNotificationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStep_DepartmentId",
                table: "WorkFlowStep",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStep_EmployeeId",
                table: "WorkFlowStep",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStep_StageId",
                table: "WorkFlowStep",
                column: "StageId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStep_WorkFlowId",
                table: "WorkFlowStep",
                column: "WorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStep_WorkModeId",
                table: "WorkFlowStep",
                column: "WorkModeId");

            migrationBuilder.CreateIndex(
                name: "WorkFlowStep_Id",
                table: "WorkFlowStep",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEvent_ActionModeId",
                table: "WorkFlowStepEvent",
                column: "ActionModeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEvent_CompletionNotificationPlanId",
                table: "WorkFlowStepEvent",
                column: "CompletionNotificationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEvent_EventSecurityLevelId",
                table: "WorkFlowStepEvent",
                column: "EventSecurityLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEvent_NextWorkFlowStepDestinationId",
                table: "WorkFlowStepEvent",
                column: "NextWorkFlowStepDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEvent_WorkFlowNotificationPlanId",
                table: "WorkFlowStepEvent",
                column: "WorkFlowNotificationPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEvent_WorkFlowStepsId",
                table: "WorkFlowStepEvent",
                column: "WorkFlowStepsId");

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepEvent_Id",
                table: "WorkFlowStepEvent",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__ActionMode__591971B00EEC9096",
                table: "WorkFlowStepEventActionMode",
                column: "ActionMode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepEventActionMode_Id",
                table: "WorkFlowStepEventActionMode",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlowStepEventSecurityLevel__591971B00EEC9096",
                table: "WorkFlowStepEventSecurityLevel",
                column: "SecurityLevel",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepEventSecurityLevel_Id",
                table: "WorkFlowStepEventSecurityLevel",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEventSecurityLevelActor_DepartmentId",
                table: "WorkFlowStepEventSecurityLevelActor",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEventSecurityLevelActor_EmployeeId",
                table: "WorkFlowStepEventSecurityLevelActor",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEventSecurityLevelActor_SecurityLevelActorId",
                table: "WorkFlowStepEventSecurityLevelActor",
                column: "SecurityLevelActorId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowStepEventSecurityLevelActor_WorkFlowStepEventId",
                table: "WorkFlowStepEventSecurityLevelActor",
                column: "WorkFlowStepEventId");

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepEventSecurityLevelActor_Id",
                table: "WorkFlowStepEventSecurityLevelActor",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlowStepEventSecurityLevelActorType__AA06C8BAA119B1A4",
                table: "WorkFlowStepEventSecurityLevelActorType",
                column: "SecurityLevelActorType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepEventSecurityLevelActorType_Id",
                table: "WorkFlowStepEventSecurityLevelActorType",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlowStepsActorType__AA06C8BAA119B1A4",
                table: "WorkFlowStepsActorType",
                column: "ActorType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepsActorType_Id",
                table: "WorkFlowStepsActorType",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlow__8FE31B33CF02E70B",
                table: "WorkFlowStepStage",
                column: "StageName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepStage_Id",
                table: "WorkFlowStepStage",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__WorkFlow__CDA7E410DEE00B0E",
                table: "WorkFlowStepWorkMode",
                column: "WorkMode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "WorkFlowStepWorkMode_Id",
                table: "WorkFlowStepWorkMode",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArbitratorStopFreeUpDecision");

            migrationBuilder.DropTable(
                name: "CitizenBankAccount");

            migrationBuilder.DropTable(
                name: "CitizenRequestAcademicDegreeFile");

            migrationBuilder.DropTable(
                name: "CitizenRequestExperienceCertificateFile");

            migrationBuilder.DropTable(
                name: "CitizenRequestFreelanceWorkDocumentFile");

            migrationBuilder.DropTable(
                name: "CitizenRequestNote");

            migrationBuilder.DropTable(
                name: "DelegationRequest");

            migrationBuilder.DropTable(
                name: "Delegator");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitrationReportExplanationRequestFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitrator");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitratorAssigningRequest");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitratorRecusalRequestFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitratorsChairmanNominationListValue");

            migrationBuilder.DropTable(
                name: "LitigationRequestCorrespondenceFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestExtendPeriodRequestFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestGroundFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestMeetingReconciliationItem");

            migrationBuilder.DropTable(
                name: "LitigationRequestNoteFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestReformerAssigningRequest");

            migrationBuilder.DropTable(
                name: "LitigationRequestRequestFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestSession");

            migrationBuilder.DropTable(
                name: "ReformerStopFreeUpDecision");

            migrationBuilder.DropTable(
                name: "SysArbitratorCount");

            migrationBuilder.DropTable(
                name: "SysArbitratorNominationListCount");

            migrationBuilder.DropTable(
                name: "SysMobileOTP");

            migrationBuilder.DropTable(
                name: "SysNotification");

            migrationBuilder.DropTable(
                name: "SysNotificationPlanItemDestination");

            migrationBuilder.DropTable(
                name: "SysSetting");

            migrationBuilder.DropTable(
                name: "SysVacationRequest");

            migrationBuilder.DropTable(
                name: "WorkFlowLog");

            migrationBuilder.DropTable(
                name: "WorkFlowNotification");

            migrationBuilder.DropTable(
                name: "WorkFlowProcessingLogFile");

            migrationBuilder.DropTable(
                name: "WorkFlowStepEventSecurityLevelActor");

            migrationBuilder.DropTable(
                name: "SysBank");

            migrationBuilder.DropTable(
                name: "CitizenRequestExperienceCertificate");

            migrationBuilder.DropTable(
                name: "SysDelegationRelationshipType");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitrationReportExplanationRequest");

            migrationBuilder.DropTable(
                name: "SysArbitratorAssigningRequestType");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitratorNominationList");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitratorRecusalRequest");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitratorsChairmanNominationList");

            migrationBuilder.DropTable(
                name: "LitigationRequestCorrespondence");

            migrationBuilder.DropTable(
                name: "LitigationRequestExtendPeriodRequest");

            migrationBuilder.DropTable(
                name: "LitigationRequestGround");

            migrationBuilder.DropTable(
                name: "LitigationRequestReconciliationReport");

            migrationBuilder.DropTable(
                name: "LitigationRequestNote");

            migrationBuilder.DropTable(
                name: "LitigationRequestRequest");

            migrationBuilder.DropTable(
                name: "SysRequestType");

            migrationBuilder.DropTable(
                name: "SysNotificationActorType");

            migrationBuilder.DropTable(
                name: "SysNotificationPlanItem");

            migrationBuilder.DropTable(
                name: "WorkFlowNotificationDestinationType");

            migrationBuilder.DropTable(
                name: "WorkFlowNotificationType");

            migrationBuilder.DropTable(
                name: "WorkFlowProcessingLog");

            migrationBuilder.DropTable(
                name: "WorkFlowStepEventSecurityLevelActorType");

            migrationBuilder.DropTable(
                name: "CitizenRequest");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitrationReport");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestCorrespondenceDestination");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestExtendPeriodRequestStatus");

            migrationBuilder.DropTable(
                name: "SysNotificationPlan");

            migrationBuilder.DropTable(
                name: "WorkFlowStepEvent");

            migrationBuilder.DropTable(
                name: "SysCitizenRequestStatus");

            migrationBuilder.DropTable(
                name: "SysArbitratorClass");

            migrationBuilder.DropTable(
                name: "SysCitizenRequestType");

            migrationBuilder.DropTable(
                name: "LitigationRequest");

            migrationBuilder.DropTable(
                name: "SysMeetingLocation");

            migrationBuilder.DropTable(
                name: "WorkFlowStep");

            migrationBuilder.DropTable(
                name: "WorkFlowStepEventSecurityLevel");

            migrationBuilder.DropTable(
                name: "WorkFlowStepEventActionMode");

            migrationBuilder.DropTable(
                name: "SysExemptionReductionCategory");

            migrationBuilder.DropTable(
                name: "LitigationRequestContract");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestCategory");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestAmount");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestOwnerType");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestStatus");

            migrationBuilder.DropTable(
                name: "OtherCategories");

            migrationBuilder.DropTable(
                name: "SysEmployee");

            migrationBuilder.DropTable(
                name: "WorkFlowStepsActorType");

            migrationBuilder.DropTable(
                name: "WorkFlow");

            migrationBuilder.DropTable(
                name: "WorkFlowStepWorkMode");

            migrationBuilder.DropTable(
                name: "WorkFlowStepStage");

            migrationBuilder.DropTable(
                name: "WorkFlowNotificationPlan");

            migrationBuilder.DropTable(
                name: "Facility");

            migrationBuilder.DropTable(
                name: "SysLitigationRequestContractType");

            migrationBuilder.DropTable(
                name: "SysRegistrationRequest");

            migrationBuilder.DropTable(
                name: "SysDepartment");

            migrationBuilder.DropTable(
                name: "Citizen");

            migrationBuilder.DropTable(
                name: "SysArbitratorStatus");

            migrationBuilder.DropTable(
                name: "SysReformerStatus");

            migrationBuilder.DropTable(
                name: "SysIdentityType");

            migrationBuilder.DropTable(
                name: "SysNationalities");

            migrationBuilder.DropTable(
                name: "SysCity");

            migrationBuilder.DropTable(
                name: "SysGender");

            migrationBuilder.DropTable(
                name: "SysFileManager");

            migrationBuilder.DropTable(
                name: "LitigationCompanyRepresentatives");

            migrationBuilder.DropTable(
                name: "SysArea");
        }
    }
}
