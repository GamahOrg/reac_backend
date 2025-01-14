using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysArbitratorClassService : IBasicService<SysArbitratorClass, Guid>
    {
        Task<BaseResponse> GetArbitratorClasses(int experienceYears);
        Task<BaseResponse> GetArbitratorClass(int experienceYears);
    }
}
