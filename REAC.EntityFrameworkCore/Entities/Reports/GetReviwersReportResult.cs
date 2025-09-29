using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{

    public class GetReviwersReportResult
    {
        [Comment("Name of the reviewer")]
        public string? ReviewerName { get; set; }
        [Comment("Status of the reviewer")]
        public string? ReviewerStatus { get; set; }

        [Comment("Total number of litigations assigned to reviewer")]
        public int? TotalLitigationCount { get; set; }
        [Comment("Number of unfinished litigation reviews")]
        public int? NotFinishedCount { get; set; }
        [Comment("Number of finished litigation reviews")]
        public int? FinishedCount { get; set; }
        [Comment("Number of litigations with review due dates")]
        public int? TotalReviewDueDateCount { get; set; }

        [Comment("Ratio of unfinished litigation reviews")]
        public decimal? NotFinishedRatio { get; set; }
        [Comment("Ratio of finished litigation reviews")]
        public decimal? FinishedRatio { get; set; }
        [Comment("Ratio of total review due dates")]
        public decimal? TotalReviewDueDateRatio { get; set; }

        [Comment("Total count of all items")]
        public int TotalCount { get; set; }

    }

    public class GetReformersReportResult
    {
        [Comment("Name of the reformer")]
        public string? ReformerName { get; set; }
        [Comment("Status of the reformer")]
        public string? ReformerStatus { get; set; }
        [Comment("Number of years of experience for the reformer")]
        public int? ExperienceYears { get; set; }
        [Comment("Count of items")]
        public int? TotalLitigationCount { get; set; }
        [Comment("Count of items")]
        public int? NotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? FinishedCount { get; set; }
        [Comment("Count of items")]
        public int? DueDateCount { get; set; }

        [Comment("Description for Not Finished Ratio")]
        public decimal? NotFinishedRatio { get; set; }
        [Comment("Description for Finished Ratio")]
        public decimal? FinishedRatio { get; set; }
        [Comment("Description for Due Date Ratio")]
        public decimal? DueDateRatio { get; set; }

        [Comment("Count of items")]
        public int TotalCount { get; set; }
    }


    public class GetArbitratorsReportResult
    {
        [Comment("Name of the reviewer")]
        public string? ArbitratorName { get; set; }
        [Comment("Description for Reviewer Status")]
        public string? ArbitratorStatus { get; set; }
        [Comment("Description for Experience Years")]
        public int? ExperienceYears { get; set; }
        [Comment("Count of items")]
        public int? TotalLitigationCount { get; set; }
        [Comment("Count of items")]
        public int? NotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? FinishedCount { get; set; }
        [Comment("Count of items")]
        public int? DueDateCount { get; set; }

        [Comment("Description for Not Finished Ratio")]
        public decimal? NotFinishedRatio { get; set; }
        [Comment("Description for Finished Ratio")]
        public decimal? FinishedRatio { get; set; }
        [Comment("Description for Due Date Ratio")]
        public decimal? DueDateRatio { get; set; }

        [Comment("Count of items")]
        public int TotalCount { get; set; }

    }

    public class GetReviewersReformersArbitratorsReportResult
    {
        [Comment("Name of the reviewer")]
        public string PersonnelName { get; set; }
        [Comment("Description for Personnel Type")]
        public string PersonnelType { get; set; }
        [Comment("Description for Reviewer Status")]
        public string PersonnelStatus { get; set; }
        [Comment("Unique identifier of the personnel type")]
        public int? PersonnelTypeId { get; set; }
        [Comment("Description for Experience Years")]
        public int? ExperienceYears { get; set; }

        [Comment("Count of items")]
        public int? TotalLitigationCount { get; set; }
        [Comment("Count of items")]
        public int? NotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? FinishedCount { get; set; }
        [Comment("Count of items")]
        public int? DueDateCount { get; set; }
        
        [Comment("Count of items")]
        public int? ChairmanTotalCount { get; set; }
        [Comment("Count of items")]
        public int? ChairmanNotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? ChairmanFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? ChairmanDueDateCount { get; set; }
        
        [Comment("Count of items")]
        public int? ArbitratorTotalCount { get; set; }
        [Comment("Count of items")]
        public int? ArbitratorNotFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? ArbitratorFinishedCount { get; set; }
        [Comment("Count of items")]
        public int? ArbitratorDueDateCount { get; set; }

        [Comment("Description for Not Finished Ratio")]
        public decimal? NotFinishedRatio { get; set; }
        [Comment("Description for Finished Ratio")]
        public decimal? FinishedRatio { get; set; }
        [Comment("Description for Due Date Ratio")]
        public decimal? DueDateRatio { get; set; }

        [Comment("Count of items")]
        public int TotalCount { get; set; }

    }
    
    public class GetReviewersReformersArbitratorsStatisticsReportResult
    {
       
        [Comment("Description for Certified Reviewers")]
        public int CertifiedReviewers { get; set; }
        [Comment("Description for Stopped Reviewers")]
        public int StoppedReviewers { get; set; }
        [Comment("Description for Certified Reformers")]
        public int CertifiedReformers { get; set; }
        [Comment("Description for Stopped Reformers")]
        public int StoppedReformers { get; set; }
        [Comment("Description for Certified Arbitrators")]
        public int CertifiedArbitrators { get; set; }
        [Comment("Description for Stopped Arbitrators")]
        public int StoppedArbitrators { get; set; }

    }

}


