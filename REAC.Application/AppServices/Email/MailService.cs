using Core.Contracts;
using Core.Dtos;
using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using Microsoft.Extensions.Options;
using REAC.Domain.Shared.Constants;
using System.Net.Mail;
using REAC.EntityFrameworkCore.Entities.System;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace REAC.Application.AppServices.Email
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
            try
            {
                var emailSetting =  GetEmailSettings();

                if (emailSetting != null && emailSetting.Result != null && emailSetting.Result.SendEmail)
                {

                    MailMessage mail = new MailMessage();
                    SmtpClient client = new SmtpClient
                    {
                        Port = emailSetting.Result.Port,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = emailSetting.Result.UseDefaultCredentials,
                        Host = emailSetting.Result.Host,
                        Credentials = new NetworkCredential(emailSetting.Result.From, emailSetting.Result.Passowrd),
                        EnableSsl = true,
                    };
                    mail.Subject = subject;
                    mail.From = new MailAddress(emailSetting.Result.From);
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    foreach (var toAddress in toAddressList)
                    {
                        if(toAddress is not null)
                        mail.To.Add(new MailAddress(toAddress));
                    }

                    try
                    {
                         client.SendMailAsync(mail);
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
            catch(Exception e) {
                return new BaseResponse { IsDone = false, Message = e.Message };

            }


        }


        async Task<SendEmailOptions?> GetEmailSettings()
        {
            SendEmailOptions? sendEmail = null;

            var query = (await _repository.GetQueryable<SysSetting, Guid>());
            var sysSettings = query.Where(x => x.Key.StartsWith(SysSettingKeyConstants.PrefexEmailConfiguration)).AsNoTracking().ToList();
            if (sysSettings.Any())
            {
                sendEmail = new SendEmailOptions();
                var disable_Email_APIs = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailConfiguration_SendEmail);
                if (disable_Email_APIs != null)
                {
                    sendEmail.SendEmail = bool.Parse(disable_Email_APIs.Value);
                }
                var emailFromAddress = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailFromAddress);
                if (emailFromAddress != null)
                {
                    sendEmail.From = emailFromAddress.Value;
                }
                //var displayNameFromEmailAddress = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.DisplayNameFromEmailAddress);
                //if (displayNameFromEmailAddress != null)
                //{
                //    sendEmail.From = displayNameFromEmailAddress.Value;
                //}
                var emailPassword = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailPassword);
                if (emailPassword != null)
                {
                    sendEmail.Passowrd = emailPassword.Value;
                }
                var emailHost = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailHost);
                if (emailHost != null)
                {
                    sendEmail.Host = emailHost.Value;
                }
                var emailPort = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailPort);
                if (emailPort != null)
                {
                    sendEmail.Port = int.Parse(emailPort.Value);
                }
                var useDefaultCredentials = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.UseDefaultCredentials);
                if (useDefaultCredentials != null)
                {
                    sendEmail.UseDefaultCredentials = bool.Parse(useDefaultCredentials.Value);
                }
            }
            return sendEmail;
        }

    }
}
