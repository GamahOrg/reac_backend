using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Invoice;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public class InvoiceStatusCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.InvoiceStatuses.AnyAsync()) return;
            await context.InvoiceStatuses.AddRangeAsync(
                new List<SysInvoiceStatus>
                {
                    new SysInvoiceStatus
                    {
                        Id =  InvoiceStatus.Payable,
                        Name = "مستحقة السداد",
                        Translations=new List<SysInvoiceStatusTranslation>
                        {
                            new SysInvoiceStatusTranslation
                            {
                                Name = "Payable",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceStatusTranslation
                            {
                                Name = "مستحقة السداد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceStatus
                    {
                        Id =  InvoiceStatus.Paid,
                        Name = "مسددة",
                        Translations=new List<SysInvoiceStatusTranslation>
                        {
                            new SysInvoiceStatusTranslation
                            {
                                Name = "Paid",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceStatusTranslation
                            {
                                Name = "مسددة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysInvoiceStatus
                    {
                        Id =  InvoiceStatus.CanceledForNonPayment,
                        Name = "ملغاة لعدم السداد",
                        Translations=new List<SysInvoiceStatusTranslation>
                        {
                            new SysInvoiceStatusTranslation
                            {
                                Name = "Canceled For Non Payment",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysInvoiceStatusTranslation
                            {
                                Name = "ملغاة لعدم السداد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                });
            context.SaveChanges();
        }
    }
}
