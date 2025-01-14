using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class MeetingLocationSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysMeetingLocations.AnyAsync()) return;
            await context.SysMeetingLocations.AddRangeAsync(
                new List<SysMeetingLocation>
                {
                    new SysMeetingLocation
                    {
                        Id = Guid.NewGuid(),
                        MeetingLocationName = "اونلاين",
                    },
                    new SysMeetingLocation
                    {
                        Id = Guid.NewGuid(),
                        MeetingLocationName = "مقر المركز",
                    }
                });
            context.SaveChanges();
        }
    }
}
