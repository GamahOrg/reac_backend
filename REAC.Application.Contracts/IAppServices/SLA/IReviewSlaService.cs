using Core.Shared.Responses;

namespace REAC.Application.Contracts.IAppServices.SLA
{
    public interface IReviewSlaService
    {
        Task<BaseResponse> Execute();
    }
}
