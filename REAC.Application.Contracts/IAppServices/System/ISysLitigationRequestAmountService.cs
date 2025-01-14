using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysLitigationRequestAmountService : IBasicService<SysLitigationRequestAmount, Guid>
    {
        Task<string?> GetLitigationRequestAmount(Guid LitigationRequestAmountId);
        Task<BaseResponse> GetOrderedListAsync();
    }
}
