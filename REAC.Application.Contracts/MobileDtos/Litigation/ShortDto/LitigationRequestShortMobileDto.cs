using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestOwnerTypeDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestStatusDtos;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.MobileDtos.Litigation.ShortDto;

public class LitigationRequestShortMobileDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public string? CityName { get; set; }
    public string? AreaName { get; set; }
    public string? ContractAddress { get; set; } // العنوان الوطني

    public DateTime? MintuesOfMeetingApprovalPlaintiffStatusOn { get; set; }
    public DateTime? MintuesOfMeetingApprovalDefendantStatusOn { get; set; }

    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsFinancialLitigation { get; set; }
    public bool? IsExemptionReduction { get; set; }
    public virtual CitizenShortDto ApplicantCitizen { get; set; } = null!;
    public virtual LitigationRequestContractShortDto Contract { get; set; } = null!;
    public virtual ICollection<LitigationRequestGroundShortMobileDto> LitigationRequestGrounds { get; set; } = null!;

    public virtual ICollection<LitigationRequestRequestShortMobileDto> LitigationRequestRequests { get; set; } = null!;

    public virtual SysLitigationRequestStatusDto ExternalStatus { get; set; } = null!;
    public virtual SysLitigationRequestStatusDto InternalStatus { get; set; } = null!;

    public virtual SysLitigationRequestAmountCreateUpdateDto LitigationRequestAmount { get; set; } = null!;

    public virtual SysLitigationRequestCategoryDto? LitigationRequestCategory { get; set; }

    public virtual SysExemptionReductionCategoryDto? SysExemptionReductionCategory { get; set; }

    public virtual SysLitigationRequestOwnerTypeDto LitigationRequestOwnerType { get; set; } = null!;

}
