using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.AppServices.Wpf.System
{
    public class WorkFlowStepWorkModeService : BasicService<WorkFlowStepWorkMode, StepWorkMode>, IWorkFlowStepWorkModeService
    {
        public WorkFlowStepWorkModeService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
    }
}
