using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Actor
{
    public class CitizenBankAccount : BaseEntity<Guid>
    {
        public Guid CitizenId { get; set; }
        public Guid BankId { get; set; }
        public string AccountOwnerName { get; set; } = null!;
        public string IBan { get; set; } = null!;
        public virtual SysBank SysBank { get; set; } = null!;
        public virtual Citizen Citizen { get; set; } = null!;
    }
}
