INSERT [dbo].[SysLitigationRequestOwnerType] ([Id], [LitigationRequestOwnerType], [Translations]) VALUES (13, N'ممثل عن جمعية', N'[{"LanguageCode":"en-US","LitigationRequestOwnerType":"Association Representitive"},{"LanguageCode":"ar","LitigationRequestOwnerType":"ممثل عن جمعية"}]')



-- Add EstablishmentLetterFileId column to Delegator
ALTER TABLE [Delegator]
ADD [EstablishmentLetterFileId] UNIQUEIDENTIFIER NULL;


-- Create index on EstablishmentLetterFileId
CREATE INDEX [IX_Delegator_EstablishmentLetterFileId]
ON [Delegator]([EstablishmentLetterFileId]);

-- Add foreign key constraint
ALTER TABLE [Delegator]
ADD CONSTRAINT [FK_Delegator_SysFileManager_EstablishmentLetterFileId]
FOREIGN KEY ([EstablishmentLetterFileId]) REFERENCES [SysFileManager]([Id]);


  update [dbo].[SysLitigationRequestOwnerType] set LitigationRequestOwnerType = N'أصالة عن نفسه' where id = 1

  alter table dbo.citizen 
  add Mobile2 nvarchar(20) null
  alter table dbo.citizen 
  add Email2 nvarchar(250) null
