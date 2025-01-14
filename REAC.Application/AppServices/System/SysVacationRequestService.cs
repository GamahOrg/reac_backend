using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using Core.GenericRepository.Shared.Pagging;
using System.Runtime.CompilerServices;
using REAC.Application.Contracts.Dtos.System.EmployeeDtos;
using REAC.Application.Contracts.Dtos.System.VacationDtos;
using iText.Kernel.Geom;
using REAC.Domain.Shared.Enum;
using SysVacationRequestCreateUpdateDto = REAC.Application.Contracts.Dtos.System.VacationDtos.SysVacationRequestCreateUpdateDto;

namespace REAC.Application.AppServices.System
{
    public class SysVacationRequestService : BasicService<SysVacationRequest, Guid>, ISysVacationRequestService
    {
        private readonly ISysEmployeeService _employeeService;
        private readonly IJwtReader _jwtReader;
        public SysVacationRequestService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysEmployeeService employeeService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _repository = repository;
            _jwtReader = jwtReader;
            _employeeService = employeeService;
        }

        public async Task<BaseResponse> CreateVacationAsync(REAC.Application.Contracts.Dtos.System.VacationDtos.SysVacationRequestCreateUpdateDto holidayRequest)
        {
            try
            {
                Guid citizenId = Guid.Empty;
                _jwtReader.GetReferenceId(ref citizenId);


                var vacation = await GetQueryableAsync();

                if (vacation.Any(x => x.EmployeeId == citizenId && x.IsApproved == null))
                    return new BaseResponse { IsDone = false, Message = "لا يمكن اضافة أجازة جديدة قبل انتهاء دراسة طلب الاجازة السابق" };

                if (vacation.Any(x => x.EmployeeId == citizenId && x.StartDate == holidayRequest.StartDate && x.EndDate == holidayRequest.EndDate))
                    return new BaseResponse { IsDone = false, Message = "يوجد طلب اجازة في نفس فترة الاجازة" };


                SysVacationRequest sysVacationRequest = new SysVacationRequest();
                sysVacationRequest.EmployeeId = citizenId;
                sysVacationRequest.StartDate = holidayRequest.StartDate;
                sysVacationRequest.EndDate = holidayRequest.EndDate;
                sysVacationRequest.Reason = holidayRequest.Reason;
                await _repository.AddAsync<SysVacationRequest, Guid>(sysVacationRequest);
                var saveResponse = await _repository.SaveChangesAsync();
                if (saveResponse.IsDone)
                    return saveResponse;
                else
                    return new BaseResponse { IsDone = false };

            }
            catch (Exception ex)
            {
                return new BaseResponse { IsDone = false, Message = "خطأ داخلي" };

            }

        }
        public async Task<BaseResponse> ProcessVacationRequestAsync(Guid requestId, bool result)
        {
            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);
            var vacationRequest = (await _repository.GetQueryable<SysVacationRequest, Guid>()).Where(x => x.Id == requestId).SingleOrDefault();

            if (vacationRequest == null)
            {
                return new BaseResponse { IsDone = false, Message = "طلب اجازة غير موجود" };

            }


            var employeeList = await _employeeService.GetAllEmployeeByDepartmentType(DepartmentType.Review);

            if (result == true)
            {
                //var empId = employeeList.Data.Where(x => x.EmployeeVacationRequests.All(v => v.IsApproved == null || v.IsApproved == false) 
                // || !x.EmployeeVacationRequests.Any(v => v.IsApproved == true && (v.StartDate <= DateTime.Today && v.EndDate >= DateTime.Today))).FirstOrDefault()?.Id;

                //if (employeeList.Data.Count(x => x.IsAdmin == false) > 1 || employeeList.Data.FirstOrDefault(x => x.Id == vacationRequest.EmployeeId).IsAdmin == true)
                //{
                vacationRequest.ApprovedBy = citizenId;
                vacationRequest.IsApproved = true;
                vacationRequest.ApprovedOn = DateTime.UtcNow;
                await _repository.SaveChangesAsync();
                return new BaseResponse { IsDone = true, Message = "تم قبول طلب الاجازة" };
                //}
                //else 
                //    return new BaseResponse { IsDone = false, Message = "لا يوجد موظف تدقيق اخر للعمل علي الدعاوي" };


            }
            else

            vacationRequest.ApprovedBy = citizenId;
            vacationRequest.IsApproved = false;
            vacationRequest.ApprovedOn = DateTime.UtcNow;
            await _repository.SaveChangesAsync();
            return new BaseResponse { IsDone = true, Message = "تم رفض طلب الاجازة" };

        }

        public async Task<BaseResponse> EmployeeList(int pageIndex, int pageSize)
        {
            PagedResult<SysVacationRequest> vacationRequests = null;

            var vacation = await GetQueryableAsync();
            vacation = vacation.Include(x => x.SysApprovedByEmployee);
            vacation = vacation.Include(x => x.SysEmployee);

            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);

            vacationRequests = vacation.Where(x => x.EmployeeId == citizenId)
                                               .OrderByDescending(x => x.StartDate)
                                               .AsSplitQuery()
                                               .AsNoTracking()
                                               .GetPaged(pageIndex, pageSize);


            var list = _mapper.Map<List<SysVacationRequestCreateUpdateDto>>(vacationRequests.Results);

            return new DataResponse<PagedResult<SysVacationRequestCreateUpdateDto>>
            {
                Data =
                  new PagedResult<SysVacationRequestCreateUpdateDto>
                  {
                      Results = list,
                      CurrentPage = vacationRequests.CurrentPage,
                      PageCount = vacationRequests.PageCount,
                      PageSize = vacationRequests.PageSize,
                      TotalCount = vacationRequests.TotalCount
                  },
                IsDone = true
            };
        }
        public async Task<BaseResponse> AdminList(int pageIndex, int pageSize)
        {
            PagedResult<SysVacationRequest> vacationRequests = null;

            var vacation = await GetQueryableAsync();
            vacation = vacation.Include(x => x.SysApprovedByEmployee);
            vacation = vacation.Include(x => x.SysEmployee);
            vacationRequests = vacation
                                               //.Where(x => x.IsApproved == null)
                                               .OrderByDescending(x => x.StartDate)
                                               .AsSplitQuery()
                                               .AsNoTracking()
                                               .GetPaged(pageIndex, pageSize);
            var list = _mapper.Map<List<SysVacationRequestCreateUpdateDto>>(vacationRequests.Results);



            return new DataResponse<PagedResult<SysVacationRequestCreateUpdateDto>>
            {
                Data =
                  new PagedResult<SysVacationRequestCreateUpdateDto>
                  {
                      Results = list,
                      CurrentPage = vacationRequests.CurrentPage,
                      PageCount = vacationRequests.PageCount,
                      PageSize = vacationRequests.PageSize,
                      TotalCount = vacationRequests.TotalCount
                  },
                IsDone = true
            };

        }


    }
}
