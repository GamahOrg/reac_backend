using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Application.Contracts.Dtos.Integration.Rent;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.Properties
{
    public class PropertiesIntegrateDto
    {
        public string? AssociationName { get; set; } // اسم الجمعية
        public string? AssociationCode { get; set; } // كود الجمعية
        public PartyDTO FirstPartyDto { get; set; }
        public PartyDTO SecondPartyDto { get; set; }
        public LitigationRequestOwnerType FirstPartyCapacity { get; set; }
        public string ContractAddress { get; set; }
        public Guid Id { get; set; }
    }
}
