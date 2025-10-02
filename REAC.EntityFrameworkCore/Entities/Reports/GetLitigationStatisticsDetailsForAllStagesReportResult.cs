using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetLitigationStatisticsDetailsForAllStagesReportResult
    {
        #region Review
        [Comment("Count of items")]
        public int? TotalReviewNotFinishedRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewNotFinishedWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewNotFinishedOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewNotFinishedPropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewNotFinishedRentCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewDueDateRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewDueDateWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewDueDateOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewDueDatePropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewDueDateRentCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewFinishedRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewFinishedWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewFinishedOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewFinishedPropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReviewFinishedRentCount { get; set; }
        #endregion
        
        #region Reconciliation
        [Comment("Count of items")]
        public int? TotalReconciliationNotFinishedRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationNotFinishedWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationNotFinishedOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationNotFinishedPropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationNotFinishedRentCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationDueDateRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationDueDateWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationDueDateOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationDueDatePropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationDueDateRentCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationFinishedRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationFinishedWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationFinishedOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationFinishedPropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalReconciliationFinishedRentCount { get; set; }
        #endregion

        #region Arbitration
        [Comment("Count of items")]
        public int? TotalArbitrationNotFinishedRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationNotFinishedWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationNotFinishedOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationNotFinishedPropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationNotFinishedRentCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationDueDateRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationDueDateWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationDueDateOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationDueDatePropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationDueDateRentCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationFinishedRealEstateBrokerageCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationFinishedWafiCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationFinishedOtherServicesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationFinishedPropertiesCount { get; set; }
        [Comment("Count of items")]
        public int? TotalArbitrationFinishedRentCount { get; set; }
        #endregion

    }
}

