using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.EntityFrameworkCore.Entities.Wpf.Business;

public partial class WorkFlowProcessingLog : BaseEntity<Guid>
{
    public DateTime CreatedOn { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid RequestId { get; set; }

    public Guid? WorkFlowStepEventId { get; set; }

    public Guid? SourceWorkFlowProcessingLogId { get; set; }

    public Guid WorkFlowStepId { get; set; }

    public bool IsWithNotes { get; set; }

    public bool? ShowForApplicant { get; set; }

    public string? Notes { get; set; }

    public bool IsFinished { get; set; }

    public DateTime? FinishedOn { get; set; }
    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual SysEmployee? Employee { get; set; } = null!;

    public virtual WorkFlowStepEvent? WorkFlowStepEvent { get; set; }

    public virtual WorkFlowProcessingLog? SourceWorkFlowProcessingLog { get; set; }

    public virtual ICollection<WorkFlowProcessingLogFile> WorkFlowProcessingLogFiles { get; set; } = new List<WorkFlowProcessingLogFile>();

    public virtual WorkFlowStep WorkFlowStep { get; set; } = null!;

    public virtual ICollection<WorkFlowProcessingLog> WorkFlowDestinationProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

}
