using System.Text.Json.Serialization;

namespace REAC.Application.Contracts.Dtos.Integration.GEE;

public class MCGEECRBasicInfoDto
{
    public YefiResponseHeader yefiResponseHeader { get; set; }
    public YefiErrorResponse? YefiErrorResponse { get; set; }
    public YefiError? yefiError { get; set; }
    public CrBasicInfo? crInformation { get; set; }
    public CrActivities? crActivities { get; set; }
    public ContactInformation? contactInformation { get; set; }
}

public class ActivitiesType
{
    public string activitiesTypeID { get; set; }
    public string activitiesTypeDescriptionAr { get; set; }
    public string activitiesTypeDescriptionEn { get; set; }
}

public class ActivityList
{
    public string activityID { get; set; }
    public string activityDescriptionAr { get; set; }
    public string activityDescriptionEn { get; set; }
    public bool? isPreLicenseIssued { get; set; }
    public bool? isPostLicenseIssued { get; set; }
}

public class CapitalCurrency
{
    public int? capitalCurrencyID { get; set; }
    public string capitalCurrencyDescAr { get; set; }
    public string capitalCurrencyDescEn { get; set; }
}

public class CompanyForm
{
    public int? companyFormID { get; set; }
    public string companyFormDescriptionAr { get; set; }
    public string companyFormDescriptionEn { get; set; }
}

public class ContactInformation
{
    public string mobileNo { get; set; }
    public string email { get; set; }
}

public class CrActivities
{
    public ActivitiesType activitiesType { get; set; }
    public List<ActivityList> activityList { get; set; }
}

public class CrBasicInfo
{
    public string crNationalNumber { get; set; }
    public string crNumber { get; set; }
    public int? versionNo { get; set; }
    public string entityFullNameAr { get; set; }
    public double? capital { get; set; }
    public bool? isMain { get; set; }
    public string mainCRNationalNumber { get; set; }
    public string mainCRNumber { get; set; }
    public bool? isLicenseBased { get; set; }
    public bool? hasEcommerce { get; set; }
    public bool? inLiquidationProcess { get; set; }
    public EntityNameLang entityNameLang { get; set; }
    public EntityType entityType { get; set; }
    public CapitalCurrency capitalCurrency { get; set; }
    public HeadquarterCity headquarterCity { get; set; }
    public CompanyForm companyForm { get; set; }
    public PartnersNationality partnersNationality { get; set; }
    public CrIssueDate crIssueDate { get; set; }
    public CrConfirmDate crConfirmDate { get; set; }
    public CrStatus crStatus { get; set; }
}

public class CrConfirmDate
{
    public string crConfirmDateGregorian { get; set; }
    public string crConfirmDateHijri { get; set; }
}

public class CrIssueDate
{
    public string crIssueDateGregorian { get; set; }
    public string crIssueDateHijri { get; set; }
}

public class CrStatus
{
    public int? crStatusID { get; set; }
    public string crStatusDescAr { get; set; }
    public string crStatusDescEn { get; set; }
}

public class EntityNameLang
{
    public int? entityNameLangID { get; set; }
    public string entityNameLangDescAr { get; set; }
    public string entityNameLangDescEn { get; set; }
}

public class EntityType
{
    public int? entityTypeID { get; set; }
    public string entityTypeDescAr { get; set; }
    public string entityTypeDescEn { get; set; }
}

public class HeadquarterCity
{
    public int? headquarterCityID { get; set; }
    public string headquarterCityNameAr { get; set; }
    public string headquarterCityNameEn { get; set; }
}

public class PartnersNationality
{
    public int? partnersNationalityID { get; set; }
    public string partnersNationalityDescAr { get; set; }
    public string partnersNationalityDescEn { get; set; }
}


public class YefiResponseHeader
{
    [JsonPropertyName("YeFI-Source-Id")]
    public string YeFISourceId { get; set; }

    [JsonPropertyName("YeFI-Time-Stamp")]
    public DateTime? YeFITimeStamp { get; set; }

    [JsonPropertyName("YeFI-Message-Id")]
    public string YeFIMessageId { get; set; }
}


public class YefiError
{
    public string raisedBy { get; set; }
    public string sourceAgency { get; set; }
    public string errorCode { get; set; }
    public string errorText { get; set; }
}