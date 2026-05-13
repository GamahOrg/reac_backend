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
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using System.IdentityModel.Tokens.Jwt;

namespace REAC.Application.AppServices.System
{
    public class SysLitigationRequestContractTypeService : BasicService<SysLitigationRequestContractType, LitigationRequestContractType>, ISysLitigationRequestContractTypeService
    {
        private readonly IUserProviderService _contractservice;
        private readonly IJwtReader _jwtReader;
        public SysLitigationRequestContractTypeService(IUserProviderService contractservice, IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {
            _contractservice = contractservice;
            _jwtReader = jwtReader;

        }

        public async Task<BaseResponse> GetAll()
        {
            var litigationRequestContractTypes = (await _repository.GetQueryable<SysLitigationRequestContractType, LitigationRequestContractType>())
                 .ToList();
                
            if(litigationRequestContractTypes.Any()) 
                return new DataResponse<List<SysLitigationRequestContractType>>() { IsDone = true , Data = litigationRequestContractTypes };

            return new DataResponse<List<SysLitigationRequestContractType>>() { IsDone = false, Data = null };

        }

        public async Task<BaseResponse> GetListAsync(CancellationToken cancellationToken)
        {
            return await base.GetListAsync<SysLitigationRequestContractTypeGetDto>(cancellationToken);
        }
        public async Task<BaseResponse> GetTermsConditionsAsync(string contractNumber, string litigationId, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.Contract)
                          .Include(x => x.Contract.LitigationRequestContractType).Include(x => x.ApplicantCitizen).AsNoTracking()
                          .Where(x => x.Contract.ContractKey == contractNumber).ToList().FirstOrDefault();

            var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                      .Include(x => x.Gender)
                      .Include(x => x.City)
                      .Include(x => x.Area)
                      .Include(x => x.Nationality)
                      .AsNoTracking()
                      .Where(x => x.Id == referenceId)
                      .FirstOrDefault();

            if (litigationRequest != null)
            {
                return new DataResponse<SysLitigationRequestContractTypeDto>
                {
                    Data = new SysLitigationRequestContractTypeDto
                    {
                        AcknowledgmentContent = (litigationRequest.Contract?.LitigationRequestContractType.AcknowledgmentContent == null) ? null :
                        $"{litigationRequest.Contract?.LitigationRequestContractType.AcknowledgmentContent.Replace("{اسم المدعي}",
                        citizen.FullName).Replace("{رقم هوية المدعي}",
                        citizen.IdentityNumber).Replace("{مصدر الهوية}",
                        citizen.District).Replace("{تاريخ الميلادي لليوم}",
                        DateTime.Now.ToString())}",
                        TypeName = litigationRequest.Contract?.LitigationRequestContractType.CodingPrefixSymbol,
                        TermsAndConditions = litigationRequest.Contract?.LitigationRequestContractType.TermsAndConditions
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
