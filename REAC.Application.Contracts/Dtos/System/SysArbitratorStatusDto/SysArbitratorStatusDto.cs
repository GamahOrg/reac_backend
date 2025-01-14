using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.ArbitratorStatusDto
{
    public class SysArbitratorStatusDto : BaseEntity<ArbitratorStatus>
    {
        public string StatusName { get; set; } = null!;
    }
}
