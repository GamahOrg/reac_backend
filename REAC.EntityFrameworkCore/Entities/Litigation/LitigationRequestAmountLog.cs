using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using System.ComponentModel.DataAnnotations.Schema;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestAmountLog : BaseEntity<Guid>
    {

        [ForeignKey("Amounts")]
        public Guid AmountId { get; set; }
        [ForeignKey("LitigationRequest")]
        public Guid LitigationRequestId { get; set; }
        public DateTime CreatedOn { get; set; }
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual SysLitigationRequestAmount Amounts { get; set; } = null!;
    }
}
