using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Invoice
{
    public class SysInvoiceStatus : BaseEntity<InvoiceStatus>
    {
        public string Name { get; set; } = null!;
        public virtual ICollection<SysInvoice> Invoices { get; set; } = new List<SysInvoice>();
        public virtual ICollection<SysInvoiceStatusTranslation> Translations { get; set; } = new List<SysInvoiceStatusTranslation>();

    }
}
