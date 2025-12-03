--------------- service id for invices ------------------------------------------------------------------
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Administrative_Fees_Billing', 1969)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Reconciliation_Fees_Billing', 1970)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Arbitration_single_arbitrator_Fees_Billing', 1971)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Arbitration_multiple_arbitrator_Fees_Billing', 1973)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Reformer_account_registration_Fees_Billing', 1974)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Reformer_membership_Fees_Billing', 1975)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Arbitrator_account_registration_Fees_Billing', 1976)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Arbitrator_membership_Fees_Billing', 1977)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Administrative_counterclaim_Fees_Billing', 1978)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Administrative_adding_additional_party_Fees_Billing', 1979)
INSERT [dbo].[SysSetting] ([Id], [Key], [Value]) VALUES (NEWID(), N'Expenses_travel_experts_translation_halls_Fees_Billing', 1980)

------------------------------------------------------------------------------
------role-Permission
insert into [dbo].[AspNetRoles] VALUES ('186DDF5C-4EA0-4093-891A-6218A76ED954','ArbitratorAdmin','ARBITRATORADMIN','e2416e30-88f6-497f-867d-e695332ace90')
insert into [dbo].[AspNetRoleClaims] values ('186DDF5C-4EA0-4093-891A-6218A76ED954','Permission','Arbitration.View')
update [dbo].[AspNetUserRoles] set roleId='186DDF5C-4EA0-4093-891A-6218A76ED954' where userid='998bb126-eb41-4a6a-a039-e85795c5dcbf'

insert into [dbo].[AspNetRoles] VALUES ('BD531BEE-AE07-42C5-ABEC-291F1A3AAEE0','ReformerAdmin','REFORMERADMIN','e2416e30-88f6-497f-867d-e695332ace80')
insert into [dbo].[AspNetRoleClaims] values ('BD531BEE-AE07-42C5-ABEC-291F1A3AAEE0','Permission','Reconciliation.View')
update [dbo].[AspNetUserRoles] set roleId='BD531BEE-AE07-42C5-ABEC-291F1A3AAEE0' where userid='A0C57BDE-D7BF-49CA-9662-9665EF3FC835'

insert into [dbo].[AspNetRoles] VALUES ('4422AD68-D49D-4769-B923-6457CFF50103','ReviewerAdmin','REVIEWERADMIN','e2416e30-88f6-497f-867d-e695332ace70')
insert into [dbo].[AspNetRoleClaims] values ('4422AD68-D49D-4769-B923-6457CFF50103','Permission','Review.View')
update [dbo].[AspNetUserRoles] set roleId='4422AD68-D49D-4769-B923-6457CFF50103' where userid='17840AB6-B134-4C5F-9273-5C97345010A3'
update [dbo].[AspNetUserRoles] set roleId='4422AD68-D49D-4769-B923-6457CFF50103' where userid='E4C57D8C-86DB-49E6-8F7C-A9948A6F860E'

--insert into [dbo].[AspNetRoleClaims] values ('B520BA17-8BDE-4868-A32D-08DD96F03296','Permission','Admin.View')
--update [dbo].[AspNetUserRoles] set roleId='B520BA17-8BDE-4868-A32D-08DD96F03296' where userid='C30016D7-556D-4A55-A4F0-EF9E5055A113'
----------------------------------------------------------------------------
---- invoice table 
alter table [LitigationRequestInvoice]
add serviceId nvarchar(50) null

--------------------------------------------------------
--------- update user id 
insert into [dbo].[AspNetRoleClaims] values ('4C443A60-DB4A-4657-D368-08DD4C38B65E','Permission','Admin.View')
update [dbo].[AspNetUserRoles] set roleId='4C443A60-DB4A-4657-D368-08DD4C38B65E' where userid='C30016D7-556D-4A55-A4F0-EF9E5055A113'
