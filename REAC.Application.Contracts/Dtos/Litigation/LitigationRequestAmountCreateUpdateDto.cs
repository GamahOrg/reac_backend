using Core.GenericRepository.Shared.BaseEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public partial class LitigationRequestAmountCreateUpdateDto : BaseEntity<Guid>
    {
        public int ArbitratorsCount { get; set; }
        public Guid AmountId { get; set; }
        public decimal? ExtraAmount { get; set; }
        public decimal? PercentageExtraAmount { get; set; }
        public decimal ArbitrationFees { get; set; }
    }
}
