CREATE TABLE [dbo].[LitigationRequestInvoice] (
    [Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY DEFAULT NEWID(),
    
    [ReferenceId] NVARCHAR(50) NULL,
    [litigationRequestId] UNIQUEIDENTIFIER NOT NULL,
    [invDateJ] DATETIME2(7) NULL,
    [invDateH] NVARCHAR(20) NULL,
    [invTime] NVARCHAR(20) NULL,
    [invDueDate] DATETIME2(7) NULL,
    [sadadNumber] NVARCHAR(50) NULL,
    [sadadStatus] NVARCHAR(100) NULL,
    [invoiceType] NVARCHAR(100) NULL,
    [invoicedNumber] NVARCHAR(50) NULL,
    [description] NVARCHAR(500) NULL,
    [isDeleted] NVARCHAR(10) NULL,
    [deletedReason] NVARCHAR(500) NULL,
    [statusId] INT NULL,
    [reference] NVARCHAR(100) NULL,
    [source] NVARCHAR(200) NULL,
    [project] NVARCHAR(200) NULL,
    [client] NVARCHAR(200) NULL,
    [createdBy] NVARCHAR(200) NULL,
    [provinceName] NVARCHAR(100) NULL,
    [cityName] NVARCHAR(100) NULL,
    [neighborhoodName] NVARCHAR(100) NULL,
    [price] DECIMAL(18, 2) NULL,
    [totalQuantity] INT NULL,
    [totalPrice] DECIMAL(18, 2) NULL,
    [totalNonTaxable] DECIMAL(18, 2) NULL,
    [totalTaxable] DECIMAL(18, 2) NULL,
    [totalTax] DECIMAL(18, 2) NULL,
    [totalTotalAfterTax] DECIMAL(18, 2) NULL,
    
    [createdOn] DATETIME2(7) NULL,
    [updatedOn] DATETIME2(7) NULL,
    
    CONSTRAINT [FK_LitigationRequestInvoice_LitigationRequest] 
        FOREIGN KEY ([litigationRequestId]) 
        REFERENCES [dbo].[LitigationRequest]([Id])
        ON DELETE NO ACTION
);
GO

-- Create Indexes for Performance
CREATE NONCLUSTERED INDEX [IX_LitigationRequestInvoice_LitigationRequestId] 
    ON [dbo].[LitigationRequestInvoice]([litigationRequestId]);
GO

CREATE NONCLUSTERED INDEX [IX_LitigationRequestInvoice_ReferenceId] 
    ON [dbo].[LitigationRequestInvoice]([ReferenceId]);
GO

CREATE NONCLUSTERED INDEX [IX_LitigationRequestInvoice_SadadNumber] 
    ON [dbo].[LitigationRequestInvoice]([sadadNumber]);
GO

--------------------------------------------------------------
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'CreateInvoice_URL', N'https://wafiservicesapi.rega.gov.sa/api/v1/rega/fawtarah/initiate-invoice')
