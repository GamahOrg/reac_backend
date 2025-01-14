using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public partial class LitigationRequestArbitrationReport : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }

        public DateTime JudgmentDate { get; set; }
        public string Facts { get; set; } = null!;
        public string Reasons { get; set; } = null!;
        public string JudgmentDecisionText { get; set; } = null!;
        public string ModificationRequest { get; set; } = null!;
        public DateTime JudgmentDecisionDate { get; set; }
        public string JudgmentDecisionPlace { get; set; } = null!;
        public Guid MeetingLocationId { get; set; }

        public virtual SysMeetingLocation MeetingLocation { get; set; } = null!;
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitrationReportExplanationRequest> LitigationRequestArbitrationReportExplanationRequests { get; set; } = new List<LitigationRequestArbitrationReportExplanationRequest>();

    }
}
