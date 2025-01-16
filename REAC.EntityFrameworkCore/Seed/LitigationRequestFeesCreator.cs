using Microsoft.EntityFrameworkCore;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Seed
{
    public class LitigationRequestFeesCreator
    {
        public static async Task SeedAsync(REACDBContext context)
        {
            if (await context.LitigationRequestFees.AnyAsync()) return;


            await context.LitigationRequestFees.AddRangeAsync(AddFees(1));
            await context.LitigationRequestFees.AddRangeAsync(AddFees(3));
            await context.LitigationRequestFees.AddRangeAsync(AddFees(5));
            await context.LitigationRequestFees.AddRangeAsync(AddFees(7));
            await context.LitigationRequestFees.AddRangeAsync(AddFees(9));
        }

        static List<LitigationRequestFees> AddFees(int arbitrationCount)
        {
            return new List<LitigationRequestFees>
            {
                GenerateLitigationRequestFees(arbitrationCount,1000001.00m,2000000.00m,23024.00m),
                GenerateLitigationRequestFees(arbitrationCount,25000001.00m,50000000.00m,135974.00m),
                GenerateLitigationRequestFees(arbitrationCount,50000001.00m,100000000.00m,167157.00m),
                GenerateLitigationRequestFees(arbitrationCount,10001.00m,20000.00m,400.00m),
                GenerateLitigationRequestFees(arbitrationCount,500001.00m,1000000.00m,16097.00m),
                GenerateLitigationRequestFees(arbitrationCount,10000000.00m,25000000.00m,92508.00m),
                GenerateLitigationRequestFees(arbitrationCount,20001.00m,30000.00m,500.00m),
                GenerateLitigationRequestFees(arbitrationCount,200001.00m,300000.00m,10730.00m),
                GenerateLitigationRequestFees(arbitrationCount,30001.00m,50000.00m,600.00m),
                GenerateLitigationRequestFees(arbitrationCount,100001.00m,200000.00m,2500.00m),
                GenerateLitigationRequestFees(arbitrationCount,1.00m,10000.00m,300.00m),
                GenerateLitigationRequestFees(arbitrationCount,5000001.00m,10000000.00m,60974.00m),
                GenerateLitigationRequestFees(arbitrationCount,2000001.00m,5000000.00m,38249.00m),
                GenerateLitigationRequestFees(arbitrationCount,300001.00m,500000.00m,12365.00m),
                GenerateLitigationRequestFees(arbitrationCount,50001.00m,100000.00m,2000.00m),
            };
        }

        static LitigationRequestFees GenerateLitigationRequestFees(int arbitrationCount, decimal fromAmount, decimal toAmount, decimal fees)
        {
            return new LitigationRequestFees
            {
                Id = Guid.NewGuid(),
                ArbitratorsCount = arbitrationCount,
                FromAmount = fromAmount,
                ToAmount = toAmount,
                Fees = fees * arbitrationCount
            };
        }
    }
}
