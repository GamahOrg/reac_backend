using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Reports
{
    public class GetListOfLitigationReformerReportFilter : GetLitigationStatisticsMainReportFilter
    {
        public string? ReformerNames { get; set; }
    }
}
