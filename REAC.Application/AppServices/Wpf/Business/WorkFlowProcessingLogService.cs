using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Wpf.Business;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.AppServices.Wpf.Business
{
    public class WorkFlowProcessingLogService : BasicService<WorkFlowProcessingLog, Guid>, IWorkFlowProcessingLogService
    {
        public WorkFlowProcessingLogService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader) : base(repository, mapper, loggerManager, jwtReader)
        {

        }

        public async Task<BaseResponse> GetAuditNotes(Guid litigationId)
        {
            var quarble = await GetQueryableAsync();
            var result = quarble.Include(x => x.WorkFlowProcessingLogFiles).ThenInclude(x => x.FileManager)
                .Where(x => x.RequestId == litigationId & x.Notes != null & x.Notes != "").ToList();

            var mapped = _mapper.Map<List<AuditNotesDto>>(result);
            return new DataResponse<List<AuditNotesDto>> { Data = mapped.OrderBy(x => x.CreatedOn).ToList(), IsDone = true };
        }
    }
}
