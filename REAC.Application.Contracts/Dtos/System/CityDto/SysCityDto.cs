using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.CityDto
{
    public class SysCityDto : BaseEntity<Guid>
    {
        public string CityName { get; set; } = null!;
        public Guid AreaId { get; set; }
    }
}
