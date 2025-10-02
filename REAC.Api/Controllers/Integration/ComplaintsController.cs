using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Integration;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers.Integration
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Integration)]
    [AllowAnonymous]
    public class ComplaintsController : ControllerBase
    {
        private readonly IComplaintsService _complaintService;
        public ComplaintsController(IComplaintsService complaintService)
        {
            _complaintService = complaintService;
        }

        [HttpGet(nameof(GetCasesByContactAsync))]
        public async Task<IActionResult> GetCasesByContactAsync(string nationalId)
          => Ok(await _complaintService.GetCasesByContactAsync(nationalId));
        
        [HttpGet(nameof(GetContactAsync))]
        public async Task<IActionResult> GetContactAsync(string nationalId)
          => Ok(await _complaintService.GetContactAsync(nationalId));
        
        [HttpGet(nameof(GetRegionListAsync))]
        public async Task<IActionResult> GetRegionListAsync()
          => Ok(await _complaintService.GetRegionListAsync());
        
        [HttpPost(nameof(UploadComplaintFileAsync))]
        public async Task<IActionResult> UploadComplaintFileAsync(UploadComplaintFileNoteDto input)
          => Ok(await _complaintService.UploadComplaintFileAsync(input));
        
        [HttpPost(nameof(CreateComplaintAsync))]
        public async Task<IActionResult> CreateComplaintAsync(CreateComplaintRequestDto input)
          => Ok(await _complaintService.CreateComplaintAsync(input));
    }
}
