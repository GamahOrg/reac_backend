using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenReformerShortDto : BaseEntity<Guid>
    {
        public string IdentityNumber { get; set; } = null!;

        public string FullName { get; set; } = null!;

        public string Mobile { get; set; } = null!;

        public string? Email { get; set; } = null!;

        public int RequestsCount { get; set; }

        public string? ReformerStatus { get; set; }

        public DateTime? ReformerStatusDate { get; set; }


    }



}
