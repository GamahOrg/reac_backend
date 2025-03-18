INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.LitigationRequestExtendRequestReplyPeriod', N'24')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.AddLitigationRequestArbitratorNominationListPeriod', N'24')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.AddLitigationRequestArbitratorNominationListWarning', N'برجاء سرعة اضافة قائمة المحكمين المرشحين لدعوي رقم {LitigationRequest}')

------------------------------------------------------------------------------------------------------
Alter table dbo.LitigationRequestExtendPeriodRequest
Add CreatedBy uniqueidentifier null

Alter table dbo.LitigationRequestExtendPeriodRequest
Add CreatedOn datetime2 null

Alter table dbo.LitigationRequest
Add IsArbitratorAdminNominationListAddedPeriodSent bit null