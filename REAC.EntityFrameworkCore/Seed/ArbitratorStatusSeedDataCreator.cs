using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{

    public static class ArbitratorStatusSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {

            if (await context.SysArbitratorStatuses.AnyAsync()) return;
            await context.SysArbitratorStatuses.AddRangeAsync(
                new List<SysArbitratorStatus>
                {
                    new SysArbitratorStatus
                    {
                        Id = ArbitratorStatus.Certified,
                        StatusName = "معتمد",
                        Translations=new List<SysArbitratorStatusTranslation>
                        {
                            new SysArbitratorStatusTranslation
                            {
                                StatusName = "معتمد",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysArbitratorStatusTranslation
                            {
                                StatusName = "Certified",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                    },
                    new SysArbitratorStatus
                    {
                        Id = ArbitratorStatus.Stopped,
                        StatusName = "موقوف",
                        Translations=new List<SysArbitratorStatusTranslation>
                        {
                            new SysArbitratorStatusTranslation
                            {
                                StatusName = "موقوف",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysArbitratorStatusTranslation
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
