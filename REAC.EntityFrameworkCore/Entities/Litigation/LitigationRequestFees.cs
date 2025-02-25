using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;
using System.ComponentModel.DataAnnotations.Schema;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestFees : BaseEntity<Guid>
    {

        public int ArbitratorsCount { get; set; }
        [ForeignKey("Amounts")]
        public Guid AmountId { get; set; }

        //  public decimal FromAmount { get; set; }
        //  public decimal? ToAmount { get; set; }
        public decimal? ExtraAmount { get; set; }
        public decimal? PercentageExtraAmount { get; set; }
        public decimal ArbitrationFees { get; set; }
        public virtual SysLitigationRequestAmount Amounts { get; set; } 
    }
}
