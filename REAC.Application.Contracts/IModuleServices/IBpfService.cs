using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Wpf.Business;

namespace REAC.Application.Contracts.IModuleServices
{
    public interface IBpfService
    {
        /// <summary>
        /// Excute Workflow
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="workflowId"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> ExcuteWorkFlow(Guid requestId, CancellationToken cancellationToken);

        /// <summary>
        /// Get Workflow Step Events For Employee
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="employeeId"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> GetWorkflowStepEventsForEmployee(Guid requestId, CancellationToken cancellationToken);

        /// <summary>
        /// Assign Request To Employee
        /// </summary>
        /// <param name="processingLogId"></param>
        /// <param name="employeeId"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> Assign(AssignRequestPayload assignRequestPayload, CancellationToken cancellationToken);

        /// <summary>
        /// Get Workflow Step Events For Applicant
        /// </summary>
        /// <param name="requestId"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> GetWorkflowStepEventsForApplicant(Guid requestId, CancellationToken cancellationToken);

        /// <summary>
        /// Process Workflow
        /// </summary>
        /// <param name="RequestId">Request Id</param>
        /// <param name="WorkflowStepId">Workflow Step Id</param>
        /// <param name="workflowStepEventId">workflow Step Event Id</param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> ProcessWorkFlow(ProcessRequestPayload requestPayload, CancellationToken cancellationToken);



        /// <summary>
        /// Get Request Log For Employee
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> GetRequestLogForEmployee(Guid requestId, CancellationToken cancellationToken);

        /// <summary>
        /// Get Request Log For Applicant
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> GetRequestLogForApplicant(Guid requestId, CancellationToken cancellationToken);

        /// <summary>
        /// Get Request Processing Log For Employee
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> GetRequestProcessingLogForEmployee(Guid requestId, CancellationToken cancellationToken);

        /// <summary>
        /// Get Request Processing Log For Reformer
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> GetWorkflowStepEventsForReformer(Guid requestId, CancellationToken cancellationToken);

        /// <summary>
        /// Get Request Processing Log For Applicant
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        public Task<BaseResponse> GetRequestProcessingLogForApplicant(Guid requestId, CancellationToken cancellationToken);
    }
}
