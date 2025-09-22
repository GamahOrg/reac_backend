using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Delegator
{
    public class GetLitigationRequestWithoutDelegator
    {
        public Guid Id { get; set; }
        public string RequestCode { get; set; }
        public LitigationRequestParty? DelegationParty { get; set; }
    }
}
