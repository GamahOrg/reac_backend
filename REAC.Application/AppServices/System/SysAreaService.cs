using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.System
{
    public partial class SysAreaService : BasicService<SysArea, Guid>, ISysAreaService
    {
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;
        private readonly ISysFileManagerService _fileService;
        public SysAreaService(ISysFileManagerService fileService, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen) : base(repository, mapper, loggerManager, jwtReader)
        {
            _fileService = fileService;
            _jwtReader = jwtReader;
            _mapper = mapper;
            _citizen = citizen;
        }

    }
}
