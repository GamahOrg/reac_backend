using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenReformer : BaseEntity<Guid>
    {
        public string FullName { get; set; } = null!;

        public int RequestsCount { get; set; }
    }

    public class CitizenAbitrator : BaseEntity<Guid>
    {
        public string FullName { get; set; } = null!;

        public int RequestsCount { get; set; }
    }
}
