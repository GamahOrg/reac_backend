using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenArbirator : BaseEntity<Guid>
    {
        public string FullName { get; set; } = null!;

        public int RequestsCount { get; set; }
    }
    public class CitizenFullLookupArbirator : BaseEntity<Guid>
    {
        public string FullName { get; set; } = null!;

        public int RequestsCount { get; set; }

        public string? AreaName { get; set; }
        public int? ExperienceYears { get; set; }
        public string? ArbitratorClass { get; set; }
        public string? Brief { get; set; }
    }
}
