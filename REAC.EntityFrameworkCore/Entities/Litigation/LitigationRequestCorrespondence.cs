using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestCorrespondence : BaseEntity<Guid>
{
    public Guid LitigationRequestId { get; set; }

    public LitigationRequestCorrespondenceDestination LitigationRequestCorrespondenceDestinationId { get; set; }

    public string? Subject { get; set; }
    public string SerialCode { get; set; }

    public string? Message { get; set; }
    public Guid? CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public LitigationRequestCategory LitigationRequestCategoryId { get; set; }
    public CorrespondenceStatus LitigationRequestCorrespondenceStatusId { get; set; }


    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual SysLitigationRequestCorrespondenceDestination LitigationRequestCorrespondenceDestination { get; set; } = null!;

    public virtual ICollection<LitigationRequestCorrespondenceFile> LitigationRequestCorrespondenceFiles { get; set; } = new List<LitigationRequestCorrespondenceFile>();
    public virtual ICollection<LitigationRequestCorrespondenceReply> LitigationRequestCorrespondenceReply { get; set; } = new List<LitigationRequestCorrespondenceReply>();

    public virtual Citizen Citizen { get; set; } = null!;
    public virtual SysLitigationRequestCategory? LitigationRequestCategory { get; set; }
    public virtual SysLitigationRequestCorrespondenceStatus? LitigationRequestCorrespondenceStatus { get; set; }

}
