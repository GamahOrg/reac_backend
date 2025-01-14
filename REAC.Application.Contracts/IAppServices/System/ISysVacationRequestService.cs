using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.System.VacationDtos;
using REAC.EntityFrameworkCore.Entities;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.Contracts.IAppServices.System
{
    public interface ISysVacationRequestService : IBasicService<SysVacationRequest, Guid>
    {

        Task<BaseResponse> EmployeeList(int pageIndex, int pageSize);
        Task<BaseResponse> AdminList(int pageIndex, int pageSize);
        Task<BaseResponse> CreateVacationAsync(REAC.Application.Contracts.Dtos.System.VacationDtos.SysVacationRequestCreateUpdateDto holidayRequest);
        Task<BaseResponse> ProcessVacationRequestAsync(Guid requestId, bool result);
    }
}
