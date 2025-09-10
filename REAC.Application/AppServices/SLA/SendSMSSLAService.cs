using Core.Dtos;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Newtonsoft.Json;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Notification;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;

namespace REAC.Application.AppServices.SLA
{
    public class SendSMSSLAService : ISendSMSSLAService
    {
        private readonly IEntityRepository _repository;
        private readonly ILoggerManager _loggerManager;
        private readonly IGEELoginService _gEELoginService;
        private readonly ISysSettingService _sysSetting;

        public SendSMSSLAService(IEntityRepository repository, ILoggerManager loggerManager, IGEELoginService gEELoginService, ISysSettingService sysSetting)
        {
            _repository = repository;
            _loggerManager = loggerManager;
            _gEELoginService = gEELoginService;
            _sysSetting = sysSetting;
        }

        public async Task SendSMS()
        {
            try
            {
                var smsList = (await _repository.GetQueryable<SysNotificationSMS, Guid>())
                   .Where(x => !x.IsSent)
                   .OrderBy(x => x.CreatedOn);

                if (smsList.Any())
                {

                    var token = await _gEELoginService.SMSLogin();
                    var url = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.SMS_URL))?.Data?.Value;
                    var notificationType = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.SMS_Notification_Type))?.Data?.Value;

                    if (!token.IsDone)
                    {
                        foreach (var smsObject in smsList)
                        {
                            smsObject.Error = token.Message;
                            smsObject.RetryAttempt++;
                        }

                        await _repository.SaveChangesAsync();
                    }
                    else
                    {

                        foreach (var smsObject in smsList)
                        {
                            try
                            {

                                var body = new SMSRequestDto()
                                {
                                    message_text = smsObject.Message,
                                    recipients = new List<string> { smsObject.MobileNumber },
                                    notification_type = notificationType
                                };

                                var result = await SendSMS(body, token.Data?.Access_token, url);

                                if (!result.IsDone)
                                {
                                    smsObject.Error = result?.Message?.ToString();
                                    smsObject.RetryAttempt++;

                                }
                                else
                                {
                                    smsObject.IsSent = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                    , new LogDto()
                                    {
                                        Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                        Message = $"Send SMS Exception ===> {JsonConvert.SerializeObject(ex)}" + "\r\n" +
                                        $"; SMS Service Exception ===> {ex.ToString()}"
                                    });
                            }


                        }
                        try
                        {

                            await _repository.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                    , new LogDto()
                                    {
                                        Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                        Message = $"Send SMS DBContext Exception ===> {JsonConvert.SerializeObject(ex)}" + "\r\n" +
                                        $"; SMS Service DBContext Exception ===> {ex.ToString()}"
                                    });
                        }
                    }
                }



            }
            catch (Exception ex)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                           Message = $"Send SMS From SLA ===> {JsonConvert.SerializeObject(ex)}" + "\r\n" +
                           $"; SMS Service ===> {ex.ToString()}"
                       });
            }
        }

        private async Task<DataResponse<SMSResponseDto>> SendSMS(SMSRequestDto sMSRequest, string token, string url)
        {
            try
            {


                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var body = JsonConvert.SerializeObject(sMSRequest);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Content = content;

                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
                           Message = $" SMS message ===> {JsonConvert.SerializeObject(sMSRequest)}" + "\r\n" +
                           $"; SMS Service Send Request ===> {JsonConvert.SerializeObject(request)}"
                       });
                var response = await client.SendAsync(request);

                var x = await response.Content.ReadAsStringAsync();
                SMSResponseDto responseDto = new SMSResponseDto();
                if (response.IsSuccessStatusCode)
                {
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
                           Message = $"SMS message ===> {JsonConvert.SerializeObject(sMSRequest)}" + "\r\n" +
                           $"; SMS Service Request ===> {JsonConvert.SerializeObject(request)}" + "\r\n" +
                           $"; SMS Service Response ===> {JsonConvert.SerializeObject(response)}"
                       });
                    responseDto.Data = await response.Content.ReadFromJsonAsync<SMSDataDto>();
                    return new DataResponse<SMSResponseDto> { IsDone = true, Data = responseDto };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    responseDto.SMSUnauthorized = await response.Content.ReadFromJsonAsync<GeneralGEEErrorDto>();

                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
                           Message = $"SMS message ===> {JsonConvert.SerializeObject(sMSRequest)}" + "\r\n" +
                           $"; SMS Service Request ===> {JsonConvert.SerializeObject(request)}" + "\r\n" +
                           $"; SMS Service Response ===> {JsonConvert.SerializeObject(response)}"
                       });
                    return new DataResponse<SMSResponseDto> { IsDone = false, Data = responseDto };
                }
                else
                {
                    responseDto.SMSError = await response.Content.ReadFromJsonAsync<SMSErrorDto>();

                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
                           Message = $"SMS message ===> {JsonConvert.SerializeObject(sMSRequest)}" + "\r\n" +
                           $"; SMS Service Request ===> {JsonConvert.SerializeObject(request)}" + "\r\n" +
                           $"; SMS Service Response ===> {JsonConvert.SerializeObject(response)}"
                       });

                    return new DataResponse<SMSResponseDto> { IsDone = false, Data = responseDto };
                }
            }
            catch (Exception e)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                           Message = $"SMS message ===> {JsonConvert.SerializeObject(sMSRequest)}" + "\r\n" +
                           $"; SMS Service ===> {e.ToString()}"
                       });
                return new DataResponse<SMSResponseDto> { IsDone = false, Message = e.Message + e.InnerException?.Message };
            }
        }
    }

    public interface ISendSMSSLAService
    {
        Task SendSMS();
    }
}
