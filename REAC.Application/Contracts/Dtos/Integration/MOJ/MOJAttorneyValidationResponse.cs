using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace REAC.Application.Contracts.Dtos.Integration.MOJ
{
    public class MOJAttorneyValidationResponse
    {
        [JsonProperty("responseObject")]
        [JsonPropertyName("responseObject")]
        public ResponseObject ResponseObject { get; set; }

        [JsonProperty("ResponseCode")]
        [JsonPropertyName("ResponseCode")]
        public int ResponseCode { get; set; }

        [JsonProperty("ResponseResult")]
        [JsonPropertyName("ResponseResult")]
        public bool ResponseResult { get; set; }

        [JsonProperty("ResponseMessage")]
        [JsonPropertyName("ResponseMessage")]
        public string ResponseMessage { get; set; }

        [JsonProperty("ResponseMessages")]
        [JsonPropertyName("ResponseMessages")]
        public object ResponseMessages { get; set; }

        [JsonProperty("ExceptionMessage")]
        [JsonPropertyName("ExceptionMessage")]
        public object ExceptionMessage { get; set; }

        [JsonProperty("ExceptionStackTrace")]
        [JsonPropertyName("ExceptionStackTrace")]
        public object ExceptionStackTrace { get; set; }
    }

    public class ResponseObject
    {
        [JsonProperty("CurrentPage")]
        [JsonPropertyName("CurrentPage")]
        public int CurrentPage { get; set; }

        [JsonProperty("PagesCount")]
        [JsonPropertyName("PagesCount")]
        public int PagesCount { get; set; }

        [JsonProperty("PageSize")]
        [JsonPropertyName("PageSize")]
        public int PageSize { get; set; }

        [JsonProperty("RowsCount")]
        [JsonPropertyName("RowsCount")]
        public int RowsCount { get; set; }

        [JsonProperty("Results")]
        [JsonPropertyName("Results")]
        public List<ResultItem> Results { get; set; }

        [JsonProperty("ResponseMessage")]
        [JsonPropertyName("ResponseMessage")]
        public string ResponseMessage { get; set; }
    }

    public class ResultItem
    {
        [JsonProperty("HasFreeText")]
        [JsonPropertyName("HasFreeText")]
        public bool HasFreeText { get; set; }

        [JsonProperty("InquiryReference")]
        [JsonPropertyName("InquiryReference")]
        public string InquiryReference { get; set; }

        [JsonProperty("Model")]
        [JsonPropertyName("Model")]
        public AttorneyModel Model { get; set; }
    }

   

    public class AttorneyModel
    {
        [JsonProperty("Code")]
        [JsonPropertyName("Code")]
        public int Code { get; set; }

        [JsonProperty("AttorneyId")]
        [JsonPropertyName("AttorneyId")]
        public int AttorneyId { get; set; }

        [JsonProperty("AttorneyTypeId")]
        [JsonPropertyName("AttorneyTypeId")]
        public int AttorneyTypeId { get; set; }

        [JsonProperty("AttorneyTypeName")]
        [JsonPropertyName("AttorneyTypeName")]
        public string AttorneyTypeName { get; set; }

        [JsonProperty("AttorneyPlaceId")]
        [JsonPropertyName("AttorneyPlaceId")]
        public int AttorneyPlaceId { get; set; }

        [JsonProperty("AttorneyPlaceName")]
        [JsonPropertyName("AttorneyPlaceName")]
        public string AttorneyPlaceName { get; set; }

        [JsonProperty("AttorneyStatusId")]
        [JsonPropertyName("AttorneyStatusId")]
        public int AttorneyStatusId { get; set; }

        [JsonProperty("AttorneyStatusName")]
        [JsonPropertyName("AttorneyStatusName")]
        public string AttorneyStatusName { get; set; }

        [JsonProperty("BaseAttorneys")]
        [JsonPropertyName("BaseAttorneys")]
        public List<object> BaseAttorneys { get; set; }

        [JsonProperty("AllowedToActOnBehalf")]
        [JsonPropertyName("AllowedToActOnBehalf")]
        public List<AllowedToActOnBehalf> AllowedToActOnBehalf { get; set; }

        [JsonProperty("FirstParties")]
        [JsonPropertyName("FirstParties")]
        public List<Party> FirstParties { get; set; }

        [JsonProperty("SecondParties")]
        [JsonPropertyName("SecondParties")]
        public List<Party> SecondParties { get; set; }

        [JsonProperty("WitnessParties")]
        [JsonPropertyName("WitnessParties")]
        public List<object> WitnessParties { get; set; }

        [JsonProperty("RequesterParties")]
        [JsonPropertyName("RequesterParties")]
        public List<object> RequesterParties { get; set; }

        [JsonProperty("IsSecondPartiesTogether")]
        [JsonPropertyName("IsSecondPartiesTogether")]
        public bool IsSecondPartiesTogether { get; set; }

        [JsonProperty("EndDateHijri")]
        [JsonPropertyName("EndDateHijri")]
        public string EndDateHijri { get; set; }

        [JsonProperty("EndDateHijriArabic")]
        [JsonPropertyName("EndDateHijriArabic")]
        public string EndDateHijriArabic { get; set; }

        [JsonProperty("EndDateGreg")]
        [JsonPropertyName("EndDateGreg")]
        public string EndDateGreg { get; set; }

        [JsonProperty("AttorneyText")]
        [JsonPropertyName("AttorneyText")]
        public string AttorneyText { get; set; }

        [JsonProperty("IssueDateHijri")]
        [JsonPropertyName("IssueDateHijri")]
        public string IssueDateHijri { get; set; }

        [JsonProperty("IssueDateHijriArabic")]
        [JsonPropertyName("IssueDateHijriArabic")]
        public string IssueDateHijriArabic { get; set; }

        [JsonProperty("IssueDateGreg")]
        [JsonPropertyName("IssueDateGreg")]
        public string IssueDateGreg { get; set; }

        [JsonProperty("IssuerName")]
        [JsonPropertyName("IssuerName")]
        public string IssuerName { get; set; }

        [JsonProperty("IssuerIdentity")]
        [JsonPropertyName("IssuerIdentity")]
        public string IssuerIdentity { get; set; }

        [JsonProperty("CopiedCode")]
        [JsonPropertyName("CopiedCode")]
        public object CopiedCode { get; set; }

        [JsonProperty("CopiedAttorneyId")]
        [JsonPropertyName("CopiedAttorneyId")]
        public object CopiedAttorneyId { get; set; }

        [JsonProperty("CopiedType")]
        [JsonPropertyName("CopiedType")]
        public object CopiedType { get; set; }

        [JsonProperty("CompanyRepresentatorName")]
        [JsonPropertyName("CompanyRepresentatorName")]
        public object CompanyRepresentatorName { get; set; }

        [JsonProperty("AttorneyFaskhList")]
        [JsonPropertyName("AttorneyFaskhList")]
        public object AttorneyFaskhList { get; set; }

        [JsonProperty("SubAttorneyList")]
        [JsonPropertyName("SubAttorneyList")]
        public object SubAttorneyList { get; set; }

        [JsonProperty("TextsValuesList")]
        [JsonPropertyName("TextsValuesList")]
        public List<TextValue> TextsValuesList { get; set; }

        [JsonProperty("Number")]
        [JsonPropertyName("Number")]
        public object Number { get; set; }

        [JsonProperty("ExternalIssueDateHijri")]
        [JsonPropertyName("ExternalIssueDateHijri")]
        public object ExternalIssueDateHijri { get; set; }

        [JsonProperty("ExternalExpireDateHijri")]
        [JsonPropertyName("ExternalExpireDateHijri")]
        public object ExternalExpireDateHijri { get; set; }

        [JsonProperty("ApprovalMOFADateHijri")]
        [JsonPropertyName("ApprovalMOFADateHijri")]
        public object ApprovalMOFADateHijri { get; set; }

        [JsonProperty("ApprovalEmbassyDateHijri")]
        [JsonPropertyName("ApprovalEmbassyDateHijri")]
        public object ApprovalEmbassyDateHijri { get; set; }

        [JsonProperty("ApprovalSaudiMOFADateHijri")]
        [JsonPropertyName("ApprovalSaudiMOFADateHijri")]
        public object ApprovalSaudiMOFADateHijri { get; set; }

        [JsonProperty("ExternalIssueDateGreg")]
        [JsonPropertyName("ExternalIssueDateGreg")]
        public object ExternalIssueDateGreg { get; set; }

        [JsonProperty("ExternalExpireDateGreg")]
        [JsonPropertyName("ExternalExpireDateGreg")]
        public object ExternalExpireDateGreg { get; set; }

        [JsonProperty("ApprovalMOFADateGreg")]
        [JsonPropertyName("ApprovalMOFADateGreg")]
        public object ApprovalMOFADateGreg { get; set; }

        [JsonProperty("ApprovalEmbassyDateGreg")]
        [JsonPropertyName("ApprovalEmbassyDateGreg")]
        public object ApprovalEmbassyDateGreg { get; set; }

        [JsonProperty("ApprovalSaudiMOFADateGreg")]
        [JsonPropertyName("ApprovalSaudiMOFADateGreg")]
        public object ApprovalSaudiMOFADateGreg { get; set; }

        [JsonProperty("ApprovalSaudiMOFADateArabic")]
        [JsonPropertyName("ApprovalSaudiMOFADateArabic")]
        public object ApprovalSaudiMOFADateArabic { get; set; }

        [JsonProperty("ExternalIssueDateArabic")]
        [JsonPropertyName("ExternalIssueDateArabic")]
        public object ExternalIssueDateArabic { get; set; }

        [JsonProperty("ExternalExpireDateArabic")]
        [JsonPropertyName("ExternalExpireDateArabic")]
        public object ExternalExpireDateArabic { get; set; }

        [JsonProperty("ApprovalMOFADateArabic")]
        [JsonPropertyName("ApprovalMOFADateArabic")]
        public object ApprovalMOFADateArabic { get; set; }

        [JsonProperty("ApprovalEmbassyDateArabic")]
        [JsonPropertyName("ApprovalEmbassyDateArabic")]
        public object ApprovalEmbassyDateArabic { get; set; }

        [JsonProperty("ArchiveDocID")]
        [JsonPropertyName("ArchiveDocID")]
        public object ArchiveDocID { get; set; }

        [JsonProperty("DocImage")]
        [JsonPropertyName("DocImage")]
        public object DocImage { get; set; }

        [JsonProperty("Source")]
        [JsonPropertyName("Source")]
        public object Source { get; set; }

        [JsonProperty("KharjiaCountry")]
        [JsonPropertyName("KharjiaCountry")]
        public object KharjiaCountry { get; set; }

        [JsonProperty("Binary")]
        [JsonPropertyName("Binary")]
        public object Binary { get; set; }

        [JsonProperty("PartyStatus")]
        [JsonPropertyName("PartyStatus")]
        public object PartyStatus { get; set; }

        [JsonProperty("OtherPartyName")]
        [JsonPropertyName("OtherPartyName")]
        public object OtherPartyName { get; set; }

        [JsonProperty("TarfType")]
        [JsonPropertyName("TarfType")]
        public object TarfType { get; set; }

        [JsonProperty("TotalClausesCount")]
        [JsonPropertyName("TotalClausesCount")]
        public int TotalClausesCount { get; set; }

        [JsonProperty("TasdeeqKharjia")]
        [JsonPropertyName("TasdeeqKharjia")]
        public object TasdeeqKharjia { get; set; }
    }

    public class AllowedToActOnBehalf
    {
        [JsonProperty("IdentityNo")]
        [JsonPropertyName("IdentityNo")]
        public string IdentityNo { get; set; }

        [JsonProperty("SocialTypeID")]
        [JsonPropertyName("SocialTypeID")]
        public int SocialTypeID { get; set; }

        [JsonProperty("SocialTypeName")]
        [JsonPropertyName("SocialTypeName")]
        public string SocialTypeName { get; set; }

        [JsonProperty("Name")]
        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonProperty("Type")]
        [JsonPropertyName("Type")]
        public int Type { get; set; }

        [JsonProperty("TypeName")]
        [JsonPropertyName("TypeName")]
        public string TypeName { get; set; }

        [JsonProperty("TypeNameEn")]
        [JsonPropertyName("TypeNameEn")]
        public string TypeNameEn { get; set; }

        [JsonProperty("SefaID")]
        [JsonPropertyName("SefaID")]
        public int SefaID { get; set; }

        [JsonProperty("SefaName")]
        [JsonPropertyName("SefaName")]
        public string SefaName { get; set; }

        [JsonProperty("NationalNumber")]
        [JsonPropertyName("NationalNumber")]
        public object NationalNumber { get; set; }

        [JsonProperty("CRNumber")]
        [JsonPropertyName("CRNumber")]
        public object CRNumber { get; set; }

        [JsonProperty("KararNumber")]
        [JsonPropertyName("KararNumber")]
        public object KararNumber { get; set; }

        [JsonProperty("MalakiNumber")]
        [JsonPropertyName("MalakiNumber")]
        public object MalakiNumber { get; set; }

        [JsonProperty("DocumentTypeName")]
        [JsonPropertyName("DocumentTypeName")]
        public object DocumentTypeName { get; set; }

        [JsonProperty("CompanyRepresentTypeID")]
        [JsonPropertyName("CompanyRepresentTypeID")]
        public object CompanyRepresentTypeID { get; set; }

        [JsonProperty("CompanyRepresentTypeName")]
        [JsonPropertyName("CompanyRepresentTypeName")]
        public object CompanyRepresentTypeName { get; set; }

        [JsonProperty("SakkNumber")]
        [JsonPropertyName("SakkNumber")]
        public object SakkNumber { get; set; }

        [JsonProperty("CompanyName")]
        [JsonPropertyName("CompanyName")]
        public object CompanyName { get; set; }
    }

    

    public class Party
    {
        [JsonProperty("IdentityNo")]
        [JsonPropertyName("IdentityNo")]
        public string IdentityNo { get; set; }

        [JsonProperty("PersonId")]
        [JsonPropertyName("PersonId")]
        public int PersonId { get; set; }

        [JsonProperty("FullName")]
        [JsonPropertyName("FullName")]
        public string FullName { get; set; }

        [JsonProperty("IdentityTypeId")]
        [JsonPropertyName("IdentityTypeId")]
        public int IdentityTypeId { get; set; }

        [JsonProperty("IdentityTypeName")]
        [JsonPropertyName("IdentityTypeName")]
        public string IdentityTypeName { get; set; }

        [JsonProperty("GenderId")]
        [JsonPropertyName("GenderId")]
        public int GenderId { get; set; }

        [JsonProperty("GenderName")]
        [JsonPropertyName("GenderName")]
        public string GenderName { get; set; }

        [JsonProperty("NationalityId")]
        [JsonPropertyName("NationalityId")]
        public int NationalityId { get; set; }

        [JsonProperty("NationalityName")]
        [JsonPropertyName("NationalityName")]
        public string NationalityName { get; set; }

        [JsonProperty("IsValid")]
        [JsonPropertyName("IsValid")]
        public bool IsValid { get; set; }

        [JsonProperty("BirthDate")]
        [JsonPropertyName("BirthDate")]
        public string BirthDate { get; set; }

        [JsonProperty("BirthDateHijri")]
        [JsonPropertyName("BirthDateHijri")]
        public string BirthDateHijri { get; set; }

        [JsonProperty("BirthDateHijriArabic")]
        [JsonPropertyName("BirthDateHijriArabic")]
        public string BirthDateHijriArabic { get; set; }

        [JsonProperty("ClearanceDateHijri")]
        [JsonPropertyName("ClearanceDateHijri")]
        public string ClearanceDateHijri { get; set; }

        [JsonProperty("ClearanceDateHijriArabic")]
        [JsonPropertyName("ClearanceDateHijriArabic")]
        public string ClearanceDateHijriArabic { get; set; }

        [JsonProperty("ClearanceDateGreg")]
        [JsonPropertyName("ClearanceDateGreg")]
        public string ClearanceDateGreg { get; set; }

        [JsonProperty("PartySefaTypeId")]
        [JsonPropertyName("PartySefaTypeId")]
        public int PartySefaTypeId { get; set; }

        [JsonProperty("PartySefaTypeName")]
        [JsonPropertyName("PartySefaTypeName")]
        public string PartySefaTypeName { get; set; }

        [JsonProperty("RepresentationText")]
        [JsonPropertyName("RepresentationText")]
        public string RepresentationText { get; set; }

        [JsonProperty("WekalaPersonId")]
        [JsonPropertyName("WekalaPersonId")]
        public int WekalaPersonId { get; set; }

        [JsonProperty("PartyValidityDisplayStatus")]
        [JsonPropertyName("PartyValidityDisplayStatus")]
        public bool PartyValidityDisplayStatus { get; set; }

        [JsonProperty("RelatedAttorneysCount")]
        [JsonPropertyName("RelatedAttorneysCount")]
        public int RelatedAttorneysCount { get; set; }

        [JsonProperty("AddedWithName")]
        [JsonPropertyName("AddedWithName")]
        public string AddedWithName { get; set; }

        [JsonProperty("PassportNo")]
        [JsonPropertyName("PassportNo")]
        public string PassportNo { get; set; }

        [JsonProperty("PassportDate")]
        [JsonPropertyName("PassportDate")]
        public string PassportDate { get; set; }

        [JsonProperty("PassportDateHijri")]
        [JsonPropertyName("PassportDateHijri")]
        public string PassportDateHijri { get; set; }

        [JsonProperty("PassportDateHijriArabic")]
        [JsonPropertyName("PassportDateHijriArabic")]
        public string PassportDateHijriArabic { get; set; }

        [JsonProperty("PassportExpiryDate")]
        [JsonPropertyName("PassportExpiryDate")]
        public string PassportExpiryDate { get; set; }

        [JsonProperty("PassportExpiryDateHijri")]
        [JsonPropertyName("PassportExpiryDateHijri")]
        public object PassportExpiryDateHijri { get; set; }

        [JsonProperty("PassportSource")]
        [JsonPropertyName("PassportSource")]
        public string PassportSource { get; set; }

        [JsonProperty("FirstName")]
        [JsonPropertyName("FirstName")]
        public string FirstName { get; set; }

        [JsonProperty("FatherName")]
        [JsonPropertyName("FatherName")]
        public string FatherName { get; set; }

        [JsonProperty("FamilyName")]
        [JsonPropertyName("FamilyName")]
        public string FamilyName { get; set; }

        [JsonProperty("GFName")]
        [JsonPropertyName("GFName")]
        public string GFName { get; set; }

        [JsonProperty("FaskhCode")]
        [JsonPropertyName("FaskhCode")]
        public object FaskhCode { get; set; }

        [JsonProperty("FaskhKetabaPlaceID")]
        [JsonPropertyName("FaskhKetabaPlaceID")]
        public object FaskhKetabaPlaceID { get; set; }

        [JsonProperty("FaskhKetabaPlaceName")]
        [JsonPropertyName("FaskhKetabaPlaceName")]
        public object FaskhKetabaPlaceName { get; set; }

        [JsonProperty("OldAttorneyNumber")]
        [JsonPropertyName("OldAttorneyNumber")]
        public object OldAttorneyNumber { get; set; }

        [JsonProperty("Identification")]
        [JsonPropertyName("Identification")]
        public object Identification { get; set; }

        [JsonProperty("PartySefa")]
        [JsonPropertyName("PartySefa")]
        public object PartySefa { get; set; }

        [JsonProperty("MobileNumber")]
        [JsonPropertyName("MobileNumber")]
        public string MobileNumber { get; set; }

        [JsonProperty("OutsideNationalID")]
        [JsonPropertyName("OutsideNationalID")]
        public string OutsideNationalID { get; set; }

        [JsonProperty("OutsideSocialIDType")]
        [JsonPropertyName("OutsideSocialIDType")]
        public int OutsideSocialIDType { get; set; }

        [JsonProperty("OutsideSocialIDTypeName")]
        [JsonPropertyName("OutsideSocialIDTypeName")]
        public object OutsideSocialIDTypeName { get; set; }

        [JsonProperty("GCCIssueDate")]
        [JsonPropertyName("GCCIssueDate")]
        public string GCCIssueDate { get; set; }

        [JsonProperty("VisaExpiryDate")]
        [JsonPropertyName("VisaExpiryDate")]
        public string VisaExpiryDate { get; set; }
    }

    public class TextValue
    {
        [JsonProperty("ControlID")]
        [JsonPropertyName("ControlID")]
        public int ControlID { get; set; }

        [JsonProperty("ControlValue")]
        [JsonPropertyName("ControlValue")]
        public object ControlValue { get; set; } // Can be bool or string

        [JsonProperty("ControlLable")]
        [JsonPropertyName("ControlLable")]
        public string ControlLable { get; set; }

        [JsonProperty("ControlAttorneyTypeName")]
        [JsonPropertyName("ControlAttorneyTypeName")]
        public string ControlAttorneyTypeName { get; set; }

        [JsonProperty("SelectedText")]
        [JsonPropertyName("SelectedText")]
        public object SelectedText { get; set; }

        [JsonProperty("ControlAttorneyTypeID")]
        [JsonPropertyName("ControlAttorneyTypeID")]
        public int ControlAttorneyTypeID { get; set; }

        [JsonProperty("ParentControlID")]
        [JsonPropertyName("ParentControlID")]
        public int ParentControlID { get; set; }

        [JsonProperty("IsHiddenWhenNoValue")]
        [JsonPropertyName("IsHiddenWhenNoValue")]
        public bool IsHiddenWhenNoValue { get; set; }
    }
}
