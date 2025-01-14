using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.Dtos.Arbitration
{
    public partial class LitigationRequestArbitrationReportCreateUpdateDto : BaseEntity<Guid>
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
        public string? MeetingLocationName { get; set; }

    }
    public partial class LitigationRequestArbitrationReportExplanationRequestGetDto : BaseEntity<Guid>
    {
        public DateTime ExplanationRequestDate { get; set; }
        public string ExplanationRequests { get; set; } = null!;
        public string? Response { get; set; }
        public DateTime? ResponseOn { get; set; }

        public virtual ICollection<LitigationRequestArbitrationReportExplanationRequestFileGetDto> LitigationRequestArbitrationReportExplanationRequestFiles { get; set; } = new List<LitigationRequestArbitrationReportExplanationRequestFileGetDto>();

    }
    public class LitigationRequestArbitrationReportExplanationRequestFileGetDto : BaseEntity<Guid>
    {
        public virtual SysFileManagerCreateUpdateDto ExplanationRequestFile { get; set; } = null!;
    }

    public partial class LitigationRequestArbitratorRecusalRequestGetDto : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }

        public Guid ArbitratorCitizenId { get; set; }
        public string FullName { get; set; }

        public string Reason { get; set; } = null!;

        public DateTime RequestOn { get; set; }
        public LitigationRequestParty RequestParty { get; set; }

        public ApproveRejectStatus? ApproveRejectStatus { get; set; }

        public DateTime? ResponseOn { get; set; }

        public virtual ICollection<LitigationRequestArbitratorRecusalRequestFile> LitigationRequestArbitratorRecusalRequestFiles { get; set; } = new List<LitigationRequestArbitratorRecusalRequestFile>();

    }
    public class LitigationRequestArbitratorRecusalRequestFileGetDto : BaseEntity<Guid>
    {
        public virtual SysFileManagerCreateUpdateDto RecusalRequestFile { get; set; } = null!;
    }

    public partial class LitigationRequestArbitrationReportExplanationResponseDto : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }

        public string Response { get; set; } = null!;

    }
    public partial class LitigationRequestArbitrationRecusalRequestResponseDto : BaseEntity<Guid>
    {
        public Guid LitigationRequestId { get; set; }

        public ApproveRejectStatus Response { get; set; }

    }


    public partial class LitigationRequestArbitrationReportExplanationRequestCreateDto
    {
        public Guid LitigationRequestId { get; set; }
        public string ExplanationRequests { get; set; } = null!;
        public List<Guid> LitigationRequestArbitrationReportExplanationRequestFiles { get; set; } = new List<Guid>();

    }
    public partial class LitigationRequestArbitrationArbitratorRecusalRequestCreateDto
    {
        public Guid LitigationRequestId { get; set; }
        public Guid ArbitratorCitizenId { get; set; }

        public string Reason { get; set; } = null!;
        public List<Guid> LitigationRequestArbitrationArbitratorRecusalRequestFiles { get; set; } = new List<Guid>();

    }
}
