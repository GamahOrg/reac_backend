using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Wpf.Business;

public partial class WorkFlowProcessingLog : BaseEntity<Guid>
{
    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }

    [Comment("Unique identifier of the employee")]
    public Guid? EmployeeId { get; set; }

    [Comment("Unique identifier of the request")]
    public Guid RequestId { get; set; }

    [Comment("Unique identifier of the work flow step event")]
    public Guid? WorkFlowStepEventId { get; set; }

    [Comment("Unique identifier of the source work flow processing log")]
    public Guid? SourceWorkFlowProcessingLogId { get; set; }

    [Comment("Unique identifier of the work flow step")]
    public Guid WorkFlowStepId { get; set; }

    [Comment("Notes")]
    public bool IsWithNotes { get; set; }

    [Comment("Description for Show For Applicant")]
    public bool? ShowForApplicant { get; set; }

    [Comment("Notes")]
    public string? Notes { get; set; }

    [Comment("Description for Is Finished")]
    public bool IsFinished { get; set; }

    [Comment("Date and time when the workflow processing was finished")]
    public DateTime? FinishedOn { get; set; }
    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual SysEmployee? Employee { get; set; } = null!;

    public virtual WorkFlowStepEvent? WorkFlowStepEvent { get; set; }

    public virtual WorkFlowProcessingLog? SourceWorkFlowProcessingLog { get; set; }

    public virtual ICollection<WorkFlowProcessingLogFile> WorkFlowProcessingLogFiles { get; set; } = new List<WorkFlowProcessingLogFile>();

    public virtual WorkFlowStep WorkFlowStep { get; set; } = null!;

    public virtual ICollection<WorkFlowProcessingLog> WorkFlowDestinationProcessingLogs { get; set; } = new List<WorkFlowProcessingLog>();

}

