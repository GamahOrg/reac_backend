using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Api;
using Core.Security.Attributes;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepWorkMode;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace Bpf.Api.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.WpfSystem)]
    [PermissionRequired(Modules.Basic)]

    public class WorkFlowStepWorkModeController : BaseController<WorkFlowStepWorkMode,
        WorkFlowStepWorkModeCreateUpdateDto, WorkFlowStepWorkModeDto
        , StepWorkMode>
    {
        public WorkFlowStepWorkModeController(IWorkFlowStepWorkModeService WorkFlowStepWorkModeService) :
            base(WorkFlowStepWorkModeService)
        {
        }
    }
}


