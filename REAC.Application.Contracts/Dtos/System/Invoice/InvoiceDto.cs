using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.Invoice
{
    public class InvoiceDto : CreationBaseEntity<Guid>
    {
        public Guid RequestId { get; set; }
        public string RequestCode { get; set; }
        public string InvoiceCode { get; set; } = null!;
        public decimal Amount { get; set; }
        public decimal InvoiceTax { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceType InvoiceTypeId { get; set; }
        public InvoiceStatus InvoiceStatusId { get; set; }
        public DateTime ExpirationDate { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? CancelledDate { get; set; }
        public virtual SysInvoiceStatusDto InvoiceStatus { get; set; } = null!;
        public virtual SysInvoiceTypeDto InvoiceType { get; set; } = null!;
    }
}
