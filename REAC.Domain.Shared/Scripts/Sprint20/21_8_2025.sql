  alter table dbo.[LitigationRequestArbitratorNominationList] 
  Add IsEscalated bit null
 
  alter table dbo.[LitigationRequestArbitratorNominationList] 
  Add EscalateDate DateTime null
  
   alter table dbo.[LitigationRequestArbitratorNominationList] 
  Add IsSelectedFromManager bit null


  alter table dbo.[LitigationRequestArbitratorNominationList] 
  Add IsPlaintiffSelected bit null

  alter table dbo.[LitigationRequestArbitratorNominationList] 
  Add IsDefendantSelected bit null

  alter table dbo.[LitigationRequestArbitratorNominationList] 
  Add IsResponse bit null

  ------------------------------------------------------------------
    alter table dbo.[LitigationRequestArbitratorsChairmanNominationList] 
  Add IsEscalated bit null
 
  alter table dbo.[LitigationRequestArbitratorsChairmanNominationList] 
  Add EscalateDate DateTime null
  
   alter table dbo.[LitigationRequestArbitratorsChairmanNominationList] 
  Add IsSelectedFromManager bit null

  alter table dbo.[LitigationRequestArbitratorsChairmanNominationList] 
  Add Response int null

  ----------------------------------------------
  ----------- Area distances=---------------------------
GO
CREATE TABLE [dbo].[SysAreaDistances](
	[Id] [uniqueidentifier] NOT NULL,
	[SourceAreaId] [uniqueidentifier] NOT NULL,
	[DestinationAreaId] [uniqueidentifier] NOT NULL,
	[Distance] [decimal](10, 2) NOT NULL,
 CONSTRAINT [PK__SysAreaDistances_3214EC0743341C89] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'6ccf00d5-6927-4d28-bd4f-01800cc132b7', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(904.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'47608da7-454a-45d7-bb9f-061e16be11de', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(878.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'572fbdf2-364d-4277-8c94-0870cd00f065', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(202.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'aac4a8fd-2dd0-464f-aef6-0a86c2bc2f27', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(338.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'bc1e4a3a-d7c6-4f57-bfcb-0caabfe15404', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(1709.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'56e3c14a-5763-4894-b6a0-0e54b08c8996', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(1512.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'9bf25168-cc0d-4bf7-bc4e-102bd64a2a86', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(972.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'bc89a858-9a79-43c9-bc84-12ea67ba8fb0', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(1445.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'0cdf7b4e-1df8-4fb2-ac97-156457e0a2d1', N'ed626869-b920-4cfd-81fb-616544b42674', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(1409.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'1149c12b-d1d7-4a8a-8e00-15db1fb3f554', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(777.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'f9b7d690-9643-4116-b25c-161fe2b0f01c', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1769.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'e8175fc0-3881-405d-9700-187228adca06', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(876.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7fab0f42-e8d1-4f5d-9650-1ae0b9e4aa9f', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(1072.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'102c3776-83b3-4a92-b44f-2044aa29d40d', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(1115.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'76be5675-0dc8-4bc3-8234-20bc825a9013', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(1158.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'51e23aa3-9340-40ad-a421-241794416359', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(1760.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'ca672584-ca80-47e2-be11-244373168ee9', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(2055.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'06e9f731-1e16-4e3f-80ba-254c4f3d04d4', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(231.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'334a6f9c-493d-4f77-b4f2-2a61c2e6127d', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(1551.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'fe51eb90-94da-4897-af3b-2cc05d825dc3', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(153.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'ad6670d0-913a-4016-98c9-2d9dfb23abd9', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(711.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'33b2fd4a-4ed2-4e30-b625-2dd879afad5a', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(447.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'74c5bced-e58d-4052-9b8c-35bd7116d644', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(330.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'78217a3c-fae6-4468-ac71-35c6a6c49ef5', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(828.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'06c6f442-6c2b-4bf0-9ccf-36afc25b4e5f', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(202.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'6a65f42a-cfcd-4b40-a06b-376cc30f1b70', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(950.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'a3b3ed91-e4d2-4514-bff7-387eefc85c79', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(330.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'0af26e0d-fbb2-4f78-8fc1-38f6d09bc7c3', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(153.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'570f2ad1-069a-4196-aeb3-391f16b372f2', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(450.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'aae51fa3-5aaa-4fe6-b3ca-3ae0632a6d69', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(636.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'874f8572-b2cf-4661-b0c6-3e890acfcb1e', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(1158.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'b3ba126b-535d-4e30-a6d3-3ebcde717514', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(1512.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7fcacedc-9752-45a4-b459-44bf376b49d9', N'ed626869-b920-4cfd-81fb-616544b42674', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(480.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'768c54b8-8a52-4f37-b13f-4703bd5cb561', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(733.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'8aaeeaae-27a0-4b07-b64a-47c6b5bb1555', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(2106.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'39d9e483-4b4c-4be3-879a-4964178ed064', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(1719.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'cb633a3d-1bcd-4b3d-a4ad-497b7cded211', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(1288.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'685dad2f-b734-4d4b-9c9f-4b2f8d687bc5', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(904.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'e1e501f2-6fcc-44ab-8d35-4d3bed90fb7c', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(2131.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'759c6fae-d46f-49e9-a59d-4e86e2007cd8', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(878.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'15538aad-b4aa-4a56-ba82-5259dffb5603', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(508.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'a6244442-af08-42a2-9817-560b3226fd0f', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(638.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'5d14e1f2-b8db-4f3a-a339-56252b069069', N'ed626869-b920-4cfd-81fb-616544b42674', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(1904.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'e6479d83-d4c4-40d4-81d0-5645cb405a47', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(450.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'5c052878-91de-4cc6-ad57-5853e975a454', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(878.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'692eec23-18bc-45af-8751-59edce820208', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(638.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'34c686cd-e8f6-4832-ad53-6169f02f13b8', N'ed626869-b920-4cfd-81fb-616544b42674', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(1709.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'cf7d6e6c-3b0d-4574-af25-63f928ffd931', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(280.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7220e2db-130d-4427-8e99-642b70b4188a', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(1373.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'0167fb2a-2e93-4e21-9959-64a49f2007c5', N'ed626869-b920-4cfd-81fb-616544b42674', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(684.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'5fc85c1e-4ae3-44bf-afc1-65c824043d17', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(1831.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'09abcbf4-f638-4e91-91da-66b5dd0e6548', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(1050.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'53f8dcc5-2bb2-4fdd-954b-6830cb0ef58d', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(355.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'29f80beb-2738-48ee-834c-685d290a8d66', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(1409.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'a0bd8a46-098b-466c-b32a-68851fd951c1', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(289.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'0afce203-fa6f-4d72-8e42-6936aa0a2c48', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(478.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'16d0dc79-cfa4-4538-b8c1-6ac61623f020', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(1831.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7c16e1c9-a3b3-41e6-b370-6bd877d6c2ea', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(1424.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'8c42c7da-9c88-4c2f-bf60-6d4ea1f39286', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1241.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'1a5fe5dc-6e36-432d-8f80-6d685d0b78f3', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1273.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'cd466160-858b-4c66-b964-6ec5d0db713a', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(480.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'397a333e-81c7-401b-94f7-703c3dc6c21f', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(1719.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'f48dfa95-cb4a-499d-92ed-711f61f50841', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(733.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'bcb083b9-66cb-4fc8-8e50-71d9fd5edd04', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1369.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'a9bed66e-1674-4973-bced-720301795ab4', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(1115.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'5228df64-fd76-4c6b-af83-72aa7baea6cf', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(1072.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'937380af-3b8d-4fef-a405-73b2ed4907b4', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(1241.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'd62264db-daa3-44a9-8bfa-73d441183066', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(395.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'e36fabcc-e7c2-403f-8e61-763a979c188e', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(1273.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'384832b9-bfd0-4b3b-b7b5-769c9aeb23c1', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(1217.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7098be9a-49a7-4be3-9471-7a347e58d6f7', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(1907.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'51d8cb7f-e00f-46f2-bd6d-7bd7ee03cda9', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(1551.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'cdf359db-2789-45fc-80e4-7d350e535b9e', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(1627.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'88eb9fbc-0ca1-481b-8c10-7dc856ea15aa', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(1769.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'd37c6814-c6a8-45c5-9c60-7dfa7be22e17', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(1274.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'791ff8b9-93f1-4f65-b5cb-84fe822e6186', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(1369.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'3656870e-4c4d-41c7-8411-85077fabeeaf', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(1050.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'6558669f-1b67-479a-9ace-873337e545a1', N'ed626869-b920-4cfd-81fb-616544b42674', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(2106.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'ef2f904f-b555-4f0a-928e-876b874021d7', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(1495.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'd18badb1-4436-4f3a-a47b-88c6ebfd57c6', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(1904.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'c652be9b-9caf-44a4-867e-8a4ced3a1375', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(1495.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'e6074070-975c-4c5e-ba74-8c3d6f687b71', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(974.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'13f13f47-29e9-467a-ad9c-8c796b633620', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(338.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'fc57d88f-942a-4072-9057-8d0783c494ec', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(508.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'f1811261-cab6-47cf-89d6-8d4a78911b79', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(712.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'16d27caf-4f0f-4d40-94ac-8ebcfaa5601c', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(913.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'fcf94b67-786a-4aa2-92a6-8ee1f7a684ea', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(662.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'193cf4c7-06e2-44a8-a358-8f8a6bbc9c27', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(1140.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'c5f18838-dc35-41fa-8b26-8f94ac285ed7', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(974.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'df955723-423c-4db3-b0df-900a7947e0c1', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1445.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'5bd38d15-cbe6-49a7-b6bf-942a6436e926', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(1038.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'46a90568-2dc4-4088-aba7-94ce9ad6d692', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(828.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'9d55da69-dc1b-4704-9a2f-94da10a7688c', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(878.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'1eeedf27-369f-4996-8988-95b7026bd5d1', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1033.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'c8804c73-cfa5-4ed4-855a-980e69866fa4', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(1424.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'df0da49e-f403-4e0a-8629-9dfe127c87f5', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(310.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'd1f061ff-d450-48b7-b618-9ed16e6eb63b', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(1760.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'072db72f-b1a0-4991-a6ec-9f31ee0632ea', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(326.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'79e14888-8033-4025-9f2a-a026e5a3652a', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(625.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'2bf705ed-84b7-44b8-b032-a21fa30292f0', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(1217.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7a06e46c-500b-4d4c-9999-a308c09d9984', N'ed626869-b920-4cfd-81fb-616544b42674', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(662.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'b84dfb85-aaac-4342-b1b1-a541746f9cc9', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(1083.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'63f6e3cf-c5d4-4d25-800c-a5920ce3be07', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(1627.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'377a0f4a-5f5a-42e4-ac3c-a5b392ac73a1', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(1907.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'66e62df5-7383-4471-9574-a619979f17eb', N'ed626869-b920-4cfd-81fb-616544b42674', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(556.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'd4dce71d-28bb-4ed8-b224-a873e8d4ae88', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(1424.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7ce87c11-0b97-49db-8aeb-ad06a2cb5ffd', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(447.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'803c0b42-3580-4b5c-9e40-ad4637edf2de', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(395.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'4bda3d7a-e874-4c2c-9539-ae6f180e011f', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1140.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'684e1b80-6102-482d-ba7d-b178d4f7428e', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(1083.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'233175b9-0834-4cf5-b2fa-b4f459ae4312', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(231.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'6fae7513-8605-4abb-8682-b64658226725', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(2055.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'07681cf0-4bfb-40ee-bdc8-bc473f8fed7d', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(1705.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'75811af4-c46d-4818-9636-bdbfa8918b86', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(355.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'fd84609d-4351-4fbd-b625-be3b9a58adf6', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(402.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'a8f44757-6757-40e4-99c4-be8a3452947b', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(1288.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'b2ef67c0-ead2-4e9d-aabe-c07a85dc0138', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(636.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'f4312eef-123f-461e-b89a-c362e6905c85', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(711.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'9b44df16-60e3-40aa-b709-c41eac0a4890', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', CAST(326.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'3a86289a-65da-4ae6-b6d8-c4c8247992e5', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(1836.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'6ca77431-bda9-4a7e-b41f-c509f4bb6d3a', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(556.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'a2844880-3013-400d-b4c9-c5a10f58c1cd', N'ed626869-b920-4cfd-81fb-616544b42674', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(1310.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'd7856e43-f7ac-48e0-9f02-c601fc859808', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(402.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'f8ee2f2b-47fe-461c-b5d4-c7e0b99d315b', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(913.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'56345060-a41e-41be-922c-c9739d29d131', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(1274.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'734c16ca-28e5-4c67-a53e-c9ee2d617bd4', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(712.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'507f23da-8409-40aa-a09b-cc6c3a71b1fc', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(1836.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'b63ae4b5-13dc-430e-92fc-ccac740f314a', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1373.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'638b8b01-a4fc-46dd-856b-cecde947e3be', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(1500.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'b80736b1-cb59-4f6d-be79-d2695527e7cb', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(1360.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'8b02b875-3edf-4c16-b0e2-d55b91da76b0', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1474.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'dd33de09-a2e1-47ab-a3f0-d8ee5fac0d8b', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(310.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'e1e2e62d-ebdd-4261-ad91-d9a37e6952e8', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(1033.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'c4cddc5b-015b-4831-89ac-db3bfe816a74', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(625.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'e43f2016-b284-4363-92cf-db9bfca2cba2', N'ed626869-b920-4cfd-81fb-616544b42674', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(1038.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'57d41a32-faec-40b9-9504-dd156b1f9b16', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(1310.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'd9a4109f-e2be-4c97-8913-df794c212110', N'7355b4f0-9271-46ca-88bd-3c0eb8bef67a', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', CAST(280.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'32921525-52ba-4900-92ec-e1685bf0fb60', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(1474.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'8350190b-80a8-4c31-baa9-e31ada0c2fac', N'28117125-7275-48e3-a2cf-afc7d1644a30', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(876.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'31f5a019-2e8f-4915-ae1d-e65fd5045479', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', CAST(846.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'157b47a7-36eb-4102-b667-e7a8e5e7a5e8', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'ed626869-b920-4cfd-81fb-616544b42674', CAST(684.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'cf128f0e-7b3b-4f27-a2a6-ea8c9fe25df2', N'ed626869-b920-4cfd-81fb-616544b42674', N'e8743c10-7a9f-410f-9b67-800e1bafc5d2', CAST(972.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'dca9c4f3-4271-43e3-97d9-ebb1d96a2629', N'ed626869-b920-4cfd-81fb-616544b42674', N'd3fcb090-2a6e-4389-8eb7-a089a4cf4c0e', CAST(1705.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'ed1aee14-0b2b-4bcd-8c92-ebb780a758d1', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(478.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'0a471809-99ab-4cdf-8e42-ec0bdf9c4172', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(1215.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'f06b3ff0-b771-4d3c-b000-eeadf6864730', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(289.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'fb1a0bde-bca6-46ac-bd94-efc6ef10e2c9', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(979.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'7ee46141-341c-4d3f-b2d3-f00aa23bdf79', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(950.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'2e72e415-c2ce-4494-8264-f6414b577f32', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'28117125-7275-48e3-a2cf-afc7d1644a30', CAST(1500.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'fa47a04f-d4b7-49ad-ac23-f658f9b77a71', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(777.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'b8dfdd41-e593-4a1a-b344-f6c9e3dd5530', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', N'd6f1a49d-e2a7-4d9b-86d6-0f1d6a941c79', CAST(1215.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'1932e0d4-105c-4623-b465-f7eb9261ae61', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', CAST(846.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'504c120d-b02e-4dbd-854f-f9af70c54e66', N'c37d85cc-ea3b-41a9-b89f-ab6d82cbebea', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', CAST(979.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'a52acdde-64a2-4087-a8e3-fda8119ca8a3', N'86efb2d9-4b25-4e46-b13d-4ec1cf8048d7', N'5e3d2ba0-c6dd-491e-aeb0-3dada690bcdc', CAST(2131.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'9910af0a-0a46-4a1e-aec7-ff2f44698938', N'0a692dce-3b9a-45eb-9d47-1c4cbfba0761', N'52101b9b-ec02-4474-a42a-a1f1fe0f881c', CAST(1424.00 AS Decimal(10, 2)))
GO
INSERT [dbo].[SysAreaDistances] ([Id], [SourceAreaId], [DestinationAreaId], [Distance]) VALUES (N'51bb79a7-0496-426d-bb58-ff9a6d84d0a0', N'6a1afc80-af39-46c9-ae2a-4bf4bbf36040', N'1f93fa8f-7d53-48ac-b88a-181b5e406a7b', CAST(1360.00 AS Decimal(10, 2)))
GO
