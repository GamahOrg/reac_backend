using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;


namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> GetWorkflowStepEventsForApplicant(Guid requestId, CancellationToken cancellationToken)
        {
            try
            {
                var processinglog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                    .Where(x => x.RequestId == requestId &
                                x.IsFinished == false &
                                x.WorkFlowStep.ActorTypeId == StepActorType.Client)
                    .Include(x => x.WorkFlowStep)
                    .ThenInclude(x => x.WorkFlowStepEvents)
                    .ThenInclude(x => x.Translations).ToList();

                List<WorkFlowStepEventShortDto> workFlowStepEventShortDtos = new List<WorkFlowStepEventShortDto>();
                processinglog?.ForEach(x =>
                {
                    x.WorkFlowStep.WorkFlowStepEvents.ToList().ForEach(y =>
                    {
                        workFlowStepEventShortDtos.Add(new WorkFlowStepEventShortDto()
                        {
                            Id = y.Id,
                            EventName = y.EventName,
                            AllowFileUpload = y.AllowFileUpload,
                            AllowNotes = y.AllowNotes,
                            ProcessingLogId = x.Id,
                            CustomActionType = y.CustomActionType
                        });
                    });
                });

                return new DataResponse<List<WorkFlowStepEventShortDto>>()
                {
                    Data = workFlowStepEventShortDtos,
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
