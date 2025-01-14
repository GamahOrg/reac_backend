using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationRequestNoteFileService : BasicService<LitigationRequestNoteFile, Guid>, ILitigationRequestNoteFileService
    {
        public LitigationRequestNoteFileService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
    }
}
