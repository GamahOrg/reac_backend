using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestReformerAssigningRequest
    {
        Task<BaseResponse> GetLitigationRequestReformerAssigningRequestsAll(int pageIndex, int pageSize, CancellationToken cancellationToken, bool ascending = false);
        Task<BaseResponse> GetLitigationRequestReformerAssigningRequests(int pageIndex, int pageSize, CancellationToken cancellationToken, bool ascending = false);
        Task<BaseResponse> AddLitigationRequestReformerAssigningRequestsResponse(AssigningRequestsPayload requestPayload, CancellationToken cancellationToken);
    }

}
