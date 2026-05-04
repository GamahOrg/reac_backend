using Core.GenericRepository.Shared.BaseEntities;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.EntityFrameworkCore.Entities.Arbitration;

public class LitigationRequestArbitrationCorrectionRequest : BaseEntity<Guid>
{
    [Comment("Foreign key to the litigation request")]
    public Guid LitigationRequestId { get; set; }

    [Comment("Citizen who submitted the correction request (plaintiff or defendant)")]
    public Guid RequestCitizenId { get; set; }

    [Comment("Description of the requested correction - max 1000 chars")]
    public string CorrectionDescription { get; set; } = null!;

    [Comment("Date when the correction request was submitted")]
    public DateTime RequestOn { get; set; }

    [Comment("Party who submitted the request (Plaintiff / Defendant)")]
    public LitigationRequestParty RequestParty { get; set; }

    [Comment("Arbitrator decision: null=Pending, Approve=Accepted, Reject=Rejected")]
    public ApproveRejectStatus? ApproveRejectStatus { get; set; }

    [Comment("Reason for rejection - required when ApproveRejectStatus = Reject, max 300 chars")]
    public string? RejectReason { get; set; }

    [Comment("Arbitrator correction decision text - max 2500 chars, filled on acceptance")]
    public string? CorrectionDecisionText { get; set; }

    [Comment("Date when the arbitrator responded to the request")]
    public DateTime? ResponseOn { get; set; }
    public DateTime CreatedOn { get; set; }
    public Guid? CreatedBy { get; set; }
    public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    public virtual Citizen RequestCitizen { get; set; } = null!;
    public virtual ICollection<LitigationRequestArbitrationCorrectionRequestFile> LitigationRequestArbitrationCorrectionRequestFiles { get; set; } = new List<LitigationRequestArbitrationCorrectionRequestFile>();
}
