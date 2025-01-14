using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    public class WAFIRequestDto
    {
        public PartyDTO RealEstateDeveloper { get; set; }
        public PartyDTO EngineeringConsultant { get; set; }
        public PartyDTO Accountant { get; set; }
    }
}
