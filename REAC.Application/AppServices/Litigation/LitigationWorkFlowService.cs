using AutoMapper;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationWorkFlowService : ILitigationWorkFlowService
    {
        private readonly ILitigationRequestService _litigationRequestService;
        private readonly IMapper _mapper;

        public LitigationWorkFlowService(IMapper mapper, ILitigationRequestService litigationRequestService)
        {
            _litigationRequestService = litigationRequestService;
            _mapper = mapper;
        }
        public async Task<BaseResponse> UpdateLitigationStatusAsync(ResponseWorkFlowDto model, Guid requestId)
        {
            var litigationRquest = await _litigationRequestService.Get(requestId) as DataResponse<LitigationRequest>;
            litigationRquest.Data.InternalStatusId = model.InternalStatus.Value;
            litigationRquest.Data.ExternalStatusId = model.ExternalStatus.Value;
            var litiationCreateUpdateDto = _mapper.Map<LitigationCreateUpdateDto>(litigationRquest.Data);
            if (litiationCreateUpdateDto != null)
                await _litigationRequestService.AddLitigation(litiationCreateUpdateDto);
            return litigationRquest;
        }
    }
}
