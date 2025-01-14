using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.AppServices.Wpf.System
{
    public class WorkFlowStepService : BasicService<WorkFlowStep, Guid>, IWorkFlowStepService
    {
        public WorkFlowStepService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }

        public async Task<Guid> GetLowestEmployee()
        {
            Guid? employeeId = new Guid();
            var quarable = await GetQueryableAsync();
            employeeId = quarable
               .Where(x => x.StepName == "Audit" && !x.IsDeleted)
               .Select(x => x.WorkFlowProcessingLogs
                   .Where(s => !s.IsFinished)
                   .GroupBy(s => s.EmployeeId)
                   .Select(g => g.Key)
                   .Min())
               .FirstOrDefault();
            if (employeeId != null)
            {
                return employeeId.Value;
            }
            else
            {
                return Guid.Parse("3afafe63-62e5-4bc9-a8b9-7713c42e7097");
            }

        }
    }
}
