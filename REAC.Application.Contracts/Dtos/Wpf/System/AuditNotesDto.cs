using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLogFile;

namespace REAC.Application.Contracts.Dtos.Wpf.System
{
    public class AuditNotesDto : BaseEntity<Guid>
    {
        public string Notes { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual ICollection<WorkFlowProcessingLogFileAllDto> WorkFlowProcessingLogFiles { get; set; } = new List<WorkFlowProcessingLogFileAllDto>();
    }
}
