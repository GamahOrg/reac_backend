using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Arbitration
{
    public class LitigationRequestArbitrationReportExplanationRequest : BaseEntity<Guid>
    {
        public DateTime ExplanationRequestDate { get; set; }
        public Guid LitigationRequestArbitrationReportId { get; set; }
        public LitigationRequestParty RequestArbitrationReportExplanationRequestParty { get; set; }
        public string ExplanationRequests { get; set; } = null!;
        public string? Response { get; set; }
        public DateTime? ResponseOn { get; set; }
        public virtual LitigationRequestArbitrationReport LitigationRequestArbitrationReport { get; set; } = null!;
        public virtual ICollection<LitigationRequestArbitrationReportExplanationRequestFile> LitigationRequestArbitrationReportExplanationRequestFiles { get; set; } = new List<LitigationRequestArbitrationReportExplanationRequestFile>();

    }
}
