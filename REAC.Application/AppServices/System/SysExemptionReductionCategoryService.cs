using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.System.LitigationRequestAmountDtos;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices;
using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using REAC.Application.Contracts.Dtos.System.LitigationRequestCategoryDtos;

namespace REAC.Application.AppServices.System
{
    public class SysExemptionReductionCategoryService : BasicService<SysExemptionReductionCategory, Guid>, ISysExemptionReductionCategoryService
    {
        public SysExemptionReductionCategoryService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
    }
}
