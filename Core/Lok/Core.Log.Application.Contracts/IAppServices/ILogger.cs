using Core.Log.Application.Contracts.Dto;
using Core.Shared.Responses;

namespace Core.Log.Application.Contracts.IAppServices
{
    public interface ILogger
    {
        Task<BaseResponse> Log(LogDto logDto);
    }
}
