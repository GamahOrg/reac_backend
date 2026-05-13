-- =====================================================
-- Database Script for User Stories #75349 & #75350
-- Beneficiary Satisfaction Survey
-- =====================================================

-- Create BeneficiarySatisfactionSurvey table
CREATE TABLE [BeneficiarySatisfactionSurvey] (
    [Id]                        UNIQUEIDENTIFIER NOT NULL,
    [LitigationRequestId]       UNIQUEIDENTIFIER NOT NULL,       -- FK to LitigationRequest
    [CitizenId]                 UNIQUEIDENTIFIER NOT NULL,       -- FK to Citizen who submitted the survey
    [StageType]                 INT              NOT NULL,       -- 1 = Arbitration, 2 = Reconciliation, 3 = RequestSubmission
    [ProcedureSatisfaction]     INT              NULL,           -- Q1: Satisfaction with procedures (1-5), null if dismissed
    [PerformanceSatisfaction]   INT              NULL,           -- Q2: Satisfaction with performance (1-5), null for StageType=3
    [IsRated]                   BIT              NOT NULL,       -- True if submitted, false if skipped
    [CreatedBy]                 UNIQUEIDENTIFIER NULL,
    [CreatedOn]                 DATETIME2        NOT NULL,
    [UpdatedBy]                 UNIQUEIDENTIFIER NULL,
    [UpdatedOn]                 DATETIME2        NULL,

    CONSTRAINT [PK__BeneficiarySatisfactionSurvey] PRIMARY KEY ([Id]),

    CONSTRAINT [FK_BeneficiarySatisfactionSurvey_LitigationRequest]
        FOREIGN KEY ([LitigationRequestId])
        REFERENCES [LitigationRequest] ([Id]),

    CONSTRAINT [FK_BeneficiarySatisfactionSurvey_Citizen]
        FOREIGN KEY ([CitizenId])
        REFERENCES [Citizen] ([Id])
);

-- Index on CitizenId for lookup performance
CREATE INDEX [IX_BeneficiarySatisfactionSurvey_CitizenId]
    ON [BeneficiarySatisfactionSurvey] ([CitizenId]);

-- Unique constraint: one survey per citizen per stage per request
CREATE UNIQUE INDEX [IX_BeneficiarySatisfactionSurvey_Unique]
    ON [BeneficiarySatisfactionSurvey] ([LitigationRequestId], [CitizenId], [StageType]);
