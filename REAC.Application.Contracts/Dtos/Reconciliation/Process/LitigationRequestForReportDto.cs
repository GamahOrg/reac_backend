using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.Domain.Shared.Enum;
using System.Collections.Generic;
using System;

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

    public class LitigationRequestForExtendPeriodRequestDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }
        public LitigationRequestExtendPeriodRequestGetDto? LitigationRequestExtendPeriodRequestCreateUpdateDto { get; set; }
        public List<LitigationRequestExtendPeriodRequestGetDto> LitigationRequestExtendPeriodRequests { get; set; } = new List<LitigationRequestExtendPeriodRequestGetDto>();
    }

    public class LitigationRequestForReportApprovalDto
    {
        public bool AllowAdd { get; set; }
        public LitigationRequestShortDto? LitigationRequestShortDto { get; set; }

        public ApproveRejectStatus? minutesOfMeetingApprovalStatus { get; set; }
        public DateTime? minutesOfMeetingApprovalStatusOn { get; set; }
    }
}
