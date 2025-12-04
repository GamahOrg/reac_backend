using Core.Security.Attributes;
using Core.Security.Authorization;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Integration;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Invoice;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IMobileAppServices.Litigation;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Api.Controllers.Integration
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Integration)]
    [PermissionRequired(Modules.LitigationRequestInvoice)]
    public class LitigationRequestInvoiceController : ControllerBase
    {
        private readonly ILitigationRequestService _litigationService;
        public LitigationRequestInvoiceController(ILitigationRequestService litigationService)
        {
            _litigationService = litigationService;

        }
        [HttpPost(nameof(SendInvoiceStatus))]
        public async Task<IActionResult> SendInvoiceStatus([FromBody] UpdateInvoiceDto model,
           CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new BaseResponse
                {
                    IsDone = false,
                    Message = "Invalid request data"
                });
            }
            var returnValue = await _litigationService.SendLitigationInvoiceRequestPaid(model);
            return Ok(returnValue);
            //// Dummy logic to decide success or failure
            //bool isSpecificTransaction = model.InvoiceId == "2511207200022004"
            //                             || model.ReferecnceId == "0b4fe481-7130-42f4-af8e-65845faf6d49";

            //if (isSpecificTransaction)
            //{
            //    return Ok(new BaseResponse
            //    {
            //        IsDone = true,
            //        Message = "Invoice updated successfully"
            //    });
            //}
            //else
            //{
            //    return BadRequest(new BaseResponse
            //    {
            //        IsDone = false,
            //        Message = "Failed to update invoice , this invoice is not exist"
            //    });
            //}
        }





    }

}
