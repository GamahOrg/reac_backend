namespace REAC.Domain.Shared.Enum
{
    public enum InvoiceStatus
    {
        //مستحقة السداد
        Payable = 1,
        //مسددة
        Paid = 2,
        //ملغاة لعدم السداد
        CanceledForNonPayment
    }
}
