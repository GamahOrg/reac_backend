using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenShortDto : BaseEntity<Guid>
    {
        public string IdentityNumber { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public string? HomeTelephone { get; set; }

        public string? WorkTelephone { get; set; }

        public string? Fax { get; set; }
        public CitizenNationalityDto Nationality { get; set; } = null!;

        public string Email { get; set; } = null!;
        public string SigntureFile { get; set; } = null!;

    }

}
