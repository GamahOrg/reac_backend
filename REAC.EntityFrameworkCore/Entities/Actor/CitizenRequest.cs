using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequest : BaseEntity<Guid>
{
    //توع الطلب
    public CitizenRequestType CitizenRequestTypeId { get; set; }

    public Guid CitizenId { get; set; }


    // Shared
    public string AcademicDegreeName { get; set; } = null!;
    public bool IsVolunteerApproval { get; set; }
    public DateTime RequestOn { get; set; }


    // محكم
    public Guid? AreaId { get; set; }
    public int? ExperienceYears { get; set; }
    public Guid? ArbitratorClassId { get; set; }
    public string? Brief { get; set; }

    //مصلح
    public bool IsRegisteredReconciliationCenter { get; set; }
    public bool IspassedRealEstateProgram { get; set; }
    public bool HaveArbitrationxperience { get; set; }
    public bool HaveHostingExperience { get; set; }
    public string? ExperiencedInArbitrationFields { get; set; }
    public string? ExperiencedInHousingstatuses { get; set; }
    public string? About { get; set; }


    public bool? IsFeesPaid { get; set; }
    public CitizenRequestStatus StatusId { get; set; }
    public SysCitizenRequestStatus Status { get; set; } = null!;

    public virtual SysCitizenRequestType SysCitizenRequestType { get; set; } = null!;
    public virtual SysArbitratorClass? SysArbitratorClass { get; set; }

    public virtual Citizen Citizen { get; set; } = null!;
    public virtual SysArea? SysArea { get; set; }

    public virtual ICollection<CitizenRequestAcademicDegreeFile> CitizenRequestAcademicDegreeFiles { get; set; } = new List<CitizenRequestAcademicDegreeFile>();
    public virtual ICollection<CitizenRequestFreelanceWorkDocumentFile> CitizenRequestFreelanceWorkDocumentFiles { get; set; } = new List<CitizenRequestFreelanceWorkDocumentFile>();
    public virtual ICollection<CitizenRequestExperienceCertificate> CitizenRequestExperienceCertificates { get; set; } = new List<CitizenRequestExperienceCertificate>();
    public virtual ICollection<CitizenRequestReconciliationCenterDegreeFile> CitizenRequestReconciliationCenterDegreeFiles { get; set; } = new List<CitizenRequestReconciliationCenterDegreeFile>();
    public virtual ICollection<CitizenRequestProgramRealEstateInstituteFile> CitizenRequestProgramRealEstateInstituteFiles { get; set; } = new List<CitizenRequestProgramRealEstateInstituteFile>();
    public virtual ICollection<CitizenRequestCVFile> CitizenRequestCVFiles { get; set; } = new List<CitizenRequestCVFile>();
    public virtual ICollection<CitizenRequestNote> CitizenRequestNotes { get; set; } = new List<CitizenRequestNote>();
}

