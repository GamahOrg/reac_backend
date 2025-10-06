using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestAmount : BaseEntity<Guid>
{
    [Comment("Display name for the litigation amount range")]
    public string LitigationAmount { get; set; } = null!;

    [Comment("Base fees for this litigation amount range")]
    public decimal Fees { get; set; }

    [Comment("Minimum amount for this litigation range")]
    public decimal? FromAmount { get; set; }
    [Comment("Maximum amount for this litigation range")]
    public decimal? ToAmount { get; set; }

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<LitigationRequestFees> LitigationRequestFees { get; set; } = new List<LitigationRequestFees>();
}

