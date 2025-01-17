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


            await context.LitigationRequestFees.AddRangeAsync(AddFees());
        }

        static List<LitigationRequestFees> AddFees()
        {
            return new List<LitigationRequestFees>
            {
                #region محكم 1
		        GenerateLitigationRequestFees(1,1.00m,10000.00m,800m),
                GenerateLitigationRequestFees(1,10001.00m,20000.00m,1000.00m),
                GenerateLitigationRequestFees(1,20001.00m,30000.00m,1200.00m),
                GenerateLitigationRequestFees(1,30001.00m,50000.00m,1400.00m),
                GenerateLitigationRequestFees(1,50001.00m,100000.00m,1600.00m),
                GenerateLitigationRequestFees(1,100001.00m,200000.00m,5000.00m,.03m,100000m),

                //GenerateLitigationRequestFees(1,200001.00m,500000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(1,200001.00m,300000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(1,300001.00m,500000.00m,10000.00m,.025m,200000m),

                GenerateLitigationRequestFees(1,500001.00m,1000000.00m,20000.00m,.02m,500000m),
                GenerateLitigationRequestFees(1,1000001.00m,2000000.00m,30000.00m,.01m,1000000m),

                //GenerateLitigationRequestFees(1,2000001.00m,3000000.00m,50000.00m,.005m,2000000m),
                //GenerateLitigationRequestFees(1,3000001.00m,null,80000.00m,.002m,3000000m), 
                GenerateLitigationRequestFees(1,2000001.00m,5000000,80000.00m,.002m,3000000m),
                GenerateLitigationRequestFees(1,5000001.00m,10000000,80000.00m,.002m,3000000m),
                GenerateLitigationRequestFees(1,10000001.00m,25000000,80000.00m,.002m,3000000m),
                GenerateLitigationRequestFees(1,25000001.00m,50000000,80000.00m,.002m,3000000m),
                GenerateLitigationRequestFees(1,50000001.00m,100000000,80000.00m,.002m,3000000m),
                GenerateLitigationRequestFees(1,100000001.00m,null,80000.00m,.002m,3000000m),

                //GenerateLitigationRequestFees(1,3000001.00m,null,80000.00m,.002m,3000000m), 
	            #endregion

                #region محكم 3
		        GenerateLitigationRequestFees(3,1.00m,10000.00m,800m),
                GenerateLitigationRequestFees(3,10001.00m,20000.00m,1000.00m),
                GenerateLitigationRequestFees(3,20001.00m,30000.00m,1200.00m),
                GenerateLitigationRequestFees(3,30001.00m,50000.00m,1400.00m),
                GenerateLitigationRequestFees(3,50001.00m,100000.00m,1600.00m),
                GenerateLitigationRequestFees(3,100001.00m,200000.00m,5000.00m,.03m,100000m),

                //GenerateLitigationRequestFees(3,200001.00m,500000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(3,200001.00m,300000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(3,300001.00m,500000.00m,10000.00m,.025m,200000m),

                GenerateLitigationRequestFees(3,500001.00m,1000000.00m,40000.00m,.02m,500000m),
                GenerateLitigationRequestFees(3,1000001.00m,2000000.00m,60000.00m,.02m,1000000m),
                
                //GenerateLitigationRequestFees(3,2000001.00m,3000000.00m,90000.00m,.01m,2000000m),
                //GenerateLitigationRequestFees(3,3000001.00m,null,120000.00m,.005m,3000000m), 
                GenerateLitigationRequestFees(3,2000001.00m,5000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(3,5000001.00m,10000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(3,10000001.00m,25000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(3,25000001.00m,50000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(3,50000001.00m,100000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(3,100000001.00m,null,120000.00m,.005m,3000000m), 
	            #endregion


                #region محكم 5
		        GenerateLitigationRequestFees(5,1.00m,10000.00m,800m),
                GenerateLitigationRequestFees(5,10001.00m,20000.00m,1000.00m),
                GenerateLitigationRequestFees(5,20001.00m,30000.00m,1200.00m),
                GenerateLitigationRequestFees(5,30001.00m,50000.00m,1400.00m),
                GenerateLitigationRequestFees(5,50001.00m,100000.00m,1600.00m),
                GenerateLitigationRequestFees(5,100001.00m,200000.00m,5000.00m,.03m,100000m),

                //GenerateLitigationRequestFees(5,200001.00m,500000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(5,200001.00m,300000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(5,300001.00m,500000.00m,10000.00m,.025m,200000m),

                GenerateLitigationRequestFees(5,500001.00m,1000000.00m,40000.00m,.02m,500000m),
                GenerateLitigationRequestFees(5,1000001.00m,2000000.00m,60000.00m,.02m,1000000m),
                //GenerateLitigationRequestFees(5,2000001.00m,3000000.00m,90000.00m,.01m,2000000m),
                //GenerateLitigationRequestFees(5,3000001.00m,null,120000.00m,.005m,3000000m), 
                GenerateLitigationRequestFees(5,2000001.00m,5000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(5,5000001.00m,10000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(5,10000001.00m,25000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(5,25000001.00m,50000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(5,50000001.00m,100000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(5,100000001.00m,null,120000.00m,.005m,3000000m), 
	            #endregion


                #region محكم 7
		        GenerateLitigationRequestFees(7,1.00m,10000.00m,800m),
                GenerateLitigationRequestFees(7,10001.00m,20000.00m,1000.00m),
                GenerateLitigationRequestFees(7,20001.00m,30000.00m,1200.00m),
                GenerateLitigationRequestFees(7,30001.00m,50000.00m,1400.00m),
                GenerateLitigationRequestFees(7,50001.00m,100000.00m,1600.00m),
                GenerateLitigationRequestFees(7,100001.00m,200000.00m,5000.00m,.03m,100000m),

                //GenerateLitigationRequestFees(7,200001.00m,500000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(7,200001.00m,300000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(7,300001.00m,500000.00m,10000.00m,.025m,200000m),

                GenerateLitigationRequestFees(7,500001.00m,1000000.00m,40000.00m,.02m,500000m),
                GenerateLitigationRequestFees(7,1000001.00m,2000000.00m,60000.00m,.02m,1000000m),
                //GenerateLitigationRequestFees(7,2000001.00m,3000000.00m,90000.00m,.01m,2000000m),
                //GenerateLitigationRequestFees(7,3000001.00m,null,120000.00m,.005m,3000000m), 
                GenerateLitigationRequestFees(7,2000001.00m,5000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(7,5000001.00m,10000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(7,10000001.00m,25000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(7,25000001.00m,50000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(7,50000001.00m,100000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(7,100000001.00m,null,120000.00m,.005m,3000000m), 
	            #endregion


                #region محكم 9
		        GenerateLitigationRequestFees(9,1.00m,10000.00m,800m),
                GenerateLitigationRequestFees(9,10001.00m,20000.00m,1000.00m),
                GenerateLitigationRequestFees(9,20001.00m,30000.00m,1200.00m),
                GenerateLitigationRequestFees(9,30001.00m,50000.00m,1400.00m),
                GenerateLitigationRequestFees(9,50001.00m,100000.00m,1600.00m),
                GenerateLitigationRequestFees(9,100001.00m,200000.00m,5000.00m,.03m,100000m),

                //GenerateLitigationRequestFees(9,200001.00m,500000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(9,200001.00m,300000.00m,10000.00m,.025m,200000m),
                GenerateLitigationRequestFees(9,300001.00m,500000.00m,10000.00m,.025m,200000m),

                GenerateLitigationRequestFees(9,500001.00m,1000000.00m,40000.00m,.02m,500000m),
                GenerateLitigationRequestFees(9,1000001.00m,2000000.00m,60000.00m,.02m,1000000m),
                //GenerateLitigationRequestFees(9,2000001.00m,3000000.00m,90000.00m,.01m,2000000m),
                //GenerateLitigationRequestFees(9,3000001.00m,null,120000.00m,.005m,3000000m), 
                GenerateLitigationRequestFees(9,2000001.00m,5000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(9,5000001.00m,10000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(9,10000001.00m,25000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(9,25000001.00m,50000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(9,50000001.00m,100000000.00m,120000.00m,.005m,3000000m),
                GenerateLitigationRequestFees(9,100000001.00m,null,120000.00m,.005m,3000000m), 
	            #endregion

            };
        }

        static LitigationRequestFees GenerateLitigationRequestFees(int arbitrationCount, decimal fromAmount, decimal? toAmount, decimal fees, decimal? percentageExtraAmount = null, decimal? extraAmount = null)
        {
            return new LitigationRequestFees
            {
                Id = Guid.NewGuid(),
                ArbitratorsCount = arbitrationCount,
                FromAmount = fromAmount,
                ToAmount = toAmount,
                Fees = fees,
                PercentageExtraAmount = percentageExtraAmount,
                ExtraAmount = extraAmount
            };
        }
    }
}
