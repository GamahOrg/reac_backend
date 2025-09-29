using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetLitigationStatisticsForAllStagesReportResult
    {
			[Comment("Count of items")]
			public int TotalReviewAllCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewNotFinishedCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewFinishedCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewDueDateCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewRealEstateBrokerageCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewWafiCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewOtherServicesCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewPropertiesCount { get; set; }
			[Comment("Count of items")]
			public int TotalReviewRentCount { get; set; }


            [Comment("Count of items")]
            public int TotalReconciliationAllCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationNotFinishedCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationFinishedCount { get; set; }
            [Comment("Count of items")]
            public int ReconciliationWasMadeCount { get; set; }
            [Comment("Count of items")]
            public int WaiverOfRightCount { get; set; }
            [Comment("Count of items")]
            public int WaiverOfTheLawSuitCount { get; set; }
            [Comment("Count of items")]
            public int LackOfReconciliationCount { get; set; }
            [Comment("Count of items")]
            public int TheReconciliationOnTradyPatformCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationDueDateCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationRealEstateBrokerageCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationWafiCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationOtherServicesCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationPropertiesCount { get; set; }
            [Comment("Count of items")]
            public int TotalReconciliationRentCount { get; set; }
        
        
            [Comment("Count of items")]
            public int TotalArbitrationAllCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationNotFinishedCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationFinishedCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationDueDateCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationRealEstateBrokerageCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationWafiCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationOtherServicesCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationPropertiesCount { get; set; }
            [Comment("Count of items")]
            public int TotalArbitrationRentCount { get; set; }



    }
}

