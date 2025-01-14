using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;



namespace REAC.Application.AppServices.Reconciliation
{
    public class LitigationRequestExtendPeriodRequestFileService : BasicService<LitigationRequestExtendPeriodRequestFile, Guid>, ILitigationRequestExtendPeriodRequestFileService
    {
        public LitigationRequestExtendPeriodRequestFileService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
    }
}
