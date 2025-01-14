using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysLitigationRequestAmount : BaseEntity<Guid>
{
    public string LitigationAmount { get; set; } = null!;

    public decimal Fees { get; set; }

    public virtual ICollection<LitigationRequest> LitigationRequests { get; set; } = new List<LitigationRequest>();
}
