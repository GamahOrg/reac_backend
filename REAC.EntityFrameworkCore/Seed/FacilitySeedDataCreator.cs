using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class FacilitySeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.Facilities.AnyAsync()) return;
            await context.Facilities.AddRangeAsync(
                new List<Facility>
                {
                    new Facility
                    {
                        Id  = Guid.Parse("25d5b134-3f63-4daf-a92d-0ce224ee024c"),
                        Code = "FAC-0001",
                        CitizenId = Guid.Parse("e5d55dcb-aa16-4719-8cdb-0ac8783269c6"),
                        Facility700Number = "7004650269839",
                        CommercialRegisterNumber = "4650269839",
                        ArabicFacilityName = "مؤسسة مراس العربية للتطوير العقاري",
                        EnglishFacilityName = "Meraas Arabia Real Estate Development Corporation",
                        Fax = null,
                        Email = "facility1@domain.com",
                        City = "ألرياض",
                        District ="العليا",
                        Street = "الملك فهد",
                        PostalCode =null,
                        SubNumber= "856899" ,
                        BuildingNumber = "B1254",
                        UnitNumber = "U-5852"
                    },
                    new Facility
                    {
                        Id  = Guid.Parse("deef9a46-e906-4f89-950b-0d0e8cb7e5a1"),
                        Code = "FAC-0002",
                        CitizenId = Guid.Parse("04540979-8ae2-489f-bf90-d59790d7204e"),
                        Facility700Number = "7004030307593",
                        CommercialRegisterNumber = "4030307593",
                        ArabicFacilityName = "مؤسسة هدير للتطوير العقاري",
                        EnglishFacilityName = "Hadeer Real Estate Development Corporation",
                        Fax = null,
                        Email = "facility2@domain.com",
                        City = "ألرياض",
                        District ="المنسك",
                        Street = "شارع الاربعين",
                        PostalCode =null,
                        SubNumber= "1255" ,
                        BuildingNumber = "B1254",
                        UnitNumber = "U-15223"
                    },
                    new Facility
                    {
                        Id  = Guid.Parse("aa4ba07a-d2f4-4b50-b84e-0d6d1914355f"),
                        Code = "FAC-0003",
                        CitizenId = Guid.Parse("89daceef-7cda-47a0-836c-eccb57cfe933"),
                        Facility700Number = "7004030489068",
                        CommercialRegisterNumber = "4030489068",
                        ArabicFacilityName = "مجموعة السلطان للعقارات",
                        EnglishFacilityName = "Sultan Real Estate Group",
                        Fax = null,
                        Email = "facility1@domain.com",
                        City = "ألرياض",
                        District ="الكحلة",
                        Street = "شارع الملك فهد",
                        PostalCode =null,
                        SubNumber= "85555" ,
                        BuildingNumber = "B1254",
                        UnitNumber = "U-18823"
                    },
                    new Facility
                    {
                        Id  = Guid.Parse("9b09716f-1122-4d61-a18c-11b0f62587e0"),
                        Code = "FAC-0004",
                        CitizenId = Guid.Parse("57008f08-3002-487d-bc6b-83ca78b56f4b"),
                        Facility700Number = "7003350161779",
                        CommercialRegisterNumber = "3350161779",
                        ArabicFacilityName = "شركة محمد عمر العييدي محاسبون ومراجعون قانونيون",
                        EnglishFacilityName = "Mohammed Omar Al-Ayedi Company, Chartered Accountants and Auditors",
                        Fax = null,
                        Email = "facility1@domain.com",
                        City = "ألرياض",
                        District ="الكحلة",
                        Street = "شارع الملك فهد",
                        PostalCode =null,
                        SubNumber= "85555" ,
                        BuildingNumber = "B1254",
                        UnitNumber = "U-18823"
                    },
                    new Facility
                    {
                        Id  = Guid.Parse("3cddd129-2597-4258-b5fb-12987e1f1b94"),
                        Code = "FAC-0005",
                        CitizenId = Guid.Parse("57008f08-3002-487d-bc6b-83ca78b56f4b"),
                        Facility700Number = "7001010322177",
                        CommercialRegisterNumber = "1010322177",
                        ArabicFacilityName = "مكتب ابراهيم محمد حسين الكاف للاستشارات الهندسية",
                        EnglishFacilityName = "Ibrahim Muhammad Hussein Al-Kaf Engineering Consulting Office",
                        Fax = null,
                        Email = "facility1@domain.com",
                        City = "ألرياض",
                        District ="الكحلة",
                        Street = "شارع الملك فهد",
                        PostalCode =null,
                        SubNumber= "85555" ,
                        BuildingNumber = "B1254",
                        UnitNumber = "U-18823"
                    }
                });
            context.SaveChanges();
        }
    }
}

