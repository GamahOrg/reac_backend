using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.EntityFrameworkCore.Entities.Invoice
{
    public class SysInvoiceType : BaseEntity<InvoiceType>
    {
        public string Name { get; set; } = null!;
        public virtual ICollection<SysInvoice> Invoices { get; set; } = new List<SysInvoice>();
        public virtual ICollection<SysInvoiceTypeTranslation> Translations { get; set; } = new List<SysInvoiceTypeTranslation>();

    }
}
