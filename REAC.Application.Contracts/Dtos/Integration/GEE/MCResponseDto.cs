namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    #region CRInformationByCRNumber

    public class MCCRInformationByCRNumberResponseDto
    {
        public CRInformationByCRNumberResponseDataDto? Data { get; set; }
        public GeneralGEEErrorDto? Error { get; set; }
        public UnauthorizedErrorDto? UnauthorizedError { get; set; }
    }


    public class CRInformationByCRNumberResponseDataDto
    {
        public GetCRInformationByCRNumberResponseDto GetCRInformationByCRNumberResponse { get; set; }
    }

    public class GetCRInformationByCRNumberResponseDto
    {
        public GetCRInformationByCRNumberResultDto GetCRInformationByCRNumberResult { get; set; }
    }

    public class GetCRInformationByCRNumberResultDto
    {
        public GetCRInformationByCRNumberResponseDetailObjectDto getCRInformationByCRNumberResponseDetailObject { get; set; }
    }

    public class GetCRInformationByCRNumberResponseDetailObjectDto
    {
        public string CRNationalNumber { get; set; }
        public string CRNumber { get; set; }
        public string CRIssueHijriDate { get; set; }
        public string CRExpiryHijriDate { get; set; }
        public string CRNameAR { get; set; }
        public string CRStatus { get; set; }
        public string CRAddress { get; set; }
    }



    public class CRInformationByCRNumberUpdatedDto
    {
        public CRInformationByCRNumberDto? Data { get; set; }
        public GeneralGEEErrorDto? Error { get; set; }
        public UnauthorizedErrorDto? UnauthorizedError { get; set; }
    }

    public class CRInformationByCRNumberDto
    {
        //رقم 700 الخاص بالمنشأة
        public string CRNationalNumber { get; set; }
        //رقم السجل التجاري
        public string CRNumber { get; set; }
        //تاريخ بداية ترخيص المنشأة
        public DateTime CRIssueHijriDate { get; set; }
        //تاريخ انتهاء ترخيص المنشأة
        public DateTime CRExpiryHijriDate { get; set; }
        //اسم المنشأة
        public DateTime CRIssueDate { get; set; }
        public DateTime CRExpiryDate { get; set; }
        public string CRName { get; set; }
        //حالة ترخيص المنشأة
        public string CRStatus { get; set; }
        // عنوان المنشأة
        public string CRAddress { get; set; }
    }

    #endregion

    #region CR Information By CR Owners

    public class MCCRInformationByCROwnersResponseDto
    {
        public CRInformationByCROwnersResponseDataDto? Data { get; set; }
        public GeneralGEEErrorDto? Error { get; set; }
        public UnauthorizedErrorDto? UnauthorizedError { get; set; }
    }

    public class CRInformationByCROwnersResponseDataDto
    {
        public GetCROwnersByCRNumberResponseDto GetCROwnersByCRNumberResponse { get; set; }
    }

    public class GetCROwnersByCRNumberResponseDto
    {
        public GetCROwnersByCRNumberResultDto GetCROwnersByCRNumberResult { get; set; }
    }

    public class GetCROwnersByCRNumberResultDto
    {
        public GetCROwnersByCRNumberResponseDetailObjectDto getCROwnersByCRNumberResponseDetailObject { get; set; }
    }

    public class GetCROwnersByCRNumberResponseDetailObjectDto
    {
        public CROwnersListDto CROwnersList { get; set; }
    }

    public class CROwnersListDto
    {
        public List<CROwnerDto> CROwner { get; set; }
    }

    public class CROwnerDto
    {
        public OwnerIDDto OwnerID { get; set; }
    }

    public class OwnerIDDto
    {
        public PersonIdentifierDto PersonIdentifier { get; set; }
    }

    public class PersonIdentifierDto
    {
        public string NationalID { get; set; }
    }


    public class CRInformationByCROwnersUpdatedDto
    {
        //أرقام هوية مُلاك السجل التجاري
        public List<string>? Data { get; set; } = new List<string>();
        public GeneralGEEErrorDto? Error { get; set; }
        public UnauthorizedErrorDto? UnauthorizedError { get; set; }
    }
    #endregion

    #region Test CR Information By CR Owners

    public class TestMCCRInformationByCROwnersResponseDto
    {
        public TestCRInformationByCROwnersResponseDataDto? Data { get; set; }
        public GeneralGEEErrorDto? Error { get; set; }
        public UnauthorizedErrorDto? UnauthorizedError { get; set; }
    }

    public class TestCRInformationByCROwnersResponseDataDto
    {
        public TestGetCROwnersByCRNumberResponseDto GetCROwnersByCRNumberResponse { get; set; }
    }

    public class TestGetCROwnersByCRNumberResponseDto
    {
        public TestGetCROwnersByCRNumberResultDto GetCROwnersByCRNumberResult { get; set; }
    }

    public class TestGetCROwnersByCRNumberResultDto
    {
        public TestGetCROwnersByCRNumberResponseDetailObjectDto getCROwnersByCRNumberResponseDetailObject { get; set; }
    }

    public class TestGetCROwnersByCRNumberResponseDetailObjectDto
    {
        public TestCROwnersListDto CROwnersList { get; set; }
    }

    public class TestCROwnersListDto
    {
        public CROwnerDto CROwner { get; set; }
    }

    public class TestCROwnerDto
    {
        public OwnerIDDto OwnerID { get; set; }
    }

    public class TestOwnerIDDto
    {
        public TestPersonIdentifierDto PersonIdentifier { get; set; }
    }

    public class TestPersonIdentifierDto
    {
        public string NationalID { get; set; }
    }


    public class TestCRInformationByCROwnersUpdatedDto
    {
        //أرقام هوية مُلاك السجل التجاري
        public List<string>? Data { get; set; } = new List<string>();
        public GeneralGEEErrorDto? Error { get; set; }
        public UnauthorizedErrorDto? UnauthorizedError { get; set; }
    }
    #endregion


    public class UnauthorizedErrorDto
    {
        public GeneralGEEErrorDto ServiceError { get; set; }
    }

}
