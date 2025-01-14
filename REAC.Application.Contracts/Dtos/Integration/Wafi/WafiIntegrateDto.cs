using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.Wafi
{
    public class WafiIntegrateDto
    {
        public PartyDTO FirstPartyDto { get; set; }
        public PartyDTO SecondPartyDto { get; set; }
        public PartyDTO ThirdPartyDto { get; set; }
        public LitigationRequestOwnerType FirstPartyCapacity { get; set; }
        public string ContractAddress { get; set; }
    }
}
