using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class CitizenRequestStatusSeedDataCreator
{
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysCitizenRequestStatuses.AnyAsync()) return;
        await context.SysCitizenRequestStatuses.AddRangeAsync(
            new List<SysCitizenRequestStatus>
            {
                    new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.Draft,
                        RequestStatus = "مسودة",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Draft",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "مسودة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.WaitingForCertify,
                        RequestStatus = " بإنتظار الإعتماد",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Waiting for approval",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = " بإنتظار الإعتماد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.Rejected,
                        RequestStatus = "تم الرفض",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Rejected",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "تم الرفض",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.WaitingForMembershipFeesToBePaid,
                        RequestStatus = "بانتظار دفع رسوم العضوية",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Waiting for membership fees to be paid",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "بانتظار دفع رسوم العضوية",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.Certified,
                        RequestStatus = "معتمد",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Certified",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "معتمد",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.SentForEdit,
                        RequestStatus = "تم الإرسال للتعديل",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Sent For Edit",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "تم الإرسال للتعديل",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.SentFromEdit,
                        RequestStatus = "تم التعديل والإرسال",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Sent For Edit",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "تم التعديل والإرسال",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                     new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.SendToPayFees,
                        RequestStatus = "تم الارسال لدفع الرسوم",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Sent To Pay Fees",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus ="تم الارسال لدفع الرسوم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                      new SysCitizenRequestStatus
                    {
                        Id =  CitizenRequestStatus.FeesPaid,
                        RequestStatus = "تم دفع الرسوم",
                        Translations=new List<SysCitizenRequestStatusTranslation>
                        {
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "Fees Paid",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysCitizenRequestStatusTranslation
                            {
                                RequestStatus = "تم دفع الرسوم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    }
            });
        context.SaveChanges();
    }
}
