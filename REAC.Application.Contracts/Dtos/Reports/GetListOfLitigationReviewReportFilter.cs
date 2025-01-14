using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Reports
{
    public class GetListOfLitigationReviewReportFilter: GetLitigationStatisticsMainReportFilter
    {
        public string? EmployeeNames { get; set; }
    }
}
