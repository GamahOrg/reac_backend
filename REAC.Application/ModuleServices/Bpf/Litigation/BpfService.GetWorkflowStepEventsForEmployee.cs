using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Wpf.System.WorkFlowStepEvent;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum.Wpf;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Globalization;

namespace Bpf.Application.ModuleServices
{
    public partial class BpfService
    {
        public async Task<BaseResponse> GetWorkflowStepEventsForEmployee(Guid requestId, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);
                var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?
                    .Where(x => x.Id == referenceId).Single();

                if (employee == null)
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "Employee Not Found"
                    };
                }


                var litigationStageFees = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                    .Where(x => x.Id == requestId & x.IsFinished == false)
                    .Select(x => new
                    {
                        x.LitigationRequestAmount,
                        x.LitigationRequestStage,
                        x.IsLitigationFeesHaveBeenCollected,
                        x.FeesDifference
                    }).First();

                decimal minFees = 0;
                if (litigationStageFees?.LitigationRequestAmount == null)
                {
                    minFees = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())?
                  .Where(x => x.FromAmount == 0)
                  .FirstOrDefault()?.Fees ?? 0;
                }


                var processinglog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                    .Where(x => x.RequestId == requestId & x.IsFinished == false)
                    .Include(x => x.LitigationRequest)
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

                WorkFlowStepEventForActorShortDtos.AllowAssign = employee.AllowAssign;
                WorkFlowStepEventForActorShortDtos.AllowReAssign = employee.AllowReAssign;
                WorkFlowStepEventForActorShortDtos.Fees = litigationStageFees.LitigationRequestStage == REAC.Domain.Shared.Enum.LitigationRequestStage.Review && !litigationStageFees.IsLitigationFeesHaveBeenCollected ? litigationStageFees.FeesDifference != 0 ? litigationStageFees.FeesDifference.ToString("0.#") : litigationStageFees.LitigationRequestAmount?.Fees.ToString("0.#") : null;
                if (litigationStageFees?.LitigationRequestAmount == null)
                {
                    WorkFlowStepEventForActorShortDtos.Fees = litigationStageFees.LitigationRequestStage == REAC.Domain.Shared.Enum.LitigationRequestStage.Review && !litigationStageFees.IsLitigationFeesHaveBeenCollected ? minFees.ToString("0.#") : null;
                }

                processinglog?.ForEach(x =>
                {
                    if (x.LitigationRequest.LitigationRequestCategoryId == REAC.Domain.Shared.Enum.LitigationRequestCategory.Reconciliation)
                    {
                        var removeAgreement = x.WorkFlowStep.WorkFlowStepEvents
                            .FirstOrDefault(x => x.ExternalStatusId == REAC.Domain.Shared.Enum.RequestStatus.AgreementRequestHasBeenSent);

                        // Remove Agreement item from the collection

                        x.WorkFlowStep.WorkFlowStepEvents.Remove(removeAgreement);
                    }


                    x.WorkFlowStep.WorkFlowStepEvents.ToList().ForEach(y =>
                    {
                        bool allowAdd = true;

                        if (y.EventName == "إرسال طلب تحصيل رسوم الصلح" || y.EventName == "تأكيد دفع رسوم الصلح")
                        {
                            var reconciliationFees = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Fees).Result.Data?.Value;

                            WorkFlowStepEventForActorShortDtos.Fees = reconciliationFees;
                        }

                        if (y.InternalStatusId == REAC.Domain.Shared.Enum.RequestStatus.AgreementRequestHasBeenSent)
                        {
                            if (x.LitigationRequest.IsAgreementRequestRequired.HasValue)
                            {
                                allowAdd = false;
                            }
                        }
                        if (allowAdd)
                        {
                            switch (y.EventSecurityLevelId)
                            {
                                case EventSecurityLevel.Specified:
                                    bool mainBreak = false;
                                    y.WorkFlowStepEventSecurityLevelActors.ToList().ForEach(z =>
                                    {

                                        if (mainBreak) { return; }

                                        switch (z.SecurityLevelActorId)
                                        {
                                            case SecurityLevelActorType.Department:
                                                if (z.DepartmentId.HasValue)
                                                {
                                                    if (employee.DepartmentId == z.DepartmentId)
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
                                                        mainBreak = true;
                                                        break;
                                                    }
                                                }
                                                break;
                                            case SecurityLevelActorType.Admin:
                                                if (z.DepartmentId.HasValue)
                                                {
                                                    if (employee.DepartmentId == z.DepartmentId & employee.IsAdmin)
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
                                                        mainBreak = true;
                                                        break;
                                                    }
                                                }
                                                break;
                                            default:
                                                if (z.EmployeeId.HasValue)
                                                {
                                                    if (employee.DepartmentId == z.DepartmentId & employee.IsAdmin)
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
                                                        mainBreak = true;
                                                    }
                                                    else
                                                    {
                                                        if (z.EmployeeId == referenceId)
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
                                                            mainBreak = true;
                                                        }
                                                        break;
                                                    }

                                                }
                                                break;
                                        }
                                    });
                                    break;
                                case EventSecurityLevel.UnSpecified:
                                    if (x.EmployeeId == employee.Id)
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
                                    }
                                    else
                                    {
                                        if (employee.IsAdmin & employee.WorkAsEmployee)
                                        {
                                            if (x.WorkFlowStep.DepartmentId == employee.DepartmentId)
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
                                            }
                                        }
                                    }
                                    break;
                            }
                        }
                    });
                });

                //filter WorkFlowStepEventShortDtos remove duplicate for show actions 
                WorkFlowStepEventForActorShortDtos.WorkFlowStepEventShortDtos =
                                                                                 WorkFlowStepEventForActorShortDtos.WorkFlowStepEventShortDtos
                                                                                     .DistinctBy(item => new
                                                                                     {
                                                                                         item.Id,
                                                                                         item.EventName,
                                                                                         item.AllowFileUpload,
                                                                                         item.AllowNotes,
                                                                                         item.CustomActionType
                                                                                     })
                                                                                     .ToList();
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
