using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Globalization;

namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> GetWorkflowStepEventsForReformer(Guid requestId, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var processinglog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                    .Where(x => x.RequestId == requestId &
                           x.IsFinished == false & x.WorkFlowStep.ProcessActorType == ProcessActorType.Reformer)
                    .Include(x => x.Employee)
                    .Include(x => x.WorkFlowStep)
                    .ThenInclude(x => x.WorkFlowStepEvents)
                    .ThenInclude(x => x.WorkFlowStepEventSecurityLevelActors).ToList();

                if (processinglog == null)
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "not  defined!!"
                    };
                }

                WorkFlowStepEventForActorShortDto WorkFlowStepEventForActorShortDtos = new WorkFlowStepEventForActorShortDto();

                processinglog?.ForEach(x =>
                {
                    x.WorkFlowStep.WorkFlowStepEvents.ToList().ForEach(y =>
                    {
                        WorkFlowStepEventForActorShortDtos.WorkFlowStepEventShortDtos.Add(new WorkFlowStepEventShortDto()
                        {
                            Id = y.Id,
                            EventName = (y.Translations.FirstOrDefault(
                            x => x.LanguageCode == CultureInfo.CurrentUICulture.Name) == null || string.IsNullOrEmpty(y.Translations.FirstOrDefault(
                            x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).EventName)) ? y.EventName : y.Translations.FirstOrDefault(
                            x => x.LanguageCode == CultureInfo.CurrentUICulture.Name).EventName,
                            AllowFileUpload = y.AllowFileUpload,
                            AllowNotes = y.AllowNotes,
                            ProcessingLogId = x.Id,
                            CustomActionType = y.CustomActionType
                        });

                    });
                });

                return new DataResponse<WorkFlowStepEventForActorShortDto>()
                {
                    Data = WorkFlowStepEventForActorShortDtos,
                    IsDone = true
                };
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }
    }
}
