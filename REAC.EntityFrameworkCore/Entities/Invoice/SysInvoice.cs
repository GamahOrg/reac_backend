using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using Microsoft.EntityFrameworkCore;

namespace REAC.EntityFrameworkCore.Entities.Invoice
{
    public class SysInvoice : CreationBaseEntity<Guid>
    {
        [Comment("Unique identifier of the request")]
        public Guid RequestId { get; set; }
        [Comment("Unique identifier of the applicant citizen")]
        public Guid ApplicantCitizenId { get; set; }
        [Comment("Invoice code")]
        public string InvoiceCode { get; set; } = null!;
        [Comment("Base amount of the invoice before tax")]
        public decimal Amount { get; set; }
        [Comment("Invoice tax amount")]
        public decimal InvoiceTax { get; set; }
        [Comment("Total amount of the invoice including tax")]
        public decimal TotalAmount { get; set; }
        [Comment("Unique identifier of the invoice type")]
        public InvoiceType InvoiceTypeId { get; set; }
        [Comment("Status identifier")]
        public InvoiceStatus InvoiceStatusId { get; set; }
        [Comment("Expiration date of the invoice")]
        public DateTime ExpirationDate { get; set; }
        [Comment("Date when the invoice was paid")]
        public DateTime? PaymentDate { get; set; }
        [Comment("Date when the invoice was cancelled")]
        public DateTime? CancelledDate { get; set; }
        public virtual SysInvoiceStatus InvoiceStatus { get; set; } = null!;
        public virtual SysInvoiceType InvoiceType { get; set; } = null!;
        public virtual LitigationRequest LitigationRequest { get; set; } = null!;
    }
}


