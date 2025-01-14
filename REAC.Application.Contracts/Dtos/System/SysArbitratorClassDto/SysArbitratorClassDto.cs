using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.AreaDto
{
    public class SysArbitratorClassDto : BaseEntity<Guid>
    {
        public string ArbitratorClassName { get; set; } = null!;
    }
}
