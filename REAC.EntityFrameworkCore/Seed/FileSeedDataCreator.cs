using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;

namespace Ticketing.EntityFrameworkCore.SeedData
{
    public static class FileSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysFileManagers.AnyAsync()) return;
            await context.SysFileManagers.AddRangeAsync(
                new List<SysFileManager>
                {
                    new SysFileManager
                    {
                        Id = Guid.Parse("e5d55dcb-aa16-4719-8cdb-0ac8783269c6"),
                        CreatedOn = DateTime.Now,
                        CreatedBy = Guid.Parse("6b4dfc44-508e-45e1-9f12-ab96b5aec1c4"),
                        FileType = "jpg",
                        FilePath="File.jpg",
                        FileDescription = "File"
                    },
                    new SysFileManager
                    {
                        Id = Guid.Parse("06f63ff3-28d8-41e2-8769-658406db0788"),
                        CreatedOn = DateTime.Now,
                        CreatedBy = Guid.Parse("6b4dfc44-508e-45e1-9f12-ab96b5aec1c4"),
                        FileType = "jpg",
                        FilePath="File.jpg",
                        FileDescription = "File"

                    }
                });
            context.SaveChanges();
        }
    }
}
