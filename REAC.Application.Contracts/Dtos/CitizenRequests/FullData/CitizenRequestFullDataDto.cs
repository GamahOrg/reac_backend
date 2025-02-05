using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.System.AreaDto;
using REAC.Application.Contracts.Dtos.System.CitizenRequestStatusDtos;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public class CitizenRequestFullDataDto : BaseEntity<Guid>
    {
        //توع الطلب

        public Guid CitizenId { get; set; }
        public Guid? AreaId { get; set; }
        public bool? IsVolunteerApproval { get; set; }

        public string AcademicDegreeName { get; set; } = null!;
        public int? ExperienceYears { get; set; }
        public Guid? ArbitratorClassId { get; set; }


        public CitizenRequestStatus StatusId { get; set; }

        public string? About { get; set; }

        public string? Brief { get; set; }
        public string? Fees { get; set; }

        public bool? IsFeesPaid { get; set; }
        public virtual SysArbitratorClassDto? SysArbitratorClass { get; set; }

        public virtual CitizenShortDto Citizen { get; set; } = null!;
        public virtual SysAreaDto? SysArea { get; set; }

        public SysCitizenRequestStatusDto Status { get; set; } = null!;

        public virtual ICollection<CitizenRequestFreelanceWorkDocumentFileFullDataDto> CitizenRequestFreelanceWorkDocumentFiles { get; set; } = new List<CitizenRequestFreelanceWorkDocumentFileFullDataDto>();
        public virtual ICollection<CitizenRequestAcademicDegreeFileFullDataDto> CitizenRequestAcademicDegreeFiles { get; set; } = new List<CitizenRequestAcademicDegreeFileFullDataDto>();
        public virtual ICollection<CitizenRequestExperienceCertificateFullDataDto> CitizenRequestExperienceCertificates { get; set; } = new List<CitizenRequestExperienceCertificateFullDataDto>();
        public virtual ICollection<CitizenRequestNoteCreateUpdateDto> CitizenRequestNotes { get; set; } = new List<CitizenRequestNoteCreateUpdateDto>();
    }
}
