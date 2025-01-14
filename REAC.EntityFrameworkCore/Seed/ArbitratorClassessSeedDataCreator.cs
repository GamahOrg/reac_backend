using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public partial class ArbitratorClassessSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysArbitratorClasses.AnyAsync()) return;
            await context.SysArbitratorClasses.AddRangeAsync(new List<SysArbitratorClass>
            {
                new SysArbitratorClass
                {
                    Id = new Guid("c37d85cc-ea3b-41a9-b89f-ab6d82cbebea"),
                    ArbitratorClassName = "أ",
                    YearsFrom = 1,
                    YearsTo = 3,
                    Translations=new List<SysArbitratorClassTranslation>
                    {
                        new SysArbitratorClassTranslation
                        {
                            ArbitratorClassName="A",
                            LanguageCode=Constants.EnglishLanguage
                        }
                        ,
                        new SysArbitratorClassTranslation
                        {
                            ArbitratorClassName="أ",
                            LanguageCode=Constants.ArabicLanguage
                        }
                    }
                },
                new SysArbitratorClass
                {
                    Id = new Guid("77f6e852-1b49-4720-a786-f31e836a1060"),
                    ArbitratorClassName = "ب",
                    YearsFrom = 4,
                    YearsTo = 8,
                    Translations=new List<SysArbitratorClassTranslation>
                    {
                        new SysArbitratorClassTranslation
                        {
                            ArbitratorClassName="B",
                            LanguageCode=Constants.EnglishLanguage
                        }
                        ,
                        new SysArbitratorClassTranslation
                        {
                            ArbitratorClassName="ب",
                            LanguageCode=Constants.ArabicLanguage
                        }
                    }
                }
                ,
                new SysArbitratorClass
                {
                    Id = new Guid("bae17b94-28ea-442d-89b5-2b99cb957feb"),
                    ArbitratorClassName = "ج",
                    YearsFrom = 9,
                    YearsTo = 15,
                    Translations=new List<SysArbitratorClassTranslation>
                    {
                        new SysArbitratorClassTranslation
                        {
                            ArbitratorClassName="C",
                            LanguageCode=Constants.EnglishLanguage
                        }
                        ,
                        new SysArbitratorClassTranslation
                        {
                            ArbitratorClassName="ج",
                            LanguageCode=Constants.ArabicLanguage
                        }
                    }
                }
            });
            context.SaveChanges();

        }
    }
}