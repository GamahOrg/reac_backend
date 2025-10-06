using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestFees : BaseEntity<Guid>
    {

        [Comment("Number of arbitrators")]
        public int ArbitratorsCount { get; set; }
        [ForeignKey("Amounts")]
        [Comment("Unique identifier of the litigation request amount")]
        public Guid AmountId { get; set; }

        //  public decimal FromAmount { get; set; }
        //  public decimal? ToAmount { get; set; }
        [Comment("Additional amount charged for extra arbitrators")]
        public decimal? ExtraAmount { get; set; }
        [Comment("Percentage")]
        public decimal? PercentageExtraAmount { get; set; }
        [Comment("Total arbitration fees for this case")]
        public decimal ArbitrationFees { get; set; }
        public virtual SysLitigationRequestAmount Amounts { get; set; } 
    }
}

