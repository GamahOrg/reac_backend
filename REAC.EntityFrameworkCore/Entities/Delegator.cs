using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class Delegator : BaseEntity<Guid>
{
    //رقم الوكالة
    [Comment("Number of the delegate")]
    public string? DelegateNumber { get; set; }
    //إرفاق الوكالة
    [Comment("Unique identifier of the delegation file")]
    public Guid? DelegationFileId { get; set; }
    //إرفاق السجل التجاري للمنشأه
    [Comment("Unique identifier of the commercial registration file")]
    public Guid? CommercialRegisterationFileId { get; set; }
    //إرفاق عقد التأسيس
    [Comment("Unique identifier of the establishment contract file")]
    public Guid? EstablishmentFileId { get; set; }
    //إرفاق مستند التمثيل
    [Comment("Unique identifier of the representation document file")]
    public Guid? RepresentationFileId { get; set; }
    //إرفاق قرار
    [Comment("Unique identifier of the decision file")]
    public Guid? DecisionFileId { get; set; }
    //إرفاق مستند اثبات الولايه
    [Comment("Unique identifier of the custody proof file")]
    public Guid? CustodyFileId { get; set; }
    [Comment("Type of delegation relationship")]
    public DelegationRelationshipType DelegationRelationshipTypeId { get; set; }

    [Comment("Unique identifier of the delegating citizen")]
    public Guid? CitizenId { get; set; }

    [Comment("Unique identifier of the facility being delegated")]
    public Guid? FacilityId { get; set; }

    [Comment("Unique identifier of the delegator citizen")]
    public Guid? DelegatorCitizenId { get; set; }
    

    [Comment("Lawyer license number")]
    public string? LawyerLicenseNumber { get; set; }

    [Comment("Expiration date of the lawyer license")]
    public DateTime? LawyerLicenseExpirationDate { get; set; }
    [Comment("Unique identifier of the law license file")]
    public Guid? LawLicenseFileId { get; set; }

    [Comment("Indicates whether the record is active")]
    public bool IsActive { get; set; }

    public string? IndividualCapacity { get; set; }
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


