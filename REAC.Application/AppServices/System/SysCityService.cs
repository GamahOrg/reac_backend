using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.System.AreaDto;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.System
{
    public partial class SysCityService : BasicService<SysCity, Guid>, ISysCityService
    {
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;
        private readonly ISysFileManagerService _fileService;
        public SysCityService(ISysFileManagerService fileService, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen) : base(repository, mapper, loggerManager, jwtReader)
        {
            _fileService = fileService;
            _jwtReader = jwtReader;
            _mapper = mapper;
            _citizen = citizen;
        }

        public async Task<BaseResponse> GetListByAreaIdAsync<TEntityCreateUpdateDto>(Guid AreaId, CancellationToken cancellationToken = default)
        {
            var cities = (await _repository.GetQueryable<SysCity, Guid>())
                                     .Where(x => x.AreaId == AreaId).ToList();
            List<SysCityCreateUpdateDto> response
                       = _mapper.Map<List<SysCityCreateUpdateDto>>((List<SysCity>)cities);
            return new DataResponse<List<SysCityCreateUpdateDto>>()
            {
                IsDone = true,
                Data = response
            };
        }
    }
}
