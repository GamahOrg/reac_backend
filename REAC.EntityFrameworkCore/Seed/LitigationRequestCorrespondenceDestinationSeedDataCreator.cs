using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed;

public partial class LitigationRequestCorrespondenceDestinationSeedDataCreator
{
    public static async Task SeedAsync(REACDBContext context)
    {
        if (await context.SysLitigationRequestCorrespondenceDestinations.AnyAsync()) return;
        await context.SysLitigationRequestCorrespondenceDestinations.AddRangeAsync(
            new List<SysLitigationRequestCorrespondenceDestination>
            {
                    new SysLitigationRequestCorrespondenceDestination
                    {
                        Id = Domain.Shared.Enum.LitigationRequestCorrespondenceDestination.Plaintiff,
                        Destination = "المدعى",
                        Translations=new List<SysLitigationRequestCorrespondenceDestinationTranslation>
                        {
                            new SysLitigationRequestCorrespondenceDestinationTranslation
                            {
                                Destination = "Plaintiff",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestCorrespondenceDestinationTranslation
                            {
                                Destination = "المدعى",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                   new SysLitigationRequestCorrespondenceDestination
                    {
                        Id = Domain.Shared.Enum.LitigationRequestCorrespondenceDestination.Defendant,
                        Destination = "المدعى عليه",
                        Translations=new List<SysLitigationRequestCorrespondenceDestinationTranslation>
                        {
                            new SysLitigationRequestCorrespondenceDestinationTranslation
                            {
                                Destination = "Defendant",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestCorrespondenceDestinationTranslation
                            {
                                Destination = "المدعى عليه",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysLitigationRequestCorrespondenceDestination
                    {
                        Id =  Domain.Shared.Enum.LitigationRequestCorrespondenceDestination.BothOfThem,
                        Destination = "كلاهما",
                        Translations=new List<SysLitigationRequestCorrespondenceDestinationTranslation>
                        {
                            new SysLitigationRequestCorrespondenceDestinationTranslation
                            {
                                Destination = "Both of them",
                                LanguageCode = Constants.EnglishLanguage
                            },
                            new SysLitigationRequestCorrespondenceDestinationTranslation
                            {
                                Destination = "كلاهما",
                                LanguageCode = Constants.ArabicLanguage
                            }
                        }
                    }
            });
        context.SaveChanges();
    }
}