namespace REAC.Application.Contracts.Dtos.Reconciliation;

public class ReconciliationReportAdminFilterDto
{
    public int PageIndex { get; set; } = 1;
    public int PageSize { get; set; } = 10;

    /// <summary>البحث برقم الدعوى</summary>
    public string? RequestCode { get; set; }

    /// <summary>البحث باسم المصلح</summary>
    public string? ReformerName { get; set; }

    /// <summary>البحث باسم أحد أطراف الدعوى</summary>
    public string? PartyName { get; set; }

    /// <summary>تاريخ تقديم الدعوى - من</summary>
    public DateTime? SubmissionDateFrom { get; set; }

    /// <summary>تاريخ تقديم الدعوى - إلى</summary>
    public DateTime? SubmissionDateTo { get; set; }

    /// <summary>تاريخ تأكيد سداد رسوم الصلح - من</summary>
    public DateTime? ReconciliationFeesPaidDateFrom { get; set; }

    /// <summary>تاريخ تأكيد سداد رسوم الصلح - إلى</summary>
    public DateTime? ReconciliationFeesPaidDateTo { get; set; }

    /// <summary>تاريخ قبول المصلح الدعوى - من</summary>
    public DateTime? ReformerAcceptanceDateFrom { get; set; }

    /// <summary>تاريخ قبول المصلح الدعوى - إلى</summary>
    public DateTime? ReformerAcceptanceDateTo { get; set; }

    /// <summary>تاريخ إصدار المحضر - من</summary>
    public DateTime? ReportIssuanceDateFrom { get; set; }

    /// <summary>تاريخ إصدار المحضر - إلى</summary>
    public DateTime? ReportIssuanceDateTo { get; set; }
}
