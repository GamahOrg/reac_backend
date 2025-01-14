using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;



namespace REAC.Application.AppServices.System
{
    public class SysLitigationRequestOwnerTypeService : BasicService<SysLitigationRequestOwnerType, LitigationRequestOwnerType>, ISysLitigationRequestOwnerTypeService
    {
        public SysLitigationRequestOwnerTypeService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
    }
}
