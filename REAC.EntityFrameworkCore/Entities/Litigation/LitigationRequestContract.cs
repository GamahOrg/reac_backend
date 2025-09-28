using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestContract : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request contract type")]
    public LitigationRequestContractType LitigationRequestContractTypeId { get; set; }

    [Comment("Contract key identifier or number")]
    public string ContractKey { get; set; } = null!; // contract number

    [Comment("Number of the version")]
    public string? VersionNumber { get; set; }

    [Comment("Name of the association")]
    public string? AssociationName { get; set; }

    [Comment("Unique identifier of the contract file")]
    public Guid? ContractFileId { get; set; }

    [Comment("Unique identifier of the association registration certificate file")]
    public Guid? AssociationRegistrationCertificateFileId { get; set; }

    [Comment("Unique identifier of the founding letter file")]
    public Guid? FoundingLetterFileId { get; set; }

    [Comment("Unique identifier of the internal regulations file")]
    public Guid? InternalRegulationsFileId { get; set; }
    [Comment("Unique identifier of the incorporation contract file")]
    public Guid? IncorporationContractFileId { get; set; }

    [Comment("Unique identifier of the first party citizen")]
    public Guid? FirstPartyCitizenId { get; set; }

    [Comment("Unique identifier of the first party facility")]
    public Guid? FirstPartyFacilityId { get; set; }

    [Comment("Role or capacity of the first party in the contract")]
    public string FirstPartyCapacity { get; set; } = null!;

    [Comment("Unique identifier of the second party citizen")]
    public Guid? SecondPartyCitizenId { get; set; }

    [Comment("Unique identifier of the second party facility")]
    public Guid? SecondPartyFacilityId { get; set; }

    [Comment("Name of the association")]
    public string? SecondPartyName { get; set; }

    [Comment("Role or capacity of the second party in the contract")]
    public string SecondPartyCapacity { get; set; } = null!;

    [Comment("Number of the version")]
    public string? SecondPartyIdentityNumber { get; set; }

    [Comment("Email address")]
    public string? SecondPartyEmail { get; set; }

    [Comment("Mobile phone number")]
    public string? SecondPartyMobile { get; set; }


    [Comment("Unique identifier of the second party registration request")]
    public Guid? SecondPartyRegistrationRequestId { get; set; }

    public virtual Citizen? FirstPartyCitizen { get; set; }

    public virtual Facility? FirstPartyFacility { get; set; }

    public virtual SysFileManager? ContractFile { get; set; }

    public virtual SysFileManager? AssociationRegistrationCertificateFile { get; set; }

    public virtual SysFileManager? FoundingLetterFile { get; set; }

    public virtual SysFileManager? InternalRegulationsFile { get; set; }
    public virtual SysFileManager? IncorporationContractFile { get; set; }

    public virtual SysLitigationRequestContractType LitigationRequestContractType { get; set; } = null!;

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();

    public virtual SysRegistrationRequest? SecondPartyRegistrationRequest { get; set; }

    public virtual Citizen? SecondPartyCitizen { get; set; }

    public virtual Facility? SecondPartyFacility { get; set; }
}

