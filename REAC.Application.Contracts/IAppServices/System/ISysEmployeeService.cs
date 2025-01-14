using Core.Shared.Responses;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysEmployeeService : IBasicService<SysEmployee, Guid>
    {
        Task<DataResponse<SysEmployee>> GetAdminsByDepartmentType(DepartmentType deptType);
        Task<DataResponse<SysEmployee>> GetEmployeeByDepartmentType(DepartmentType deptType);
        Task<DataResponse<List<SysEmployee>>> GetAllEmployeeByDepartmentType(DepartmentType deptType);
    }
}
