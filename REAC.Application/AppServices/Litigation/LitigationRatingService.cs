using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Litigation.Rating;
using REAC.Application.Contracts.IAppServices;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationRatingService : BasicService<LitigationRatingDto, Guid>, ILitigationRatingService
    {
        private readonly REACDBContext _context;
        private readonly IMapper _mapper;
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;

        public LitigationRatingService(REACDBContext context, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen) : base(repository, mapper, loggerManager, jwtReader)
        {
            _context = context;
            _mapper = mapper;
            _jwtReader = jwtReader;
            _citizen = citizen;

        }

        public async Task<BaseResponse> GetRatings(Guid litigationId)
        {
            var quarble = await GetQueryableAsync();
            var result = quarble.Include(x => x.RatingValue).FirstOrDefault(x => x.LitigationId == litigationId);
            return new DataResponse<LitigationRatingDto> { Data = result, IsDone = true };
        }

        public async Task<BaseResponse> AddRating(Guid litigationId, Guid citizenId, int ratingValue)
        {

            var ratingUser = new LitigationRequest();

            if (ratingUser.Id == litigationId && ratingUser.ApplicantCitizenId == citizenId)
            {
                var newRating = new LitigationRatingDto
                {
                    LitigationId = litigationId,
                    CitizenId = citizenId,
                    PretenderRating = ratingValue,
                    RatedOn = DateTime.UtcNow
                };
                return await base.AddAsync(newRating);
            }
            else
            {
                var newRating = new LitigationRatingDto
                {
                    LitigationId = litigationId,
                    CitizenId = citizenId,
                    DefendantRating = ratingValue,
                    RatedOn = DateTime.UtcNow
                };
                return await base.AddAsync(newRating);
            }

        }

        //public async Task<LitigationRatingDto> UpdateRating(Guid citizenId, int newRatingValue)
        //{
        //    var existingRating = await _repository.GetByIdAsync<LitigationRatingDto, Guid>(citizenId, default);
        //    existingRating.RatingValue = newRatingValue;
        //    return await base.UpdateAsync(existingRating);
        //}
        public async Task<BaseResponse> DeleteRating(Guid RatingId)
        {
            return await base.RemoveAsync(RatingId);
        }
    }
}
