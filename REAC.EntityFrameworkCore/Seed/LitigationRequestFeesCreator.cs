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
            context.SaveChanges();
        }

        static List<LitigationRequestFees> AddFees()
        {
            return new List<LitigationRequestFees>
            {
                #region محكم 1
		        GenerateLitigationRequestFees(1,Guid.Parse("049A31BE-0A9D-49F1-B0D6-96112EFF872E"),800m),
                GenerateLitigationRequestFees(1,Guid.Parse("2C30F52E-EA14-4C5B-AA31-C34FDE466A35"),1000.00m),
                GenerateLitigationRequestFees(1,Guid.Parse("188A3533-F6D7-4D8D-815E-2EED483366E5"),1200.00m),
                GenerateLitigationRequestFees(1,Guid.Parse("30B8CB40-3FAF-4BDC-A932-32265CF6F0F7"),1400.00m),
                GenerateLitigationRequestFees(1,Guid.Parse("EB675BE6-7E2B-479D-B82A-BCD1C2D9CA55"),1600.00m),
                GenerateLitigationRequestFees(1,Guid.Parse("C4E1F0D2-E9E4-40E4-81CB-0F4056630810"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("C443125E-3BAB-4DBD-8059-5224EB5B33C9"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("D400C677-B62F-487A-ABC6-502CD95B306B"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("5FAD6041-95A2-46B3-8B40-7A5CF9673D89"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("91B8B9C2-385C-4EC9-8AE8-90F15E82A823"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("16915895-323A-4999-B711-B0ED5E4B4AA7"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("AF38248D-0466-4983-BF16-C1EA3BBF3B2C"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("35613DF2-2AA9-4EC2-9FBC-D45F297EB64A"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("77962EEC-DC9C-4114-BCC0-E9981FDE0D42"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("FBC1ADCE-4F2A-41FC-91A8-F236675E53E8"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(1,Guid.Parse("1FC9EFC9-E6F0-46D9-B327-F60114937DC6"),5000.00m,.03m,100000m),

	            #endregion

                #region محكم 3
		        GenerateLitigationRequestFees(3,Guid.Parse("049A31BE-0A9D-49F1-B0D6-96112EFF872E"),800m),
                GenerateLitigationRequestFees(3,Guid.Parse("2C30F52E-EA14-4C5B-AA31-C34FDE466A35"),1000.00m),
                GenerateLitigationRequestFees(3,Guid.Parse("188A3533-F6D7-4D8D-815E-2EED483366E5"),1200.00m),
                GenerateLitigationRequestFees(3,Guid.Parse("30B8CB40-3FAF-4BDC-A932-32265CF6F0F7"),1400.00m),
                GenerateLitigationRequestFees(3,Guid.Parse("EB675BE6-7E2B-479D-B82A-BCD1C2D9CA55"),1600.00m),
                GenerateLitigationRequestFees(3,Guid.Parse("C4E1F0D2-E9E4-40E4-81CB-0F4056630810"),5000.00m,.03m,100000m),                GenerateLitigationRequestFees(1,Guid.Parse("C443125E-3BAB-4DBD-8059-5224EB5B33C9"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("D400C677-B62F-487A-ABC6-502CD95B306B"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("5FAD6041-95A2-46B3-8B40-7A5CF9673D89"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("91B8B9C2-385C-4EC9-8AE8-90F15E82A823"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("16915895-323A-4999-B711-B0ED5E4B4AA7"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("AF38248D-0466-4983-BF16-C1EA3BBF3B2C"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("35613DF2-2AA9-4EC2-9FBC-D45F297EB64A"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("77962EEC-DC9C-4114-BCC0-E9981FDE0D42"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("FBC1ADCE-4F2A-41FC-91A8-F236675E53E8"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(3,Guid.Parse("1FC9EFC9-E6F0-46D9-B327-F60114937DC6"),5000.00m,.03m,100000m),
	            #endregion


                #region محكم 5
		        GenerateLitigationRequestFees(5,Guid.Parse("049A31BE-0A9D-49F1-B0D6-96112EFF872E"),800m),
                GenerateLitigationRequestFees(5,Guid.Parse("2C30F52E-EA14-4C5B-AA31-C34FDE466A35"),1000.00m),
                GenerateLitigationRequestFees(5,Guid.Parse("188A3533-F6D7-4D8D-815E-2EED483366E5"),1200.00m),
                GenerateLitigationRequestFees(5,Guid.Parse("30B8CB40-3FAF-4BDC-A932-32265CF6F0F7"),1400.00m),
                GenerateLitigationRequestFees(5,Guid.Parse("EB675BE6-7E2B-479D-B82A-BCD1C2D9CA55"),1600.00m),
                GenerateLitigationRequestFees(5,Guid.Parse("C4E1F0D2-E9E4-40E4-81CB-0F4056630810"),5000.00m,.03m,100000m),                GenerateLitigationRequestFees(1,Guid.Parse("C443125E-3BAB-4DBD-8059-5224EB5B33C9"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("D400C677-B62F-487A-ABC6-502CD95B306B"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("5FAD6041-95A2-46B3-8B40-7A5CF9673D89"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("91B8B9C2-385C-4EC9-8AE8-90F15E82A823"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("16915895-323A-4999-B711-B0ED5E4B4AA7"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("AF38248D-0466-4983-BF16-C1EA3BBF3B2C"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("35613DF2-2AA9-4EC2-9FBC-D45F297EB64A"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("77962EEC-DC9C-4114-BCC0-E9981FDE0D42"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("FBC1ADCE-4F2A-41FC-91A8-F236675E53E8"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(5,Guid.Parse("1FC9EFC9-E6F0-46D9-B327-F60114937DC6"),5000.00m,.03m,100000m),
	            #endregion


                #region محكم 7
		        GenerateLitigationRequestFees(7,Guid.Parse("049A31BE-0A9D-49F1-B0D6-96112EFF872E"),800m),
                GenerateLitigationRequestFees(7,Guid.Parse("2C30F52E-EA14-4C5B-AA31-C34FDE466A35"),1000.00m),
                GenerateLitigationRequestFees(7,Guid.Parse("188A3533-F6D7-4D8D-815E-2EED483366E5"),1200.00m),
                GenerateLitigationRequestFees(7,Guid.Parse("30B8CB40-3FAF-4BDC-A932-32265CF6F0F7"),1400.00m),
                GenerateLitigationRequestFees(7,Guid.Parse("EB675BE6-7E2B-479D-B82A-BCD1C2D9CA55"),1600.00m),
                GenerateLitigationRequestFees(7,Guid.Parse("C4E1F0D2-E9E4-40E4-81CB-0F4056630810"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("C443125E-3BAB-4DBD-8059-5224EB5B33C9"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("D400C677-B62F-487A-ABC6-502CD95B306B"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("5FAD6041-95A2-46B3-8B40-7A5CF9673D89"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("91B8B9C2-385C-4EC9-8AE8-90F15E82A823"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("16915895-323A-4999-B711-B0ED5E4B4AA7"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("AF38248D-0466-4983-BF16-C1EA3BBF3B2C"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("35613DF2-2AA9-4EC2-9FBC-D45F297EB64A"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("77962EEC-DC9C-4114-BCC0-E9981FDE0D42"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("FBC1ADCE-4F2A-41FC-91A8-F236675E53E8"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(7,Guid.Parse("1FC9EFC9-E6F0-46D9-B327-F60114937DC6"),5000.00m,.03m,100000m),
	            #endregion


                #region محكم 9
		        GenerateLitigationRequestFees(9,Guid.Parse("049A31BE-0A9D-49F1-B0D6-96112EFF872E"),800m),
                GenerateLitigationRequestFees(9,Guid.Parse("2C30F52E-EA14-4C5B-AA31-C34FDE466A35"),1000.00m),
                GenerateLitigationRequestFees(9,Guid.Parse("188A3533-F6D7-4D8D-815E-2EED483366E5"),1200.00m),
                GenerateLitigationRequestFees(9,Guid.Parse("30B8CB40-3FAF-4BDC-A932-32265CF6F0F7"),1400.00m),
                GenerateLitigationRequestFees(9,Guid.Parse("EB675BE6-7E2B-479D-B82A-BCD1C2D9CA55"),1600.00m),
                GenerateLitigationRequestFees(9,Guid.Parse("C4E1F0D2-E9E4-40E4-81CB-0F4056630810"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("C443125E-3BAB-4DBD-8059-5224EB5B33C9"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("D400C677-B62F-487A-ABC6-502CD95B306B"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("5FAD6041-95A2-46B3-8B40-7A5CF9673D89"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("91B8B9C2-385C-4EC9-8AE8-90F15E82A823"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("16915895-323A-4999-B711-B0ED5E4B4AA7"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("AF38248D-0466-4983-BF16-C1EA3BBF3B2C"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("35613DF2-2AA9-4EC2-9FBC-D45F297EB64A"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("77962EEC-DC9C-4114-BCC0-E9981FDE0D42"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("FBC1ADCE-4F2A-41FC-91A8-F236675E53E8"),5000.00m,.03m,100000m),
                GenerateLitigationRequestFees(9,Guid.Parse("1FC9EFC9-E6F0-46D9-B327-F60114937DC6"),5000.00m,.03m,100000m),
	            #endregion

            };
        }

        static LitigationRequestFees GenerateLitigationRequestFees(int arbitrationCount,Guid amountId, decimal fees, decimal? percentageExtraAmount = null, decimal? extraAmount = null)
        {
            return new LitigationRequestFees
            {
                Id = Guid.NewGuid(),
                ArbitratorsCount = arbitrationCount,
                AmountId = amountId,
                ArbitrationFees = fees,
                PercentageExtraAmount = percentageExtraAmount,
                ExtraAmount = extraAmount
            };
        }
    }
}
