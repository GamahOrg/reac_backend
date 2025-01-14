using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class LitigationRequestOwnerTypeSeedDataCreator
{
    //public enum LitigationRequestOwnerType
    //{
    //    ///<summary>مقدم عن نفسه</summary>
    //    Personal = 1,
    //    ///<summary>ممثل شركة</summary>
    //    CompanyRepresentative = 2,
    //    ///<summary>محامي عن شركة</summary>
    //    LawyerForCompany = 3,
    //    ///<summary> وكيل عن شركة</summary>
    //    AgentForCompany = 4,
    //    ///<summary>محامي عن فرد</summary>
    //    LawyerForIndividual = 3,
    //    ///<summary> وكيل عن فرد</summary>
    //    AgentForIndividual = 4
    //}
    /// <summary>
    /// مقدم عن نفسه – مفوض
    /// </summary>
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
                        LitigationRequestOwnerType = "وكيل عن فرد",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Agent For Individual",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "وكيل عن فرد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.LawyerForIndividual,
                        LitigationRequestOwnerType = "محامي عن فرد",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Lawyer For Individual",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "محامي عن فرد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.AgentForCompany,
                        LitigationRequestOwnerType = "وكيل عن شركة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Agent For Company",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "وكيل عن شركة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.LawyerForCompany,
                        LitigationRequestOwnerType = "محامي عن شركة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "Lawyer For Company",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "محامي عن شركة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                       new SysLitigationRequestOwnerType
                    {
                        Id = LitigationRequestOwnerType.CompanyRepresentative,
                        LitigationRequestOwnerType = "ممثل شركة",
                        Translations=new List<SysLitigationRequestOwnerTypeTranslation>
                        {
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "CompanyRepresentative",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestOwnerTypeTranslation
                            {
                                LitigationRequestOwnerType = "ممثل شركة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    }
            });
        context.SaveChanges();
    }

}
