
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'SLA.ArbitratorRegisterationFeesPeriod', N'72')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'SLA.ArbitratorRegisterationFeesFirstNotificationPeriod', N'48')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'SLA.ArbitratorRegisterationFeesNotification', N'برجاء دفع رسوم التسجيل كمحكم قبل انتهاء المدة المحددة لذلك')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'SLA.ArbitratorMembershipFeesPeriod', N'72')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'SLA.ArbitratorMembershipFeesFirstNotificationPeriod', N'48')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'SLA.ArbitratorMembershipFeesNotification', N'برجاء دفع رسوم العضوية كمحكم قبل انتهاء المدة المحددة لذلك')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'SLA.ArbitratorMembershipRenewalFeesNotification', N'برجاء دفع رسوم تجديد العضوية كمحكم قبل انتهاء المدة المحددة لذلك')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'Configurable.ArbitratorRegisterationFees', N'200')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'Configurable.ArbitratorMembershipFees', N'250')
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NewID(), N'LitigationRequest.ModificationRequest', N'تم أضافة طلب تعديل علي الدعوى رقم {LitigationCode}')
-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
INSERT INTO [dbo].[SysSetting] (Id, [Key], [Value])
VALUES 
    (NEWID(), 'Review.Cancelled', 'تم إلغاء المهمة  رقم {LitigationCode}'),
    (NEWID(), 'Review.ReAssigned', 'تم إرسال مهمة مدقق الى مدقق اّخر المهمة  رقم {LitigationCode}');

DELETE FROM [dbo].[LitigationRequestFees];
---------------------------------------------------------------------------------------------------------------------------------------------------------------------
Insert into dbo.LitigationRequestFees
Select NEWID() , 1 , 100.00, null , null , Id
from dbo.SysLitigationRequestAmount
Insert into dbo.LitigationRequestFees
Select NEWID() , 3 , 100.00, null , null , Id
from dbo.SysLitigationRequestAmount
Insert into dbo.LitigationRequestFees
Select NEWID() , 5 , 100.00, null , null , Id
from dbo.SysLitigationRequestAmount
Insert into dbo.LitigationRequestFees
Select NEWID() , 7 , 100.00, null , null , Id
from dbo.SysLitigationRequestAmount
Insert into dbo.LitigationRequestFees
Select NEWID() , 9 , 100.00, null , null , Id
from dbo.SysLitigationRequestAmount