using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenArbitratorShortDto : BaseEntity<Guid>
    {
        public string IdentityNumber { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public string? Email { get; set; } = null!;
        public int? ExperienceYears { get; set; }
        public string? ArbitratorClass { get; set; }
        public int? RequestsCount { get; set; }
        public string? Brief { get; set; }

        public string? ArbitratorStatus { get; set; }

        public DateTime? ArbitratorStatusDate { get; set; }

    }

    public class CitizenArbitratorShortStatusDto : BaseEntity<Guid>
    {
        public string IdentityNumber { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public string? Email { get; set; } = null!;
        public int? ExperienceYears { get; set; }
        public string? ArbitratorClass { get; set; }
        public int? RequestsCount { get; set; }
        public string? Brief { get; set; }

        public string? ArbitratorStatus { get; set; }

        public DateTime? ArbitratorStatusDate { get; set; }
        public int? PlaintiffPriority { get; set; }
        public int? DefendantPriority { get; set; }

        public string Status { get; set; } = null!;
    }
    public class CitizenArbitratorCharmanShortStatusDto : BaseEntity<Guid>
    {
        public string IdentityNumber { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public string? Email { get; set; } = null!;
        public int? ExperienceYears { get; set; }
        public string? ArbitratorClass { get; set; }
        public int? RequestsCount { get; set; }
        public string? Brief { get; set; }

        public string? ArbitratorStatus { get; set; }
        public int? TotalPriority { get; set; }
        public DateTime? ArbitratorStatusDate { get; set; }
        public string Status { get; set; } = null!;
    }
    public class CitizenArbitratorShortStatusWithValueDto : BaseEntity<Guid>
    {
        public Guid CitizenId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Brief { get; set; }

        public int? Value { get; set; }
    }

}
