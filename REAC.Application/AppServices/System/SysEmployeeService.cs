using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Security.Entities;
using Core.Shared.Constants;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.System.EmployeeDtos;
using REAC.Application.Contracts.Dtos.UserManage;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;
using System.IdentityModel.Tokens.Jwt;

namespace REAC.Application.AppServices.System
{
    public class SysEmployeeService : BasicService<SysEmployee, Guid>, ISysEmployeeService
    {
        private readonly UserManager<User> _userManager;

        public SysEmployeeService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, UserManager<User> userManager) : base(repository, mapper, loggerManager, jwtReader)
        {
            _userManager = userManager;
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

        public async Task<BaseResponse> CreateEmployee(SysEmployeeDto sysEmployeeDto)
        {
            try
            {

                // Check Asp User
                var isExistUser = _userManager.Users.Any(x => x.UserName == sysEmployeeDto.IdentityNumber);

                if (isExistUser)
                    return new BaseResponse() { IsDone = false, Message = "رقم الهويه موجود مسبقا" };


                var newEmployee = new SysEmployee()
                {
                    Id = Guid.NewGuid(),
                    AllowAssign = true,
                    AllowReAssign = true,
                    CreatedOn = DateTime.Now,
                    DepartmentId = sysEmployeeDto.DepartmentId,
                    Email = sysEmployeeDto.Email,
                    Mobile = sysEmployeeDto.Mobile,
                    EmployeeName = sysEmployeeDto.EmployeeName,
                    IsActive = sysEmployeeDto.IsActive,
                    IsAdmin = sysEmployeeDto.IsAdmin,
                    Translations = new List<SysEmployeeTranslation>()
                                 {
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = sysEmployeeDto.IsAdmin ? "Department admin" : "Department Employee",
                                         LanguageCode=Constants.EnglishLanguage
                                     },
                                     new SysEmployeeTranslation()
                                     {
                                         EmployeeName = sysEmployeeDto.EmployeeName,
                                         LanguageCode=Constants.ArabicLanguage
                                     }
                                 }
                };

                User user = new User { UserName = sysEmployeeDto.IdentityNumber.Trim(), Email = sysEmployeeDto.Email.Trim(), EmailConfirmed = true, ReferenceId = newEmployee.Id };


                var identityUser = await _userManager.CreateAsync(user, "123456");
                if (!identityUser.Succeeded || identityUser.Errors.Any())
                    return new BaseResponse() { IsDone = false, Message = "خطأ في تسجيل الحساب" };

                newEmployee.UserId = user.Id;

                var addNewEmployee = await _repository.AddAsync<SysEmployee, Guid>(newEmployee);


                var result = await _repository.SaveChangesAsync();
                if (!result.IsDone)
                {

                    var deleteIdentityUser = await _userManager.DeleteAsync(user);

                    return result;
                }

                return new BaseResponse() { IsDone = true, Message = "تم أضافة موظف بنجاح" };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }
        public async Task<BaseResponse> List(int pageIndex, int pageSize, string? searchKey, bool? isActive)
        {
            PagedResult<SysEmployee> sysEmployees = null;

            var employees = await GetQueryableAsync();
            employees = (employees.Include(x => x.Department));

            if (!string.IsNullOrEmpty(searchKey))
            {
                employees = (employees.Where(l => l.Mobile.Contains(searchKey) ||
                                                                            l.Department.DepartmentName.Contains(searchKey) ||
                                                                             l.EmployeeName.Contains(searchKey)));
            }
            if (isActive.HasValue) 
            {
                employees = (employees.Where(l => l.IsActive == isActive.Value));

            } 

            sysEmployees = employees.OrderByDescending(x => x.CreatedOn)
                                               .AsSplitQuery()
                                               .AsNoTracking()
                                               .GetPaged(pageIndex, pageSize);


            var list = _mapper.Map<List<SysEmployeeListDto>>(sysEmployees.Results);

            return new DataResponse<PagedResult<SysEmployeeListDto>>
            {
                Data =
                  new PagedResult<SysEmployeeListDto>
                  {
                      Results = list,
                      CurrentPage = sysEmployees.CurrentPage,
                      PageCount = sysEmployees.PageCount,
                      PageSize = sysEmployees.PageSize,
                      TotalCount = sysEmployees.TotalCount
                  },
                IsDone = true
            };

        }
        public async Task<BaseResponse> UpdateEmployeeStatus(Guid employeeId, bool isActive)
        {
            try
            {
                var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?
                      .Where(x => x.Id == employeeId)
                      .SingleOrDefault();
        
                if (employee == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير موجود"
                    };

                employee.IsActive = isActive;   

                await _repository.SaveChangesAsync();
                
                if (isActive == true)   
                    return new BaseResponse() { IsDone = true, Message = "تم تفعيل حساب الوظف" };
                else
                    return new BaseResponse() { IsDone = true, Message = "تم إيقاف حساب الوظف" };

            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.ToString(),
                    Message = ex.Message
                };
            }


        }
    }
}
