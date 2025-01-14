using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.AppServices.Wpf.System
{
    public class WorkFlowNotificationPlanService : BasicService<WorkFlowNotificationPlan, Guid>, IWorkFlowNotificationPlanService
    {
        public WorkFlowNotificationPlanService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
    }
}
