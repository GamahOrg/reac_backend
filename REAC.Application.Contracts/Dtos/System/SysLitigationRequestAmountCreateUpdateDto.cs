using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestAmountCreateUpdateDto : BaseEntity<Guid>
{
    public string LitigationAmount { get; set; } = null!;

    public decimal Fees { get; set; }
    public decimal? FromAmount { get; set; }
    public decimal? ToAmount { get; set; }
}
