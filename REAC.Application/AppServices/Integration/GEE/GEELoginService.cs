using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;
using System.Net.Http.Json;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class GEELoginService : IGEELoginService
    {
        private readonly ISysSettingService _sysSetting;

        public GEELoginService(ISysSettingService sysSetting)
        {
            _sysSetting = sysSetting;
        }

        public async Task<DataResponse<GEELoginResponseDto>> Login()
        {
            try
            {
                var settings = await GetLoginGEEData();
                if (!settings.IsDone || settings.Data is null)
                    return new DataResponse<GEELoginResponseDto>() { IsDone = false, Message = "" };

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, settings.Data.LoginURL.ToString());
                var collection = new List<KeyValuePair<string, string>>();
                collection.Add(new("grant_type", settings.Data.authorizationDto.grant_type));
                collection.Add(new("client_id", settings.Data.authorizationDto.client_id));
                collection.Add(new("client_secret", settings.Data.authorizationDto.client_secret));
                var content = new FormUrlEncodedContent(collection);
                request.Content = content;
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                if (response.IsSuccessStatusCode)
                {
                    var integrationResponse = await response.Content.ReadFromJsonAsync<GEELoginResponseDto>();
                    return new DataResponse<GEELoginResponseDto> { IsDone = true, Data = integrationResponse };
                }

                return new DataResponse<GEELoginResponseDto> { IsDone = false, Message = response.StatusCode.ToString() };
            }
            catch (Exception e)
            {
                return new DataResponse<GEELoginResponseDto> { IsDone = false, Message = e.Message + e.InnerException?.Message };
            }
        }


        private async Task<DataResponse<GEELoginDto>> GetLoginGEEData()
        {
            var settings = await _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.GEE_Key);

            if (settings is null || !settings.IsDone || settings.Data is null)
                return new DataResponse<GEELoginDto>() { IsDone = false, Message = "" };

            GEELoginDto loginDto = new GEELoginDto()
            {
                LoginURL = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.GEE_LoginURL).Value,
                authorizationDto = new GEELoginAuthorizationDto()
                {
                    client_id = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.GEE_client_id).Value,
                    client_secret = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.GEE_client_secret).Value,
                    grant_type = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.GEE_grant_type).Value,
                },
            };
            return new DataResponse<GEELoginDto> { Data = loginDto, IsDone = true };
        }
    }
}
