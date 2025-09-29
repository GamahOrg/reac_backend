using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{

    public class GetArbitratorDashbourdResult
    {
        [Comment("Count of items")]
        public int? TotalLitigationCount { get; set; }
        [Comment("Count of items")]
        public int? NotFinishedCount { get; set; }

        [Comment("Count of items")]
        public int? FinishedCount { get; set; }
        [Comment("Count of items")]
        public int? DueDateReconciliationCount { get; set; }
        [Comment("Count of items")]
        public int? DueDateArbitrationCount { get; set; }
        [Comment("Count of items")]
        public int? RealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? RealEstateBrokerageFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? RealEstateBrokerageNotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? RealEstateBrokerageDueDateCount { get; set; }
        [Comment("Count of items")]
        public int? WafiCount { get; set; }
        [Comment("Count of items")]
        public int? WafiFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? WafiNotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? WafiDueDateCount { get; set; }
        [Comment("Count of items")]
        public int? OtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? OtherServicesFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? OtherServicesNotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? OtherServicesDueDateCount { get; set; }
        [Comment("Count of items")]
        public int? RentCount { get; set; }
        [Comment("Count of items")]
        public int? RentFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? RentNotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? RentDueDateCount { get; set; }
        [Comment("Count of items")]
        public int? PropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? PropertiesFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? PropertiesNotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? PropertiesDueDateCount { get; set; }
        [Comment("Description for Not Finished Ratio")]
        public decimal? NotFinishedRatio { get; set; }
        [Comment("Description for Finished Ratio")]
        public decimal? FinishedRatio { get; set; }
        [Comment("Description for Due Date Reconciliation Ratio")]
        public decimal? DueDateReconciliationRatio { get; set; }
        [Comment("Description for Due Date Arbitration Ratio")]
        public decimal? DueDateArbitrationRatio { get; set; }
        [Comment("Description for Real Estate Brokerage Ratio")]
        public decimal? RealEstateBrokerageRatio { get; set; }
        [Comment("Description for Real Estate Brokerage Finished Ratio")]
        public decimal? RealEstateBrokerageFinishedRatio { get; set; }
        [Comment("Description for Real Estate Brokerage Not Finished Ratio")]
        public decimal? RealEstateBrokerageNotFinishedRatio { get; set; }
        [Comment("Description for Real Estate Brokerage Due Date Ratio")]
        public decimal? RealEstateBrokerageDueDateRatio { get; set; }

        [Comment("Description for Wafi Ratio")]
        public decimal? WafiRatio { get; set; }
        [Comment("Description for Wafi Finished Ratio")]
        public decimal? WafiFinishedRatio { get; set; }
        [Comment("Description for Wafi Not Finished Ratio")]
        public decimal? WafiNotFinishedRatio { get; set; }
        [Comment("Description for Wafi Due Date Ratio")]
        public decimal? WafiDueDateRatio { get; set; }
        [Comment("Description for Other Services Ratio")]
        public decimal? OtherServicesRatio { get; set; }
        [Comment("Description for Other Services Finished Ratio")]
        public decimal? OtherServicesFinishedRatio { get; set; }
        [Comment("Description for Other Services Not Finished Ratio")]
        public decimal? OtherServicesNotFinishedRatio { get; set; }
        [Comment("Description for Other Services Due Date Ratio")]
        public decimal? OtherServicesDueDateRatio { get; set; }
        [Comment("Description for Rent Ratio")]
        public decimal? RentRatio { get; set; }
        [Comment("Description for Rent Finished Ratio")]
        public decimal? RentFinishedRatio { get; set; }
        [Comment("Description for Rent Not Finished Ratio")]
        public decimal? RentNotFinishedRatio { get; set; }
        [Comment("Description for Rent Due Date Ratio")]
        public decimal? RentDueDateRatio { get; set; }
        [Comment("Description for Properties Ratio")]
        public decimal? PropertiesRatio { get; set; }
        [Comment("Description for Properties Finished Ratio")]
        public decimal? PropertiesFinishedRatio { get; set; }
        [Comment("Description for Properties Not Finished Ratio")]
        public decimal? PropertiesNotFinishedRatio { get; set; }
        [Comment("Description for Properties Due Date Ratio")]
        public decimal? PropertiesDueDateRatio { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewDueDateCount { get; set; }
        [Comment("Total")]
        public decimal? TotalReviewDueDateRatio { get; set; }

    }

}

