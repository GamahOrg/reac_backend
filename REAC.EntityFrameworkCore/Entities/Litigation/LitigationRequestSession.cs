using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Litigation;

public partial class LitigationRequestSession : BaseEntity<Guid>
{
    [Comment("Unique identifier of the litigation request")]
    public Guid LitigationRequestId { get; set; }
    [Comment("Date and time of the note")]
    public DateTime SessionDatetime { get; set; }
    [Comment("Unique identifier of the litigation request session request type")]
    public LitigationRequestSessionRequestType LitigationRequestSessionRequestTypeId { get; set; }
    [Comment("Type of the session (ArbitrationSession, VerdictPronouncement, DeliberationBetweenArbitrators)")]
    public LitigationSessionType? SessionTypeId { get; set; }
    [Comment("Date and time when the record was created")]
    public DateTime CreatedOn { get; set; }
    [Comment("URL")]
    public string? SessionUrl { get; set; }
    [Comment("Unique identifier of the meeting")]
    public long? MeetingId { get; set; }
    public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    public virtual ICollection<LititgationRequestModificationRequest> LitigationRequestModificationRequests { get; set; } = new List<LititgationRequestModificationRequest>();

}


