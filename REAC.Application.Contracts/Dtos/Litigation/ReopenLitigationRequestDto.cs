using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Litigation;

/// <summary>Input DTO for reopening a closed litigation request</summary>
public class ReopenLitigationRequestDto
{
    public Guid LitigationRequestId { get; set; }

    /// <summary>سبب إعادة الفتح - حقل إلزامي - max 500 chars</summary>
    public string ReopenReason { get; set; } = null!;
}

/// <summary>DTO returned after fetching reopen details for a litigation request</summary>
public class LitigationRequestReopenInfoDto
{
    /// <summary>رقم الدعوى</summary>
    public Guid LitigationRequestId { get; set; }

    /// <summary>رقم الدعوى</summary>
    public string? RequestCode { get; set; }

    /// <summary>نوع الدعوى (صلح / تحكيم / صلح وتحكيم)</summary>
    public string? RequestTypeName { get; set; }

    /// <summary>حالة السداد الاتعاب الإدارية</summary>
    public string? AdministrativeFeesPaymentStatus { get; set; }

    /// <summary>حالة السداد لرسوم الصلح</summary>
    public string? ReconciliationFeesPaymentStatus { get; set; }

    /// <summary>حالة السداد لرسوم التحكيم</summary>
    public string? ArbitrationFeesPaymentStatus { get; set; }

    /// <summary>تاريخ تقديم الدعوى</summary>
    public DateTime? SubmissionDate { get; set; }

    /// <summary>تاريخ الغاء الدعوى</summary>
    public DateTime? CancellationDate { get; set; }

    /// <summary>الحالة قبل الإلغاء</summary>
    public string? StatusBeforeCancellation { get; set; }

    /// <summary>الحالة بعد إعادة الفتح (يتم تحديدها تلقائيًا)</summary>
    public string? StatusAfterReopen { get; set; }

    /// <summary>تاريخ إعادة الفتح</summary>
    public DateTime? ReopenDate { get; set; }

    /// <summary>سبب إعادة الفتح</summary>
    public string? ReopenReason { get; set; }

    /// <summary>Whether the reopen action is allowed</summary>
    public bool AllowReopen { get; set; }
}

/// <summary>List DTO for displaying closed litigation requests eligible for reopening</summary>
public class LitigationRequestClosedListDto
{
    public Guid LitigationRequestId { get; set; }

    /// <summary>رقم الدعوى</summary>
    public string? RequestCode { get; set; }

    /// <summary>نوع الدعوى</summary>
    public string? RequestTypeName { get; set; }

    /// <summary>حالة الدعوى</summary>
    public string? InternalStatusName { get; set; }

    /// <summary>تاريخ تقديم الدعوى</summary>
    public DateTime? SubmissionDate { get; set; }

    /// <summary>تاريخ الإلغاء</summary>
    public DateTime? CancellationDate { get; set; }

    /// <summary>اسم مقدم الطلب</summary>
    public string? ApplicantName { get; set; }
}
