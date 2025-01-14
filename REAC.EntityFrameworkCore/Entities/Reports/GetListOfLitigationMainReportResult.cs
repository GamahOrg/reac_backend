using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetListOfLitigationMainReportResult
    {
        public Guid LitigationRequestId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? RequestCode { get; set; }
        public string? ContractAddress { get; set; }
        public decimal? Fees { get; set; }
        public string? AreaName { get; set; }
        public string? CityName { get; set; }
        public string? CategoryName { get; set; }
        public string? RequestStatus { get; set; }
        public string? FirstParty { get; set; }
        public string? SecondParty { get; set; }
    }
}
