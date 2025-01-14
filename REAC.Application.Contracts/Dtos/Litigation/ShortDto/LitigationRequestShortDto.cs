using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestOwnerTypeDtos;
using REAC.Application.Contracts.Dtos.System.LitigationRequestStatusDtos;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestShortDto : BaseEntity<Guid>
{
    public string? RequestCode { get; set; }
    public string? CityName { get; set; }
    public string? AreaName { get; set; }
    public RequestStatus ExternalStatusId { get; set; }
    public string? ContractAddress { get; set; } // العنوان الوطني

    public ApproveRejectStatus? MintuesOfMeetingApprovalPlaintiffStatus { get; set; }

    public ApproveRejectStatus? MintuesOfMeetingApprovalDefendantStatus { get; set; }
    public DateTime? MintuesOfMeetingApprovalPlaintiffStatusOn { get; set; }
    public DateTime? MintuesOfMeetingApprovalDefendantStatusOn { get; set; }
   
    public ApproveRejectStatus? PlaintiffAgreementRequestResponse { get; set; }
    public DateTime? PlaintiffAgreementRequestResponseOn { get; set; }
    public ApproveRejectStatus? DefendantAgreementRequestResponse { get; set; }
    public DateTime? DefendantAgreementRequestResponseOn { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool? IsFinancialLitigation { get; set; }
    public bool? IsExemptionReduction { get; set; }
    public virtual CitizenShortDto ApplicantCitizen { get; set; } = null!;
    public virtual CitizenShortDto ArbitratorCitizen { get; set; } = null!;
    public virtual CitizenShortDto ReformerCitizen { get; set; } = null!;
    public virtual LitigationRequestContractShortDto Contract { get; set; } = null!;
    public virtual List<LitigationRequestRequestCreateUpdateDto> LitigationRequestRequests { get; set; } = null!;

    public virtual SysLitigationRequestStatusDto ExternalStatus { get; set; } = null!;
    public virtual SysLitigationRequestStatusDto InternalStatus { get; set; } = null!;

    public virtual SysLitigationRequestAmountCreateUpdateDto LitigationRequestAmount { get; set; } = null!;

    public virtual SysLitigationRequestCategoryDto? LitigationRequestCategory { get; set; }

    public virtual SysExemptionReductionCategoryDto? SysExemptionReductionCategory { get; set; }

    public virtual SysLitigationRequestOwnerTypeDto LitigationRequestOwnerType { get; set; } = null!;

}
