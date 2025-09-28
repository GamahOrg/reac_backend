using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor;

public partial class CitizenRequest : BaseEntity<Guid>
{
    //توع الطلب
    [Comment("Unique identifier of the citizen request type")]
    public CitizenRequestType? CitizenRequestTypeId { get; set; }

    [Comment("Unique identifier of the citizen")]
    public Guid CitizenId { get; set; }


    // Shared
    [Comment("Name of the academic degree")]
    public string AcademicDegreeName { get; set; } = null!;
    [Comment("Indicates whether volunteer approval is granted")]
    public bool? IsVolunteerApproval { get; set; }
    [Comment("Date when the request was submitted")]
    public DateTime RequestOn { get; set; }


    // محكم
    [Comment("Unique identifier of the area")]
    public Guid? AreaId { get; set; }
    [Comment("Number of years of experience")]
    public int? ExperienceYears { get; set; }
    [Comment("Unique identifier of the arbitrator class")]
    public Guid? ArbitratorClassId { get; set; }
    [Comment("Brief description or summary")]
    public string? Brief { get; set; }

    //مصلح
    [Comment("Indicates whether registered with reconciliation center")]
    public bool? IsRegisteredReconciliationCenter { get; set; }
    [Comment("Indicates whether passed real estate program")]
    public bool? IspassedRealEstateProgram { get; set; }
    [Comment("Indicates whether has arbitration experience")]
    public bool? HaveArbitrationxperience { get; set; }
    [Comment("Indicates whether has hosting experience")]
    public bool? HaveHostingExperience { get; set; }
    [Comment("Fields of experience in arbitration")]
    public string? ExperiencedInArbitrationFields { get; set; }
    [Comment("Housing experience statuses")]
    public string? ExperiencedInHousingstatuses { get; set; }
    [Comment("Additional information about the applicant")]
    public string? About { get; set; }

    [Comment("Years of experience in freelance arbitration")]
    public int? ExperiencedInFreelanceArbitration { get; set; }
    [Comment("Indicates whether registered with bachelor Sharia degree")]
    public bool? IsregisteredWithBachelorShareaaDegree { get; set; }
    [Comment("Indicates whether has experience in freelance arbitration")]
    public bool? HaveExperiencedInFreelanceArbitration { get; set; }

    [Comment("Indicates whether the citizen request fees have been paid")]
    public bool? IsFeesPaid { get; set; }
    [Comment("Status identifier")]
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



