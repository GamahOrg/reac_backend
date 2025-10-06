using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetListOfLitigationGeneralReportResult : GetListOfLitigationMainReportResult
    {
        [Comment("Category of the ids")]
        public string? LitigationRequestCategoryIds { get; set; }
    }
}
