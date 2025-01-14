using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Litigation;

public partial class LitigationRequestContractShortDto : BaseEntity<Guid>
{
    public LitigationRequestContractType LitigationRequestContractTypeId { get; set; }

    public string ContractKey { get; set; } = null!; // contract number

    public string? VersionNumber { get; set; }

    public string? AssociationName { get; set; }

    public string FirstPartyCapacity { get; set; } = null!;

    public string SecondPartyCapacity { get; set; } = null!;

    public virtual CitizenShortDto? FirstPartyCitizen { get; set; }

    public virtual FacilityShortDto? FirstPartyFacility { get; set; }

    public virtual CitizenShortDto? SecondPartyCitizen { get; set; }

    public virtual FacilityShortDto? SecondPartyFacility { get; set; }

    public virtual SysFileManagerCreateUpdateDto? ContractFile { get; set; }

    public virtual SysFileManagerCreateUpdateDto? AssociationRegistrationCertificateFile { get; set; }

    public virtual SysFileManagerCreateUpdateDto? FoundingLetterFile { get; set; }

    public virtual SysFileManagerCreateUpdateDto? InternalRegulationsFile { get; set; }

    public virtual SysLitigationRequestContractTypeGetDto LitigationRequestContractType { get; set; } = null!;


}
