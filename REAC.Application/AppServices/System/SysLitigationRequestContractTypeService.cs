using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.System.LitigationRequestContractTypeDtos;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices.System
{
    public class SysLitigationRequestContractTypeService : BasicService<SysLitigationRequestContractType, LitigationRequestContractType>, ISysLitigationRequestContractTypeService
    {
        private readonly IUserProviderService _contractservice;
        public SysLitigationRequestContractTypeService(IUserProviderService contractservice, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {
            _contractservice = contractservice;
        }
        public async Task<BaseResponse> GetListAsync(CancellationToken cancellationToken)
        {
            return await base.GetListAsync<SysLitigationRequestContractTypeGetDto>(cancellationToken);
        }
        public async Task<BaseResponse> GetTermsConditionsAsync(string contractNumber, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.Contract)
                          .Include(x => x.Contract.LitigationRequestContractType).Include(x => x.ApplicantCitizen).AsNoTracking()
                          .Where(x => x.Contract.ContractKey == contractNumber).ToList().FirstOrDefault();

            if (litigationRequest != null)
            {
                return new DataResponse<SysLitigationRequestContractTypeDto>
                {
                    Data = new SysLitigationRequestContractTypeDto
                    {
                        AcknowledgmentContent = (litigationRequest.Contract.LitigationRequestContractType.AcknowledgmentContent == null) ? null :
                        $"{litigationRequest.Contract.LitigationRequestContractType.AcknowledgmentContent.Replace("{اسم المدعي}",
                        litigationRequest.Contract.LitigationRequests.FirstOrDefault().ApplicantCitizen.FullName).Replace("{رقم هوية المدعي}",
                        litigationRequest.Contract.LitigationRequests.FirstOrDefault().ApplicantCitizen.IdentityNumber).Replace("{مصدر الهوية}",
                        litigationRequest.Contract.LitigationRequests.FirstOrDefault().ApplicantCitizen.District).Replace("{تاريخ الميلادي لليوم}",
                        DateTime.Now.ToString())}",
                        TypeName = litigationRequest.Contract.LitigationRequestContractType.CodingPrefixSymbol,
                        TermsAndConditions = litigationRequest.Contract.LitigationRequestContractType.TermsAndConditions
                    },
                    IsDone = true
                };

            }

            return new BaseResponse
            {
                IsDone = false
            };


        }
    }
}
