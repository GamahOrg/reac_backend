using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace REAC.Application.Contracts.IAppServices.Wpf.System
{
    public interface IWorkFlowStepService : IBasicService<WorkFlowStep, Guid>
    {
        Task<Guid> GetLowestEmployee();
    }
}
