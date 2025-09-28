using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitrationReportExplanationRequest : BaseEntity<Guid>
    {
        [Comment("Date when the explanation request was submitted")]
        public DateTime ExplanationRequestDate { get; set; }
        [Comment("Unique identifier of the litigation request arbitration report")]
        public Guid LitigationRequestArbitrationReportId { get; set; }
        [Comment("Description for Request Arbitration Report Explanation Request Party")]
        public LitigationRequestParty RequestArbitrationReportExplanationRequestParty { get; set; }
        [Comment("Description for Explanation Requests")]
        public string ExplanationRequests { get; set; } = null!;
        [Comment("Description for Response")]
        public string? Response { get; set; }
        [Comment("Date when the explanation request response was provided")]
        public DateTime? ResponseOn { get; set; }
        public virtual LitigationRequestArbitrationReport LitigationRequestArbitrationReport { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitrationReportExplanationRequestFile> LitigationRequestArbitrationReportExplanationRequestFiles { get; set; } = new List<LitigationRequestArbitrationReportExplanationRequestFile>();

    }
}

