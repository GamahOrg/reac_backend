using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Facility
{
    public class GetAllFacilitiesResponseDto : BaseEntity<Guid>
    {
        public string ArabicFacilityName { get; set; } = null!;

        public string EnglishFacilityName { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Facility700Number { get; set; } = null!;
        public string? CommercialRegisterNumber { get; set; }
        public string UnitNumber { get; set; } = null!;

    }
}
