using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.Invoice
{
    public class CreateInvoiceDto : BaseEntity<Guid>
    {
        public Guid ApplicantCitizenId { get; set; }
        public Guid RequestId { get; set; }
        public decimal Amount { get; set; }
        public decimal Taxes { get; set; }
        public decimal TotalAmount { get; set; }
        public InvoiceType InvoiceTypeId { get; set; }
    }
}
