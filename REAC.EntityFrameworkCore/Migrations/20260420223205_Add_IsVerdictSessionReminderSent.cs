using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REAC.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsVerdictSessionReminderSent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE Name = N'IsVerdictSessionReminderSent' AND Object_ID = Object_ID(N'LitigationRequest'))
                    ALTER TABLE [LitigationRequest] ADD [IsVerdictSessionReminderSent] bit NULL;
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name = N'LitigationRequestArbitrationCorrectionRequest')
                BEGIN
                    CREATE TABLE [LitigationRequestArbitrationCorrectionRequest] (
                        [Id] uniqueidentifier NOT NULL,
                        [LitigationRequestId] uniqueidentifier NOT NULL,
                        [RequestCitizenId] uniqueidentifier NOT NULL,
                        [CorrectionDescription] nvarchar(max) NOT NULL,
                        [RequestOn] datetime2 NOT NULL,
                        [RequestParty] int NOT NULL,
                        [ApproveRejectStatus] int NULL,
                        [RejectReason] nvarchar(max) NULL,
                        [CorrectionDecisionText] nvarchar(max) NULL,
                        [ResponseOn] datetime2 NULL,
                        [CreatedOn] datetime2 NOT NULL,
                        [CreatedBy] uniqueidentifier NULL,
                        CONSTRAINT [PK__LitigationRequestArbitrationCorrectionRequest] PRIMARY KEY ([Id]),
                        CONSTRAINT [FKLitigationRequestArbitrationCorrectionRequest] FOREIGN KEY ([LitigationRequestId]) REFERENCES [LitigationRequest] ([Id]),
                        CONSTRAINT [FKRequestCitizenArbitrationCorrectionRequest] FOREIGN KEY ([RequestCitizenId]) REFERENCES [Citizen] ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name = N'LitigationRequestReopenLogs')
                BEGIN
                    CREATE TABLE [LitigationRequestReopenLogs] (
                        [Id] uniqueidentifier NOT NULL,
                        [LitigationRequestId] uniqueidentifier NOT NULL,
                        [ReopenReason] nvarchar(max) NOT NULL,
                        [PreviousInternalStatusId] int NOT NULL,
                        [NewInternalStatusId] int NOT NULL,
                        [PreviousStage] int NULL,
                        [NewStage] int NULL,
                        [PreviousSubStage] int NULL,
                        [NewSubStage] int NULL,
                        [ReopenedOn] datetime2 NOT NULL,
                        [ReopenedBy] uniqueidentifier NULL,
                        [CreatedOn] datetime2 NOT NULL,
                        [CreatedBy] uniqueidentifier NULL,
                        CONSTRAINT [PK_LitigationRequestReopenLogs] PRIMARY KEY ([Id]),
                        CONSTRAINT [FK_LitigationRequestReopenLogs_LitigationRequest_LitigationRequestId] FOREIGN KEY ([LitigationRequestId]) REFERENCES [LitigationRequest] ([Id]) ON DELETE CASCADE,
                        CONSTRAINT [FK_LitigationRequestReopenLogs_SysEmployee_ReopenedBy] FOREIGN KEY ([ReopenedBy]) REFERENCES [SysEmployee] ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE Name = N'LitigationRequestArbitrationCorrectionRequestFile')
                BEGIN
                    CREATE TABLE [LitigationRequestArbitrationCorrectionRequestFile] (
                        [Id] uniqueidentifier NOT NULL,
                        [LitigationRequestArbitrationCorrectionRequestId] uniqueidentifier NOT NULL,
                        [CorrectionRequestFileId] uniqueidentifier NOT NULL,
                        [CreatedOn] datetime2 NOT NULL,
                        CONSTRAINT [PK__LitigationRequestArbitrationCorrectionRequestFile] PRIMARY KEY ([Id]),
                        CONSTRAINT [FKArbitrationCorrectionRequestArbitrationCorrectionRequestFile] FOREIGN KEY ([LitigationRequestArbitrationCorrectionRequestId]) REFERENCES [LitigationRequestArbitrationCorrectionRequest] ([Id]),
                        CONSTRAINT [FKCorrectionRequestFileArbitrationCorrectionRequestFile] FOREIGN KEY ([CorrectionRequestFileId]) REFERENCES [SysFileManager] ([Id])
                    );
                END
            ");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'IX_LitigationRequestArbitrationCorrectionRequest_LitigationRequestId')
                    CREATE INDEX [IX_LitigationRequestArbitrationCorrectionRequest_LitigationRequestId] ON [LitigationRequestArbitrationCorrectionRequest] ([LitigationRequestId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'IX_LitigationRequestArbitrationCorrectionRequest_RequestCitizenId')
                    CREATE INDEX [IX_LitigationRequestArbitrationCorrectionRequest_RequestCitizenId] ON [LitigationRequestArbitrationCorrectionRequest] ([RequestCitizenId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'LitigationRequestArbitrationCorrectionRequest_Id')
                    CREATE UNIQUE INDEX [LitigationRequestArbitrationCorrectionRequest_Id] ON [LitigationRequestArbitrationCorrectionRequest] ([Id]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'IX_LitigationRequestArbitrationCorrectionRequestFile_CorrectionRequestFileId')
                    CREATE INDEX [IX_LitigationRequestArbitrationCorrectionRequestFile_CorrectionRequestFileId] ON [LitigationRequestArbitrationCorrectionRequestFile] ([CorrectionRequestFileId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'IX_LitigationRequestArbitrationCorrectionRequestFile_LitigationRequestArbitrationCorrectionRequestId')
                    CREATE INDEX [IX_LitigationRequestArbitrationCorrectionRequestFile_LitigationRequestArbitrationCorrectionRequestId] ON [LitigationRequestArbitrationCorrectionRequestFile] ([LitigationRequestArbitrationCorrectionRequestId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'LitigationRequestArbitrationCorrectionRequestFile_Id')
                    CREATE UNIQUE INDEX [LitigationRequestArbitrationCorrectionRequestFile_Id] ON [LitigationRequestArbitrationCorrectionRequestFile] ([Id]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'IX_LitigationRequestReopenLogs_LitigationRequestId')
                    CREATE INDEX [IX_LitigationRequestReopenLogs_LitigationRequestId] ON [LitigationRequestReopenLogs] ([LitigationRequestId]);
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE Name = N'IX_LitigationRequestReopenLogs_ReopenedBy')
                    CREATE INDEX [IX_LitigationRequestReopenLogs_ReopenedBy] ON [LitigationRequestReopenLogs] ([ReopenedBy]);
            ");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LitigationRequestArbitrationCorrectionRequestFile");

            migrationBuilder.DropTable(
                name: "LitigationRequestReopenLogs");

            migrationBuilder.DropTable(
                name: "LitigationRequestArbitrationCorrectionRequest");

            migrationBuilder.DropColumn(
                name: "IsVerdictSessionReminderSent",
                table: "LitigationRequest");
        }
    }
}
