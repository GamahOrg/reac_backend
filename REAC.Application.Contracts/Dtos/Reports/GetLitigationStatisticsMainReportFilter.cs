using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Reports
{
    public class GetLitigationStatisticsMainReportFilter
    {
        public string? EmployeeId { get; set; }
        public int? LitigationRequestStageId { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public string? AreaIds { get; set; }
        public string? CitiesIds { get; set; }
        public string? LitigationRequestCategoryIds { get; set; }
        public decimal? FromFees { get; set; }
        public decimal? ToFees { get; set; }
    }
}
