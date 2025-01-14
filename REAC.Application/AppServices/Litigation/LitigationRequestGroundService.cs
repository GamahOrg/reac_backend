using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.EntityFrameworkCore.Entities.Litigation;


namespace REAC.Application.AppServices.Litigation
{
    public class CitizenService : BasicService<LitigationRequestGround, Guid>, ILitigationRequestGroundService
    {
        public CitizenService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
        public async Task<BaseResponse> AddReference(LitigationRequestGroundCreateUpdateDto model)
        {
            return await base.AddAsync(model);
        }
        public async Task<Guid> GetAddedReferenceId()
        {
            var quarable = await GetQueryableAsync();
            var latestAdded = quarable.OrderByDescending(x => x.Id).FirstOrDefault();
            return latestAdded.Id;
        }
    }
}
