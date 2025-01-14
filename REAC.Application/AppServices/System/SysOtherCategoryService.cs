using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.AppServices.System
{
    public class SysOtherCategoryService: BasicService<OtherCategory, Guid>, ISysOtherCategoryService
    {
        public SysOtherCategoryService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
    }
    
}
