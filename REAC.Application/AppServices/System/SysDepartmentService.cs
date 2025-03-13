using AutoMapper;
using Azure;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.System.AreaDto;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;


namespace REAC.Application.AppServices.System
{
    public class SysDepartmentService : BasicService<SysDepartment, Guid>, ISysDepartmentService
    {
        public SysDepartmentService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }

        public async Task<BaseResponse> ToggleAutoDistributionOnAndOff(int departmentType, int status)
        {
            try
            {
                var toggleResult = (await _repository.GetQueryable<SysDepartment, Guid>())
                                     .Where(x => x.DepartmentType == (DepartmentType)departmentType)
                                     .ExecuteUpdateAsync(upd => upd.SetProperty(cr => cr.DistributionMode, (DepartmentDistributionMode)status));

                if (toggleResult.Result != 1)
                    return new BaseResponse() { IsDone = true, Message = "حدث خطأ اثناء التعديل" };

                var getDitributionMode = await GetDistributionModeByDepartmentType(departmentType);
                getDitributionMode.Message = "تم التعديل بنجاح";

                return getDitributionMode;

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }

        }

        public async Task<BaseResponse> GetDistributionModeByDepartmentType(int departmentType)
        {
            try
            {
                var distributionMode = (await _repository.GetQueryable<SysDepartment, Guid>())?
                                     .FirstOrDefault(x => x.DepartmentType == (DepartmentType)departmentType)?.DistributionMode;

                return new DataResponse<int?>() { IsDone = true, Data = distributionMode != null ? (int)distributionMode : null };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }

        }
    }
}
