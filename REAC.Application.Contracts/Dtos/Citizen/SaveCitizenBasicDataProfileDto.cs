using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class SaveCitizenBasicDataProfileDto : BaseEntity<Guid>
    {
        public string? Code { get; set; }

        public IdentityType IdentityTypeId { get; set; }

        public string IdentityNumber { get; set; } = null!;

        public string FullName { get; set; } = null!;
        public DateTime IdentityExpiryDate { get; set; }

        public Guid NationalityId { get; set; }
        public string NationalityName { get; set; }

        public DateTime BirthDate { get; set; }

        public string? NationalAddress { get; set; }

        public string ShortName { get; set; } = null!;

        public Gender GenderId { get; set; }
        public string GenderName { get; set; }


        public string? Employer { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public string? HomeTelephone { get; set; }

        public string? WorkTelephone { get; set; }

        public string? Fax { get; set; }

        public string Email { get; set; } = null!;

        public Guid AreaId { get; set; }

        public Guid CityId { get; set; }

        public string District { get; set; } = null!;

        public string Street { get; set; } = null!;

        public string? PostalCode { get; set; }

        public string? SubNumber { get; set; }

        public string BuildingNumber { get; set; } = null!;

        public string UnitNumber { get; set; } = null!;

        public bool IsLawyer { get; set; } = false;

        public string? LawyerLicenseNumber { get; set; }

        public DateTime? LawyerLicenseExpirationDate { get; set; }
        public Guid? IdentityFileId { get; set; }

        public Guid? LawLicenseFileId { get; set; }
        public Guid? SigntureFileId { get; set; }

    }
}
