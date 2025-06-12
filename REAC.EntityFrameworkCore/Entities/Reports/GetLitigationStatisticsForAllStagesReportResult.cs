using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetLitigationStatisticsForAllStagesReportResult
    {
			public int TotalReviewAllCount { get; set; }
			public int TotalReviewNotFinishedCount { get; set; }
			public int TotalReviewFinishedCount { get; set; }
			public int TotalReviewDueDateCount { get; set; }
			public int TotalReviewRealEstateBrokerageCount { get; set; }
			public int TotalReviewWafiCount { get; set; }
			public int TotalReviewOtherServicesCount { get; set; }
			public int TotalReviewPropertiesCount { get; set; }
			public int TotalReviewRentCount { get; set; }


            public int TotalReconciliationAllCount { get; set; }
            public int TotalReconciliationNotFinishedCount { get; set; }
            public int TotalReconciliationFinishedCount { get; set; }
            public int ReconciliationWasMadeCount { get; set; }
            public int WaiverOfRightCount { get; set; }
            public int WaiverOfTheLawSuitCount { get; set; }
            public int LackOfReconciliationCount { get; set; }
            public int TheReconciliationOnTradyPatformCount { get; set; }
            public int TotalReconciliationDueDateCount { get; set; }
            public int TotalReconciliationRealEstateBrokerageCount { get; set; }
            public int TotalReconciliationWafiCount { get; set; }
            public int TotalReconciliationOtherServicesCount { get; set; }
            public int TotalReconciliationPropertiesCount { get; set; }
            public int TotalReconciliationRentCount { get; set; }
        
        
            public int TotalArbitrationAllCount { get; set; }
            public int TotalArbitrationNotFinishedCount { get; set; }
            public int TotalArbitrationFinishedCount { get; set; }
            public int TotalArbitrationDueDateCount { get; set; }
            public int TotalArbitrationRealEstateBrokerageCount { get; set; }
            public int TotalArbitrationWafiCount { get; set; }
            public int TotalArbitrationOtherServicesCount { get; set; }
            public int TotalArbitrationPropertiesCount { get; set; }
            public int TotalArbitrationRentCount { get; set; }



    }
}
