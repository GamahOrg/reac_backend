using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenReformerDetailShortDto : BaseEntity<Guid>
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

        public int RequestsCount { get; set; }
        public ReformerStatus? ReformerStatusId { get; set; }

        public string? ReformerStatus { get; set; }

        public DateTime? ReformerStatusDate { get; set; }

        public ICollection<ReformerStopFreeUpDetailDecisionDto> ReformerStopFreeUpDetailDecisionDtos { get; set; } = new List<ReformerStopFreeUpDetailDecisionDto>();

    }

}
