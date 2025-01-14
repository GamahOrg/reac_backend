using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.EntityFrameworkCore.Entities.System;


namespace REAC.Application.AppServices.System
{
    public class SysLitigationRequestAmountService : BasicService<SysLitigationRequestAmount, Guid>, ISysLitigationRequestAmountService
    {
        public SysLitigationRequestAmountService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }
        public async Task<string?> GetLitigationRequestAmount(Guid LitigationRequestAmountId)
        {
            var quarable = await GetQueryableAsync();
            var litigationRequestAmount = quarable.FirstOrDefault(x => x.Id == LitigationRequestAmountId)?.LitigationAmount;
            return litigationRequestAmount;
        }
        public async override Task<BaseResponse> GetListAsync<TEntityCreateUpdateDto>(CancellationToken cancellationToken = default)
        {
            var response = await _repository.GetListAsync<SysLitigationRequestAmount, Guid>(cancellationToken);
            if (!response.IsDone)
                return response;

            List<SysLitigationRequestAmountCreateUpdateDto> entityDtos
                = _mapper.Map<List<SysLitigationRequestAmountCreateUpdateDto>>
                (((DataResponse<List<SysLitigationRequestAmount>>)response).Data);
            return new DataResponse<List<SysLitigationRequestAmountCreateUpdateDto>>()
            {
                IsDone = response.IsDone,
                Data = entityDtos.OrderBy(x => x.LitigationAmount).ToList(),
                Message = response.Message
            };
        }
        
        public async  Task<BaseResponse> GetOrderedListAsync()
        {
            var response = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())
                .OrderBy(x => x.Fees).ToList();
                
            List<SysLitigationRequestAmountCreateUpdateDto> entityDtos
                = _mapper.Map<List<SysLitigationRequestAmountCreateUpdateDto>>
                ((response));
            return new DataResponse<List<SysLitigationRequestAmountCreateUpdateDto>>()
            {
                IsDone = true,
                Data = entityDtos,
                
            };
        }
    }
}
