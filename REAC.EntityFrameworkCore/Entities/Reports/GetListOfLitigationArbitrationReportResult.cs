using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetListOfLitigationArbitrationReportResult : GetListOfLitigationMainReportResult
    {
        public string? ArbitrationName { get; set; }
    }
}
