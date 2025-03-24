INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.LitigationRequestExtendRequestReplyPeriod', N'24')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.AddLitigationRequestArbitratorNominationListPeriod', N'24')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.AddLitigationRequestArbitratorNominationListWarning', N'برجاء سرعة اضافة قائمة المحكمين المرشحين لدعوي رقم {LitigationRequest}')
INSERT INTO SysSetting (Id, [Key], [Value]) VALUES (NEWID(), 'LitigationRequest.Creation.Accept.Defendant', N'تم تقديم دعوى رقم {LitigationCode} والمقامة ضدكم ');

------------------------------------------------------------------------------------------------------
Alter table dbo.LitigationRequestExtendPeriodRequest
Add CreatedBy uniqueidentifier null

Alter table dbo.LitigationRequestExtendPeriodRequest
Add CreatedOn datetime2 null

Alter table dbo.LitigationRequest
Add IsArbitratorAdminNominationListAddedPeriodSent bit null



------------------------------------------------------------------------------------------------------

INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.AddLitigationRequestArbitrationSessionPeriod', N'24')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'SLA.AddLitigationRequestArbitrationSessionWarning', N'برجاء سرعة تحديد موعد الجلسه لدعوي رقم {LitigationRequest}')
  

 Alter table dbo.LitigationRequest
Add IsArbitratorSessionTimeAddedPeriodSent bit null

----------------------------------------------------------------


INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'WafiData_URL', N'https://wafiservicesapi.rega.gov.sa/api/REAC/GetLicenseProjectPartiesData?licenseNumber={0}&identityNumber={1}')
