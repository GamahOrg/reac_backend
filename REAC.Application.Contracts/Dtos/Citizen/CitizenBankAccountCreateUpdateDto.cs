using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Citizen
{
    public class CitizenBankAccountCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid BankId { get; set; }
        public string AccountOwnerName { get; set; } = null!;
        public string IBan { get; set; } = null!;
    }
}
