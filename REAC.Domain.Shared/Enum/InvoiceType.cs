namespace REAC.Domain.Shared.Enum
{
    public enum InvoiceType
    {
        // الرسوم الإدارية للدعوى
        AdministrativeForLawsuitFees = 1,
        //رسوم المصالحة
        ReconciliationFees = 2,
        //رسوم التحكيم
        ArbitrationFees = 3,
        //رسوم تسجيل محكم
        ArbitratorRegistrationFees = 4,
        //رسوم تسجيل مصلح
        ReconciliatorRegistrationFees = 5,
        //رسوم عضوية مصلح
        ReconciliatorMembershipFees = 6,
        //رسوم عضوية محكم
        ArbitratorMembershipFees = 7
    }
}
