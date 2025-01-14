using Core.Shared.Responses;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestReconciliationReportService
    {
        Task<BaseResponse> AddCitizenLitigationRequestForReportReply(Guid requestId,string nationalId, ApproveRejectStatus requestMinutesOfMeetingApprovalStatus);
    }
}
