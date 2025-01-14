using Core.Shared.Responses;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysLitigationRequestContractTypeService : IBasicService<SysLitigationRequestContractType, LitigationRequestContractType>
    {
        Task<BaseResponse> GetListAsync(CancellationToken cancellationToken);
        Task<BaseResponse> GetTermsConditionsAsync(string contractNumber, CancellationToken cancellationToken);
    }
}
