using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Api;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLog;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Application.Contracts.IAppServices.Wpf.Business;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace Bpf.Api.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.WpfBusiness)]
    [PermissionRequired(Modules.Basic)]


    public class WorkFlowProcessingLogController : BaseController<WorkFlowProcessingLog,
        WorkFlowProcessingLogCreateUpdateDto, WorkFlowProcessingLogCreateUpdateDto
        , Guid>
    {
        private readonly IWorkFlowProcessingLogService _WorkFlowLogService;

        public WorkFlowProcessingLogController(IWorkFlowProcessingLogService WorkFlowProcessingLogService, IWorkFlowProcessingLogService WorkFlowLogService) :
            base(WorkFlowProcessingLogService)
        {
            _WorkFlowLogService = WorkFlowLogService;
        }
        [HttpGet(nameof(GetAuditNotesById))]
        [AllowAnonymous]
        public async Task<ActionResult<AuditNotesDto>> GetAuditNotesById(Guid litigationId)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _WorkFlowLogService.GetAuditNotes(litigationId);

                return Ok(returnValue);
            }

            return BadRequest();
        }
    }
}

