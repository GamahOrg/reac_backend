namespace REAC.Domain.Shared.Enum;



public enum LitigationRequestOwnerType
{
    ///<summary>مقدم عن نفسه</summary>
    Personal = 1,
    ///<summary>ممثل منشأة</summary>
    CompanyRepresentative = 2,
    ///<summary> وكيل منشأة</summary>
    AgentForCompany = 3,
    ///<summary>ممثل فرد</summary>
    IndividualRepresentative = 4,
    ///<summary> وكيل فرد</summary>
    AgentForIndividual = 5,

    // NEW
    ///<summary>مدير شركة</summary>
    CompanyManager = 6,
    ///<summary>رئيس مجلس الادارة </summary>
    CompanyCEO = 7,
    ///<summary>مصفي</summary>
    Filterer = 8,
    ///<summary>حارس قضائي</summary>
    JudicialGuard = 9,
    ///<summary>ولي</summary>
    Custodian = 10,
    ///<summary>العضو المنتدب</summary>
    ManagingDirector = 11,
    ///<summary>أمين تفليسة</summary>
    BankruptcyTrustee = 12

}

