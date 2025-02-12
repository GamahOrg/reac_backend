using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.System.DepartmentDtos;
using REAC.Application.Contracts.Dtos.System.EmployeeDtos;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysEmployeeService : IBasicService<SysEmployee, Guid>
    {
        Task<DataResponse<SysEmployee>> GetAdminsByDepartmentType(DepartmentType deptType);
        Task<BaseResponse> CreateEmployee(SysEmployeeDto sysEmployeeDto);
        Task<DataResponse<SysEmployee>> GetEmployeeByDepartmentType(DepartmentType deptType);
        Task<DataResponse<List<SysEmployee>>> GetAllEmployeeByDepartmentType(DepartmentType deptType);
        Task<BaseResponse> List(int pageIndex, int pageSize, string? searchKe, bool? isActive);
        Task<BaseResponse> UpdateEmployeeStatus(Guid employeeId, bool isActive);
        Task<DataResponse<List<SysDepartmentDistributionModeDto>>> GetDistributioonMode();
        Task<BaseResponse> UpdateDistributioonMode(DepartmentDistributionMode distributionMode);

    }
}
