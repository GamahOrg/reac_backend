using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class GenderSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysGenders.AnyAsync()) return;
            await context.SysGenders.AddRangeAsync(
                new List<SysGender>
                {
                    new SysGender
                    {
                        Id = Gender.Male,
                        Gender="ذكر",
                        Translations=new List<SysGenderTranslation>
                        {
                            new SysGenderTranslation
                            {
                                Gender="ذكر",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysGenderTranslation
                            {
                                Gender="Male",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysGender
                    {
                        Id = Gender.Female,
                        Gender="أنثى",
                        Translations=new List<SysGenderTranslation>
                        {
                            new SysGenderTranslation
                            {
                                Gender="أنثى",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysGenderTranslation
                            {
                                Gender="Female",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                    }
                });
            context.SaveChanges();
        }
    }
}
