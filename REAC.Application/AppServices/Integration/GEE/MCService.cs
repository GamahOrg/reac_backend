using Core.Shared.Responses;
using Newtonsoft.Json;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;

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
                    if (responseDto.Data != null)
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


        public async Task<DataResponse<CRInformationByCRNumberDto>> GetCRNationalNumberInformation(string CRNationalNumber)
        {
            DataResponse<CRInformationByCRNumberDto> dataResponse = new DataResponse<CRInformationByCRNumberDto>();

            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new DataResponse<CRInformationByCRNumberDto> { IsDone = false, Message = "حدث خطأ أثناء التحقق" };

                var urlFromSetting = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.MC_GetCRBasicInfo_URL))?.Data?.Value;
                if (string.IsNullOrWhiteSpace(urlFromSetting))
                    return new DataResponse<CRInformationByCRNumberDto> { IsDone = false, Message = "حدث خطأ أثناء التحقق" };

                string url = urlFromSetting;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Headers.Add("Custom-CRNationalNumber", CRNationalNumber);
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var basicInfoDto = await response.Content.ReadFromJsonAsync<MCGEECRBasicInfoDto>();

                    if (basicInfoDto == null)
                        return new DataResponse<CRInformationByCRNumberDto> { Message = "البيانات المدخلة غير صحيحة" };

                    if (basicInfoDto.yefiError != null)
                    {
                        return new DataResponse<CRInformationByCRNumberDto>() { IsDone = false, Message = basicInfoDto.yefiError.errorText };
                    }

                    if (basicInfoDto.YefiErrorResponse != null && basicInfoDto.YefiErrorResponse.yefiError != null)
                    {
                        return new DataResponse<CRInformationByCRNumberDto>() { IsDone = false, Message = basicInfoDto.YefiErrorResponse.yefiError.errorText };
                    }


                    if (basicInfoDto.crBasicInfo!.crStatus!.crStatusID!.Value != (int)CRStatusTypeEnum.Active)
                    {
                        return new DataResponse<CRInformationByCRNumberDto>() { IsDone = false, Message = "رقم السجل التجاري غير فعال" };
                    }

                    var cRInformationByCRNumberDto = await MapCRInformationByCRNumber(basicInfoDto.crBasicInfo!);

                    //if (cRInformationByCRNumberDto.CRExpiryDate < DateTime.Now)
                    //    return new DataResponse<CRInformationByCRNumberDto> { IsDone = false, Message = "رقم السجل التجاري منتهي" };


                    return new DataResponse<CRInformationByCRNumberDto>
                    {
                        IsDone = true,
                        Data = cRInformationByCRNumberDto
                    };
                }
            }
            catch (Exception e)
            {
                return new DataResponse<CRInformationByCRNumberDto> { IsDone = false, Message = e.Message + e.InnerException?.Message };
            }

            return dataResponse;
        }
        public async Task<BaseResponse> ValidateRelatedCRs(string crNationalNumber, string identifierTypeID, string identifierNo, string nationalityID)
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new BaseResponse { IsDone = false, Message = "حدث خطأ أثناء التحقق" };

                var urlFromSetting = (await _sysSetting.GetBySettingKey(SysSettingKeyConstants.MC_GetCRPersonRelated_URL))?.Data?.Value;
                if (string.IsNullOrWhiteSpace(urlFromSetting))
                    return new BaseResponse { IsDone = false, Message = "حدث خطأ أثناء التحقق" };

                string url = urlFromSetting;

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                request.Headers.Add("Custom-IdentifierTypeID", identifierTypeID.ToString());
                request.Headers.Add("Custom-IdentifierNo", identifierNo.ToString());
                request.Headers.Add("Custom-nationalityID", nationalityID.ToString());

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var personRelatedCRsDto = await response.Content.ReadFromJsonAsync<MCGEEPersonRelatedCRsDto>();

                    if (personRelatedCRsDto == null)
                        return new BaseResponse { Message = "البيانات المدخلة غير صحيحة" };

                    if (personRelatedCRsDto.YefiErrorResponse != null)
                        return new BaseResponse { IsDone = true, Message = personRelatedCRsDto?.YefiErrorResponse?.yefiError?.errorText ?? "خطأ اُثناء التحقق من الملاك" };


                    if (personRelatedCRsDto.relatedCRList == null || (personRelatedCRsDto.relatedCRList != null && (!personRelatedCRsDto.relatedCRList.Any() || !personRelatedCRsDto.relatedCRList.Any(cr => cr.crBasicInfo.crNationalNumber == crNationalNumber))))
                        return new BaseResponse { IsDone = false, Message = "ليس أحد ملاك المنشاة تأكد من البيانات المدخلة" };

                    return new BaseResponse { IsDone = true };
                }
            }
            catch (Exception e)
            {
                return new BaseResponse { Message = e.Message + e.InnerException?.Message };
            }


            return new BaseResponse { Message = "البيانات المدخلة غير صحيحة" };
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
                , "dd-MM-yyyy", CultureInfo.GetCultureInfo("ar-sa"), DateTimeStyles.None);

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

        private async Task<CRInformationByCRNumberDto> MapCRInformationByCRNumber(CrBasicInfo crBasicInfo)
        {
            var commercialRegisteStartDate = DateTime.ParseExact(crBasicInfo.crIssueDate.crIssueDateGregorian, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var commercialRegisteEndDate = DateTime.ParseExact(crBasicInfo.crConfirmDate.crConfirmDateGregorian, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            CRInformationByCRNumberDto cRInformationByCRNumberDto = new CRInformationByCRNumberDto
            {
                CRNumber = crBasicInfo.crNumber,
                CRNationalNumber = crBasicInfo.crNationalNumber,
                CRName = crBasicInfo.entityFullNameAr,
                CRExpiryDate = commercialRegisteEndDate,
                CRIssueDate = commercialRegisteStartDate
            };


            return cRInformationByCRNumberDto;
        }

    }
}
