using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Domain.Shared.Helpers
{
    public class RequestCode   // رقم الدعوي
    {
        public static string GenerateRequestCode(LitigationRequestContractType litigationRequestContractTypeId, int litigationCount)
        {
            string code = "";
            var x = (int)LitigationRequestContractType.Rent;
            switch (litigationRequestContractTypeId)
            {
                case LitigationRequestContractType.Rent:
                    if (litigationCount != 0)
                        code = "R" + (litigationCount + 1).ToString("D5") + "-" + DateTime.Now.ToString("yy");
                    else
                        code = "R00001" + "-" + DateTime.Now.ToString("yy"); 
                    break;

                case LitigationRequestContractType.Properties:
                    if (litigationCount != 0)
                        code = "O" + (litigationCount + 1).ToString("D5") + "-" + DateTime.Now.ToString("yy");
                    else
                        code = "O00001" + "-" + DateTime.Now.ToString("yy");
                    break;

                case LitigationRequestContractType.Wafi:
                    if (litigationCount != 0)
                        code = "W" + (litigationCount + 1).ToString("D5") + "-" + DateTime.Now.ToString("yy");
                    else
                        code = "W00001" + "-" + DateTime.Now.ToString("yy");
                    break;

                case LitigationRequestContractType.RealEstateBrokerage:
                    if (litigationCount != 0)
                        code = "B" + (litigationCount + 1).ToString("D5") + "-" + DateTime.Now.ToString("yy");
                     else
                        code = "B00001" + "-" + DateTime.Now.ToString("yy");
                    break;

                case LitigationRequestContractType.OtherServices:
                    if (litigationCount != 0)
                        code = "N" + (litigationCount + 1).ToString("D5") + "-" + DateTime.Now.ToString("yy");
                    else
                        code = "N00001" + "-" + DateTime.Now.ToString("yy");

                    break;
            }

            return code;
        }
    }
}
