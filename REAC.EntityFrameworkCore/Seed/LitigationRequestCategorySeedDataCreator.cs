using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class LitigationRequestCategorySeedDataCreator
{
    /// <summary>
    /// صلح – تحكيم-صلح وتحكيم
    /// </summary>
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysLitigationRequestCategories.AnyAsync()) return;
        await context.SysLitigationRequestCategories.AddRangeAsync(
            new List<SysLitigationRequestCategory>
            {
                    new SysLitigationRequestCategory
                    {
                        Id = Domain.Shared.Enum.LitigationRequestCategory.Reconciliation,
                        CategoryName = "صلح",
                        BpfId = Guid.Parse("15d87d0d-3cf4-4969-9c7e-933aeea0e5f5"),
                        Translations=new List<SysLitigationRequestCategoryTranslation>
                        {
                            new SysLitigationRequestCategoryTranslation
                            {
                                CategoryName = "Reconciliation",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestCategoryTranslation
                            {
                                CategoryName = "صلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestCategory
                    {
                        Id =  Domain.Shared.Enum.LitigationRequestCategory.Arbitration,
                        CategoryName = "تحكيم",
                        BpfId = Guid.Parse("8efe7ba7-25fa-4dd7-a299-e4744a81e217"),
                        Translations=new List<SysLitigationRequestCategoryTranslation>
                        {
                            new SysLitigationRequestCategoryTranslation
                            {
                                CategoryName = "Arbitration",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestCategoryTranslation
                            {
                                CategoryName = "تحكيم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestCategory
                    {
                        Id = Domain.Shared.Enum.LitigationRequestCategory.ReconciliationAndArbitration,
                        CategoryName = "صلح وتحكيم",
                        BpfId = Guid.Parse("15d87d0d-3cf4-4969-9c7e-933aeea0e5f5"),
                        Translations=new List<SysLitigationRequestCategoryTranslation>
                        {
                            new SysLitigationRequestCategoryTranslation
                            {
                                CategoryName = "Reconciliation and arbitration",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestCategoryTranslation
                            {
                                CategoryName = "صلح وتحكيم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
            });
        context.SaveChanges();
    }
}
