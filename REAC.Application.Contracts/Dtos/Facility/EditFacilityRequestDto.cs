using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Facility
{
    public class EditFacilityRequestDto : BaseEntity<Guid>
    {

        public string? Fax { get; set; }

        public string? Email { get; set; }

        public string City { get; set; } = null!;

        public string District { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string? PostalCode { get; set; }

        public string? SubNumber { get; set; }

        public string BuildingNumber { get; set; } = null!;
        public string UnitNumber { get; set; } = null!;

    }
}
