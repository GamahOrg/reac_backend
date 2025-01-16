using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Invoice;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public class InvoiceTypeCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.InvoiceTypes.AnyAsync()) return;
            await context.InvoiceTypes.AddRangeAsync(
                new List<SysInvoiceType>
                {
                    new SysInvoiceType
                    {
                        Id =  InvoiceType.AdministrativeForLawsuitFees,
                        Name = "الرسوم الإدارية للدعوى",
                        Translations=new List<SysInvoiceTypeTranslation>
                        {
                            new SysInvoiceTypeTranslation
                            {
                                Name = "Administrative For Lawsuit Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceTypeTranslation
                            {
                                Name = "الرسوم الإدارية للدعوى",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceType
                    {
                        Id =  InvoiceType.ReconciliationFees,
                        Name = "رسوم المصالحة",
                        Translations=new List<SysInvoiceTypeTranslation>
                        {
                            new SysInvoiceTypeTranslation
                            {
                                Name = "Reconciliation Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceTypeTranslation
                            {
                                Name = "رسوم المصالحة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceType
                    {
                        Id =  InvoiceType.ArbitrationFees,
                        Name = "رسوم التحكيم",
                        Translations=new List<SysInvoiceTypeTranslation>
                        {
                            new SysInvoiceTypeTranslation
                            {
                                Name = "Reconciliation Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceTypeTranslation
                            {
                                Name = "رسوم التحكيم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceType
                    {
                        Id =  InvoiceType.ArbitratorRegistrationFees,
                        Name = "رسوم تسجيل محكم",
                        Translations=new List<SysInvoiceTypeTranslation>
                        {
                            new SysInvoiceTypeTranslation
                            {
                                Name = "Arbitrator Registration Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceTypeTranslation
                            {
                                Name = "رسوم تسجيل محكم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceType
                    {
                        Id =  InvoiceType.ReconciliatorRegistrationFees,
                        Name = "رسوم تسجيل مصلح",
                        Translations=new List<SysInvoiceTypeTranslation>
                        {
                            new SysInvoiceTypeTranslation
                            {
                                Name = "Reconciliator Registration Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceTypeTranslation
                            {
                                Name = "رسوم تسجيل مصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceType
                    {
                        Id =  InvoiceType.ReconciliatorMembershipFees,
                        Name = "رسوم عضوية مصلح",
                        Translations=new List<SysInvoiceTypeTranslation>
                        {
                            new SysInvoiceTypeTranslation
                            {
                                Name = "Reconciliator Membership Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceTypeTranslation
                            {
                                Name = "رسوم عضوية مصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceType
                    {
                        Id =  InvoiceType.ArbitratorMembershipFees,
                        Name = "رسوم عضوية محكم",
                        Translations=new List<SysInvoiceTypeTranslation>
                        {
                            new SysInvoiceTypeTranslation
                            {
                                Name = "Arbitrator Membership Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceTypeTranslation
                            {
                                Name = "رسوم عضوية محكم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    }
                });
            context.SaveChanges();
        }
    }
}
