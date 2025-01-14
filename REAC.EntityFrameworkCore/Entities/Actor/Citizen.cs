using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor;


public partial class Citizen : BaseEntity<Guid>
{
    public string IdentityNumber { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public Guid? NationalityId { get; set; }
    public string? ShortName { get; set; } = null!;
    public DateTime? BirthDate { get; set; }
    public Gender? GenderId { get; set; }
    public DateTime? IdentityExpiryDate { get; set; }
    public string? Code { get; set; }
    public IdentityType? IdentityTypeId { get; set; }
    public Guid? UserId { get; set; }
    public string? NationalAddress { get; set; }
    public Guid? PassCitizenId { get; set; }
    public Guid? IdentityFileId { get; set; }
    public Guid? SigntureFileId { get; set; }
    public string? Employer { get; set; } = null!;
    public string Mobile { get; set; } = null!;
    public string? HomeTelephone { get; set; }
    public string? WorkTelephone { get; set; }
    public string? Fax { get; set; }
    public string? Email { get; set; } = null!;
    public Guid? AreaId { get; set; }
    public Guid? CityId { get; set; }
    public string? District { get; set; } = null!;
    public string? Street { get; set; } = null!;
    public string? PostalCode { get; set; }
    public string? SubNumber { get; set; }
    public string? BuildingNumber { get; set; } = null!;
    public string? UnitNumber { get; set; } = null!;
    public bool? IsLawyer { get; set; } = false;
    public bool? IsReformer { get; set; } = false;
    public ReformerStatus? ReformerStatusId { get; set; }
    public bool? IsArbitrator { get; set; } = false;
    public ArbitratorStatus? ArbitratorStatusId { get; set; }

    public string? LawyerLicenseNumber { get; set; }
    public DateTime? LawyerLicenseExpirationDate { get; set; }
    public Guid? LawLicenseFileId { get; set; }
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
    public Guid? LitigationCompanyRepresentativeId { get; set; }
    public virtual LitigationCompanyRepresentative LitigationCompanyRepresentative { get; set; }

}



