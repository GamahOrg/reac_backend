using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.DirectoryServices.Protocols;

namespace REAC.Application.AppServices.SLA
{
    public interface IArbitrationSlaService
    {
        Task<BaseResponse> Execute();
    }
    public class ArbitrationSlaService : IArbitrationSlaService
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ISysSettingService _sysSettingService;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ILoggerManager _loggerManager;

        public ArbitrationSlaService(ISysSettingService sysSettingService, IMapper mapper, IEntityRepository repository, ISysEmployeeService employeeService, INotificationService notificationService, ILoggerManager loggerManager)
        {
            _mapper = mapper;
            _repository = repository;
            _sysSettingService = sysSettingService;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _loggerManager = loggerManager;
        }
        public async Task<BaseResponse> Execute()
        {

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                             .Include(x => x.ApplicantCitizen)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.WorkFlowStep)
                             .Include(x => x.WorkFlowProcessingLogs)
                             .ThenInclude(x => x.Employee)
                             .Include(x => x.LitigationRequestReopenLogs)
                             .Include(x => x.Contract)
                             .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                             .Include(x => x.Contract.FirstPartyCitizen)
                             .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                             .Include(x => x.Contract.SecondPartyCitizen)
                             .Where(x => x.InternalStatusId == RequestStatus.CollectionOfArbitrationFeesRequestIsBeingSent && !x.IsArbitrationFeesHaveBeenCollected)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int firstReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitrationFeesNotPaidReminderFirstPeriod).Result.Data?.Value, out firstReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod);
                    int secondReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitrationFeesNotPaidReminderSecondPeriod).Result.Data?.Value, out secondReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod);


                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (litigationRequest.IsLitigationRequestArbitrationFeesNotPaidFirstReminderSent != true)
                        {
                            if (firstReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod > 0)
                            {

                                if (litigationRequest.ArbitratorsCountDateTime is not null)
                                {
                                    double totalReviewHours = 0;
                                    totalReviewHours = (DateTime.Now - (DateTime)litigationRequest.ArbitratorsCountDateTime).TotalHours;


                                    if (totalReviewHours >= firstReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod)
                                    {
                                        litigationRequest.IsLitigationRequestArbitrationFeesNotPaidFirstReminderSent = true;


                                        // تذكير بدفع رسوم التحكيم للدعوي
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitrationFeesReminder).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitrationFeesReminder).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }

                                            await SendNotificationMS(body, msg, litigationRequest, "دفع رسوم الدعوى", NotificationActorType.Applicant, RequestType.LitigationRequest,
                                          notifyApplicant: true, notifyDeptAdmin: false, notifyFirstParty: true);
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
                        else
                        {
                            if (secondReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod > 0)
                            {


                                if (litigationRequest.ArbitratorsCountDateTime is not null)
                                {
                                    double totalReviewHours = 0;
                                    totalReviewHours = (DateTime.Now - (DateTime)litigationRequest.ArbitratorsCountDateTime).TotalHours;

                                    if (totalReviewHours >= secondReminderForApplicantToPayLitigationRequestArbitrationFeesPeriod && !litigationRequest.LitigationRequestReopenLogs.Any())
                                    {

                                        litigationRequest.InternalStatusId = RequestStatus.Cancelled;
                                        litigationRequest.ExternalStatusId = RequestStatus.Cancelled;
                                        litigationRequest.IsFinished = true;
                                        //Send Notification for Applicant 
                                        // Mail - Sms - Application Notification
                                        // تم الغاء الدعوي رقم لعدم دفع الرسوم   .....
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_PayLitigationArbitrationFees_Applicant).Result.Data?.Value;
                                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_PayLitigationArbitrationFees_Applicant).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }

                                            await SendNotificationMS(body, msg, litigationRequest, "الغاء دعوي التحكيم", NotificationActorType.Applicant, RequestType.LitigationRequest,
                                           notifyApplicant: true, notifyFirstParty: true, notifyDeptAdmin: true);
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
                    var repositorySaveResponse = await _repository.SaveChangesAsync(default);
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;
                }
            }






            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                               .Include(x => x.Contract)
                               .ThenInclude(x => x.FirstPartyCitizen)
                               .Include(x => x.Contract)
                               .ThenInclude(x => x.SecondPartyCitizen)
                               .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                               .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                             .Where(x => x.LitigationRequestStage == LitigationRequestStage.Arbitration
                              & x.IsFinished != true & x.ArbitratorsSelectionDurationDateTime != null)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int LitigationRequestArbitratorCountSelectionResponsePeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorCountSelectionResponsePeriod).Result.Data?.Value, out LitigationRequestArbitratorCountSelectionResponsePeriod);
                    int LitigationRequestArbitratorCountSelectionDecisionPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorCountSelectionDecisionPeriod).Result.Data?.Value, out LitigationRequestArbitratorCountSelectionDecisionPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (LitigationRequestArbitratorCountSelectionResponsePeriod > 0)
                        {
                            if (litigationRequest.IsArbitratorCountSelectionResponsePeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.ArbitratorsSelectionDurationDateTime).Value.TotalHours;
                                if (totalHours >= LitigationRequestArbitratorCountSelectionResponsePeriod)
                                {
                                    litigationRequest.IsArbitratorCountSelectionResponsePeriodSent = true;
                                    if (!litigationRequest.FirstPartyArbitratorsCount.HasValue)
                                    {
                                        //Send Notification for Plaintiff 
                                        // Mail - Sms - Application Notification
                                        // يرجى سرعة اتمام عملية إختيار عدد المحكمين على الدعوى رقم ......
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FirstWarning).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FirstWarning).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار عدد المحكمين", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
                                               , notifyFirstParty: true, notifyApplicant: true);
                                        }
                                        catch (Exception e)
                                        {

                                        }
                                    }
                                    if (!litigationRequest.SecondPartyArbitratorsCount.HasValue)
                                    {
                                        //Send Notification for Defendant
                                        // Mail - Sms - Application Notification
                                        // تذكير يرجى سرعة اتمام عملية إختيار عدد المحكمين على الدعوى رقم ......

                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FirstWarning).Result.Data?.Value;
                                        msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                        string body;
                                        using (StreamReader streamReader = File.OpenText(pathToFile))
                                        {
                                            body = await streamReader.ReadToEndAsync();
                                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FirstWarning).Result.Data?.Value);
                                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                        }
                                        await SendNotificationMS(body, msg, litigationRequest, " سرعة اتمام عملية إختيار عدد المحكمين", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
                                          , notifySecondParty: true);
                                    }
                                }
                            }
                        }

                        if (LitigationRequestArbitratorCountSelectionDecisionPeriod > 0)
                        {
                            if (litigationRequest.IsArbitratorCountSelectionDecisionPeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.ArbitratorsSelectionDurationDateTime).Value.TotalHours;
                                if (totalHours >= LitigationRequestArbitratorCountSelectionDecisionPeriod)
                                {
                                    litigationRequest.IsArbitratorCountSelectionDecisionPeriodSent = true;

                                    if (!litigationRequest.FirstPartyArbitratorsCount.HasValue)
                                    {
                                        //Send Notification for Plaintiff
                                        // Mail - Sms - Application Notification
                                        // تذكير يرجى سرعة اتمام عملية إختيار عدد المحكمين على الدعوى رقم ......

                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_SecondWarning).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_SecondWarning).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار عدد المحكمين", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
                                              , notifyApplicant: true, notifyFirstParty: true);
                                        }
                                        catch (Exception ex) { }



                                    }
                                    if (!litigationRequest.SecondPartyArbitratorsCount.HasValue)
                                    {
                                        //Send Notification for Defendant
                                        // Mail - Sms - Application Notification
                                        // تذكير يرجى سرعة اتمام عملية إختيار عدد المحكمين على الدعوى رقم ......
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_SecondWarning).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_SecondWarning).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار عدد المحكمين", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
                                              , notifySecondParty: true);
                                        }
                                        catch (Exception ex) { }

                                    }

                                    //Send Notification for Arbitration department admin
                                    // Mail - Sms - Application Notification
                                    if (!litigationRequest.FirstPartyArbitratorsCount.HasValue & !litigationRequest.SecondPartyArbitratorsCount.HasValue)
                                    {
                                        // لم يقم المدعى والمدعى عليه بتحديد عدد المحكمين غلى الدعوى رقم ........
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FinalWarning_Both).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FinalWarning_Both).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "لم يقم المدعى والمدعى عليه بتحديد عدد المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
                                              , notifyDeptAdmin: true);
                                        }
                                        catch (Exception ex) { }


                                    }
                                    else
                                    {
                                        if (!litigationRequest.FirstPartyArbitratorsCount.HasValue)
                                        {
                                            // لم يقم المدعى بتحديد عدد المحكمين غلى الدعوى رقم ........
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FinalWarning_Plantiff).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FinalWarning_Plantiff).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "لم يقم المدعى بتحديد عدد المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
                                                  , notifyDeptAdmin: true);
                                            }
                                            catch (Exception ex) { }

                                        }
                                        if (!litigationRequest.SecondPartyArbitratorsCount.HasValue)
                                        {
                                            // لم يقم المدعى عليه بتحديد عدد المحكمين غلى الدعوى رقم ........
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FinalWarning_Defendant).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_FinalWarning_Defendant).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "لم يقم المدعى بتحديد عدد المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
                                                  , notifyDeptAdmin: true);
                                            }
                                            catch (Exception ex) { }

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
                              .Include(x => x.LitigationRequestArbitratorNominationLists)
                              .Where(x => x.LitigationRequestStage == LitigationRequestStage.Arbitration
                              & x.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestAddArbitratorListNomination
                              & x.ArbitratorsCountDateTime != null
                              & x.IsFinished != true)
                             .ToList();
            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {
                    int AddLitigationRequestArbitratorNominationListPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AddLitigationRequestArbitratorNominationListPeriod).Result.Data?.Value, out AddLitigationRequestArbitratorNominationListPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (litigationRequest.IsArbitratorAdminNominationListAddedPeriodSent != true)
                        {
                            double totalHours = 0;

                            totalHours += (DateTime.Now - litigationRequest.ArbitratorsCountDateTime).Value.TotalHours;

                            if (totalHours >= AddLitigationRequestArbitratorNominationListPeriod)
                            {
                                litigationRequest.IsArbitratorAdminNominationListAddedPeriodSent = true;

                                //Send Notification for Dept Admin 
                                // Mail - Sms - Application Notification
                                // يرجى سرعة اتمام عملية ترشيح هيئة التحكيم على الدعوى رقم ......
                                try
                                {
                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AddLitigationRequestArbitratorNominationListWarning).Result.Data?.Value;
                                    msg = msg?.Replace("{LitigationRequest}", litigationRequest?.RequestCode);
                                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                    string body;
                                    using (StreamReader streamReader = File.OpenText(pathToFile))
                                    {
                                        body = await streamReader.ReadToEndAsync();
                                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value);
                                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                    }
                                    await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار مرشحين هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorNominationList
                                      , notifyDeptAdmin: true);


                                }
                                catch (Exception ex) { }
                            }


                        }
                    }

                    var repositorySaveResponse = await _repository.SaveChangesAsync(default);
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;
                }
            }



            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                                .Include(x => x.Contract)
                                .ThenInclude(x => x.FirstPartyCitizen)
                                .Include(x => x.Contract)
                                .ThenInclude(x => x.SecondPartyCitizen)
                                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                              .Include(x => x.LitigationRequestArbitratorNominationLists)
                              .Where(x => x.LitigationRequestStage == LitigationRequestStage.Arbitration
                              & x.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestAddArbitratorListNominationDone
                              & x.IsFinished != true)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {

                    int LitigationRequestArbitratorNominationListResponsePeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorNominationListResponsePeriod).Result.Data?.Value, out LitigationRequestArbitratorNominationListResponsePeriod);
                    int LitigationRequestArbitratorNominationListDecisionPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorNominationListDecisionPeriod).Result.Data?.Value, out LitigationRequestArbitratorNominationListDecisionPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (LitigationRequestArbitratorNominationListResponsePeriod > 0)
                        {
                            if (litigationRequest.IsArbitratorNominationListResponsePeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.ArbitratorsNominationListCreationDateTime).Value.TotalHours;
                                if (totalHours >= LitigationRequestArbitratorNominationListResponsePeriod)
                                {
                                    litigationRequest.IsArbitratorNominationListResponsePeriodSent = true;
                                    if (litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.PlaintiffPriority.HasValue != true).Count() > 0)
                                    {
                                        //Send Notification for Plaintiff 
                                        // Mail - Sms - Application Notification
                                        // يرجى سرعة اتمام عملية إختيار هيئة التحكيم على الدعوى رقم ......
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
                                              , notifyApplicant: true, notifyFirstParty: true);
                                        }
                                        catch (Exception ex) { }

                                    }
                                    if (litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.DefendantPriority.HasValue != true).Count() > 0)
                                    {
                                        //Send Notification for Defendant
                                        // Mail - Sms - Application Notification
                                        // يرجى سرعة اتمام عملية إختيار هيئة التحكيم على الدعوى رقم ......
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
                                              , notifySecondParty: true);
                                        }
                                        catch (Exception ex) { }

                                    }
                                }
                            }
                        }

                        if (LitigationRequestArbitratorNominationListDecisionPeriod > 0)
                        {
                            if (litigationRequest.IsArbitratorNominationListDecisionPeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.ArbitratorsNominationListCreationDateTime).Value.TotalHours;

                                if (totalHours >= LitigationRequestArbitratorNominationListDecisionPeriod)
                                {
                                    litigationRequest.IsArbitratorNominationListDecisionPeriodSent = true;

                                    if (litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.PlaintiffPriority.HasValue != true).Count() > 0)
                                    {
                                        //Send Notification for Plaintiff 
                                        // Mail - Sms - Application Notification
                                        // يرجى سرعة اتمام عملية إختيار هيئة التحكيم على الدعوى رقم ......
                                        try
                                        {

                                            if (litigationRequest.ArbitratorsCount == 1 && litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.DefendantPriority.HasValue == true).Count() > 0)
                                            {
                                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CreatedOn = DateTime.Now,
                                                    RequestId = litigationRequest.Id,
                                                    ShowForApplicant = false,
                                                    LogMessage = "تم إضافة أولوية إختيار قائمة تعيين هيئة التحكيم من قبل النظام للمدعى لتخطيه مدة الاختيار القصوى  "
                                                });
                                                litigationRequest.IsPlaintiffArbitratorNominationListDone = true;

                                                foreach (var item in litigationRequest?.LitigationRequestArbitratorNominationLists.ToList())
                                                {
                                                    item.PlaintiffPriority = item.DefendantPriority;
                                                    item.TotalPriority = item.PlaintiffPriority + item.DefendantPriority;

                                                }
                                                litigationRequest.ArbitratorsNominationListSelectionDoneDateTime = DateTime.Now;
                                                litigationRequest.ExternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;
                                                litigationRequest.InternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;
                                                var selectedArbitrator = litigationRequest.LitigationRequestArbitratorNominationLists.OrderBy(x => x.TotalPriority).FirstOrDefault();
                                                if (selectedArbitrator != null)
                                                {
                                                    await AddSingleNominationAssigningArbitrator(litigationRequest, selectedArbitrator);
                                                }
                                            }
                                            else
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
                                                  , notifyApplicant: true, notifyFirstParty: true);

                                            }
                                        }
                                        catch (Exception ex) { }

                                    }
                                    if (litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.DefendantPriority.HasValue != true).Count() > 0)
                                    {
                                        //Send Notification for Defendant
                                        // Mail - Sms - Application Notification
                                        // يرجى سرعة اتمام عملية إختيار هيئة التحكيم على الدعوى رقم ......
                                        try
                                        {

                                            if (litigationRequest.ArbitratorsCount == 1 && litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.PlaintiffPriority.HasValue == true).Count() > 0)
                                            {
                                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CreatedOn = DateTime.Now,
                                                    RequestId = litigationRequest.Id,
                                                    ShowForApplicant = false,
                                                    LogMessage = "تم إضافة أولوية إختيار قائمة تعيين هيئة التحكيم من قبل النظام للمدعى عليه لتخطيه مدة الاختيار القصوى  "
                                                });
                                                litigationRequest.IsDefendantArbitratorNominationListDone = true;

                                                foreach (var item in litigationRequest?.LitigationRequestArbitratorNominationLists.ToList())
                                                {
                                                    item.DefendantPriority = item.PlaintiffPriority;
                                                    item.TotalPriority = item.PlaintiffPriority + item.DefendantPriority;

                                                }
                                                litigationRequest.ArbitratorsNominationListSelectionDoneDateTime = DateTime.Now;
                                                litigationRequest.ExternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;
                                                litigationRequest.InternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;
                                                var selectedArbitrator = litigationRequest.LitigationRequestArbitratorNominationLists.OrderBy(x => x.TotalPriority).FirstOrDefault();
                                                if (selectedArbitrator != null)
                                                {
                                                    await AddSingleNominationAssigningArbitrator(litigationRequest, selectedArbitrator);
                                                }
                                            }
                                            else
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
                                                  , notifySecondParty: true);
                                            }
                                        }
                                        catch (Exception ex) { }

                                    }

                                    //Send Notification for Arbitration department admin
                                    // Mail - Sms - Application Notification
                                    if (!litigationRequest.FirstPartyArbitratorsCount.HasValue & !litigationRequest.SecondPartyArbitratorsCount.HasValue)
                                    {
                                        // لم يقم المدعى والمدعى عليه بإنهاء تعيين هيئة المحكمين غلى الدعوى رقم ........

                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FinalWarning_Both).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FinalWarning_Both).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "لم يقم المدعى والمدعى عليه بإنهاء تعيين هيئة المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
                                              , notifyDeptAdmin: true);
                                        }
                                        catch (Exception)
                                        {

                                        }
                                    }
                                    else
                                    {
                                        if (!litigationRequest.FirstPartyArbitratorsCount.HasValue)
                                        {
                                            // لم يقم المدعى بإنهاء تعيين هيئة المحكمين غلى الدعوى رقم ........
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FinalWarning_Plantiff).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FinalWarning_Plantiff).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "لم يقم المدعى بإنهاء تعيين هيئة المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
                                                  , notifyDeptAdmin: true);
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                        if (!litigationRequest.SecondPartyArbitratorsCount.HasValue)
                                        {
                                            // لم يقم المدعى عليه بإنهاء تعيين هيئة المحكمين غلى الدعوى رقم ........
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FinalWarning_Defendant).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FinalWarning_Defendant).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "لم يقم المدعى عليه بإنهاء تعيين هيئة المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
                                                  , notifyDeptAdmin: true);
                                            }
                                            catch (Exception)
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
                              .Include(x => x.Contract)
                              .ThenInclude(x => x.FirstPartyCitizen)
                              .Include(x => x.Contract)
                              .ThenInclude(x => x.SecondPartyCitizen)
                              .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                              .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                              .Include(x => x.LitigationRequestArbitratorLists).ThenInclude(x => x.ArbitratorCitizen)
                              .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                              .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)
                              .Where(x => x.LitigationRequestStage == LitigationRequestStage.Arbitration
                              & x.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNominationDone
                              & x.IsFinished != true)
                             .ToList();

            if (litigationRequests != null)
            {
                if (litigationRequests.Any())
                {

                    int LitigationRequestArbitratorChairmanNominationListResponseFirstPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorNominationListResponsePeriod).Result.Data?.Value, out LitigationRequestArbitratorChairmanNominationListResponseFirstPeriod);
                    int LitigationRequestArbitratorChairmanNominationListDecisionSecondPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorNominationListDecisionPeriod).Result.Data?.Value, out LitigationRequestArbitratorChairmanNominationListDecisionSecondPeriod);

                    foreach (var litigationRequest in litigationRequests)
                    {
                        if (LitigationRequestArbitratorChairmanNominationListResponseFirstPeriod > 0)
                        {
                            if (litigationRequest.IsArbitratorChairmanNominationListResponsePeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.ArbitratorsChairmanNominationListCreationDateTime).Value.TotalHours;
                                if (totalHours >= LitigationRequestArbitratorChairmanNominationListResponseFirstPeriod)
                                {
                                    litigationRequest.IsArbitratorChairmanNominationListResponsePeriodSent = true;

                                    foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
                                    {
                                        int count = 0;
                                        foreach (var chairman in litigationRequest.LitigationRequestArbitratorsChairmanNominationLists)
                                        {
                                            count += chairman.LitigationRequestArbitratorsChairmanNominationListValues.Where(x => x.ArbitratorCitizenId == arbitrator.Id).Count();
                                        }
                                        if (count == 0)
                                        {
                                            //Send Notification for arbitrator
                                            // Mail - Sms - Application Notification
                                            // يرجى سرعة اتمام عملية إختيار رئيس هيئة التحكيم على الدعوى رقم ......
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsChairmanSelect_FirstWarning).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsChairmanSelect_FirstWarning).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار رئيس هيئة التحكيم", NotificationActorType.Arbitrator, RequestType.ArbitratorChairmanNominationList, arbitrator
                                                  , notifyArbitrators: true);
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (LitigationRequestArbitratorChairmanNominationListDecisionSecondPeriod > 0)
                        {
                            if (litigationRequest.IsArbitratorChairmanNominationListDecisionPeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequest.ArbitratorsChairmanNominationListCreationDateTime).Value.TotalHours;

                                if (totalHours >= LitigationRequestArbitratorChairmanNominationListDecisionSecondPeriod)
                                {
                                    litigationRequest.IsArbitratorChairmanNominationListDecisionPeriodSent = true;

                                    foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
                                    {
                                        int count = 0;
                                        foreach (var chairman in litigationRequest.LitigationRequestArbitratorsChairmanNominationLists)
                                        {
                                            count += chairman.LitigationRequestArbitratorsChairmanNominationListValues.Where(x => x.ArbitratorCitizenId == arbitrator.Id).Count();
                                        }

                                        if (count == 0)
                                        {
                                            //Send Notification for arbitrator
                                            // Mail - Sms - Application Notification
                                            // يرجى سرعة اتمام عملية إختيار رئيس هيئة التحكيم على الدعوى رقم ......
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsChairmanSelect_FirstWarning).Result.Data?.Value;
                                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsChairmanSelect_FirstWarning).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                                }
                                                await SendNotificationMS(body, msg, litigationRequest, "سرعة اتمام عملية إختيار رئيس هيئة التحكيم", NotificationActorType.Arbitrator, RequestType.ArbitratorChairmanNominationList, arbitrator
                                                  , notifyArbitrators: true);
                                            }
                                            catch (Exception)
                                            {

                                            }
                                        }
                                    }
                                    if (!litigationRequest.ArbitratorsChairmanNominationListSelectionDoneDateTime.HasValue)
                                    {
                                        //Send Notification for arbitration department admin
                                        // Mail - Sms - Application Notification
                                        // لم يتم تغيين رئيس هيئة التحكيم على الدعوى رقم .....
                                        try
                                        {
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsChairmanSelect_FinalWarning_Admin).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                            string body;
                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                            {
                                                body = await streamReader.ReadToEndAsync();
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsChairmanSelect_FinalWarning_Admin).Result.Data?.Value);
                                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                            }
                                            await SendNotificationMS(body, msg, litigationRequest, "لم يتم تعيين رئيس هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorChairmanNominationList
                                              , notifyDeptAdmin: true);
                                        }
                                        catch (Exception ex) { }

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

            var litigationRequestArbitratorAssigningRequests = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())?
                                                                .Include(x => x.ArbitratorCitizen)
                                                                .Include(x => x.LitigationRequest)
                                                                    .ThenInclude(lr => lr.LitigationRequestArbitratorNominationLists)
                                                                        .ThenInclude(nl => nl.ArbitratorCitizen)
                                                                .Include(x => x.LitigationRequest)
                                                                    .ThenInclude(lr => lr.LitigationRequestArbitratorsChairmanNominationLists)
                                                                        .ThenInclude(cl => cl.ArbitratorChairmanCitizen)
                                                                            .ThenInclude(ac => ac.CitizenRequests)
                                                                 .Where(x => x.Response == LitigationRequestArbitratorAssigningRequestStatus.None)
                                                                 .AsSplitQuery()
                                                                 .ToList();

            if (litigationRequestArbitratorAssigningRequests != null)
            {
                if (litigationRequestArbitratorAssigningRequests.Any())
                {
                    int LitigationRequestArbitratorAssigningRequestResponseFirstPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorAssigningRequestResponseFirstPeriod).Result.Data?.Value, out LitigationRequestArbitratorAssigningRequestResponseFirstPeriod);
                    int LitigationRequestArbitratorAssigningRequestResponseSecondPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorAssigningRequestResponseSecondPeriod).Result.Data?.Value, out LitigationRequestArbitratorAssigningRequestResponseSecondPeriod);
                    int LitigationRequestArbitratorAssigningRequestDecisionPeriod = 0;
                    int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestArbitratorAssigningRequestDecisionPeriod).Result.Data?.Value, out LitigationRequestArbitratorAssigningRequestDecisionPeriod);
                    int index = 0;
                    foreach (var litigationRequestArbitratorAssigningRequest in litigationRequestArbitratorAssigningRequests)
                    {
                        if (LitigationRequestArbitratorAssigningRequestResponseFirstPeriod > 0)
                        {
                            if (litigationRequestArbitratorAssigningRequest.LitigationRequest.IsArbitratorAssigningRequestResponseFirstPeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequestArbitratorAssigningRequest.RequestOn).TotalHours;
                                if (totalHours >= LitigationRequestArbitratorAssigningRequestResponseFirstPeriod)
                                {
                                    litigationRequestArbitratorAssigningRequest.LitigationRequest.IsArbitratorAssigningRequestResponseFirstPeriodSent = true;

                                    // litigationRequestArbitratorAssigningRequest.CitizenId
                                    // Send Notification for arbitrator
                                    // Mail - Sms - Application Notification
                                    // يرجى سرعة الرد على طلب مهمة التحكيم على الدعوى رقم ......
                                    try
                                    {
                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_FirstWarning).Result.Data?.Value;
                                        msg = msg.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                        string body;
                                        using (StreamReader streamReader = File.OpenText(pathToFile))
                                        {
                                            body = await streamReader.ReadToEndAsync();
                                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_FirstWarning).Result.Data?.Value);
                                            body = body?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        }
                                        var applicationNotifications = new List<SysNotificationDto>();

                                        applicationNotifications.Add(new SysNotificationDto()
                                        {
                                            ActorId = litigationRequestArbitratorAssigningRequest.CitizenId,
                                            CreatedOn = DateTime.Now,
                                            ActorTypeId = NotificationActorType.Arbitrator,
                                            IsViewed = false,
                                            ViewedOn = null,
                                            RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequest.Id,
                                            RequestTypeId = RequestType.ArbitratorTaskrequests,
                                            Message = msg
                                        });

                                        var notificationResult = await _notificationService.SendNotifications(
                                            new SendEmailDto()
                                            {
                                                Body = body,
                                                Subject = "سرعة الرد على طلب مهمة التحكيم",
                                                To = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Email }

                                            },
                                            new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                            {
                                                message_text = msg,
                                                notification_type = "SMS",
                                                recipients = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Mobile }
                                            }, applicationNotifications
                                            );
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }

                        if (LitigationRequestArbitratorAssigningRequestResponseSecondPeriod > 0)
                        {
                            if (litigationRequestArbitratorAssigningRequest.LitigationRequest.IsArbitratorAssigningRequestResponseSecondPeriodSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequestArbitratorAssigningRequest.RequestOn).TotalHours;
                                if (totalHours >= LitigationRequestArbitratorAssigningRequestResponseSecondPeriod)
                                {
                                    litigationRequestArbitratorAssigningRequest.LitigationRequest.IsArbitratorAssigningRequestResponseSecondPeriodSent = true;

                                    // litigationRequestArbitratorAssigningRequest.CitizenId
                                    // Send Notification for arbitrator
                                    // Mail - Sms - Application Notification
                                    // يرجى سرعة الرد على طلب مهمة التحكيم على الدعوى رقم ......
                                    try
                                    {
                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_FirstWarning).Result.Data?.Value;
                                        msg = msg.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                        string body;
                                        using (StreamReader streamReader = File.OpenText(pathToFile))
                                        {
                                            body = await streamReader.ReadToEndAsync();
                                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_FirstWarning).Result.Data?.Value);
                                            body = body?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        }

                                        var applicationNotifications = new List<SysNotificationDto>();

                                        applicationNotifications.Add(new SysNotificationDto()
                                        {
                                            ActorId = litigationRequestArbitratorAssigningRequest.CitizenId,
                                            CreatedOn = DateTime.Now,
                                            ActorTypeId = NotificationActorType.Arbitrator,
                                            IsViewed = false,
                                            ViewedOn = null,
                                            RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequest.Id,
                                            RequestTypeId = RequestType.ArbitratorTaskrequests,
                                            Message = msg
                                        });

                                        var notificationResult = await _notificationService.SendNotifications(
                                            new SendEmailDto()
                                            {
                                                Body = body,
                                                Subject = "سرعة الرد على طلب مهمة التحكيم",
                                                To = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Email }

                                            },
                                            new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                            {
                                                message_text = msg,
                                                notification_type = "SMS",
                                                recipients = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Mobile }
                                            }, applicationNotifications
                                            );
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }

                        if (LitigationRequestArbitratorAssigningRequestDecisionPeriod > 0)
                        {
                            if (litigationRequestArbitratorAssigningRequest.LitigationRequest.IsArbitratorReminderForCancelArbitrationSent != true)
                            {
                                double totalHours = 0;

                                totalHours += (DateTime.Now - litigationRequestArbitratorAssigningRequest.RequestOn).TotalHours;
                                if (totalHours >= LitigationRequestArbitratorAssigningRequestDecisionPeriod)
                                {
                                    litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.Cancelled;
                                    litigationRequestArbitratorAssigningRequest.ResponseOn = DateTime.Now;
                                    if (litigationRequestArbitratorAssigningRequests.Count() == index + 1)
                                    {
                                        litigationRequestArbitratorAssigningRequest.LitigationRequest.IsArbitratorReminderForCancelArbitrationSent = true;
                                    }
                                    // Send Notification for old arbitrator and dept Admin
                                    // old arbitrator = litigationRequestArbitratorAssigningRequest.CitizenId
                                    // Mail - Sms - Application Notification
                                    // تم إلغاء طلب مهمة التحكيم كمحكم على الدعوى رقم ..... لتجاوز الرد على المهمة مهلته القصوى 

                                    try
                                    {
                                        var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                                        var msg = "";
                                        string body;
                                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                        using (StreamReader streamReader = File.OpenText(pathToFile))
                                        {
                                            body = await streamReader.ReadToEndAsync();
                                        }
                                        switch (litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId)
                                        {
                                            case ArbitratorAssigningRequestType.Arbitrator:
                                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_Cancelled).Result.Data?.Value;
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_Cancelled).Result.Data?.Value);
                                                break;
                                            case ArbitratorAssigningRequestType.ArbitratorsChairman:
                                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorChairmanTaskRequest_Cancelled).Result.Data?.Value;
                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_Cancelled).Result.Data?.Value);
                                                break;
                                        }
                                        body = body?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        msg = msg.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                        var applicationNotifications = new List<SysNotificationDto>();

                                        applicationNotifications.Add(new SysNotificationDto()
                                        {
                                            ActorId = litigationRequestArbitratorAssigningRequest.CitizenId,
                                            CreatedOn = DateTime.Now,
                                            ActorTypeId = NotificationActorType.Arbitrator,
                                            IsViewed = false,
                                            ViewedOn = null,
                                            RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequest.Id,
                                            RequestTypeId = RequestType.ArbitratorTaskrequests,
                                            Message = msg
                                        });

                                        applicationNotifications.Add(new SysNotificationDto()
                                        {
                                            ActorId = deptAdminInfoArbitration.Data.Id,
                                            CreatedOn = DateTime.Now,
                                            ActorTypeId = NotificationActorType.Employee,
                                            IsViewed = false,
                                            ViewedOn = null,
                                            RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequest.Id,
                                            RequestTypeId = RequestType.ArbitratorTaskrequests,
                                            Message = msg
                                        });

                                        var notificationResult = await _notificationService.SendNotifications(
                                            new SendEmailDto()
                                            {
                                                Body = body,
                                                Subject = "تم إلغاء طلب مهمة التحكيم كمحكم",
                                                To = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Email, deptAdminInfoArbitration.Data.Email }

                                            },
                                            new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                            {
                                                message_text = msg,
                                                notification_type = "SMS",
                                                recipients = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Mobile, deptAdminInfoArbitration.Data.Mobile }
                                            }, applicationNotifications
                                            );
                                    }
                                    catch (Exception ex)
                                    {

                                    }



                                    switch (litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId)
                                    {
                                        case ArbitratorAssigningRequestType.Arbitrator:

                                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                            {
                                                Id = Guid.NewGuid(),
                                                CreatedOn = DateTime.Now,
                                                RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                                ShowForApplicant = false,
                                                LogMessage = "تم إلغاء طلب مهمة التحكيم للمحكم  " + litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName + " كمحكم"
                                            });
                                            ///TODO: if single arbitrator or multiple
                                            if (litigationRequestArbitratorAssigningRequest.LitigationRequest.ArbitratorsCount > 1)
                                            {
                                                var arbitratorsAssigned = litigationRequestArbitratorAssigningRequest.LitigationRequest.LitigationRequestArbitratorNominationLists.Where(s => s.IsSent == true && s.IsResponse != true).ToList();
                                                foreach (var item in arbitratorsAssigned)
                                                {
                                                    item.IsResponse = true;
                                                    //update litigation request for plaintiff or deffendant to reassign
                                                    if (item.IsDefendantSelected == true)
                                                    {
                                                        item.LitigationRequest.IsDefendantArbitratorNominationListDone = false;
                                                    }
                                                    if (item.IsPlaintiffSelected == true)
                                                    {
                                                        item.LitigationRequest.IsPlaintiffArbitratorNominationListDone = false;
                                                    }
                                                    //item.LitigationRequestArbitratorAssigningRequests.FirstOrDefault().Response = LitigationRequestArbitratorAssigningRequestStatus.Cancelled;
                                                }

                                                // Send Notification
                                                // يرجى اعادة تعيين محكم جديد للدعوة رقم 
                                                try
                                                {
                                                    string body;
                                                    var msg = $"يتطلب القرار اليدوي لاعادة تعيين محكم للدعوى رقم {litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode}";
                                                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                    using (StreamReader streamReader = File.OpenText(pathToFile))
                                                    {
                                                        body = await streamReader.ReadToEndAsync();
                                                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_ReAssign).Result.Data?.Value);
                                                        body = body.Replace("{ArbitratorName}", litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName);
                                                        body = body?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                                    }
                                                    var applicationNotifications = new List<SysNotificationDto>();
                                                    if (litigationRequestArbitratorAssigningRequest.LitigationRequest.IsDefendantArbitratorNominationListDone == false)
                                                    {
                                                        await SendNotificationMS(body, msg, litigationRequestArbitratorAssigningRequest.LitigationRequest, "اعادة تعيين محكم جديد", NotificationActorType.Defendant, RequestType.ArbitratorTaskrequests
                                                               , notifyDeptAdmin: true, notifySecondParty: true);

                                                    }
                                                    if (litigationRequestArbitratorAssigningRequest.LitigationRequest.IsPlaintiffArbitratorNominationListDone == false)
                                                    {
                                                        await SendNotificationMS(body, msg, litigationRequestArbitratorAssigningRequest.LitigationRequest, "اعادة تعيين محكم جديد", NotificationActorType.Plaintiff, RequestType.ArbitratorTaskrequests
                                                               , notifyDeptAdmin: true, notifyFirstParty: true, notifyApplicant: true);
                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                            else
                                            {
                                                var arbitratorsList = litigationRequestArbitratorAssigningRequest.LitigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsSent != true).OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                                                if (arbitratorsList != null)
                                                {
                                                    if (arbitratorsList.Count > 0)
                                                    {
                                                        LitigationRequestArbitratorAssigningRequest nextLitigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                                                        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId;
                                                        nextLitigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                                                        nextLitigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
                                                        nextLitigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = arbitratorsList.FirstOrDefault().Id;
                                                        nextLitigationRequestArbitratorAssigningRequest.CitizenId = arbitratorsList.FirstOrDefault().ArbitratorCitizenId;
                                                        nextLitigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                                                        await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(nextLitigationRequestArbitratorAssigningRequest);
                                                        arbitratorsList.FirstOrDefault().IsSent = true;

                                                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                                        {
                                                            Id = Guid.NewGuid(),
                                                            CreatedOn = DateTime.Now,
                                                            RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                                            ShowForApplicant = false,
                                                            LogMessage = "تم إرسال طلب مهمة تحكيم للمحكم " + arbitratorsList.FirstOrDefault()?.ArbitratorCitizen.FullName + "كمحكم"
                                                        });
                                                        // Send Notification for new arbitrator and arbiration department admin
                                                        // new arbitrator = arbitratorsList.FirstOrDefault().ArbitratorCitizenId
                                                        // Mail - Sms - Application Notification
                                                        // تم إرسال طلب مهمة تحكيم جديدة كمحكم على الدعوى رقم ..... 
                                                        try
                                                        {
                                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_ReAssign).Result.Data?.Value;
                                                            msg = msg.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                            string body;
                                                            using (StreamReader streamReader = File.OpenText(pathToFile))
                                                            {
                                                                body = await streamReader.ReadToEndAsync();
                                                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_ReAssign).Result.Data?.Value);
                                                                body = body?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                                            }
                                                            await SendNotificationMS(body, msg, litigationRequestArbitratorAssigningRequest.LitigationRequest, "تعيين رئيس هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorTaskrequests
                                                            , notifyDeptAdmin: true);

                                                            var applicationNotifications = new List<SysNotificationDto>();

                                                            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                                                                .FirstOrDefault(c => c.Id == nextLitigationRequestArbitratorAssigningRequest.CitizenId);


                                                            applicationNotifications.Add(new SysNotificationDto()
                                                            {
                                                                ActorId = nextLitigationRequestArbitratorAssigningRequest.CitizenId,
                                                                CreatedOn = DateTime.Now,
                                                                ActorTypeId = NotificationActorType.Arbitrator,
                                                                IsViewed = false,
                                                                ViewedOn = null,
                                                                RequestId = nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                                                RequestTypeId = RequestType.ArbitratorTaskrequests,
                                                                Message = msg
                                                            });

                                                            var notificationResult = await _notificationService.SendNotifications(
                                                                new SendEmailDto()
                                                                {
                                                                    Body = msg,
                                                                    Subject = "تم إرسال طلب مهمة تحكيم جديدة كمحكم",
                                                                    To = new List<string>() { citizenInfo.Email }

                                                                },
                                                                new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                                {
                                                                    message_text = msg,
                                                                    notification_type = "SMS",
                                                                    recipients = new List<string>() { citizenInfo.Mobile }
                                                                }, applicationNotifications
                                                                );
                                                        }
                                                        catch (Exception ex)
                                                        {

                                                        }


                                                    }
                                                }
                                            }
                                            break;
                                        case ArbitratorAssigningRequestType.ArbitratorsChairman:

                                            // Send Notification for old arbitrator
                                            // old arbitrator = litigationRequestArbitratorAssigningRequest.CitizenId
                                            // Mail - Sms - Application Notification
                                            // تم إلغاء طلب مهمة التحكيم كرئيس هيئة تحكيم على الدعوى رقم ..... لتجاوز الرد على المهمة مهلته القصوى 
                                            try
                                            {
                                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorChairmanTaskRequest_Cancelled).Result.Data?.Value;
                                                msg = msg.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                string body;
                                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                                {
                                                    body = await streamReader.ReadToEndAsync();
                                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorChairmanTaskRequest_Cancelled).Result.Data?.Value);
                                                    body = body?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                                }

                                                var applicationNotifications = new List<SysNotificationDto>();

                                                applicationNotifications.Add(new SysNotificationDto()
                                                {
                                                    ActorId = litigationRequestArbitratorAssigningRequest.CitizenId,
                                                    CreatedOn = DateTime.Now,
                                                    ActorTypeId = NotificationActorType.Arbitrator,
                                                    IsViewed = false,
                                                    ViewedOn = null,
                                                    RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequest.Id,
                                                    RequestTypeId = RequestType.ArbitratorTaskrequests,
                                                    Message = msg
                                                });

                                                var notificationResult = await _notificationService.SendNotifications(
                                                    new SendEmailDto()
                                                    {
                                                        Body = body,
                                                        Subject = "تم إلغاء طلب مهمة التحكيم كرئيس هيئة تحكيم",
                                                        To = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Email }

                                                    },
                                                    new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                    {
                                                        message_text = msg,
                                                        notification_type = "SMS",
                                                        recipients = new List<string>() { litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.Mobile }
                                                    }, applicationNotifications
                                                    );
                                            }
                                            catch (Exception ex)
                                            {

                                            }


                                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                            {
                                                Id = Guid.NewGuid(),
                                                CreatedOn = DateTime.Now,
                                                RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                                ShowForApplicant = false,
                                                LogMessage = "تم إلغاء طلب مهمة التحكيم للمحكم  " + litigationRequestArbitratorAssigningRequest.ArbitratorCitizen.FullName + " كرئيس هيئة التحكيم"
                                            });
                                            var chairmanArbitratorsList = litigationRequestArbitratorAssigningRequest.LitigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Where(x => x.IsSent != true).OrderBy(x => x.TotalPriority).ToList();

                                            // Calculate total priority scores for each arbitrator
                                            var arbitratorScores = new List<ArbitratorChairmanSelectionScore>();

                                            foreach (var arbitrator in chairmanArbitratorsList)
                                            {
                                                var score = new ArbitratorChairmanSelectionScore
                                                {
                                                    Arbitrator = arbitrator,
                                                    TotalScore = arbitrator.TotalPriority,
                                                    ExperienceYears = arbitrator.ArbitratorChairmanCitizen?.CitizenRequests?
                                                                                                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears ?? 0,
                                                    RequestsCount = arbitrator.ArbitratorChairmanCitizen?.ArbitratorLitigationRequests.Count() + arbitrator.ArbitratorChairmanCitizen?.LitigationRequestArbitratorLists.Count,
                                                    GeographicalPriority = await CompareGeographicalDistanceChairman(arbitrator.ArbitratorChairmanCitizen, litigationRequestArbitratorAssigningRequest.LitigationRequest)
                                                };
                                                arbitratorScores.Add(score);
                                            }

                                            // Apply score & weight calculation criteria
                                            var selectedArbitrator = await ApplySelectionCriteriaChairman(litigationRequestArbitratorAssigningRequest.LitigationRequest, chairmanArbitratorsList, arbitratorScores);

                                            if (selectedArbitrator != null)
                                            {

                                                LitigationRequestArbitratorAssigningRequest nextLitigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                                                nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId;
                                                nextLitigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                                                nextLitigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
                                                nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId;
                                                //nextLitigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = chairmanArbitratorsList.FirstOrDefault().Id;
                                                nextLitigationRequestArbitratorAssigningRequest.CitizenId = selectedArbitrator.ArbitratorChairmanCitizenId;
                                                nextLitigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                                                await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(nextLitigationRequestArbitratorAssigningRequest);
                                                selectedArbitrator.IsSent = true;

                                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CreatedOn = DateTime.Now,
                                                    RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                                    ShowForApplicant = false,
                                                    LogMessage = "تم إرسال طلب مهمة للمحكم " + selectedArbitrator?.ArbitratorChairmanCitizen.FullName + " كرئيس هيئة التحكيم"
                                                });
                                                // Send Notification for old arbitrator and arbiration department admin
                                                // new arbitrator = arbitratorsList.FirstOrDefault().ArbitratorCitizenId
                                                // Mail - Sms - Application Notification
                                                // تم إرسال طلب مهمة تحكيم جديدة كرئيس هيئة على الدعوى رقم ..... 
                                                try
                                                {
                                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorChairmanTaskRequest_ReAssign).Result.Data?.Value;
                                                    msg = msg.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                                    string body;
                                                    using (StreamReader streamReader = File.OpenText(pathToFile))
                                                    {
                                                        body = await streamReader.ReadToEndAsync();
                                                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorChairmanTaskRequest_ReAssign).Result.Data?.Value);
                                                        body = body?.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);
                                                    }
                                                    await SendNotificationMS(body, msg, litigationRequestArbitratorAssigningRequest.LitigationRequest, "تم إرسال طلب مهمة تحكيم جديدة كرئيس هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorTaskrequests
                                                    , notifyDeptAdmin: true);

                                                    var applicationNotifications = new List<SysNotificationDto>();

                                                    var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                                                        .FirstOrDefault(c => c.Id == nextLitigationRequestArbitratorAssigningRequest.CitizenId);


                                                    applicationNotifications.Add(new SysNotificationDto()
                                                    {
                                                        ActorId = nextLitigationRequestArbitratorAssigningRequest.CitizenId,
                                                        CreatedOn = DateTime.Now,
                                                        ActorTypeId = NotificationActorType.Arbitrator,
                                                        IsViewed = false,
                                                        ViewedOn = null,
                                                        RequestId = nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                                        RequestTypeId = RequestType.ArbitratorTaskrequests,
                                                        Message = msg
                                                    });

                                                    var notificationResult = await _notificationService.SendNotifications(
                                                        new SendEmailDto()
                                                        {
                                                            Body = msg,
                                                            Subject = "تم إرسال طلب مهمة تحكيم جديدة كمحكم",
                                                            To = new List<string>() { citizenInfo.Email }

                                                        },
                                                        new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                                        {
                                                            message_text = msg,
                                                            notification_type = "SMS",
                                                            recipients = new List<string>() { citizenInfo.Mobile }
                                                        }, applicationNotifications
                                                        );
                                                }
                                                catch (Exception ex)
                                                {

                                                }

                                            }
                                            break;
                                    }
                                }
                            }
                        }
                        index++;
                        var repositorySaveResponse = await _repository.SaveChangesAsync(default);
                        if (!repositorySaveResponse.IsDone)
                            return repositorySaveResponse;
                    }
                }
            }


            var litigationRequestArbitrationExtendRequests = (await _repository.GetQueryable<LitigationRequestExtendPeriodRequest, Guid>())
                                .Include(x => x.LitigationRequest.ArbitratorCitizen)
                .Where(x => x.LitigationRequestExtendPeriodRequestTypeId == LitigationRequestExtendPeriodRequestType.Arbitration
                && x.LitigationRequestExtendPeriodRequestStatusId == null && x.CreatedOn != null);

            if (litigationRequestArbitrationExtendRequests.Any())
            {
                int LitigationRequestReconciliationExtendRequestResponsePeriod = 0;
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_LitigationRequestExtendRequestReplyPeriod).Result.Data?.Value, out LitigationRequestReconciliationExtendRequestResponsePeriod);


                foreach (var extendRequest in litigationRequestArbitrationExtendRequests)
                {
                    if (extendRequest.CreatedOn is not null)
                    {
                        double totalHours = 0;

                        totalHours += (DateTime.Now - extendRequest.CreatedOn).Value.TotalHours;

                        if (totalHours > LitigationRequestReconciliationExtendRequestResponsePeriod)
                        {
                            extendRequest.LitigationRequestExtendPeriodRequestStatusId = LitigationRequestExtendPeriodRequestStatus.Reject;
                            extendRequest.FinishStatusReason = "لم يتم الرد في خلال الفتره المحدده";

                            // Send Notification
                            // تم رفض طلب مد مدة التحكيم
                            try
                            {
                                var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExtendRequest_Rejected).Result.Data?.Value;
                                msg = msg.Replace("{LitigationCode}", extendRequest.LitigationRequest.RequestCode);
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                string body;
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExtendRequest_Rejected).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", extendRequest.LitigationRequest?.RequestCode);
                                }
                                await SendNotificationMS(body, msg, extendRequest.LitigationRequest, "رفض طلب مد مدة التحكيم", NotificationActorType.Arbitrator, RequestType.ExtendArbitrationPeriodRequest, notifyArbitratorChairman: true, notifyDeptAdmin: true);

                            }
                            catch (Exception e)
                            {
                            }

                        }
                    }

                }
            }


            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                               .Include(x => x.ArbitratorCitizen)
                               .Include(x => x.LitigationRequestSessions.Where(y => y.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration))
                             .Where(x => x.LitigationRequestStage == LitigationRequestStage.Arbitration
                             & x.IsFinished != true & x.ArbitratorCitizenId != null)
                             .ToList();

            if (litigationRequests is not null && litigationRequests.Any())
            {
                foreach (var litigationRequest in litigationRequests)
                {
                    if (!litigationRequest.LitigationRequestSessions.Any())
                    {
                        var arbitratorChairmanAssigningDate = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())?
                            .FirstOrDefault(x => x.LitigationRequestId == litigationRequest.Id && x.CitizenId == litigationRequest.ArbitratorCitizenId && x.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve)?.ResponseOn;

                        if (arbitratorChairmanAssigningDate != null && litigationRequest.IsArbitratorSessionTimeAddedPeriodSent != true)
                        {
                            int AddLitigationRequestArbitrationSessionPeriod = 0;
                            int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AddLitigationRequestArbitrationSessionPeriod).Result.Data?.Value, out AddLitigationRequestArbitrationSessionPeriod);
                            double totalHours = 0;

                            totalHours += (DateTime.Now - arbitratorChairmanAssigningDate).Value.TotalHours;

                            if (totalHours > AddLitigationRequestArbitrationSessionPeriod)
                            {
                                litigationRequest.IsArbitratorSessionTimeAddedPeriodSent = true;

                                // Send Notification
                                // اشعار بتحديد موعد الجلسة
                                try
                                {
                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AddLitigationRequestArbitrationSessionWarning).Result.Data?.Value;
                                    msg = msg.Replace("{LitigationRequest}", litigationRequest.RequestCode);
                                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                    string body;
                                    using (StreamReader streamReader = File.OpenText(pathToFile))
                                    {
                                        body = await streamReader.ReadToEndAsync();
                                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExtendRequest_Rejected).Result.Data?.Value);
                                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                    }
                                    await SendNotificationMS(body, msg, litigationRequest, "اشعار بتحديد موعد الجلسة", NotificationActorType.Arbitrator, RequestType.ExtendArbitrationPeriodRequest, notifyArbitratorChairman: true);

                                }
                                catch (Exception e)
                                {
                                }
                            }
                        }
                    }
                }
            }







            var repositoryLitigationRequestExtendRequestResponse = await _repository.SaveChangesAsync(default);

            await SendVerdictSessionReminders();

            return new BaseResponse() { IsDone = true };
        }

        private async Task SendVerdictSessionReminders()
        {
            int reminderHoursBefore = 24;
            int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_VerdictSessionReminderHoursBefore).Result.Data?.Value, out reminderHoursBefore);
            if (reminderHoursBefore <= 0) reminderHoursBefore = 24;

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                .Include(x => x.ApplicantCitizen)
                .Include(x => x.ArbitratorCitizen)
                .Include(x => x.Contract.FirstPartyCitizen)
                .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                .Include(x => x.Contract.SecondPartyCitizen)
                .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                .Include(x => x.LitigationRequestSessions)
                .Where(x => x.IsVerdictSessionReminderSent != true
                    && x.IsFinished != true
                    && x.LitigationRequestStage == LitigationRequestStage.Arbitration
                    && x.LitigationRequestSessions.Any(s =>
                        s.SessionTypeId == LitigationSessionType.VerdictPronouncement
                        && s.SessionDatetime > DateTime.Now))
                .ToList();

            if (litigationRequests == null || !litigationRequests.Any())
                return;

            var reminderMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_VerdictSessionReminderMessage).Result.Data?.Value
                ?? "تذكير: موعد جلسة النطق بالحكم للدعوى رقم {LitigationCode} خلال {Hours} ساعة";

            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");

            foreach (var litigationRequest in litigationRequests)
            {
                var verdictSession = litigationRequest.LitigationRequestSessions
                    .FirstOrDefault(s => s.SessionTypeId == LitigationSessionType.VerdictPronouncement
                        && s.SessionDatetime > DateTime.Now);
                if (verdictSession == null) continue;

                double hoursUntilSession = (verdictSession.SessionDatetime - DateTime.Now).TotalHours;
                if (hoursUntilSession > reminderHoursBefore) continue;

                litigationRequest.IsVerdictSessionReminderSent = true;
                _repository.Update<LitigationRequest, Guid>(litigationRequest);

                try
                {
                    var msg = reminderMsg
                        .Replace("{LitigationCode}", litigationRequest.RequestCode)
                        .Replace("{Hours}", reminderHoursBefore.ToString());

                    string body = string.Empty;
                    if (File.Exists(pathToFile))
                    {
                        using StreamReader sr = File.OpenText(pathToFile);
                        body = await sr.ReadToEndAsync();
                        body = body?.Replace("{content}", msg);
                        body = body?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                    }

                    await SendNotificationMS(body, msg, litigationRequest, "تذكير بجلسة النطق بالحكم",
                        NotificationActorType.Plaintiff, RequestType.LitigationRequest,
                        notifyApplicant: true, notifyFirstParty: true, notifySecondParty: true, notifyDeptAdmin: true, notifyArbitrators: true, notifyArbitratorChairman: true);
                }
                catch (Exception ex)
                {
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database, new LogDto()
                    {
                        Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                        Message = ex.ToString()
                    });
                }
            }
        }


        private async Task SendNotificationMS(string mailMsg, string msg, LitigationRequest litigationRequest, string subject, NotificationActorType notificationActorType, RequestType requestType, LitigationRequestArbitratorList? arbitrator = null, bool notifyApplicant = false, bool notifyArbitrators = false, bool notifyDeptAdmin = false, bool notifyFirstParty = false, bool notifySecondParty = false, bool notifyArbitratorChairman = false)
        {
            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
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

            // Notify Arbitrator Citizen
            if (notifyArbitrators && arbitrator?.ArbitratorCitizen is not null)
            {
                objEmail.To.Add(arbitrator.ArbitratorCitizen.Email);
                objSms.recipients.Add(arbitrator.ArbitratorCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = arbitrator.ArbitratorCitizenId,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Department Admin
            if (notifyDeptAdmin && deptAdminInfoArbitration != null)
            {
                objEmail.To.Add(deptAdminInfoArbitration.Data.Email);
                objSms.recipients.Add(deptAdminInfoArbitration.Data.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = deptAdminInfoArbitration.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Chairman 
            if (notifyArbitratorChairman && litigationRequest.ArbitratorCitizen != null)
            {
                objEmail.To.Add(litigationRequest.ArbitratorCitizen.Email);
                objSms.recipients.Add(litigationRequest.ArbitratorCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ArbitratorCitizen.Id,
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
            // Notify First Party Facility
            //if (notifyFirstParty && litigationRequest.Contract?.FirstPartyFacility != null && litigationRequest.Contract.FirstPartyFacility.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.FirstPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}



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
            // Notify Second Party Facility
            //if (notifySecondParty && litigationRequest.Contract?.SecondPartyFacility != null)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.SecondPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
        }



        #region Extention method for Add Single Nomination Chairman Assigning Arbitrator


        private async Task<LitigationRequestArbitratorsChairmanNominationList> ApplySelectionCriteriaChairman(LitigationRequest litigationRequest, List<LitigationRequestArbitratorsChairmanNominationList> arbitratorsList, List<ArbitratorChairmanSelectionScore> arbitratorScores)
        {

            // Step 1: Find arbitrators with minimum total score
            var minScore = arbitratorScores.Min(x => x.TotalScore);
            var candidatesWithMinScore = arbitratorScores.Where(x => x.TotalScore == minScore).ToList();

            if (candidatesWithMinScore.Count == 1)
            {
                return candidatesWithMinScore.First().Arbitrator;
            }

            // Step 2: Apply experience tiebreaker (highest experience wins)
            var maxExperience = candidatesWithMinScore.Max(x => x.ExperienceYears);
            var experienceCandidates = candidatesWithMinScore.Where(x => x.ExperienceYears == maxExperience).ToList();

            if (experienceCandidates.Count == 1)
            {
                return experienceCandidates.First().Arbitrator;
            }

            // Step 3: Apply case count tiebreaker (lowest case count wins)
            var minCaseCount = experienceCandidates.Min(x => x.RequestsCount);
            var caseCountCandidates = experienceCandidates.Where(x => x.RequestsCount == minCaseCount).ToList();

            if (caseCountCandidates.Count == 1)
            {
                return caseCountCandidates.First().Arbitrator;
            }

            // Step4: Apply geographical distance tiebreaker (closest area wins)
            var bestGeographicalPriority = caseCountCandidates.Min(x => x.GeographicalPriority);
            var geographicalCandidates = caseCountCandidates.Where(x => x.GeographicalPriority == bestGeographicalPriority).ToList();

            if (geographicalCandidates.Count == 1)
            {
                return geographicalCandidates.First().Arbitrator;
            }

            // If still tied, return null to escalate to manager
            // Escalate to arbitration office manager
            await EscalateToManagerChairman(litigationRequest, arbitratorsList, geographicalCandidates);
            return null;
        }
        private async Task<decimal> CompareGeographicalDistanceChairman(Citizen arbitratorCitizen, LitigationRequest litigationRequest)
        {
            var arbitratorCitizenRequest = arbitratorCitizen.CitizenRequests?.Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                       .OrderBy(x => x.RequestOn).LastOrDefault();

            if (arbitratorCitizenRequest == null) return int.MaxValue;

            if (litigationRequest.AreaId == arbitratorCitizenRequest.AreaId)
            {
                return 0;
            }
            else
            {
                var areaDistances = (await _repository.GetQueryable<SysAreaDistances, Guid>())
                                  .Where(s => s.SourceAreaId == litigationRequest.AreaId && s.DestinationAreaId == arbitratorCitizenRequest.AreaId).FirstOrDefault();
                if (areaDistances != null)
                {
                    return areaDistances.Distance;
                }
                return int.MaxValue;
            }
            // Priority: 0 = same area, 1 = different area
            //if (arbitratorCitizenRequest.AreaId == litigationRequest.AreaId)
            //{
            //    return 0; // Same area - highest priority
            //}
            //else
            //{
            //    return 1; // Different area - lowest priority
            //}
        }

        // Escalate to arbitration office manager
        private async Task EscalateToManagerChairman(LitigationRequest litigationRequest, List<LitigationRequestArbitratorsChairmanNominationList> tiedArbitrators, List<ArbitratorChairmanSelectionScore> arbitratorScores)
        {
            // Find arbitrators with minimum total score
            //var minScore = arbitratorScores.Min(x => x.TotalScore);
            //tiedArbitrators = tiedArbitrators.Where(x => x.TotalPriority == minScore).ToList();
            tiedArbitrators = arbitratorScores.Select(x => x.Arbitrator).ToList();

            foreach (var arbitrator in tiedArbitrators)
            {

                // Reset the sent status for escalation
                arbitrator.IsEscalated = true;
                arbitrator.EscalateDate = DateTime.Now;
                //arbitrator.IsSelectedFromManager = false;
            }
            litigationRequest.IsArbitratorsChairmanDecisionRequired = true;
            // Notify arbitration office manager
            // Send Notification
            // يتطلب القرار اليدوي لاختيار المحكم للدعوى
            try
            {
                var msg = $"يتطلب القرار اليدوي لاختيار المحكم للدعوى رقم {litigationRequest.RequestCode}";
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AssignedAndSent).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }
                var arbitrationAdmin = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                if (arbitrationAdmin.IsDone)
                {
                    List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        }

                        };
                    List<string> emailList = new List<string>() {
                                  arbitrationAdmin.Data?.Email
                                };

                    List<string> recipientList = new List<string>()
                            {
                                  arbitrationAdmin.Data?.Mobile
                            };

                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                    {
                        To = emailList,
                        Subject = "يتطلب القرار اليدوي لاختيار المحكم للدعوى رقم",
                        Body = body
                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                    {
                        message_text = msg,
                        recipients = recipientList,
                        notification_type = "SMS",

                    }, notificationList);
                }
            }
            catch (Exception ex)
            {


            }

        }

        #endregion

        public async Task AddSingleNominationAssigningArbitrator(LitigationRequest litigationRequest, LitigationRequestArbitratorNominationList selectedArbitrator)
        {

            LitigationRequestArbitratorAssigningRequest litigationRequestOneArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.RequestOn = DateTime.Now;
            litigationRequestOneArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = selectedArbitrator.Id;
            litigationRequestOneArbitratorAssigningRequest.CitizenId = selectedArbitrator.ArbitratorCitizenId;
            litigationRequestOneArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
            await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestOneArbitratorAssigningRequest);
            selectedArbitrator.IsSent = true;
            // Send Notification
            // يوجد طلب مهمه جديده كمحكم علي الدعوي رقم
            try
            {
                var Fmsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value;
                Fmsg = Fmsg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }
                var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                                    .Where(c => c.Id == litigationRequestOneArbitratorAssigningRequest.CitizenId)
                                    .FirstOrDefault();


                List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = litigationRequestOneArbitratorAssigningRequest.CitizenId,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.ArbitratorTaskrequests,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=Fmsg
                        }

                        };
                List<string> emailList = new List<string>() {
                                  citizenInfo.Email
                                };

                List<string> recipientList = new List<string>()
                            {
                                  citizenInfo.Mobile
                            };

                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                {
                    To = emailList,
                    Subject = "طلب مهمة جديدة كمحكم علي الدعوي",
                    Body = body
                }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                {
                    message_text = Fmsg,
                    recipients = recipientList,
                    notification_type = "SMS",

                }, notificationList);

            }
            catch (Exception e)
            {


            }


        }

        // Escalate to arbitration office manager
        private async Task EscalateToManager(LitigationRequest litigationRequest, List<LitigationRequestArbitratorNominationList> tiedArbitrators, List<ArbitratorSelectionScore> arbitratorScores)
        {
            // Find arbitrators with minimum total score
            //var minScore = arbitratorScores.Min(x => x.TotalScore);
            //tiedArbitrators = tiedArbitrators.Where(x => x.TotalPriority == minScore).ToList();
            tiedArbitrators = arbitratorScores.Select(x => x.Arbitrator).ToList();

            foreach (var arbitrator in tiedArbitrators)
            {

                // Reset the sent status for escalation
                arbitrator.IsEscalated = true;
                arbitrator.EscalateDate = DateTime.Now;
                //arbitrator.IsSelectedFromManager = false;
            }

            // Notify arbitration office manager
            // Send Notification
            // يتطلب القرار اليدوي لاختيار المحكم للدعوى
            try
            {
                var msg = $"يتطلب القرار اليدوي لاختيار المحكم للدعوى رقم {litigationRequest.RequestCode}";
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AssignedAndSent).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }
                var arbitrationAdmin = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                if (arbitrationAdmin.IsDone)
                {
                    List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        }

                        };
                    List<string> emailList = new List<string>() {
                                  arbitrationAdmin.Data?.Email
                                };

                    List<string> recipientList = new List<string>()
                            {
                                  arbitrationAdmin.Data?.Mobile
                            };

                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                    {
                        To = emailList,
                        Subject = "يتطلب القرار اليدوي لاختيار المحكم للدعوى رقم",
                        Body = body
                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                    {
                        message_text = msg,
                        recipients = recipientList,
                        notification_type = "SMS",

                    }, notificationList);
                }
            }
            catch (Exception ex)
            {


            }

        }

    }
}
