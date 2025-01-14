using Newtonsoft.Json;
using REAC.Application.Contracts.Dtos.Integration.Nafath;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Helpers;
using System.Text.Json.Serialization;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{

    #region Nafath

    public class NafathRequestDto
    {
        public string NationalId { get; set; }
        public bool IsRegister { get; set; }
    }

    public class NafathRequestBodyDto
    {
        public string Action { get; set; }
        public NafathRequestParametersBodyDto Parameters { get; set; }
    }

    public class NafathRequestParametersBodyDto
    {
        public string service { get; set; }
        public string id { get; set; }
    }

    public class NafathResponseDto
    {
        public bool IsDraftAccount { get; set; }
        public NafathResponseDataDto? Data { get; set; }
        public NafathResponseErrorDto? Error { get; set; }
    }

    public class NafathResponseDataDto
    {
        public string TransId { get; set; }
        public string Random { get; set; }
    }

    public class NafathStatusResponse
    {
        public string TransactionId { get; set; }
        public bool IsValid { get; set; }
        public string Message { get; set; } 
    }

    #endregion


    #region Nafath Check API

    public class NafathCheckBodyDto
    {
        public string Action { get; set; }
        public NafathTransactionDto Parameters { get; set; }
    }

    public class NafathCheckRequestDto
    {
        public string Id { get; set; }
        public string Random { get; set; }
        public string TransId { get; set; }
        public string Token { get; set; }
    }

    public class NafathCheckResponseDto
    {
        public NafathCheckResponseDataDto? Data { get; set; }
        public NafathResponseErrorDto? Error { get; set; }
    }

    public class NafathCheckResponseDataDto
    {
        public string Status { get; set; }
        public string? AccessToken { get; set; }
    }


    public class WafiReceivedCallBackModel
    {
        public string TransactionId { get; set; }
        public bool IsValid { get; set; }
    }

    #endregion


    #region Nafath Get User Info

    public class NafathUserInfoDto
    {
        public UserInfoDto user_info { get; set; }
    }
    



    public class UserInfoDto
    {
        [Newtonsoft.Json.JsonConverter(typeof(ToStringJsonConverter))]
        public long id { get; set; }

        [JsonProperty("full_name#ar")]
        public string full_name_ar { get; set; }

        [JsonProperty("first_name#ar")]
        public string first_name_ar { get; set; }

        [JsonProperty("id_expiry_date#g")]
        public string id_expiry_date_g { get; set; }

        public int nationality { get; set; }

        [JsonProperty("nationality#ar")]
        public string nationality_ar { get; set; }

        [JsonProperty("dob#g")]
        public string dob_g { get; set; }

        public string gender { get; set; }
    } 
    
    

    public class UserInfoUpdatedDto
    {
        //رقم الهوية
        public string? NationalId { get; set; }
        //الاسم بالكامل
        public string? FullName { get; set; }
        // الاسم الأول
        public string? FirstName { get; set; }
        // تاريخ انتهاء الهوية الوطنية
        public DateTime? NationalIdExpiryDate { get; set; }
        public Guid? NationalityId { get; set; }
        //الجنسية
        public string? Nationality { get; set; }
        //تاريخ الميلاد
        public DateTime? DateOfBirth { get; set; }
        //الجنس
        public Gender? Gender { get; set; }
        public bool IsDraftAccount { get; set; } = false;
        public bool IsArbitrator { get; set; } = false;
        public bool IsReformer { get; set; } = false;
        public bool IsLawye { get; set; } = false;
    }

    #endregion


    #region Nafath Get User Address Info
    public class NafathUserAddressInfo
    {
        public GetIndividualWaselAddressResponse GetIndividualWaselAddressResponse { get; set; }
    }

    public class GetIndividualWaselAddressResponse
    {
        public GetIndividualWaselAddressResult GetIndividualWaselAddressResult { get; set; }
    }

    public class GetIndividualWaselAddressResult
    {
        public GetIndividualWaselAddressResponseDetailObject getIndividualWaselAddressResponseDetailObject { get; set; }
    }

    public class GetIndividualWaselAddressResponseDetailObject
    {
        [Newtonsoft.Json.JsonConverter(typeof(SingleOrArrayJsonConverter<WaselAddress>))]
        public List<WaselAddress> WaselAddress { get; set; }
    }

    public class WaselAddress
    {
        public string BuildingNumber { get; set; }
        public string AdditionalNumber { get; set; }
        public string ZipCode { get; set; }
        public string UnitNumber { get; set; }
        public string DistrictAreaArabic { get; set; }
        public string DistrictAreaEnglish { get; set; }
        public string StreetNameArabic { get; set; }
        public string StreetNameEnglish { get; set; }
        public string CityNameArabic { get; set; }
        public string CityNameEnglish { get; set; }
        public string FullName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }


    public class UserAddressInfoUpdatedDto
    {
        public string BuildingNumber { get; set; }
        public string AdditionalNumber { get; set; }
        public string ZipCode { get; set; }
        public string UnitNumber { get; set; }
        public string DistrictAreaArabic { get; set; }
        public string DistrictAreaEnglish { get; set; }
        public string StreetNameArabic { get; set; }
        public string StreetNameEnglish { get; set; }
        public string CityNameArabic { get; set; }
        public string CityNameEnglish { get; set; }
        public string FullName { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        
    }

    #endregion

    public class NafathResponseErrorDto
    {
        public string Code { get; set; }
        public string Message { get; set; }
    }


}
