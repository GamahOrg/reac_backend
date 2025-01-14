using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.CitizenRequestStatusDtos;

public partial class SysCitizenRequestStatusDto : BaseEntity<CitizenRequestStatus>
{
    public string RequestStatus { get; set; } = null!;


}
