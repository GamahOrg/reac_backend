using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public partial class LitigationRequestAmountSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysLitigationRequestAmounts.AnyAsync()) return;
            await context.SysLitigationRequestAmounts.AddRangeAsync(new List<SysLitigationRequestAmount>
            {
                new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("049A31BE-0A9D-49F1-B0D6-96112EFF872E"),
                    LitigationAmount = "0-10,000 \\ رسوم التسجيل 300",
                    Fees = 300,
                    FromAmount=1,
                    ToAmount=10000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("2C30F52E-EA14-4C5B-AA31-C34FDE466A35"),
                    LitigationAmount = "10,001-20,000 \\ رسوم التسجيل 400",
                    Fees = 400,
                    FromAmount=10001,
                    ToAmount=20000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("188A3533-F6D7-4D8D-815E-2EED483366E5"),
                    LitigationAmount = "20,001-30,000 \\ رسوم التسجيل 500",
                    Fees = 500,
                    FromAmount=20001,
                    ToAmount=30000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("30B8CB40-3FAF-4BDC-A932-32265CF6F0F7"),
                    LitigationAmount = "30,001-50,000 \\ رسوم التسجيل 600",
                    Fees = 600,
                    FromAmount=30001,
                    ToAmount=50000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("EB675BE6-7E2B-479D-B82A-BCD1C2D9CA55"),
                    LitigationAmount = "50,001-100,000 \\ رسوم التسجيل 2,000",
                    Fees = 2000,
                    FromAmount=50001,
                    ToAmount=100000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("C4E1F0D2-E9E4-40E4-81CB-0F4056630810"),
                    LitigationAmount = "100,001-200,000 \\ رسوم التسجيل 2,500",
                    Fees = 2500,
                    FromAmount=100001,
                    ToAmount=200000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("C443125E-3BAB-4DBD-8059-5224EB5B33C9"),
                    LitigationAmount = "200,001-300,000 \\ رسوم التسجيل 10,730",
                    Fees = 10730m,
                    FromAmount=200001,
                    ToAmount=300000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("D400C677-B62F-487A-ABC6-502CD95B306B"),
                    LitigationAmount = "300,001-500,000 \\ رسوم التسجيل 12,365",
                    Fees = 12365m,
                    FromAmount=300001,
                    ToAmount=500000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("5FAD6041-95A2-46B3-8B40-7A5CF9673D89"),
                    LitigationAmount = "500,001-1,000,000 \\ رسوم التسجيل 16,097",
                    Fees = 16097m,
                    FromAmount=500001,
                    ToAmount=1000000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("91B8B9C2-385C-4EC9-8AE8-90F15E82A823"),
                    LitigationAmount = "1,000,001-2,000,000 \\ رسوم التسجيل 23,024",
                    Fees = 23024m,
                    FromAmount=1000001,
                    ToAmount=2000000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("16915895-323A-4999-B711-B0ED5E4B4AA7"),
                    LitigationAmount = "2,000,001-5,000,000 \\ رسوم التسجيل 38,249",
                    Fees = 38249m,
                     FromAmount=2000001,
                    ToAmount=5000000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("AF38248D-0466-4983-BF16-C1EA3BBF3B2C"),
                    LitigationAmount = "5,000,001-10,000,000 \\ رسوم التسجيل 60,974",
                    Fees = 60974m,
                     FromAmount=5000001,
                    ToAmount=10000000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("35613DF2-2AA9-4EC2-9FBC-D45F297EB64A"),
                    LitigationAmount = "10,000,001-25,000,000 \\ رسوم التسجيل 92,508",
                    Fees = 92508m,
                    FromAmount=10000001,
                    ToAmount=25000000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("77962EEC-DC9C-4114-BCC0-E9981FDE0D42"),
                    LitigationAmount = "25,000,001-50,000,000 \\ رسوم التسجيل 135,974",
                    Fees = 135974m,
                    FromAmount=25000001,
                    ToAmount=50000000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("FBC1ADCE-4F2A-41FC-91A8-F236675E53E8"),
                    LitigationAmount = "50,000,001-100,000,000 \\ رسوم التسجيل 167,157",
                    Fees = 167157m,
                    FromAmount=50000001,
                    ToAmount=100000000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.Parse("1FC9EFC9-E6F0-46D9-B327-F60114937DC6"),
                    LitigationAmount = "100,000,001-فأعلي \\ رسوم التسجيل 181,714",
                    Fees = 181714m,
                    FromAmount=100000001,
                 },

            }
            );
            context.SaveChanges();

        }
    }
}