using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class ArbitratorAssigningRequestSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysArbitratorAssigningRequests.AnyAsync()) return;
            await context.SysArbitratorAssigningRequests.AddRangeAsync(
                new List<SysArbitratorAssigningRequestType>
                {
                    new SysArbitratorAssigningRequestType
                    {
                        Id = REAC.Domain.Shared.Enum.ArbitratorAssigningRequestType.Arbitrator,
                        ArbitratorAssigningRequestName = "محكم",
                    },
                    new SysArbitratorAssigningRequestType
                    {
                        Id =  REAC.Domain.Shared.Enum.ArbitratorAssigningRequestType.ArbitratorsChairman,
                        ArbitratorAssigningRequestName = "رئيس هئية تحكيم",
                    }
                });
            context.SaveChanges();
        }
    }
}
