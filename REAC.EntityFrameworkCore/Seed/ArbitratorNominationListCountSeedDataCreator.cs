using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class ArbitratorNominationListCountSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysArbitratorNominationListCounts.AnyAsync()) return;
            await context.SysArbitratorNominationListCounts.AddRangeAsync(
                new List<SysArbitratorNominationListCount>
                {
                    new SysArbitratorNominationListCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 1,
                        ArbitratorNominationListCount = 5,
                        ArbitratorChairmanNominationListCount = 3,

                    },
                   new SysArbitratorNominationListCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 3,
                        ArbitratorNominationListCount = 7,
                        ArbitratorChairmanNominationListCount = 3,

                    },
                   new SysArbitratorNominationListCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 5,
                        ArbitratorNominationListCount = 7,
                        ArbitratorChairmanNominationListCount = 3,

                    },
                   new SysArbitratorNominationListCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 7,
                        ArbitratorNominationListCount = 9,
                        ArbitratorChairmanNominationListCount = 3,

                    },
                   new SysArbitratorNominationListCount
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorCount = 9,
                        ArbitratorNominationListCount = 11,
                        ArbitratorChairmanNominationListCount = 3,

                    },
                });
            context.SaveChanges();
        }
    }
}
