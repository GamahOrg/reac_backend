using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Seed
{
    public class CorrespondenceStatusSeedDataCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.SysLitigationRequestCorrespondenceStatus.AnyAsync()) return;
            await context.SysLitigationRequestCorrespondenceStatus.AddRangeAsync(
                new List<SysLitigationRequestCorrespondenceStatus>
                {
                    new SysLitigationRequestCorrespondenceStatus
                    {
                        Id = (Domain.Shared.Enum.CorrespondenceStatus) 1 ,
                        EnName = "RepliedOn",
                        ArName = "تم الرد",
                    },
                    new SysLitigationRequestCorrespondenceStatus
                    {
                        Id =  (Domain.Shared.Enum.CorrespondenceStatus) 2,
                        EnName = "NotRepliedOn",
                        ArName = "لم يتم الرد",
                    }
                });
            context.SaveChanges();
        }
    }
}

