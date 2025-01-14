using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using REAC.Domain.Shared.Helpers;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class SMSService : ISMSService
    {
        private readonly ISysSettingService _sysSetting;
        private readonly IGEELoginService _gEELoginService;

        public SMSService(ISysSettingService sysSetting, IGEELoginService gEELoginService)
        {
            _sysSetting = sysSetting;
            _gEELoginService = gEELoginService;
        }


        public async Task<DataResponse<SMSResponseDto>> SendSMS(SMSRequestDto sMSRequest)
        {
            try
            {
                if(sMSRequest is null)
                    return new DataResponse<SMSResponseDto> { IsDone = false, Message = "" };

                var token = (await _gEELoginService.Login()).Data?.Access_token;
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
            catch (Exception e)
            {
                return new DataResponse<SMSResponseDto> { IsDone = false, Message = e.Message + e.InnerException?.Message };
            }
        }


    }
}
