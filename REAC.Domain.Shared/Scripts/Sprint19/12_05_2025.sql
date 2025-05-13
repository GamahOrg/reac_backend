CREATE TABLE [dbo].[LitigationRequestAmountLog](
	[Id] [uniqueidentifier] NOT NULL,
	[AmountId] [uniqueidentifier] NOT NULL,
	[LitigationRequestId] [uniqueidentifier] NOT NULL,
	[CreatedOn] [datetime2](7) NOT NULL,
 CONSTRAINT [PK__LitigAmount__3214EC0729841352] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LitigationRequestAmountLog]  WITH CHECK ADD  CONSTRAINT [FK_LitigationRequestAmountLog_LitigationRequest_LitigationRequestId] FOREIGN KEY([LitigationRequestId])
REFERENCES [dbo].[LitigationRequest] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[LitigationRequestAmountLog] CHECK CONSTRAINT [FK_LitigationRequestAmountLog_LitigationRequest_LitigationRequestId]
GO

ALTER TABLE [dbo].[LitigationRequestAmountLog]  WITH CHECK ADD  CONSTRAINT [FK_LitigationRequestAmountLog_SysLitigationRequestAmount_AmountId] FOREIGN KEY([AmountId])
REFERENCES [dbo].[SysLitigationRequestAmount] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[LitigationRequestAmountLog] CHECK CONSTRAINT [FK_LitigationRequestAmountLog_SysLitigationRequestAmount_AmountId]
GO

-----------------------------------------------
Alter table dbo.LitigationRequest
Add FeesDifference decimal (18, 2) NOT NULL

ALTER TABLE [dbo].[LitigationRequest] ADD  DEFAULT ((0.0)) FOR [FeesDifference]
----------------------------------------------