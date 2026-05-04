using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Rating;
using REAC.Application.Contracts.IAppServices;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.AppServices.Litigation;

public class BeneficiarySatisfactionSurveyService : BasicService<BeneficiarySatisfactionSurveyDto, Guid>, IBeneficiarySatisfactionSurveyService
{
    private readonly REACDBContext _context;

    public BeneficiarySatisfactionSurveyService(
        REACDBContext context,
        IEntityRepository repository,
        IMapper mapper,
        ILoggerManager loggerManager,
        IJwtReader jwtReader)
        : base(repository, mapper, loggerManager, jwtReader)
    {
        _context = context;
    }

    public async Task<BaseResponse> AddSurvey(BeneficiarySatisfactionSurveyDto dto)
    {
        var alreadyRated = await _context.BeneficiarySatisfactionSurveys
            .AnyAsync(s => s.LitigationRequestId == dto.LitigationRequestId
                        && s.CitizenId == dto.CitizenId
                        && s.StageType == dto.StageType);

        if (alreadyRated)
        {
            return new BaseResponse { IsDone = false, Message = "SurveyAlreadySubmitted" };
        }

        //if (dto.IsRated)
        //{
        //    if (dto.ProcedureSatisfaction is null or < 1 or > 5
        //        || dto.PerformanceSatisfaction is null or < 1 or > 5)
        //    {
        //        return new BaseResponse { IsDone = false, Message = "InvalidRatingValue" };
        //    }
        //}
        //else
        //{
        //    dto.ProcedureSatisfaction = null;
        //    dto.PerformanceSatisfaction = null;
        //}
        dto.CreatedOn = DateTime.Now;
        var result= await _repository.AddAsync<BeneficiarySatisfactionSurvey, Guid>(new BeneficiarySatisfactionSurvey
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            LitigationRequestId = dto.LitigationRequestId,
            CitizenId = dto.CitizenId,
            IsRated = dto.IsRated,
            PerformanceSatisfaction = dto.PerformanceSatisfaction,
            ProcedureSatisfaction = dto.ProcedureSatisfaction,
            StageType = dto.StageType,
        });
        var Response = await _repository.SaveChangesAsync();
        return Response;
    }

    public async Task<DataResponse<bool>> HasUserRated(Guid litigationRequestId, Guid citizenId, int stageType)
    {
        var exists = await _context.BeneficiarySatisfactionSurveys
            .AnyAsync(s => s.LitigationRequestId == litigationRequestId
                        && s.CitizenId == citizenId
                        && s.StageType == stageType);

        return new DataResponse<bool> { IsDone = true, Data = exists };
    }
}
