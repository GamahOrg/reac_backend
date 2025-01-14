using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Api;
using Core.Security.Attributes;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace Bpf.Api.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.WpfSystem)]
    [PermissionRequired(Modules.WorkFlowStepEvent)]

    public class WorkFlowStepEventController : BaseController<WorkFlowStepEvent,
        WorkFlowStepEventCreateUpdateDto, WorkFlowStepEventDto
        , Guid>
    {
        public WorkFlowStepEventController(IWorkFlowStepEventService WorkFlowStepEventService) :
            base(WorkFlowStepEventService)
        {
        }
    }
}


