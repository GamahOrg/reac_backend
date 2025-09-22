using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Delegator
{
    public class GetDelegationRequest
    {
        public Guid Id { get; set; }
        public string? LitigationRequestCode { get; set; }
        public LitigationRequestParty DelegationParty { get; set; }

        public LitigationRequestOwnerType LitigationRequestOwnerTypeId { get; set; }

        public DateTime RequestedOn { get; set; }

        public bool? IsApproved { get; set; }
        public bool? IsActive { get; set; }

        public DateTime? ApprovedOn { get; set; }
        public Guid? ModifiedBy { get; set; }
        public string? Reason { get; set; }

    }
}
