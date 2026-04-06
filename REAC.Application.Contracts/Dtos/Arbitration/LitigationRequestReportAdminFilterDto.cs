namespace REAC.Application.Contracts.Dtos.Arbitration;

public class LitigationRequestReportAdminFilterDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    /// <summary>البحث برقم الدعوى</summary>
    public string? RequestCode { get; set; }

    /// <summary>البحث باسم المحكم</summary>
    public string? ArbitratorName { get; set; }

    /// <summary>البحث باسم أحد أطراف الدعوى</summary>
    public string? PartyName { get; set; }

    /// <summary>تاريخ تقديم الدعوى - من</summary>
    public DateTime? SubmissionDateFrom { get; set; }

    /// <summary>تاريخ تقديم الدعوى - إلى</summary>
    public DateTime? SubmissionDateTo { get; set; }

    /// <summary>تاريخ قبول الدعوى من المدقق - من</summary>
    public DateTime? AuditorAcceptanceDateFrom { get; set; }

    /// <summary>تاريخ قبول الدعوى من المدقق - إلى</summary>
    public DateTime? AuditorAcceptanceDateTo { get; set; }

    /// <summary>تاريخ قبول المحكم الدعوى - من</summary>
    public DateTime? ArbitratorAcceptanceDateFrom { get; set; }

    /// <summary>تاريخ قبول المحكم الدعوى - إلى</summary>
    public DateTime? ArbitratorAcceptanceDateTo { get; set; }

    /// <summary>تاريخ إصدار الحكم - من</summary>
    public DateTime? JudgmentDateFrom { get; set; }

    /// <summary>تاريخ إصدار الحكم - إلى</summary>
    public DateTime? JudgmentDateTo { get; set; }
}
