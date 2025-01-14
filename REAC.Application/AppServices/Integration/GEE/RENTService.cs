using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Newtonsoft.Json;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace REAC.Application.AppServices.Integration.GEE
{
    public class RENTService : IRENTService
    {
        private readonly IGEELoginService _gEELoginService;
        private readonly ILoggerManager _loggerManager;
        private readonly ISysSettingService _sysSetting;

        public RENTService(IGEELoginService gEELoginService, ILoggerManager loggerManager, ISysSettingService sysSetting)
        {
            _gEELoginService = gEELoginService;
            _loggerManager = loggerManager;
            _sysSetting = sysSetting;
        }

        //public Task<DataResponse<RentRequestDTO>> GetRentContract(string token, string contractCode, string versionNo)
        //{
        //    //Integration Code will be implemented here
        //   //return new RentRequestDTO
        //   //{
        //   //    ContractKey = "1234abcd",
        //   //    VersionNumber = "7777",
        //   //    FirstPartyDto = new PartyDTO
        //   //    {
        //   //        Name = "FirstPartyName",
        //   //        Identityu = "123abcd",
        //   //        NationalAddress = "First testAddress",
        //   //        Nationality = "Egyption",
        //   //        FacilityNumber = null,
        //   //        CommercialRegistrationNo = null,
        //   //        FacilityName = null,
        //   //        Capacity = "مستاجر",
        //   //        Mobile = "12345678",
        //   //        Email = "first@gmail.com",
        //   //        BirthDate = new DateTime(1999, 1, 1)
        //   //    },
        //   //    SecondPartyDto = new PartyDTO
        //   //    {
        //   //        Name = "Second Party",
        //   //        IdentityNo = "123abcdooooo",
        //   //        NationalAddress = "SencodtestAddress",
        //   //        Nationality = "Egyption",
        //   //        FacilityNumber = null,
        //   //        CommercialRegistrationNo = null,
        //   //        FacilityName = null,
        //   //        Capacity = "مستاجر",
        //   //        Mobile = "12345678",
        //   //        Email = "first@gmail.com",
        //   //        BirthDate = new DateTime(1999, 1, 1)

        //   //    },
        //   //    FirstPartyCapacity = FirstPartyCapacity,
        //   //    ContractAddress = "contractAddressTest"
        //   //};
        //}
        public async Task<DataResponse<EjarApiResponse>> GetRentContractDetails(long contractNumber)
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

                var url = _sysSetting.GetByIntegrationKey(SysSettingKeyConstants.Ejar_ContractDetails).Result.Data?.FirstOrDefault()?.Value;


                if (string.IsNullOrEmpty(url))
                    return new DataResponse<EjarApiResponse>() { IsDone = false, Message = "خطأ في استرجاع رابط ايجار" };

                url = string.Format(url, contractNumber);
                //var url = $"https://api-dev.rega.gov.sa/v1/ejar/external/contracts/details?pageNumber=1&pageSize=1&contractNumber={contractNumber}";

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<EjarApiResponse>(content);

                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Debug,
                           Message = $"Ejar Response ====> {JsonSerializer.Serialize(result)}"
                       });

                    return new DataResponse<EjarApiResponse>() { IsDone = true, Data = result };
                }

                return new DataResponse<EjarApiResponse>() { IsDone = false, Message = "خطأ في استرجاع بيانات ايجار, من فضلك راجع البيانات المدخله" };

            }
            catch (Exception ex)
            {
                return new DataResponse<EjarApiResponse>() { IsDone = false, Message = "خطأ في استرجاع بيانات ايجار" };
            }
        }

        public async Task<DataResponse<EjarApiResponse>> GetRentContractFinancialDetails(long contractNumber)
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

                var domain = "https://api-dev.rega.gov.sa/v1/ejar/external/financial-contracts/details";

                var url = $"https://api-dev.rega.gov.sa/v1/ejar/external/financial-contracts/details?pageNumber=1&pageSize=1&contractNumber={contractNumber}";

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<EjarApiResponse>(content);

                    return new DataResponse<EjarApiResponse>() { IsDone = true, Data = result };
                }

                return new DataResponse<EjarApiResponse>() { IsDone = false, Message = "خطأ في استرجاع بيانات ايجار الماليه, من فضلك راجع البيانات المدخله" };

            }
            catch (Exception ex)
            {
                return new DataResponse<EjarApiResponse>() { IsDone = false, Message = "خطأ في استرجاع بيانات ايجار الماليه" };
            }
        }
    }

}
