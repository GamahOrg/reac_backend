using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Create.Contract;
using REAC.Application.Contracts.Dtos.Litigation.Create.Defendant;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Contract;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface IUserProviderService : IBasicService<LitigationRequestContract, Guid>
    {
        Task<LitigationRequestContract> GetContract(string contractKey);
        Task<BaseResponse> AddModificationRequest(LitigationModificationRequestDto model);
        Task<BaseResponse> AddOtherContract(OtherCreateUpdateDto model);
        Task<BaseResponse> AddContractDocuments(ContractDocuments model);
        Task<BaseResponse> AddDefendant(DefendantCreateUpdateDto model);
        Task<BaseResponse> RentIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string contractNumber, string versionNumber);
        Task<BaseResponse> RealStateIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string contractNumber);
        Task<BaseResponse> WafiIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string WafiLicenseNo, WafiRequestOwnerType firstPartType, WafiRequestOwnerType secondPartyType);
        Task<BaseResponse> PropertiesIntegrate(LitigationRequestOwnerType FirstPartyCapacity, string associationCode, string associationName);
        Task<int> GetContractCountByContractType(LitigationRequestContractType litigationRequestContractTypeId);

    }
}
