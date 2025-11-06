using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.System.Invoice;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Invoice;

namespace REAC.Api.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.System)]
    [AllowAnonymous]

    public class SysInvoiceController : BaseController<SysInvoice, CreateInvoiceDto, InvoiceDto, Guid>
    {
        private readonly ISysInvoiceService _sysInvoiceService;

        public SysInvoiceController(ISysInvoiceService sysInvoiceService) : base(sysInvoiceService)
        {
            _sysInvoiceService = sysInvoiceService;
        }

        [HttpGet(nameof(GetPagedList))]
        public async Task<ActionResult> GetPagedList([FromQuery] int pageIndex, [FromQuery] int pageSize)
        {
            return Ok(await _sysInvoiceService.GetPagedList(pageIndex, pageSize));

        }

        [HttpGet(nameof(GetInvoiceId))]
        public async Task<ActionResult> GetInvoiceId([FromQuery] Guid id)
        {
            return Ok(await _sysInvoiceService.GetInvoiceId(id));

        }


        [HttpGet(nameof(PayInvoice))]
        public async Task<ActionResult> PayInvoice([FromQuery] Guid id)
        {
            return Ok(await _sysInvoiceService.PayInvoice(id));

        }

    }
}
