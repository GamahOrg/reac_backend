using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;


namespace REAC.Application.AppServices.Integration.GEE
{
    public class MOJService : IMOJService
    {
        private readonly ISysSettingService _sysSetting;
        private readonly IGEELoginService _gEELoginService;

        public MOJService(ISysSettingService sysSetting, IGEELoginService gEELoginService)
        {
            _sysSetting = sysSetting;
            _gEELoginService = gEELoginService;
        }

        public async Task<DataResponse<MOJResponseDto>> ExternalAttorneyInquiry(MOJRequestDto mOJRequest)
        {
            try
            {
                if (mOJRequest is null)
                    return new DataResponse<MOJResponseDto> { IsDone = false, Message = "" };

                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new DataResponse<MOJResponseDto> { IsDone = false, Message = "" };

                var settings = await _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.MOJ_Key);
                if (!settings.IsDone || settings.Data is null)
                    return new DataResponse<MOJResponseDto>() { IsDone = false, Message = "" };

                var urlFromSetting = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.MOJ_URL)?.Value;
                if (string.IsNullOrWhiteSpace(urlFromSetting))
                    return new DataResponse<MOJResponseDto> { IsDone = false, Message = "" };

                string url = string.Format(urlFromSetting, mOJRequest.AttorneyCode, mOJRequest.FirstParties);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);

                MOJResponseDto responseDto = new MOJResponseDto();
                if (response.IsSuccessStatusCode)
                {
                    responseDto.Data = await response.Content.ReadFromJsonAsync<ResponseObjectDto>();
                    var verifyPowerOfAttorney = await VerifyPowerOfAttorney(responseDto.Data, mOJRequest);
                    if (verifyPowerOfAttorney.IsDone)
                        responseDto.IsValid = true;
                    return new DataResponse<MOJResponseDto> { IsDone = true, Data = responseDto };

                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    responseDto.UnauthorizedError = await response.Content.ReadFromJsonAsync<UnauthorizedErrorDto>();
                    return new DataResponse<MOJResponseDto> { IsDone = false, Data = responseDto };
                }
                else
                {
                    responseDto.Error = await response.Content.ReadFromJsonAsync<GeneralGEEErrorDto>();
                    return new DataResponse<MOJResponseDto> { IsDone = false, Data = responseDto };
                }
            }
            catch (Exception e)
            {
                return new DataResponse<MOJResponseDto> { IsDone = false, Message = e.Message + e.InnerException?.Message };
            }
        }

        private async Task<BaseResponse> VerifyPowerOfAttorney(ResponseObjectDto responseObjectDto, MOJRequestDto mOJRequestDto)
        {
            if (responseObjectDto is null || mOJRequestDto is null || responseObjectDto.responseObject.Results is null)
                return new BaseResponse() { IsDone = false, Message = "" };

            var powerOfAttorney = responseObjectDto.responseObject.Results.FirstOrDefault(x => x.Model.Code == mOJRequestDto.AttorneyCode);
            if (powerOfAttorney is not null)
            {
                var firstPartiesIsExist = powerOfAttorney.Model.FirstParties.Any(x => x.IdentityNo == mOJRequestDto.FirstParties);
                var secondPartiesIsExist = powerOfAttorney.Model.SecondParties.Any(x => x.IdentityNo == mOJRequestDto.SecondParties);

                CultureInfo provider = new CultureInfo("en-US");
                CultureInfo.CurrentCulture = provider;
                var endDateGreg = DateTime.ParseExact(powerOfAttorney.Model.EndDateGreg, "M/d/yyyy h:mm:ss tt", provider);
                var mOJControlID = powerOfAttorney.Model.TextsValuesList.Any(x => x.ControlID == SysSettingKeyConstants.MOJControlId);

                if (firstPartiesIsExist && secondPartiesIsExist && endDateGreg > DateTime.Now && mOJControlID)
                    return new BaseResponse() { IsDone = true, Message = "" };
            }

            return new BaseResponse() { IsDone = false, Message = "" };
        }

    }
}
