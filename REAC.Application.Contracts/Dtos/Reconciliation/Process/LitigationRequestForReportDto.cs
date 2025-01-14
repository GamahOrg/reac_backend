using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Application.Contracts.Dtos.Reconciliation.Process
{
    public class LitigationRequestForReportDto
    {
        public bool AllowAdd { get; set; }
        public bool AllowDecision { get; set; }
        public ApproveRejectStatus ApproveRejectStatus { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestReconciliationReportCreateUpdateDto? Report { get; set; }
    }
    public class LitigationRequestForAgreementRequestDto
    {
        public bool AllowAdd { get; set; }
        public ApproveRejectStatus ApproveRejectStatus { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }
    public class LitigationRequestForArbitrationReportDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestArbitrationReportCreateUpdateDto? Report { get; set; }
    }
    public class LitigationRequestForArbitrationReportExplanationRequestDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestArbitrationReportExplanationRequestGetDto? Report { get; set; }
    }

    public class LitigationRequestForArbitratorRecusalRequestDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestArbitratorRecusalRequestGetDto? request { get; set; }
    }

    public class LitigationRequestForAllArbitratorCountDto
    {
        public bool AllowAdd { get; set; }
        public int? ArbitratorsCount { get; set; }
        public int? SecondPartyArbitratorsCount { get; set; }
        public int? FirstPartyArbitratorsCount { get; set; }

        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }

    public class LitigationRequestForAllArbitratorNominationListDto
    {
        public bool AllowAdd { get; set; }
        public int? ListCount { get; set; }

        public List<CitizenArbitratorShortStatusDto> CitizenArbitratorShortStatusDtos { get; set; } = new List<CitizenArbitratorShortStatusDto>();
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }
    public class LitigationRequestForAllArbitratorCharmanNominationListDto
    {
        public bool AllowAdd { get; set; }
        public bool AllowDecision { get; set; }
        public int? ListCount { get; set; }

        public List<CitizenArbitratorCharmanShortStatusDto> CitizenArbitratorCharmanShortStatusDtos { get; set; } = new List<CitizenArbitratorCharmanShortStatusDto>();
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }
    public class LitigationRequestForAllArbitratorNominationListWithPartiesValueDto
    {
        public bool AllowAdd { get; set; }
        public List<CitizenArbitratorShortStatusWithValueDto> CitizenArbitratorShortStatusDtos { get; set; } = new List<CitizenArbitratorShortStatusWithValueDto>();
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }
    public class LitigationRequestForArbitratorSelectionDurationDto
    {
        public bool AllowAdd { get; set; }
        public int? ArbitratorSelectionDuration { get; set; }

        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }

    public class LitigationRequestForArbitratorCountDto
    {
        public bool AllowAdd { get; set; }
        public int? ArbitratorsCount { get; set; }


        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
    }
    public class LitigationRequestForExtendPeriodRequestDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestExtendPeriodRequestGetDto? LitigationRequestExtendPeriodRequestCreateUpdateDto { get; set; }
    }

    public class LitigationRequestForReportApprovalDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }

        public ApproveRejectStatus? minutesOfMeetingApprovalStatus { get; set; }
        public DateTime? minutesOfMeetingApprovalStatusOn { get; set; }
    }
}
