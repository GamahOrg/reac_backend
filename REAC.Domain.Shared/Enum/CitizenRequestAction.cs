namespace REAC.Domain.Shared.Enum
{
    public enum CitizenRequestAction
    {
        ///<summary>أرسال</summary>
        Send = 1,
        ///<summary>موافقة مع عدم وجود رسوم</summary>
        CertifiedWithOutFees = 2,
        ///<summary>موافقة مع وجود رسوم</summary>
        CertifiedWithFees = 3,
        ///<summary>رفض</summary>
        Reject = 4,
        ///<summary>إرسال للتعديل</summary>
        SendForEdit = 5,
        ///<summary>تم التعديل والإرسال</summary>
        SendFromEdit = 6,
        ///<summary>ارسال اشعار لدفع الرسوم</summary>
        SendToPayFees = 7,
        ///<summary>اتمام دفع الرسوم</summary>
        FeesPaid = 8
    }
}
