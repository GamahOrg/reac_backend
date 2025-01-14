using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;

namespace Core.Log.Application.Instance
{
    public class SMSLogger : ILogger
    {
        public Task<BaseResponse> Log(LogDto logDto)
        {
            throw new NotImplementedException();
        }
    }
}
