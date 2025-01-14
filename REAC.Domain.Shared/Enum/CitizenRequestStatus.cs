namespace REAC.Domain.Shared.Enum
{
    public enum CitizenRequestStatus
    {
        ///<summary>مسودة</summary>
        Draft = 1,
        ///<summary> بإنتظار الاعتماد</summary>
        WaitingForCertify = 2,
        ///<summary>موافقة مع وجود رسوم</summary>
        WaitingForMembershipFeesToBePaid = 3,
        ///<summary>معتمد</summary>
        Certified = 4,
        ///<summary>تم الرفض</summary>
        Rejected = 5,
        ///<summary>تم الإرسال للتعديل</summary>
        SentForEdit = 6,
        ///<summary>تم التعديل والإرسال</summary>
        SentFromEdit = 7,
        ///<summary>تم الارسال لدفع الرسوم</summary>
        SendToPayFees = 8,
        ///<summary>تم دفع الرسوم</summary>
        FeesPaid = 9,
    }

}
