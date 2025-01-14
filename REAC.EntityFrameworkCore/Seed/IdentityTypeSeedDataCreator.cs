using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class IdentityTypeSeedDataCreator
{
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysIdentityTypes.AnyAsync()) return;
        await context.SysIdentityTypes.AddRangeAsync(
            new List<SysIdentityType>
            {
                //هوية وطنية – هوية خليجية – إقامة
                    new SysIdentityType
                    {
                        Id = IdentityType.NationalIdentity,
                        IdentityType = "هوية وطنية",
                        Translations=new List<SysIdentityTypeTranslation>
                        {
                            new SysIdentityTypeTranslation
                            {
                                IdentityType = "National Identity",
                                LanguageCode=Constants.EnglishLanguage
                            },
                            new SysIdentityTypeTranslation
                            {
                                IdentityType = "هوية وطنية",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysIdentityType
                    {
                        Id = IdentityType.GulfIdentity,
                        IdentityType = "هوية خليجية",
                        Translations=new List<SysIdentityTypeTranslation>
                        {
                            new SysIdentityTypeTranslation
                            {
                                IdentityType = "Gulf Identity",
                                LanguageCode=Constants.EnglishLanguage
                            },
                            new SysIdentityTypeTranslation
                            {
                                IdentityType = "هوية خليجية",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysIdentityType
                    {
                        Id = IdentityType.Residence,
                        IdentityType = "إقامة",
                        Translations=new List<SysIdentityTypeTranslation>
                        {
                            new SysIdentityTypeTranslation
                            {
                                IdentityType = "Residence",
                                LanguageCode=Constants.EnglishLanguage
                            },
                            new SysIdentityTypeTranslation
                            {
                                IdentityType = "إقامة",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }
                    },
            });
        context.SaveChanges();
    }

}
