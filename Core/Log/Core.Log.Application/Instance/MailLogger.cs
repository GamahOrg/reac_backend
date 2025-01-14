using Core.Contracts;
using Core.Log.Application.Contracts.Dto;
using Core.Shared.Responses;
using Microsoft.Extensions.Options;
using ILogger = Core.Log.Application.Contracts.IAppServices.ILogger;

namespace Core.Log.Application.Instance
{
    public class MailLogger : ILogger
    {
        private readonly IMailService _mailService;
        readonly EmailLoggerOptions options;

        public MailLogger(IMailService mailService, IOptions<EmailLoggerOptions> options)
        {
            _mailService = mailService;
            this.options = options.Value;
        }
        public async Task<BaseResponse> Log(LogDto logDto)
        {
            try
            {
                var To = new List<string>() { options.To };
                await _mailService.SendMail(To, logDto.Type.ToString() + " REAC System", logDto.Format());
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


