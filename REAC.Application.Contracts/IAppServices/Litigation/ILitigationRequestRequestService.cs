using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestRequestService : IBasicService<LitigationRequestRequest, Guid>
    {
        Task<BaseResponse> AddRequests(LitigationRequestRequestCreateUpdateDto model);
        Task<Guid> GetAddedRequestId();
    }
}
