using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.Litigation
{
    public class LitigationRequestFees : BaseEntity<Guid>
    {
        public int ArbitratorsCount { get; set; }
        public decimal FromAmount { get; set; }
        public decimal? ToAmount { get; set; }
        public decimal? ExtraAmount { get; set; }
        public decimal? PercentageExtraAmount { get; set; }
        public decimal Fees { get; set; }
    }
}
