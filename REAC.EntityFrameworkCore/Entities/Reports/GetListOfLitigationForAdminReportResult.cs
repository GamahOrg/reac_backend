using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetListOfLitigationForAdminReportResult
    {
        [Comment("Full name")]
        public string? ArbitratorsFullNames { get; set; }
        [Comment("Name of the arbitrator chairman")]
        public string? ArbitratorChairmanName { get; set; }
        [Comment("Name of the arbitrator chairman")]
        public string? ReformerName { get; set; }
        [Comment("Name of the arbitrator chairman")]
        public string? EmployeeName { get; set; }
        [Comment("Date and time when the record was created")]
        public DateTime? CreatedOn { get; set; }
        [Comment("Description for Request Code")]
        public string? RequestCode { get; set; }
        [Comment("Address")]
        public string? ContractAddress { get; set; }
        [Comment("Amount")]
        public string? LitigationAmount { get; set; }
        [Comment("Name of the arbitrator chairman")]
        public string? AreaName { get; set; }
        [Comment("Name of the arbitrator chairman")]
        public string? CityName { get; set; }
        [Comment("Category of the name")]
        public string? CategoryName { get; set; }
        [Comment("Description for Request Status")]
        public string? RequestStatus { get; set; }
        [Comment("Description for First Party")]
        public string? FirstParty { get; set; }
        [Comment("Description for Second Party")]
        public string? SecondParty { get; set; }
    }
}

