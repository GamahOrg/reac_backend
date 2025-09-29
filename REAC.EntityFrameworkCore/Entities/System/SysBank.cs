using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysBank : BaseEntity<Guid>
    {
        [Comment("Name of the bank")]
        public string BankName { get; set; } = null!;

        public virtual ICollection<CitizenBankAccount> CitizenBankAccounts { get; set; } = new List<CitizenBankAccount>();

    }
}

