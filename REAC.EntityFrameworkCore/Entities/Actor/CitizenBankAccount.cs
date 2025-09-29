using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Actor
{
    public class CitizenBankAccount : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the citizen")]
        public Guid CitizenId { get; set; }
        [Comment("Unique identifier of the bank")]
        public Guid BankId { get; set; }
        [Comment("Name of the account owner")]
        public string AccountOwnerName { get; set; } = null!;
        [Comment("International Bank Account Number")]
        public string IBan { get; set; } = null!;
        public virtual SysBank SysBank { get; set; } = null!;
        public virtual Citizen Citizen { get; set; } = null!;
    }
}
