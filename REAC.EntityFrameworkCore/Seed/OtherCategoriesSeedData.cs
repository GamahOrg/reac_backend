using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Seed
{
    public class OtherCategoriesSeedData
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.OtherCategories.AnyAsync()) return;
            await context.OtherCategories.AddRangeAsync(new List<OtherCategory>
            {
                new OtherCategory
                {
                    Id = new Guid("22000000-0000-0000-0000-000000000000"),


                    CategoryName = "العقارات",

                },
                new OtherCategory
                {
                    Id = new Guid("11000330-0000-0000-0000-000000000000"),


                    CategoryName = "الأراضي",

                },
            

            }
            );
            context.SaveChanges();

        }
    }
}
