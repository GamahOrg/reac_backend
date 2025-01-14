using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowLog;

public partial class WorkFlowLogDto : BaseEntity<Guid>
{
    public DateTime CreatedOn { get; set; }

    public Guid RequestId { get; set; }

    public string? LogMessage { get; set; }
}


