using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities;

namespace REAC.Application.Contracts.IAppServices
{
    public interface ICitizenService
    {
        Task<DateTime?> GetIdentityExpirationDate(Guid? userId);
        Task<ICollection<Facility>> GetRepresentativeCitizens(Guid? userId);
        Task<(Guid, bool)> CheckCitizenExist(string identityNumber);
        Task<BaseResponse> GetCitizenBasicData();
        Task<BaseResponse> UpdateCitizenBasicData(SaveCitizenBasicDataProfileDto citizenBasicDataProfileDto);
        Task<BaseResponse> GetDelegatedPersonsLawyer();
        Task<BaseResponse> GetDelegatedPersonsAgent();
        Task<BaseResponse> GetDelegatedFacilitiesLawyer();
        Task<BaseResponse> GetDelegatedFacilitiesAgent();


        Task<BaseResponse> GetFacilities(RelationShipType relationShipId);
        Task<BaseResponse> GetPersons(RelationShipType relationShipId);

        Task<BaseResponse> GetOwnerFacilities();
        Task<BaseResponse> GetReformersLookup(Guid? citizenId, Guid requestId);
        Task<BaseResponse> GetArbitratorsLookup(Guid? citizenId, Guid requestId);
        Task<BaseResponse> GetArbitratorsFullLookup(Guid? citizenId, Guid requestId);
        Task<BaseResponse> GetReformersDecisions(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        Task<BaseResponse> GetReformersDecisions(Guid citizenId);
        Task<BaseResponse> GetArbitratorsDecisions(int pageIndex, int pageSize, CancellationToken cancellationToken = default);
        Task<BaseResponse> GetArbitratorsDecisions(Guid citizenId);


        Task<BaseResponse> AddArbitratorFreeUpDecision(ArbitratorFreeUpDecisionCreateDto arbitratorFreeUpDecisionCreateDto);
        Task<BaseResponse> AddArbitratorStopDecision(ArbitratorStopDecisionCreateDto arbitratorStopDecisionCreateDto);
        Task<BaseResponse> AddReformerFreeUpDecision(ReformerFreeUpDecisionCreateDto reformerFreeUpDecisionCreateDto);
        Task<BaseResponse> AddReformerStopDecision(ReformerStopDecisionCreateDto reformerStopDecisionCreateDto);

        Task<BaseResponse> DeActivateReformerAccount(ReformerStopDecisionCreateDto reformerStopDecisionCreateDto);
        Task<BaseResponse> ActivateReformerAccount(ReformerFreeUpDecisionCreateDto reformerFreeUpDecisionCreateDto);
        Task<BaseResponse> DeActivateArbitratorAccount(ArbitratorStopDecisionCreateDto arbitratorStopDecisionCreateDto);
        Task<BaseResponse> ActivateArbitratorAccount(ArbitratorFreeUpDecisionCreateDto arbitratorFreeUpDecisionCreateDto);
    }
}
