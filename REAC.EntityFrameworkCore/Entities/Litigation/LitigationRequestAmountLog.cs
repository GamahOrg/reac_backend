using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestAmountLog : BaseEntity<Guid>
    {

        [ForeignKey("Amounts")]
        [Comment("Unique identifier of the litigation request amount")]
        public Guid AmountId { get; set; }
        [ForeignKey("LitigationRequest")]
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }
        [Comment("Date and time when the record was created")]
        public DateTime CreatedOn { get; set; }
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual SysLitigationRequestAmount Amounts { get; set; } = null!;
    }
}

