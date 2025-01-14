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
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

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
        public ArbitrationSlaService(ISysSettingService sysSettingService, IMapper mapper, IEntityRepository repository, ISysEmployeeService employeeService, INotificationService notificationService)
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
                              .Where(x => x.LitigationRequestStage == LitigationRequestStage.Arbitration
                              & x.IsFinished != true)
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

                                            await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية الصلح", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
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

                                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsCount_SecondWarning).Result.Data?.Value;
                                        msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                                        await SendNotificationMS(msg, litigationRequest, "تذكيرسرعة اتمام عملية الصلح", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
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

                                            await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار عدد المحكمين", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
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

                                            await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار عدد المحكمين", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
                                              , notifyApplicant: true, notifyFirstParty: true);
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

                                            await SendNotificationMS(msg, litigationRequest, "لم يقم المدعى والمدعى عليه بتحديد عدد المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
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

                                                await SendNotificationMS(msg, litigationRequest, "لم يقم المدعى بتحديد عدد المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
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

                                                await SendNotificationMS(msg, litigationRequest, "لم يقم المدعى بتحديد عدد المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
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

                                            await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
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

                                            await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
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
                                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorsSelect_FirstWarning).Result.Data?.Value;
                                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                                            await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Plaintiff, RequestType.ArbitratorNominationList
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

                                            await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار هيئة التحكيم", NotificationActorType.Defendant, RequestType.ArbitratorNominationList
                                              , notifySecondParty: true);
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

                                            await SendNotificationMS(msg, litigationRequest, "لم يقم المدعى والمدعى عليه بإنهاء تعيين هيئة المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
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

                                                await SendNotificationMS(msg, litigationRequest, "لم يقم المدعى بإنهاء تعيين هيئة المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
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

                                                await SendNotificationMS(msg, litigationRequest, "لم يقم المدعى عليه بإنهاء تعيين هيئة المحكمين", NotificationActorType.Employee, RequestType.ArbitratorNominationList
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

                                                await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار رئيس هيئة التحكيم", NotificationActorType.Arbitrator, RequestType.ArbitratorChairmanNominationList, arbitrator
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

                                                await SendNotificationMS(msg, litigationRequest, "سرعة اتمام عملية إختيار رئيس هيئة التحكيم", NotificationActorType.Arbitrator, RequestType.ArbitratorChairmanNominationList, arbitrator
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

                                            await SendNotificationMS(msg, litigationRequest, "لم يتم تعيين رئيس هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorChairmanNominationList
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
                                  .ThenInclude(x => x.LitigationRequestArbitratorNominationLists)
                                  .ThenInclude(x => x.ArbitratorCitizen)
                                  .Include(x => x.LitigationRequest)
                                  .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                                  .ThenInclude(x => x.ArbitratorChairmanCitizen)
                                  .Where(x => x.Response == LitigationRequestArbitratorAssigningRequestStatus.None)
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
                                                Body = msg,
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
                                                Body = msg,
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
                            double totalHours = 0;

                            totalHours += (DateTime.Now - litigationRequestArbitratorAssigningRequest.RequestOn).TotalHours;
                            if (totalHours >= LitigationRequestArbitratorAssigningRequestDecisionPeriod)
                            {
                                litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.Cancelled;
                                litigationRequestArbitratorAssigningRequest.ResponseOn = DateTime.Now;
                                // Send Notification for old arbitrator
                                // old arbitrator = litigationRequestArbitratorAssigningRequest.CitizenId
                                // Mail - Sms - Application Notification
                                // تم إلغاء طلب مهمة التحكيم كمحكم على الدعوى رقم ..... لتجاوز الرد على المهمة مهلته القصوى 
                                try
                                {
                                    var msg = "";
                                    switch (litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId)
                                    {
                                        case ArbitratorAssigningRequestType.Arbitrator:
                                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorTaskRequest_Cancelled).Result.Data?.Value;
                                            break;
                                        case ArbitratorAssigningRequestType.ArbitratorsChairman:
                                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorChairmanTaskRequest_Cancelled).Result.Data?.Value;
                                            break;
                                    }

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

                                    var notificationResult = await _notificationService.SendNotifications(
                                        new SendEmailDto()
                                        {
                                            Body = msg,
                                            Subject = "تم إلغاء طلب مهمة التحكيم كمحكم",
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

                                                    await SendNotificationMS(msg, litigationRequestArbitratorAssigningRequest.LitigationRequest, "لم يتم تعيين رئيس هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorTaskrequests
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
                                                    Body = msg,
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
                                        if (chairmanArbitratorsList != null)
                                        {
                                            if (chairmanArbitratorsList.Count > 0)
                                            {
                                                LitigationRequestArbitratorAssigningRequest nextLitigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                                                nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId;
                                                nextLitigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                                                nextLitigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
                                                nextLitigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId;
                                                //nextLitigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = chairmanArbitratorsList.FirstOrDefault().Id;
                                                nextLitigationRequestArbitratorAssigningRequest.CitizenId = chairmanArbitratorsList.FirstOrDefault().ArbitratorChairmanCitizenId;
                                                nextLitigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                                                await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(nextLitigationRequestArbitratorAssigningRequest);
                                                chairmanArbitratorsList.FirstOrDefault().IsSent = true;

                                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                                {
                                                    Id = Guid.NewGuid(),
                                                    CreatedOn = DateTime.Now,
                                                    RequestId = litigationRequestArbitratorAssigningRequest.LitigationRequestId,
                                                    ShowForApplicant = false,
                                                    LogMessage = "تم إرسال طلب مهمة للمحكم " + chairmanArbitratorsList.FirstOrDefault()?.ArbitratorChairmanCitizen.FullName + " كرئيس هيئة التحكيم"
                                                });
                                                // Send Notification for old arbitrator and arbiration department admin
                                                // new arbitrator = arbitratorsList.FirstOrDefault().ArbitratorCitizenId
                                                // Mail - Sms - Application Notification
                                                // تم إرسال طلب مهمة تحكيم جديدة كرئيس هيئة على الدعوى رقم ..... 
                                                try
                                                {
                                                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ArbitratorChairmanTaskRequest_ReAssign).Result.Data?.Value;
                                                    msg = msg.Replace("{LitigationCode}", litigationRequestArbitratorAssigningRequest.LitigationRequest.RequestCode);

                                                    await SendNotificationMS(msg, litigationRequestArbitratorAssigningRequest.LitigationRequest, "تم إرسال طلب مهمة تحكيم جديدة كرئيس هيئة التحكيم", NotificationActorType.Employee, RequestType.ArbitratorTaskrequests
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
                                        break;
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


        private async Task SendNotificationMS(string msg, LitigationRequest litigationRequest, string subject, NotificationActorType notificationActorType, RequestType requestType, LitigationRequestArbitratorList? arbitrator = null, bool notifyApplicant = false, bool notifyArbitrators = false, bool notifyDeptAdmin = false, bool notifyFirstParty = false, bool notifySecondParty = false)
        {
            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
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
            if (notifyFirstParty && litigationRequest.Contract?.FirstPartyFacility != null && litigationRequest.Contract.FirstPartyFacility.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
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
            if (notifySecondParty && litigationRequest.Contract?.SecondPartyFacility != null)
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
