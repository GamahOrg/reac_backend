using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Invoice
{
    public class SysInvoice : CreationBaseEntity<Guid>
    {
        public Guid RequestId { get; set; }
        public Guid ApplicantCitizenId { get; set; }
        public string InvoiceCode { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal InvoiceTax { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceType InvoiceTypeId { get; set; }
        public InvoiceStatus InvoiceStatusId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public virtual SysInvoiceStatus InvoiceStatus { get; set; } = null!;
        public virtual SysInvoiceType InvoiceType { get; set; } = null!;
    }
}
