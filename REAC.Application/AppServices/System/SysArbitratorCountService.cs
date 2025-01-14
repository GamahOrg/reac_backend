using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.System
{
    public partial class SysArbitratorCountService : BasicService<SysArbitratorCount, Guid>, ISysArbitratorCountService
    {
        private readonly IJwtReader _jwtReader;
        private readonly ICitizenService _citizen;
        private readonly ISysFileManagerService _fileService;
        public SysArbitratorCountService(ISysFileManagerService fileService, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ICitizenService citizen) : base(repository, mapper, loggerManager, jwtReader)
        {
            _fileService = fileService;
            _jwtReader = jwtReader;
            _mapper = mapper;
            _citizen = citizen;
        }
        public async override Task<BaseResponse> GetListAsync<TEntityCreateUpdateDto>(CancellationToken cancellationToken = default)
        {
            var response = await _repository.GetListAsync<SysArbitratorCount, Guid>(cancellationToken);
            if (!response.IsDone)
                return response;

            List<SysArbitratorCountCreateUpdateDto> entityDtos
                = _mapper.Map<List<SysArbitratorCountCreateUpdateDto>>
                (((DataResponse<List<SysArbitratorCount>>)response).Data);
            return new DataResponse<List<SysArbitratorCountCreateUpdateDto>>()
            {
                IsDone = response.IsDone,
                Data = entityDtos.OrderBy(x => x.ArbitratorCount).ToList(),
                Message = response.Message
            };
        }
    }
}
