using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using REAC.Application.Contracts.Dtos.Zoom;
using System.Globalization;

namespace REAC.Application.ModuleServices
{
    public partial class ArbitrationService
    {
        #region Session
        public async Task<BaseResponse> AddArbitrationSession(ScheduleSessionProcessRequestPayload requestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);
                //CultureInfo saCulture = new CultureInfo("ar-SA");
                //TimeZoneInfo ksaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                //DateTime ksaDateTime = TimeZoneInfo.ConvertTime(requestPayload.SessionDateTime != null ? DateTime.Parse(requestPayload.SessionDateTime.ToString()) : DateTime.Now, ksaTimeZone);
                // requestPayload.SessionDateTime = ksaDateTime;// requestPayload.SessionDateTimeStr != null ? DateTime.Parse(requestPayload.SessionDateTimeStr, saCulture) :DateTime.Now;
                requestPayload.SessionDateTime = requestPayload.SessionDateTime.ToLocalTime();

                if (requestPayload.SessionTypeId == LitigationSessionType.VerdictPronouncement)
                    return await AddVerdictPronouncementSession(requestPayload, cancellationToken);
                if (requestPayload.SessionTypeId == LitigationSessionType.DeliberationBetweenArbitrators)
                    return await AddDeliberationSession(requestPayload, cancellationToken);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration &&
                        x.SessionTypeId == LitigationSessionType.ArbitrationSession))
                        .Include(x => x.ArbitratorCitizen)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.LitigationRequestArbitratorLists)
                          .ThenInclude(x => x.ArbitratorCitizen)
                          .Include(x => x.LitigationRequestExtendPeriodRequests)
                          .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                        .FirstOrDefault(x => x.Id == requestPayload.RequestId);

                if (litigationRequest.LitigationRequestArbitratorLists.Count >= 3)
                {
                    bool isChairman = litigationRequest.LitigationRequestArbitratorAssigningRequests
                        .Any(x => x.ArbitratorAssigningRequestTypeId == ArbitratorAssigningRequestType.ArbitratorsChairman
                            && x.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve
                            && x.CitizenId == referenceId);
                    if (!isChairman)
                        return new BaseResponse() { IsDone = false, Message = "لا يمكن إضافة جلسة إلا لرئيس هيئة التحكيم" };
                }

                if (requestPayload.SessionDateTime < litigationRequest.SentOn)
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة جلسة قبل تاريخ تقديم طلب الدعوى"
                    };
                }

                if (requestPayload.SessionDateTime.Date <= DateTime.Now)
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة جلسة بتاريخ قديم"
                    };
                }
                var arbitirationPeriod = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Period).Result.Data?.Value;
                if (litigationRequest.StartArbitrationDate != null && arbitirationPeriod != null)
                {
                    int ExtendDays = litigationRequest.LitigationRequestExtendPeriodRequests.LastOrDefault()?.RequiredDays??0;
                    int daysRemaining = (int.Parse(arbitirationPeriod)+ ExtendDays) - (DateTime.Now - litigationRequest.StartArbitrationDate).Value.Days;
                    int daysDiff = (requestPayload.SessionDateTime - DateTime.Now).Days;
                    if (daysDiff >= daysRemaining)
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "موعد جلسة التحكيم يجب الا تتخطى مدة مرحلة التحكيم"
                        };
                    }
                }

                // Conflict check against all parties and arbitrators on the same day (across arbitration and reconciliation)
                var allSessionsOnDay = (await _repository.GetQueryable<LitigationRequestSession, Guid>())?
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract)
                    .Include(x => x.LitigationRequest).ThenInclude(x => x.LitigationRequestArbitratorLists)
                    .Where(x => x.SessionDatetime.Date == requestPayload.SessionDateTime.Date
                        && x.LitigationRequestId != requestPayload.RequestId)
                    .ToList() ?? new List<LitigationRequestSession>();

                foreach (var existingSession in allSessionsOnDay)
                {
                    DateTime sessionEnd = existingSession.SessionDatetime.AddHours(1);
                    DateTime sessionStart = existingSession.SessionDatetime.AddHours(-1);
                    bool timeConflicts = requestPayload.SessionDateTime > sessionStart && requestPayload.SessionDateTime <= sessionEnd;
                    if (!timeConflicts) continue;

                    var existingReq = existingSession.LitigationRequest;

                    // Check conflict with arbitrators
                    bool arbitratorConflict = existingReq.LitigationRequestArbitratorLists
                        .Any(a => litigationRequest.LitigationRequestArbitratorLists.Any(b => b.ArbitratorCitizenId == a.ArbitratorCitizenId));
                    if (arbitratorConflict || existingReq.ArbitratorCitizenId == litigationRequest.ArbitratorCitizenId)
                        return new BaseResponse { IsDone = false, Message = "هذا الموعد يتعارض مع جلسة أخرى لأحد أعضاء هيئة التحكيم، برجاء اختيار موعد آخر" };

                    // Check conflict with case parties
                    var existingFirstPartyCitizenId = existingReq.Contract?.FirstPartyCitizenId;
                    var existingSecondPartyCitizenId = existingReq.Contract?.SecondPartyCitizenId;
                    var existingFirstPartyFacilityId = existingReq.Contract?.FirstPartyFacilityId;
                    var existingSecondPartyFacilityId = existingReq.Contract?.SecondPartyFacilityId;

                    bool partyConflict =
                        (existingFirstPartyCitizenId != null && (existingFirstPartyCitizenId == litigationRequest.Contract?.FirstPartyCitizenId || existingFirstPartyCitizenId == litigationRequest.Contract?.SecondPartyCitizenId)) ||
                        (existingSecondPartyCitizenId != null && (existingSecondPartyCitizenId == litigationRequest.Contract?.FirstPartyCitizenId || existingSecondPartyCitizenId == litigationRequest.Contract?.SecondPartyCitizenId)) ||
                        (existingFirstPartyFacilityId != null && existingFirstPartyFacilityId == litigationRequest.Contract?.FirstPartyFacilityId) ||
                        (existingSecondPartyFacilityId != null && existingSecondPartyFacilityId == litigationRequest.Contract?.SecondPartyFacilityId);
                    if (partyConflict)
                        return new BaseResponse { IsDone = false, Message = "هذا الموعد يتعارض مع جلسة أخرى لأحد أطراف الدعوى، برجاء اختيار موعد آخر" };
                }

                var updateResponse = new BaseResponse();
                bool updatedSession = false;
                if (litigationRequest.LitigationRequestSessions != null)
                {
                    var lastPastSession = litigationRequest.LitigationRequestSessions
                        .Where(x => x.SessionDatetime <= DateTime.Now)
                        .OrderByDescending(x => x.SessionDatetime)
                        .FirstOrDefault();
                    if (lastPastSession != null && requestPayload.SessionDateTime <= lastPastSession.SessionDatetime)
                        return new BaseResponse { IsDone = false, Message = "لا يمكن إضافة جلسة بتاريخ سابق لجلسة منتهية" };

                    foreach (var session in litigationRequest.LitigationRequestSessions.ToList())
                    {
                        if (requestPayload.SessionDateTime == session.SessionDatetime)
                            return new BaseResponse()
                            {
                                IsDone = false,
                                Message = "يوجد جلسة بنفس التاريخ"
                            };
                        if (requestPayload.SessionDateTime > session.SessionDatetime || requestPayload.SessionDateTime < session.SessionDatetime)
                        {

                            session.SessionDatetime = requestPayload.SessionDateTime;
                            // First, get all related modification requests
                            var relatedModificationRequests = (await _repository.GetQueryable<LititgationRequestModificationRequest, Guid>())?

                                .Where(x => x.LitigationRequestSessionId == session.Id)
                                .ToList();

                            // Delete related modification requests first
                            foreach (var modRequest in relatedModificationRequests)
                            {
                                _repository.Remove<LititgationRequestModificationRequest, Guid>(modRequest);
                            }

                            updateResponse = _repository.Remove<LitigationRequestSession, Guid>(session);
                            await _repository.SaveChangesAsync();
                            updatedSession = true;
                            if (!updateResponse.IsDone)
                            {
                                return new BaseResponse { IsDone = false, Message = "لا يمكن تعديل موعد الجلسة" };
                            }
                            break;
                        }

                        //if (requestPayload.SessionDateTime < session.SessionDatetime)
                        //    return new BaseResponse()
                        //    {
                        //        IsDone = false,
                        //        Message = "لا يمكن اضافة جلسة بتاريخ سابق لجلسة سابقة"
                        //    };

                        if (session.SessionDatetime > DateTime.Now)
                            return new BaseResponse()
                            {
                                IsDone = false,
                                Message = "يوجد جلسة مضافة فى وقت لاحق عن التاريخ الحالى"
                            };
                    }
                }

                bool allowAdd = true;

                if (litigationRequest.IsFinished.HasValue)
                {
                    if (litigationRequest.IsFinished.Value)
                        allowAdd = false;
                }

                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                    allowAdd = false;

                //if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitrationSession)
                //    allowAdd = false;


                int sessionCountValidation = 3;
                try
                {
                    sessionCountValidation = int.Parse((await _settingService.GetBySettingKey(SysSettingKeyConstants.Litigation_Request_Max_Session_ReScheduleTime)).Data?.Value);
                }
                catch { }
                if (litigationRequest.LitigationRequestSessions != null)
                {
                    if (litigationRequest.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) >= sessionCountValidation)
                        allowAdd = false;
                    else
                    {
                        if (litigationRequest.LitigationRequestSessions.Count == 0 && updatedSession == false)
                        {
                            litigationRequest.InternalStatusId = RequestStatus.AsessionTimeHasBeenSet;
                            litigationRequest.ExternalStatusId = RequestStatus.AsessionTimeHasBeenSet;
                        }
                        else
                        {
                            litigationRequest.InternalStatusId = RequestStatus.AsessionTimeHasBeenReset;
                            litigationRequest.ExternalStatusId = RequestStatus.AsessionTimeHasBeenReset;
                        }
                    }
                }

                int arbitratorMaxSessionsCountPerDay = 0;
                int.TryParse(_settingService.GetBySettingKey(SysSettingKeyConstants.SLA_Arbitration_ArbitratorMaxSessionsCountPerDay).Result.Data?.Value, out arbitratorMaxSessionsCountPerDay);
                if (arbitratorMaxSessionsCountPerDay > 0)
                {
                    var litigationRequestSession = (await _repository.GetQueryable<LitigationRequestSession, Guid>())?
                     .Where(x => x.LitigationRequest.ArbitratorCitizenId == referenceId & x.SessionDatetime.Date == requestPayload.SessionDateTime.Date & x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation).Count();

                    if (litigationRequestSession >= arbitratorMaxSessionsCountPerDay)
                    {
                        return new BaseResponse() { IsDone = false, Message = "يوجد " + litigationRequestSession.ToString() + " جلسة خلال اليوم ولا يمكن إضافة المزيد من الجلسات" };

                    }
                }

                if (allowAdd)
                {
                    CultureInfo provider = new CultureInfo("en-US");
                    CultureInfo.CurrentCulture = provider;
                    var sessionDateGreg = DateTime.ParseExact(requestPayload.SessionDateTime.ToString(), "M/d/yyyy h:mm:ss tt", provider);
                    var _zoomMeeting = await _zoomService.GenerateMeeting($"{litigationRequest.RequestCode}_{sessionDateGreg.ToString()}", sessionDateGreg);
                    var sessionUrl = _zoomMeeting?.join_url;
                    LitigationRequestSession litigationRequestSession = new LitigationRequestSession()
                    {
                        Id = Guid.NewGuid(),
                        LitigationRequestId = requestPayload.RequestId,
                        SessionDatetime = requestPayload.SessionDateTime,
                        CreatedOn = DateTime.Now,
                        LitigationRequestSessionRequestTypeId = LitigationRequestSessionRequestType.Arbitration,
                        SessionUrl = sessionUrl,
                        MeetingId = _zoomMeeting?.id,
                        SessionTypeId = requestPayload.SessionTypeId,
                    };

                    if (litigationRequest.LitigationRequestSessions.Count == 0 && updatedSession == false)
                    {
                        litigationRequest.InternalStatusId = RequestStatus.AsessionTimeHasBeenSet;
                        litigationRequest.ExternalStatusId = RequestStatus.AsessionTimeHasBeenSet;
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم اضافة جلسة التحكيم " + requestPayload.SessionDateTime.ToString()
                        });
                    }
                    else
                    {
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إعادة تعيين جلسة التحكيم " + requestPayload.SessionDateTime.ToString()
                        });
                        litigationRequest.InternalStatusId = RequestStatus.AsessionTimeHasBeenReset;
                        litigationRequest.ExternalStatusId = RequestStatus.AsessionTimeHasBeenReset;
                    }


                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.JudgmentDecisionIssuing;
                    await _repository.AddAsync<LitigationRequestSession, Guid>(litigationRequestSession);

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;

                    // Send Notification
                    // تم إضافة جلسة التحكيم

                    try
                    {
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        if (arbitrationAdmin.IsDone)
                        {
                            var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationSession_Added).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            //msg = msg?.Replace("{SessionLink}", sessionUrl);
                            msg = string.Concat(msg, "بالموعد", requestPayload.SessionDateTime.ToString());
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationSession_Added).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                //body = body?.Replace("{SessionLink}", sessionUrl);
                            }


                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                        //          new SysNotificationDto{
                        //ActorId = litigationRequest.ArbitratorCitizen.Id,
                        //RequestId=litigationRequest.Id,
                        //CreatedOn= DateTime.Now,
                        //RequestTypeId=RequestType.LitigationRequest,
                        //ActorTypeId=NotificationActorType.Arbitrator,
                        //Message=msg
                        //},
                        new SysNotificationDto{
                        ActorId = litigationRequest.ApplicantCitizen.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Plaintiff,
                        Message=msg
                        },
                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        }

                        };
                            List<string> emails = new List<string>() {
                                litigationRequest.ApplicantCitizen.Email,
                                  arbitrationAdmin.Data?.Email
                                };

                            List<string> recipients = new List<string>()
                            {
                                litigationRequest.ApplicantCitizen.Mobile,
                                  arbitrationAdmin.Data?.Mobile
                            };
                            if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
                            {
                                notifications.Add(new SysNotificationDto
                                {
                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                                    RequestId = litigationRequest.Id,
                                    CreatedOn = DateTime.Now,
                                    RequestTypeId = RequestType.ArbitrationSession,
                                    ActorTypeId = NotificationActorType.Plaintiff,
                                    Message = msg
                                });
                                emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                                recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);


                            }
                            ;
                            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
                            {
                                notifications.Add(new SysNotificationDto
                                {
                                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                                    RequestId = litigationRequest.Id,
                                    CreatedOn = DateTime.Now,
                                    RequestTypeId = RequestType.ArbitrationSession,
                                    ActorTypeId = NotificationActorType.Plaintiff,
                                    Message = msg
                                });
                                emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                                recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
                            }
                            ;
                            if (litigationRequest?.Contract?.SecondPartyCitizenId != null)
                            {
                                notifications.Add(new SysNotificationDto
                                {
                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyCitizenId,
                                    RequestId = litigationRequest.Id,
                                    CreatedOn = DateTime.Now,
                                    RequestTypeId = RequestType.ArbitrationSession,
                                    ActorTypeId = NotificationActorType.Defendant,
                                    Message = msg
                                });
                                emails.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                                recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                            }
                            ;
                            if (litigationRequest?.Contract?.SecondPartyFacilityId != null)
                            {
                                notifications.Add(new SysNotificationDto
                                {
                                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.CitizenId,
                                    RequestId = litigationRequest.Id,
                                    CreatedOn = DateTime.Now,
                                    RequestTypeId = RequestType.ArbitrationSession,
                                    ActorTypeId = NotificationActorType.Defendant,
                                    Message = msg
                                });
                                emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Email);
                                recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
                            }
                            ;

                            foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
                            {
                                notifications.Add(new SysNotificationDto
                                {
                                    ActorId = (Guid)arbitrator.ArbitratorCitizenId,
                                    RequestId = litigationRequest.Id,
                                    CreatedOn = DateTime.Now,
                                    RequestTypeId = RequestType.ArbitrationSession,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    Message = msg
                                });
                                emails.Add(arbitrator.ArbitratorCitizen.Email);
                                recipients.Add(arbitrator.ArbitratorCitizen.Mobile);
                            }

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = " جلسة التحكيم",
                                Body = body
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, notifications);
                        }
                    }
                    catch (Exception e)
                    {


                    }



                }
                else
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة جلسة جديدة"
                    };

                }
            }
            catch (Exception ex)
            {
                new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return await GetLitigationRequestForSession(requestPayload.RequestId, cancellationToken);

        }
        public async Task<BaseResponse> GetLitigationRequestForSession(Guid requestId, CancellationToken cancellationToken)
        {
            if (requestId == Guid.Empty)
                return new BaseResponse() { IsDone = false, Message = "Not Found" };

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);


            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.Area)
                          .Include(x => x.City)
                          .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration))
                          .Include(x => x.LitigationRequestModificationRequests)
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestReconciliationReports)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.DefendantCitizen)
                          .Include(x => x.DefendantFacility)
                          .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                          .Include(x => x.InternalStatus)
                          .Include(x => x.ExternalStatus)
                          .Include(x => x.LitigationRequestAmount)
                          .Include(x => x.LitigationRequestCategory)
                          .Include(x => x.SysExemptionReductionCategory)
                          .Include(x => x.LitigationRequestOwnerType)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.ContractFile)
                          .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                          .Include(x => x.Contract.FoundingLetterFile)
                          .Include(x => x.Contract.InternalRegulationsFile)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.Contract.LitigationRequestContractType)
                          .Include(x => x.LitigationRequestArbitratorLists).AsNoTracking()
                          .Where(x => x.Id == requestId
                          & (x.ArbitratorCitizenId == referenceId
                             || x.LitigationRequestArbitratorLists.Any(a => a.ArbitratorCitizenId == referenceId))
                          ).ToList().FirstOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            var sessions = _mapper.Map<List<LitigationRequestSessionCreateUpdateDto>>(litigationRequest?.LitigationRequestSessions);

            var modifications = _mapper.Map<List<LitigationRequestModificationRequestsDto>>(litigationRequest?.LitigationRequestModificationRequests);


            bool allowAdd = true;
            bool isDone = false;

            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    isDone = true;
                }
            }
            if (!isDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                    allowAdd = false;
                else
                {
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitrationSession)
                    {
                        if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.JudgmentDecisionIssuing)
                        {
                            allowAdd = false;
                        }
                    }
                    else
                    {
                        int sessionCountValidation = 3;
                        try
                        {
                            sessionCountValidation = int.Parse((await _settingService.GetBySettingKey(SysSettingKeyConstants.Litigation_Request_Max_Session_ReScheduleTime)).Data?.Value);
                        }
                        catch { }
                        if (litigationRequest.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) >= sessionCountValidation)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.LitigationRequestArbitrationReports != null)
                            {
                                if (litigationRequest?.LitigationRequestArbitrationReports.Count > 0)
                                {
                                    allowAdd = false;
                                }
                            }
                        }
                    }
                }
            }

            return new DataResponse<LitigationRequestForSessionDto>()
            {
                Data = new LitigationRequestForSessionDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    Sessions = (sessions.Count > 0) ? sessions : null,
                    Modifications = (modifications.Count > 0) ? modifications : null,

                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                      .Include(x => x.Area)
                                      .Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions.
                                      Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration))
                                      .Include(x => x.LitigationRequestReconciliationReports)
                                      .Include(x => x.ApplicantCitizen)
                                      .Include(x => x.DefendantCitizen)
                                      .Include(x => x.DefendantFacility)
                                      .Include(x => x.InternalStatus)
                                      .Include(x => x.ExternalStatus)
                                      .Include(x => x.LitigationRequestAmount)
                                      .Include(x => x.LitigationRequestCategory)
                                      .Include(x => x.SysExemptionReductionCategory)
                                      .Include(x => x.LitigationRequestOwnerType)
                                      .Include(x => x.Contract)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.Contract.ContractFile)
                                      .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                                      .Include(x => x.Contract.FoundingLetterFile)
                                      .Include(x => x.Contract.InternalRegulationsFile)
                                      .Include(x => x.Contract.FirstPartyFacility)
                                      .Include(x => x.Contract.FirstPartyCitizen)
                                      .Include(x => x.Contract.SecondPartyFacility)
                                      .Include(x => x.Contract.SecondPartyCitizen)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.LitigationRequestArbitratorLists).AsNoTracking();



            litigationRequests = entitiesQuerable.Where(x => (x.ArbitratorCitizenId == referenceId
                                         || x.LitigationRequestArbitratorLists.Any(a => a.ArbitratorCitizenId == referenceId))
                                      & x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);




            List<LitigationRequestMircoDto> litigationRequestMircoDtos = new List<LitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    bool allowAdd = true;
                    bool isDone = false;
                    if (litigationRequest.IsFinished.HasValue)
                    {
                        if (litigationRequest.IsFinished.Value)
                        {
                            allowAdd = false;
                            isDone = true;
                        }
                    }
                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitrationSession)
                                allowAdd = false;
                            else
                            {
                                int sessionCountValidation = 3;
                                try
                                {
                                    sessionCountValidation = int.Parse((await _settingService.GetBySettingKey(SysSettingKeyConstants.Litigation_Request_Max_Session_ReScheduleTime)).Data?.Value);
                                }
                                catch { }
                                if (litigationRequest.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) >= sessionCountValidation)
                                    allowAdd = false;
                                else
                                {
                                    //if (litigationRequest?.LitigationRequestReconciliationReports != null)
                                    //{
                                    //    if (litigationRequest?.LitigationRequestReconciliationReports.Count > 0)
                                    //    {
                                    //        allowAdd = false;
                                    //    }
                                    //}
                                }
                            }
                        }
                    }
                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);

                }
            }
            return new DataResponse<PagedResult<LitigationRequestMircoDto>>
            {
                Data =
                   new PagedResult<LitigationRequestMircoDto>
                   {
                       Results = litigationRequestMircoDtos,
                       CurrentPage = litigationRequests.CurrentPage,
                       PageCount = litigationRequests.PageCount,
                       PageSize = litigationRequests.PageSize,
                       TotalCount = litigationRequests.TotalCount
                   },
                IsDone = true
            };
        }

        //عرض جلسات لطرفى الدعوى
        public async Task<BaseResponse> GetLitigationRequestForAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                         .Include(x => x.Area)
                                      .Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration && x.SessionDatetime.Date >= DateTime.Today.Date))
                                      .Include(x => x.LitigationRequestReconciliationReports)
                                      .Include(x => x.ApplicantCitizen)
                                      .Include(x => x.DefendantCitizen)
                                      .Include(x => x.DefendantFacility)
                                      .Include(x => x.InternalStatus)
                                      .Include(x => x.ExternalStatus)
                                      .Include(x => x.LitigationRequestAmount)
                                      .Include(x => x.LitigationRequestCategory)
                                      .Include(x => x.SysExemptionReductionCategory)
                                      .Include(x => x.LitigationRequestOwnerType)
                                      .Include(x => x.Contract)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.Contract.ContractFile)
                                      .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                                      .Include(x => x.Contract.FoundingLetterFile)
                                      .Include(x => x.Contract.InternalRegulationsFile)
                                      .Include(x => x.Contract.FirstPartyFacility)
                                      .Include(x => x.Contract.FirstPartyCitizen)
                                      .Include(x => x.Contract.SecondPartyFacility)
                                      .Include(x => x.Contract.SecondPartyCitizen)
                                      .Include(x => x.Contract.LitigationRequestContractType);
            litigationRequests = entitiesQuerable
                .Where(x =>


                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) > 0
                & x.ApplicantCitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

                //|
                //x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                //& x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) > 0 & x.RequestCode != null
                //& x.Contract.FirstPartyCitizenId == referenceId
                //& x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

                |
                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) > 0
                & x.RequestCode != null
                & x.Contract.SecondPartyCitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation


                |

                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) > 0
                & x.RequestCode != null
                & x.Contract.FirstPartyFacility.CitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation


                |

                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) > 0
                & x.RequestCode != null
                & x.Contract.SecondPartyFacility.CitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation)
                                        .OrderByDescending(x => x.CreatedOn)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);


            List<LitigationRequestMircoForRequestSessionDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestSessionDto>();
            try
            {
                if (litigationRequests != null)
                {
                    foreach (var litigationRequest in litigationRequests.Results)
                    {
                        foreach (var session in litigationRequest.LitigationRequestSessions.Where(x => x.SessionTypeId != LitigationSessionType.DeliberationBetweenArbitrators))
                        {
                            LitigationRequestMircoForRequestSessionDto litigationRequestMircoDto = new LitigationRequestMircoForRequestSessionDto();
                            litigationRequestMircoDto.Id = litigationRequest.Id;
                            litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                            litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                            litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                            litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility.ArabicFacilityName : litigationRequest.DefendantCitizen.FullName;
                            litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                            litigationRequestMircoDto.SessionDatetime = session.SessionDatetime;
                            litigationRequestMircoDto.Url = session.SessionUrl;
                            litigationRequestMircoDto.SessionType = session.SessionTypeId switch
                            {
                                LitigationSessionType.ArbitrationSession => "جلسة تحكيم",
                                LitigationSessionType.VerdictPronouncement => "جلسة النطق بالحكم",
                                LitigationSessionType.DeliberationBetweenArbitrators => "جلسة مداولة بين المحكمين",
                                null => null,
                                _ => null
                            };
                            litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return new DataResponse<PagedResult<LitigationRequestMircoForRequestSessionDto>>
            {
                Data =
                   new PagedResult<LitigationRequestMircoForRequestSessionDto>
                   {
                       Results = litigationRequestMircoDtos,
                       CurrentPage = litigationRequests.CurrentPage,
                       PageCount = litigationRequests.PageCount,
                       PageSize = litigationRequests.PageSize,
                       TotalCount = litigationRequests.TotalCount
                   },
                IsDone = true
            };
        }
        // عرض جلسات للمحكم
        public async Task<BaseResponse> GetLitigationRequestForAllSessionForArbitrator(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);
                PagedResult<LitigationRequest> litigationRequests = null;
                var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                             .Include(x => x.Area)
                                          .Include(x => x.City)
                                          .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration))
                                          .Include(x => x.LitigationRequestReconciliationReports)
                                          .Include(x => x.ApplicantCitizen)
                                          .Include(x => x.DefendantCitizen)
                                          .Include(x => x.DefendantFacility)
                                          .Include(x => x.InternalStatus)
                                          .Include(x => x.ExternalStatus)
                                          .Include(x => x.LitigationRequestAmount)
                                          .Include(x => x.LitigationRequestCategory)
                                          .Include(x => x.SysExemptionReductionCategory)
                                          .Include(x => x.LitigationRequestOwnerType)
                                          .Include(x => x.Contract)
                                          .Include(x => x.Contract.LitigationRequestContractType)
                                          .Include(x => x.Contract.ContractFile)
                                          .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                                          .Include(x => x.Contract.FoundingLetterFile)
                                          .Include(x => x.Contract.InternalRegulationsFile)
                                          .Include(x => x.Contract.FirstPartyFacility)
                                          .Include(x => x.Contract.FirstPartyCitizen)
                                          .Include(x => x.Contract.SecondPartyFacility)
                                          .Include(x => x.Contract.SecondPartyCitizen)
                                          .Include(x => x.Contract.LitigationRequestContractType)
                                          .Where(x => x.LitigationRequestSessions.Any(y => y.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration));

                litigationRequests = entitiesQuerable
                    .Where(x => x.LitigationRequestArbitratorLists.Count(x => x.ArbitratorCitizenId == referenceId) > 0
                    & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                     & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                    |
                    x.ArbitratorCitizenId == referenceId
                    & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                     & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                    )
                                            .OrderByDescending(x => x.LitigationRequestSessions.OrderBy(x => x.CreatedOn)
                                            .LastOrDefault().SessionDatetime)
                                            .AsSplitQuery()
                                            .AsNoTracking()
                                            .GetPaged(pageIndex, pageSize);


                List<LitigationRequestMircoForRequestSessionDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestSessionDto>();
                if (litigationRequests != null)
                {
                    foreach (var litigationRequest in litigationRequests.Results)
                        foreach (var litigationRequestSessions in litigationRequest.LitigationRequestSessions)
                        {
                            LitigationRequestMircoForRequestSessionDto litigationRequestMircoDto = new LitigationRequestMircoForRequestSessionDto();
                            litigationRequestMircoDto.Id = litigationRequest.Id;
                            litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                            litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                            litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                            litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
                            litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                            litigationRequestMircoDto.SessionDatetime = litigationRequestSessions.SessionDatetime;
                            litigationRequestMircoDto.Url = litigationRequestSessions.SessionUrl;
                            litigationRequestMircoDto.SessionType = litigationRequestSessions.SessionTypeId switch
                            {
                                LitigationSessionType.ArbitrationSession => "جلسة تحكيم",
                                LitigationSessionType.VerdictPronouncement => "جلسة النطق بالحكم",
                                LitigationSessionType.DeliberationBetweenArbitrators => "جلسة مداولة بين المحكمين",
                                null => null,
                                _ => null
                            };

                            litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                        }
                }

                return new DataResponse<PagedResult<LitigationRequestMircoForRequestSessionDto>>
                {
                    Data =
                       new PagedResult<LitigationRequestMircoForRequestSessionDto>
                       {
                           Results = litigationRequestMircoDtos,
                           CurrentPage = litigationRequests.CurrentPage,
                           PageCount = litigationRequests.PageCount,
                           PageSize = litigationRequests.PageSize,
                           TotalCount = litigationRequests.TotalCount
                       },
                    IsDone = true
                };
        }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
    }


}
// عرض كل الجلسات 
public async Task<BaseResponse> GetLitigationRequestForFullAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
{
    Guid referenceId = Guid.Empty;
    _jwtReader.GetReferenceId(ref referenceId);
    PagedResult<LitigationRequest> litigationRequests = null;
    var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                 .Include(x => x.Area)
                              .Include(x => x.City)
                              .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration))
                              .Include(x => x.LitigationRequestReconciliationReports)
                              .Include(x => x.ApplicantCitizen)
                              .Include(x => x.DefendantCitizen)
                              .Include(x => x.DefendantFacility)
                              .Include(x => x.InternalStatus)
                              .Include(x => x.ExternalStatus)
                              .Include(x => x.LitigationRequestAmount)
                              .Include(x => x.LitigationRequestCategory)
                              .Include(x => x.SysExemptionReductionCategory)
                              .Include(x => x.LitigationRequestOwnerType)
                              .Include(x => x.Contract)
                              .Include(x => x.Contract.LitigationRequestContractType)
                              .Include(x => x.Contract.ContractFile)
                              .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                              .Include(x => x.Contract.FoundingLetterFile)
                              .Include(x => x.Contract.InternalRegulationsFile)
                              .Include(x => x.Contract.FirstPartyFacility)
                              .Include(x => x.Contract.FirstPartyCitizen)
                              .Include(x => x.Contract.SecondPartyFacility)
                              .Include(x => x.Contract.SecondPartyCitizen)
                              .Include(x => x.Contract.LitigationRequestContractType)
                              .Where(x => x.LitigationRequestSessions.Any(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration));

    litigationRequests = entitiesQuerable
        .Where(x => x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation & x.InternalStatusId != RequestStatus.Rejected
         & x.LitigationRequestStage == LitigationRequestStage.Arbitration)

                                .OrderByDescending(x => x.LitigationRequestSessions.OrderBy(x => x.CreatedOn)
                                .LastOrDefault().SessionDatetime)
                                .AsSplitQuery()
                                .AsNoTracking()
                                .GetPaged(pageIndex, pageSize);


    List<LitigationRequestMircoForRequestSessionDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestSessionDto>();
    if (litigationRequests != null)
    {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    foreach (var session in litigationRequest.LitigationRequestSessions)
                    {
                        LitigationRequestMircoForRequestSessionDto litigationRequestMircoDto = new LitigationRequestMircoForRequestSessionDto();
                        litigationRequestMircoDto.Id = litigationRequest.Id;
                        litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                        litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                        litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                        litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility.ArabicFacilityName : litigationRequest.DefendantCitizen.FullName;
                        litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                        litigationRequestMircoDto.SessionDatetime = session.SessionDatetime;
                        litigationRequestMircoDto.Url = session.SessionUrl;
                        litigationRequestMircoDto.SessionType = session.SessionTypeId switch
                        {
                            LitigationSessionType.ArbitrationSession => "جلسة تحكيم",
                            LitigationSessionType.VerdictPronouncement => "جلسة النطق بالحكم",
                            LitigationSessionType.DeliberationBetweenArbitrators => "جلسة مداولة بين المحكمين",
                            null => null,
                            _ => null
                        };
                        litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                    }
                }
            }

    return new DataResponse<PagedResult<LitigationRequestMircoForRequestSessionDto>>
    {
        Data =
           new PagedResult<LitigationRequestMircoForRequestSessionDto>
           {
               Results = litigationRequestMircoDtos,
               CurrentPage = litigationRequests.CurrentPage,
               PageCount = litigationRequests.PageCount,
               PageSize = litigationRequests.PageSize,
               TotalCount = litigationRequests.TotalCount
           },
        IsDone = true
    };

}

private async Task<BaseResponse> AddVerdictPronouncementSession(ScheduleSessionProcessRequestPayload requestPayload, CancellationToken cancellationToken)
{
    try
    {
        Guid referenceId = Guid.Empty;
        _jwtReader.GetReferenceId(ref referenceId);

        var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
            .Include(x => x.LitigationRequestSessions.Where(x =>
                x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration &&
                x.SessionTypeId == LitigationSessionType.VerdictPronouncement))
            .Include(x => x.ArbitratorCitizen)
            .Include(x => x.ApplicantCitizen)
            .Include(x => x.Contract)
            .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
            .Include(x => x.Contract.FirstPartyCitizen)
            .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
            .Include(x => x.Contract.SecondPartyCitizen)
            .Include(x => x.LitigationRequestArbitratorLists).ThenInclude(x => x.ArbitratorCitizen)
            .Include(x => x.LitigationRequestArbitratorAssigningRequests)
            .Include(x => x.LitigationRequestExtendPeriodRequests)
            .FirstOrDefault(x => x.Id == requestPayload.RequestId);

        if (litigationRequest == null)
            return new BaseResponse { IsDone = false, Message = "طلب غير موجود" };

        if (litigationRequest.LitigationRequestArbitratorLists.Count >= 3)
        {
            bool isChairman = litigationRequest.LitigationRequestArbitratorAssigningRequests
                .Any(x => x.ArbitratorAssigningRequestTypeId == ArbitratorAssigningRequestType.ArbitratorsChairman
                    && x.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve
                    && x.CitizenId == referenceId);
            if (!isChairman)
                return new BaseResponse() { IsDone = false, Message = "لا يمكن إضافة جلسة إلا لرئيس هيئة التحكيم" };
        }

        if (litigationRequest.LitigationRequestStage != LitigationRequestStage.Arbitration)
            return new BaseResponse { IsDone = false, Message = "الدعوى ليست في مرحلة التحكيم" };

        if (litigationRequest.IsFinished.HasValue && litigationRequest.IsFinished.Value)
            return new BaseResponse { IsDone = false, Message = "لا يمكن إضافة جلسة على دعوى مكتملة" };

        if (requestPayload.SessionDateTime < litigationRequest.SentOn)
            return new BaseResponse { IsDone = false, Message = "لا يمكن إضافة جلسة قبل تاريخ تقديم طلب الدعوى" };

        if (requestPayload.SessionDateTime.Date <= DateTime.Now)
            return new BaseResponse { IsDone = false, Message = "لا يمكن إضافة جلسة بتاريخ قديم" };

        var arbitrationPeriod = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Period).Result.Data?.Value;
        if (litigationRequest.StartArbitrationDate != null && arbitrationPeriod != null)
        {
            int extendDays = litigationRequest.LitigationRequestExtendPeriodRequests?.LastOrDefault()?.RequiredDays ?? 0;
            int daysRemaining = (int.Parse(arbitrationPeriod) + extendDays) - (DateTime.Now - litigationRequest.StartArbitrationDate).Value.Days;
            int daysDiff = (requestPayload.SessionDateTime - DateTime.Now).Days;
            if (daysDiff >= daysRemaining)
                return new BaseResponse { IsDone = false, Message = "موعد جلسة النطق بالحكم يجب ألا تتخطى مدة مرحلة التحكيم" };
        }

        bool isRescheduling = false;
        var existingVerdictSession = litigationRequest.LitigationRequestSessions
            .FirstOrDefault(x => x.SessionTypeId == LitigationSessionType.VerdictPronouncement);

        if (existingVerdictSession != null)
        {
            if (existingVerdictSession.SessionDatetime <= DateTime.Now)
                return new BaseResponse { IsDone = false, Message = "لا يمكن إنشاء جلسة نطق حكم جديدة، الجلسة السابقة مكتملة" };

            if (existingVerdictSession.SessionDatetime == requestPayload.SessionDateTime)
                return new BaseResponse { IsDone = false, Message = "يوجد جلسة نطق حكم بنفس التاريخ" };

            var relatedModificationRequests = (await _repository.GetQueryable<LititgationRequestModificationRequest, Guid>())?
                .Where(x => x.LitigationRequestSessionId == existingVerdictSession.Id).ToList();
            foreach (var modRequest in relatedModificationRequests)
                _repository.Remove<LititgationRequestModificationRequest, Guid>(modRequest);

            _repository.Remove<LitigationRequestSession, Guid>(existingVerdictSession);
            await _repository.SaveChangesAsync();
            isRescheduling = true;
        }

        // Conflict check against all parties and arbitrators on the same day (across arbitration and reconciliation)
        var allSessionsOnDay = (await _repository.GetQueryable<LitigationRequestSession, Guid>())?
            .Include(x => x.LitigationRequest).ThenInclude(x => x.Contract)
            .Include(x => x.LitigationRequest).ThenInclude(x => x.LitigationRequestArbitratorLists)
            .Where(x => x.SessionDatetime.Date == requestPayload.SessionDateTime.Date
                && x.LitigationRequestId != requestPayload.RequestId)
            .ToList() ?? new List<LitigationRequestSession>();

        foreach (var existingSession in allSessionsOnDay)
        {
            DateTime sessionEnd = existingSession.SessionDatetime.AddHours(1);
            DateTime sessionStart = existingSession.SessionDatetime.AddHours(-1);
            bool timeConflicts = requestPayload.SessionDateTime > sessionStart && requestPayload.SessionDateTime <= sessionEnd;
            if (!timeConflicts) continue;

            var existingReq = existingSession.LitigationRequest;

            // Check conflict with arbitrators
            bool arbitratorConflict = existingReq.LitigationRequestArbitratorLists
                .Any(a => litigationRequest.LitigationRequestArbitratorLists.Any(b => b.ArbitratorCitizenId == a.ArbitratorCitizenId));
            if (arbitratorConflict || existingReq.ArbitratorCitizenId == litigationRequest.ArbitratorCitizenId)
                return new BaseResponse { IsDone = false, Message = "هذا الموعد يتعارض مع جلسة أخرى لأحد أعضاء هيئة التحكيم، برجاء اختيار موعد آخر" };

            // Check conflict with case parties
            var existingFirstPartyCitizenId = existingReq.Contract?.FirstPartyCitizenId;
            var existingSecondPartyCitizenId = existingReq.Contract?.SecondPartyCitizenId;
            var existingFirstPartyFacilityId = existingReq.Contract?.FirstPartyFacilityId;
            var existingSecondPartyFacilityId = existingReq.Contract?.SecondPartyFacilityId;

            bool partyConflict =
                (existingFirstPartyCitizenId != null && (existingFirstPartyCitizenId == litigationRequest.Contract?.FirstPartyCitizenId || existingFirstPartyCitizenId == litigationRequest.Contract?.SecondPartyCitizenId)) ||
                (existingSecondPartyCitizenId != null && (existingSecondPartyCitizenId == litigationRequest.Contract?.FirstPartyCitizenId || existingSecondPartyCitizenId == litigationRequest.Contract?.SecondPartyCitizenId)) ||
                (existingFirstPartyFacilityId != null && existingFirstPartyFacilityId == litigationRequest.Contract?.FirstPartyFacilityId) ||
                (existingSecondPartyFacilityId != null && existingSecondPartyFacilityId == litigationRequest.Contract?.SecondPartyFacilityId);
            if (partyConflict)
                return new BaseResponse { IsDone = false, Message = "هذا الموعد يتعارض مع جلسة أخرى لأحد أطراف الدعوى، برجاء اختيار موعد آخر" };
        }

        CultureInfo provider = new CultureInfo("en-US");
        CultureInfo.CurrentCulture = provider;
        var sessionDateGreg = DateTime.ParseExact(requestPayload.SessionDateTime.ToString(), "M/d/yyyy h:mm:ss tt", provider);
        var zoomMeeting = await _zoomService.GenerateMeeting($"{litigationRequest.RequestCode}_{sessionDateGreg}", sessionDateGreg);

        var newSession = new LitigationRequestSession
        {
            Id = Guid.NewGuid(),
            LitigationRequestId = requestPayload.RequestId,
            SessionDatetime = requestPayload.SessionDateTime,
            CreatedOn = DateTime.Now,
            LitigationRequestSessionRequestTypeId = LitigationRequestSessionRequestType.Arbitration,
            SessionTypeId = LitigationSessionType.VerdictPronouncement,
            SessionUrl = zoomMeeting?.join_url,
            MeetingId = zoomMeeting?.id,
        };

        await _repository.AddAsync<LitigationRequestSession, Guid>(newSession);
        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            RequestId = litigationRequest.Id,
            ShowForApplicant = false,
            LogMessage = isRescheduling
                ? "تم إعادة جدولة جلسة النطق بالحكم " + requestPayload.SessionDateTime
                : "تم تحديد موعد جلسة النطق بالحكم " + requestPayload.SessionDateTime
        });

        var saveResponse = await _repository.SaveChangesAsync(new CancellationToken());
        if (!saveResponse.IsDone) return saveResponse;

        try
        {
            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            var msg = isRescheduling
                ? $"تم إعادة جدولة جلسة النطق بالحكم للدعوى {litigationRequest.RequestCode} بالموعد {requestPayload.SessionDateTime}"
                : $"تم تحديد موعد جلسة النطق بالحكم للدعوى {litigationRequest.RequestCode} بالموعد {requestPayload.SessionDateTime}";
            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
            string body;
            using (StreamReader streamReader = File.OpenText(pathToFile))
            {
                body = await streamReader.ReadToEndAsync();
                body = body?.Replace("{content}", msg);
                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
            }

            List<SysNotificationDto> notifications = new List<SysNotificationDto>();
            List<string> emails = new List<string>();
            List<string> recipients = new List<string>();

            notifications.Add(new SysNotificationDto
            {
                ActorId = litigationRequest.ApplicantCitizen.Id,
                RequestId = litigationRequest.Id,
                CreatedOn = DateTime.Now,
                RequestTypeId = RequestType.LitigationRequest,
                ActorTypeId = NotificationActorType.Plaintiff,
                Message = msg
            });
            emails.Add(litigationRequest.ApplicantCitizen.Email);
            recipients.Add(litigationRequest.ApplicantCitizen.Mobile);

            if (arbitrationAdmin.IsDone)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)arbitrationAdmin.Data?.Id,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.LitigationRequest,
                    ActorTypeId = NotificationActorType.Employee,
                    Message = msg
                });
                emails.Add(arbitrationAdmin.Data?.Email);
                recipients.Add(arbitrationAdmin.Data?.Mobile);
            }

            if (litigationRequest?.Contract?.FirstPartyCitizenId != null && litigationRequest?.Contract?.FirstPartyCitizenId != litigationRequest.ApplicantCitizenId)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)litigationRequest.Contract.FirstPartyCitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.ArbitrationSession,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    Message = msg
                });
                emails.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);
            }

            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility.CitizenId != litigationRequest.ApplicantCitizenId)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.CitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.ArbitrationSession,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    Message = msg
                });
                emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Email);
                recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
            }

            if (litigationRequest?.Contract?.SecondPartyCitizenId != null)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)litigationRequest.Contract.SecondPartyCitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.ArbitrationSession,
                    ActorTypeId = NotificationActorType.Defendant,
                    Message = msg
                });
                emails.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
            }

            if (litigationRequest?.Contract?.SecondPartyFacilityId != null)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.CitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.ArbitrationSession,
                    ActorTypeId = NotificationActorType.Defendant,
                    Message = msg
                });
                emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Email);
                recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
            }

            foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)arbitrator.ArbitratorCitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.ArbitrationSession,
                    ActorTypeId = NotificationActorType.Arbitrator,
                    Message = msg
                });
                emails.Add(arbitrator.ArbitratorCitizen.Email);
                recipients.Add(arbitrator.ArbitratorCitizen.Mobile);
            }

            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto
            {
                To = emails,
                Subject = "جلسة النطق بالحكم",
                Body = body
            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto
            {
                message_text = msg,
                recipients = recipients,
                notification_type = "SMS"
            }, notifications);
        }
        catch { }
    }
    catch (Exception ex)
    {
        new ExceptionResponse() { IsDone = false, Exception = ex.Message + "\n" + ex.ToString() };
    }
    return await GetLitigationRequestForSession(requestPayload.RequestId, cancellationToken);
}

private async Task<BaseResponse> AddDeliberationSession(ScheduleSessionProcessRequestPayload requestPayload, CancellationToken cancellationToken)
{
    try
    {
        Guid referenceId = Guid.Empty;
        _jwtReader.GetReferenceId(ref referenceId);

        var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
            .Include(x => x.LitigationRequestArbitratorLists).ThenInclude(x => x.ArbitratorCitizen)
            .Include(x => x.ArbitratorCitizen)
            .Include(x => x.ApplicantCitizen)
            .Include(x => x.Contract.FirstPartyCitizen)
            .Include(x => x.Contract.SecondPartyCitizen)
            .Include(x => x.LitigationRequestArbitratorAssigningRequests)
            .Include(x => x.LitigationRequestExtendPeriodRequests)
            .FirstOrDefault(x => x.Id == requestPayload.RequestId);

        if (litigationRequest == null)
            return new BaseResponse { IsDone = false, Message = "طلب غير موجود" };

        if (litigationRequest.LitigationRequestArbitratorLists.Count >= 3)
        {
            bool isChairman = litigationRequest.LitigationRequestArbitratorAssigningRequests
                .Any(x => x.ArbitratorAssigningRequestTypeId == ArbitratorAssigningRequestType.ArbitratorsChairman
                    && x.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve
                    && x.CitizenId == referenceId);
            if (!isChairman)
                return new BaseResponse() { IsDone = false, Message = "لا يمكن إضافة جلسة إلا لرئيس هيئة التحكيم" };
        }
        else if (litigationRequest.ArbitratorCitizenId != referenceId)
            return new BaseResponse { IsDone = false, Message = "لا يمكن للمحكم اضافة جلسة من نوع مداولة بين المحكمين" };

        if (litigationRequest.LitigationRequestStage != LitigationRequestStage.Arbitration)
            return new BaseResponse { IsDone = false, Message = "الدعوى ليست في مرحلة التحكيم" };

        if (litigationRequest.LitigationRequestArbitratorLists.Count < 2)
            return new BaseResponse { IsDone = false, Message = "لا يمكن إنشاء جلسة مداولة إلا إذا كان عدد المحكمين 3 أو أكثر" };

        if (requestPayload.SessionDateTime < litigationRequest.SentOn)
            return new BaseResponse { IsDone = false, Message = "لا يمكن إضافة جلسة قبل تاريخ تقديم طلب الدعوى" };

        if (requestPayload.SessionDateTime.Date <= DateTime.Now)
            return new BaseResponse { IsDone = false, Message = "لا يمكن إضافة جلسة بتاريخ قديم" };

        var arbitrationPeriod = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Period).Result.Data?.Value;
        if (litigationRequest.StartArbitrationDate != null && arbitrationPeriod != null)
        {
            int extendDays = litigationRequest.LitigationRequestExtendPeriodRequests?.LastOrDefault()?.RequiredDays ?? 0;
            int daysRemaining = (int.Parse(arbitrationPeriod) + extendDays) - (DateTime.Now - litigationRequest.StartArbitrationDate).Value.Days;
            int daysDiff = (requestPayload.SessionDateTime - DateTime.Now).Days;
            if (daysDiff >= daysRemaining)
                return new BaseResponse { IsDone = false, Message = "موعد جلسة المداولة يجب ألا تتخطى مدة مرحلة التحكيم" };
        }

        // Time conflict check against arbitrators' other sessions (across all session types)
        var arbitratorIds = litigationRequest.LitigationRequestArbitratorLists
            .Select(x => x.ArbitratorCitizenId).ToList();
        var conflictingSessions = (await _repository.GetQueryable<LitigationRequestSession, Guid>())?
            .Where(x => x.SessionDatetime.Date == requestPayload.SessionDateTime.Date
                && x.LitigationRequestId != requestPayload.RequestId
                && (x.LitigationRequest.LitigationRequestArbitratorLists
                    .Any(a => arbitratorIds.Contains(a.ArbitratorCitizenId))
                    || (x.LitigationRequest.ArbitratorCitizenId != null && arbitratorIds.Contains((Guid)x.LitigationRequest.ArbitratorCitizenId))))
            .ToList();

        foreach (var cs in conflictingSessions)
        {
            DateTime sessionEnd = cs.SessionDatetime.AddHours(1);
            DateTime sessionStart = cs.SessionDatetime.AddHours(-1);
            if (requestPayload.SessionDateTime > sessionStart && requestPayload.SessionDateTime <= sessionEnd)
                return new BaseResponse { IsDone = false, Message = "هذا الموعد يتعارض مع جلسة أخرى لأحد المحكمين، برجاء اختيار موعد آخر" };
        }

        CultureInfo provider = new CultureInfo("en-US");
        CultureInfo.CurrentCulture = provider;
        var sessionDateGreg = DateTime.ParseExact(requestPayload.SessionDateTime.ToString(), "M/d/yyyy h:mm:ss tt", provider);
        var zoomMeeting = await _zoomService.GenerateMeeting($"{litigationRequest.RequestCode}_{sessionDateGreg}", sessionDateGreg);

        var newSession = new LitigationRequestSession
        {
            Id = Guid.NewGuid(),
            LitigationRequestId = requestPayload.RequestId,
            SessionDatetime = requestPayload.SessionDateTime,
            CreatedOn = DateTime.Now,
            LitigationRequestSessionRequestTypeId = LitigationRequestSessionRequestType.Arbitration,
            SessionTypeId = LitigationSessionType.DeliberationBetweenArbitrators,
            SessionUrl = zoomMeeting?.join_url,
            MeetingId = zoomMeeting?.id,
        };

        await _repository.AddAsync<LitigationRequestSession, Guid>(newSession);

        // Log - NO case status change per business rules
        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog
        {
            Id = Guid.NewGuid(),
            CreatedOn = DateTime.Now,
            RequestId = litigationRequest.Id,
            ShowForApplicant = false,
            LogMessage = "تم تحديد موعد جلسة مداولة بين المحكمين " + requestPayload.SessionDateTime
        });

        var saveResponse = await _repository.SaveChangesAsync(new CancellationToken());
        if (!saveResponse.IsDone) return saveResponse;

        // Notifications - arbitrators ONLY (not parties per business rules)
        try
        {
            var msg = $"تم تحديد موعد جلسة مداولة بين المحكمين للدعوى {litigationRequest.RequestCode} بالموعد {requestPayload.SessionDateTime}";
            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
            string body;
            using (StreamReader streamReader = File.OpenText(pathToFile))
            {
                body = await streamReader.ReadToEndAsync();
                body = body?.Replace("{content}", msg);
                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
            }

            List<SysNotificationDto> notifications = new List<SysNotificationDto>();
            List<string> emails = new List<string>();
            List<string> recipients = new List<string>();

            foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
            {
                notifications.Add(new SysNotificationDto
                {
                    ActorId = (Guid)arbitrator.ArbitratorCitizenId,
                    RequestId = litigationRequest.Id,
                    CreatedOn = DateTime.Now,
                    RequestTypeId = RequestType.ArbitrationSession,
                    ActorTypeId = NotificationActorType.Arbitrator,
                    Message = msg
                });
                emails.Add(arbitrator.ArbitratorCitizen.Email);
                recipients.Add(arbitrator.ArbitratorCitizen.Mobile);
            }

            if (notifications.Any())
                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto
                {
                    To = emails,
                    Subject = "جلسة مداولة بين المحكمين",
                    Body = body
                }, new Contracts.Dtos.Integration.GEE.SMSRequestDto
                {
                    message_text = msg,
                    recipients = recipients,
                    notification_type = "SMS"
                }, notifications);
        }
        catch { }
    }
    catch (Exception ex)
    {
        new ExceptionResponse() { IsDone = false, Exception = ex.Message + "\n" + ex.ToString() };
    }
    return await GetLitigationRequestForSession(requestPayload.RequestId, cancellationToken);
}

        #endregion
    }
}
