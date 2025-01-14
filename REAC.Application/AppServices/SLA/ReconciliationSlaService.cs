using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.AppServices.SLA
{
    public interface IReconciliationSlaService
    {
        Task<BaseResponse> Execute();
    }
    public class ReconciliationSlaService : IReconciliationSlaService
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ISysSettingService _sysSettingService;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;


        public ReconciliationSlaService(ISysSettingService sysSettingService, IMapper mapper, IEntityRepository repository, ISysEmployeeService employeeService, INotificationService notificationService)
        {
            _mapper = mapper;
            _repository = repository;
            _sysSettingService = sysSettingService;
            _employeeService = employeeService;
            _notificationService = notificationService;
        }
        public async Task<BaseResponse> Execute()
        {

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                                .Include(x => x.Contract)
                                .ThenInclude(x => x.FirstPartyCitizen)
                                .Include(x => x.Contract)
                                 .ThenInclude(x => x.SecondPartyCitizen)
                                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.ReformerCitizen)//المصلح مواطن
                                .Include(x => x.ApplicantCitizen)
                              .Where(x => x.LitigationRequestStage == LitigationRequestStage.Reconciliation
                              & x.IsFinished != true)
                              .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int ReformerReminderForReconciliationFirstPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReformerReminderForReconciliationFirstPeriod).Result.Data?.Value, out ReformerReminderForReconciliationFirstPeriod);
                    int ReformerReminderForReconciliationSecondPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReformerReminderForReconciliationSecondPeriod).Result.Data?.Value, out ReformerReminderForReconciliationSecondPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (ReformerReminderForReconciliationFirstPeriod > 0)
                        {
                            if (litigationRequest.IsReformerReminderForReconciliationFirstPeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.StartReconciliationDate).Value.TotalHours;
                                if (totalHours >= ReformerReminderForReconciliationFirstPeriod)
                                {
                                    litigationRequest.IsReformerReminderForReconciliationFirstPeriodSent = true;

                                    //Send Notification for reformer(reformer actorType),RequestType(ReformerTaskrequests)
                                    // Mail - Sms - Application Notification
                                    // يرجى سرعة اتمام عملية الصلح على الدعوى رقم ......
                                    try
                                    {
                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishReconciliation).Result.Data?.Value;
                                        msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                                        await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية الصلح", NotificationActorType.Reformer, RequestType.ReformerTaskrequests,
                                           false, true);
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }

                        if (ReformerReminderForReconciliationSecondPeriod > 0)
                        {
                            double totalReviewHours = 0;

                            totalReviewHours += (DateTime.Now - litigationRequest.StartReconciliationDate).Value.TotalHours;
                            if (totalReviewHours >= ReformerReminderForReconciliationSecondPeriod)
                            {
                                // Send Notification for Reconciliation admin (employee actorType),RequestType(ReformerTaskrequests)
                                // Mail - Sms - Application Notification
                                // يرجى العلم أن المصلح تجاوز الفترة المسموح بها للصلح على الدعوى رقم .....
                                try
                                {
                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishReconciliation_WarnReconciliationAdmin).Result.Data?.Value;
                                    msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                    msg = msg?.Replace("{ReformerName}", litigationRequest?.ReformerCitizen?.FullName);

                                    await SendNotificationMS(msg, litigationRequest, "المصلح تجاوز الفترة المسموح بها", NotificationActorType.Employee, RequestType.LitigationRequest,
                                       false, false, true);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }

                        var repositorySaveResponse = await _repository.SaveChangesAsync(default);
                        if (!repositorySaveResponse.IsDone)
                            return repositorySaveResponse;
                    }
                }
            }

            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                              .Include(x => x.LitigationRequestReconciliationReports)
                               .Include(x => x.Contract)
                                .ThenInclude(x => x.FirstPartyCitizen)
                                .Include(x => x.Contract)
                                 .ThenInclude(x => x.SecondPartyCitizen)
                                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.ReformerCitizen)//المصلح مواطن
                                .Include(x => x.ApplicantCitizen)
                              .Where(x => x.LitigationRequestStage == LitigationRequestStage.Reconciliation
                              & x.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestReportAdded
                              & x.LitigationRequestReconciliationReports.Count > 0
                              & x.IsFinished != true)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int LitigationRequestReconciliationReportResponsePeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestReconciliationReportResponsePeriod).Result.Data?.Value, out LitigationRequestReconciliationReportResponsePeriod);
                    int LitigationRequestReconciliationReportDecisionPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestReconciliationReportDecisionPeriod).Result.Data?.Value, out LitigationRequestReconciliationReportDecisionPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (LitigationRequestReconciliationReportResponsePeriod > 0)
                        {
                            if (litigationRequest.IsLitigationRequestReconciliationReportResponsePeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.LitigationRequestReconciliationReports.FirstOrDefault().ReportDate).TotalHours;
                                if (totalHours >= LitigationRequestReconciliationReportResponsePeriod)
                                {
                                    litigationRequest.IsLitigationRequestReconciliationReportResponsePeriodSent = true;
                                    if (litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus == null)
                                    {
                                        //Send Notification for Plaintiff //
                                        // Mail - Sms - Application Notification
                                        // يرجى سرعة الرد على محضر الصلح الصادر على الدعوى رقم ......
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReconciliationReport_PleaseReply).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                                            await SendNotificationMS(msg, litigationRequest, " سرعة الرد على محضر الصلح", NotificationActorType.Plaintiff, RequestType.ReformationReport,
                                               true, false, false, true, false, true);//المدعى  سواء فرد او شركة
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                    if (litigationRequest.MintuesOfMeetingApprovalDefendantStatus == null)
                                    {
                                        //Send Notification for Defendant
                                        // Mail - Sms - Application Notification
                                        // يرجى سرعة الرد على محضر الصلح الصادر على الدعوى رقم ......
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReconciliationReport_PleaseReply).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                                            await SendNotificationMS(msg, litigationRequest, " سرعة الرد على محضر الصلح", NotificationActorType.Defendant, RequestType.ReformationReport,
                                               false, false, false, false, true, false, true);//المدعى عليه سواء فرد او شركة
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                }
                            }
                        }

                        if (LitigationRequestReconciliationReportDecisionPeriod > 0)
                        {
                            double totalReviewHours = 0;

                            totalReviewHours += (DateTime.Now - litigationRequest.LitigationRequestReconciliationReports.FirstOrDefault().ReportDate).TotalHours;
                            if (totalReviewHours >= LitigationRequestReconciliationReportDecisionPeriod)
                            {
                                //Send Notification for reformer
                                // Mail - Sms - Application Notification
                                // يرجى العلم أن الرد على محضر الصلح الصادر على الدعوى رقم ..... قد تجاوز مهلته القصوى
                                try
                                {
                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReconciliationReport_WarnReformer).Result.Data?.Value;
                                    msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                                    await SendNotificationMS(msg, litigationRequest, "الرد على محضر الصلح", NotificationActorType.Reformer, RequestType.ReformationReport,
                                       false, true);
                                }
                                catch (Exception e)
                                {

                                }
                            }
                        }

                        var repositorySaveResponse = await _repository.SaveChangesAsync(default);
                        if (!repositorySaveResponse.IsDone)
                            return repositorySaveResponse;
                    }
                }
            }


            var litigationRequestReformerAssigningRequests = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())?
                              .Include(x => x.ReformerCitizen)//old Former
                              .Include(x => x.LitigationRequest)
                              .ThenInclude(x => x.ReformerCitizen)//المصلح مواطن                              
                              .Where(x => x.Response == LitigationRequestReformerAssigningRequestStatus.None)
                              .ToList();

            if (litigationRequestReformerAssigningRequests != null)
            {
                if (litigationRequestReformerAssigningRequests.Any())
                {
                    int LitigationRequestReformerAssigningRequestResponsePeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestReformerAssigningRequestResponsePeriod).Result.Data?.Value, out LitigationRequestReformerAssigningRequestResponsePeriod);
                    int LitigationRequestReformerAssigningRequestDecisionPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestReformerAssigningRequestDecisionPeriod).Result.Data?.Value, out LitigationRequestReformerAssigningRequestDecisionPeriod);

                    foreach (var litigationRequestReformerAssigningRequest in litigationRequestReformerAssigningRequests)
                    {
                        if (LitigationRequestReformerAssigningRequestResponsePeriod > 0)
                        {
                            if (litigationRequestReformerAssigningRequest.LitigationRequest.IsLitigationRequestReformerAssigningRequestResponsePeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequestReformerAssigningRequest.RequestOn).Value.TotalHours;
                                if (totalHours >= LitigationRequestReformerAssigningRequestResponsePeriod)
                                {
                                    litigationRequestReformerAssigningRequest.LitigationRequest.IsLitigationRequestReformerAssigningRequestResponsePeriodSent = true;

                                    
                                    try
                                    {

                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReconciliationTask_PleaseReply).Result.Data?.Value;
                                        msg = msg?.Replace("{LitigationCode}", litigationRequestReformerAssigningRequest.LitigationRequest?.RequestCode);
                                        var objEmail = new SendEmailDto()
                                        {
                                            Body = msg,
                                            Subject = "سرعة الرد على طلب مهمة الصلح",
                                            To = new List<string>()
                                        {
                                            litigationRequestReformerAssigningRequest.ReformerCitizen.Email
                                        }
                                        };

                                        var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                        {
                                            message_text = msg,
                                            recipients = new List<string>() { litigationRequestReformerAssigningRequest.ReformerCitizen.Mobile },
                                            notification_type = "SMS",
                                        };

                                        var lstNotification = new List<SysNotificationDto>()
                                    {
                                        new SysNotificationDto()
                                        {
                                            ActorId = litigationRequestReformerAssigningRequest.CitizenId,
                                            CreatedOn = DateTime.Now,
                                            ActorTypeId = NotificationActorType.Reformer,
                                            IsViewed = false,
                                            ViewedOn = null,
                                            RequestId = litigationRequestReformerAssigningRequest.LitigationRequestId,
                                            RequestTypeId = RequestType.ReformerTaskrequests,
                                            Message = msg
                                        }
                                    };
                                        await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }

                        if (LitigationRequestReformerAssigningRequestDecisionPeriod > 0)
                        {
                            double totalHours = 0;

                            totalHours += (DateTime.Now - litigationRequestReformerAssigningRequest.RequestOn).Value.TotalHours;
                            if (totalHours >= LitigationRequestReformerAssigningRequestDecisionPeriod)
                            {
                                litigationRequestReformerAssigningRequest.Response = LitigationRequestReformerAssigningRequestStatus.Cancelled;
                                litigationRequestReformerAssigningRequest.ResponseOn = DateTime.Now;

                                // Send Notification for old reformer
                                // old reformer = litigationRequestReformerAssigningRequest.CitizenId
                                // Mail - Sms - Application Notification
                                // تم إلغاء طلب مهمة الصلح على الدعوى رقم ..... لتجاوز الرد على المهمة مهلته القصوى 
                                try
                                {

                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReconciliationTaskCancelled).Result.Data?.Value;
                                    msg = msg?.Replace("{LitigationCode}", litigationRequestReformerAssigningRequest.LitigationRequest?.RequestCode);
                                    var objEmail = new SendEmailDto()
                                    {
                                        Body = msg,
                                        Subject = "إلغاء طلب مهمة الصلح",
                                        To = new List<string>()
                                        {
                                            litigationRequestReformerAssigningRequest.ReformerCitizen.Email
                                        }
                                    };

                                    var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { litigationRequestReformerAssigningRequest.ReformerCitizen.Mobile },
                                        notification_type = "SMS",
                                    };

                                    var lstNotification = new List<SysNotificationDto>()
                                    {
                                        new SysNotificationDto()
                                        {
                                            ActorId = litigationRequestReformerAssigningRequest.CitizenId,
                                            CreatedOn = DateTime.Now,
                                            ActorTypeId = NotificationActorType.Reformer,
                                            IsViewed = false,
                                            ViewedOn = null,
                                            RequestId = litigationRequestReformerAssigningRequest.LitigationRequestId,
                                            RequestTypeId = RequestType.ReformerTaskrequests,
                                            Message = msg
                                        }
                                    };
                                    await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
                                }
                                catch (Exception e)
                                {

                                }



                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequestReformerAssigningRequest.LitigationRequestId,
                                    ShowForApplicant = false,
                                    LogMessage = "تم إلغاء طلب مهمة الصلح للمصلح " + litigationRequestReformerAssigningRequest.ReformerCitizen.FullName
                                });

                                var department = (await _repository.GetQueryable<SysDepartment, Guid>())?
                                .Where(x => x.DepartmentType == DepartmentType.Reconciliation)
                                .SingleOrDefault();
                                if (department != null)
                                {
                                    switch (department.DistributionMode)
                                    {
                                        case DepartmentDistributionMode.AutoDistribution:
                                            var reformer = (await _repository.GetQueryable<Citizen, Guid>())?
                                                .Include(x => x.ReformerLitigationRequests)
                                                        .Where(x => x.IsReformer == true & x.ReformerStatusId == ReformerStatus.Certified & x.Id != litigationRequestReformerAssigningRequest.CitizenId)
                                                        .OrderBy(x => x.ReformerLitigationRequests
                                                        .Count(t => t.IsFinished == false))
                                                        .FirstOrDefault();
                                            if (reformer != null)
                                            {

                                                LitigationRequestReformerAssigningRequest newLitigationRequestReformerAssigningRequest = new LitigationRequestReformerAssigningRequest();
                                                newLitigationRequestReformerAssigningRequest.CitizenId = reformer.Id;
                                                newLitigationRequestReformerAssigningRequest.RequestOn = DateTime.Now;
                                                newLitigationRequestReformerAssigningRequest.LitigationRequestId = litigationRequestReformerAssigningRequest.LitigationRequestId;
                                                newLitigationRequestReformerAssigningRequest.Response = LitigationRequestReformerAssigningRequestStatus.None;
                                                await _repository.AddAsync<LitigationRequestReformerAssigningRequest, Guid>(newLitigationRequestReformerAssigningRequest);

                                                litigationRequestReformerAssigningRequest.LitigationRequest.IsLitigationRequestReformerAssigningRequestResponsePeriodSent = false;

                                                // Send Notification for new reformer
                                                // new reformer = reformer.Id
                                                // Mail - Sms - Application Notification
                                                // يوجد طلب مهمة صلح جديد على الدعوى رقم  ..........
                                                try
                                                {

                                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReconciliationTask_New).Result.Data?.Value;
                                                    msg = msg?.Replace("{LitigationCode}", litigationRequestReformerAssigningRequest.LitigationRequest?.RequestCode);
                                                    var objEmail = new SendEmailDto()
                                                    {
                                                        Body = msg,
                                                        Subject = "طلب مهمة صلح جديد على الدعوى",
                                                        To = new List<string>()
                                                        {
                                                            reformer.Email
                                                        }
                                                    };

                                                    var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                    {
                                                        message_text = msg,
                                                        recipients = new List<string>() { reformer.Mobile },
                                                        notification_type = "SMS",
                                                    };

                                                    var lstNotification = new List<Contracts.Dtos.Notification.SysNotificationDto>()
                                                    {
                                                        new Contracts.Dtos.Notification.SysNotificationDto()
                                                        {
                                                            ActorId = reformer.Id,
                                                            CreatedOn = DateTime.Now,
                                                            ActorTypeId = NotificationActorType.Reformer,
                                                            IsViewed = false,
                                                            ViewedOn = null,
                                                            RequestId = litigationRequestReformerAssigningRequest.LitigationRequestId,
                                                            RequestTypeId = RequestType.ReformerTaskrequests,
                                                            Message = msg
                                                        }
                                                    };
                                                    await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
                                                }
                                                catch (Exception e)
                                                {

                                                }

                                                // Send Notification for Reconciliation admin
                                                // Mail - Sms - Application Notification
                                                // يرجى العلم أن الرد على مهمة الصلح للمصلح على الدعوى رقم ..... قد تجاوز مهلته القصوى وتم إلغاؤها وارسال مهمة جديدة للمصلح reformer.FullName
                                                try
                                                {
                                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReconciliationTask_ReAssigned).Result.Data?.Value;
                                                    msg = msg?.Replace("{LitigationCode}", litigationRequestReformerAssigningRequest.LitigationRequest?.RequestCode);
                                                    msg = msg?.Replace("{PreviousReformer}", litigationRequestReformerAssigningRequest.ReformerCitizen.FullName);
                                                    msg = msg?.Replace("{CurrentReformer}", reformer.FullName);

                                                    await SendNotificationMS(msg, litigationRequestReformerAssigningRequest.LitigationRequest, "الرد على مهمة الصلح للمصلح", NotificationActorType.Employee, RequestType.LitigationRequest,
                                                       false, false, true);
                                                }
                                                catch (Exception e)
                                                {

                                                }

                                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CreatedOn = DateTime.Now,
                                                    RequestId = litigationRequestReformerAssigningRequest.LitigationRequestId,
                                                    ShowForApplicant = false,
                                                    LogMessage = "تم إرسال طلب مهمة الصلح للمصلح " + reformer.FullName
                                                });
                                            }
                                            break;
                                    }
                                }
                            }
                        }

                        var repositorySaveResponse = await _repository.SaveChangesAsync(default);
                        if (!repositorySaveResponse.IsDone)
                            return repositorySaveResponse;
                    }
                }
            }

            return new BaseResponse() { IsDone = true };
        }

        private async Task  SendNotificationMS(string msg, LitigationRequest litigationRequest, string subject, NotificationActorType notificationActorType, RequestType requestType, bool notifyApplicant = false, bool notifyReformer = false, bool notifyDeptAdmin = false, bool notifyFirstParty = false, bool notifySecondParty = false, bool notifyFirstPartyFacility = false, bool notifySecondPartyFacility = false)
        {
            var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
            var objEmail = new SendEmailDto()
            {
                Body = msg,
                Subject = subject,
                To = new List<string>()
            };

            var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>(),
                notification_type = "SMS",
            };

            var lstNotification = new List<SysNotificationDto>();

            // Notify Applicant Citizen
            if (notifyApplicant && litigationRequest.ApplicantCitizen != null)
            {
                objEmail.To.Add(litigationRequest.ApplicantCitizen.Email);
                objSms.recipients.Add(litigationRequest.ApplicantCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ApplicantCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Reformer Citizen
            if (notifyReformer && litigationRequest.ReformerCitizenId.HasValue)
            {
                objEmail.To.Add(litigationRequest.ReformerCitizen.Email);
                objSms.recipients.Add(litigationRequest.ReformerCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ReformerCitizenId.Value,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Reformer,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationSession,
                    Message = msg,
                });
            }

            // Notify Department Admin
            if (notifyDeptAdmin && deptAdminInfoReconciliation != null)
            {
                objEmail.To.Add(deptAdminInfoReconciliation.Data.Email);
                objSms.recipients.Add(deptAdminInfoReconciliation.Data.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = deptAdminInfoReconciliation.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify First Party Citizen
            if (notifyFirstParty && litigationRequest.Contract?.FirstPartyCitizen != null && litigationRequest.Contract.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Second Party Citizen
            if (notifySecondParty && litigationRequest.Contract?.SecondPartyCitizen != null)
            {
                objEmail.To.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify First Party Facility
            if (notifyFirstPartyFacility && litigationRequest.Contract?.FirstPartyFacility != null && litigationRequest.Contract.FirstPartyFacility.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest.Contract.FirstPartyFacility.Email);
                objSms.recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Second Party Facility
            if (notifySecondPartyFacility && litigationRequest.Contract?.SecondPartyFacility != null)
            {
                objEmail.To.Add(litigationRequest.Contract.SecondPartyFacility.Email);
                objSms.recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
        }

    }
}
