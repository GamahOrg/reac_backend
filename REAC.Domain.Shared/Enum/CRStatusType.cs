namespace REAC.Domain.Shared.Enum
{
    public enum CRStatusType
    {

        /// <remarks/>
        Active,

        /// <remarks/>
        Expired,

        /// <remarks/>
        Suspended,

        /// <remarks/>
        Cancelled,
    }

    public enum CRStatusTypeEnum
    {
        // نشط
        Active = 1,
        // معلق
        Suspended = 2,
        //مشطوب
        Deleted = 3,

    }
}
