using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Api;
using Core.Security.Attributes;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowNotificationPlan;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace Bpf.Api.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.WpfSystem)]
    [PermissionRequired(Modules.WorkFlowNotificationPlan)]

    public class WorkFlowNotificationPlanController : BaseController<WorkFlowNotificationPlan,
        WorkFlowNotificationPlanCreateUpdateDto, WorkFlowNotificationPlanDto
        , Guid>
    {

        public WorkFlowNotificationPlanController(IWorkFlowNotificationPlanService WorkFlowNotificationPlanService) :
            base(WorkFlowNotificationPlanService)
        {
        }
    }
}

