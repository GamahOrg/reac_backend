using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitrationReport : BaseEntity<Guid>
    {
        [Comment("Unique identifier of the litigation request")]
        public Guid LitigationRequestId { get; set; }

        [Comment("Date when the judgment was issued")]
        public DateTime JudgmentDate { get; set; }
        [Comment("Facts and findings presented in the arbitration report")]
        public string Facts { get; set; } = null!;
        [Comment("Reasoning and justification for the arbitration decision")]
        public string Reasons { get; set; } = null!;
        [Comment("Full text of the judgment decision")]
        public string JudgmentDecisionText { get; set; } = null!;
        [Comment("Details of any modification requests to the arbitration report")]
        public string ModificationRequest { get; set; } = null!;
        [Comment("Date when the judgment decision was made")]
        public DateTime JudgmentDecisionDate { get; set; }
        [Comment("Location where the arbitration judgment decision was issued")]
        public string JudgmentDecisionPlace { get; set; } = null!;
        [Comment("Unique identifier of the litigation request")]
        public Guid MeetingLocationId { get; set; }

        public virtual SysMeetingLocation MeetingLocation { get; set; } = null!;
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitrationReportExplanationRequest> LitigationRequestArbitrationReportExplanationRequests { get; set; } = new List<LitigationRequestArbitrationReportExplanationRequest>();

    }
}

