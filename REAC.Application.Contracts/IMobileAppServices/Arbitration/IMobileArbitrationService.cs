using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Arbitration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IMobileAppServices.Arbitration
{
    public interface IMobileArbitrationService
    {
        Task<BaseResponse> AddArbitrationReportExplanationRequest(LitigationRequestArbitrationReportExplanationRequestCreateDto litigationRequestArbitrationReportExplanationRequestCreateDto);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequest(Guid requestId);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequest(int pageIndex, int pageSize);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequestForArbitrator(Guid requestId);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequestForArbitrator(int pageIndex, int pageSize);
        Task<BaseResponse> AddArbitratorsCount(ArbitratorCountRequestPayload arbitratorCountRequestPayload);
        Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsNominationListReply(ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsNominationListReply(Guid litigationRequestId);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsNominationListReply(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorsCount(Guid requestId);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsCountReply(int pageIndex, int pageSize);
        Task<BaseResponse> GetLitigationRequestForReport(Guid requestId);
    }
}
