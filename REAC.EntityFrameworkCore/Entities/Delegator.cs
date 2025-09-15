using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System.ComponentModel.DataAnnotations.Schema;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class Delegator : BaseEntity<Guid>
{
    //رقم الوكالة
    public string? DelegateNumber { get; set; }
    //إرفاق الوكالة
    public Guid? DelegationFileId { get; set; }
    //إرفاق السجل التجاري للمنشأه
    public Guid? CommercialRegisterationFileId { get; set; }
    //إرفاق عقد التأسيس
    public Guid? EstablishmentFileId { get; set; }
    //إرفاق مستند التمثيل
    public Guid? RepresentationFileId { get; set; }
    //إرفاق قرار
    public Guid? DecisionFileId { get; set; }
    //إرفاق مستند اثبات الولايه
    public Guid? CustodyFileId { get; set; }
    public DelegationRelationshipType DelegationRelationshipTypeId { get; set; }

    public Guid? CitizenId { get; set; }

    public Guid? FacilityId { get; set; }

    public Guid? DelegatorCitizenId { get; set; }
    

    public string? LawyerLicenseNumber { get; set; }

    public DateTime? LawyerLicenseExpirationDate { get; set; }
    public Guid? LawLicenseFileId { get; set; }

    public bool IsActive { get; set; }
    public virtual SysDelegationRelationshipType SysDelegationRelationshipType { get; set; } = null!;

    public virtual Citizen? Citizen { get; set; }

    public virtual Citizen DelegatorCitizen { get; set; } = null!;

    public virtual SysFileManager? DelegationFile { get; set; }
    [ForeignKey(nameof(CommercialRegisterationFileId))]
    public virtual SysFileManager? CommercialRegisterationFile { get; set; }

    [ForeignKey(nameof(EstablishmentFileId))]
    public virtual SysFileManager? EstablishmentFile { get; set; }
    
    [ForeignKey(nameof(RepresentationFileId))]
    public virtual SysFileManager? RepresentationFile { get; set; }

    [ForeignKey(nameof(DecisionFileId))]
    public virtual SysFileManager? DecisionFile { get; set; }

    [ForeignKey(nameof(CustodyFileId))]
    public virtual SysFileManager? CustodyFile { get; set; }

    public virtual Facility? Facility { get; set; }

    [InverseProperty(nameof(LitigationRequest.Delegator))]
    public virtual ICollection<LitigationRequest> DelegatorLitigationRequests { get; set; } = new List<LitigationRequest>();

    [InverseProperty(nameof(DelegationRequest.Delegator))]
    public virtual ICollection<DelegationRequest> DelegatorDelegationRequests { get; set; } = new List<DelegationRequest>();



}
