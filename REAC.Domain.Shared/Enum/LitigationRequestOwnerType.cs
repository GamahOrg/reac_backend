namespace REAC.Domain.Shared.Enum;



public enum LitigationRequestOwnerType
{
    ///<summary>مقدم عن نفسه</summary>
    Personal = 1,
    ///<summary>ممثل شركة</summary>
    CompanyRepresentative = 2,
    ///<summary>محامي عن شركة</summary>
    LawyerForCompany = 3,
    ///<summary> وكيل عن شركة</summary>
    AgentForCompany = 4,
    ///<summary>محامي عن فرد</summary>
    LawyerForIndividual = 5,
    ///<summary> وكيل عن فرد</summary>
    AgentForIndividual = 6
}

