using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Facility;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = APISections.Public)]
    //[Authorize]
    [AllowAnonymous]
    public class FacilityController : ControllerBase
    {
        private readonly IFacilityService _facilityService;
        public FacilityController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        [HttpGet(nameof(GetAllFacilities))]
        public async Task<ActionResult> GetAllFacilities([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] string? searchKey)
            => Ok(await _facilityService.GetAllFacilitiesAsync(pageIndex, pageSize, searchKey));

        [HttpGet(nameof(GetAllFacilitiesDropdown))]
        public async Task<ActionResult> GetAllFacilitiesDropdown()
            => Ok(await _facilityService.GetAllFacilitiesDropdown());



        [HttpGet(nameof(GetFacilityById))]
        public async Task<ActionResult> GetFacilityById(Guid facilityId)
            => Ok(await _facilityService.GetFacilityByIdAsync(facilityId));


        [HttpPost(nameof(EditFacility))]
        public async Task<ActionResult> EditFacility(EditFacilityRequestDto editFacilityRequestDto)
           => Ok(await _facilityService.EditFacilityAsync(editFacilityRequestDto));


        [HttpPost(nameof(ValidateAndAddFacility))]
        public async Task<ActionResult> ValidateAndAddFacility(AddFacilityRequestDto addFacilityRequestDto)
           => Ok(await _facilityService.ValidateAndAddFacility(addFacilityRequestDto.CRNumber, Guid.Empty));

    }
}
