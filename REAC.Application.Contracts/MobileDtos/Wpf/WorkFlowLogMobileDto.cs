using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.MobileDtos.Wpf;

public partial class WorkFlowLogMobileDto : BaseEntity<Guid>
{
    public DateTime CreatedOn { get; set; }

    public Guid RequestId { get; set; }

    public string? LogMessage { get; set; }
}


