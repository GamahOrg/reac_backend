using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.LitigationRequestStatusDtos;

public partial class SysLitigationRequestStatusDto : BaseEntity<RequestStatus>
{
    public string RequestStatus { get; set; } = null!;


}
