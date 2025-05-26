using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Reports
{
    public class GenericReportsFilter
    {
        public string? RequestCode { get; set; }
        public List<int>? LitigationRequestStatusIds { get; set; }
        public List<string>? EmployeeIds { get; set; }
        public List<string>? ReformerIds { get; set; }
        public List<string>? ArbitratorIds { get; set; }
        public List<string>? ArbitratorChairmanIds { get; set; }
        public int? LitigationRequestStageId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public List<string>? AreaIds { get; set; }
        public List<string>? CitiesIds { get; set; }

        public List<int>? LitigationRequestCategoryIds { get; set; }
        public string? LitigationRequestAmountId { get; set; }

        public int? Skip { get; set; }
        public int? Take { get; set; }
        public bool? IsAdmin { get; set; }
        public string? SortDirection { get; set; }
    }
}
