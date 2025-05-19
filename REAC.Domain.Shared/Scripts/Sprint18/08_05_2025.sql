-- Ensure the `RequestId` column in `SysInvoice` is unique
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT UQ_SysInvoice_RequestId UNIQUE (RequestId);

-- Add the foreign key constraint
ALTER TABLE [dbo].[Invoices]
ADD CONSTRAINT FK_LitigationRequest_SysInvoice
FOREIGN KEY (RequestId) REFERENCES LitigationRequest(Id)
ON DELETE NO ACTION;