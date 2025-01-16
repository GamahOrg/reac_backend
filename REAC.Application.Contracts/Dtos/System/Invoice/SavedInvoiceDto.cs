using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.System.Invoice
{
    public class SavedInvoiceDto : BaseEntity<Guid>
    {
        public string InvoiceCode { get; set; }
    }
}
