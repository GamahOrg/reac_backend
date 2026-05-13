using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation.Rating;

namespace REAC.Application.Contracts.IAppServices;

public interface IBeneficiarySatisfactionSurveyService : IBasicService<BeneficiarySatisfactionSurveyDto, Guid>
{
    Task<BaseResponse> AddSurvey(BeneficiarySatisfactionSurveyDto dto);
    Task<DataResponse<bool>> HasUserRated(Guid litigationRequestId, Guid citizenId, int stageType);
}
