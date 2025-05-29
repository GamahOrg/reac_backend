using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Reports
{

    public class GetReviwersReportResult
    {
        public string? ReviewerName { get; set; }
        public string? ReviewerStatus { get; set; }

        public int? TotalLitigationCount { get; set; }
        public int? NotFinishedCount { get; set; }
        public int? FinishedCount { get; set; }
        public int? TotalReviewDueDateCount { get; set; }

        public decimal? NotFinishedRatio { get; set; }
        public decimal? FinishedRatio { get; set; }
        public decimal? TotalReviewDueDateRatio { get; set; }

    }

    public class GetReformersReportResult
    {
        public string? ReformerName { get; set; }
        public string? ReformerStatus { get; set; }
        public int? ExperienceYears { get; set; }
        public int? TotalLitigationCount { get; set; }
        public int? NotFinishedCount { get; set; }
        public int? FinishedCount { get; set; }
        public int? DueDateCount { get; set; }

        public decimal? NotFinishedRatio { get; set; }
        public decimal? FinishedRatio { get; set; }
        public decimal? DueDateRatio { get; set; }

    }


    public class GetArbitratorsReportResult
    {
        public string? ArbitratorName { get; set; }
        public string? ArbitratorStatus { get; set; }
        public int? ExperienceYears { get; set; }
        public int? TotalLitigationCount { get; set; }
        public int? NotFinishedCount { get; set; }
        public int? FinishedCount { get; set; }
        public int? DueDateCount { get; set; }

        public decimal? NotFinishedRatio { get; set; }
        public decimal? FinishedRatio { get; set; }
        public decimal? DueDateRatio { get; set; }

    }


}
