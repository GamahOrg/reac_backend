using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetListOfLitigationReviewReportResult
    {
        [Comment("Name of the employee")]
        public string? EmployeeName { get; set; }
        [Comment("Date and time when the record was created")]
        public DateTime? CreatedOn { get; set; }
        [Comment("Description for Request Code")]
        public string? RequestCode { get; set; }
        [Comment("Address")]
        public string? ContractAddress { get; set; }
        [Comment("Amount")]
        public string? LitigationAmount { get; set; }
        [Comment("Name of the employee")]
        public string? AreaName { get; set; }
        [Comment("Name of the employee")]
        public string? CityName { get; set; }
        [Comment("Category of the name")]
        public string? CategoryName { get; set; }
        [Comment("Description for Request Status")]
        public string? RequestStatus { get; set; }
        [Comment("Description for First Party")]
        public string? FirstParty { get; set; }
        [Comment("Description for Second Party")]
        public string? SecondParty { get; set; }
        [Comment("Count of items")]
        public string? TotalCount { get; set; }
    }
    
  
}


