using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.CitizenRequests.CreateUpdate;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public class CitizenRequestCreateUpdateDto : BaseEntity<Guid>
    {
        //توع الطلب
        public CitizenRequestType CitizenRequestTypeId { get; set; }
        public Guid? AreaId { get; set; }
        public bool IsVolunteerApproval { get; set; }
        public bool IsRegisteredReconciliationCenter { get; set; }
        public bool HaveArbitrationxperience { get; set; }
        public string? ExperiencedInArbitrationFields { get; set; }
        public bool HaveHostingExperience { get; set; }
        public string? ExperiencedInHousingstatuses { get; set; }
        public string? About { get; set; }
        public bool IspassedRealEstateProgram { get; set; }
        public bool IsSend { get; set; }
        public bool? IsFeesPaid { get; set; }
        public string AcademicDegreeName { get; set; } = null!;
        public int? ExperienceYears { get; set; }
        public Guid? ArbitratorClassId { get; set; }

        public string? Brief { get; set; }
        public virtual ICollection<CitizenRequestFreelanceWorkDocumentFileCreateUpdateDto> CitizenRequestFreelanceWorkDocumentFiles { get; set; } = new List<CitizenRequestFreelanceWorkDocumentFileCreateUpdateDto>();
        public virtual ICollection<CitizenRequestAcademicDegreeFileCreateUpdateDto> CitizenRequestAcademicDegreeFiles { get; set; } = new List<CitizenRequestAcademicDegreeFileCreateUpdateDto>();
        public virtual ICollection<CitizenRequestExperienceCertificateCreateUpdateDto> CitizenRequestExperienceCertificates { get; set; } = new List<CitizenRequestExperienceCertificateCreateUpdateDto>();
        public virtual ICollection<CitizenRequestReconciliationCenterDegreeFileCreateUpdateDto> CitizenRequestReconciliationCenterDegreeFiles { get; set; } = new List<CitizenRequestReconciliationCenterDegreeFileCreateUpdateDto>();
        public virtual ICollection<CitizenRequestCitizenRequestProgramRealEstateInstituteFileCreateUpdateDto> CitizenRequestProgramRealEstateInstituteFiles { get; set; } = new List<CitizenRequestCitizenRequestProgramRealEstateInstituteFileCreateUpdateDto>();
        public virtual ICollection<CitizenRequestCVFileCreateUpdateDto> CitizenRequestCVFiles { get; set; } = new List<CitizenRequestCVFileCreateUpdateDto>();
    }
}
