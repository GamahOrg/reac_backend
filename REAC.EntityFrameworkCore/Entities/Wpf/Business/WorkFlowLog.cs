using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Wpf.Business;

public partial class WorkFlowLog : BaseEntity<Guid>
{
    public DateTime CreatedOn { get; set; }

    public Guid RequestId { get; set; }

    public bool ShowForApplicant { get; set; }

    public string? LogMessage { get; set; }
    public virtual ICollection<WorkFlowLogTranslation> Translations { get; set; } = new List<WorkFlowLogTranslation>();


}
