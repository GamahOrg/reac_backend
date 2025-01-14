using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public class CitizenRequestTypeSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysCitizenRequestType.AnyAsync()) return;
            await context.SysCitizenRequestType.AddRangeAsync(
                new List<SysCitizenRequestType>
                {
                    new SysCitizenRequestType
                    {
                        Id = CitizenRequestType.Reformer,
                        CitizenRequestTypeName="مصلح",
                        BpfId = Guid.Parse("b7871240-69df-48cf-b9d4-2f644cb431d2"),
                        Translations=new List<SysCitizenRequestTypeTranslation>
                        {
                            new SysCitizenRequestTypeTranslation
                            {
                                CitizenRequestTypeName="مصلح",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysCitizenRequestTypeTranslation
                            {
                                CitizenRequestTypeName="Reformer",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysCitizenRequestType
                    {
                        Id = CitizenRequestType.Arbitrator,
                        CitizenRequestTypeName="محكم",
                        BpfId = Guid.Parse("dd6d470a-d29d-4545-82b1-32ffc82182cc"),

                        Translations=new List<SysCitizenRequestTypeTranslation>
                        {
                            new SysCitizenRequestTypeTranslation
                            {
                                CitizenRequestTypeName="محكم",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysCitizenRequestTypeTranslation
                            {
                                CitizenRequestTypeName="Arbitrator",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                    }
                });
            context.SaveChanges();
        }
    }
}
