using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using System.Net.Http.Json;
using System.Net;
using System.Globalization;
using AutoMapper.Internal;
using Newtonsoft.Json;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class MCService : IMCService
    {
        private readonly ISysSettingService _sysSetting;
        private readonly IGEELoginService _gEELoginService;

        public MCService(ISysSettingService sysSetting, IGEELoginService gEELoginService)
        {
            _sysSetting = sysSetting;
            _gEELoginService = gEELoginService;
        }

        
        public async Task<DataResponse<CRInformationByCRNumberUpdatedDto>> GetCRInformationByCRNumber(string crNumber)
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new DataResponse<CRInformationByCRNumberUpdatedDto> { IsDone = false, Message = "" };

                var urlFromSetting = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.MC_GetCRInformationByCRNumber_URL))?.Data?.Value;
                if (string.IsNullOrWhiteSpace(urlFromSetting))
                    return new DataResponse<CRInformationByCRNumberUpdatedDto>() { IsDone = false, Message = "" };
                
                string url = string.Format(urlFromSetting, crNumber);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);

                MCCRInformationByCRNumberResponseDto responseDto = new MCCRInformationByCRNumberResponseDto();
                CRInformationByCRNumberUpdatedDto cRInformationByCRNumberUpdatedDto = new CRInformationByCRNumberUpdatedDto();
                if (response.IsSuccessStatusCode)
                {
                    responseDto.Data = await response.Content.ReadFromJsonAsync<CRInformationByCRNumberResponseDataDto>();
                    cRInformationByCRNumberUpdatedDto.Data = await MapCRInformationByCRNumber(responseDto);
                    return new DataResponse<CRInformationByCRNumberUpdatedDto> { IsDone = true, Data = cRInformationByCRNumberUpdatedDto };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    cRInformationByCRNumberUpdatedDto.UnauthorizedError = await response.Content.ReadFromJsonAsync<UnauthorizedErrorDto>();
                    return new DataResponse<CRInformationByCRNumberUpdatedDto> { IsDone = false, Data = cRInformationByCRNumberUpdatedDto };
                }
                else
                {
                    cRInformationByCRNumberUpdatedDto.Error = await response.Content.ReadFromJsonAsync<GeneralGEEErrorDto>();
                    return new DataResponse<CRInformationByCRNumberUpdatedDto> { IsDone = false, Data = cRInformationByCRNumberUpdatedDto };
                }
            }
            catch (Exception e)
            {
                return new DataResponse<CRInformationByCRNumberUpdatedDto> { IsDone = false, Message = e.Message + e.InnerException?.Message };
            }
        }


        public async Task<DataResponse<CRInformationByCROwnersUpdatedDto>> GetCRInformationByCROwners(string crNumber)
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new DataResponse<CRInformationByCROwnersUpdatedDto>() { IsDone = false, Message = "" };

                var urlFromSetting = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.MC_GetCRInformationByCROwners_URL))?.Data?.Value;
                if (string.IsNullOrWhiteSpace(urlFromSetting))
                    return new DataResponse<CRInformationByCROwnersUpdatedDto>() { IsDone = false, Message = "" };
                
                string url = string.Format(urlFromSetting, crNumber);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);
                var x = await response.Content.ReadAsStringAsync();

                MCCRInformationByCROwnersResponseDto responseDto = new MCCRInformationByCROwnersResponseDto();
                CRInformationByCROwnersUpdatedDto cRInformationByCROwnersUpdatedDto = new CRInformationByCROwnersUpdatedDto();
                if (response.IsSuccessStatusCode)
                {
                    responseDto.Data = await MapResponseData(await response.Content.ReadAsStringAsync());
                    //responseDto.Data = await response.Content.ReadFromJsonAsync<CRInformationByCROwnersResponseDataDto>();
                    if(responseDto.Data != null)
                    {
                        foreach (var item in responseDto.Data.GetCROwnersByCRNumberResponse.GetCROwnersByCRNumberResult.getCROwnersByCRNumberResponseDetailObject.CROwnersList.CROwner)
                        {
                            cRInformationByCROwnersUpdatedDto.Data?.Add(item.OwnerID?.PersonIdentifier?.NationalID);
                        }
                    }
                    return new DataResponse<CRInformationByCROwnersUpdatedDto> { IsDone = true, Data = cRInformationByCROwnersUpdatedDto };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    cRInformationByCROwnersUpdatedDto.UnauthorizedError = await response.Content.ReadFromJsonAsync<UnauthorizedErrorDto>();
                    return new DataResponse<CRInformationByCROwnersUpdatedDto> { IsDone = false, Data = cRInformationByCROwnersUpdatedDto };
                }
                else
                {
                    cRInformationByCROwnersUpdatedDto.Error = await response.Content.ReadFromJsonAsync<GeneralGEEErrorDto>();
                    return new DataResponse<CRInformationByCROwnersUpdatedDto> { IsDone = false, Data = cRInformationByCROwnersUpdatedDto };
                }
            }
            catch (Exception e)
            {
                return new DataResponse<CRInformationByCROwnersUpdatedDto> { IsDone = false, Message = e.Message + e.InnerException?.Message };
            }
        }

        private async Task<CRInformationByCROwnersResponseDataDto> MapResponseData(string response)
        {
            CRInformationByCROwnersResponseDataDto responseData = new CRInformationByCROwnersResponseDataDto();
            try
            {
                responseData = JsonConvert.DeserializeObject<CRInformationByCROwnersResponseDataDto>(response);
                return responseData;
            }
            catch (Exception)
            {
                TestCRInformationByCROwnersResponseDataDto testResponse = new TestCRInformationByCROwnersResponseDataDto();
                testResponse = JsonConvert.DeserializeObject<TestCRInformationByCROwnersResponseDataDto>(response);


                var cROwnerList = new List<CROwnerDto>();
                var cROwner = new CROwnerDto()
                {
                    OwnerID = new OwnerIDDto()
                    {
                        PersonIdentifier = new PersonIdentifierDto()
                        {
                            NationalID = testResponse.GetCROwnersByCRNumberResponse.GetCROwnersByCRNumberResult
                            .getCROwnersByCRNumberResponseDetailObject.CROwnersList.CROwner.OwnerID.PersonIdentifier.NationalID,
                        },
                    },
                };
                cROwnerList.Add(cROwner);

                responseData = new CRInformationByCROwnersResponseDataDto()
                {
                    GetCROwnersByCRNumberResponse = new GetCROwnersByCRNumberResponseDto()
                    {
                        GetCROwnersByCRNumberResult = new GetCROwnersByCRNumberResultDto()
                        {
                            getCROwnersByCRNumberResponseDetailObject = new GetCROwnersByCRNumberResponseDetailObjectDto()
                            {
                                CROwnersList = new CROwnersListDto()
                                {
                                    CROwner = cROwnerList,
                                }
                            }
                        },
                    },
                        //testResponse.TestGetCROwnersByCRNumberResponse
                };
                return responseData;
            }
        }

        private async Task<CRInformationByCRNumberDto> MapCRInformationByCRNumber(MCCRInformationByCRNumberResponseDto mCResponseDto)
        {
            var cRExpiryHijriDate = DateTime.ParseExact(mCResponseDto.Data.GetCRInformationByCRNumberResponse.GetCRInformationByCRNumberResult.getCRInformationByCRNumberResponseDetailObject.CRExpiryHijriDate
                ,"dd-MM-yyyy",CultureInfo.GetCultureInfo("ar-sa"),DateTimeStyles.None);
            
            var cRIssueHijriDate = DateTime.ParseExact(mCResponseDto.Data.GetCRInformationByCRNumberResponse.GetCRInformationByCRNumberResult.getCRInformationByCRNumberResponseDetailObject.CRIssueHijriDate
                , "dd-MM-yyyy", CultureInfo.GetCultureInfo("ar-SA"), DateTimeStyles.None);

            return new CRInformationByCRNumberDto()
            {
                CRNumber = mCResponseDto.Data.GetCRInformationByCRNumberResponse.GetCRInformationByCRNumberResult.getCRInformationByCRNumberResponseDetailObject.CRNumber,
                CRAddress = mCResponseDto.Data.GetCRInformationByCRNumberResponse.GetCRInformationByCRNumberResult.getCRInformationByCRNumberResponseDetailObject.CRAddress,
                CRName = mCResponseDto.Data.GetCRInformationByCRNumberResponse.GetCRInformationByCRNumberResult.getCRInformationByCRNumberResponseDetailObject.CRNameAR,
                CRNationalNumber = mCResponseDto.Data.GetCRInformationByCRNumberResponse.GetCRInformationByCRNumberResult.getCRInformationByCRNumberResponseDetailObject.CRNationalNumber,
                CRStatus = mCResponseDto.Data.GetCRInformationByCRNumberResponse.GetCRInformationByCRNumberResult.getCRInformationByCRNumberResponseDetailObject.CRStatus,
                CRIssueDate = cRIssueHijriDate,
                CRExpiryDate = cRExpiryHijriDate,
            };
        }

    }
}
