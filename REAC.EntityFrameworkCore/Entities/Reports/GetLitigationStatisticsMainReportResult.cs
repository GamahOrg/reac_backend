using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetLitigationStatisticsMainReportResult
    {
        public int TotalLitigationCount { get; set; }
        public int NotFinishedCount { get; set; }
        public int FinishedCount { get; set; }
        public int RealEstateBrokerageCount { get; set; }
        public int WafiCount { get; set; }
        public int OtherServicesCount { get; set; }
        public int RentCount { get; set; }
        public int PropertiesCount { get; set; }

        public decimal NotFinishedRatio { get; set; }
        public decimal FinishedRatio { get; set; }
        public decimal RealEstateBrokerageRatio { get; set; }
        public decimal WafiRatio { get; set; }
        public decimal OtherServicesRatio { get; set; }
        public decimal RentRatio { get; set; }
        public decimal PropertiesRatio { get; set; }
    }
}
