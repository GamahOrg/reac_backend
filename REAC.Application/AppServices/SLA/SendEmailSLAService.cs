using Core.Dtos;
using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using REAC.EntityFrameworkCore.Entities.Notification;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using Core.Log.Application.Contracts.IAppServices;
using Core.Log.Application.Contracts.Dto;
using Newtonsoft.Json;
using System.Net.Http.Json;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;

namespace REAC.Application.AppServices.SLA
{
    public class SendEmailSLAService : ISendEmailSLAService
    {
        private readonly IEntityRepository _repository;
        public static SendEmailOptions emailSetting;
        private readonly ILoggerManager _loggerManager;
        private readonly IGEELoginService _gEELoginService;
        private readonly ISysSettingService _sysSetting;
        public SendEmailSLAService(IEntityRepository repository, ILoggerManager loggerManager, IGEELoginService gEELoginService, ISysSettingService sysSetting)
        {
            _repository = repository;
            _loggerManager = loggerManager;
            _gEELoginService = gEELoginService;
            _sysSetting = sysSetting;
        }

        #region Old Code

        //public async Task SendMail()
        //{
        //    try
        //    {

        //        if (emailSetting != null && emailSetting != null && emailSetting.SendEmail)
        //        {
        //            var emails = (await _repository.GetQueryable<SysNotificationEmail, Guid>())
        //           .Where(x => !x.IsSent);

        //            if (emails.Any())
        //            {
        //                foreach (var email in emails)
        //                {

        //                    MailMessage mail = new MailMessage();
        //                    SmtpClient client = new SmtpClient
        //                    {
        //                        Port = emailSetting.Port,
        //                        DeliveryMethod = SmtpDeliveryMethod.Network,
        //                        UseDefaultCredentials = emailSetting.UseDefaultCredentials,
        //                        Host = emailSetting.Host,
        //                        Credentials = new NetworkCredential(emailSetting.From, emailSetting.Passowrd),
        //                        EnableSsl = true,
        //                    };
        //                    mail.Subject = email.Subject;
        //                    mail.From = new MailAddress(emailSetting.From);

        //                    mail.Body = email.Body;
        //                    mail.IsBodyHtml = true;
        //                    mail.To.Add(new MailAddress(email.ToAddressList));

        //                    try
        //                    {
        //                        await client.SendMailAsync(mail);
        //                        email.Error = null;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        email.Error = ex.ToString();
        //                    }
        //                    finally
        //                    {

        //                        email.IsSent = true;

        //                        await _repository.SaveChangesAsync();
        //                    }

        //                }
        //            }



        //        }

        //    }
        //    catch (Exception e)
        //    {

        //        _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
        //              , new LogDto()
        //              {
        //                  Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
        //                  Message = $"SendMail Exception ===> {e.ToString()}"
        //              });
        //    }

        //}
        #endregion

        public async Task SendMail()
        {

            var emails = (await _repository.GetQueryable<SysNotificationEmail, Guid>())
           .Where(x => !x.IsSent)
           .OrderByDescending(x => x.CreatedOn)
           .Take(10);

            if (emails.Any())
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                var url = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.EmailConfiguration_URL))?.Data?.Value;

                foreach (var email in emails)
                {
                    try
                    {
                        var res = await SendMail(email, token, url);

                        if (res.IsDone)
                        {
                            email.IsSent = true;
                            email.Error = null;
                        }
                        else
                        {
                            email.RetryAttempt++;
                            email.Error = res.Data.SMSError != null ? res.Data.SMSError.ToString() : res.Data.SMSUnauthorized.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        email.Error = ex.ToString();
                    }
                    finally
                    {
                        await _repository.SaveChangesAsync();
                    }

                }
            }

        }

        public async Task GetEmailSettings()
        {
            try
            {
                var query = await _repository.GetQueryable<SysSetting, Guid>();
                var sysSettings = query.Where(x => x.Key.StartsWith(SysSettingKeyConstants.PrefexEmailConfiguration)).AsNoTracking().ToList();
                if (sysSettings.Any())
                {
                    emailSetting = new SendEmailOptions();
                    var disable_Email_APIs = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailConfiguration_SendEmail);
                    if (disable_Email_APIs != null)
                    {
                        emailSetting.SendEmail = bool.Parse(disable_Email_APIs.Value);
                    }

                    var emailUrl = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailConfiguration_URL);
                    if (emailUrl != null)
                    {
                        emailSetting.EmailURL = emailUrl.Value;
                    }
                    var emailFromAddress = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailFromAddress);
                    if (emailFromAddress != null)
                    {
                        emailSetting.From = emailFromAddress.Value;
                    }
                    //var displayNameFromEmailAddress = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.DisplayNameFromEmailAddress);
                    //if (displayNameFromEmailAddress != null)
                    //{
                    //    sendEmail.From = displayNameFromEmailAddress.Value;
                    //}
                    var emailPassword = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailPassword);
                    if (emailPassword != null)
                    {
                        emailSetting.Passowrd = emailPassword.Value;
                    }
                    var emailHost = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailHost);
                    if (emailHost != null)
                    {
                        emailSetting.Host = emailHost.Value;
                    }
                    var emailPort = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.EmailPort);
                    if (emailPort != null)
                    {
                        emailSetting.Port = int.Parse(emailPort.Value);
                    }
                    var useDefaultCredentials = sysSettings.FirstOrDefault(x => x.Key == SysSettingKeyConstants.UseDefaultCredentials);
                    if (useDefaultCredentials != null)
                    {
                        emailSetting.UseDefaultCredentials = bool.Parse(useDefaultCredentials.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                      , new LogDto()
                                      {
                                          Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
                                          Message = $"GetEmailSettings Exception ===> {ex.ToString()}"
                                      });
            }

            //return emailSetting;
        }

        private async Task<DataResponse<SMSResponseDto>> SendMail(SysNotificationEmail emailRequest, string token, string url)
        {

            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("Authorization", $"Bearer {token}");
           
            var formData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("subject", emailRequest.Subject),
                new KeyValuePair<string, string>("html_body", emailRequest.Body),
                new KeyValuePair<string, string>("to", emailRequest.ToAddressList)
            };

            request.Content = new FormUrlEncodedContent(formData);


            var response = await client.SendAsync(request);

            var x = await response.Content.ReadAsStringAsync();
            SMSResponseDto responseDto = new SMSResponseDto();
            if (response.IsSuccessStatusCode)
            {

                responseDto.Data = await response.Content.ReadFromJsonAsync<SMSDataDto>();
                return new DataResponse<SMSResponseDto> { IsDone = true, Data = responseDto };
            }
            else if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                responseDto.SMSUnauthorized = await response.Content.ReadFromJsonAsync<GeneralGEEErrorDto>();

                return new DataResponse<SMSResponseDto> { IsDone = false, Data = responseDto };
            }
            else
            {
                responseDto.SMSError = await response.Content.ReadFromJsonAsync<SMSErrorDto>();

                return new DataResponse<SMSResponseDto> { IsDone = false, Data = responseDto };
            }

        }


        private List<string> ConvertToList(string csv)
        {
            // Split the string by comma and trim whitespace
            List<string> list = new List<string>(csv.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));

            // Trim each item to remove leading/trailing whitespace
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = list[i].Trim();
            }

            return list;
        }
    }

    public interface ISendEmailSLAService
    {
        Task SendMail();
        Task GetEmailSettings();
    }
}
