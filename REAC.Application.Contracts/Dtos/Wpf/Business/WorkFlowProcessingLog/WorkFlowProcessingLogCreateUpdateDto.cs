using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLogFile;

namespace REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLog;

public partial class WorkFlowProcessingLogCreateUpdateDto : BaseEntity<Guid>
{
    public DateTime CreatedOn { get; set; }

    public Guid? EmployeeId { get; set; }

    public Guid RequestId { get; set; }

    public Guid? WorkFlowStepEventId { get; set; }

    public bool? ShowForApplicant { get; set; }

    public Guid? SourceWorkFlowProcessingLogId { get; set; }

    public Guid WorkFlowStepId { get; set; }

    public bool IsWithNotes { get; set; }

    public string? Notes { get; set; }

    public bool IsFinished { get; set; }

    public DateTime? FinishedOn { get; set; }


    public List<WorkFlowProcessingLogFileCreateUpdateDto> WorkFlowProcessingLogFiles { get; set; } = new List<WorkFlowProcessingLogFileCreateUpdateDto>();

}
