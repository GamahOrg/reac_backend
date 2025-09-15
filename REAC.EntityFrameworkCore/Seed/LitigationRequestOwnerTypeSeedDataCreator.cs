using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class LitigationRequestOwnerTypeSeedDataCreator
{
    
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysLitigationRequestOwnerTypes.AnyAsync()) return;
        await context.SysLitigationRequestOwnerTypes.AddRangeAsync(
            new List<SysLitigationRequestOwnerType>
            {
                    new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.Personal,
                        LitigationRequestOwnerType = "مقدم عن نفسه",
                        Translations = new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Personal",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "مقدم عن نفسه",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.AgentForIndividual,
                        LitigationRequestOwnerType = "وكيل فرد",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Agent For Individual",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "وكيل فرد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.IndividualRepresentative,
                        LitigationRequestOwnerType = "ممثل فرد",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Individual Representative",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "ممثل فرد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.AgentForCompany,
                        LitigationRequestOwnerType = "وكيل منشأة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Agent For Company",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "وكيل منشأة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                       new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.CompanyRepresentative,
                        LitigationRequestOwnerType = "ممثل منشأة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Company Representative",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "ممثل منشأة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },

                       // NEW
                       new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.CompanyManager,
                        LitigationRequestOwnerType = "مدير شركة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Company Manager",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "مدير شركة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.CompanyCEO,
                        LitigationRequestOwnerType = "رئيس مجلس الادارة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Company CEO",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "رئيس مجلس الادارة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.Filterer,
                        LitigationRequestOwnerType = "مصفي",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Filterer",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "مصفي",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.JudicialGuard,
                        LitigationRequestOwnerType = "حارس قضائي",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Judicial Guard",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "حارس قضائي",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.Custodian,
                        LitigationRequestOwnerType = "ولي",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Custodian",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "ولي",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.ManagingDirector,
                        LitigationRequestOwnerType = "العضو المنتدب",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Managing Director",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "العضو المنتدب",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.BankruptcyTrustee,
                        LitigationRequestOwnerType = "أمين تفليسة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Bankruptcy Trustee",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "أمين تفليسة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
            });
        context.SaveChanges();
    }

}
