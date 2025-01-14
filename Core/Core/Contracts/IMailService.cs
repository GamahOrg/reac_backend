using Core.Shared.Responses;

namespace Core.Contracts
{
    public interface IMailService
    {
        /// <summary>
        /// Send email to multiple email accounts with subject and message
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task<BaseResponse> SendMail(List<string> to, string subject, string body);
    }
}
