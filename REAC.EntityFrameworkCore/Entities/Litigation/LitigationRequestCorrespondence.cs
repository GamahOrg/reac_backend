using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestCorrespondence : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Unique identifier of the litigation request correspondence destination")]
    public LitigationRequestCorrespondenceDestination LitigationRequestCorrespondenceDestinationId { get; set; }

    [Comment("Subject")]
    public string? Subject { get; set; }
    [Comment("Description for Serial Code")]
    public string SerialCode { get; set; }

    [Comment("Description for Message")]
    public string? Message { get; set; }
    [Comment("Description for Created By")]
    public Guid? CreatedBy { get; set; }

    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }

    [Comment("Unique identifier of the litigation request category")]
    public LitigationRequestCategory LitigationRequestCategoryId { get; set; }
    [Comment("Status identifier")]
    public CorrespondenceStatus LitigationRequestCorrespondenceStatusId { get; set; }


    public virtual LitigationRequest LitigationRequest { get; set; } = null!;

    public virtual SysLitigationRequestCorrespondenceDestination LitigationRequestCorrespondenceDestination { get; set; } = null!;

    public virtual ICollection<LitigationRequestCorrespondenceFile> LitigationRequestCorrespondenceFiles { get; set; } = new List<LitigationRequestCorrespondenceFile>();
    public virtual ICollection<LitigationRequestCorrespondenceReply> LitigationRequestCorrespondenceReply { get; set; } = new List<LitigationRequestCorrespondenceReply>();

    public virtual Citizen Citizen { get; set; } = null!;
    public virtual SysLitigationRequestCategory? LitigationRequestCategory { get; set; }
    public virtual SysLitigationRequestCorrespondenceStatus? LitigationRequestCorrespondenceStatus { get; set; }

}


