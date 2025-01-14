using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.System
{
    public class SysEmployeeService : BasicService<SysEmployee, Guid>, ISysEmployeeService
    {
        public SysEmployeeService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }

        public async Task<DataResponse<SysEmployee>> GetAdminsByDepartmentType(DepartmentType deptType)
        {
            var DeptObj = (await _repository.GetQueryable<SysEmployee, Guid>())
                            .Include(e => e.Department)
                            .Where(x => x.Department.DepartmentType == deptType & x.IsAdmin == true & x.IsActive == true & x.IsDeleted != true)
                            .FirstOrDefault();
            if (DeptObj != null)
                return new DataResponse<SysEmployee> { Data = DeptObj, IsDone = true };
            else
                return new DataResponse<SysEmployee> { Data = null, IsDone = false };

        }

        public async Task<DataResponse<SysEmployee>> GetEmployeeByDepartmentType(DepartmentType deptType)
        {
            var DeptObj = (await _repository.GetQueryable<SysEmployee, Guid>())
                             .Include(e => e.Department)
                             .Where(x => x.Department.DepartmentType == deptType & x.IsAdmin == false & x.IsActive == true & x.IsDeleted != true)
                             .FirstOrDefault();
            if (DeptObj != null)
                return new DataResponse<SysEmployee> { Data = DeptObj, IsDone = true };
            else
                return new DataResponse<SysEmployee> { Data = null, IsDone = false };
        }
        
        public async Task<DataResponse<List<SysEmployee>>> GetAllEmployeeByDepartmentType(DepartmentType deptType)
        {
            var DeptObj = (await _repository.GetQueryable<SysEmployee, Guid>())
                             .Include(e => e.Department)
                             .Where(x => x.Department.DepartmentType == deptType & x.IsDeleted != true)
                             .ToList();
            if (DeptObj != null)
                return new DataResponse<List<SysEmployee>> { Data = DeptObj, IsDone = true };
            else
                return new DataResponse<List<SysEmployee>> { Data = null, IsDone = false };
        }
    }
}
