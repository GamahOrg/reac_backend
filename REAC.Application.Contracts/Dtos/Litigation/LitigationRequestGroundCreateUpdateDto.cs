using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestGroundCreateUpdateDto : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }
    public List<SysFileManagerCreateUpdateDto> ReferenceFiles { get; set; } = new List<SysFileManagerCreateUpdateDto>();
    public string? Title { get; set; }
}
