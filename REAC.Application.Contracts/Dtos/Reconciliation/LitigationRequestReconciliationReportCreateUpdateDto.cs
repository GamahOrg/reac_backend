using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Reconciliation;

public partial class LitigationRequestReconciliationReportCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public DateTime ReportDate { get; set; }

    public string ReconciliationSessionDetails { get; set; } = null!;

    public string? ModificationRequest { get; set; } = null!;

    public int? ReconciliationRequestDecisionStatus { get; set; }

    public virtual ICollection<LitigationRequestReconciliationReportItemCreateUpdateDto> LitigationRequestMeetingReconciliationItems { get; set; } = new List<LitigationRequestReconciliationReportItemCreateUpdateDto>();

}
