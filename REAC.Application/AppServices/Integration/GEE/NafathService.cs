using AutoMapper.Internal;
using Azure.Core;
using Core.Dtos;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Security.Contracts;
using Core.Security.Entities;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using REAC.Application.AppServices.Integration.Hubs;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Integration.Nafath;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.UserManage;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.ModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Integration;
using REAC.EntityFrameworkCore.Entities.System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using static Dapper.SqlMapper;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace REAC.Application.AppServices.Integration.GEE
{
    public class NafathService : INafathService
    {
        private readonly ISysSettingService _sysSetting;
        private readonly IAccountService _accountService;
        private readonly IEntityRepository _repository;
        private readonly IGEELoginService _gEELoginService;
        private readonly IHubContext<IdentityHub, IIdentityClient> _identityHub;
        private readonly IHubContext<LitigationRequestAgreementHub, ILitigationRequestAgreementStatusClient> _litigationRequestAgreementHub;
        private readonly INafathTransactionService _nafathTransactionService;
        private readonly INafathAgreementRequestService _nafathAgreementRequestService;
        private readonly ILitigationRequestAgreementRequestService _agreementRequestService;
        private readonly UserManager<User> _userManager;
        private readonly ILoggerManager _loggerManager;
        private readonly INafathReconciliationReportService _nafathReconciliationReportService;
        private readonly ILitigationRequestReconciliationReportService _litigationRequestReconciliationReportService;
        private readonly IHubContext<NafathReconciliationReportHub, INafathReconciliationReportStatus> _nafathReconciliationReportHub;
        public NafathService(ISysSettingService sysSetting, IAccountService accountService, IEntityRepository repository, IGEELoginService gEELoginService, IHubContext<IdentityHub, IIdentityClient> identityHub,
            INafathTransactionService nafathTransactionService, INafathAgreementRequestService nafathAgreementRequestService, UserManager<User> userManager, ILoggerManager loggerManager, IHubContext<LitigationRequestAgreementHub, ILitigationRequestAgreementStatusClient> litigationRequestAgreementHub, ILitigationRequestAgreementRequestService agreementRequestService, INafathReconciliationReportService nafathReconciliationReportService, IHubContext<NafathReconciliationReportHub, INafathReconciliationReportStatus> nafathReconciliationReportHub, ILitigationRequestReconciliationReportService litigationRequestReconciliationReportService)
        {
            _sysSetting = sysSetting;
            _accountService = accountService;
            _repository = repository;
            _gEELoginService = gEELoginService;
            _identityHub = identityHub;
            _nafathTransactionService = nafathTransactionService;
            _nafathAgreementRequestService = nafathAgreementRequestService;
            _userManager = userManager;
            _loggerManager = loggerManager;
            _litigationRequestAgreementHub = litigationRequestAgreementHub;
            _agreementRequestService = agreementRequestService;
            _nafathReconciliationReportService = nafathReconciliationReportService;
            _nafathReconciliationReportHub = nafathReconciliationReportHub;
            _litigationRequestReconciliationReportService = litigationRequestReconciliationReportService;
        }

        public async Task<DataResponse<NafathResponseDto>> Nafath(NafathRequestDto nafathRequestDto, CancellationToken cancellationToken)
        {
            try
            {
                if (nafathRequestDto is null)
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = "" };

                string? accessToken = null;
                string? refreshToken = null;
                bool isDraftAccount = false;
                bool isArbitrator = false;
                bool isReformer = false;
                bool isLawyer = false;

                var existCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                    .FirstOrDefault(x => x.IdentityNumber == nafathRequestDto.NationalId);

                if (existCitizen is not null)
                {
                    isArbitrator = existCitizen.IsArbitrator ?? false;
                    isReformer = existCitizen.IsReformer ?? false;
                    isLawyer = existCitizen.IsLawyer ?? false;
                    isDraftAccount = existCitizen.IsDraftAccount ?? false;
                }

                if (!nafathRequestDto.IsRegister && (existCitizen is not null && !isDraftAccount))
                {
                    UserLoginDto UserLogin = new UserLoginDto()
                    { UserName = nafathRequestDto.NationalId, Password = "123456" };
                    var login = await _accountService.LoginAsync(UserLogin, cancellationToken);
                    if (!login.IsDone || login == null || login.Data == null)
                        return new DataResponse<NafathResponseDto> { IsDone = false, Message = login?.Message ?? string.Empty };

                    accessToken = login.Data.AccessToken;
                    refreshToken = login.Data.RefreshToken;
                }

                if (!nafathRequestDto.IsRegister && existCitizen is null && !isDraftAccount)
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = "هذا المستخدم غير مسجل بالنظام" };

                if (nafathRequestDto.IsRegister)
                {
                    var isExistUser = _userManager.Users.Any(x => x.UserName == nafathRequestDto.NationalId);
                    if (!isDraftAccount && isExistUser)
                        return new DataResponse<NafathResponseDto> { IsDone = false, Message = "المستخدم موجود بالفعل" };
                }

                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = "" };

                var requestBody = await GetNafathSettingsData(nafathRequestDto.NationalId);
                if (!requestBody.IsDone || requestBody.Data is null)
                    return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "" };

                var url = _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.Nafath_URL).Result.Data?.FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(url))
                    return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "" };

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var body = JsonConvert.SerializeObject(requestBody.Data);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);

                var x = response.Content.ReadAsStringAsync();
                NafathResponseDto? nafathResponse = new NafathResponseDto();
                if (response.IsSuccessStatusCode)
                {
                    nafathResponse.Data = await response.Content.ReadFromJsonAsync<NafathResponseDataDto>();
                    nafathResponse.IsDraftAccount = isDraftAccount;
                    //if(nafathResponse.Data is null || nafathResponse.Data.TransId is null || nafathResponse.Data.Random is null)
                    //{
                    //    var nafathTransaction = _nafathTransactionService.GettAll().FirstOrDefault(x => x.NationalId == nafathRequestDto.NationalId);
                    //    NafathResponseDataDto nafathResponseData = new NafathResponseDataDto()
                    //    {
                    //        Random = nafathTransaction.RandomNumber,
                    //        TransId = nafathTransaction.TransactionId,
                    //    };
                    //    nafathResponse.Data = nafathResponseData;
                    //    return new DataResponse<NafathResponseDto> { IsDone = true, Data = nafathResponse };

                    //}
                    if (nafathResponse.Data is not null && nafathResponse.Data.Random is not null && nafathResponse.Data.TransId is not null)
                    {
                        NafathTransaction nafathTransaction = new NafathTransaction()
                        {
                            AccessToken = accessToken,
                            RefreshToken = refreshToken,
                            NationalId = nafathRequestDto.NationalId,
                            RandomNumber = nafathResponse.Data.Random,
                            TransactionId = nafathResponse.Data.TransId,
                            RequestExpired = DateTime.Now.AddMinutes(2),
                            IsDraftAccount = isDraftAccount,
                            IsReformer = isReformer,
                            IsArbitrator = isArbitrator,
                            IsLawye = isLawyer,
                        };
                        _nafathTransactionService.Add(nafathTransaction);
                    }
                    return new DataResponse<NafathResponseDto> { IsDone = true, Data = nafathResponse };
                }
                else
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        nafathResponse.Error = await response.Content.ReadFromJsonAsync<NafathResponseErrorDto>();
                        nafathResponse.IsDraftAccount = isDraftAccount;
                    }

                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = nafathResponse.Error?.Message ?? nafathResponse.Error?.Code ?? "", Data = nafathResponse };
                }
            }
            catch (Exception e)
            {
                return new DataResponse<NafathResponseDto>() { IsDone = false, Message = e.Message + e.InnerException };
            }
        }


        public async Task CheckNafathStatus()
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                           Message = "get access token from API GEE error"
                       });

                var nafathCheckList = _nafathTransactionService.GettAll();
                var removeList = new List<NafathTransaction>();
                if (nafathCheckList is not null && nafathCheckList.Any())
                {
                    foreach (var nafathTransaction in nafathCheckList)
                    {
                        if (nafathTransaction.RequestExpired < DateTime.Now)
                        {
                            IdentityStatus identityStatus = new IdentityStatus()
                            {
                                TransactionId = nafathTransaction.TransactionId,
                                IsValid = false,
                                Token = null,
                                UserInfoUpdated = null,
                            };
                            removeList.Add(nafathTransaction);
                            await _identityHub.Clients.Group(identityStatus.TransactionId).ReceivedIdentityStatus(identityStatus);
                            continue;
                        }

                        NafathCheckRequestDto nafathCheckRequestDto = new NafathCheckRequestDto()
                        {
                            Id = nafathTransaction.NationalId,
                            Random = nafathTransaction.RandomNumber,
                            TransId = nafathTransaction.TransactionId,
                            Token = token
                        };

                        var nafathCheckStatusResponse = (await NafathCheckStatus(nafathCheckRequestDto));
                        if (nafathCheckStatusResponse is not null && nafathCheckStatusResponse.Data is not null)
                        {
                            if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_COMPLETED
                                && nafathCheckStatusResponse.Data?.Data?.AccessToken is not null)
                            {
                                if (nafathTransaction.AccessToken is not null && nafathTransaction.RefreshToken is not null)
                                {
                                    IdentityStatus identityStatusWithToken = new IdentityStatus()
                                    {
                                        TransactionId = nafathTransaction.TransactionId,
                                        IsValid = true,
                                        Token = nafathTransaction.AccessToken,
                                        RefreshToken = nafathTransaction.RefreshToken,
                                        UserInfoUpdated = new UserInfoUpdatedDto()
                                        {
                                            IsDraftAccount = nafathTransaction.IsDraftAccount,
                                            IsLawye = nafathTransaction.IsLawye,
                                            IsArbitrator = nafathTransaction.IsArbitrator,
                                            IsReformer = nafathTransaction.IsReformer,
                                        }
                                    };
                                    await _identityHub.Clients.Group(identityStatusWithToken.TransactionId).ReceivedIdentityStatus(identityStatusWithToken);

                                    removeList.Add(nafathTransaction);
                                    continue;
                                }

                                IdentityStatus identityStatus = new IdentityStatus();

                                var userInfo = await NafathGetUserInfo(nafathCheckStatusResponse.Data.Data.AccessToken, token);

                                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                , new LogDto()
                                {
                                    Type = Core.Log.Domain.Shared.Enums.LogType.Information,
                                    Message = $"User Info ==> {JsonSerializer.Serialize(userInfo)}"
                                });

                                if (!string.IsNullOrEmpty(token) && nafathCheckRequestDto is not null)
                                {
                                    if (!string.IsNullOrEmpty(nafathCheckRequestDto.Id))
                                    {
                                        var userAddressInfo = await NafathGetUserAddressInfo(token, nafathCheckRequestDto.Id);

                                        if (userAddressInfo is not null)
                                        {
                                            if (userAddressInfo.IsDone)
                                            {
                                                identityStatus.UserAddressInfoUpdated = userAddressInfo.Data;

                                            }
                                        }
                                    }

                                }


                                if (userInfo.IsDone)
                                {

                                    identityStatus.TransactionId = nafathTransaction.TransactionId;
                                    identityStatus.IsValid = true;
                                    identityStatus.Token = nafathTransaction.AccessToken;
                                    identityStatus.RefreshToken = nafathTransaction.RefreshToken;
                                    identityStatus.UserInfoUpdated = new UserInfoUpdatedDto()
                                    {
                                        IsDraftAccount = nafathTransaction.IsDraftAccount,
                                        IsLawye = nafathTransaction.IsLawye,
                                        IsArbitrator = nafathTransaction.IsArbitrator,
                                        IsReformer = nafathTransaction.IsReformer,
                                        DateOfBirth = userInfo.Data?.DateOfBirth,
                                        FirstName = userInfo.Data?.FirstName,
                                        FullName = userInfo.Data?.FullName,
                                        Gender = userInfo.Data?.Gender,
                                        NationalId = userInfo.Data?.NationalId,
                                        NationalIdExpiryDate = userInfo.Data?.NationalIdExpiryDate,
                                        Nationality = userInfo.Data?.Nationality,
                                        NationalityId = userInfo.Data?.NationalityId,
                                    };

                                    await _identityHub.Clients.Group(identityStatus.TransactionId).ReceivedIdentityStatus(identityStatus);

                                    removeList.Add(nafathTransaction);
                                    continue;
                                }

                            }

                            if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_REJECTED
                            || nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_EXPIRED)
                            {
                                IdentityStatus identityStatus = new IdentityStatus()
                                {
                                    TransactionId = nafathTransaction.TransactionId,
                                    IsValid = false,
                                };
                                await _identityHub.Clients.Group(identityStatus.TransactionId).ReceivedIdentityStatus(identityStatus);

                                removeList.Add(nafathTransaction);
                                continue;
                            }
                        }
                    }

                    if (removeList.Any())
                    {
                        foreach (var nafathTransaction in removeList)
                        {
                            _nafathTransactionService.Remove(nafathTransaction);
                        }
                    }
                }

            }
            catch (Exception)
            {
            }

        }

        private async Task<DataResponse<UserInfoUpdatedDto>> NafathGetUserInfo(string nafathToken, string loginToken)
        {
            try
            {
                var url = _sysSetting.GetBySettingKey(SysSettingKeyConstants.NafathGetUserInfo_URL).Result.Data?.Value;
                var enableNafathToken = bool.Parse(_sysSetting.GetBySettingKey(SysSettingKeyConstants.GEENafathEnableNafathToken_Key).Result.Data?.Value);

                if (string.IsNullOrEmpty(nafathToken) || string.IsNullOrEmpty(loginToken) || string.IsNullOrEmpty(url))
                    return new DataResponse<UserInfoUpdatedDto>() { IsDone = false, Message = "" };

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);

                if (enableNafathToken)
                {
                    request.Headers.Add("Authorization", $"Bearer {loginToken}");
                    request.Headers.Add("nafath-token", $"{nafathToken}");
                }
                else
                {
                    request.Headers.Add("Authorization", $"Bearer {nafathToken}");
                }
                //request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var jwtToken = await response.Content.ReadAsStringAsync();
                    var handler = new JwtSecurityTokenHandler();
                    var readJwtToken = handler.ReadJwtToken(jwtToken);
                    var serializePayloadToJsonObj = readJwtToken.Payload.SerializeToJson();
                    //var userInfo = JsonSerializer.Deserialize<NafathUserInfoDto>(serializePayloadToJsonObj);
                    var userInfo = JsonConvert.DeserializeObject<NafathUserInfoDto>(serializePayloadToJsonObj);

                    var userInfoUpdated = await MapUserInfoDataAsync(userInfo);

                    if (!userInfoUpdated.IsDone)
                        return new DataResponse<UserInfoUpdatedDto> { IsDone = true, Message = "خطأ في استخراج بيانات العميل" };


                    return new DataResponse<UserInfoUpdatedDto> { IsDone = true, Data = userInfoUpdated.Data };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new DataResponse<UserInfoUpdatedDto> { IsDone = false, Message = "غير مصرح" };
                else
                    return new DataResponse<UserInfoUpdatedDto> { IsDone = false, Message = "حدث خطأ بالاتصال بنفاذ" };
            }
            catch (Exception e)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                               , new LogDto()
                               {
                                   Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                   Message = $"User Info Exception ==> {e}"
                               });

                return new DataResponse<UserInfoUpdatedDto>() { IsDone = false, Message = e.Message + e.InnerException };
            }
        }

        private async Task<DataResponse<List<UserAddressInfoUpdatedDto>>> NafathGetUserAddressInfo(string token, string identiyNumber)
        {
            try
            {
                var url = _sysSetting.GetBySettingKey(SysSettingKeyConstants.NafathGetUserAddressInfo_URL).Result.Data?.Value;

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(url))
                    return new DataResponse<List<UserAddressInfoUpdatedDto>> { IsDone = false, Message = "" };

                url = string.Format(url, identiyNumber);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    //var userAddressInfo = JsonSerializer.Deserialize<NafathUserAddressInfo>(content);
                    var userAddressInfo = JsonConvert.DeserializeObject<NafathUserAddressInfo>(content);
                    var userAddressInfoUpdated = await MapUserAddressInfoDataAsync(userAddressInfo.GetIndividualWaselAddressResponse.GetIndividualWaselAddressResult.getIndividualWaselAddressResponseDetailObject.WaselAddress);
                    return new DataResponse<List<UserAddressInfoUpdatedDto>> { IsDone = true, Data = userAddressInfoUpdated.Data };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new DataResponse<List<UserAddressInfoUpdatedDto>> { IsDone = false, Message = "" };
                else
                    return new DataResponse<List<UserAddressInfoUpdatedDto>> { IsDone = false, Message = "" };
            }
            catch (Exception e)
            {
                return new DataResponse<List<UserAddressInfoUpdatedDto>> { IsDone = false, Message = e.Message + e.InnerException };
            }
        }
        private async Task<DataResponse<UserInfoUpdatedDto>> MapUserInfoDataAsync(NafathUserInfoDto nafathUserInfoDto)
        {
            var nationality = (await _repository.GetQueryable<SysNationality, Guid>()).FirstOrDefault(x => x.NationalityCode == nafathUserInfoDto.user_info.nationality);

            var nationalityId = Guid.NewGuid();
            if (nationality == null)
            {
                SysNationality sysNationality = new SysNationality()
                {
                    Id = Guid.NewGuid(),
                    NationalityCode = nafathUserInfoDto.user_info.nationality,
                    NationalityName = nafathUserInfoDto.user_info.nationality_ar,
                    CreatedBy = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    IsDeleted = false,
                };

                await _repository.AddAsync<SysNationality, Guid>(sysNationality);
                var result = await _repository.SaveChangesAsync();
                if (!result.IsDone) return new DataResponse<UserInfoUpdatedDto> { IsDone = false, Message = result.Message };

                nationalityId = sysNationality.Id;
            }

            _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                               , new LogDto()
                               {
                                   Type = Core.Log.Domain.Shared.Enums.LogType.Information,
                                   Message = $"Nafath User Info Dto ==> {JsonSerializer.Serialize(nafathUserInfoDto)}"
                               });

            return new DataResponse<UserInfoUpdatedDto>()
            {
                IsDone = true,
                Data = new UserInfoUpdatedDto()
                {
                    FullName = nafathUserInfoDto.user_info.full_name_ar,
                    FirstName = nafathUserInfoDto.user_info.first_name_ar,
                    NationalId = nafathUserInfoDto.user_info.id.ToString(),
                    DateOfBirth = nafathUserInfoDto.user_info.dob_g is not null ? DateTime.ParseExact(nafathUserInfoDto.user_info.dob_g.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                    NationalIdExpiryDate = nafathUserInfoDto.user_info.id_expiry_date_g is not null ?
                        DateTime.ParseExact(nafathUserInfoDto.user_info.id_expiry_date_g.ToString(), "yyyy-MM-dd", CultureInfo.InvariantCulture) : null,
                    Gender = nafathUserInfoDto.user_info.gender == "M" ? Domain.Shared.Enum.Gender.Male : Domain.Shared.Enum.Gender.Female,
                    NationalityId = nationality?.Id ?? nationalityId,
                    Nationality = nationality?.NationalityName ?? nafathUserInfoDto.user_info.nationality_ar,
                }
            };
        }

        private async Task<DataResponse<List<UserAddressInfoUpdatedDto>>> MapUserAddressInfoDataAsync(List<WaselAddress> nafathUserInfoDto)
        {
            List<UserAddressInfoUpdatedDto> arr = new List<UserAddressInfoUpdatedDto>();
            for (int i = 0; i < nafathUserInfoDto.Count(); i++)
            {
                var obj = new UserAddressInfoUpdatedDto()
                {
                    AdditionalNumber = nafathUserInfoDto[i].AdditionalNumber,
                    BuildingNumber = nafathUserInfoDto[i].BuildingNumber,
                    ZipCode = nafathUserInfoDto[i].ZipCode,
                    UnitNumber = nafathUserInfoDto[i].UnitNumber,
                    DistrictAreaArabic = nafathUserInfoDto[i].DistrictAreaArabic,
                    DistrictAreaEnglish = nafathUserInfoDto[i].DistrictAreaEnglish,
                    StreetNameArabic = nafathUserInfoDto[i].StreetNameArabic,
                    StreetNameEnglish = nafathUserInfoDto[i].StreetNameEnglish,
                    CityNameArabic = nafathUserInfoDto[i].CityNameArabic,
                    CityNameEnglish = nafathUserInfoDto[i].CityNameEnglish,
                    FullName = nafathUserInfoDto[i].FullName,
                    Latitude = nafathUserInfoDto[i].Latitude,
                    Longitude = nafathUserInfoDto[i].Longitude
                };
                arr.Add(obj);

            }
            return new DataResponse<List<UserAddressInfoUpdatedDto>>()
            {
                IsDone = true,
                Data = arr
                //Data = new UserAddressInfoUpdatedDto()
                //{
                //    AdditionalNumber = nafathUserInfoDto.AdditionalNumber,
                //    BuildingNumber = nafathUserInfoDto.BuildingNumber,
                //    ZipCode = nafathUserInfoDto.ZipCode,
                //    UnitNumber = nafathUserInfoDto.UnitNumber,
                //    DistrictAreaArabic = nafathUserInfoDto.DistrictAreaArabic,
                //    DistrictAreaEnglish = nafathUserInfoDto.DistrictAreaEnglish,
                //    StreetNameArabic = nafathUserInfoDto.StreetNameArabic,
                //    StreetNameEnglish = nafathUserInfoDto.StreetNameEnglish,
                //    CityNameArabic = nafathUserInfoDto.CityNameArabic,
                //    CityNameEnglish = nafathUserInfoDto.CityNameEnglish,
                //    FullName = nafathUserInfoDto.FullName,
                //    Latitude = nafathUserInfoDto.Latitude,
                //    Longitude = nafathUserInfoDto.Longitude,

                //}
            };
        }
        private async Task<DataResponse<NafathCheckResponseDto>> NafathCheckStatus(NafathCheckRequestDto nafathCheckRequestDto)
        {
            try
            {
                var settings = await _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.NafathCheck_Key);
                if (nafathCheckRequestDto is null || settings is null || !settings.IsDone || settings.Data is null)
                    return new DataResponse<NafathCheckResponseDto>() { IsDone = false, Message = "" };

                var url = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.NafathCheck_URL)?.Value;

                NafathCheckBodyDto nafathCheckBodyDto = new NafathCheckBodyDto()
                {
                    Action = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.NafathCheck_Action).Value,
                    Parameters = new NafathTransactionDto()
                    {
                        id = nafathCheckRequestDto.Id,
                        random = nafathCheckRequestDto.Random,
                        transId = nafathCheckRequestDto.TransId
                    },
                };


                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {nafathCheckRequestDto.Token}");
                var body = JsonConvert.SerializeObject(nafathCheckBodyDto);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var x = await response.Content.ReadAsStringAsync();
                NafathCheckResponseDto? nafathCheckResponse = new NafathCheckResponseDto();
                if (response.IsSuccessStatusCode)
                {
                    nafathCheckResponse.Data = await response.Content.ReadFromJsonAsync<NafathCheckResponseDataDto>();
                    return new DataResponse<NafathCheckResponseDto> { IsDone = true, Data = nafathCheckResponse };
                }
                else
                {
                    nafathCheckResponse.Error = await response.Content.ReadFromJsonAsync<NafathResponseErrorDto>();
                    return new DataResponse<NafathCheckResponseDto> { IsDone = false, Data = nafathCheckResponse };
                }
            }
            catch (Exception e)
            {
            _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
              , new LogDto()
              {
                  Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                  Message = $"Nafath Check Status ==> {e}"
              });
                return new DataResponse<NafathCheckResponseDto>() { IsDone = false, Message = e.Message + e.InnerException };
            }
        }

        private async Task<DataResponse<NafathRequestBodyDto>> GetNafathSettingsData(string nationalId)
        {
            var settings = await _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.Nafath_Key);

            if (settings is null || !settings.IsDone || settings.Data is null || string.IsNullOrEmpty(nationalId))
                return new DataResponse<NafathRequestBodyDto>() { IsDone = false, Message = "" };

            NafathRequestBodyDto nafathRequestBodyDto = new NafathRequestBodyDto()
            {
                Action = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.Nafath_Action).Value,
                Parameters = new NafathRequestParametersBodyDto()
                {
                    service = settings.Data.FirstOrDefault(x => x.Key == SysSettingKeyConstants.Nafath_Service).Value,
                    id = nationalId,
                },
            };
            return new DataResponse<NafathRequestBodyDto> { Data = nafathRequestBodyDto, IsDone = true };
        }

        public async Task<BaseResponse> CheckStatusForMobile(string transactionId)
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                           Message = "get access token from API GEE error"
                       });

                var nafathCheckItem = _nafathTransactionService.GettAll().FirstOrDefault(x => x.TransactionId == transactionId);

                if (nafathCheckItem is null)
                    return new BaseResponse() { IsDone = false, Message = "لا يوجد بيانات لهذا الرقم" };

                var removeList = new List<NafathTransaction>();


                if (nafathCheckItem.RequestExpired < DateTime.Now)
                {
                    IdentityStatus identityStatus = new IdentityStatus()
                    {
                        TransactionId = nafathCheckItem.TransactionId,
                        IsValid = false,
                        Token = null,
                        UserInfoUpdated = null,
                    };
                    removeList.Add(nafathCheckItem);
                    return new DataResponse<IdentityStatus>() { IsDone = true, Data = identityStatus, Message = "Token Expired" };
                }

                NafathCheckRequestDto nafathCheckRequestDto = new NafathCheckRequestDto()
                {
                    Id = nafathCheckItem.NationalId,
                    Random = nafathCheckItem.RandomNumber,
                    TransId = nafathCheckItem.TransactionId,
                    Token = token
                };

                var nafathCheckStatusResponse = (await NafathCheckStatus(nafathCheckRequestDto));


                if (nafathCheckStatusResponse is not null && nafathCheckStatusResponse.Data is not null)
                {
                    if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_COMPLETED
                        && nafathCheckStatusResponse.Data?.Data?.AccessToken is not null)
                    {
                        if (nafathCheckItem.AccessToken is not null && nafathCheckItem.RefreshToken is not null)
                        {
                            IdentityStatus identityStatusWithToken = new IdentityStatus()
                            {
                                TransactionId = nafathCheckItem.TransactionId,
                                IsValid = true,
                                Token = nafathCheckItem.AccessToken,
                                RefreshToken = nafathCheckItem.RefreshToken,
                                UserInfoUpdated = new UserInfoUpdatedDto()
                                {
                                    IsDraftAccount = nafathCheckItem.IsDraftAccount,
                                    IsLawye = nafathCheckItem.IsLawye,
                                    IsArbitrator = nafathCheckItem.IsArbitrator,
                                    IsReformer = nafathCheckItem.IsReformer,
                                }
                            };

                            removeList.Add(nafathCheckItem);
                            return new DataResponse<IdentityStatus>() { IsDone = true, Data = identityStatusWithToken };

                        }

                        IdentityStatus identityStatus = new IdentityStatus();

                        var userInfo = await NafathGetUserInfo(nafathCheckStatusResponse.Data.Data.AccessToken, token);

                        if (!userInfo.IsDone)
                            return new ExceptionResponse() { IsDone = false, Message = "خطأ في استخراج بيانات العميل" };

                        _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                               , new LogDto()
                               {
                                   Type = Core.Log.Domain.Shared.Enums.LogType.Information,
                                   Message = $"User Info ==> {JsonSerializer.Serialize(userInfo)}"
                               });

                        if (!string.IsNullOrEmpty(token) && nafathCheckRequestDto is not null)
                        {
                            if (!string.IsNullOrEmpty(nafathCheckRequestDto.Id))
                            {
                                var userAddressInfo = await NafathGetUserAddressInfo(token, nafathCheckRequestDto.Id);

                                if (userAddressInfo is not null)
                                {
                                    if (userAddressInfo.IsDone)
                                    {
                                        identityStatus.UserAddressInfoUpdated = userAddressInfo.Data;

                                    }
                                }
                            }

                        }


                        if (userInfo.IsDone)
                        {

                            identityStatus.TransactionId = nafathCheckItem.TransactionId;
                            identityStatus.IsValid = true;
                            identityStatus.Token = nafathCheckItem.AccessToken;
                            identityStatus.RefreshToken = nafathCheckItem.RefreshToken;
                            identityStatus.UserInfoUpdated = new UserInfoUpdatedDto()
                            {
                                IsDraftAccount = nafathCheckItem.IsDraftAccount,
                                IsLawye = nafathCheckItem.IsLawye,
                                IsArbitrator = nafathCheckItem.IsArbitrator,
                                IsReformer = nafathCheckItem.IsReformer,
                                DateOfBirth = userInfo.Data?.DateOfBirth,
                                FirstName = userInfo.Data?.FirstName,
                                FullName = userInfo.Data?.FullName,
                                Gender = userInfo.Data?.Gender,
                                NationalId = userInfo.Data?.NationalId,
                                NationalIdExpiryDate = userInfo.Data?.NationalIdExpiryDate,
                                Nationality = userInfo.Data?.Nationality,
                                NationalityId = userInfo.Data?.NationalityId,
                            };


                            removeList.Add(nafathCheckItem);
                            return new DataResponse<IdentityStatus>() { IsDone = true, Data = identityStatus };

                        }

                    }

                    if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_REJECTED
                    || nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_EXPIRED)
                    {
                        IdentityStatus identityStatus = new IdentityStatus()
                        {
                            TransactionId = nafathCheckItem.TransactionId,
                            IsValid = false,
                        };

                        removeList.Add(nafathCheckItem);
                        return new DataResponse<IdentityStatus>() { IsDone = true, Data = identityStatus };

                    }
                }

                if (removeList.Any())
                {
                    foreach (var nafathTransaction in removeList)
                    {
                        _nafathTransactionService.Remove(nafathTransaction);
                    }
                }
                return new BaseResponse() { IsDone = false, Message = "حدث خطأ او لم يتم تفعيل الرقم السري" };



            }
            catch (Exception e)
            {

                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                               , new LogDto()
                               {
                                   Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                   Message = $"Nafath Exception ==> {e}"
                               });

                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }


        #region Agreement Request Otp

        public async Task<DataResponse<NafathResponseDto>> NafathAgreementRequest(NafathRequestDto nafathRequestDto, NafathAgreementRequestTransactionsDto listInput)
        {
            try
            {
                if (nafathRequestDto is null)
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = "" };

                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = "" };

                var requestBody = await GetNafathSettingsData(nafathRequestDto.NationalId);
                if (!requestBody.IsDone || requestBody.Data is null)
                    return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "" };

                var url = _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.Nafath_URL).Result.Data?.FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(url))
                    return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "" };

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var body = JsonConvert.SerializeObject(requestBody.Data);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);

                var x = response.Content.ReadAsStringAsync();
                NafathResponseDto? nafathResponse = new NafathResponseDto();
                if (response.IsSuccessStatusCode)
                {
                    nafathResponse.Data = await response.Content.ReadFromJsonAsync<NafathResponseDataDto>();

                    if (nafathResponse.Data is not null && nafathResponse.Data.Random is not null && nafathResponse.Data.TransId is not null)
                    {
                        NafathAgreementRequestTransactionsDto nafathTransaction = new NafathAgreementRequestTransactionsDto()
                        {
                            LitigationRequestId = listInput.LitigationRequestId,
                            NationalId = listInput.NationalId,
                            AgreementStatus = listInput.AgreementStatus,
                            TransactionId = nafathResponse.Data.TransId,
                            //TransactionId = "456",
                            RandomNumber = nafathResponse.Data.Random,
                            //RandomNumber = "15",
                            RequestExpired = DateTime.Now.AddMinutes(2),
                            AgreementStatusOn = DateTime.Now
                        };
                        _nafathAgreementRequestService.Add(nafathTransaction);
                    }
                    //return new DataResponse<NafathResponseDto> { IsDone = true, Data = 
                    //    new NafathResponseDto()
                    //    {
                    //        Data = new NafathResponseDataDto()
                    //        {
                    //            Random = "15",
                    //            TransId = "456"
                    //        }
                    //    }
                    //    };

                    return new DataResponse<NafathResponseDto> { IsDone = true, Data = nafathResponse };
                }
                else
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        nafathResponse.Error = await response.Content.ReadFromJsonAsync<NafathResponseErrorDto>();
                    }

                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = nafathResponse.Error?.Message ?? nafathResponse.Error?.Code ?? "", Data = nafathResponse };
                }
            }
            catch (Exception e)
            {
                return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "خطأ في الاتصال بنفاذ" };
            }
        }


        public async Task CheckNafathAgreementRequestStatus()
        {
            try
            {
                

                var nafathCheckList = _nafathAgreementRequestService.GetAll();
                var removeList = new List<NafathAgreementRequestTransactionsDto>();
                if (nafathCheckList is not null && nafathCheckList.Any())
                {
                    var token = (await _gEELoginService.Login()).Data?.Access_token;
                    if (token is null)
                        _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                           , new LogDto()
                           {
                               Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                               Message = "get access token from API GEE error"
                           });

                    foreach (var nafathTransaction in nafathCheckList)
                    {
                        if (nafathTransaction.RequestExpired < DateTime.Now)
                        {

                            removeList.Add(nafathTransaction);
                            await _litigationRequestAgreementHub.Clients.Group(nafathTransaction.TransactionId).ReceivedLitigationRequestAgreementStatus(new BaseResponse() { IsDone = false, Message = "صلاحية الرقم منتهيه" });
                            continue;
                        }

                        NafathCheckRequestDto nafathCheckRequestDto = new NafathCheckRequestDto()
                        {
                            Id = nafathTransaction.NationalId,
                            Random = nafathTransaction.RandomNumber,
                            TransId = nafathTransaction.TransactionId,
                            Token = token
                        };

                        var nafathCheckStatusResponse = (await NafathCheckStatus(nafathCheckRequestDto));
                        //var nafathCheckStatusResponse = new DataResponse<NafathCheckResponseDto>()
                        //{
                        //    IsDone = true,
                        //    Data = new NafathCheckResponseDto() { Data = new NafathCheckResponseDataDto() { Status = SysSettingKeyConstants.NafathStatus_COMPLETED , AccessToken = "123" }, Error = null } 
                        //};
                        if (nafathCheckStatusResponse is not null && nafathCheckStatusResponse.Data is not null)
                        {
                            if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_COMPLETED
                                && nafathCheckStatusResponse.Data?.Data?.AccessToken is not null)
                            {
                                var addCitizenLitigationRequestForAgreementRequestReplyReturnValue = await _agreementRequestService.AddCitizenLitigationRequestForAgreementRequestReply(nafathTransaction.LitigationRequestId, nafathTransaction.NationalId, nafathTransaction.AgreementStatus);


                                await _litigationRequestAgreementHub.Clients.Group(nafathTransaction.TransactionId).ReceivedLitigationRequestAgreementStatus(addCitizenLitigationRequestForAgreementRequestReplyReturnValue);

                                removeList.Add(nafathTransaction);
                                continue;

                            }

                            if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_REJECTED
                            || nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_EXPIRED)
                            {

                                removeList.Add(nafathTransaction);
                                await _litigationRequestAgreementHub.Clients.Group(nafathTransaction.TransactionId).ReceivedLitigationRequestAgreementStatus(new BaseResponse() { IsDone = false, Message = "صلاحية الرقم منتهيه" });
                                continue;
                            }
                        }
                    }

                    if (removeList.Any())
                    {
                        foreach (var nafathTransaction in removeList)
                        {
                            _nafathAgreementRequestService.Remove(nafathTransaction);
                        }
                    }
                }

            }
            catch (Exception)
            {
            }

        }
        #endregion


        #region Reconciliation Report Otp
        public async Task<DataResponse<NafathResponseDto>> NafathReconciliationReport(NafathRequestDto nafathRequestDto, NafathReconciliationReportTransactionDto listInput)
        {
            try
            {
                if (nafathRequestDto is null)
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = "" };
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = "" };
                var requestBody = await GetNafathSettingsData(nafathRequestDto.NationalId);
                if (!requestBody.IsDone || requestBody.Data is null)
                    return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "" };
                var url = _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.Nafath_URL).Result.Data?.FirstOrDefault()?.Value;
                if (string.IsNullOrEmpty(url))
                    return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "" };
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var body = JsonConvert.SerializeObject(requestBody.Data);
                var content = new StringContent(body, Encoding.UTF8, "application/json");
                request.Content = content;
                var response = await client.SendAsync(request);
                var x = response.Content.ReadAsStringAsync();
                NafathResponseDto? nafathResponse = new NafathResponseDto();
                if (response.IsSuccessStatusCode)
                {
                    nafathResponse.Data = await response.Content.ReadFromJsonAsync<NafathResponseDataDto>();
                    if (nafathResponse.Data is not null && nafathResponse.Data.Random is not null && nafathResponse.Data.TransId is not null)
                    {
                        NafathReconciliationReportTransactionDto nafathTransaction = new NafathReconciliationReportTransactionDto()
                        {
                            LitigationRequestId = listInput.LitigationRequestId,
                            NationalId = listInput.NationalId,
                            RequestMinutesOfMeetingApprovalStatus = listInput.RequestMinutesOfMeetingApprovalStatus,
                            TransactionId = nafathResponse.Data.TransId,
                            //TransactionId = "456",
                            RandomNumber = nafathResponse.Data.Random,
                            //RandomNumber = "15",
                            RequestExpired = DateTime.Now.AddMinutes(2),
                            RequestMinutesOfMeetingApprovalOn = DateTime.Now
                        };
                        _nafathReconciliationReportService.Add(nafathTransaction);
                    }
                    //return new DataResponse<NafathResponseDto>
                    //{
                    //    IsDone = true,
                    //    Data =
                    //    new NafathResponseDto()
                    //    {
                    //        Data = new NafathResponseDataDto()
                    //        {
                    //            Random = "15",
                    //            TransId = "456"
                    //        }
                    //    }
                    //};

                    return new DataResponse<NafathResponseDto> { IsDone = true, Data = nafathResponse };

                }
                else
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    if (!string.IsNullOrEmpty(jsonResponse))
                    {
                        nafathResponse.Error = await response.Content.ReadFromJsonAsync<NafathResponseErrorDto>();
                    }
                    return new DataResponse<NafathResponseDto> { IsDone = false, Message = nafathResponse.Error?.Message ?? nafathResponse.Error?.Code ?? "", Data = nafathResponse };
                }
            }
            catch (Exception e)
            {
                return new DataResponse<NafathResponseDto>() { IsDone = false, Message = "خطأ في الاتصال بنفاذ" };
            }
        }



        public async Task CheckNafathReconciliationReportStatus()
        {
            try
            {
                
                var nafathCheckList = _nafathReconciliationReportService.GetAll();
                var removeList = new List<NafathReconciliationReportTransactionDto>();
                if (nafathCheckList is not null && nafathCheckList.Any())
                {
                    var token = (await _gEELoginService.Login()).Data?.Access_token;
                    if (token is null)
                        _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                           , new LogDto()
                           {
                               Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                               Message = "get access token from API GEE error"
                           });

                    foreach (var nafathTransaction in nafathCheckList)
                    {
                        if (nafathTransaction.RequestExpired < DateTime.Now)
                        {
                            removeList.Add(nafathTransaction);
                            await _nafathReconciliationReportHub.Clients.Group(nafathTransaction.TransactionId).ReceivedNafathReconciliationReportStatus(new BaseResponse() { IsDone = false, Message = "صلاحية الرقم منتهيه" });
                            continue;
                        }
                        NafathCheckRequestDto nafathCheckRequestDto = new NafathCheckRequestDto()
                        {
                            Id = nafathTransaction.NationalId,
                            Random = nafathTransaction.RandomNumber,
                            TransId = nafathTransaction.TransactionId,
                            Token = token
                        };
                        var nafathCheckStatusResponse = (await NafathCheckStatus(nafathCheckRequestDto));
                        //var nafathCheckStatusResponse = new DataResponse<NafathCheckResponseDto>()
                        //{
                        //    IsDone = true,
                        //    Data = new NafathCheckResponseDto() { Data = new NafathCheckResponseDataDto() { Status = SysSettingKeyConstants.NafathStatus_COMPLETED, AccessToken = "123" }, Error = null }
                        //};
                        if (nafathCheckStatusResponse is not null && nafathCheckStatusResponse.Data is not null)
                        {
                            if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_COMPLETED
                                && nafathCheckStatusResponse.Data?.Data?.AccessToken is not null)
                            {
                                var addCitizenLitigationRequestForReportReply = await _litigationRequestReconciliationReportService.AddCitizenLitigationRequestForReportReply(nafathTransaction.LitigationRequestId, nafathTransaction.NationalId, nafathTransaction.RequestMinutesOfMeetingApprovalStatus);

                                await _nafathReconciliationReportHub.Clients.Group(nafathTransaction.TransactionId).ReceivedNafathReconciliationReportStatus(addCitizenLitigationRequestForReportReply);
                                removeList.Add(nafathTransaction);
                                continue;
                            }
                            if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_REJECTED
                            || nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_EXPIRED)
                            {
                                removeList.Add(nafathTransaction);
                                await _nafathReconciliationReportHub.Clients.Group(nafathTransaction.TransactionId).ReceivedNafathReconciliationReportStatus(new BaseResponse() { IsDone = false, Message = "صلاحية الرقم منتهيه" });
                                continue;
                            }
                        }
                    }
                    if (removeList.Any())
                    {
                        foreach (var nafathTransaction in removeList)
                        {
                            _nafathReconciliationReportService.Remove(nafathTransaction);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public async Task<BaseResponse> CheckNafathReconciliationReportStatusForMobile(string transactionId)
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                           Message = "get access token from API GEE error"
                       });
                var nafathCheckItem = _nafathReconciliationReportService.GetAll().FirstOrDefault(x => x.TransactionId == transactionId);
                if (nafathCheckItem is null)
                    return new BaseResponse() { IsDone = false, Message = "لا يوجد بيانات لهذا الرقم" };

                var removeList = new List<NafathReconciliationReportTransactionDto>();

                if (nafathCheckItem.RequestExpired < DateTime.Now)
                {
                    IdentityStatus identityStatus = new IdentityStatus()
                    {
                        TransactionId = nafathCheckItem.TransactionId,
                        IsValid = false,
                        Token = null,
                        UserInfoUpdated = null,
                    };
                    removeList.Add(nafathCheckItem);
                    return new DataResponse<IdentityStatus>() { IsDone = true, Data = identityStatus, Message = "Token Expired" };

                }
                NafathCheckRequestDto nafathCheckRequestDto = new NafathCheckRequestDto()
                {
                    Id = nafathCheckItem.NationalId,
                    Random = nafathCheckItem.RandomNumber,
                    TransId = nafathCheckItem.TransactionId,
                    Token = token
                };
                var nafathCheckStatusResponse = (await NafathCheckStatus(nafathCheckRequestDto));
                //var nafathCheckStatusResponse = new DataResponse<NafathCheckResponseDto>()
                //{
                //    IsDone = true,
                //    Data = new NafathCheckResponseDto() { Data = new NafathCheckResponseDataDto() { Status = SysSettingKeyConstants.NafathStatus_COMPLETED, AccessToken = "123" }, Error = null }
                //};
                if (nafathCheckStatusResponse is not null && nafathCheckStatusResponse.Data is not null)
                {
                    if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_COMPLETED
                        && nafathCheckStatusResponse.Data?.Data?.AccessToken is not null)
                    {
                        var addCitizenLitigationRequestForReportReply = await _litigationRequestReconciliationReportService.AddCitizenLitigationRequestForReportReply(nafathCheckItem.LitigationRequestId, nafathCheckItem.NationalId, nafathCheckItem.RequestMinutesOfMeetingApprovalStatus);
                        if (addCitizenLitigationRequestForReportReply.IsDone)
                        {
                            return addCitizenLitigationRequestForReportReply;
                        }

                        //var nafathReconciliationReport = _nafathReconciliationReportHub.Clients.Group(nafathCheckItem.TransactionId).ReceivedNafathReconciliationReportStatus(addCitizenLitigationRequestForReportReply);
                        //if (nafathReconciliationReport.IsCompleted)
                        //{
                        //    return new BaseResponse() { IsDone = true, Message= "تم الحفظ بنجاح" };
                        //}
                        removeList.Add(nafathCheckItem);

                    }
                    if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_REJECTED
                    || nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_EXPIRED)
                    {
                        removeList.Add(nafathCheckItem);
                        //var nafathReconciliationReport = _nafathReconciliationReportHub.Clients.Group(nafathCheckItem.TransactionId).ReceivedNafathReconciliationReportStatus(new BaseResponse() { IsDone = false, Message = "صلاحية الرقم منتهيه" });
                        //if (nafathReconciliationReport.IsCompleted)
                        //{
                        //}
                            return new BaseResponse() { IsDone = false, Message = "حدث خطأ او لم يتم تفعيل الرقم السري" };
                    }
                }

                if (removeList.Any())
                {
                    foreach (var nafathTransaction in removeList)
                    {
                        _nafathReconciliationReportService.Remove(nafathTransaction);
                    }
                }
                return new BaseResponse() { IsDone = false, Message = "حدث خطأ او لم يتم تفعيل الرقم السري" };

            }
            catch (Exception e)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                              , new LogDto()
                              {
                                  Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                  Message = $"Nafath reconciliation report Exception ==> {e}"
                              });

                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }
        }

public async Task<BaseResponse> CheckNafathAgreementRequestStatusForMobile(string transactionId)
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                           Message = "get access token from API GEE error"
                       });
                var nafathCheckItem = _nafathAgreementRequestService.GetAll().FirstOrDefault(x => x.TransactionId == transactionId);
                if (nafathCheckItem is null)
                    return new BaseResponse() { IsDone = false, Message = "لا يوجد بيانات لهذا الرقم" };

                var removeList = new List<NafathAgreementRequestTransactionsDto>();

                if (nafathCheckItem.RequestExpired < DateTime.Now)
                {
                    IdentityStatus identityStatus = new IdentityStatus()
                    {
                        TransactionId = nafathCheckItem.TransactionId,
                        IsValid = false,
                        Token = null,
                        UserInfoUpdated = null,
                    };
                    removeList.Add(nafathCheckItem);
                    return new DataResponse<IdentityStatus>() { IsDone = true, Data = identityStatus, Message = "Token Expired" };

                }
                NafathCheckRequestDto nafathCheckRequestDto = new NafathCheckRequestDto()
                {
                    Id = nafathCheckItem.NationalId,
                    Random = nafathCheckItem.RandomNumber,
                    TransId = nafathCheckItem.TransactionId,
                    Token = token
                };
                var nafathCheckStatusResponse = (await NafathCheckStatus(nafathCheckRequestDto));

                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                              , new LogDto()
                              {
                                  Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                  Message = $"nafath Check Status Response ==> {nafathCheckStatusResponse}"
                              });
                              
                //var nafathCheckStatusResponse = new DataResponse<NafathCheckResponseDto>()
                //{
                //    IsDone = true,
                //    Data = new NafathCheckResponseDto() { Data = new NafathCheckResponseDataDto() { Status = SysSettingKeyConstants.NafathStatus_COMPLETED, AccessToken = "123" }, Error = null }
                //};
                if (nafathCheckStatusResponse is not null && nafathCheckStatusResponse.Data is not null)
                {
                    if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_COMPLETED
                        && nafathCheckStatusResponse.Data?.Data?.AccessToken is not null)
                    {
                        var addCitizenLitigationRequestForAgreementRequestReplyReturnValue = await _agreementRequestService.AddCitizenLitigationRequestForAgreementRequestReply(nafathCheckItem.LitigationRequestId, nafathCheckItem.NationalId, nafathCheckItem.AgreementStatus);
                        if (addCitizenLitigationRequestForAgreementRequestReplyReturnValue.IsDone)
                        {
                            return addCitizenLitigationRequestForAgreementRequestReplyReturnValue;
                        }

                        //var nafathReconciliationReport = _nafathReconciliationReportHub.Clients.Group(nafathCheckItem.TransactionId).ReceivedNafathReconciliationReportStatus(addCitizenLitigationRequestForReportReply);
                        //if (nafathReconciliationReport.IsCompleted)
                        //{
                        //    return new BaseResponse() { IsDone = true, Message= "تم الحفظ بنجاح" };
                        //}
                        removeList.Add(nafathCheckItem);

                    }
                    if (nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_REJECTED
                    || nafathCheckStatusResponse.Data?.Data?.Status == SysSettingKeyConstants.NafathStatus_EXPIRED)
                    {
                        removeList.Add(nafathCheckItem);
                        //var nafathReconciliationReport = _nafathReconciliationReportHub.Clients.Group(nafathCheckItem.TransactionId).ReceivedNafathReconciliationReportStatus(new BaseResponse() { IsDone = false, Message = "صلاحية الرقم منتهيه" });
                        //if (nafathReconciliationReport.IsCompleted)
                        //{
                        //}
                        return new BaseResponse() { IsDone = false, Message = "حدث خطأ او لم يتم تفعيل الرقم السري" };
                    }
                }

                if (removeList.Any())
                {
                    foreach (var nafathTransaction in removeList)
                    {
                        _nafathAgreementRequestService.Remove(nafathTransaction);
                    }
                }
                return new BaseResponse() { IsDone = false, Message = "حدث خطأ او لم يتم تفعيل الرقم السري" };

            }
            catch (Exception e)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                              , new LogDto()
                              {
                                  Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                  Message = $"Nafath Agreement Request Exception ==> {e}"
                              });

                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }
        }



        #endregion

    }
}
