    using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationRequestGroundFileService : BasicService<LitigationRequestGroundFile, Guid>, ILitigationRequestGroundFileService
    {
        public LitigationRequestGroundFileService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
        public async Task<BaseResponse> AddReferenceFiles(LitigationRequestGroundFileCreateUpdateDto model)
        {
            return await base.AddAsync(model);
        }
    }
}
