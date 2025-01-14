using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public class CitizenRequestShortDataDto : BaseEntity<Guid>
    {
        //توع الطلب
        public CitizenRequestType CitizenRequestTypeId { get; set; }
        public string CitizenName { get; set; }

        public string IdentityNumber { get; set; }
        public string CitizenRequestType { get; set; }

        public string? AreaName { get; set; }

        public string AcademicDegreeName { get; set; } = null!;
        public bool? IsVolunteerApproval { get; set; }
        public DateTime RequestOn { get; set; }

        public int? ExperienceYears { get; set; }

        public string? ArbitratorClassName { get; set; }
        public string StatusName { get; set; } = null!;
        public CitizenRequestStatus StatusId { get; set; }
    }
}

