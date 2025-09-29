using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysLitigationRequestAmountService : IBasicService<SysLitigationRequestAmount, Guid>
    {
        Task<string?> GetLitigationRequestAmount(Guid LitigationRequestAmountId);
        Task<BaseResponse> GetOrderedListAsync();
        Task<DataResponse<SysLitigationRequestAmountCreateUpdateDto>> GetByIdAsync(Guid id);
        Task<BaseResponse> Create(SysLitigationRequestAmountCreateUpdateDto sysLitigationRequestAmountCreateUpdateDto, CancellationToken cancellationToken);
        Task<BaseResponse> UpdateAsync(Guid id, SysLitigationRequestAmountCreateUpdateDto sysLitigationRequestAmountCreateUpdateDto, CancellationToken cancellationToken);
        Task<BaseResponse> Delete(Guid id, CancellationToken cancellationToken);
        Task<BaseResponse> GetRegisterationFees(decimal amount);
    }
}
