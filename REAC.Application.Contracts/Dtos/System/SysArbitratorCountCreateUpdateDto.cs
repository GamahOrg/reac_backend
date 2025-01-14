using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System
{
    public class SysArbitratorCountCreateUpdateDto : BaseEntity<Guid>
    {
        public int ArbitratorCount { get; set; }
    }
}
