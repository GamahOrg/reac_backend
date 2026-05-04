
-- طلب تصحيح حكم التحكيم
CREATE TABLE [dbo].[LitigationRequestArbitrationCorrectionRequest] (
    [Id]                     UNIQUEIDENTIFIER NOT NULL,
    [LitigationRequestId]    UNIQUEIDENTIFIER NOT NULL,
    [RequestCitizenId]       UNIQUEIDENTIFIER NOT NULL,
    [CorrectionDescription]  NVARCHAR(1000)   NOT NULL,
    [RequestOn]              DATETIME2        NOT NULL,
    [RequestParty]           INT              NOT NULL,
    [ApproveRejectStatus]    INT              NULL,
    [RejectReason]           NVARCHAR(300)    NULL,
    [CorrectionDecisionText] NVARCHAR(2500)   NULL,
    [ResponseOn]             DATETIME2        NULL,
    [CreatedOn]              DATETIME2        NOT NULL,
    [CreatedBy]              UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__LitigationRequestArbitrationCorrectionRequest] PRIMARY KEY ([Id]),
    CONSTRAINT [FKLitigationRequestArbitrationCorrectionRequest]
        FOREIGN KEY ([LitigationRequestId]) REFERENCES [dbo].[LitigationRequest] ([Id]),
    CONSTRAINT [FKRequestCitizenArbitrationCorrectionRequest]
        FOREIGN KEY ([RequestCitizenId]) REFERENCES [dbo].[Citizen] ([Id])
);

CREATE UNIQUE INDEX [LitigationRequestArbitrationCorrectionRequest_Id]
    ON [dbo].[LitigationRequestArbitrationCorrectionRequest] ([Id]);
------------------------------------------

-- ملفات طلب تصحيح حكم التحكيم
CREATE TABLE [dbo].[LitigationRequestArbitrationCorrectionRequestFile] (
    [Id]                                        UNIQUEIDENTIFIER NOT NULL,
    [LitigationRequestArbitrationCorrectionRequestId] UNIQUEIDENTIFIER NOT NULL,
    [CorrectionRequestFileId]                   UNIQUEIDENTIFIER NOT NULL,
    [CreatedOn]                                 DATETIME2        NOT NULL,
    [CreatedBy]                                 UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__LitigationRequestArbitrationCorrectionRequestFile] PRIMARY KEY ([Id]),
    CONSTRAINT [FKArbitrationCorrectionRequestArbitrationCorrectionRequestFile]
        FOREIGN KEY ([LitigationRequestArbitrationCorrectionRequestId])
        REFERENCES [dbo].[LitigationRequestArbitrationCorrectionRequest] ([Id]),
    CONSTRAINT [FKCorrectionRequestFileArbitrationCorrectionRequestFile]
        FOREIGN KEY ([CorrectionRequestFileId]) REFERENCES [dbo].[SysFileManager] ([Id])
);

CREATE UNIQUE INDEX [LitigationRequestArbitrationCorrectionRequestFile_Id]
    ON [dbo].[LitigationRequestArbitrationCorrectionRequestFile] ([Id]);
------------------------------------------

-- سجل إعادة فتح الطلبات المغلقة
CREATE TABLE [dbo].[LitigationRequestReopenLogs] (
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [LitigationRequestId]       UNIQUEIDENTIFIER NOT NULL,
    [ReopenReason]              NVARCHAR(500)    NOT NULL,
    [PreviousInternalStatusId]  INT              NOT NULL,
    [NewInternalStatusId]       INT              NOT NULL,
    [PreviousStage]             INT              NULL,
    [NewStage]                  INT              NULL,
    [PreviousSubStage]          INT              NULL,
    [NewSubStage]               INT              NULL,
    [ReopenedOn]                DATETIME2        NOT NULL,
    [ReopenedBy]                UNIQUEIDENTIFIER NULL,
    [CreatedOn]                 DATETIME2        NOT NULL,
    [CreatedBy]                 UNIQUEIDENTIFIER NULL,
    CONSTRAINT [PK__LitigationRequestReopenLog] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LitigationRequestReopenLog_LitigationRequest]
        FOREIGN KEY ([LitigationRequestId]) REFERENCES [dbo].[LitigationRequest] ([Id]),
    CONSTRAINT [FK_LitigationRequestReopenLog_SysEmployee]
        FOREIGN KEY ([ReopenedBy]) REFERENCES [dbo].[SysEmployee] ([Id])
);

CREATE UNIQUE INDEX [LitigationRequestReopenLog_Id]
    ON [dbo].[LitigationRequestReopenLog] ([Id]);
------------------------------------------
