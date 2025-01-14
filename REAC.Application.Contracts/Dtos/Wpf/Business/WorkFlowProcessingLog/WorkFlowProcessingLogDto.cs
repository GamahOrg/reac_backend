using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLogFile;

namespace REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLog
{
    public partial class WorkFlowProcessingLogDto
    {
        public string EventName { get; set; } = null!;

        public string StepName { get; set; } = null!;

        public Guid DepartmentId { get; set; }

        public Guid EmployeeId { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid SourceWorkFlowProcessingLogId { get; set; }

        public DateTime FinishedOn { get; set; }

        public string? Notes { get; set; }

        public virtual ICollection<WorkFlowProcessingLogFileDto> WorkFlowProcessingLogFiles { get; set; } = new List<WorkFlowProcessingLogFileDto>();

    }
}
