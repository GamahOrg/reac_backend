using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using REAC.EntityFrameworkCore.Entities.Notification;
using Core.GenericRepository.BaseRepository;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class SMSService : ISMSService
    {
        private readonly ISysSettingService _sysSetting;
        private readonly IGEELoginService _gEELoginService;
        private readonly ILoggerManager _loggerManager;
        private readonly IEntityRepository _repository;


        public SMSService(ISysSettingService sysSetting, IGEELoginService gEELoginService, ILoggerManager loggerManager, IEntityRepository repository)
        {
            _sysSetting = sysSetting;
            _gEELoginService = gEELoginService;
            _loggerManager = loggerManager;
            _repository = repository;
        }


        public async Task<DataResponse<SMSResponseDto>> SendSMS(SMSRequestDto sMSRequest)
        {
            try
            {
                if (sMSRequest is null)
                    return new DataResponse<SMSResponseDto> { IsDone = false, Message = "" };

                var token = (await _gEELoginService.Login()).Data?.Access_token;
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                      , new LogDto()
                      {
                          Type = Core.Log.Domain.Shared.Enums.LogType.Trace,
                          Message = $"SMS Service Token ===> {token}"
                      });
                if (token is null)
                    return new DataResponse<SMSResponseDto> { IsDone = false, Message = "" };

                var url = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.SMS_URL))?.Data?.Value;
                if (string.IsNullOrWhiteSpace(url))
                    return new DataResponse<SMSResponseDto>() { IsDone = false, Message = "" };

                sMSRequest.notification_type = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.SMS_Notification_Type))?.Data?.Value;
                if (string.IsNullOrWhiteSpace(sMSRequest.notification_type))
                    return new DataResponse<SMSResponseDto>() { IsDone = false, Message = "" };

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

        public async Task SendSMSToSLA(SMSRequestDto sMSRequest)
        {
            try
            {
                foreach (var recipient in sMSRequest.recipients)
                {
                    var smsExist = (await _repository.GetQueryable<SysNotificationSMS, Guid>())
                        .Any(x => x.MobileNumber == recipient && x.Message == sMSRequest.message_text && !x.IsSent);

                    if (!smsExist)
                    {
                        await _repository.AddAsync<SysNotificationSMS, Guid>(new SysNotificationSMS()
                        {
                            Message = sMSRequest.message_text,
                            CreatedOn = DateTime.Now,
                            Id = Guid.NewGuid(),
                            IsSent = false,
                            RetryAttempt = 0,
                            MobileNumber = recipient
                        });
                        await _repository.SaveChangesAsync();
                    }

                }

            }
            catch (Exception e)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                           Message = $"SendSMSToSLA ===> {JsonConvert.SerializeObject(sMSRequest)}" + "\r\n" +
                           $"; SMS Service ===> {e.ToString()}"
                       });
            }
        }
    }
}
