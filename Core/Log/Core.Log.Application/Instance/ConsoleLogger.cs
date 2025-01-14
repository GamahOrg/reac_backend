using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using System.Diagnostics;

namespace Core.Log.Application.Instance
{
    public class ConsoleLogger : ILogger
    {
        public async Task<BaseResponse> Log(LogDto logDto)
        {
            try
            {
                Debug.WriteLine(logDto.Format());
                Console.WriteLine(logDto.Format());
            }
            catch (Exception exc)
            {
                return new ExceptionResponse()
                {
                    IsDone = true,
                    Exception = exc.ToString(),
                    Message = exc.Message
                };
            }
            return new BaseResponse()
            {
                IsDone = true
            };
        }
    }
}
