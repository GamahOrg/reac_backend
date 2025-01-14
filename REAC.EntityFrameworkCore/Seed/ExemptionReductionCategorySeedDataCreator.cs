using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public partial class ExemptionReductionCategorySeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysExemptionReductionCategories.AnyAsync()) return;
            await context.SysExemptionReductionCategories.AddRangeAsync(new List<SysExemptionReductionCategory>
            {
                new SysExemptionReductionCategory
                {
                    Id = Guid.NewGuid(),
                    ExemptionReductionCategoryName = "أسر السجناء",
                    Translations=new List<SysExemptionReductionCategoryTranslation>
                        {
                            new SysExemptionReductionCategoryTranslation
                            {
                                ExemptionReductionCategoryName = "Families of prisoners",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysExemptionReductionCategoryTranslation
                            {
                                ExemptionReductionCategoryName = "1000001-5000000",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                },
                new SysExemptionReductionCategory
                {
                    Id = Guid.NewGuid(),
                    ExemptionReductionCategoryName = "أسر شهداء الواجب",
                    Translations=new List<SysExemptionReductionCategoryTranslation>
                        {
                            new SysExemptionReductionCategoryTranslation
                            {
                    ExemptionReductionCategoryName = "أسر شهداء الواجب",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysExemptionReductionCategoryTranslation
                            {
                    ExemptionReductionCategoryName = "Families of martyrs of duty",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                },
                new SysExemptionReductionCategory
                {
                    Id = Guid.NewGuid(),
                    ExemptionReductionCategoryName = "مستفيدي الضمان الاجتماعي",
                    Translations=new List<SysExemptionReductionCategoryTranslation>
                        {
                            new SysExemptionReductionCategoryTranslation
                            {
                    ExemptionReductionCategoryName = "مستفيدي الضمان الاجتماعي",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysExemptionReductionCategoryTranslation
                            {
                    ExemptionReductionCategoryName = "Social security beneficiaries",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                }

            }
            );
            context.SaveChanges();

        }
    }
}