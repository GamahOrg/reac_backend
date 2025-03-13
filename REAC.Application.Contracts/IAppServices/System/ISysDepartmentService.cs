using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysDepartmentService : IBasicService<SysDepartment, Guid>
    {
        Task<BaseResponse> ToggleAutoDistributionOnAndOff(int departmentType, int status);
        Task<BaseResponse> GetDistributionModeByDepartmentType(int departmentType);
    }
}
