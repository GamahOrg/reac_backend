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
                    Id = Guid.NewGuid(),
                    LitigationAmount = "0-10,000 \\ رسوم التسجيل 300",
                    Fees = 300
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "10,001-20,000 \\ رسوم التسجيل 400",
                    Fees = 400
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "20,001-30,000 \\ رسوم التسجيل 500",
                    Fees = 500
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "30,001-50,000 \\ رسوم التسجيل 600",
                    Fees = 600
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "50,001-100,000 \\ رسوم التسجيل 2,000",
                    Fees = 2000
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "100,001-200,000 \\ رسوم التسجيل 2,500",
                    Fees = 2500
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "200,001-300,000 \\ رسوم التسجيل 10,730",
                    Fees = 10730m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "300,001-500,000 \\ رسوم التسجيل 12,365",
                    Fees = 12365m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "500,001-1,000,000 \\ رسوم التسجيل 16,097",
                    Fees = 16097m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "1,000,001-2,000,000 \\ رسوم التسجيل 23,024",
                    Fees = 23024m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "2,000,001-5,000,000 \\ رسوم التسجيل 38,249",
                    Fees = 38249m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "5,000,001-10,000,000 \\ رسوم التسجيل 60,974",
                    Fees = 60974m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "10,000,000-25,000,000 \\ رسوم التسجيل 92,508",
                    Fees = 92508m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "25,000,001-50,000,000 \\ رسوم التسجيل 135,974",
                    Fees = 135974m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "50,000,001-100,000,000 \\ رسوم التسجيل 167,157",
                    Fees = 167157m
                 },new SysLitigationRequestAmount
                {
                    Id = Guid.NewGuid(),
                    LitigationAmount = "100,000,001-فأعلي \\ رسوم التسجيل 181,714",
                    Fees = 181714m
                 },

            }
            );
            context.SaveChanges();

        }
    }
}