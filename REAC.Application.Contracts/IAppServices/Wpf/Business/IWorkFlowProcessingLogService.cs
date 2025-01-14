using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.Contracts.IAppServices.Wpf.Business
{
    public interface IWorkFlowProcessingLogService : IBasicService<WorkFlowProcessingLog, Guid>
    {
        Task<BaseResponse> GetAuditNotes(Guid litigationId);
    }
}
