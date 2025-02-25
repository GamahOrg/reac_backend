using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestAmountCreateUpdateDto : BaseEntity<Guid>
{
    public string LitigationAmount { get; set; } = null!;

    public decimal Fees { get; set; }
    public decimal? FromAmount { get; set; }
    public decimal? ToAmount { get; set; }

    public List<LitigationRequestAmountCreateUpdateDto> LitigationRequestFees { get; set; }
}
