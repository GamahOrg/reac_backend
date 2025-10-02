using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;


public partial class Citizen : BaseEntity<Guid>
{
    [Comment("Identity number of the citizen")]
    public string IdentityNumber { get; set; } = null!;
    [Comment("Full name of the citizen")]
    public string FullName { get; set; } = null!;
    [Comment("Unique identifier of the nationality")]
    public Guid? NationalityId { get; set; }
    [Comment("Short name of the citizen")]
    public string? ShortName { get; set; } = null!;
    [Comment("Date of birth")]
    public DateTime? BirthDate { get; set; }
    [Comment("Gender identifier")]
    public Gender? GenderId { get; set; }
    [Comment("Expiry date of the identity document")]
    public DateTime? IdentityExpiryDate { get; set; }
    [Comment("Citizen code")]
    public string? Code { get; set; }
    [Comment("Type of identity document")]
    public IdentityType? IdentityTypeId { get; set; }
    [Comment("Unique identifier of the user")]
    public Guid? UserId { get; set; }
    [Comment("Address")]
    public string? NationalAddress { get; set; }
    [Comment("Unique identifier of the pass citizen")]
    public Guid? PassCitizenId { get; set; }
    [Comment("Unique identifier of the identity file")]
    public Guid? IdentityFileId { get; set; }
    [Comment("Unique identifier of the signature file")]
    public Guid? SigntureFileId { get; set; }
    [Comment("Employer information")]
    public string? Employer { get; set; } = null!;
    [Comment("Mobile phone number")]
    public string Mobile { get; set; } = null!;
    [Comment("Telephone number")]
    public string? HomeTelephone { get; set; }
    [Comment("Telephone number")]
    public string? WorkTelephone { get; set; }
    [Comment("Fax number")]
    public string? Fax { get; set; }
    [Comment("Email address")]
    public string? Email { get; set; } = null!;
    [Comment("Unique identifier of the area")]
    public Guid? AreaId { get; set; }
    [Comment("Unique identifier of the city")]
    public Guid? CityId { get; set; }
    [Comment("District")]
    public string? District { get; set; } = null!;
    [Comment("Street address")]
    public string? Street { get; set; } = null!;
    [Comment("Postal code")]
    public string? PostalCode { get; set; }
    [Comment("Sub number")]
    public string? SubNumber { get; set; }
    [Comment("Additional number")]
    public string? AdditionalNumber { get; set; }
    [Comment("Building number")]
    public string? BuildingNumber { get; set; } = null!;
    [Comment("Unit number")]
    public string? UnitNumber { get; set; } = null!;
    [Comment("Indicates whether the person is a lawyer")]
    public bool? IsLawyer { get; set; } = false;
    [Comment("Indicates whether the person is a reformer")]
    public bool? IsReformer { get; set; } = false;
    [Comment("Status identifier")]
    public ReformerStatus? ReformerStatusId { get; set; }
    [Comment("Indicates whether the person is an arbitrator")]
    public bool? IsArbitrator { get; set; } = false;
    [Comment("Status identifier")]
    public ArbitratorStatus? ArbitratorStatusId { get; set; }

    [Comment("Lawyer license number")]
    public string? LawyerLicenseNumber { get; set; }
    [Comment("Expiration date of the lawyer license")]
    public DateTime? LawyerLicenseExpirationDate { get; set; }
    [Comment("Unique identifier of the law license file")]
    public Guid? LawLicenseFileId { get; set; }
    [Comment("Indicates whether this is a draft account")]
    public bool? IsDraftAccount { get; set; }
    public virtual ICollection<LitigationRequestArbitratorsChairmanNominationListValue> LitigationRequestArbitratorsChairmanNominationListArbitratorValues { get; set; } = new List<LitigationRequestArbitratorsChairmanNominationListValue>();

    public virtual ICollection<LitigationRequestArbitratorRecusalRequest> LitigationRequestArbitratorRecusalRequests { get; set; } = new List<LitigationRequestArbitratorRecusalRequest>();
    public virtual ICollection<LitigationRequestArbitratorsChairmanNominationListValue> LitigationRequestArbitratorsChairmanNominationListChairmanValues { get; set; } = new List<LitigationRequestArbitratorsChairmanNominationListValue>();
    public virtual ICollection<LititgationRequestModificationRequest> LitigationRequestModificationRequests { get; set; } = new List<LititgationRequestModificationRequest>();
    public virtual ICollection<LitigationRequestNote> LitigationRequestNotes { get; set; } = new List<LitigationRequestNote>();
    public virtual ICollection<LitigationRequestCorrespondence> LitigationRequestCorrespondences { get; set; } = new List<LitigationRequestCorrespondence>();
    public virtual ICollection<LitigationRequestCorrespondenceReply> LitigationRequestCorrespondenceReply { get; set; } = new List<LitigationRequestCorrespondenceReply>();

    public virtual ICollection<Facility> Facilities { get; set; } = new List<Facility>();
    public virtual ICollection<Delegator> FromDelegators { get; set; } = new List<Delegator>();
    public virtual ICollection<ReformerStopFreeUpDecision> ReformerStopFreeUpDecisions { get; set; } = new List<ReformerStopFreeUpDecision>();
    public virtual ICollection<ArbitratorStopFreeUpDecision> ArbitratorStopFreeUpDecisions { get; set; } = new List<ArbitratorStopFreeUpDecision>();
    public virtual ICollection<Delegator> ToDelegators { get; set; } = new List<Delegator>();
    public virtual ICollection<CitizenRequest> CitizenRequests { get; set; } = new List<CitizenRequest>();
    public virtual ICollection<LitigationRequestArbitratorList> LitigationRequestArbitratorLists { get; set; } = new List<LitigationRequestArbitratorList>();
    public virtual ICollection<LitigationRequestArbitratorNominationList> LitigationRequestArbitratorNominationLists { get; set; } = new List<LitigationRequestArbitratorNominationList>();
    public virtual ICollection<LitigationRequestArbitratorsChairmanNominationList> LitigationRequestArbitratorsChairmanNominationLists { get; set; } = new List<LitigationRequestArbitratorsChairmanNominationList>();



    public virtual ICollection<CitizenBankAccount> CitizenBankAccounts { get; set; } = new List<CitizenBankAccount>();
    public virtual SysGender? Gender { get; set; }
    public virtual SysCity? City { get; set; }
    public virtual SysArea? Area { get; set; }
    public virtual SysReformerStatus? SysReformerStatus { get; set; }
    public virtual SysArbitratorStatus? SysArbitratorStatus { get; set; }

    public virtual SysNationality? Nationality { get; set; }
    public virtual SysFileManager? IdentityFile { get; set; }
    public virtual SysFileManager? LawLicenseFile { get; set; }
    public virtual SysFileManager? SigntureFile { get; set; }
    public virtual ICollection<LitigationRequest> ArbitratorLitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<LitigationRequestArbitratorAssigningRequest> LitigationRequestArbitratorAssigningRequests { get; set; } = new List<LitigationRequestArbitratorAssigningRequest>();

    public virtual ICollection<LitigationRequest> ReformerLitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual SysIdentityType? IdentityType { get; set; }
    public virtual ICollection<LitigationRequestContract> LitigationRequestContractFirstPartyCitizens { get; set; } = new List<LitigationRequestContract>();
    public virtual ICollection<LitigationRequestContract> LitigationRequestContractSecondPartyCitizens { get; set; } = new List<LitigationRequestContract>();
    public virtual ICollection<LitigationRequestReformerAssigningRequest> LitigationRequestReformerAssigningRequests { get; set; } = new List<LitigationRequestReformerAssigningRequest>();
    public virtual ICollection<LitigationRequest> DelegatorLitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<SysRegistrationRequest> SysRegistrationRequestCitizens { get; set; } = new List<SysRegistrationRequest>();
    public virtual ICollection<SysRegistrationRequest> SysRegistrationRequestSourceCitizens { get; set; } = new List<SysRegistrationRequest>();
    // Foreign key for LitigationCompanyRepresentative
    [Comment("Unique identifier of the litigation company representative")]
    public Guid? LitigationCompanyRepresentativeId { get; set; }
    public virtual LitigationCompanyRepresentative LitigationCompanyRepresentative { get; set; }

}





