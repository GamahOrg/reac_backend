using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.SLA;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Text.Json;

namespace REAC.Application.AppServices.SLA
{

    public class ReviewSlaService : IReviewSlaService
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ISysSettingService _sysSettingService;
        private readonly INotificationService _notificationService;
        private readonly ISysEmployeeService _employeeService;
        private readonly ILoggerManager _loggerManager;
        private readonly IServiceProvider _serviceProvider;

        public ReviewSlaService(ISysSettingService sysSettingService, IMapper mapper, IEntityRepository repository, INotificationService notificationService, ISysEmployeeService employeeService, ILoggerManager loggerManager, IServiceProvider serviceProvider)
        {
            _mapper = mapper;
            _repository = repository;
            _sysSettingService = sysSettingService;
            _notificationService = notificationService;
            _employeeService = employeeService;
            _loggerManager = loggerManager;
            _serviceProvider = serviceProvider;
        }
        public async Task<BaseResponse> Execute()
        {

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                             .Include(x => x.ApplicantCitizen)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.WorkFlowStep)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.Employee)
                             .Include(x => x.Contract)
                             .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                             .Include(x => x.Contract.FirstPartyCitizen)
                             .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                             .Include(x => x.Contract.SecondPartyCitizen)
                             .Where(x => x.InternalStatusId == RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int firstReminderForApplicantToPayLitigationRequestFeesPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestFeesNotPaidReminderFirstPeriod).Result.Data?.Value, out firstReminderForApplicantToPayLitigationRequestFeesPeriod);


                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (litigationRequest.IsLitigationRequestFeesNotPaidFirstReminderSent != true)
                        {
                            if (firstReminderForApplicantToPayLitigationRequestFeesPeriod > 0)
                            {
                                var workFlowProcessingLog = litigationRequest.WorkFlowProcessingLogs.Where(x => x.EmployeeId.HasValue == false & x.IsFinished == false).OrderBy(x => x.CreatedOn).LastOrDefault();


                                if (workFlowProcessingLog is not null)
                                {
                                    double totalReviewHours = 0;
                                    totalReviewHours = (DateTime.Now - workFlowProcessingLog.CreatedOn).TotalHours;


                                    if (totalReviewHours >= firstReminderForApplicantToPayLitigationRequestFeesPeriod)
                                    {
                                        litigationRequest.IsLitigationRequestFeesNotPaidFirstReminderSent = true;
                                        litigationRequest.InternalStatusId = RequestStatus.Cancelled;
                                        litigationRequest.ExternalStatusId = RequestStatus.Cancelled;



                                        //Send Notification for Applicant 
                                        // Mail - Sms - Application Notification
                                        // تم الغاء الدعوي رقم لعدم دفع الرسوم   .....
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_PayLitigationFees_Applicant).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_PayLitigationFees_Applicant).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }

                                            await SendNotificationMS(body, msg, litigationRequest, "دفع رسوم الدعوى", NotificationActorType.Applicant, RequestType.LitigationRequest,
                                           true, false, true, true, false, true, false);
                                        }
                                        catch (Exception e)
                                        {
                                            _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                            , new LogDto()
                                            {
                                                Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                                Message = e.ToString()
                                            });


                                        }

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




            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                            .Include(x => x.ApplicantCitizen)
                            .Include(x => x.WorkFlowProcessingLogs)
                            .ThenInclude(x => x.WorkFlowStep)
                            .Include(x => x.WorkFlowProcessingLogs)
                            .ThenInclude(x => x.Employee)
                            .Include(x => x.Contract)
                            .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                            .Include(x => x.Contract.FirstPartyCitizen)
                            .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                            .Include(x => x.Contract.SecondPartyCitizen)
                            .Where(x => x.InternalStatusId == RequestStatus.New && x.IsFinished != true)
                            .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int reminderForReviewerPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReminderForReviewer).Result.Data?.Value, out reminderForReviewerPeriod);
                    int reminderForReviewerManagerPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReminderForReviewerManager).Result.Data?.Value, out reminderForReviewerManagerPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (litigationRequest.IsReminderForReviewerSent != true)
                        {
                            if (reminderForReviewerPeriod > 0)
                            {
                                var workFlowProcessingLogs = litigationRequest.WorkFlowProcessingLogs.Where(x => x.EmployeeId.HasValue == true && !x.IsFinished).OrderBy(x => x.CreatedOn).ToList();
                                Guid? employeeId = workFlowProcessingLogs.Last().EmployeeId;
                                var employeeWorkFlowProcessingLogs = workFlowProcessingLogs.Where(x => x.EmployeeId == employeeId);

                                if (employeeWorkFlowProcessingLogs.Any())
                                {
                                    double totalReviewHours = 0;


                                    totalReviewHours = (DateTime.Now - employeeWorkFlowProcessingLogs.FirstOrDefault().CreatedOn).TotalHours;

                                    if (totalReviewHours >= reminderForReviewerPeriod)
                                    {
                                        litigationRequest.IsReminderForReviewerSent = true;

                                        foreach (var employeeWorkFlowProcessingLog in employeeWorkFlowProcessingLogs)
                                        {
                                            //Send Notification for reviewer employee
                                            // Mail - Sms - Application Notification
                                            //يرجى اتمام مراجعة الدعوى رقم .....
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishLitigation_ReviewerEmployee).Result.Data?.Value;
                                                msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishLitigation_ReviewerEmployee).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }

                                                var applicationNotifications = new List<SysNotificationDto>();

                                                applicationNotifications.Add(new SysNotificationDto()
                                                {
                                                    ActorId = (Guid)employeeWorkFlowProcessingLog.EmployeeId,
                                                    CreatedOn = DateTime.Now,
                                                    ActorTypeId = NotificationActorType.Employee,
                                                    IsViewed = false,
                                                    ViewedOn = null,
                                                    RequestId = litigationRequest.Id,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    Message = msg
                                                });

                                                var notificationResult = await _notificationService.SendNotifications(
                                                    new SendEmailDto()
                                                    {
                                                        Body = body,
                                                        Subject = "يرجي اتمام مراجعة الدعوي",
                                                        To = new List<string>() { employeeWorkFlowProcessingLog.Employee.Email }

                                                    },
                                                    new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                    {
                                                        message_text = msg,
                                                        notification_type = "SMS",
                                                        recipients = new List<string>() { employeeWorkFlowProcessingLog.Employee.Mobile }
                                                    }, applicationNotifications
                                                    );
                                            }
                                            catch (Exception e)
                                            {
                                                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                        , new LogDto()
                                        {
                                            Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                            Message = e.ToString()
                                        });
                                            }

                                        }

                                    }
                                }

                            }
                        }

                        if (litigationRequest.IsReminderForReviewerManagerSent != true)
                        {
                            if (reminderForReviewerManagerPeriod > 0)
                            {
                                var workFlowProcessingLogs = litigationRequest.WorkFlowProcessingLogs.Where(x => x.EmployeeId.HasValue == true && !x.IsFinished).OrderBy(x => x.CreatedOn).ToList();
                                Guid? employeeId = workFlowProcessingLogs.Last().EmployeeId;
                                var employeeWorkFlowProcessingLogs = workFlowProcessingLogs.Where(x => x.EmployeeId == employeeId);

                                if (employeeWorkFlowProcessingLogs.Any())
                                {
                                    double totalReviewHours = 0;

                                    totalReviewHours = (DateTime.Now - employeeWorkFlowProcessingLogs.FirstOrDefault().CreatedOn).TotalHours;


                                    if (totalReviewHours >= reminderForReviewerManagerPeriod)
                                    {
                                        litigationRequest.IsReminderForReviewerManagerSent = true;

                                        var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?
                                                             .Include(x => x.Department)
                                                             .ThenInclude(x => x.SysEmployees)
                                                             .ThenInclude(x => x.WorkFlowProcessingLogs)
                                                             .Where(x => x.Id == employeeId)
                                                             .FirstOrDefault();

                                        if (employee.Department.DistributionMode == DepartmentDistributionMode.AutoDistribution)
                                        {
                                            if (!employeeWorkFlowProcessingLogs.LastOrDefault().IsFinished)
                                            {
                                                Guid? newEmployeeId = employee?.Department.SysEmployees?.Where(x => x.Id != employeeId & x.IsAdmin == false).OrderBy(x => x.WorkFlowProcessingLogs.Where(x => x.IsFinished != true).Count()).FirstOrDefault()?.Id;

                                                if (!newEmployeeId.HasValue)
                                                {
                                                    newEmployeeId = employee?.Department.SysEmployees.Where(x => x.Id != employeeId & x.IsAdmin == true).ToList().FirstOrDefault()?.Id;
                                                }

                                                if (newEmployeeId.HasValue)
                                                {
                                                    litigationRequest.IsReminderForReviewerSent = false;
                                                    litigationRequest.IsReminderForReviewerManagerSent = false;

                                                    //employeeWorkFlowProcessingLogs.LastOrDefault().IsFinished = true;
                                                    //employeeWorkFlowProcessingLogs.LastOrDefault().FinishedOn = DateTime.Now;

                                                    //WorkFlowProcessingLog workFlowProcessingLog = new WorkFlowProcessingLog();
                                                    //workFlowProcessingLog.Id = Guid.NewGuid();
                                                    //workFlowProcessingLog.EmployeeId = newEmployeeId;
                                                    //workFlowProcessingLog.IsFinished = false;
                                                    //workFlowProcessingLog.RequestId = litigationRequest.Id;
                                                    //workFlowProcessingLog.CreatedOn = DateTime.Now;
                                                    //workFlowProcessingLog.WorkFlowStepId = employeeWorkFlowProcessingLogs.LastOrDefault().WorkFlowStepId;
                                                    //await _repository.AddAsync<WorkFlowProcessingLog, Guid>(workFlowProcessingLog);
                                                    foreach (var workFlowProcessingLog in workFlowProcessingLogs.Where(x => x.EmployeeId == employeeId))
                                                    {
                                                        workFlowProcessingLog.EmployeeId = newEmployeeId;
                                                        workFlowProcessingLog.CreatedOn = DateTime.Now;
                                                    }


                                                    //Send Notification for old reviewer
                                                    // Mail - Sms - Application Notification
                                                    //تم سحب دعوى جديدة رقم ..... لتخطى مدة التدقيق الفصوى
                                                    try
                                                    {
                                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_WithdrewLitigation_OldReviewerEmployee).Result.Data?.Value;
                                                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                        string body;
                                                        using (StreamReader streamReader = File.OpenText(pathToFile))
                                                        {
                                                            body = await streamReader.ReadToEndAsync();
                                                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_WithdrewLitigation_OldReviewerEmployee).Result.Data?.Value);
                                                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                        }

                                                        var applicationNotifications = new List<SysNotificationDto>();

                                                        applicationNotifications.Add(new SysNotificationDto()
                                                        {
                                                            ActorId = employee.Id,
                                                            CreatedOn = DateTime.Now,
                                                            ActorTypeId = NotificationActorType.Employee,
                                                            IsViewed = false,
                                                            ViewedOn = null,
                                                            RequestId = litigationRequest.Id,
                                                            RequestTypeId = RequestType.LitigationRequest,
                                                            Message = msg
                                                        });

                                                        var notificationResult = await _notificationService.SendNotifications(
                                                            new SendEmailDto()
                                                            {
                                                                Body = body,
                                                                Subject = "تم سحب دعوى جديدة لتخطى مدة التدقيق الفصوى",
                                                                To = new List<string>() { employee.Email }

                                                            },
                                                            new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                            {
                                                                message_text = msg,
                                                                notification_type = "SMS",
                                                                recipients = new List<string>() { employee.Mobile }
                                                            }, applicationNotifications
                                                            );
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }




                                                    //Send Notification for new reviewer
                                                    // Mail - Sms - Application Notification
                                                    //تم ارسال دعوى جديدة رقم ..... للتدقيق
                                                    try
                                                    {
                                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_NewReviewerEmployee).Result.Data?.Value;
                                                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                        string body;
                                                        using (StreamReader streamReader = File.OpenText(pathToFile))
                                                        {
                                                            body = await streamReader.ReadToEndAsync();
                                                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_NewReviewerEmployee).Result.Data?.Value);
                                                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                        }

                                                        var employeeInfo = (await _repository.GetQueryable<SysEmployee, Guid>()).Where(e => e.Id == newEmployeeId).FirstOrDefault();

                                                        var applicationNotifications = new List<SysNotificationDto>();

                                                        applicationNotifications.Add(new SysNotificationDto()
                                                        {
                                                            ActorId = (Guid)newEmployeeId,
                                                            CreatedOn = DateTime.Now,
                                                            ActorTypeId = NotificationActorType.Employee,
                                                            IsViewed = false,
                                                            ViewedOn = null,
                                                            RequestId = litigationRequest.Id,
                                                            RequestTypeId = RequestType.LitigationRequest,
                                                            Message = msg
                                                        });

                                                        var notificationResult = await _notificationService.SendNotifications(
                                                            new SendEmailDto()
                                                            {
                                                                Body = body,
                                                                Subject = "تم ارسال دعوى جديدة  للتدقيق",
                                                                To = new List<string>() { employeeInfo.Email }

                                                            },
                                                            new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                            {
                                                                message_text = msg,
                                                                notification_type = "SMS",
                                                                recipients = new List<string>() { employeeInfo.Mobile }
                                                            }, applicationNotifications
                                                            );
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }



                                                    //Send Notification for review department admin
                                                    // Mail - Sms - Application Notification
                                                    //تم ارسال الدعوى رقم .... إلى مدقق جديد لتخطى المدقق السابق مدة التدقيق الفصوى

                                                    try
                                                    {
                                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_ReviewerAdmin).Result.Data?.Value;
                                                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                        string body;
                                                        using (StreamReader streamReader = File.OpenText(pathToFile))
                                                        {
                                                            body = await streamReader.ReadToEndAsync();
                                                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_ReviewerAdmin).Result.Data?.Value);
                                                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                        }

                                                        var adminInfo = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);

                                                        var applicationNotifications = new List<SysNotificationDto>();

                                                        applicationNotifications.Add(new SysNotificationDto()
                                                        {
                                                            ActorId = adminInfo.Data.Id,
                                                            CreatedOn = DateTime.Now,
                                                            ActorTypeId = NotificationActorType.Employee,
                                                            IsViewed = false,
                                                            ViewedOn = null,
                                                            RequestId = litigationRequest.Id,
                                                            RequestTypeId = RequestType.LitigationRequest,
                                                            Message = msg
                                                        });

                                                        var notificationResult = await _notificationService.SendNotifications(
                                                            new SendEmailDto()
                                                            {
                                                                Body = body,
                                                                Subject = "تم ارسال دعوى  إلى مدقق جديد لتخطى المدقق السابق مدة التدقيق الفصوى",
                                                                To = new List<string>() { adminInfo.Data.Email }

                                                            },
                                                            new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                            {
                                                                message_text = msg,
                                                                notification_type = "SMS",
                                                                recipients = new List<string>() { adminInfo.Data.Mobile }
                                                            }, applicationNotifications
                                                            );
                                                    }
                                                    catch (Exception ex)
                                                    {

                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {


                                            // Send Notification for review department admin
                                            // Mail - Sms - Application Notification
                                            // يوجد تأخير تجاوز المده المسموح بها للتدقيق على الدعوى رقم .....
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_ReviewerAdmin).Result.Data?.Value;
                                                msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_ReviewerAdmin).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }

                                                var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);

                                                var applicationNotifications = new List<SysNotificationDto>();

                                                applicationNotifications.Add(new SysNotificationDto()
                                                {
                                                    ActorId = (Guid)deptAdminInfoReview.Data.Id,
                                                    CreatedOn = DateTime.Now,
                                                    ActorTypeId = NotificationActorType.Employee,
                                                    IsViewed = false,
                                                    ViewedOn = null,
                                                    RequestId = litigationRequest.Id,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    Message = msg
                                                });

                                                var notificationResult = await _notificationService.SendNotifications(
                                                    new SendEmailDto()
                                                    {
                                                        Body = body,
                                                        Subject = "تم ارسال دعوى  إلى مدقق جديد لتخطى المدقق السابق مدة التدقيق الفصوى",
                                                        To = new List<string>() { deptAdminInfoReview.Data.Email }

                                                    },
                                                    new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                    {
                                                        message_text = msg,
                                                        notification_type = "SMS",
                                                        recipients = new List<string>() { deptAdminInfoReview.Data.Mobile }
                                                    }, applicationNotifications
                                                    );
                                            }
                                            catch (Exception ex)
                                            {

                                            }
                                        }
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

            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                             .Include(x => x.ApplicantCitizen)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.WorkFlowStep)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.Employee)
                             .Where(x => x.InternalStatusId == RequestStatus.SentBackForEdit & x.IsFinished != true)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int reminderForapplicantFirstPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FirstEditReminderForRequestApplicant).Result.Data?.Value, out reminderForapplicantFirstPeriod);
                    int reminderForApplicantSecondPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_SecondEditReminderForForRequestApplicant).Result.Data?.Value, out reminderForApplicantSecondPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        var workFlowProcessingLog = litigationRequest.WorkFlowProcessingLogs.Where(x => x.EmployeeId.HasValue == false & x.IsFinished == false).OrderBy(x => x.CreatedOn).LastOrDefault();

                        if (workFlowProcessingLog != null)
                        {
                            double totalReviewHours = 0;

                            totalReviewHours = (DateTime.Now - workFlowProcessingLog.CreatedOn).TotalHours;

                            if (reminderForapplicantFirstPeriod > 0)
                            {
                                if (litigationRequest.IsReminderForApplicantSent != true | litigationRequest.IsReminderForApplicantSent is null)
                                {
                                    if (totalReviewHours >= reminderForapplicantFirstPeriod)
                                    {
                                        litigationRequest.IsReminderForApplicantSent = true;

                                        //Send Notification for applicant and plaintiff
                                        // Mail - Sms - Application Notification
                                        //يرجى سرعة معالجة كافة الملاحظات على الدعوى رقم ........

                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishNotes).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            msg = msg.Replace("{ExpirationDate}", $"{(reminderForApplicantSecondPeriod - (int)totalReviewHours) / 24}");

                                            msg = msg.Replace("{ExpirationDate}", $"{(reminderForApplicantSecondPeriod - (int)totalReviewHours) / 24}");
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishNotes).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                body = body.Replace("{ExpirationDate}", $"{(reminderForApplicantSecondPeriod - (int)totalReviewHours) / 24}");
                                            }

                                            List<SysNotificationDto> notifications = new List<SysNotificationDto>()
                                            {
                                                new SysNotificationDto{
                                                ActorId = litigationRequest.ApplicantCitizen.Id,
                                                RequestId=litigationRequest.Id,
                                                CreatedOn= DateTime.Now,
                                                RequestTypeId=RequestType.LitigationRequest,
                                                ActorTypeId=NotificationActorType.Plaintiff,
                                                Message=msg
                                                }
                                            };

                                            List<string> emails = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Email,
                                            };

                                            List<string> recipients = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Mobile,
                                            };


                                            if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);


                                            };
                                            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
                                            };


                                            var notificationResult = await _notificationService.SendNotifications(
                                                new SendEmailDto()
                                                {
                                                    Body = body,
                                                    Subject = "يرجى سرعة معالجة كافة الملاحظات على الدعوى",
                                                    To = emails

                                                },
                                                new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                {
                                                    message_text = msg,
                                                    notification_type = "SMS",
                                                    recipients = recipients
                                                }, notifications
                                                );
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            }
                            if (reminderForApplicantSecondPeriod > 0)
                            {
                                if (litigationRequest.IsReminderForCancelLitigationForNotesSent != true)
                                {
                                    if (totalReviewHours >= reminderForApplicantSecondPeriod)


                                    {
                                        litigationRequest.IsFinished = true;
                                        litigationRequest.IsActive = false;
                                        litigationRequest.InternalStatusId = RequestStatus.Cancelled;
                                        litigationRequest.ExternalStatusId = RequestStatus.Cancelled;
                                        litigationRequest.WorkFlowProcessingLogs.OrderBy(x => x.CreatedOn).LastOrDefault().IsFinished = true;
                                        litigationRequest.WorkFlowProcessingLogs.OrderBy(x => x.CreatedOn).LastOrDefault().FinishedOn = DateTime.Now;
                                        litigationRequest.IsReminderForCancelLitigationForNotesSent = true;

                                        //Send Notification for applicant and plaintiff and review department admin and reviewer
                                        // Mail - Sms - Application Notification
                                        //تم إلفاء الدعوى رقم ........ لعدم الرد على الملاحظات المرسلة
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationCancelled).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationCancelled).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);
                                            var deptEmployeeInfoReview = litigationRequest.WorkFlowProcessingLogs.Where(w => w.EmployeeId != null).OrderBy(x => x.CreatedOn).LastOrDefault()?.Employee;


                                            List<SysNotificationDto> notifications = new List<SysNotificationDto>()
                                            {
                                                new SysNotificationDto{
                                                ActorId = litigationRequest.ApplicantCitizen.Id,
                                                RequestId=litigationRequest.Id,
                                                CreatedOn= DateTime.Now,
                                                RequestTypeId=RequestType.LitigationRequest,
                                                ActorTypeId=NotificationActorType.Plaintiff,
                                                Message=msg
                                                },
                                                new SysNotificationDto{
                                                ActorId = deptAdminInfoReview.Data.Id,
                                                RequestId=litigationRequest.Id,
                                                CreatedOn= DateTime.Now,
                                                RequestTypeId=RequestType.LitigationRequest,
                                                ActorTypeId=NotificationActorType.Employee,
                                                Message=msg
                                                },
                                                new SysNotificationDto{
                                                ActorId = deptEmployeeInfoReview.Id,
                                                RequestId=litigationRequest.Id,
                                                CreatedOn= DateTime.Now,
                                                RequestTypeId=RequestType.LitigationRequest,
                                                ActorTypeId=NotificationActorType.Employee,
                                                Message=msg
                                                }
                                            };

                                            List<string> emails = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Email,
                                                deptEmployeeInfoReview.Email,
                                                deptAdminInfoReview.Data.Email
                                            };

                                            List<string> recipients = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Mobile,
                                                deptEmployeeInfoReview.Mobile,
                                                deptAdminInfoReview.Data.Mobile
                                            };


                                            if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);


                                            };
                                            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
                                            };


                                            var notificationResult = await _notificationService.SendNotifications(
                                                new SendEmailDto()
                                                {
                                                    Body = body,
                                                    Subject = "تم إلفاء الدعوى لعدم الرد على الملاحظات المرسلة",
                                                    To = emails

                                                },
                                                new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                {
                                                    message_text = msg,
                                                    notification_type = "SMS",
                                                    recipients = recipients
                                                }, notifications
                                                );
                                        }
                                        catch (Exception ex)
                                        {

                                        }
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

            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                             .Include(x => x.ApplicantCitizen)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.WorkFlowStep)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.Employee)
                             .Where(x => x.InternalStatusId == RequestStatus.AgreementRequestHasBeenSent & x.IsFinished != true)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int firstAgreementRequestForRequest = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FirstAgreementRequestForRequest).Result.Data?.Value, out firstAgreementRequestForRequest);
                    int secondAgreementRequestForRequest = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_SecondAgreementRequestForForRequest).Result.Data?.Value, out secondAgreementRequestForRequest);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        double totalReviewHours = 0;

                        totalReviewHours = (DateTime.Now - litigationRequest.AgreementRequestRequiredOn).Value.TotalHours;

                        if (firstAgreementRequestForRequest > 0)
                        {
                            if (litigationRequest.IsReminderForAgreementRequestForRequestSent is null || litigationRequest.IsReminderForAgreementRequestForRequestSent != true)
                            {
                                if (totalReviewHours >= firstAgreementRequestForRequest)
                                {
                                    litigationRequest.IsReminderForAgreementRequestForRequestSent = true;

                                    if (!litigationRequest.DefendantAgreementRequestResponse.HasValue)
                                    {
                                        //Send Notification for Defendant
                                        // Mail - Sms - Application Notification
                                        //يرجى سرعة على طلب المشارطة المرسل على الدعوى رقم ........
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishAgreementRequest).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishAgreementRequest).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            List<SysNotificationDto> notifications = new List<SysNotificationDto>();
                                            List<string> emails = new List<string>();
                                            List<string> recipients = new List<string>();


                                            if (litigationRequest?.Contract?.SecondPartyCitizenId != null)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Defendant,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                                                recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                                            };
                                            if (litigationRequest?.Contract?.SecondPartyFacilityId != null)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.CitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Defendant,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Email);
                                                recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
                                            };


                                            var notificationResult = await _notificationService.SendNotifications(
                                                new SendEmailDto()
                                                {
                                                    Body = body,
                                                    Subject = "يرجى سرعة الرد على طلب المشارطة المرسل على الدعوى",
                                                    To = emails

                                                },
                                                new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                {
                                                    message_text = msg,
                                                    notification_type = "SMS",
                                                    recipients = recipients
                                                }, notifications
                                                );
                                        }
                                        catch (Exception ex)
                                        {

                                        }


                                    }
                                    if (!litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                                    {
                                        //Send Notification for applicant and plaintiff
                                        // Mail - Sms - Application Notification
                                        //يرجى سرعة على طلب المشارطة المرسل على الدعوى رقم ........

                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishAgreementRequest).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_FinishAgreementRequest).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }


                                            List<SysNotificationDto> notifications = new List<SysNotificationDto>()
                                            {
                                                new SysNotificationDto{
                                                ActorId = litigationRequest.ApplicantCitizen.Id,
                                                RequestId=litigationRequest.Id,
                                                CreatedOn= DateTime.Now,
                                                RequestTypeId=RequestType.LitigationRequest,
                                                ActorTypeId=NotificationActorType.Plaintiff,
                                                Message=msg
                                                }
                                            };

                                            List<string> emails = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Email,
                                            };

                                            List<string> recipients = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Mobile,
                                            };


                                            if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);


                                            };
                                            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
                                            };
                                            if (litigationRequest?.Contract?.SecondPartyCitizenId != null)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Defendant,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                                                recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                                            };
                                            if (litigationRequest?.Contract?.SecondPartyFacilityId != null)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.CitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Defendant,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Email);
                                                recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
                                            };


                                            var notificationResult = await _notificationService.SendNotifications(
                                                new SendEmailDto()
                                                {
                                                    Body = body,
                                                    Subject = "يرجى سرعة الرد على طلب المشارطة المرسل على الدعوى",
                                                    To = emails

                                                },
                                                new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                {
                                                    message_text = msg,
                                                    notification_type = "SMS",
                                                    recipients = recipients
                                                }, notifications
                                                );
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                            }
                        }

                        if (secondAgreementRequestForRequest > 0)
                        {
                            if (litigationRequest.IsReminderForCancelLitigationForAgreementSent != true)
                            {
                                if (totalReviewHours >= secondAgreementRequestForRequest)
                                {
                                    // لو الدعوي تحكيم يتم الالغاء
                                    if (litigationRequest.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                                    {
                                        litigationRequest.IsFinished = true;
                                        litigationRequest.IsActive = false;
                                        litigationRequest.InternalStatusId = RequestStatus.Cancelled;
                                        litigationRequest.ExternalStatusId = RequestStatus.Cancelled;
                                        litigationRequest.WorkFlowProcessingLogs.OrderBy(x => x.CreatedOn).LastOrDefault().IsFinished = true;
                                        litigationRequest.WorkFlowProcessingLogs.OrderBy(x => x.CreatedOn).LastOrDefault().FinishedOn = DateTime.Now;
                                        litigationRequest.IsReminderForCancelLitigationForAgreementSent = true;

                                        //Send Notification for applicant and plaintiff and Defendant and review department admin
                                        // Mail - Sms - Application Notification
                                        //تم إلفاء الدعوى رقم ........ لعدم الرد على طلب المشارطة

                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AgreementCancelled).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AgreementCancelled).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }

                                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                            {
                                                Id = Guid.NewGuid(),
                                                CreatedOn = DateTime.Now,
                                                RequestId = litigationRequest.Id,
                                                ShowForApplicant = false,
                                                LogMessage = msg
                                            });

                                            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);


                                            List<SysNotificationDto> notifications = new List<SysNotificationDto>()
                                            {
                                                new SysNotificationDto{
                                                ActorId = litigationRequest.ApplicantCitizen.Id,
                                                RequestId=litigationRequest.Id,
                                                CreatedOn= DateTime.Now,
                                                RequestTypeId=RequestType.LitigationRequest,
                                                ActorTypeId=NotificationActorType.Plaintiff,
                                                Message=msg
                                                },
                                                new SysNotificationDto{
                                                ActorId = deptAdminInfoReview.Data.Id,
                                                RequestId=litigationRequest.Id,
                                                CreatedOn= DateTime.Now,
                                                RequestTypeId=RequestType.LitigationRequest,
                                                ActorTypeId=NotificationActorType.Employee,
                                                Message=msg
                                                }
                                            };

                                            List<string> emails = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Email,
                                                deptAdminInfoReview.Data.Email
                                            };

                                            List<string> recipients = new List<string>()
                                            {
                                                litigationRequest.ApplicantCitizen.Mobile,
                                                deptAdminInfoReview.Data.Mobile
                                            };


                                            if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);


                                            };
                                            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                                                recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
                                            };
                                            if (litigationRequest?.Contract?.SecondPartyCitizen != null)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyCitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                                                recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);


                                            };
                                            if (litigationRequest?.Contract?.SecondPartyFacility != null)
                                            {
                                                notifications.Add(new SysNotificationDto
                                                {
                                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.CitizenId,
                                                    RequestId = litigationRequest.Id,
                                                    CreatedOn = DateTime.Now,
                                                    RequestTypeId = RequestType.LitigationRequest,
                                                    ActorTypeId = NotificationActorType.Plaintiff,
                                                    Message = msg
                                                });
                                                emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Email);
                                                recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
                                            };


                                            var notificationResult = await _notificationService.SendNotifications(
                                                new SendEmailDto()
                                                {
                                                    Body = body,
                                                    Subject = "تم إلفاء الدعوى لعدم الرد على الملاحظات المرسلة",
                                                    To = emails

                                                },
                                                new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                {
                                                    message_text = msg,
                                                    notification_type = "SMS",
                                                    recipients = recipients
                                                }, notifications
                                                );
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    // لو صلح وتحكيم تكمل صلح فقط
                                    else if (litigationRequest.LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration)
                                    {
                                        var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                                        await _bpfService.ExecuteWorkFlowForSLA(litigationRequest.Id, default, 2);
                                        litigationRequest.InternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;
                                        litigationRequest.ExternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;
                                        litigationRequest.IsReminderForCancelLitigationForAgreementSent = true;
                                        litigationRequest.IsAgreementRequestRequired = false;


                                        try
                                        {
                                            // Send Notification


                                            string msg = "";
                                            string body;
                                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AgreementPeriodFinishedWarning).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ExcuteWorkflow).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }

                                            await SendNotificationMSEmployeeAndAdmin(body, msg, litigationRequest, RequestType.LitigationRequest, "Litigation Request Sent");
                                        }
                                        catch (Exception e)
                                        {

                                        }
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


        private async Task SendNotificationMS(string mailMsg, string msg, LitigationRequest litigationRequest, string subject, NotificationActorType notificationActorType, RequestType requestType, bool notifyApplicant = false, bool notifyReformer = false, bool notifyDeptAdmin = false, bool notifyFirstParty = false, bool notifySecondParty = false, bool notifyFirstPartyFacility = false, bool notifySecondPartyFacility = false)
        {
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
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
            if (notifyDeptAdmin && deptAdminInfoReview != null)
            {
                objEmail.To.Add(deptAdminInfoReview.Data.Email);
                objSms.recipients.Add(deptAdminInfoReview.Data.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = deptAdminInfoReview.Data.Id,
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

        private async Task SendNotificationMSEmployeeAndAdmin(string mailMsg, string msg, LitigationRequest litigationRequest, RequestType requestTypeId, string subject)
        {
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);
            var employee = litigationRequest.WorkFlowProcessingLogs.Where(w => w.EmployeeId != null).OrderBy(x => x.CreatedOn).LastOrDefault()?.Employee;

            //ApplicantCitizen
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
                Subject = subject,
                To = new List<string>() { litigationRequest.ApplicantCitizen?.Email }

            };
            var objSms = new SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>() { litigationRequest.ApplicantCitizen.Mobile },
                notification_type = "SMS",
            };
            var lstNotification = new List<SysNotificationDto>();


            //مدير ادارة التدقيق
            if (deptAdminInfoReview is not null)
            {
                objEmail.To.Add(deptAdminInfoReview?.Data?.Email);
                objSms.recipients.Add(deptAdminInfoReview?.Data?.Mobile);

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = deptAdminInfoReview.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
            }

            // الموظف المعين علي الدعوي
            if (employee is not null)
            {
                objEmail.To.Add(employee?.Email);
                objSms.recipients.Add(employee?.Mobile);

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = employee.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
            }

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);

        }

    }
}
