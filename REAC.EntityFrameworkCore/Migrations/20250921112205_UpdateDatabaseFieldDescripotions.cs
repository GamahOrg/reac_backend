using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseFieldDescripotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "WorkMode",
                table: "WorkFlowStepWorkMode",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Work Mode",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "StageName",
                table: "WorkFlowStepStage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Name of the stage",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "WorkFlowStepStage",
                type: "int",
                nullable: true,
                comment: "Display order of the workflow step stage",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ActorType",
                table: "WorkFlowStepsActorType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Actor Type",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "SecurityLevelActorType",
                table: "WorkFlowStepEventSecurityLevelActorType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Security Level Actor Type",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepEventId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow step event",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "SecurityLevelActorId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the security level actor",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the employee",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the department",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecurityLevel",
                table: "WorkFlowStepEventSecurityLevel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Security Level",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "ActionMode",
                table: "WorkFlowStepEventActionMode",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Action Mode",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepsId",
                table: "WorkFlowStepEvent",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow steps",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowProcessingLogForApplicant",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Show Processing Log For Applicant",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowLogForApplicant",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Show Log For Applicant",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "SameEmployee",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Same Employee",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "NextWorkFlowStepDestinationId",
                table: "WorkFlowStepEvent",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the next work flow step destination",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LogMessage",
                table: "WorkFlowStepEvent",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                comment: "Description for Log Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestSubStage",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                comment: "Description for Litigation Request Sub Stage",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestStage",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                comment: "Description for Litigation Request Stage",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinishedValue",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: true,
                comment: "Value",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAllowEditValue",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: true,
                comment: "Value",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdminApprovalRequired",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Is Admin Approval Required",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActiveValue",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: true,
                comment: "Value",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InternalStatusId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExternalStatusId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "EventSecurityLevelId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the event security level",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "WorkFlowStepEvent",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Name of the event",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<int>(
                name: "CustomActionType",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                comment: "Description for Custom Action Type",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CompletionNotificationPlanId",
                table: "WorkFlowStepEvent",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the completion notification plan",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "AllowNotes",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Indicates whether notes are allowed for this workflow step event",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowFileUpload",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "File",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowCompletionNotificationPlan",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Allow Completion Notification Plan",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeIsFinished",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Allow Change Is Finished",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeIsAllowEdit",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Allow Change Is Allow Edit",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeIsActive",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the record is active",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeInternalStatus",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Allow Change Internal Status",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeExternalStatus",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                comment: "Description for Allow Change External Status",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "ActionModeId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                comment: "Unique identifier of the event action mode",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "WorkModeId",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the step work mode",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the workflow",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                table: "WorkFlowStep",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Name of the step",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "StageId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the stage",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ShowForApplicant",
                table: "WorkFlowStep",
                type: "bit",
                nullable: false,
                comment: "Indicates whether this step is visible to the applicant",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessActorType",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                comment: "Type of actor responsible for processing this step",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                comment: "Order of this step in the workflow sequence",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "InternalStatusId",
                table: "WorkFlowStep",
                type: "int",
                nullable: true,
                comment: "Internal status of the request at this step",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExternalStatusId",
                table: "WorkFlowStep",
                type: "int",
                nullable: true,
                comment: "External status of the request at this step",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the employee",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the department",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreationNotificationPlanId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the creation notification plan",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreationLogMessage",
                table: "WorkFlowStep",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                comment: "Message logged when this step is created",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<int>(
                name: "ActorTypeId",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the step actor type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowProcessingLogId",
                table: "WorkFlowProcessingLogFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow processing log",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileId",
                table: "WorkFlowProcessingLogFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow processing log",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow step",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepEventId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the work flow step event",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceWorkFlowProcessingLogId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the source work flow processing log",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ShowForApplicant",
                table: "WorkFlowProcessingLog",
                type: "bit",
                nullable: true,
                comment: "Description for Show For Applicant",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "WorkFlowProcessingLog",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Notes",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsWithNotes",
                table: "WorkFlowProcessingLog",
                type: "bit",
                nullable: false,
                comment: "Notes",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "WorkFlowProcessingLog",
                type: "bit",
                nullable: false,
                comment: "Description for Is Finished",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishedOn",
                table: "WorkFlowProcessingLog",
                type: "datetime",
                nullable: true,
                comment: "Date and time when the workflow processing was finished",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the employee",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "WorkFlowProcessingLog",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationType",
                table: "WorkFlowNotificationType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Notification Type",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "NotificationPlan",
                table: "WorkFlowNotificationPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Description for Notification Plan",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DestinationType",
                table: "WorkFlowNotificationDestinationType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Destination Type",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowNotificationTypeId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow notification type",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowNotificationPlanId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow notification plan",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowNotificationDestinationTypeId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the work flow notification destination type",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationMessage",
                table: "WorkFlowNotification",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Content of the notification message",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the employee",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the department",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "ShowForApplicant",
                table: "WorkFlowLog",
                type: "bit",
                nullable: false,
                comment: "Description for Show For Applicant",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "WorkFlowLog",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "LogMessage",
                table: "WorkFlowLog",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                comment: "Description for Log Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "WorkFlowLog",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "WorkFlowName",
                table: "WorkFlow",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Name of the work flow",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlow",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the department",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "SysVacationRequest",
                type: "date",
                nullable: false,
                comment: "Start date of the vacation",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "SysVacationRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Reason for the vacation request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "SysVacationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the vacation request is approved",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "SysVacationRequest",
                type: "date",
                nullable: false,
                comment: "End date of the vacation",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "SysVacationRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the employee",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "SysVacationRequest",
                type: "date",
                nullable: true,
                comment: "Date and time when the request was approved",
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApprovedBy",
                table: "SysVacationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the employee who approved the request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SysSetting",
                type: "nvarchar(225)",
                maxLength: 225,
                nullable: false,
                comment: "Configuration setting value",
                oldClrType: typeof(string),
                oldType: "nvarchar(225)",
                oldMaxLength: 225);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SysSetting",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Configuration setting key identifier",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "RequestTypeName",
                table: "SysRequestType",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Type of the name",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceCitizenId",
                table: "SysRegistrationRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the source citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestedOn",
                table: "SysRegistrationRequest",
                type: "date",
                nullable: false,
                comment: "Date when the registration request was submitted",
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDoneOn",
                table: "SysRegistrationRequest",
                type: "date",
                nullable: true,
                comment: "Date when the registration was completed",
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Mobile phone number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRegistrationDone",
                table: "SysRegistrationRequest",
                type: "bit",
                nullable: false,
                comment: "Description for Is Registration Done",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Number of the identity",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Facility700Number",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Number of the identity",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SysRegistrationRequest",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                comment: "Email address",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "SysRegistrationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the source citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CRANumber",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Number of the identity",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SysReformerStatus",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Status of the name",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "RetryAttempt",
                table: "SysNotificationSMS",
                type: "int",
                nullable: false,
                comment: "Description for Retry Attempt",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "SysNotificationSMS",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Number of the mobile",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "SysNotificationSMS",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "SysNotificationSMS",
                type: "bit",
                nullable: false,
                comment: "Description for Is Sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "SysNotificationSMS",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Error",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "SysNotificationSMS",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "SysNotificationPlanItemId",
                table: "SysNotificationPlanItemDestination",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the sys notification plan item",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationType",
                table: "SysNotificationPlanItemDestination",
                type: "int",
                nullable: false,
                comment: "Notification Type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationDestinationType",
                table: "SysNotificationPlanItemDestination",
                type: "int",
                nullable: false,
                comment: "Description for Notification Destination Type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationActorTypeId",
                table: "SysNotificationPlanItemDestination",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the notification actor type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "SysNotificationPlanItemDestination",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the department",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SysNotificationPlanId",
                table: "SysNotificationPlanItem",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the sys notification plan",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "SmsTemplate",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Sms Template",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "MailTemplate",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Mail Template",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EmailSubject",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Email address",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationNotificationTemplate",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Application Notification Template",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NotificationPlanName",
                table: "SysNotificationPlan",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Name of the notification plan",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "SysNotificationEmails",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Subject",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "RetryAttempt",
                table: "SysNotificationEmails",
                type: "int",
                nullable: false,
                comment: "Description for Retry Attempt",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "SysNotificationEmails",
                type: "bit",
                nullable: false,
                comment: "Description for Is Sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "SysNotificationEmails",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Error",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "SysNotificationEmails",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "SysNotificationEmails",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Body",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ActorType",
                table: "SysNotificationActorType",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Actor Type",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ViewedOn",
                table: "SysNotification",
                type: "datetime2",
                nullable: true,
                comment: "Date and time when the notification was viewed",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestTypeId",
                table: "SysNotification",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the request type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "SysNotification",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationActorTypeId",
                table: "SysNotification",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the notification actor type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "SysNotification",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                comment: "Description for Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647);

            migrationBuilder.AlterColumn<bool>(
                name: "IsViewed",
                table: "SysNotification",
                type: "bit",
                nullable: false,
                comment: "Description for Is Viewed",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "SysNotification",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "ActorId",
                table: "SysNotification",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the actor",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "NationalityName",
                table: "SysNationalities",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the nationality",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "NationalityCode",
                table: "SysNationalities",
                type: "int",
                nullable: false,
                comment: "Description for Nationality Code",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "SysMobileOTP",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Mobile phone number",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDateTime",
                table: "SysMobileOTP",
                type: "datetime2",
                nullable: false,
                comment: "Expiry date and time of the OTP",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SysMobileOTP",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "Description for Code",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLocationName",
                table: "SysMeetingLocation",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the meeting location",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "RequestStatus",
                table: "SysLitigationRequestStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Request Status",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SysLitigationRequestExtendPeriodRequestStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Status of the name",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "EnName",
                table: "SysLitigationRequestCorrespondenceStatus",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the en",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ArName",
                table: "SysLitigationRequestCorrespondenceStatus",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the en",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "SysLitigationRequestCorrespondenceDestination",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Description for Destination",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Name of the litigation request contract type",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "TermsAndConditions",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Terms and conditions applicable to this contract type",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReconciliationPeriod",
                table: "SysLitigationRequestContractType",
                type: "int",
                nullable: false,
                comment: "Duration of the reconciliation period in days for this contract type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CodingPrefixSymbol",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Prefix symbol used for coding litigation requests of this contract type",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AcknowledgmentContent",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Content of the acknowledgment for this contract type",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TermsAndConditions",
                table: "SysLitigationRequestCategory",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Terms And Conditions",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "BpfId",
                table: "SysLitigationRequestCategory",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the bpf",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "AcknowledgmentContent",
                table: "SysLitigationRequestCategory",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Content",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ToAmount",
                table: "SysLitigationRequestAmount",
                type: "decimal(18,2)",
                nullable: true,
                comment: "Maximum amount for this litigation range",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LitigationAmount",
                table: "SysLitigationRequestAmount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Display name for the litigation amount range",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "FromAmount",
                table: "SysLitigationRequestAmount",
                type: "decimal(18,2)",
                nullable: true,
                comment: "Minimum amount for this litigation range",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Fees",
                table: "SysLitigationRequestAmount",
                type: "money",
                nullable: false,
                comment: "Base fees for this litigation amount range",
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysInvoiceType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Name of the invoice type",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysInvoiceStatus",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Description for Name",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityType",
                table: "SysIdentityType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Identity Type",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "SysGender",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "Description for Gender",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "SysFileManager",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "Description for File Type",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "SysFileManager",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "Path",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "FileDescription",
                table: "SysFileManager",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExemptionReductionCategoryName",
                table: "SysExemptionReductionCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Category of the name",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<bool>(
                name: "WorkAsEmployee",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the person works as an employee",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "SysEmployee",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the user",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "SysEmployee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Mobile phone number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLocked",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                defaultValueSql: "('false')",
                comment: "Indicates whether the employee account is locked",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "('false')");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the employee is an administrator",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                defaultValueSql: "('true')",
                comment: "Indicates whether the record is active",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "('true')");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "SysEmployee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Name of the employee",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SysEmployee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Email address",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "SysEmployee",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the department",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowReAssign",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the employee can reassign tasks",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowAssign",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the employee can assign tasks",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<int>(
                name: "DistributionMode",
                table: "SysDepartment",
                type: "int",
                nullable: false,
                comment: "Description for Distribution Mode",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentType",
                table: "SysDepartment",
                type: "int",
                nullable: false,
                comment: "Description for Department Type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "SysDepartment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Name of the department",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "DelegationRelationshipTypeName",
                table: "SysDelegationRelationshipType",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Type of the name",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "CityName",
                table: "SysCity",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Name of the city",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "SysCity",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the area",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CitizenRequestTypeName",
                table: "SysCitizenRequestType",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                comment: "Type of the name",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<Guid>(
                name: "BpfId",
                table: "SysCitizenRequestType",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the bpf",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "RequestStatus",
                table: "SysCitizenRequestStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                comment: "Description for Request Status",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "SysBank",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the bank",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "AreaName",
                table: "SysArea",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Name of the area",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SysArbitratorStatus",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Status of the name",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorNominationListCount",
                table: "SysArbitratorNominationListCount",
                type: "int",
                nullable: false,
                comment: "Number of arbitrator nomination lists",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorCount",
                table: "SysArbitratorNominationListCount",
                type: "int",
                nullable: false,
                comment: "Number of arbitrators",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorChairmanNominationListCount",
                table: "SysArbitratorNominationListCount",
                type: "int",
                nullable: false,
                comment: "Number of arbitrator chairman nomination lists",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorCount",
                table: "SysArbitratorCount",
                type: "int",
                nullable: false,
                comment: "Number of arbitrators",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "YearsTo",
                table: "SysArbitratorClass",
                type: "int",
                nullable: false,
                comment: "Description for Years To",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "YearsFrom",
                table: "SysArbitratorClass",
                type: "int",
                nullable: false,
                comment: "Description for Years From",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "ArbitratorClassName",
                table: "SysArbitratorClass",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Name of the arbitrator class",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "ArbitratorAssigningRequestName",
                table: "SysArbitratorAssigningRequestType",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the arbitrator assigning request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "StoppingReason",
                table: "ReformerStopFreeUpDecision",
                type: "int",
                nullable: true,
                comment: "Reason for stopping the reformer",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReformerStatusId",
                table: "ReformerStopFreeUpDecision",
                type: "int",
                nullable: false,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "LateLitigationRequestNumber",
                table: "ReformerStopFreeUpDecision",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Number of the late litigation request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionTo",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                comment: "End date of the decision period",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionFrom",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                comment: "Start date of the decision period",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionDate",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: false,
                comment: "Date of the reformer decision",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByRole",
                table: "ReformerStopFreeUpDecision",
                type: "int",
                nullable: true,
                comment: "Role of the user who created this record",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "ReformerStopFreeUpDecision",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the user who created this record",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "ReformerStopFreeUpDecision",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "OtherCategories",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the other category",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ModificationRequest",
                table: "LititgationRequestModificationRequest",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Modification Request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the litigation request session",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "LititgationRequestModificationRequest",
                type: "bit",
                nullable: true,
                comment: "Description for Is Approved",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LititgationRequestModificationRequest",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Description for Created By",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ActorType",
                table: "LititgationRequestModificationRequest",
                type: "int",
                nullable: true,
                comment: "Description for Actor Type",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SessionUrl",
                table: "LitigationRequestSession",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "URL",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SessionDatetime",
                table: "LitigationRequestSession",
                type: "datetime2",
                nullable: false,
                comment: "Date and time of the note",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<long>(
                name: "MeetingId",
                table: "LitigationRequestSession",
                type: "bigint",
                nullable: true,
                comment: "Unique identifier of the meeting",
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestSessionRequestTypeId",
                table: "LitigationRequestSession",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request session request type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestSession",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestSession",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestRequestId",
                table: "LitigationRequestRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "RequestContent",
                table: "LitigationRequestRequest",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                comment: "Content",
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestReformerAssigningRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the reformer assignment request was responded to",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Response",
                table: "LitigationRequestReformerAssigningRequest",
                type: "int",
                nullable: false,
                comment: "Description for Response",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "LitigationRequestReformerAssigningRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the reformer assignment request was submitted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "LitigationRequestReformerAssigningRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Reject Reason",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LitigationRequestReformerAssigningRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Reason",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestReformerAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "LitigationRequestReformerAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReportDate",
                table: "LitigationRequestReconciliationReport",
                type: "datetime",
                nullable: false,
                comment: "Date when the reconciliation report was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "ReconciliationSessionDetails",
                table: "LitigationRequestReconciliationReport",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                comment: "Description for Reconciliation Session Details",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<string>(
                name: "ModificationRequest",
                table: "LitigationRequestReconciliationReport",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Description for Modification Request",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestReconciliationReport",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "NoteId",
                table: "LitigationRequestNoteFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the note",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestNoteFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the note",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedOn",
                table: "LitigationRequestNote",
                type: "datetime2",
                nullable: true,
                comment: "Date and time when the record was last updated",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "LitigationRequestNote",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the user who last updated this note",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LitigationRequestNote",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                comment: "Title of the litigation request note",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "LitigationRequestNote",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Detailed notes or comments about the litigation request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestNote",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestNote",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestNote",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestNote",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the user who created this note",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReconciliationItem",
                table: "LitigationRequestMeetingReconciliationItem",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Reconciliation Item",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestReconciliationReportId",
                table: "LitigationRequestMeetingReconciliationItem",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request reconciliation report",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestGroundId",
                table: "LitigationRequestGroundFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request ground",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestGroundFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request ground",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LitigationRequestGround",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Title",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestGround",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "PercentageExtraAmount",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: true,
                comment: "Percentage",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExtraAmount",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: true,
                comment: "Additional amount charged for extra arbitrators",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorsCount",
                table: "LitigationRequestFees",
                type: "int",
                nullable: false,
                comment: "Number of arbitrators",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "ArbitrationFees",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Total arbitration fees for this case",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "AmountId",
                table: "LitigationRequestFees",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request amount",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestExtendPeriodRequestId",
                table: "LitigationRequestExtendPeriodRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request extend period request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestExtendPeriodRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the file manager",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "RequiredDays",
                table: "LitigationRequestExtendPeriodRequest",
                type: "int",
                nullable: false,
                comment: "Number of additional days requested for the extension",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "RequestReason",
                table: "LitigationRequestExtendPeriodRequest",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "Reason for requesting the period extension",
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "LitigationRequestExtendPeriodRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Additional notes about the period extension request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestExtendPeriodRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestExtendPeriodRequestTypeId",
                table: "LitigationRequestExtendPeriodRequest",
                type: "int",
                nullable: false,
                comment: "Type of the period extension request",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestExtendPeriodRequestStatusId",
                table: "LitigationRequestExtendPeriodRequest",
                type: "int",
                nullable: true,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FinishStatusReason",
                table: "LitigationRequestExtendPeriodRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Status of the reason",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestExtendPeriodRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestExtendPeriodRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the user who created this request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestCorrespondenceReplyId",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request correspondence reply",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the file manager",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Description for Created By",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ReplyMessage",
                table: "LitigationRequestCorrespondenceReply",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Reply Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestCorrespondenceId",
                table: "LitigationRequestCorrespondenceReply",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request correspondence",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestCorrespondenceReply",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestCorrespondenceReply",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Description for Created By",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestCorrespondenceId",
                table: "LitigationRequestCorrespondenceFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request correspondence",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestCorrespondenceFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request correspondence",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "LitigationRequestCorrespondence",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "Subject",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerialCode",
                table: "LitigationRequestCorrespondence",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Serial Code",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "LitigationRequestCorrespondence",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Message",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestCorrespondence",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCorrespondenceDestinationId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request correspondence destination",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request category",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestCorrespondence",
                type: "datetime",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestCorrespondence",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Description for Created By",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VersionNumber",
                table: "LitigationRequestContract",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Number of the version",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondPartyRegistrationRequestId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the second party registration request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyName",
                table: "LitigationRequestContract",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Name of the association",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyMobile",
                table: "LitigationRequestContract",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Mobile phone number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyIdentityNumber",
                table: "LitigationRequestContract",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Number of the version",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondPartyFacilityId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the second party facility",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyEmail",
                table: "LitigationRequestContract",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Email address",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondPartyCitizenId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the second party citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyCapacity",
                table: "LitigationRequestContract",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Role or capacity of the second party in the contract",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestContractTypeId",
                table: "LitigationRequestContract",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request contract type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "InternalRegulationsFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the internal regulations file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "IncorporationContractFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the incorporation contract file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FoundingLetterFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the founding letter file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FirstPartyFacilityId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the first party facility",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FirstPartyCitizenId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the first party citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstPartyCapacity",
                table: "LitigationRequestContract",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Role or capacity of the first party in the contract",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ContractKey",
                table: "LitigationRequestContract",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                comment: "Contract key identifier or number",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<Guid>(
                name: "ContractFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the contract file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssociationRegistrationCertificateFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the association registration certificate file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssociationName",
                table: "LitigationRequestContract",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Name of the association",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "int",
                nullable: false,
                comment: "Description for Priority",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "bit",
                nullable: false,
                comment: "Description for Is Sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the arbitrator citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorChairmanCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the arbitrator chairman citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "TotalPriority",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "int",
                nullable: true,
                comment: "Total",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Response",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "int",
                nullable: true,
                comment: "Description for Response",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: false,
                comment: "Description for Is Sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: true,
                comment: "Description for Is Selected From Manager",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: true,
                comment: "Description for Is Escalated",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "datetime2",
                nullable: true,
                comment: "Date when the chairman nomination was escalated",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorChairmanCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "RecusalRequestFileId",
                table: "LitigationRequestArbitratorRecusalRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request arbitrator recusal request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestArbitratorRecusalRequestId",
                table: "LitigationRequestArbitratorRecusalRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request arbitrator recusal request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the arbitrator recusal request was responded to",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestParty",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "int",
                nullable: false,
                comment: "Description for Request Party",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "datetime2",
                nullable: false,
                comment: "Date when the arbitrator recusal request was submitted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Reason",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ApproveRejectStatus",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "int",
                nullable: true,
                comment: "Description for Approve Reject Status",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "TotalPriority",
                table: "LitigationRequestArbitratorNominationList",
                type: "int",
                nullable: true,
                comment: "Total combined priority score for this arbitrator",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffPriority",
                table: "LitigationRequestArbitratorNominationList",
                type: "int",
                nullable: true,
                comment: "Priority ranking assigned by the plaintiff for this arbitrator",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorNominationList",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the nomination list has been sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                comment: "Indicates whether this arbitrator was selected by the manager",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsResponse",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                comment: "Indicates whether a response has been received for this nomination",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsPlaintiffSelected",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                comment: "Indicates whether this arbitrator was selected by the plaintiff",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the nomination has been escalated to management",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefendantSelected",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                comment: "Indicates whether this arbitrator was selected by the defendant",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorNominationList",
                type: "datetime2",
                nullable: true,
                comment: "Date when the nomination was escalated",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DefendantPriority",
                table: "LitigationRequestArbitratorNominationList",
                type: "int",
                nullable: true,
                comment: "Priority ranking assigned by the defendant for this arbitrator",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitratorNominationList",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the arbitrator assignment request was responded to",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Response",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "int",
                nullable: false,
                comment: "Description for Response",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "datetime2",
                nullable: false,
                comment: "Date when the arbitrator assignment request was submitted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Reject Reason",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorAssigningRequestTypeId",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitrator",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitrator",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExplanationRequestFileId",
                table: "LitigationRequestArbitrationReportExplanationRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the arbitration report explanation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitrationReportExplanationRequestId",
                table: "LitigationRequestArbitrationReportExplanationRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the arbitration report explanation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the explanation request response was provided",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description for Response",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "RequestArbitrationReportExplanationRequestParty",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "int",
                nullable: false,
                comment: "Description for Request Arbitration Report Explanation Request Party",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestArbitrationReportId",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request arbitration report",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ExplanationRequests",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Description for Explanation Requests",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExplanationRequestDate",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "datetime2",
                nullable: false,
                comment: "Date when the explanation request was submitted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Reasons",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                comment: "Reasoning and justification for the arbitration decision",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "ModificationRequest",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                comment: "Details of any modification requests to the arbitration report",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<Guid>(
                name: "MeetingLocationId",
                table: "LitigationRequestArbitrationReport",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitrationReport",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "JudgmentDecisionText",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                comment: "Full text of the judgment decision",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<string>(
                name: "JudgmentDecisionPlace",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Location where the arbitration judgment decision was issued",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JudgmentDecisionDate",
                table: "LitigationRequestArbitrationReport",
                type: "datetime",
                nullable: false,
                comment: "Date when the judgment decision was made",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JudgmentDate",
                table: "LitigationRequestArbitrationReport",
                type: "datetime",
                nullable: false,
                comment: "Date when the judgment was issued",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "Facts",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                comment: "Facts and findings presented in the arbitration report",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestAmountLog",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestAmountLog",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "AmountId",
                table: "LitigationRequestAmountLog",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request amount",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LitigationRequest",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                comment: "Title",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartReconciliationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when reconciliation period starts",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartArbitrationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when arbitration period starts",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentOn",
                table: "LitigationRequest",
                type: "datetime",
                nullable: true,
                comment: "Date when the litigation request was sent",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SecondPartyArbitratorsCount",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Number of second party arbitrators",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "RequestCode",
                table: "LitigationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Request code for the litigation",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ReformerCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the reformer citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReconciliationPeriod",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Duration of the reconciliation period in days",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ReconciliationFees",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Fees for the reconciliation stage",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "PretenderRating",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Rating given by the plaintiff for the litigation process",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlaintiffAgreementRequestResponseOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the plaintiff responded to the agreement request",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffAgreementRequestResponse",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Response status of the plaintiff to the agreement request",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidOn",
                table: "LitigationRequest",
                type: "datetime",
                nullable: true,
                comment: "Date when the payment was made",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OtherCategories",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the other category",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MintuesOfMeetingApprovalPlaintiffStatusOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the plaintiff approved/rejected the meeting minutes",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MintuesOfMeetingApprovalPlaintiffStatus",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Approval status of the plaintiff for the minutes of meeting",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "MintuesOfMeetingApprovalDefendantStatusOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the defendant approved/rejected the meeting minutes",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "MintuesOfMeetingApprovalDefendantStatus",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Approval status of the defendant for the minutes of meeting",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestSubStage",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Current sub-stage of the litigation request",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestStage",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Current stage of the litigation request",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestOwnerTypeId",
                table: "LitigationRequest",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request owner type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Unique identifier of the litigation request category",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestAmountId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the litigation request amount",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "JudgmentDecisionIssuingDateTime",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date and time of the note",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the litigation request has been sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForReviewerSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether reminder for reviewer was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForReviewerManagerSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether reminder for reviewer manager was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForCancelLitigationForNotesSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether reminder for cancel litigation due to notes was sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForCancelLitigationForAgreementSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether reminder for cancel litigation due to agreement was sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForApplicantSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether reminder for applicant was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForAgreementRequestForRequestSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether reminder for agreement request was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerReminderForReconciliationSecondPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether second reminder for reconciliation period was sent to reformer",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerReminderForReconciliationFirstPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether reformer reminder for reconciliation first period was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerReminderForCancelReconciliationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether reminder for cancel reconciliation was sent to reformer",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerFinalReminderForReconciliationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether final reminder for reconciliation was sent to reformer",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReconciliationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether reconciliation fees have been collected",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaid",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether payment has been made",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMinutesOfMeetingApprovalRequired",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether minutes of meeting approval is required for this litigation",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReformerAssigningRequestResponsePeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether litigation request reformer assigning request response period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReconciliationReportResponsePeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether litigation request reconciliation report response period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReconciliationReportDecisionPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether litigation request reconciliation report decision period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReconciliationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether first reminder for unpaid reconciliation fees was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether first reminder for unpaid litigation fees was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestArbitrationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether first reminder for unpaid arbitration fees was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether litigation fees have been collected",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the litigation request has been completed",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinancialLitigation",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether this litigation involves financial matters",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsExemptionReduction",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether exemption reduction applies to this litigation",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsCollectionOfReconciliationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether reconciliation fees collection request is being sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCollectionOfLitigationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether litigation fees collection request is being sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCollectionOfArbitrationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether arbitration fees collection request is being sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAssignNewReformerForCancelReconciliationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether notification to assign new reformer for cancelled reconciliation was sent",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorsChairmanDecisionRequired",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether arbitrators chairman decision is required for this litigation",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorSessionTimeAddedPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether arbitrator session time added period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorReminderForCancelArbitrationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether reminder for cancel arbitration was sent to arbitrator",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorCountSelectionResponsePeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether arbitrator count selection response period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorCountSelectionDecisionPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether arbitrator count selection decision period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorAssigningRequestResponseSecondPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether arbitrator assigning request response second period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorAssigningRequestResponseFirstPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether arbitrator assigning request response first period notification was sent",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitrationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether arbitration fees have been collected",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAllowEdit",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                comment: "Indicates whether editing is allowed for this litigation request",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAgreementRequestRequired",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether agreement request is required for this litigation",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the record is active",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InternalStatusId",
                table: "LitigationRequest",
                type: "int",
                nullable: false,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "FirstPartyArbitratorsCount",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Number of first party arbitrators",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "FeesDifference",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Difference between calculated and actual fees",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Fees",
                table: "LitigationRequest",
                type: "money",
                nullable: false,
                comment: "Total fees for this litigation request",
                oldClrType: typeof(decimal),
                oldType: "money");

            migrationBuilder.AlterColumn<int>(
                name: "ExternalStatusId",
                table: "LitigationRequest",
                type: "int",
                nullable: false,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExemptionReductionFileId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the exemption reduction file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExemptionReductionCategoryId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the exemption reduction category",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndReconciliationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when reconciliation period ends",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndArbitrationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when arbitration period ends",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "LitigationRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Description",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the delegator",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the delegator citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DefendantRating",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Rating given by the defendant for the litigation process",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DefendantAgreementRequestResponseOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the defendant responded to the agreement request",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DefendantAgreementRequestResponse",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Response status of the defendant to the agreement request",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequest",
                type: "datetime",
                nullable: false,
                comment: "Date and time when the record was created",
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContractId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the litigation request contract",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "ContractAddress",
                table: "LitigationRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Address",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CityId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the city",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the area",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArbitratorsSelectionDurationDateTime",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Duration",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArbitratorsCountDateTime",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date and time when arbitrators count was determined",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorsCount",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Total number of arbitrators",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorSelectionDuration",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Duration",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the arbitrator citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArbitrationPeriod",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                comment: "Duration of the arbitration period in days",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ArbitrationFees",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Fees for the arbitration stage",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the applicant citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "AgreementRequestRequiredOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date when the agreement request was required",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Total amount of the invoice including tax",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true,
                comment: "Date when the invoice was paid",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceTypeId",
                table: "Invoices",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the invoice type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "InvoiceTax",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Invoice tax amount",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceStatusId",
                table: "Invoices",
                type: "int",
                nullable: false,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceCode",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Invoice code",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                comment: "Expiration date of the invoice",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CancelledDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true,
                comment: "Date when the invoice was cancelled",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantCitizenId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the applicant citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                comment: "Base amount of the invoice before tax",
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<string>(
                name: "UnitNumber",
                table: "Facility",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "Unit number",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubNumber",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Sub number of the facility address",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Facility",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Street address",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Postal code",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                table: "Facility",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Fax number",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Facility700Number",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "700 number identifier of the facility",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "EnglishFacilityName",
                table: "Facility",
                type: "nvarchar(max)",
                nullable: true,
                comment: "English name of the facility",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Facility",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                comment: "Email address",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "Facility",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "District",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CommercialRegisterNumber",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Number of the commercial register",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Facility code identifier",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Facility",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "City where the facility is located",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "Facility",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuildingNumber",
                table: "Facility",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "Building number of the facility address",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ArabicFacilityName",
                table: "Facility",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the arabic facility",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "RepresentationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the representation document file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LawyerLicenseNumber",
                table: "Delegator",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Lawyer license number",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LawyerLicenseExpirationDate",
                table: "Delegator",
                type: "datetime2",
                nullable: true,
                comment: "Expiration date of the lawyer license",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LawLicenseFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the law license file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Delegator",
                type: "bit",
                nullable: false,
                comment: "Indicates whether the record is active",
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AlterColumn<Guid>(
                name: "FacilityId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the facility being delegated",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "EstablishmentFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the establishment contract file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorCitizenId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the delegator citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DelegationRelationshipTypeId",
                table: "Delegator",
                type: "int",
                nullable: false,
                comment: "Type of delegation relationship",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the delegation file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DelegateNumber",
                table: "Delegator",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Number of the delegate",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DecisionFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the decision file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CustodyFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the custody proof file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CommercialRegisterationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the commercial registration file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the delegating citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestedOn",
                table: "DelegationRequest",
                type: "datetime2",
                nullable: false,
                comment: "Date when the delegation request was submitted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "DelegationRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Reason for the delegation request or modification",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedOn",
                table: "DelegationRequest",
                type: "datetime2",
                nullable: true,
                comment: "Date and time when the record was last modified",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ModifiedBy",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the user who last modified this record",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestOwnerTypeId",
                table: "DelegationRequest",
                type: "int",
                nullable: false,
                comment: "Unique identifier of the litigation request owner type",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the litigation request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "DelegationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the delegation request has been approved",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "DelegationRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the record is active",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the delegator",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DelegationParty",
                table: "DelegationRequest",
                type: "int",
                nullable: false,
                comment: "Party involved in the delegation (plaintiff or defendant)",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "DelegationRequest",
                type: "datetime",
                nullable: true,
                comment: "Date when the delegation request was approved",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ReconciliationCenterDegreeFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the reconciliation center degree file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "RealEstateInstituteFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the real estate institute program file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "CitizenRequestNote",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                comment: "Notes or comments about the citizen request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NoteDateTime",
                table: "CitizenRequestNote",
                type: "datetime2",
                nullable: false,
                comment: "Date and time of the note",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestNote",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "FreelanceWorkDocumentFileId",
                table: "CitizenRequestFreelanceWorkDocumentFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the freelance work document file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestFreelanceWorkDocumentFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExperienceCertificateFileId",
                table: "CitizenRequestExperienceCertificateFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the experience certificate file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestExperienceCertificateId",
                table: "CitizenRequestExperienceCertificateFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request experience certificate",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "IssuingEntityName",
                table: "CitizenRequestExperienceCertificate",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the issuing entity",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestExperienceCertificate",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "CertificateName",
                table: "CitizenRequestExperienceCertificate",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the experience certificate",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AcquisitionDate",
                table: "CitizenRequestExperienceCertificate",
                type: "datetime2",
                nullable: false,
                comment: "Date when the certificate was acquired",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestCVFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CVFileId",
                table: "CitizenRequestCVFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the CV file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestAcademicDegreeFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen request",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "AcademicDegreeFileId",
                table: "CitizenRequestAcademicDegreeFile",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the academic degree file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "CitizenRequest",
                type: "int",
                nullable: false,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "CitizenRequest",
                type: "datetime2",
                nullable: false,
                comment: "Date when the request was submitted",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<bool>(
                name: "IsregisteredWithBachelorShareaaDegree",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether registered with bachelor Sharia degree",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IspassedRealEstateProgram",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether passed real estate program",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsVolunteerApproval",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether volunteer approval is granted",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsRegisteredReconciliationCenter",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether registered with reconciliation center",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsFeesPaid",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the citizen request fees have been paid",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HaveHostingExperience",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether has hosting experience",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HaveExperiencedInFreelanceArbitration",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether has experience in freelance arbitration",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "HaveArbitrationxperience",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                comment: "Indicates whether has arbitration experience",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInHousingstatuses",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Housing experience statuses",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExperiencedInFreelanceArbitration",
                table: "CitizenRequest",
                type: "int",
                nullable: true,
                comment: "Years of experience in freelance arbitration",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInArbitrationFields",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Fields of experience in arbitration",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceYears",
                table: "CitizenRequest",
                type: "int",
                nullable: true,
                comment: "Number of years of experience",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CitizenRequestTypeId",
                table: "CitizenRequest",
                type: "int",
                nullable: true,
                comment: "Unique identifier of the citizen request type",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "CitizenRequest",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Brief",
                table: "CitizenRequest",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                comment: "Brief description or summary",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "CitizenRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the area",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorClassId",
                table: "CitizenRequest",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the arbitrator class",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AcademicDegreeName",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the academic degree",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "About",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Additional information about the applicant",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IBan",
                table: "CitizenBankAccount",
                type: "nvarchar(max)",
                nullable: false,
                comment: "International Bank Account Number",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "CitizenBankAccount",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankId",
                table: "CitizenBankAccount",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the bank",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "AccountOwnerName",
                table: "CitizenBankAccount",
                type: "nvarchar(max)",
                nullable: false,
                comment: "Name of the account owner",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "WorkTelephone",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Telephone number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the user",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UnitNumber",
                table: "Citizen",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "Unit number",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SubNumber",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Sub number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Citizen",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "Street address",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "SigntureFileId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the signature file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Short name of the citizen",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ReformerStatusId",
                table: "Citizen",
                type: "int",
                nullable: true,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Postal code",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PassCitizenId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the pass citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the nationality",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NationalAddress",
                table: "Citizen",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Address",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                comment: "Mobile phone number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationCompanyRepresentativeId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the litigation company representative",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LawyerLicenseNumber",
                table: "Citizen",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Lawyer license number",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "LawyerLicenseExpirationDate",
                table: "Citizen",
                type: "datetime2",
                nullable: true,
                comment: "Expiration date of the lawyer license",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "LawLicenseFileId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the law license file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformer",
                table: "Citizen",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the person is a reformer",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsLawyer",
                table: "Citizen",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the person is a lawyer",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDraftAccount",
                table: "Citizen",
                type: "bit",
                nullable: true,
                comment: "Indicates whether this is a draft account",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitrator",
                table: "Citizen",
                type: "bit",
                nullable: true,
                comment: "Indicates whether the person is an arbitrator",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdentityTypeId",
                table: "Citizen",
                type: "int",
                nullable: true,
                comment: "Type of identity document",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "Citizen",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                comment: "Identity number of the citizen",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "IdentityFileId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the identity file",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "IdentityExpiryDate",
                table: "Citizen",
                type: "date",
                nullable: true,
                comment: "Expiry date of the identity document",
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "HomeTelephone",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Telephone number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "GenderId",
                table: "Citizen",
                type: "int",
                nullable: true,
                comment: "Gender identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Citizen",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                comment: "Full name of the citizen",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Fax number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Employer",
                table: "Citizen",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                comment: "Employer information",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Citizen",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                comment: "Email address",
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "Citizen",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                comment: "District",
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Citizen code",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "CityId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the city",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BuildingNumber",
                table: "Citizen",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                comment: "Building number",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "Citizen",
                type: "datetime2",
                nullable: true,
                comment: "Date of birth",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                comment: "Unique identifier of the area",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorStatusId",
                table: "Citizen",
                type: "int",
                nullable: true,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNumber",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                comment: "Additional number",
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "StoppingReason",
                table: "ArbitratorStopFreeUpDecision",
                type: "int",
                nullable: true,
                comment: "Reason for stopping or freeing up the arbitrator",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LateLitigationRequestNumber",
                table: "ArbitratorStopFreeUpDecision",
                type: "nvarchar(max)",
                nullable: true,
                comment: "Number of the late litigation request",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionTo",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                comment: "End date of the arbitrator stop/free up decision period",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionFrom",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                comment: "Start date of the arbitrator stop/free up decision period",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionDate",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: false,
                comment: "Date when the arbitrator stop/free up decision was made",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "ArbitratorStopFreeUpDecision",
                type: "uniqueidentifier",
                nullable: false,
                comment: "Unique identifier of the citizen",
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorStatusId",
                table: "ArbitratorStopFreeUpDecision",
                type: "int",
                nullable: false,
                comment: "Status identifier",
                oldClrType: typeof(int),
                oldType: "int");


        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AlterColumn<string>(
                name: "WorkMode",
                table: "WorkFlowStepWorkMode",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Work Mode");

            migrationBuilder.AlterColumn<string>(
                name: "StageName",
                table: "WorkFlowStepStage",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Name of the stage");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "WorkFlowStepStage",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Display order of the workflow step stage");

            migrationBuilder.AlterColumn<string>(
                name: "ActorType",
                table: "WorkFlowStepsActorType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Actor Type");

            migrationBuilder.AlterColumn<string>(
                name: "SecurityLevelActorType",
                table: "WorkFlowStepEventSecurityLevelActorType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Security Level Actor Type");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepEventId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow step event");

            migrationBuilder.AlterColumn<int>(
                name: "SecurityLevelActorId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the security level actor");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the employee");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlowStepEventSecurityLevelActor",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the department");

            migrationBuilder.AlterColumn<string>(
                name: "SecurityLevel",
                table: "WorkFlowStepEventSecurityLevel",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Security Level");

            migrationBuilder.AlterColumn<string>(
                name: "ActionMode",
                table: "WorkFlowStepEventActionMode",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Action Mode");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepsId",
                table: "WorkFlowStepEvent",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow steps");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowProcessingLogForApplicant",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Show Processing Log For Applicant");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowLogForApplicant",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Show Log For Applicant");

            migrationBuilder.AlterColumn<bool>(
                name: "SameEmployee",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Same Employee");

            migrationBuilder.AlterColumn<Guid>(
                name: "NextWorkFlowStepDestinationId",
                table: "WorkFlowStepEvent",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the next work flow step destination");

            migrationBuilder.AlterColumn<string>(
                name: "LogMessage",
                table: "WorkFlowStepEvent",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldComment: "Description for Log Message");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestSubStage",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Description for Litigation Request Sub Stage");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestStage",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Description for Litigation Request Stage");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinishedValue",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Value");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAllowEditValue",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Value");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdminApprovalRequired",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Admin Approval Required");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActiveValue",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Value");

            migrationBuilder.AlterColumn<int>(
                name: "InternalStatusId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<int>(
                name: "ExternalStatusId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<int>(
                name: "EventSecurityLevelId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the event security level");

            migrationBuilder.AlterColumn<string>(
                name: "EventName",
                table: "WorkFlowStepEvent",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Name of the event");

            migrationBuilder.AlterColumn<int>(
                name: "CustomActionType",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Description for Custom Action Type");

            migrationBuilder.AlterColumn<Guid>(
                name: "CompletionNotificationPlanId",
                table: "WorkFlowStepEvent",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the completion notification plan");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowNotes",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether notes are allowed for this workflow step event");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowFileUpload",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "File");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowCompletionNotificationPlan",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Allow Completion Notification Plan");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeIsFinished",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Allow Change Is Finished");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeIsAllowEdit",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Allow Change Is Allow Edit");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeIsActive",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the record is active");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeInternalStatus",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Allow Change Internal Status");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowChangeExternalStatus",
                table: "WorkFlowStepEvent",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Allow Change External Status");

            migrationBuilder.AlterColumn<int>(
                name: "ActionModeId",
                table: "WorkFlowStepEvent",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Unique identifier of the event action mode");

            migrationBuilder.AlterColumn<int>(
                name: "WorkModeId",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the step work mode");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the workflow");

            migrationBuilder.AlterColumn<string>(
                name: "StepName",
                table: "WorkFlowStep",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Name of the step");

            migrationBuilder.AlterColumn<Guid>(
                name: "StageId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the stage");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowForApplicant",
                table: "WorkFlowStep",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether this step is visible to the applicant");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessActorType",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Type of actor responsible for processing this step");

            migrationBuilder.AlterColumn<int>(
                name: "Order",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Order of this step in the workflow sequence");

            migrationBuilder.AlterColumn<int>(
                name: "InternalStatusId",
                table: "WorkFlowStep",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Internal status of the request at this step");

            migrationBuilder.AlterColumn<int>(
                name: "ExternalStatusId",
                table: "WorkFlowStep",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "External status of the request at this step");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the employee");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the department");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreationNotificationPlanId",
                table: "WorkFlowStep",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the creation notification plan");

            migrationBuilder.AlterColumn<string>(
                name: "CreationLogMessage",
                table: "WorkFlowStep",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldComment: "Message logged when this step is created");

            migrationBuilder.AlterColumn<int>(
                name: "ActorTypeId",
                table: "WorkFlowStep",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the step actor type");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowProcessingLogId",
                table: "WorkFlowProcessingLogFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow processing log");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileId",
                table: "WorkFlowProcessingLogFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow processing log");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow step");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowStepEventId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the work flow step event");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceWorkFlowProcessingLogId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the source work flow processing log");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowForApplicant",
                table: "WorkFlowProcessingLog",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Description for Show For Applicant");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the request");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "WorkFlowProcessingLog",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Notes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsWithNotes",
                table: "WorkFlowProcessingLog",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Notes");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "WorkFlowProcessingLog",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Finished");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FinishedOn",
                table: "WorkFlowProcessingLog",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldComment: "Date and time when the workflow processing was finished");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowProcessingLog",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the employee");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "WorkFlowProcessingLog",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationType",
                table: "WorkFlowNotificationType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Notification Type");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationPlan",
                table: "WorkFlowNotificationPlan",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Description for Notification Plan");

            migrationBuilder.AlterColumn<string>(
                name: "DestinationType",
                table: "WorkFlowNotificationDestinationType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Destination Type");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowNotificationTypeId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow notification type");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowNotificationPlanId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow notification plan");

            migrationBuilder.AlterColumn<Guid>(
                name: "WorkFlowNotificationDestinationTypeId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the work flow notification destination type");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationMessage",
                table: "WorkFlowNotification",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Content of the notification message");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the employee");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlowNotification",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the department");

            migrationBuilder.AlterColumn<bool>(
                name: "ShowForApplicant",
                table: "WorkFlowLog",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Show For Applicant");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "WorkFlowLog",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the request");

            migrationBuilder.AlterColumn<string>(
                name: "LogMessage",
                table: "WorkFlowLog",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldComment: "Description for Log Message");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "WorkFlowLog",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<string>(
                name: "WorkFlowName",
                table: "WorkFlow",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Name of the work flow");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "WorkFlow",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the department");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "SysVacationRequest",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "Start date of the vacation");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "SysVacationRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Reason for the vacation request");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "SysVacationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the vacation request is approved");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "SysVacationRequest",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "End date of the vacation");

            migrationBuilder.AlterColumn<Guid>(
                name: "EmployeeId",
                table: "SysVacationRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the employee");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "SysVacationRequest",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true,
                oldComment: "Date and time when the request was approved");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApprovedBy",
                table: "SysVacationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the employee who approved the request");

            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "SysSetting",
                type: "nvarchar(225)",
                maxLength: 225,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(225)",
                oldMaxLength: 225,
                oldComment: "Configuration setting value");

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "SysSetting",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Configuration setting key identifier");

            migrationBuilder.AlterColumn<string>(
                name: "RequestTypeName",
                table: "SysRequestType",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Type of the name");

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceCitizenId",
                table: "SysRegistrationRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the source citizen");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestedOn",
                table: "SysRegistrationRequest",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldComment: "Date when the registration request was submitted");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationDoneOn",
                table: "SysRegistrationRequest",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true,
                oldComment: "Date when the registration was completed");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Mobile phone number");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRegistrationDone",
                table: "SysRegistrationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Registration Done");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Number of the identity");

            migrationBuilder.AlterColumn<string>(
                name: "Facility700Number",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Number of the identity");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SysRegistrationRequest",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldComment: "Email address");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "SysRegistrationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the source citizen");

            migrationBuilder.AlterColumn<string>(
                name: "CRANumber",
                table: "SysRegistrationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Number of the identity");

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SysReformerStatus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Status of the name");

            migrationBuilder.AlterColumn<int>(
                name: "RetryAttempt",
                table: "SysNotificationSMS",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Retry Attempt");

            migrationBuilder.AlterColumn<string>(
                name: "MobileNumber",
                table: "SysNotificationSMS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Number of the mobile");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "SysNotificationSMS",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Message");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "SysNotificationSMS",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Sent");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "SysNotificationSMS",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Error");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "SysNotificationSMS",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "SysNotificationPlanItemId",
                table: "SysNotificationPlanItemDestination",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the sys notification plan item");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationType",
                table: "SysNotificationPlanItemDestination",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Notification Type");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationDestinationType",
                table: "SysNotificationPlanItemDestination",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Notification Destination Type");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationActorTypeId",
                table: "SysNotificationPlanItemDestination",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the notification actor type");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "SysNotificationPlanItemDestination",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the department");

            migrationBuilder.AlterColumn<Guid>(
                name: "SysNotificationPlanId",
                table: "SysNotificationPlanItem",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the sys notification plan");

            migrationBuilder.AlterColumn<string>(
                name: "SmsTemplate",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Sms Template");

            migrationBuilder.AlterColumn<string>(
                name: "MailTemplate",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Mail Template");

            migrationBuilder.AlterColumn<string>(
                name: "EmailSubject",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Email address");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationNotificationTemplate",
                table: "SysNotificationPlanItem",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Application Notification Template");

            migrationBuilder.AlterColumn<string>(
                name: "NotificationPlanName",
                table: "SysNotificationPlan",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Name of the notification plan");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "SysNotificationEmails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Subject");

            migrationBuilder.AlterColumn<int>(
                name: "RetryAttempt",
                table: "SysNotificationEmails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Retry Attempt");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "SysNotificationEmails",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Sent");

            migrationBuilder.AlterColumn<string>(
                name: "Error",
                table: "SysNotificationEmails",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Error");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "SysNotificationEmails",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<string>(
                name: "Body",
                table: "SysNotificationEmails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Body");

            migrationBuilder.AlterColumn<string>(
                name: "ActorType",
                table: "SysNotificationActorType",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Actor Type");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ViewedOn",
                table: "SysNotification",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date and time when the notification was viewed");

            migrationBuilder.AlterColumn<int>(
                name: "RequestTypeId",
                table: "SysNotification",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the request type");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "SysNotification",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the request");

            migrationBuilder.AlterColumn<int>(
                name: "NotificationActorTypeId",
                table: "SysNotification",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the notification actor type");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "SysNotification",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647,
                oldComment: "Description for Message");

            migrationBuilder.AlterColumn<bool>(
                name: "IsViewed",
                table: "SysNotification",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Viewed");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "SysNotification",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "ActorId",
                table: "SysNotification",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the actor");

            migrationBuilder.AlterColumn<string>(
                name: "NationalityName",
                table: "SysNationalities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the nationality");

            migrationBuilder.AlterColumn<int>(
                name: "NationalityCode",
                table: "SysNationalities",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Nationality Code");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "SysMobileOTP",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Mobile phone number");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDateTime",
                table: "SysMobileOTP",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Expiry date and time of the OTP");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "SysMobileOTP",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "Description for Code");

            migrationBuilder.AlterColumn<string>(
                name: "MeetingLocationName",
                table: "SysMeetingLocation",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the meeting location");

            migrationBuilder.AlterColumn<string>(
                name: "RequestStatus",
                table: "SysLitigationRequestStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Request Status");

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SysLitigationRequestExtendPeriodRequestStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Status of the name");

            migrationBuilder.AlterColumn<string>(
                name: "EnName",
                table: "SysLitigationRequestCorrespondenceStatus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the en");

            migrationBuilder.AlterColumn<string>(
                name: "ArName",
                table: "SysLitigationRequestCorrespondenceStatus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the en");

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "SysLitigationRequestCorrespondenceDestination",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Description for Destination");

            migrationBuilder.AlterColumn<string>(
                name: "TypeName",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Name of the litigation request contract type");

            migrationBuilder.AlterColumn<string>(
                name: "TermsAndConditions",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Terms and conditions applicable to this contract type");

            migrationBuilder.AlterColumn<int>(
                name: "ReconciliationPeriod",
                table: "SysLitigationRequestContractType",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Duration of the reconciliation period in days for this contract type");

            migrationBuilder.AlterColumn<string>(
                name: "CodingPrefixSymbol",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Prefix symbol used for coding litigation requests of this contract type");

            migrationBuilder.AlterColumn<string>(
                name: "AcknowledgmentContent",
                table: "SysLitigationRequestContractType",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Content of the acknowledgment for this contract type");

            migrationBuilder.AlterColumn<string>(
                name: "TermsAndConditions",
                table: "SysLitigationRequestCategory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Terms And Conditions");

            migrationBuilder.AlterColumn<Guid>(
                name: "BpfId",
                table: "SysLitigationRequestCategory",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the bpf");

            migrationBuilder.AlterColumn<string>(
                name: "AcknowledgmentContent",
                table: "SysLitigationRequestCategory",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Content");

            migrationBuilder.AlterColumn<decimal>(
                name: "ToAmount",
                table: "SysLitigationRequestAmount",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "Maximum amount for this litigation range");

            migrationBuilder.AlterColumn<string>(
                name: "LitigationAmount",
                table: "SysLitigationRequestAmount",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Display name for the litigation amount range");

            migrationBuilder.AlterColumn<decimal>(
                name: "FromAmount",
                table: "SysLitigationRequestAmount",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "Minimum amount for this litigation range");

            migrationBuilder.AlterColumn<decimal>(
                name: "Fees",
                table: "SysLitigationRequestAmount",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldComment: "Base fees for this litigation amount range");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysInvoiceType",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Name of the invoice type");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "SysInvoiceStatus",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Description for Name");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityType",
                table: "SysIdentityType",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Identity Type");

            migrationBuilder.AlterColumn<string>(
                name: "Gender",
                table: "SysGender",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "Description for Gender");

            migrationBuilder.AlterColumn<string>(
                name: "FileType",
                table: "SysFileManager",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "Description for File Type");

            migrationBuilder.AlterColumn<string>(
                name: "FilePath",
                table: "SysFileManager",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldComment: "Path");

            migrationBuilder.AlterColumn<string>(
                name: "FileDescription",
                table: "SysFileManager",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "ExemptionReductionCategoryName",
                table: "SysExemptionReductionCategory",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Category of the name");

            migrationBuilder.AlterColumn<bool>(
                name: "WorkAsEmployee",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the person works as an employee");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "SysEmployee",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the user");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "SysEmployee",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Mobile phone number");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLocked",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                defaultValueSql: "('false')",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "('false')",
                oldComment: "Indicates whether the employee account is locked");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAdmin",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the employee is an administrator");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                defaultValueSql: "('true')",
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValueSql: "('true')",
                oldComment: "Indicates whether the record is active");

            migrationBuilder.AlterColumn<string>(
                name: "EmployeeName",
                table: "SysEmployee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Name of the employee");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "SysEmployee",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Email address");

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "SysEmployee",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the department");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowReAssign",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the employee can reassign tasks");

            migrationBuilder.AlterColumn<bool>(
                name: "AllowAssign",
                table: "SysEmployee",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the employee can assign tasks");

            migrationBuilder.AlterColumn<int>(
                name: "DistributionMode",
                table: "SysDepartment",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Distribution Mode");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentType",
                table: "SysDepartment",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Department Type");

            migrationBuilder.AlterColumn<string>(
                name: "DepartmentName",
                table: "SysDepartment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Name of the department");

            migrationBuilder.AlterColumn<string>(
                name: "DelegationRelationshipTypeName",
                table: "SysDelegationRelationshipType",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Type of the name");

            migrationBuilder.AlterColumn<string>(
                name: "CityName",
                table: "SysCity",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Name of the city");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "SysCity",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the area");

            migrationBuilder.AlterColumn<string>(
                name: "CitizenRequestTypeName",
                table: "SysCitizenRequestType",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldComment: "Type of the name");

            migrationBuilder.AlterColumn<Guid>(
                name: "BpfId",
                table: "SysCitizenRequestType",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the bpf");

            migrationBuilder.AlterColumn<string>(
                name: "RequestStatus",
                table: "SysCitizenRequestStatus",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldComment: "Description for Request Status");

            migrationBuilder.AlterColumn<string>(
                name: "BankName",
                table: "SysBank",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the bank");

            migrationBuilder.AlterColumn<string>(
                name: "AreaName",
                table: "SysArea",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Name of the area");

            migrationBuilder.AlterColumn<string>(
                name: "StatusName",
                table: "SysArbitratorStatus",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Status of the name");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorNominationListCount",
                table: "SysArbitratorNominationListCount",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of arbitrator nomination lists");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorCount",
                table: "SysArbitratorNominationListCount",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of arbitrators");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorChairmanNominationListCount",
                table: "SysArbitratorNominationListCount",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of arbitrator chairman nomination lists");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorCount",
                table: "SysArbitratorCount",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of arbitrators");

            migrationBuilder.AlterColumn<int>(
                name: "YearsTo",
                table: "SysArbitratorClass",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Years To");

            migrationBuilder.AlterColumn<int>(
                name: "YearsFrom",
                table: "SysArbitratorClass",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Years From");

            migrationBuilder.AlterColumn<string>(
                name: "ArbitratorClassName",
                table: "SysArbitratorClass",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Name of the arbitrator class");

            migrationBuilder.AlterColumn<string>(
                name: "ArbitratorAssigningRequestName",
                table: "SysArbitratorAssigningRequestType",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the arbitrator assigning request");

            migrationBuilder.AlterColumn<int>(
                name: "StoppingReason",
                table: "ReformerStopFreeUpDecision",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Reason for stopping the reformer");

            migrationBuilder.AlterColumn<int>(
                name: "ReformerStatusId",
                table: "ReformerStopFreeUpDecision",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<string>(
                name: "LateLitigationRequestNumber",
                table: "ReformerStopFreeUpDecision",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Number of the late litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionTo",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "End date of the decision period");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionFrom",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Start date of the decision period");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionDate",
                table: "ReformerStopFreeUpDecision",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date of the reformer decision");

            migrationBuilder.AlterColumn<int>(
                name: "CreatedByRole",
                table: "ReformerStopFreeUpDecision",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Role of the user who created this record");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "ReformerStopFreeUpDecision",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the user who created this record");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "ReformerStopFreeUpDecision",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "OtherCategories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the other category");

            migrationBuilder.AlterColumn<string>(
                name: "ModificationRequest",
                table: "LititgationRequestModificationRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Modification Request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestSessionId",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the litigation request session");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "LititgationRequestModificationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Description for Is Approved");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LititgationRequestModificationRequest",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LititgationRequestModificationRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Description for Created By");

            migrationBuilder.AlterColumn<int>(
                name: "ActorType",
                table: "LititgationRequestModificationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Description for Actor Type");

            migrationBuilder.AlterColumn<string>(
                name: "SessionUrl",
                table: "LitigationRequestSession",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "URL");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SessionDatetime",
                table: "LitigationRequestSession",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time of the note");

            migrationBuilder.AlterColumn<long>(
                name: "MeetingId",
                table: "LitigationRequestSession",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true,
                oldComment: "Unique identifier of the meeting");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestSessionRequestTypeId",
                table: "LitigationRequestSession",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request session request type");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestSession",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestSession",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestRequestId",
                table: "LitigationRequestRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request request");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request request");

            migrationBuilder.AlterColumn<string>(
                name: "RequestContent",
                table: "LitigationRequestRequest",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldComment: "Content");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestReformerAssigningRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the reformer assignment request was responded to");

            migrationBuilder.AlterColumn<int>(
                name: "Response",
                table: "LitigationRequestReformerAssigningRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Response");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "LitigationRequestReformerAssigningRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the reformer assignment request was submitted");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "LitigationRequestReformerAssigningRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Reject Reason");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LitigationRequestReformerAssigningRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Reason");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestReformerAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "LitigationRequestReformerAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReportDate",
                table: "LitigationRequestReconciliationReport",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "Date when the reconciliation report was created");

            migrationBuilder.AlterColumn<string>(
                name: "ReconciliationSessionDetails",
                table: "LitigationRequestReconciliationReport",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldComment: "Description for Reconciliation Session Details");

            migrationBuilder.AlterColumn<string>(
                name: "ModificationRequest",
                table: "LitigationRequestReconciliationReport",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "Description for Modification Request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestReconciliationReport",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "NoteId",
                table: "LitigationRequestNoteFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the note");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestNoteFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the note");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedOn",
                table: "LitigationRequestNote",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date and time when the record was last updated");

            migrationBuilder.AlterColumn<Guid>(
                name: "UpdatedBy",
                table: "LitigationRequestNote",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the user who last updated this note");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LitigationRequestNote",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldComment: "Title of the litigation request note");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "LitigationRequestNote",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Detailed notes or comments about the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestNote",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestNote",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestNote",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestNote",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the user who created this note");

            migrationBuilder.AlterColumn<string>(
                name: "ReconciliationItem",
                table: "LitigationRequestMeetingReconciliationItem",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Reconciliation Item");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestReconciliationReportId",
                table: "LitigationRequestMeetingReconciliationItem",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request reconciliation report");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestGroundId",
                table: "LitigationRequestGroundFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request ground");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestGroundFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request ground");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LitigationRequestGround",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Title");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestGround",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<decimal>(
                name: "PercentageExtraAmount",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "Percentage");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExtraAmount",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true,
                oldComment: "Additional amount charged for extra arbitrators");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorsCount",
                table: "LitigationRequestFees",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of arbitrators");

            migrationBuilder.AlterColumn<decimal>(
                name: "ArbitrationFees",
                table: "LitigationRequestFees",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Total arbitration fees for this case");

            migrationBuilder.AlterColumn<Guid>(
                name: "AmountId",
                table: "LitigationRequestFees",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request amount");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestExtendPeriodRequestId",
                table: "LitigationRequestExtendPeriodRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request extend period request");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestExtendPeriodRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the file manager");

            migrationBuilder.AlterColumn<int>(
                name: "RequiredDays",
                table: "LitigationRequestExtendPeriodRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Number of additional days requested for the extension");

            migrationBuilder.AlterColumn<string>(
                name: "RequestReason",
                table: "LitigationRequestExtendPeriodRequest",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(300)",
                oldMaxLength: 300,
                oldComment: "Reason for requesting the period extension");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "LitigationRequestExtendPeriodRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Additional notes about the period extension request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestExtendPeriodRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestExtendPeriodRequestTypeId",
                table: "LitigationRequestExtendPeriodRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Type of the period extension request");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestExtendPeriodRequestStatusId",
                table: "LitigationRequestExtendPeriodRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<string>(
                name: "FinishStatusReason",
                table: "LitigationRequestExtendPeriodRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Status of the reason");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestExtendPeriodRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestExtendPeriodRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the user who created this request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestCorrespondenceReplyId",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request correspondence reply");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the file manager");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestCorrespondenceReplyFile",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Description for Created By");

            migrationBuilder.AlterColumn<string>(
                name: "ReplyMessage",
                table: "LitigationRequestCorrespondenceReply",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Reply Message");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestCorrespondenceId",
                table: "LitigationRequestCorrespondenceReply",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request correspondence");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestCorrespondenceReply",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestCorrespondenceReply",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Description for Created By");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestCorrespondenceId",
                table: "LitigationRequestCorrespondenceFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request correspondence");

            migrationBuilder.AlterColumn<Guid>(
                name: "FileManagerId",
                table: "LitigationRequestCorrespondenceFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request correspondence");

            migrationBuilder.AlterColumn<string>(
                name: "Subject",
                table: "LitigationRequestCorrespondence",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "Subject");

            migrationBuilder.AlterColumn<string>(
                name: "SerialCode",
                table: "LitigationRequestCorrespondence",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Serial Code");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "LitigationRequestCorrespondence",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Message");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestCorrespondence",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCorrespondenceStatusId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCorrespondenceDestinationId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request correspondence destination");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequestCorrespondence",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request category");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestCorrespondence",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedBy",
                table: "LitigationRequestCorrespondence",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Description for Created By");

            migrationBuilder.AlterColumn<string>(
                name: "VersionNumber",
                table: "LitigationRequestContract",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Number of the version");

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondPartyRegistrationRequestId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the second party registration request");

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyName",
                table: "LitigationRequestContract",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Name of the association");

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyMobile",
                table: "LitigationRequestContract",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Mobile phone number");

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyIdentityNumber",
                table: "LitigationRequestContract",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Number of the version");

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondPartyFacilityId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the second party facility");

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyEmail",
                table: "LitigationRequestContract",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Email address");

            migrationBuilder.AlterColumn<Guid>(
                name: "SecondPartyCitizenId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the second party citizen");

            migrationBuilder.AlterColumn<string>(
                name: "SecondPartyCapacity",
                table: "LitigationRequestContract",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Role or capacity of the second party in the contract");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestContractTypeId",
                table: "LitigationRequestContract",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request contract type");

            migrationBuilder.AlterColumn<Guid>(
                name: "InternalRegulationsFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the internal regulations file");

            migrationBuilder.AlterColumn<Guid>(
                name: "IncorporationContractFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the incorporation contract file");

            migrationBuilder.AlterColumn<Guid>(
                name: "FoundingLetterFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the founding letter file");

            migrationBuilder.AlterColumn<Guid>(
                name: "FirstPartyFacilityId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the first party facility");

            migrationBuilder.AlterColumn<Guid>(
                name: "FirstPartyCitizenId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the first party citizen");

            migrationBuilder.AlterColumn<string>(
                name: "FirstPartyCapacity",
                table: "LitigationRequestContract",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Role or capacity of the first party in the contract");

            migrationBuilder.AlterColumn<string>(
                name: "ContractKey",
                table: "LitigationRequestContract",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldComment: "Contract key identifier or number");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContractFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the contract file");

            migrationBuilder.AlterColumn<Guid>(
                name: "AssociationRegistrationCertificateFileId",
                table: "LitigationRequestContract",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the association registration certificate file");

            migrationBuilder.AlterColumn<string>(
                name: "AssociationName",
                table: "LitigationRequestContract",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Name of the association");

            migrationBuilder.AlterColumn<int>(
                name: "Priority",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Priority");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Sent");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the arbitrator citizen");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorChairmanCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationListValue",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the arbitrator chairman citizen");

            migrationBuilder.AlterColumn<int>(
                name: "TotalPriority",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Total");

            migrationBuilder.AlterColumn<int>(
                name: "Response",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Description for Response");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Description for Is Sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Description for Is Selected From Manager");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Description for Is Escalated");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the chairman nomination was escalated");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorChairmanCitizenId",
                table: "LitigationRequestArbitratorsChairmanNominationList",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "RecusalRequestFileId",
                table: "LitigationRequestArbitratorRecusalRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request arbitrator recusal request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestArbitratorRecusalRequestId",
                table: "LitigationRequestArbitratorRecusalRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request arbitrator recusal request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the arbitrator recusal request was responded to");

            migrationBuilder.AlterColumn<int>(
                name: "RequestParty",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Request Party");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the arbitrator recusal request was submitted");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Reason");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "ApproveRejectStatus",
                table: "LitigationRequestArbitratorRecusalRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Description for Approve Reject Status");

            migrationBuilder.AlterColumn<int>(
                name: "TotalPriority",
                table: "LitigationRequestArbitratorNominationList",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Total combined priority score for this arbitrator");

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffPriority",
                table: "LitigationRequestArbitratorNominationList",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Priority ranking assigned by the plaintiff for this arbitrator");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorNominationList",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the nomination list has been sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSelectedFromManager",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether this arbitrator was selected by the manager");

            migrationBuilder.AlterColumn<bool>(
                name: "IsResponse",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether a response has been received for this nomination");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPlaintiffSelected",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether this arbitrator was selected by the plaintiff");

            migrationBuilder.AlterColumn<bool>(
                name: "IsEscalated",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the nomination has been escalated to management");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDefendantSelected",
                table: "LitigationRequestArbitratorNominationList",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether this arbitrator was selected by the defendant");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EscalateDate",
                table: "LitigationRequestArbitratorNominationList",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the nomination was escalated");

            migrationBuilder.AlterColumn<int>(
                name: "DefendantPriority",
                table: "LitigationRequestArbitratorNominationList",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Priority ranking assigned by the defendant for this arbitrator");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitratorNominationList",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the arbitrator assignment request was responded to");

            migrationBuilder.AlterColumn<int>(
                name: "Response",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Response");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the arbitrator assignment request was submitted");

            migrationBuilder.AlterColumn<string>(
                name: "RejectReason",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Reject Reason");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorAssigningRequestTypeId",
                table: "LitigationRequestArbitratorAssigningRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitrator",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequestArbitrator",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExplanationRequestFileId",
                table: "LitigationRequestArbitrationReportExplanationRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the arbitration report explanation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitrationReportExplanationRequestId",
                table: "LitigationRequestArbitrationReportExplanationRequestFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the arbitration report explanation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ResponseOn",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the explanation request response was provided");

            migrationBuilder.AlterColumn<string>(
                name: "Response",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description for Response");

            migrationBuilder.AlterColumn<int>(
                name: "RequestArbitrationReportExplanationRequestParty",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Description for Request Arbitration Report Explanation Request Party");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestArbitrationReportId",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request arbitration report");

            migrationBuilder.AlterColumn<string>(
                name: "ExplanationRequests",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Description for Explanation Requests");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExplanationRequestDate",
                table: "LitigationRequestArbitrationReportExplanationRequest",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the explanation request was submitted");

            migrationBuilder.AlterColumn<string>(
                name: "Reasons",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000,
                oldComment: "Reasoning and justification for the arbitration decision");

            migrationBuilder.AlterColumn<string>(
                name: "ModificationRequest",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldComment: "Details of any modification requests to the arbitration report");

            migrationBuilder.AlterColumn<Guid>(
                name: "MeetingLocationId",
                table: "LitigationRequestArbitrationReport",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestArbitrationReport",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<string>(
                name: "JudgmentDecisionText",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000,
                oldComment: "Full text of the judgment decision");

            migrationBuilder.AlterColumn<string>(
                name: "JudgmentDecisionPlace",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Location where the arbitration judgment decision was issued");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JudgmentDecisionDate",
                table: "LitigationRequestArbitrationReport",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "Date when the judgment decision was made");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JudgmentDate",
                table: "LitigationRequestArbitrationReport",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "Date when the judgment was issued");

            migrationBuilder.AlterColumn<string>(
                name: "Facts",
                table: "LitigationRequestArbitrationReport",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 5000,
                oldComment: "Facts and findings presented in the arbitration report");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "LitigationRequestAmountLog",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequestAmountLog",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "AmountId",
                table: "LitigationRequestAmountLog",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request amount");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "LitigationRequest",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldComment: "Title");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartReconciliationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when reconciliation period starts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartArbitrationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when arbitration period starts");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SentOn",
                table: "LitigationRequest",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldComment: "Date when the litigation request was sent");

            migrationBuilder.AlterColumn<int>(
                name: "SecondPartyArbitratorsCount",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Number of second party arbitrators");

            migrationBuilder.AlterColumn<string>(
                name: "RequestCode",
                table: "LitigationRequest",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Request code for the litigation");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReformerCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the reformer citizen");

            migrationBuilder.AlterColumn<int>(
                name: "ReconciliationPeriod",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Duration of the reconciliation period in days");

            migrationBuilder.AlterColumn<decimal>(
                name: "ReconciliationFees",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Fees for the reconciliation stage");

            migrationBuilder.AlterColumn<int>(
                name: "PretenderRating",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Rating given by the plaintiff for the litigation process");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PlaintiffAgreementRequestResponseOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the plaintiff responded to the agreement request");

            migrationBuilder.AlterColumn<int>(
                name: "PlaintiffAgreementRequestResponse",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Response status of the plaintiff to the agreement request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaidOn",
                table: "LitigationRequest",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldComment: "Date when the payment was made");

            migrationBuilder.AlterColumn<Guid>(
                name: "OtherCategories",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the other category");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MintuesOfMeetingApprovalPlaintiffStatusOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the plaintiff approved/rejected the meeting minutes");

            migrationBuilder.AlterColumn<int>(
                name: "MintuesOfMeetingApprovalPlaintiffStatus",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Approval status of the plaintiff for the minutes of meeting");

            migrationBuilder.AlterColumn<DateTime>(
                name: "MintuesOfMeetingApprovalDefendantStatusOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the defendant approved/rejected the meeting minutes");

            migrationBuilder.AlterColumn<int>(
                name: "MintuesOfMeetingApprovalDefendantStatus",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Approval status of the defendant for the minutes of meeting");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestSubStage",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Current sub-stage of the litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestStage",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Current stage of the litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestOwnerTypeId",
                table: "LitigationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request owner type");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestCategoryId",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Unique identifier of the litigation request category");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestAmountId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the litigation request amount");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JudgmentDecisionIssuingDateTime",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date and time of the note");

            migrationBuilder.AlterColumn<bool>(
                name: "IsSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the litigation request has been sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForReviewerSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether reminder for reviewer was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForReviewerManagerSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether reminder for reviewer manager was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForCancelLitigationForNotesSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether reminder for cancel litigation due to notes was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForCancelLitigationForAgreementSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether reminder for cancel litigation due to agreement was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForApplicantSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether reminder for applicant was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReminderForAgreementRequestForRequestSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether reminder for agreement request was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerReminderForReconciliationSecondPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether second reminder for reconciliation period was sent to reformer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerReminderForReconciliationFirstPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether reformer reminder for reconciliation first period was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerReminderForCancelReconciliationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether reminder for cancel reconciliation was sent to reformer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformerFinalReminderForReconciliationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether final reminder for reconciliation was sent to reformer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReconciliationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether reconciliation fees have been collected");

            migrationBuilder.AlterColumn<bool>(
                name: "IsPaid",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether payment has been made");

            migrationBuilder.AlterColumn<bool>(
                name: "IsMinutesOfMeetingApprovalRequired",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether minutes of meeting approval is required for this litigation");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReformerAssigningRequestResponsePeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether litigation request reformer assigning request response period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReconciliationReportResponsePeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether litigation request reconciliation report response period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReconciliationReportDecisionPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether litigation request reconciliation report decision period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestReconciliationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether first reminder for unpaid reconciliation fees was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether first reminder for unpaid litigation fees was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationRequestArbitrationFeesNotPaidFirstReminderSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether first reminder for unpaid arbitration fees was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLitigationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether litigation fees have been collected");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinished",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the litigation request has been completed");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFinancialLitigation",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether this litigation involves financial matters");

            migrationBuilder.AlterColumn<bool>(
                name: "IsExemptionReduction",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether exemption reduction applies to this litigation");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCollectionOfReconciliationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether reconciliation fees collection request is being sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCollectionOfLitigationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether litigation fees collection request is being sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsCollectionOfArbitrationFeesRequestIsBeingSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether arbitration fees collection request is being sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAssignNewReformerForCancelReconciliationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether notification to assign new reformer for cancelled reconciliation was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorsChairmanDecisionRequired",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether arbitrators chairman decision is required for this litigation");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorSessionTimeAddedPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether arbitrator session time added period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorReminderForCancelArbitrationSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether reminder for cancel arbitration was sent to arbitrator");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorCountSelectionResponsePeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether arbitrator count selection response period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorCountSelectionDecisionPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether arbitrator count selection decision period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorAssigningRequestResponseSecondPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether arbitrator assigning request response second period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitratorAssigningRequestResponseFirstPeriodSent",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether arbitrator assigning request response first period notification was sent");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitrationFeesHaveBeenCollected",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether arbitration fees have been collected");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAllowEdit",
                table: "LitigationRequest",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether editing is allowed for this litigation request");

            migrationBuilder.AlterColumn<bool>(
                name: "IsAgreementRequestRequired",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether agreement request is required for this litigation");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "LitigationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the record is active");

            migrationBuilder.AlterColumn<int>(
                name: "InternalStatusId",
                table: "LitigationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<int>(
                name: "FirstPartyArbitratorsCount",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Number of first party arbitrators");

            migrationBuilder.AlterColumn<decimal>(
                name: "FeesDifference",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Difference between calculated and actual fees");

            migrationBuilder.AlterColumn<decimal>(
                name: "Fees",
                table: "LitigationRequest",
                type: "money",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "money",
                oldComment: "Total fees for this litigation request");

            migrationBuilder.AlterColumn<int>(
                name: "ExternalStatusId",
                table: "LitigationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExemptionReductionFileId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the exemption reduction file");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExemptionReductionCategoryId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the exemption reduction category");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndReconciliationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when reconciliation period ends");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndArbitrationDate",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when arbitration period ends");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "LitigationRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Description");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the delegator");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the delegator citizen");

            migrationBuilder.AlterColumn<int>(
                name: "DefendantRating",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Rating given by the defendant for the litigation process");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DefendantAgreementRequestResponseOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the defendant responded to the agreement request");

            migrationBuilder.AlterColumn<int>(
                name: "DefendantAgreementRequestResponse",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Response status of the defendant to the agreement request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedOn",
                table: "LitigationRequest",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "Date and time when the record was created");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContractId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the litigation request contract");

            migrationBuilder.AlterColumn<string>(
                name: "ContractAddress",
                table: "LitigationRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Address");

            migrationBuilder.AlterColumn<Guid>(
                name: "CityId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the city");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the area");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArbitratorsSelectionDurationDateTime",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Duration");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ArbitratorsCountDateTime",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date and time when arbitrators count was determined");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorsCount",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Total number of arbitrators");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorSelectionDuration",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Duration");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the arbitrator citizen");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitrationPeriod",
                table: "LitigationRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Duration of the arbitration period in days");

            migrationBuilder.AlterColumn<decimal>(
                name: "ArbitrationFees",
                table: "LitigationRequest",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Fees for the arbitration stage");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantCitizenId",
                table: "LitigationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the applicant citizen");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AgreementRequestRequiredOn",
                table: "LitigationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the agreement request was required");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Total amount of the invoice including tax");

            migrationBuilder.AlterColumn<Guid>(
                name: "RequestId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PaymentDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the invoice was paid");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceTypeId",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the invoice type");

            migrationBuilder.AlterColumn<decimal>(
                name: "InvoiceTax",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Invoice tax amount");

            migrationBuilder.AlterColumn<int>(
                name: "InvoiceStatusId",
                table: "Invoices",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceCode",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Invoice code");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpirationDate",
                table: "Invoices",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Expiration date of the invoice");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CancelledDate",
                table: "Invoices",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date when the invoice was cancelled");

            migrationBuilder.AlterColumn<Guid>(
                name: "ApplicantCitizenId",
                table: "Invoices",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the applicant citizen");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                table: "Invoices",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldComment: "Base amount of the invoice before tax");

            migrationBuilder.AlterColumn<string>(
                name: "UnitNumber",
                table: "Facility",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "Unit number");

            migrationBuilder.AlterColumn<string>(
                name: "SubNumber",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Sub number of the facility address");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Facility",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Street address");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Postal code");

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                table: "Facility",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Fax number");

            migrationBuilder.AlterColumn<string>(
                name: "Facility700Number",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "700 number identifier of the facility");

            migrationBuilder.AlterColumn<string>(
                name: "EnglishFacilityName",
                table: "Facility",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "English name of the facility");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Facility",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldComment: "Email address");

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "Facility",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "District");

            migrationBuilder.AlterColumn<string>(
                name: "CommercialRegisterNumber",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Number of the commercial register");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Facility",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Facility code identifier");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                table: "Facility",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "City where the facility is located");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "Facility",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the citizen");

            migrationBuilder.AlterColumn<string>(
                name: "BuildingNumber",
                table: "Facility",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "Building number of the facility address");

            migrationBuilder.AlterColumn<string>(
                name: "ArabicFacilityName",
                table: "Facility",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the arabic facility");

            migrationBuilder.AlterColumn<Guid>(
                name: "RepresentationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the representation document file");

            migrationBuilder.AlterColumn<string>(
                name: "LawyerLicenseNumber",
                table: "Delegator",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Lawyer license number");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LawyerLicenseExpirationDate",
                table: "Delegator",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Expiration date of the lawyer license");

            migrationBuilder.AlterColumn<Guid>(
                name: "LawLicenseFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the law license file");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "Delegator",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldComment: "Indicates whether the record is active");

            migrationBuilder.AlterColumn<Guid>(
                name: "FacilityId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the facility being delegated");

            migrationBuilder.AlterColumn<Guid>(
                name: "EstablishmentFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the establishment contract file");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorCitizenId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the delegator citizen");

            migrationBuilder.AlterColumn<int>(
                name: "DelegationRelationshipTypeId",
                table: "Delegator",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Type of delegation relationship");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the delegation file");

            migrationBuilder.AlterColumn<string>(
                name: "DelegateNumber",
                table: "Delegator",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Number of the delegate");

            migrationBuilder.AlterColumn<Guid>(
                name: "DecisionFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the decision file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CustodyFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the custody proof file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CommercialRegisterationFileId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the commercial registration file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "Delegator",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the delegating citizen");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestedOn",
                table: "DelegationRequest",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the delegation request was submitted");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "DelegationRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Reason for the delegation request or modification");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedOn",
                table: "DelegationRequest",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date and time when the record was last modified");

            migrationBuilder.AlterColumn<Guid>(
                name: "ModifiedBy",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the user who last modified this record");

            migrationBuilder.AlterColumn<int>(
                name: "LitigationRequestOwnerTypeId",
                table: "DelegationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Unique identifier of the litigation request owner type");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationRequestId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the litigation request");

            migrationBuilder.AlterColumn<bool>(
                name: "IsApproved",
                table: "DelegationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the delegation request has been approved");

            migrationBuilder.AlterColumn<bool>(
                name: "IsActive",
                table: "DelegationRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the record is active");

            migrationBuilder.AlterColumn<Guid>(
                name: "DelegatorId",
                table: "DelegationRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the delegator");

            migrationBuilder.AlterColumn<int>(
                name: "DelegationParty",
                table: "DelegationRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Party involved in the delegation (plaintiff or defendant)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ApprovedOn",
                table: "DelegationRequest",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldComment: "Date when the delegation request was approved");

            migrationBuilder.AlterColumn<Guid>(
                name: "ReconciliationCenterDegreeFileId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the reconciliation center degree file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestReconciliationCenterDegreeFiles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request");

            migrationBuilder.AlterColumn<Guid>(
                name: "RealEstateInstituteFileId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the real estate institute program file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestProgramRealEstateInstituteFiles",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request");

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "CitizenRequestNote",
                type: "nvarchar(max)",
                maxLength: 2147483647,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldMaxLength: 2147483647,
                oldComment: "Notes or comments about the citizen request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NoteDateTime",
                table: "CitizenRequestNote",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time of the note");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestNote",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request");

            migrationBuilder.AlterColumn<Guid>(
                name: "FreelanceWorkDocumentFileId",
                table: "CitizenRequestFreelanceWorkDocumentFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the freelance work document file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestFreelanceWorkDocumentFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExperienceCertificateFileId",
                table: "CitizenRequestExperienceCertificateFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the experience certificate file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestExperienceCertificateId",
                table: "CitizenRequestExperienceCertificateFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request experience certificate");

            migrationBuilder.AlterColumn<string>(
                name: "IssuingEntityName",
                table: "CitizenRequestExperienceCertificate",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the issuing entity");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestExperienceCertificate",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request");

            migrationBuilder.AlterColumn<string>(
                name: "CertificateName",
                table: "CitizenRequestExperienceCertificate",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the experience certificate");

            migrationBuilder.AlterColumn<DateTime>(
                name: "AcquisitionDate",
                table: "CitizenRequestExperienceCertificate",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the certificate was acquired");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestCVFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request");

            migrationBuilder.AlterColumn<Guid>(
                name: "CVFileId",
                table: "CitizenRequestCVFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the CV file");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenRequestId",
                table: "CitizenRequestAcademicDegreeFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen request");

            migrationBuilder.AlterColumn<Guid>(
                name: "AcademicDegreeFileId",
                table: "CitizenRequestAcademicDegreeFile",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the academic degree file");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "CitizenRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<DateTime>(
                name: "RequestOn",
                table: "CitizenRequest",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the request was submitted");

            migrationBuilder.AlterColumn<bool>(
                name: "IsregisteredWithBachelorShareaaDegree",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether registered with bachelor Sharia degree");

            migrationBuilder.AlterColumn<bool>(
                name: "IspassedRealEstateProgram",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether passed real estate program");

            migrationBuilder.AlterColumn<bool>(
                name: "IsVolunteerApproval",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether volunteer approval is granted");

            migrationBuilder.AlterColumn<bool>(
                name: "IsRegisteredReconciliationCenter",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether registered with reconciliation center");

            migrationBuilder.AlterColumn<bool>(
                name: "IsFeesPaid",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the citizen request fees have been paid");

            migrationBuilder.AlterColumn<bool>(
                name: "HaveHostingExperience",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether has hosting experience");

            migrationBuilder.AlterColumn<bool>(
                name: "HaveExperiencedInFreelanceArbitration",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether has experience in freelance arbitration");

            migrationBuilder.AlterColumn<bool>(
                name: "HaveArbitrationxperience",
                table: "CitizenRequest",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether has arbitration experience");

            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInHousingstatuses",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Housing experience statuses");

            migrationBuilder.AlterColumn<int>(
                name: "ExperiencedInFreelanceArbitration",
                table: "CitizenRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Years of experience in freelance arbitration");

            migrationBuilder.AlterColumn<string>(
                name: "ExperiencedInArbitrationFields",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Fields of experience in arbitration");

            migrationBuilder.AlterColumn<int>(
                name: "ExperienceYears",
                table: "CitizenRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Number of years of experience");

            migrationBuilder.AlterColumn<int>(
                name: "CitizenRequestTypeId",
                table: "CitizenRequest",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Unique identifier of the citizen request type");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "CitizenRequest",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen");

            migrationBuilder.AlterColumn<string>(
                name: "Brief",
                table: "CitizenRequest",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true,
                oldComment: "Brief description or summary");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "CitizenRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the area");

            migrationBuilder.AlterColumn<Guid>(
                name: "ArbitratorClassId",
                table: "CitizenRequest",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the arbitrator class");

            migrationBuilder.AlterColumn<string>(
                name: "AcademicDegreeName",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the academic degree");

            migrationBuilder.AlterColumn<string>(
                name: "About",
                table: "CitizenRequest",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Additional information about the applicant");

            migrationBuilder.AlterColumn<string>(
                name: "IBan",
                table: "CitizenBankAccount",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "International Bank Account Number");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "CitizenBankAccount",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen");

            migrationBuilder.AlterColumn<Guid>(
                name: "BankId",
                table: "CitizenBankAccount",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the bank");

            migrationBuilder.AlterColumn<string>(
                name: "AccountOwnerName",
                table: "CitizenBankAccount",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldComment: "Name of the account owner");

            migrationBuilder.AlterColumn<string>(
                name: "WorkTelephone",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Telephone number");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the user");

            migrationBuilder.AlterColumn<string>(
                name: "UnitNumber",
                table: "Citizen",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "Unit number");

            migrationBuilder.AlterColumn<string>(
                name: "SubNumber",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Sub number");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                table: "Citizen",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "Street address");

            migrationBuilder.AlterColumn<Guid>(
                name: "SigntureFileId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the signature file");

            migrationBuilder.AlterColumn<string>(
                name: "ShortName",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Short name of the citizen");

            migrationBuilder.AlterColumn<int>(
                name: "ReformerStatusId",
                table: "Citizen",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Postal code");

            migrationBuilder.AlterColumn<Guid>(
                name: "PassCitizenId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the pass citizen");

            migrationBuilder.AlterColumn<Guid>(
                name: "NationalityId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the nationality");

            migrationBuilder.AlterColumn<string>(
                name: "NationalAddress",
                table: "Citizen",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Address");

            migrationBuilder.AlterColumn<string>(
                name: "Mobile",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldComment: "Mobile phone number");

            migrationBuilder.AlterColumn<Guid>(
                name: "LitigationCompanyRepresentativeId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the litigation company representative");

            migrationBuilder.AlterColumn<string>(
                name: "LawyerLicenseNumber",
                table: "Citizen",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Lawyer license number");

            migrationBuilder.AlterColumn<DateTime>(
                name: "LawyerLicenseExpirationDate",
                table: "Citizen",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Expiration date of the lawyer license");

            migrationBuilder.AlterColumn<Guid>(
                name: "LawLicenseFileId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the law license file");

            migrationBuilder.AlterColumn<bool>(
                name: "IsReformer",
                table: "Citizen",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the person is a reformer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsLawyer",
                table: "Citizen",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the person is a lawyer");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDraftAccount",
                table: "Citizen",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether this is a draft account");

            migrationBuilder.AlterColumn<bool>(
                name: "IsArbitrator",
                table: "Citizen",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldComment: "Indicates whether the person is an arbitrator");

            migrationBuilder.AlterColumn<int>(
                name: "IdentityTypeId",
                table: "Citizen",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Type of identity document");

            migrationBuilder.AlterColumn<string>(
                name: "IdentityNumber",
                table: "Citizen",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldComment: "Identity number of the citizen");

            migrationBuilder.AlterColumn<Guid>(
                name: "IdentityFileId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the identity file");

            migrationBuilder.AlterColumn<DateTime>(
                name: "IdentityExpiryDate",
                table: "Citizen",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true,
                oldComment: "Expiry date of the identity document");

            migrationBuilder.AlterColumn<string>(
                name: "HomeTelephone",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Telephone number");

            migrationBuilder.AlterColumn<int>(
                name: "GenderId",
                table: "Citizen",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Gender identifier");

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "Citizen",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldComment: "Full name of the citizen");

            migrationBuilder.AlterColumn<string>(
                name: "Fax",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Fax number");

            migrationBuilder.AlterColumn<string>(
                name: "Employer",
                table: "Citizen",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true,
                oldComment: "Employer information");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Citizen",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true,
                oldComment: "Email address");

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "Citizen",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true,
                oldComment: "District");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Citizen code");

            migrationBuilder.AlterColumn<Guid>(
                name: "CityId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the city");

            migrationBuilder.AlterColumn<string>(
                name: "BuildingNumber",
                table: "Citizen",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true,
                oldComment: "Building number");

            migrationBuilder.AlterColumn<DateTime>(
                name: "BirthDate",
                table: "Citizen",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Date of birth");

            migrationBuilder.AlterColumn<Guid>(
                name: "AreaId",
                table: "Citizen",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true,
                oldComment: "Unique identifier of the area");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorStatusId",
                table: "Citizen",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Status identifier");

            migrationBuilder.AlterColumn<string>(
                name: "AdditionalNumber",
                table: "Citizen",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true,
                oldComment: "Additional number");

            migrationBuilder.AlterColumn<int>(
                name: "StoppingReason",
                table: "ArbitratorStopFreeUpDecision",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComment: "Reason for stopping or freeing up the arbitrator");

            migrationBuilder.AlterColumn<string>(
                name: "LateLitigationRequestNumber",
                table: "ArbitratorStopFreeUpDecision",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldComment: "Number of the late litigation request");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionTo",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "End date of the arbitrator stop/free up decision period");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionFrom",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldComment: "Start date of the arbitrator stop/free up decision period");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DecisionDate",
                table: "ArbitratorStopFreeUpDecision",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the arbitrator stop/free up decision was made");

            migrationBuilder.AlterColumn<Guid>(
                name: "CitizenId",
                table: "ArbitratorStopFreeUpDecision",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldComment: "Unique identifier of the citizen");

            migrationBuilder.AlterColumn<int>(
                name: "ArbitratorStatusId",
                table: "ArbitratorStopFreeUpDecision",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldComment: "Status identifier");
        }
    }
}
