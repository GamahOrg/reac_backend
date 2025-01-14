using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class LitigationRequestExtendPeriodRequestStatusSeedDataCreator
{
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysLitigationRequestExtendPeriodRequestStatuses.AnyAsync()) return;
        await context.SysLitigationRequestExtendPeriodRequestStatuses.AddRangeAsync(
            new List<SysLitigationRequestExtendPeriodRequestStatus>
            {
                    new SysLitigationRequestExtendPeriodRequestStatus
                    {
                        Id = Domain.Shared.Enum.LitigationRequestExtendPeriodRequestStatus.Approve,
                        StatusName = "موافقة",
                        Translations=new List<SysLitigationRequestExtendPeriodRequestStatusTranslation>
                        {
                            new SysLitigationRequestExtendPeriodRequestStatusTranslation
                            {
                                StatusName = "Approve",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestExtendPeriodRequestStatusTranslation
                            {
                                StatusName = "موافقة",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                   new SysLitigationRequestExtendPeriodRequestStatus
                    {
                        Id = Domain.Shared.Enum.LitigationRequestExtendPeriodRequestStatus.Reject,
                        StatusName = "رفض",
                        Translations=new List<SysLitigationRequestExtendPeriodRequestStatusTranslation>
                        {
                            new SysLitigationRequestExtendPeriodRequestStatusTranslation
                            {
                                StatusName = "Reject",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestExtendPeriodRequestStatusTranslation
                            {
                                StatusName = "رفض",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    }
            });
        context.SaveChanges();
    }
}
