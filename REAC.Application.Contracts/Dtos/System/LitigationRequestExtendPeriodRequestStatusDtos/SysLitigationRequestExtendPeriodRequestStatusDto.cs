using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestExtendPeriodRequestStatusDtos;

public partial class SysLitigationRequestExtendPeriodRequestStatusDto : BaseEntity<LitigationRequestExtendPeriodRequestStatus>
{
    public string StatusName { get; set; } = null!;


}
