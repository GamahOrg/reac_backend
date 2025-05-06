INSERT [dbo].[SysLitigationRequestStatus] ([Id], [RequestStatus], [CreatedBy], [CreatedOn], [UpdatedBy], [UpdatedOn], [IsDeleted], [DeletedBy], [DeletedOn], [Translations]) VALUES (36, N'بانتظار اختيار محكم', NULL, CAST(N'2025-03-17T13:26:52.4459584' AS DateTime2), NULL, NULL, 0, NULL, NULL, N'[{"LanguageCode":"en-US","RequestStatus":"WaitingForArbitratorsSelection"},{"LanguageCode":"ar","RequestStatus":"بانتظار اختيار المحكمين"}]')
-------------------
  --adding this script to sprint 18
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Arbitrator.UpdateJoiningRequest.ToPayEditFees', N'تنويه برجاء دفع رسوم تعديل طلب الانضمام كمحكم {CitizenName}')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Employee.Arbitrator.UpdateJoiningRequest.ToPayEditFees', N' تم ارسال طلب {CitizenName} للانضمام كمحكم لدفع رسوم تعديل الطلب')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Arbitrator.UpdateJoiningRequest.PaidEditFees', N'تم دفع رسوم طلب تعديل الطلب {CitizenName} للانضمام كمحكم ')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Employee.Arbitrator.UpdateJoiningRequest.PaidEditFees', N'تم دفع رسوم طلب تعديل الطلب {CitizenName} للانضمام كمحكم ')
 -----------
INSERT [dbo].SysCitizenRequestStatus ([Id], [RequestStatus], [Translations]) VALUES (10, N'تم الارسال لدفع رسوم تعديل طلب الانضمام', N'[{"LanguageCode":"en-US","RequestStatus":"Sent To Pay Edit Fees"},{"LanguageCode":"ar","RequestStatus":"\u062A\u0645 \u0627\u0644\u0627\u0631\u0633\u0627\u0644 \u0644\u062F\u0641\u0639 \u0627\u0644\u0631\u0633\u0648\u0645"}]')
INSERT [dbo].SysCitizenRequestStatus ([Id], [RequestStatus], [Translations]) VALUES (11, N'تم دفع رسوم تعديل طلب الانضمام', N'[{"LanguageCode":"en-US","RequestStatus":"Fees Edit Paid"},{"LanguageCode":"ar","RequestStatus":"\u062A\u0645 \u062F\u0641\u0639 \u0627\u0644\u0631\u0633\u0648\u0645"}]')
--------------
INSERT INTO SysLitigationRequestStatus (Id, RequestStatus, CreatedOn, IsDeleted, Translations)
VALUES (
    35,  
    N'بإنتظار تحديد عدد هيئة التحكيم',  
    GETDATE(),
	0,
    N'[
        {"LanguageCode":"en-US", "RequestStatus":"Waiting For Determine Arbitrators Number"},
        {"LanguageCode":"ar", "RequestStatus":"بإنتظار تحديد عدد هيئة التحكيم"}
    ]'  
);