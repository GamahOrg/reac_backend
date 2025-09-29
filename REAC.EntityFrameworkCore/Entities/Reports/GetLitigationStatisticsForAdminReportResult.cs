using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetLitigationStatisticsForAdminReportResult
    {
        [Comment("Description for Litigation Request Stage")]
        public string LitigationRequestStage { get; set; }
        [Comment("Count of items")]
        public int TotalLitigationCount { get; set; }
        [Comment("Count of items")]
        public int? NotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? FinishedCount { get; set; }
        [Comment("Count of items")]
        public int? RealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? WafiCount { get; set; }
        [Comment("Count of items")]
        public int? OtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? RentCount { get; set; }
        [Comment("Count of items")]
        public int? PropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewDueDateCount { get; set; }
        [Comment("Count of items")]
        public int? DueDateReconciliationCount { get; set; }
        [Comment("Count of items")]
        public int? DueDateArbitrationCount { get; set; }
        [Comment("Description for Not Finished Ratio")]
        public decimal? NotFinishedRatio { get; set; }
        [Comment("Description for Finished Ratio")]
        public decimal? FinishedRatio { get; set; }
        [Comment("Description for Real Estate Brokerage Ratio")]
        public decimal? RealEstateBrokerageRatio { get; set; }
        [Comment("Description for Wafi Ratio")]
        public decimal? WafiRatio { get; set; }
        [Comment("Description for Other Services Ratio")]
        public decimal? OtherServicesRatio { get; set; }
        [Comment("Description for Rent Ratio")]
        public decimal? RentRatio { get; set; }
        [Comment("Description for Properties Ratio")]
        public decimal? PropertiesRatio { get; set; }
        [Comment("Total")]
        public decimal? TotalReviewDueDateRatio { get; set; }
        [Comment("Description for Due Date Reconciliation Ratio")]
        public decimal? DueDateReconciliationRatio { get; set; }
        [Comment("Description for Due Date Arbitration Ratio")]
        public decimal? DueDateArbitrationRatio { get; set; }
    }
}

