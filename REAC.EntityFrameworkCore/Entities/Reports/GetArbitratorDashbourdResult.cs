using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Reports
{

    public class GetArbitratorDashbourdResult
    {
        public int? TotalLitigationCount { get; set; }
        public int? NotFinishedCount { get; set; }

        public int? FinishedCount { get; set; }
        public int? DueDateReconciliationCount { get; set; }
        public int? DueDateArbitrationCount { get; set; }
        public int? RealEstateBrokerageCount { get; set; }
        public int? RealEstateBrokerageFinishedCount { get; set; }
        public int? RealEstateBrokerageNotFinishedCount { get; set; }
        public int? RealEstateBrokerageDueDateCount { get; set; }
        public int? WafiCount { get; set; }
        public int? WafiFinishedCount { get; set; }
        public int? WafiNotFinishedCount { get; set; }
        public int? WafiDueDateCount { get; set; }
        public int? OtherServicesCount { get; set; }
        public int? OtherServicesFinishedCount { get; set; }
        public int? OtherServicesNotFinishedCount { get; set; }
        public int? OtherServicesDueDateCount { get; set; }
        public int? RentCount { get; set; }
        public int? RentFinishedCount { get; set; }
        public int? RentNotFinishedCount { get; set; }
        public int? RentDueDateCount { get; set; }
        public int? PropertiesCount { get; set; }
        public int? PropertiesFinishedCount { get; set; }
        public int? PropertiesNotFinishedCount { get; set; }
        public int? PropertiesDueDateCount { get; set; }
        public decimal? NotFinishedRatio { get; set; }
        public decimal? FinishedRatio { get; set; }
        public decimal? DueDateReconciliationRatio { get; set; }
        public decimal? DueDateArbitrationRatio { get; set; }
        public decimal? RealEstateBrokerageRatio { get; set; }
        public decimal? RealEstateBrokerageFinishedRatio { get; set; }
        public decimal? RealEstateBrokerageNotFinishedRatio { get; set; }
        public decimal? RealEstateBrokerageDueDateRatio { get; set; }

        public decimal? WafiRatio { get; set; }
        public decimal? WafiFinishedRatio { get; set; }
        public decimal? WafiNotFinishedRatio { get; set; }
        public decimal? WafiDueDateRatio { get; set; }
        public decimal? OtherServicesRatio { get; set; }
        public decimal? OtherServicesFinishedRatio { get; set; }
        public decimal? OtherServicesNotFinishedRatio { get; set; }
        public decimal? OtherServicesDueDateRatio { get; set; }
        public decimal? RentRatio { get; set; }
        public decimal? RentFinishedRatio { get; set; }
        public decimal? RentNotFinishedRatio { get; set; }
        public decimal? RentDueDateRatio { get; set; }
        public decimal? PropertiesRatio { get; set; }
        public decimal? PropertiesFinishedRatio { get; set; }
        public decimal? PropertiesNotFinishedRatio { get; set; }
        public decimal? PropertiesDueDateRatio { get; set; }
        public int? TotalReviewDueDateCount { get; set; }
        public decimal? TotalReviewDueDateRatio { get; set; }

    }

}
