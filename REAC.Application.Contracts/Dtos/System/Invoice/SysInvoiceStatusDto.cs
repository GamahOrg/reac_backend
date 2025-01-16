using Core.GenericRepository.Shared.BaseEntities;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.System.Invoice
{
    public class SysInvoiceStatusDto : BaseEntity<InvoiceStatus>
    {
        public string Name { get; set; } = null!;
    }
}
