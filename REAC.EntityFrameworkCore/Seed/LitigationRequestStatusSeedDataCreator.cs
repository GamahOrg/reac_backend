using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class LitigationRequestStatusSeedDataCreator
{
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysLitigationRequestStatuses.AnyAsync()) return;
        await context.SysLitigationRequestStatuses.AddRangeAsync(
            new List<SysLitigationRequestStatus>
            {
                    new SysLitigationRequestStatus
                    {
                        Id =  Domain.Shared.Enum.RequestStatus.New,
                        RequestStatus = "جديدة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "New",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "جديدة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id =  Domain.Shared.Enum.RequestStatus.Received,
                        RequestStatus = "مُستلمة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Received",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "مُستلمة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.AsessionTimeHasBeenSet,
                        RequestStatus = "تم تحديد ميعاد جلسة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "A session time has been set",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم تحديد ميعاد جلسة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.AsessionTimeHasBeenReset,
                        RequestStatus = "تم إعادة تحديد ميعاد جلسة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Asession Time Has Been Reset ",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم إعادة تحديد ميعاد جلسة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.TheSessionWasCompleted,
                        RequestStatus = "تمت الجلسة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "The session was completed",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تمت الجلسة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id =  Domain.Shared.Enum.RequestStatus.TheMinutesOfTheMeetingWereWritten,
                        RequestStatus = "تم كتابة محضر الجلسة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "The minutes of the meeting were written",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم كتابة محضر الجلسة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.WaiverOfRight,
                        RequestStatus = "تنازل عن الحق",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Waiver of right",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تنازل عن الحق",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id =  Domain.Shared.Enum.RequestStatus.WaiverOfTheLitigation,
                        RequestStatus = "تنازل عن الدعوى",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Waiver of the lawsuit",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تنازل عن الدعوى",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.ReconciliationWasNotMade,
                        RequestStatus = "عدم الصُلح",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Lack of reconciliation",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "عدم الصُلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.SaveAndClose,
                        RequestStatus = "حفظ وإغلاق",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Save and close",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "حفظ وإغلاق",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.SaveAsDraft,
                        RequestStatus = "حفظ كمسودة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Save as Draft",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "حفظ كمسودة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.Rejected,
                        RequestStatus = "مرفوضة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Rejected",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "مرفوضة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.SentBackForEdit,
                        RequestStatus = "معادة للتعديل",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Send Back For Edit",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "معادة للتعديل",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.Edited,
                        RequestStatus = "تم التعديل",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Edited",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "تم التعديل",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.AgreementRequestHasBeenSent,
                        RequestStatus = "قيد المشارطة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Charter RequestSent ",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "قيد المشارطة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.AcceptanceOfTheReconciliationReport,
                        RequestStatus = "تم قبول محضر الصلح",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Acceptance Of The Reconciliation Report ",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "تم قبول محضر الصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.RejectionOfTheReconciliationReport,
                        RequestStatus = "تم رفض محضر الصلح",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Rejection Of The Reconciliation Report ",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "تم رفض محضر الصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.ReconciliationWasMade,
                        RequestStatus = "تم الصلح",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Reconciliation Was Made ",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "تم الصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.ReconciliationWasNotMadeAndMoveToArbitration,
                        RequestStatus = "عدم الصُلح والانتقال للتحكيم",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Reconciliation Was Not Made And Move To Arbitration ",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "عدم الصُلح والانتقال للتحكيم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.JudgmentDecisionIssued,
                        RequestStatus = "تم إصدار حكم التحكيم",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Judgment Decision Issued",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "تم إصدار حكم التحكيم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.WaitingForExplanation,
                        RequestStatus = "بإنتظار التفسير",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Wait For Ex",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus =  "بإنتظار التفسير",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.ExplanationResponsed,
                        RequestStatus = "تم الرد على طلب التفسير",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Explanation Responsed",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم الرد على طلب التفسير",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.AgreementRequestHasBeenRejected,
                        RequestStatus = "تم رفض طلب المشارطة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Agreement Request Has Been Rejected",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم رفض طلب المشارطة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.ReconciliationOnTradyPlatform,
                        RequestStatus = "تم الصلح بواسظة منصة تراضى ",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "The Reconciliation on Trady platform",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم الصلح بواسطة منصة تراضى ",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent,
                        RequestStatus = "تم إرسال طلب تحصيل رسوم الدعوة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Collection Of Litigation Fees Request Is Being Sent",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم إرسال طلب تحصيل رسوم الدعوة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.LitigationFeesHaveBeenCollected,
                        RequestStatus = "تم تحصيل رسوم الدعوة",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Litigation Fees Have Been Collected",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم تحصيل رسوم الدعوة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent,
                        RequestStatus = "تم إرسال طلب تحصيل رسوم الصلح",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Collection Of Reconciliation Fees Request Is Being Sent",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم إرسال طلب تحصيل رسوم الصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.ReconciliationFeesHaveBeenCollected,
                        RequestStatus = "تم تحصيل رسوم الصلح",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Reconciliation Fees Have Been Collected",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم تحصيل رسوم الصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.CollectionOfArbitrationFeesRequestIsBeingSent,
                        RequestStatus = "تم إرسال طلب تحصيل رسوم التحكيم",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Collection Of Arbitration Fees Request Is Being Sent",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم إرسال طلب تحصيل رسوم التحكيم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.ArbitrationFeesHaveBeenCollected,
                        RequestStatus = "تم تحصيل رسوم التحكيم",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Arbitration Fees Have Been Collected",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم تحصيل رسوم التحكيم",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestStatus
                    {
                        Id = Domain.Shared.Enum.RequestStatus.LitigationApprovedCollectionOfReconciliationFeesRequestIsBeingSend,
                        RequestStatus = "تم قبول الدعوى وجارى إرسال طلب تحصيل رسوم الصلح",
                        CreatedOn = DateTime.Now,
                        Translations=new List<SysLitigationRequestStatusTranslation>
                        {
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "Litigation Approved Collection Of Reconciliation Fees Request Is Being Send",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestStatusTranslation
                            {
                                RequestStatus = "تم قبول الدعوى وجارى إرسال طلب تحصيل رسوم الصلح",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    }
            });
        context.SaveChanges();
    }
}
