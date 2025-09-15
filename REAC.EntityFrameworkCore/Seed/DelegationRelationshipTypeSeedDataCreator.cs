using Core.Shared.Constants;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class DelegationRelationshipTypeSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysDelegationRelationshipTypes.AnyAsync()) return;
            await context.SysDelegationRelationshipTypes.AddRangeAsync(
                new List<SysDelegationRelationshipType>
                {
                    new SysDelegationRelationshipType
                    {
                        Id = DelegationRelationshipType.Individual,
                        DelegationRelationshipTypeName="فرد",
                        Translations=new List<SysDelegationRelationshipTypeTranslation>
                        {
                            new SysDelegationRelationshipTypeTranslation
                            {
                                DelegationRelationshipTypeName="فرد",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysDelegationRelationshipTypeTranslation
                            {
                                DelegationRelationshipTypeName="Individual",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }
                    },
                    new SysDelegationRelationshipType
                    {
                        Id = DelegationRelationshipType.Facility,
                        DelegationRelationshipTypeName="منشأة",
                        Translations=new List<SysDelegationRelationshipTypeTranslation>
                        {
                            new SysDelegationRelationshipTypeTranslation
                            {
                                DelegationRelationshipTypeName="منشأة",
                                LanguageCode=Constants.EnglishLanguage
                            },
                             new SysDelegationRelationshipTypeTranslation
                            {
                                DelegationRelationshipTypeName="Facility",
                                LanguageCode=Constants.ArabicLanguage
                            }
                        }

                    }
                    //,
                    //new SysDelegationRelationshipType
                    //{
                    //    Id = DelegationRelationshipType.CompanyRepresentative,
                    //    DelegationRelationshipTypeName="ممثل عن شركة",
                    //    Translations=new List<SysDelegationRelationshipTypeTranslation>
                    //    {
                    //        new SysDelegationRelationshipTypeTranslation
                    //        {
                    //            DelegationRelationshipTypeName="ممثل عن شركة",
                    //            LanguageCode=Constants.EnglishLanguage
                    //        },
                    //         new SysDelegationRelationshipTypeTranslation
                    //        {
                    //            DelegationRelationshipTypeName="Company Representative",
                    //            LanguageCode=Constants.ArabicLanguage
                    //        }
                    //    }

                    //}
                });
            context.SaveChanges();
        }
    }
}
