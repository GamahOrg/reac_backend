using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.System;
using REAC.EntityFrameworkCore.Entities.System;
using static Dapper.SqlMapper;
using System.Threading;
using System.Runtime.InteropServices;


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

        public async Task<BaseResponse> GetOrderedListAsync()
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

        public async Task<BaseResponse> Create(SysLitigationRequestAmountCreateUpdateDto sysLitigationRequestAmountCreateUpdateDto, CancellationToken cancellationToken)
        {
            try
            {
                var checkIfExist = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())
                .Any(x => x.FromAmount == sysLitigationRequestAmountCreateUpdateDto.FromAmount && x.ToAmount == sysLitigationRequestAmountCreateUpdateDto.ToAmount && x.Fees == sysLitigationRequestAmountCreateUpdateDto.Fees);

                if (checkIfExist)
                    return new BaseResponse() { IsDone = false, Message = "هذه الرسوم الأدارية موجودة مسبقا" };

                SysLitigationRequestAmount originalEntity = _mapper.Map<SysLitigationRequestAmount>(sysLitigationRequestAmountCreateUpdateDto);
                await _repository.AddAsync<SysLitigationRequestAmount, Guid>(originalEntity, cancellationToken);
                var response = await _repository.SaveChangesAsync(cancellationToken);
                if (response.IsDone)
                {
                    return new DataResponse<SysLitigationRequestAmount>()
                    {
                        IsDone = true,
                        Data = originalEntity,
                        Message = response.Message
                    };
                }
                return response;
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }

        }
    }
}
