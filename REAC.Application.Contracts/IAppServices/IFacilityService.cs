using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Facility;

namespace REAC.Application.Contracts.IAppServices
{
    public interface IFacilityService
    {
        Task<BaseResponse> GetAllFacilitiesAsync(int pageIndex, int pageSize, string? searchKey);
        Task<BaseResponse> GetAllFacilitiesDropdown();
        Task<BaseResponse> GetFacilityByIdAsync(Guid facilityId);
        Task<BaseResponse> EditFacilityAsync(EditFacilityRequestDto input);
        Task<BaseResponse> ValidateAndAddFacility(string crNumber, Guid? citizenId);
    }
}
