using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Arbitration;

/// <summary>Used by citizen (party) to submit a correction request</summary>
public class LitigationRequestArbitrationCorrectionRequestCreateDto
{
    public Guid LitigationRequestId { get; set; }

    /// <summary>وصف التصحيح المطلوب - max 1000 chars</summary>
    public string CorrectionDescription { get; set; } = null!;

    /// <summary>Optional file attachments (list of SysFileManager IDs)</summary>
    public List<Guid> FileIds { get; set; } = new();
}

/// <summary>Used by arbitrator to accept or reject a correction request</summary>
public class LitigationRequestArbitrationCorrectionRequestResponseDto
{
    public Guid Id { get; set; }

    public Guid LitigationRequestId { get; set; }

    /// <summary>Arbitrator decision: Approve or Reject</summary>
    public ApproveRejectStatus Response { get; set; }

    /// <summary>Required when Response = Reject - max 300 chars</summary>
    public string? RejectReason { get; set; }

    /// <summary>Required when Response = Approve - max 2500 chars</summary>
    public string? CorrectionDecisionText { get; set; }
}

/// <summary>DTO returned when fetching a correction request record</summary>
public class LitigationRequestArbitrationCorrectionRequestGetDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }
    public Guid RequestCitizenId { get; set; }
    public string? RequestCitizenName { get; set; }
    public string CorrectionDescription { get; set; } = null!;
    public DateTime RequestDate { get; set; }
    public LitigationRequestParty RequestParty { get; set; }
    public ApproveRejectStatus? ApproveRejectStatus { get; set; }
    public string? RejectReason { get; set; }
    public string? CorrectionDecisionText { get; set; }
    public DateTime? ResponseOn { get; set; }
    public List<LitigationRequestArbitrationCorrectionRequestFileGetDto> Files { get; set; } = new();

    // رقم الدعوى
    public string? RequestCode { get; set; }
    // تاريخ إصدار الحكم
    public DateTime? JudgmentDecisionDate { get; set; }
    // حالة الدعوى
    public string? Status { get; set; }

    // هيئة التحكيم - المحكم العقاري
    public string? ArbitratorName { get; set; }
    public string? ArbitratorIdentityNumber { get; set; }
    public string? ArbitratorMobile { get; set; }
    public string? ArbitratorCity { get; set; }
    public string? ArbitratorMembershipCode { get; set; }

    // الطرف الأول
    public string? FirstPartyName { get; set; }
    public string? FirstPartyIdentityNumber { get; set; }
    public string? FirstPartyRole { get; set; }
    public string? FirstPartyAddress { get; set; }
    public string? FirstPartyMobile { get; set; }
    public string? FirstPartyEmail { get; set; }

    // الطرف الثاني
    public string? SecondPartyName { get; set; }
    public string? SecondPartyIdentityNumber { get; set; }
    public string? SecondPartyRole { get; set; }
    public string? SecondPartyAddress { get; set; }
    public string? SecondPartyMobile { get; set; }
    public string? SecondPartyEmail { get; set; }
}

public class LitigationRequestArbitrationCorrectionRequestFileGetDto : BaseEntity<Guid>
{
    public Guid CorrectionRequestFileId { get; set; }
    public string? FilePath { get; set; }
    public string? FileType { get; set; }
}
