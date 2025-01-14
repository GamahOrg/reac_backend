using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Api;
using Core.Security.Attributes;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlow;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace Bpf.Api.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.WpfSystem)]
    [PermissionRequired(Modules.WorkFlow)]

    public class WorkFlowController : BaseController<WorkFlow,
        WorkFlowCreateUpdateDto, WorkFlowDto
        , Guid>
    {
        public WorkFlowController(IWorkFlowService WorkFlowService) :
            base(WorkFlowService)
        {
        }
    }
}


