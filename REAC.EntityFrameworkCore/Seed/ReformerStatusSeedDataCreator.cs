using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{

    public static class ReformerStatusSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {

            if (await context.SysReformerStatuses.AnyAsync()) return;
            await context.SysReformerStatuses.AddRangeAsync(
                new List<SysReformerStatus>
                {
                    new SysReformerStatus
                    {
                        Id = ReformerStatus.Certified,
                        StatusName = "معتمد",
                        Translations=new List<SysReformerStatusTranslation>
                        {
                            new SysReformerStatusTranslation
                            {
                                StatusName = "معتمد",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysReformerStatusTranslation
                            {
                                StatusName = "Certified",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                    },
                    new SysReformerStatus
                    {
                        Id = ReformerStatus.Stopped,
                        StatusName = "موقوف",
                        Translations=new List<SysReformerStatusTranslation>
                        {
                            new SysReformerStatusTranslation
                            {
                                StatusName = "موقوف",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysReformerStatusTranslation
                            {
                                StatusName = "Stopped",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }
                    }
                });
            context.SaveChanges();
        }
    }
}
