using Core.Shared.Responses;
using Microsoft.AspNetCore.Mvc;
using REAC.Application.Contracts.Dtos.Delegator;
using REAC.Application.Contracts.Dtos.Integration.GEE;

namespace REAC.Application.Contracts.IAppServices
{
    public interface IDelegatorService
    {
        Task<BaseResponse> GetDelegators(int pageIndex, int pageSize, string? searchKey);
        Task<BaseResponse> GetDelegatorById(Guid delegatorId);

        Task<BaseResponse> SaveDelegator(DelegatorDto citizenDelegatorCreateUpdateDto);
        Task<BaseResponse> ValidateAgentDelegator(ValidateDelegatorDto validateDelegatorDto);
        Task<DataResponse<PartiesDto>> GetDelegatorCitizenData(GetDeligatorCitizenData getDelegatorDataDto);

        Task<string> GenerateCitizenCode();

        Task<BaseResponse> GetLawyersDelegators(int pageIndex, int pageSize, string? searchKey);
        Task<BaseResponse> GetLawyerDelegatorById(Guid delegatorId);
        Task<BaseResponse> ValidateLawyerDelagator(ValidateLawyerDelegatorDto delegatorDto);
        Task<BaseResponse> AddLawyerDelagator(LawyerDelegatorDto delegatorDto);
        Task<BaseResponse> AddLawyerCitizenAndDelagator(LawyerDelegatorDto delegatorDto);
        Task<BaseResponse> DeleteLawyerDelegator(Guid delegatorId);
    }
}
