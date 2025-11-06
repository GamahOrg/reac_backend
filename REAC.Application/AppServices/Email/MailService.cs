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
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Notification;
using Core.Log.Application.Contracts.IAppServices;
using Core.Log.Application.Contracts.Dto;

namespace REAC.Application.AppServices.Email
{
    public class MailService : IMailService
    {
        private readonly SendEmailOptions options;
        private readonly IEntityRepository _repository;
        //private readonly ILoggerManager _loggerManager;
        private SendEmailOptions sendEmail;

        public MailService(IOptions<SendEmailOptions> options, IEntityRepository repository)
        {
            this.options = options.Value;
            _repository = repository;
            //_loggerManager = loggerManager;
        }
        public async Task<BaseResponse> SendMail(List<string> toAddressList, string subject, string body)
        {
            try
            {
                var emailSetting = GetEmailSettings();

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
                        EnableSsl = false,
                    };
                    mail.Subject = subject;
                    mail.From = new MailAddress(emailSetting.Result.From);
                    /*    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", body);
                        }
                    */
                    mail.Body = body;
                    mail.IsBodyHtml = true;

                    foreach (var toAddress in toAddressList)
                    {
                        if (toAddress is not null)
                            mail.To.Add(new MailAddress(toAddress));
                    }

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
            catch (Exception e)
            {
                return new BaseResponse { IsDone = false, Message = e.Message };

            }


        }

        public async Task SendMailToSLA(List<string> toAddressList, string subject, string body)
        {
            try
            {
                // Remove duplicates while preserving order
                var uniqueAddresses = toAddressList
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                foreach (var toAddress in uniqueAddresses)
                {
                    // Check if email already exists
                    //var emailExist = (await _repository.GetQueryable<SysNotificationEmail, Guid>())
                    //    .Any(x => x.ToAddressList == toAddress && x.Body == body && !x.IsSent && x.CreatedOn.Hour == DateTime.Now.Hour );
                    if (toAddress != null)
                        await _repository.AddAsync<SysNotificationEmail, Guid>(new SysNotificationEmail()
                        {
                            Subject = subject,
                            Body = body,
                            CreatedOn = DateTime.Now,
                            Id = Guid.NewGuid(),
                            IsSent = false,
                            RetryAttempt = 0,
                            ToAddressList = toAddress
                        });


                }

                await _repository.SaveChangesAsync();
            }
            catch (Exception e)
            {
                //_loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                //      , new LogDto()
                //      {
                //          Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
                //          Message = $"SendMailToSLA Exception ===> {e.ToString()}"
                //      });

            }


        }


        async Task<SendEmailOptions?> GetEmailSettings()
        {
            //SendEmailOptions? sendEmail = null;

            var query = (await _repository.GetQueryable<SysSetting, Guid>());
            var sysSettings = query.Where(x => x.Key.StartsWith(SysSettingKeyConstants.PrefexEmailConfiguration)).AsNoTracking().ToList();
            if (sysSettings.Any())
            {
                //sendEmail = new SendEmailOptions();
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
