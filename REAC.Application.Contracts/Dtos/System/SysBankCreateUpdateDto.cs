using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class SysBankCreateUpdateDto : BaseEntity<Guid>
    {
        public string BankName { get; set; } = null!;

    }
}
