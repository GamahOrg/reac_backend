using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Arbitration
{
    public class LitigationRequestArbitrationAssignedRequestFilter
    {
        public string? LitigationRequestCode { get; set; }
        public int? LitigationRequestType { get; set; }
        public int? RequestStatus { get; set; }
        public string? PlaintiffName { get; set; }
        public string? DefendantName { get; set; }
    }
}
