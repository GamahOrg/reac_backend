using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowLog;
using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowProcessingLog;
using REAC.Application.Contracts.Dtos.Wpf.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices.Wpf.Business;
using REAC.Application.Contracts.IModuleServices;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using REAC.EntityFrameworkCore.Entities.Wpf.System;

namespace Bpf.Application.ModuleServices
{

    public partial class BpfService : IBpfService
    {
        private readonly IMapper _mapper;
        private readonly IEntityRepository _repository;
        private readonly IWorkFlowProcessingLogService _workFlowProcessingLogService;
        private readonly IWorkFlowLogService _workFlowLogService;
        private readonly ILitigationWorkFlowService _litigationWorkFlowService;
        private readonly IJwtReader _jwtReader;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;
        private readonly ISysEmployeeService _employeeService;

        public BpfService(IEntityRepository repository,
                          IWorkFlowProcessingLogService workFlowProcessingLogService,
                          IMapper mapper,
                          IJwtReader jwtReader,
                          IWorkFlowLogService workFlowLogService, ILitigationWorkFlowService litigationWorkFlowServic, ISysSettingService sysSettingService, INotificationService notificationService, ISysEmployeeService employeeService)
        {
            _mapper = mapper;
            _repository = repository;
            _workFlowProcessingLogService = workFlowProcessingLogService;
            _workFlowLogService = workFlowLogService;
            _litigationWorkFlowService = litigationWorkFlowServic;
            _jwtReader = jwtReader;
            _sysSettingService = sysSettingService;
            _notificationService = notificationService;
            _employeeService = employeeService;
        }


        #region Helpers
        /// <summary>
        /// Execute Work Flow Steps
        /// </summary>
        /// <param name="WorkflowId">Work flow object Id</param>
        /// <param name="requestId">request Id</param>
        /// <param name="order">step order</param>
        /// <param name="cancellationToken">cancellation Token</param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        private async Task<BaseResponse> ExecuteWorkFlowSteps(Guid WorkflowId, Guid requestId, Guid? sourceWorkFlowProcessingLogId, int order, bool? sameEmployee, CancellationToken cancellationToken)
        {
            var workFlow = (await _repository.GetQueryable<WorkFlow, Guid>())?
                    .Where(x => x.Id == WorkflowId)
                    .Include(x => x.Department)
                    .Include(x => x.WorkFlowSteps.Where(t => t.Order == order))
                    .ThenInclude(x => x.Department)
                    .ThenInclude(x => x.SysEmployees
                    .OrderBy(x => x.WorkFlowProcessingLogs.Count(x => x.IsFinished)).ThenBy(x => x.IsAdmin))
                    .ThenInclude(x => x.EmployeeVacationRequests)
                    .FirstOrDefault();

            #region ValidateWorkflow
            var ValidationResponse = ValidateWorkflow(workFlow);
            if (!ValidationResponse.IsDone)
                return ValidationResponse;
            #endregion

            #region Steps Execution
            BaseResponse response = new BaseResponse();
            foreach (var step in workFlow.WorkFlowSteps)
            {
                response = await ExecuteWorkFlowStep(step, requestId, sourceWorkFlowProcessingLogId, sameEmployee, cancellationToken);
                return response;
            }
            #endregion

            return response;
        }

        /// <summary>
        /// Execute Work Flow Step
        /// </summary>
        /// <param name="step">step object</param>
        /// <param name="requestId">request Id</param>
        /// <param name="cancellationToken">cancellation Token</param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        private async Task<BaseResponse> ExecuteWorkFlowStep(WorkFlowStep? step, Guid requestId, Guid? sourceWorkFlowProcessingLogId, bool? sameEmployee, CancellationToken cancellationToken)
        {
            if (step == null)
                return new BaseResponse() { IsDone = false, Message = "WorkFlow Step can't be null" };

            #region Create WorkFlowProcessingLog Object
            WorkFlowProcessingLogCreateUpdateDto workFlowProcessingLog = new WorkFlowProcessingLogCreateUpdateDto()
            {
                SourceWorkFlowProcessingLogId = sourceWorkFlowProcessingLogId,
                CreatedOn = DateTime.Now,
                RequestId = requestId,
                WorkFlowStepId = step.Id,
                IsFinished = false
            };
            #endregion

            #region Set Employee Id

            #region Same Employee
            if (sameEmployee.HasValue)
            {
                if (sameEmployee.Value)
                {
                    var preProcessingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                        .Where(x => x.RequestId == requestId & x.WorkFlowStep.DepartmentId == step.DepartmentId).OrderBy(x => x.CreatedOn).ToList();

                    if (preProcessingLog != null && preProcessingLog.Any())
                    {
                        workFlowProcessingLog.EmployeeId = preProcessingLog.LastOrDefault()?.EmployeeId;
                    }
                }
            }
            #endregion
            #region Set Other Employee Id
            if (step.ProcessActorType == ProcessActorType.Employee)
            {
                if (!workFlowProcessingLog.EmployeeId.HasValue)
                {

                    switch (step.ActorTypeId)
                    {
                        case StepActorType.Department:
                            switch (step.Department?.DistributionMode)
                            {
                                case DepartmentDistributionMode.AutoDistribution:
                                    // Get Employee id by min requests
                                    var employee = step.Department.SysEmployees.Where(x => x.IsAdmin == false);
                                    //var empId = employee.Where(x => x.EmployeeVacationRequests.All(v => v.IsApproved == null || v.IsApproved == false)
                                    // || !x.EmployeeVacationRequests.Any(v => v.IsApproved == true && (v.StartDate <= DateTime.Today && v.EndDate >= DateTime.Today))).FirstOrDefault()?.Id;

                                    var empId = employee.FirstOrDefault()?.Id;
                                    workFlowProcessingLog.EmployeeId = empId;
                                    break;
                                case DepartmentDistributionMode.ManualDistribution:
                                    //no employee
                                    break;
                                case DepartmentDistributionMode.NoneDistribution:
                                    //no employee
                                    break;
                            }
                            break;
                        case StepActorType.Official:
                            workFlowProcessingLog.EmployeeId = step.EmployeeId;
                            break;
                        case StepActorType.Employee:
                            workFlowProcessingLog.EmployeeId = step.EmployeeId;
                            break;
                        case StepActorType.Admin:
                            // Get Department Admin Id
                            workFlowProcessingLog.EmployeeId = step.Department?.SysEmployees.Where(x => x.IsAdmin).FirstOrDefault()?.Id;
                            break;
                    }
                }
            }

            #endregion
            #endregion

            #region Execute Creation Notification Plan
            if (step.CreationNotificationPlanId.HasValue)
            {
                var executeNotificationPlanResponse = await ExecuteNotificationPlan(step.CreationNotificationPlanId.Value);
                if (!executeNotificationPlanResponse.IsDone)
                    return executeNotificationPlanResponse;
            }
            #endregion

            #region Create Creation Log
            if (step.CreationLogMessage != null)
            {
                WorkFlowLogCreateUpdateDto workFlowLog = new WorkFlowLogCreateUpdateDto()
                {
                    CreatedOn = DateTime.UtcNow,
                    ShowForApplicant = step.ShowForApplicant,
                    LogMessage = step.CreationLogMessage,
                    RequestId = requestId

                };

                step.Translations?.ToList().ForEach(x =>
                {
                    workFlowLog.Translations.Add(new WorkFlowLogTranslation()
                    {
                        LanguageCode = x.LanguageCode,
                        LogMessage = x.CreationLogMessage
                    });
                });
                var workFlowLogResponse = await _workFlowLogService.AddAsync(workFlowLog);
                if (!workFlowLogResponse.IsDone)
                    return workFlowLogResponse;
            }
            #endregion

            var response = await _workFlowProcessingLogService.AddAsync(workFlowProcessingLog, cancellationToken);
            if (!response.IsDone)
                return response;

            return new DataResponse<ResponseWorkFlowDto>()
            {
                IsDone = true,
                Data = new ResponseWorkFlowDto()
                {
                    ExternalStatus = step.ExternalStatusId,
                    InternalStatus = step.InternalStatusId,
                    IsActive = true,
                    IsFinished = false
                }
            };
        }

        /// <summary>
        /// Get Request Log
        /// </summary>
        /// <param name="requestId">request Id</param>
        /// <param name="IsforApplicant">Is for Applicant flage</param>
        /// <param name="cancellationToken">cancellation Token</param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        private async Task<BaseResponse> GetRequestLog(Guid requestId, bool IsforApplicant, CancellationToken cancellationToken)
        {
            try
            {
                List<WorkFlowLog>? workFlowLog = null;
                switch (IsforApplicant)
                {
                    case true:
                        workFlowLog = (await _repository.GetQueryable<WorkFlowLog, Guid>())?
                        .Where(x => x.RequestId == requestId & x.ShowForApplicant == true)
                        .Include(x => x.Translations)
                        .OrderBy(x => x.CreatedOn)
                        .ToList();
                        break;
                    case false:
                        workFlowLog = (await _repository.GetQueryable<WorkFlowLog, Guid>())?
                        .Where(x => x.RequestId == requestId)
                        .Include(x => x.Translations)
                        .OrderBy(x => x.CreatedOn)
                        .ToList();
                        break;
                }
                var result = _mapper.Map<List<WorkFlowLogDto>>(workFlowLog);

                return new DataResponse<List<WorkFlowLogDto>>()
                {
                    IsDone = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }

        /// <summary>
        /// Get Request Processing Log
        /// </summary>
        /// <param name="requestId">request Id</param>
        /// <param name="IsforApplicant">Is for Applicant flage</param>
        /// <param name="cancellationToken">cancellation Token</param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        private async Task<BaseResponse> GetRequestProcessingLog(Guid requestId, bool isShowforApplicant, CancellationToken cancellationToken)
        {
            try
            {
                List<WorkFlowProcessingLog>? workFlowProcessingLog = null;
                switch (isShowforApplicant)
                {
                    case true:
                        workFlowProcessingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                        .Where(x => x.RequestId == requestId & x.ShowForApplicant == true & x.IsFinished == true)
                        .Include(x => x.WorkFlowStep)
                        .Include(x => x.WorkFlowStepEvent)
                        .ThenInclude(x => x.Translations)
                        .Include(x => x.Employee)
                        .ThenInclude(x => x.Department)
                        .Include(x => x.WorkFlowProcessingLogFiles)
                        .OrderBy(x => x.CreatedOn)
                        .ToList();
                        break;
                    case false:
                        workFlowProcessingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                        .Where(x => x.RequestId == requestId & x.IsFinished == true)
                        .Include(x => x.WorkFlowStep)
                        .Include(x => x.WorkFlowStepEvent)
                        .ThenInclude(x => x.Translations)
                        .Include(x => x.Employee)
                        .ThenInclude(x => x.Department)
                        .Include(x => x.WorkFlowProcessingLogFiles)
                        .OrderBy(x => x.CreatedOn)
                        .ToList();
                        break;
                }
                var result = _mapper.Map<List<WorkFlowProcessingLogDto>>(workFlowProcessingLog);

                return new DataResponse<List<WorkFlowProcessingLogDto>>()
                {
                    IsDone = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }

        /// <summary>
        /// Validate Work flow
        /// </summary>
        /// <param name="workFlow">workFlow Object</param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        private BaseResponse ValidateWorkflow(WorkFlow? workFlow)
        {
            #region Validate workFlow object and first step object
            if (workFlow == null)
                return new BaseResponse()
                {
                    IsDone = false,
                    Message = "workFlow not found"
                };
            if (!workFlow.WorkFlowSteps.Any())
                return new BaseResponse()
                {
                    IsDone = false,
                    Message = "WorkFlowStep not found"
                };
            #endregion
            return new BaseResponse() { IsDone = true };
        }

        ///ToDo ↓↓↓↓↓↓

        /// <summary>
        /// Execute Notification Plan by Id
        /// </summary>
        /// <param name="notificationPlanId">Notification Plan Id</param>
        /// <returns>Return BaseResponse include process status in isDone Flage and responsed data</returns>
        private async Task<BaseResponse> ExecuteNotificationPlan(Guid notificationPlanId)
        {
            return new BaseResponse() { IsDone = true };
        }
        #endregion
    }
}
