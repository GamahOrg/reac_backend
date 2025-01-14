using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class ArbitratorCountSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysArbitratorCounts.AnyAsync()) return;
            await context.SysArbitratorCounts.AddRangeAsync(
                new List<SysArbitratorCount>
                {
                    new SysArbitratorCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 1,
                    },
                   new SysArbitratorCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 3,
                    },
                   new SysArbitratorCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 5,
                    },
                   new SysArbitratorCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 7,

                    },
                   new SysArbitratorCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 9,
                    },
                });
            context.SaveChanges();
        }
    }
}
