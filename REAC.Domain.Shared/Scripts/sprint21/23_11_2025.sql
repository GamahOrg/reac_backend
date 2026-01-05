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

------------------------------------------------------
INSERT [dbo].[AspNetUsers] ([Id], [RefreshToken], [ReferenceId], [RefreshTokenExpiryTime], [UserName], [NormalizedUserName], [Email], [NormalizedEmail], [EmailConfirmed], [PasswordHash], [SecurityStamp], [ConcurrencyStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEnd], [LockoutEnabled], [AccessFailedCount]) VALUES (N'f2ae37bd-b87f-4e73-8aa4-07dc99fe35f3', N'eLi97GkYIOl6iVM5XrDRwjZk+NzkCvWzvvWF+366aACVu+DHh+3nxk33IQF/XvVNhF8QrjCtn22T9ZpSSrnc2g==', N'7f14c6d6-8b17-4749-9c60-a07e191bc4f1', CAST(N'2025-12-16T16:30:23.9498453' AS DateTime2), N'integration', N'INTEGRATION', N'integration@domain.com', N'INTEGRATION@DOMAIN.COM', 1, N'AQAAAAIAAYagAAAAEF2wKb3hyA4C4UL3qY2clY72vs0TDPR9Z6VCMj47BAA2GPuMdXGjahP7IUZClHFCcA==', N'FE7ZSIB3O7HCPZTMCYBZEQLBXJWRXWU5', N'52a601f0-a7a1-4eac-b9a7-0e6e2b50b6af', NULL, 0, 0, NULL, 1, 0)
