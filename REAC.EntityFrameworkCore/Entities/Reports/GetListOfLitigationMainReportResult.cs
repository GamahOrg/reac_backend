using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Reports
{
    public class GetListOfLitigationMainReportResult
    {
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }
        [Comment("Date and time when the record was created")]
        public DateTime? CreatedOn { get; set; }
        [Comment("Description for Request Code")]
        public string? RequestCode { get; set; }
        [Comment("Address")]
        public string? ContractAddress { get; set; }
        //public decimal? Fees { get; set; }
        [Comment("Name of the area")]
        public string? AreaName { get; set; }
        [Comment("Name of the area")]
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

