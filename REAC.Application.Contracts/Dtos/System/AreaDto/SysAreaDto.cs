using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.AreaDto
{
    public class SysAreaDto : BaseEntity<Guid>
    {
        public string AreaName { get; set; } = null!;
    }
}
