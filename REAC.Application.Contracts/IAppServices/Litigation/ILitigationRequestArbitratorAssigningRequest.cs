using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestArbitratorAssigningRequestService
    {
        Task<BaseResponse> GetLitigationRequestArbitratorAssigningRequestsAll(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestArbitratorAssigningRequests(CancellationToken cancellationToken);
        Task<BaseResponse> AddLitigationRequestArbitratorAssigningRequestsResponse(AssigningArbitratorRequestsPayload requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestArbitratorAssigningRequests(Guid requestId, Guid LitigationRequestArbitratorAssigningRequestId, CancellationToken cancellationToken);
    }
}
