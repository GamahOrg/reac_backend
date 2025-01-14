using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class BankSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysBanks.AnyAsync()) return;
            await context.SysBanks.AddRangeAsync(
                new List<SysBank>
                {
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "البنك الأهلي السعودي",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "بنك الراجحي",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "بنك الرياض",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "البنك السعودى الأول",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "البنك العربى الوطنى ",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "مصرف الإنماء",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "البنك السعودي الفرنسي",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "البنك السعودي للإستثمار",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "بنك الجزيرة",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "بنك البلاد",
                    },
                    new SysBank
                    {
                        Id = Guid.NewGuid(),
                        BankName = "بنك ميم",
                    }
                });
            context.SaveChanges();
        }
    }
}
