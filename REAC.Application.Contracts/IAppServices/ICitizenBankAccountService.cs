using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Citizen;

namespace REAC.Application.Contracts.IAppServices
{
    public interface ICitizenBankAccountService
    {
        Task<BaseResponse> GetCitizenBankAccount();
        Task<BaseResponse> UpdateCitizenBankAccount(CitizenBankAccountCreateUpdateDto citizenBankAccountCreateUpdateDto);
    }
}
