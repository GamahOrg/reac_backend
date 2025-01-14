using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenArbitratorDetailShortDto : BaseEntity<Guid>
    {
        public string IdentityNumber { get; set; } = null!;
        public string? AcademicDegreeName { get; set; } = null!;
        public bool? IsVolunteerApproval { get; set; }
        public string FullName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public string? HomeTelephone { get; set; }

        public string? WorkTelephone { get; set; }

        public string? Fax { get; set; }

        public string? Email { get; set; } = null!;

        public string? AreaName { get; set; }
        public int? ExperienceYears { get; set; }
        public string? ArbitratorClass { get; set; }
        public string? Brief { get; set; }

        public int RequestsCount { get; set; }
        public ArbitratorStatus? ArbitratorStatusId { get; set; }

        public string? ArbitratorStatus { get; set; }

        public DateTime? ArbitratorStatusDate { get; set; }

        public List<ArbitratorStopFreeUpDetailDecisionDto> ArbitratorStopFreeUpDetailDecisionDtos { get; set; } = new List<ArbitratorStopFreeUpDetailDecisionDto>();
    }
}
