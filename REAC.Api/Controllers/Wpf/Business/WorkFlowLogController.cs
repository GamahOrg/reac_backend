using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Api;
using Core.Security.Attributes;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowLog;
using REAC.Application.Contracts.IAppServices.Wpf.Business;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace Bpf.Api.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.WpfBusiness)]
    [PermissionRequired(Modules.Basic)]


    public class WorkFlowLogController : BaseController<WorkFlowLog,
        WorkFlowLogCreateUpdateDto, WorkFlowLogCreateUpdateDto
        , Guid>
    {

        public WorkFlowLogController(IWorkFlowLogService WorkFlowLogService) :
            base(WorkFlowLogService)
        {
        }
    }
}

