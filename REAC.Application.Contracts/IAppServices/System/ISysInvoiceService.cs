using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.System.Invoice;
using REAC.EntityFrameworkCore.Entities.Invoice;


namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysInvoiceService : IBasicService<SysInvoice, Guid>
    {
        Task<DataResponse<PagedResult<InvoiceDto>>> GetPagedList(int pageIndex, int pageSize);
        Task<DataResponse<InvoiceDto>> GetInvoiceId(Guid id);
        //Task<DataResponse<InvoiceDto>> GetInvoiceByRequest(Guid requestId, InvoiceType invoiceTypeId);
        Task<DataResponse<SavedInvoiceDto>> AddInvoice(CreateInvoiceDto invoiceDto, bool saveChnages = false);
        Task<BaseResponse> PayInvoice(Guid id);
    }
}
