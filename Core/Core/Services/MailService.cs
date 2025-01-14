using Core.Contracts;
using Core.Dtos;
using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using Microsoft.Extensions.Options;
using REAC.Domain.Shared.Constants;
using System.Net.Mail;

namespace Core.Services
{
    public class MailService : IMailService
    {
        private readonly SendEmailOptions options;
        private readonly IEntityRepository _repository;

        public MailService(IOptions<SendEmailOptions> options, IEntityRepository repository)
        {
            this.options = options.Value;
            _repository = repository;
        }
        public async Task<BaseResponse> SendMail(List<string> toAddressList, string subject, string body)
        {
            if (options.SendEmail)
            {
                MailMessage mail = new MailMessage();

                SmtpClient client = new SmtpClient
                {
                    Port = options.Port,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = options.UseDefaultCredentials,
                    Host = options.Host,
                    Credentials = new System.Net.NetworkCredential(options.From, options.Passowrd),
                    EnableSsl = true
                };
                mail.Subject = subject;
                mail.From = new MailAddress(options.From);
                mail.Body = body;
                mail.IsBodyHtml = true;

                foreach (var toAddress in toAddressList)
                    mail.To.Add(new MailAddress(toAddress));

                try
                {
                    await client.SendMailAsync(mail);
                    return new BaseResponse { IsDone = true };
                }
                catch (Exception ex)
                {
                    return new BaseResponse { IsDone = false, Message = ex.Message + ex.InnerException?.Message };
                }
            }
            else
            {
                return new BaseResponse { IsDone = true };
            }


        }




    }
}
