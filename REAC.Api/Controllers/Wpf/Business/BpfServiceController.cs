using Core.Security.Attributes;
using Core.Security.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.IModuleServices;
using REAC.Domain.Shared.Constants;

namespace Bpf.Api.Controllers.Business
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.WpfBusiness)]
    [PermissionRequired(Modules.Basic)]

    public class BpfServiceController : ControllerBase
    {

        IBpfService _bpfService;
        public BpfServiceController(IBpfService bpfService)
        {
            _bpfService = bpfService;
        }

        [HttpPost(nameof(ExcuteWorkFlow))]
        public virtual async Task<IActionResult> ExcuteWorkFlow(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.ExcuteWorkFlow(requestId, cancellationToken);
                return Ok(returnValue);
            }
            return BadRequest();
        }

        [HttpPost(nameof(ProcessWorkFlow))]
        public virtual async Task<IActionResult> ProcessWorkFlow([FromBody] ProcessRequestPayload requestPayload, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.ProcessWorkFlow(requestPayload, cancellationToken);
                return Ok(returnValue);
            }
            return BadRequest();
        }

        [HttpGet(nameof(GetWorkflowStepEventsForApplicant) + "/requestId")]
        public virtual async Task<IActionResult> GetWorkflowStepEventsForApplicant(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.GetWorkflowStepEventsForApplicant(requestId, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [HttpGet(nameof(GetWorkflowStepEventsForEmployee) + "/requestId")]
        public virtual async Task<IActionResult> GetWorkflowStepEventsForEmployee(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.GetWorkflowStepEventsForEmployee(requestId, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [HttpPost(nameof(Assign))]
        public virtual async Task<IActionResult> Assign([FromBody] AssignRequestPayload assignRequestPayload, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.Assign(assignRequestPayload, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [HttpGet(nameof(GetRequestLogForEmployee) + "/requestId")]
        public virtual async Task<IActionResult> GetRequestLogForEmployee(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.GetRequestLogForEmployee(requestId, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [HttpGet(nameof(GetRequestLogForApplicant) + "/requestId")]
        public virtual async Task<IActionResult> GetRequestLogForApplicant(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.GetRequestLogForApplicant(requestId, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }

        [HttpGet(nameof(GetRequestProcessingLogForEmployee) + "/requestId")]
        public virtual async Task<IActionResult> GetRequestProcessingLogForEmployee(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.GetRequestProcessingLogForEmployee(requestId, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }
        [HttpGet(nameof(GetWorkflowStepEventsForReformer) + "/requestId")]
        public virtual async Task<IActionResult> GetWorkflowStepEventsForReformer(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.GetWorkflowStepEventsForReformer(requestId, cancellationToken);
                return Ok(returnValue);
            }
            return BadRequest();
        }
        [HttpGet(nameof(GetRequestProcessingLogForApplicant) + "/requestId")]
        public virtual async Task<IActionResult> GetRequestProcessingLogForApplicant(Guid requestId, CancellationToken cancellationToken)
        {
            if (ModelState.IsValid)
            {
                var returnValue = await _bpfService.GetRequestProcessingLogForApplicant(requestId, cancellationToken);
                return Ok(returnValue);
            }

            return BadRequest();
        }


    }
}
