using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.Business;

public partial class WorkFlowLog : BaseEntity<Guid>
{
    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }

    [Comment("Unique identifier of the request")]
    public Guid RequestId { get; set; }

    [Comment("Description for Show For Applicant")]
    public bool ShowForApplicant { get; set; }

    [Comment("Description for Log Message")]
    public string? LogMessage { get; set; }
    public virtual ICollection<WorkFlowLogTranslation> Translations { get; set; } = new List<WorkFlowLogTranslation>();


}

