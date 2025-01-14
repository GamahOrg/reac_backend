using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.IAppServices
{

    public interface ILitigationRatingService : IBasicService<LitigationRequest, Guid>
    {
        Task<BaseResponse> GetRatings(Guid litigationId);
        Task<BaseResponse> AddRating(Guid litigationId, Guid CitizenId, int RatingValue);
        //   Task<BaseResponse> UpdateRating(Guid RatingId, int RatingValue);
        Task<BaseResponse> DeleteRating(Guid RatingId);
    }
}
