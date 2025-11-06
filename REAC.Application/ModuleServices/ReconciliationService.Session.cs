using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Zoom;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Globalization;
using System.Linq.Expressions;

namespace REAC.Application.ModuleServices
{
    public partial class ReconciliationService
    {
        #region Session
        public async Task<BaseResponse> AddReconciliationSession(ScheduleSessionProcessRequestPayload requestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);
                //requestPayload.SessionDateTime = requestPayload.SessionDateTimeStr != null ? DateTime.Parse(requestPayload.SessionDateTimeStr) : DateTime.Now;
                //TimeZoneInfo ksaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Arab Standard Time");
                //DateTime ksaDateTime = TimeZoneInfo.ConvertTime(requestPayload.SessionDateTime != null ? DateTime.Parse(requestPayload.SessionDateTime.ToString()) : DateTime.Now, ksaTimeZone);
                //requestPayload.SessionDateTime = ksaDateTime;// requestPayload.SessionDateTimeStr != null ? DateTime.Parse(requestPayload.SessionDateTimeStr, saCulture) :DateTime.Now;
                requestPayload.SessionDateTime = requestPayload.SessionDateTime.ToLocalTime();
                var msg = "";
                var sessionUrl = "";
                var _zoomMeeting = new ZoomMeting();
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                         .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation))
                        .Include(x => x.Contract)
                        .ThenInclude(x => x.FirstPartyCitizen)
                        .Include(x => x.Contract)
                         .ThenInclude(x => x.SecondPartyCitizen)
                        .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                        .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                        .Include(x => x.ReformerCitizen)//رئيس لجنة الصبلح
                        .Include(x => x.ApplicantCitizen)
                        .Where(x => x.Id == requestPayload.RequestId)
                        .Single();

                #region Validation for reformer & citizen schadual Time sessions meeting
                // get old session requests for reformer 
                var OldLitigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                       .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation && x.SessionDatetime.Date == requestPayload.SessionDateTime.Date))
                       .Include(x => x.Contract);

                var OldlitigationRequestForReformer = OldLitigationRequests?
                       .Where(x =>x.LitigationRequestSessions.Any(x=>x.SessionDatetime.Date== requestPayload.SessionDateTime.Date))
                       .Where(x => x.ReformerCitizenId == referenceId)
                       .Where(x => x.Id != requestPayload.RequestId);

                // for each all old session for reformer validation 
                foreach (var request in OldlitigationRequestForReformer)
                {
                    foreach (var session in request.LitigationRequestSessions.ToList())
                    {

                        DateTime sessionEndTime = session.SessionDatetime.AddHours(1);
                        DateTime sessionStartTime = session.SessionDatetime.AddHours(-1);
                        // Check if the new session time falls within the period of the existing session
                        if (requestPayload.SessionDateTime > sessionStartTime && requestPayload.SessionDateTime <= sessionEndTime)
                        {
                            return new BaseResponse()
                            {
                                IsDone = false,
                                Message = "هذا الموعد يتعارض مع جلسة اخرى  برجاء اختيار موعد آخر"
                            };

                        }
                    }
                }

                //get all sessions for citizens and validate time 
                var OldlitigationRequestForCitizens = OldLitigationRequests?
                       .Where(x => x.LitigationRequestSessions.Any(x => x.SessionDatetime.Date == requestPayload.SessionDateTime.Date))
                       .Where(x=> x.Contract.FirstPartyCitizenId == litigationRequest.Contract.FirstPartyCitizenId || x.Contract.SecondPartyCitizenId == litigationRequest.Contract.SecondPartyCitizenId) 
                       .Where(x => x.Id != requestPayload.RequestId);

                //// for each all old session for citizens validation 
                foreach (var request in OldlitigationRequestForCitizens)
                {
                    foreach (var session in request.LitigationRequestSessions.ToList())
                    {

                        DateTime sessionEndTime = session.SessionDatetime.AddHours(1);
                        DateTime sessionStartTime = session.SessionDatetime.AddHours(-1);
                        // Check if the new session time falls within the period of the existing session
                        if (requestPayload.SessionDateTime > sessionStartTime && requestPayload.SessionDateTime <= sessionEndTime)
                        {
                            return new BaseResponse()
                            {
                                IsDone = false,
                                Message = "هذا الموعد يتعارض مع جلسة اخرى لأحد الأطراف برجاء اختيار موعد آخر"
                            };

                        }
                    }
                }
                #endregion

                int ReformerReminderForReconciliationSecondPeriod = 0;
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_ReformerReminderForReconciliationSecondPeriod).Result.Data?.Value, out ReformerReminderForReconciliationSecondPeriod);

                if (ReformerReminderForReconciliationSecondPeriod > 0)
                {
                    double totalReviewHours = 0;

                    totalReviewHours += (DateTime.Now - litigationRequest.StartReconciliationDate).Value.TotalHours;
                    if (totalReviewHours >= ReformerReminderForReconciliationSecondPeriod)
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = " المصلح تجاوز الفترة المسموح بها للصلح على الدعوى"
                        };
                    }
                }


                if (requestPayload.SessionDateTime < litigationRequest.SentOn)
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة جلسة قبل تاريخ تقديم طلب الدعوى"
                    };
                }
                if (litigationRequest.StartReconciliationDate.HasValue)
                {
                    DateTime newDate = litigationRequest.StartReconciliationDate.Value.AddDays((double)litigationRequest.ReconciliationPeriod);

                    if (requestPayload.SessionDateTime > newDate)
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "لا يمكن إضافة جلسة تتخطى  مدة صلح الدعوى"
                        };
                    }
                }
                var updateResponse = new BaseResponse();
                bool updatedSession = false;
                if (litigationRequest.LitigationRequestSessions != null)
                {
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

                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                    allowAdd = false;

                if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestReportAdded)
                    allowAdd = false;


                int ReformerMaxSessionsCountPerDay = 0;
               
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_Reconciliation_ReformerMaxSessionsCountPerDay).Result.Data?.Value, out ReformerMaxSessionsCountPerDay);
                if (ReformerMaxSessionsCountPerDay > 0)
                {
                    var litigationRequestSession = (await _repository.GetQueryable<LitigationRequestSession, Guid>())?
                     .Where(x => x.LitigationRequest.ReformerCitizenId == referenceId & x.SessionDatetime.Date == requestPayload.SessionDateTime.Date & x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation).Count();

                    if (litigationRequestSession >= ReformerMaxSessionsCountPerDay)
                    {
                        return new BaseResponse() { IsDone = false, Message = "يوجد " + litigationRequestSession.ToString() + " جلسة خلال اليوم ولا يمكن إضافة المزيد من الجلسات" };

                    }
                }


                int sessionCountValidation = 3;
                try
                {
                    sessionCountValidation = int.Parse((await _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Litigation_Request_Max_Session_ReScheduleTime)).Data?.Value);
                }
                catch { }
                if (litigationRequest.LitigationRequestSessions != null)
                {
                    if (litigationRequest.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) >= sessionCountValidation)
                        allowAdd = false;
                    else
                    {
                        CultureInfo provider = new CultureInfo("en-US");
                        CultureInfo.CurrentCulture = provider;
                        var sessionDateGreg = DateTime.ParseExact(requestPayload.SessionDateTime.ToString(), "M/d/yyyy h:mm:ss tt", provider);
                        _zoomMeeting = await _zoomService.GenerateMeeting($"{litigationRequest.RequestCode}_{requestPayload.SessionDateTime.ToString()}", sessionDateGreg);
                        sessionUrl = _zoomMeeting?.join_url;

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
                                LogMessage = "تم تحديد جلسة الصلح " + requestPayload.SessionDateTime.ToString()
                            });
                            // Send Notification
                            // تم تعيين جلسة الصلح
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ReconciliationSession_set).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                //msg = msg?.Replace("{SessionLink}", sessionUrl);
                                msg = msg?.Replace("{SessionDatetime}", sessionDateGreg.ToString("M/d/yyyy h:mm:ss tt", provider));
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                string body;
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ReconciliationSession_set).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                    //body = body?.Replace("{SessionLink}", sessionUrl);
                                    body = body?.Replace("{SessionDatetime}", sessionDateGreg.ToString("M/d/yyyy h:mm:ss tt", provider));
                                }
                                await SendNotificationMS(body, msg, litigationRequest);
                            }
                            catch (Exception e)
                            {

                            }

                        }
                        else
                        {
                            litigationRequest.InternalStatusId = RequestStatus.AsessionTimeHasBeenReset;
                            litigationRequest.ExternalStatusId = RequestStatus.AsessionTimeHasBeenReset;
                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إعادة تحديد جلسة الصلح" + requestPayload.SessionDateTime.ToString()
                            });

                            // Send Notification
                            // تم إعادة تعيين جلسة الصلح
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ReconciliationSession_Reset).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                //msg = msg?.Replace("{SessionLink}", sessionUrl);
                                msg = msg?.Replace("{SessionDatetime}", sessionDateGreg.ToString("M/d/yyyy h:mm:ss tt", provider));
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                string body;
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ReconciliationSession_Reset).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                    //body = body?.Replace("{SessionLink}", sessionUrl);
                                    body = body?.Replace("{SessionDatetime}", sessionDateGreg.ToString("M/d/yyyy h:mm:ss tt", provider));
                                }


                                await SendNotificationMS(body, msg, litigationRequest);
                            }
                            catch (Exception e)
                            {

                            }

                        }
                    }


                }
                if (allowAdd)
                {
                    LitigationRequestSession litigationRequestSession = new LitigationRequestSession()
                    {
                        Id = Guid.NewGuid(),
                        LitigationRequestId = requestPayload.RequestId,
                        SessionDatetime = requestPayload.SessionDateTime,
                        CreatedOn = DateTime.Now,
                        LitigationRequestSessionRequestTypeId = LitigationRequestSessionRequestType.Reconciliation,
                        SessionUrl = sessionUrl,
                        MeetingId = _zoomMeeting?.id,
                    };


                    await _repository.AddAsync<LitigationRequestSession, Guid>(litigationRequestSession);

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;


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
        private async Task SendNotificationMS(string emailMsg, string msg, LitigationRequest litigationRequest)
        {
            var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
            //ApplicantCitizen


            var objEmail = new SendEmailDto()
            {
                Body = emailMsg,
                Subject = "Litigation Request Session Added",
                To = new List<string>() { litigationRequest.ApplicantCitizen?.Email }

            };
            var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>() { litigationRequest.ApplicantCitizen.Mobile },
                notification_type = "SMS",
            };
            var lstNotification = new List<SysNotificationDto>();
            lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            {
                ActorId = litigationRequest.ApplicantCitizen.Id,
                CreatedOn = DateTime.Now,
                ActorTypeId = NotificationActorType.Plaintiff,
                IsViewed = false,
                ViewedOn = null,
                RequestId = litigationRequest.Id,
                RequestTypeId = RequestType.ReformationSession,
                Message = msg,
            });
            //المصلح اللى على الدعوة
            if (litigationRequest.ReformerCitizenId.HasValue)
            {
                objEmail.To.Add(litigationRequest?.ReformerCitizen.Email);
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
                objSms.recipients.Add(litigationRequest?.ReformerCitizen.Mobile);

            }
            //مدير ادارة الصلح
            if (deptAdminInfoReconciliation is not null)
            {
                objEmail.To.Add(deptAdminInfoReconciliation?.Data?.Email);
                objSms.recipients.Add(deptAdminInfoReconciliation?.Data?.Mobile);

                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = deptAdminInfoReconciliation.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationSession,
                    Message = msg,
                });
            }
            if (litigationRequest?.Contract?.FirstPartyCitizen != null && litigationRequest?.Contract?.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyCitizen.Email);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationSession,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyCitizen.Mobile);


            }
            if (litigationRequest?.Contract?.SecondPartyCitizen != null)
            {
                objEmail.To.Add(litigationRequest?.Contract?.SecondPartyCitizen.Email);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Defendant,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationSession,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.SecondPartyCitizen.Mobile);

            }
            //litigationRequest.Contract.FirstPartyFacility.Citizen
            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyFacility.Email);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationSession,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyFacility?.Citizen?.Mobile);


            }
                if (litigationRequest?.Contract?.SecondPartyFacility != null)
            {
                objEmail.To.Add(litigationRequest?.Contract?.SecondPartyFacility.Email);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility?.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Defendant,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationSession,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.SecondPartyFacility?.Citizen?.Mobile);

            }
            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);

        }
        public async Task<BaseResponse> GetLitigationRequestForSession(Guid requestId, CancellationToken cancellationToken)
        {
            if (requestId == Guid.Empty)
                return new BaseResponse() { IsDone = false, Message = "Not Found" };

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            if (referenceId == Guid.Empty)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.Area)
                          .Include(x => x.City)
                          .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation))
                          .Include(x => x.LitigationRequestModificationRequests)
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestReconciliationReports)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.InternalStatus)
                          .Include(x => x.ExternalStatus)
                .Include(x => x.LitigationRequestReformerAssigningRequests)
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
                          .Where(x => x.Id == requestId &&
                       x.LitigationRequestReformerAssigningRequests.Any(l => l.CitizenId == referenceId) & x.Id == requestId

                          ).ToList().FirstOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };

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
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                    allowAdd = false;
                else
                {

                    if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestReportAdded)
                        allowAdd = false;
                    else
                    {
                        int sessionCountValidation = 3;
                        try
                        {
                            sessionCountValidation = int.Parse((await _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Litigation_Request_Max_Session_ReScheduleTime)).Data?.Value);
                        }
                        catch { }
                        if (litigationRequest.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) >= sessionCountValidation)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.LitigationRequestReconciliationReports != null)
                            {
                                if (litigationRequest?.LitigationRequestReconciliationReports.Count > 0)
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

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                      .Include(x => x.Area)
                                      .Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation))
                                      .Include(x => x.LitigationRequestReconciliationReports)
                                      .Include(x => x.ApplicantCitizen)
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
                                      .Where(x => x.ReformerCitizenId == referenceId & x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration).OrderByDescending(x => x.CreatedOn).GetPaged(pageIndex, pageSize);

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
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
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
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                            allowAdd = false;
                        else
                        {

                            if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestReportAdded)
                                allowAdd = false;
                            else
                            {
                                int sessionCountValidation = 3;
                                try
                                {
                                    sessionCountValidation = int.Parse((await _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Litigation_Request_Max_Session_ReScheduleTime)).Data?.Value);
                                }
                                catch { }
                                if (litigationRequest.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) >= sessionCountValidation)
                                    allowAdd = false;
                                else
                                {
                                    if (litigationRequest?.LitigationRequestReconciliationReports != null)
                                    {
                                        if (litigationRequest?.LitigationRequestReconciliationReports.Count > 0)
                                        {
                                            allowAdd = false;
                                        }
                                    }
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
        // جلسات المدعى والمدعى عليه
        public async Task<BaseResponse> GetLitigationRequestForAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                         .Include(x => x.Area)
                                      .Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation))
                                      .Include(x => x.LitigationRequestReconciliationReports)
                                      .Include(x => x.ApplicantCitizen)
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
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) > 0
                & x.ApplicantCitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration & x.ReformerCitizenId.HasValue

                //|
                //x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                //& x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) > 0 & x.RequestCode != null
                //& x.Contract.FirstPartyCitizenId == referenceId
                //& x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration & x.ReformerCitizenId.HasValue

                |
                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) > 0
                & x.RequestCode != null
                & x.Contract.SecondPartyCitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration & x.ReformerCitizenId.HasValue


                |

                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) > 0
                & x.RequestCode != null
                & x.Contract.FirstPartyFacility.CitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration & x.ReformerCitizenId.HasValue


                |

                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation) > 0
                & x.RequestCode != null
                & x.Contract.SecondPartyFacility.CitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration & x.ReformerCitizenId.HasValue)
                                        .OrderByDescending(x => x.CreatedOn)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);


            List<LitigationRequestMircoForRequestSessionDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestSessionDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoForRequestSessionDto litigationRequestMircoDto = new LitigationRequestMircoForRequestSessionDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.SessionDatetime = litigationRequest.LitigationRequestSessions.OrderBy(x => x.SessionDatetime).Last().SessionDatetime;
                    litigationRequestMircoDto.Url = litigationRequest.LitigationRequestSessions.OrderBy(x => x.SessionDatetime).Last().SessionUrl;
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
        // جلسات المصلح
        public async Task<BaseResponse> GetLitigationRequestForAllSessionForReformer(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                         .Include(x => x.Area)
                                      .Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation))
                                      .Include(x => x.LitigationRequestReconciliationReports)
                                      .Include(x => x.ApplicantCitizen)
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
                .Where(x => x.ReformerCitizenId == referenceId & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation)
                .Count() > 0)
                                        .OrderByDescending(x => x.CreatedOn)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);


            List<LitigationRequestMircoForRequestSessionDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestSessionDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoForRequestSessionDto litigationRequestMircoDto = new LitigationRequestMircoForRequestSessionDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.SessionDatetime = litigationRequest.LitigationRequestSessions.OrderBy(x => x.SessionDatetime).Last().SessionDatetime;
                    litigationRequestMircoDto.Url = litigationRequest.LitigationRequestSessions.OrderBy(x => x.SessionDatetime).Last().SessionUrl;
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
        // كل جلسات الصلح
        public async Task<BaseResponse> GetLitigationRequestForFullAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                         .Include(x => x.Area)
                                      .Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation))
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
            litigationRequests = entitiesQuerable.Where(x => x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration
            & x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId
            == LitigationRequestSessionRequestType.Reconciliation).Count() > 0)
                                        .OrderByDescending(x => x.CreatedOn)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);


            List<LitigationRequestMircoForRequestSessionDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestSessionDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoForRequestSessionDto litigationRequestMircoDto = new LitigationRequestMircoForRequestSessionDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility.ArabicFacilityName : litigationRequest.DefendantCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.SessionDatetime = litigationRequest.LitigationRequestSessions.OrderBy(x => x.SessionDatetime).Last().SessionDatetime;
                    litigationRequestMircoDto.Url = litigationRequest.LitigationRequestSessions.OrderBy(x => x.SessionDatetime).Last().SessionUrl;
                    litigationRequestMircoDto.MeetingId = litigationRequest.LitigationRequestSessions.OrderBy(x => x.SessionDatetime).Last().MeetingId;
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
        // تعيين ال Reformer
        public async Task<BaseResponse> GetLitigationRequestForFullAllReformer(int pageIndex, int pageSize, CancellationToken cancellationToken, LitigationRequestReconciliationAssignReformersFilter? filter, bool ascending = false)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var entitiesQueryable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.ReformerCitizen)
                .Include(x => x.Area)
                .Include(x => x.City)
                .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Reconciliation))
                .Include(x => x.LitigationRequestReconciliationReports)
                .Include(x => x.LitigationRequestReformerAssigningRequests.Where(r => r.Response == 0))
                .ThenInclude(x => x.ReformerCitizen)
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

            var litigationRequestsQuery = entitiesQueryable
                .Where(x => x.LitigationRequestStage != LitigationRequestStage.Review
                             && x.LitigationRequestStage != LitigationRequestStage.Arbitration
                             && x.RequestCode != null
                             && x.InternalStatusId != RequestStatus.Rejected
                             && x.InternalStatusId != RequestStatus.ReconciliationWasMade
                             && x.InternalStatusId != RequestStatus.Cancelled
                             && x.InternalStatusId != RequestStatus.SaveAndClose);

            if (filter is not null)
            {
                Expression<Func<LitigationRequest, bool>> combinedCondition = PredicateBuilder.True<LitigationRequest>();

                if (filter.ReformerName is not null)
                {
                    combinedCondition = combinedCondition.AndAlso(x =>
                        x.LitigationRequestReformerAssigningRequests.Any(y => (y.ReformerCitizen.FullName.Contains(filter.ReformerName)) && y.Response == 0)
                        || x.ReformerCitizen.FullName.Contains(filter.ReformerName));
                    litigationRequestsQuery = entitiesQueryable.Where(combinedCondition);

                }
                if (filter.AreaId is not null)
                {
                    combinedCondition = combinedCondition.AndAlso(x =>
                        x.ReformerCitizen.AreaId == filter.AreaId
                        || x.LitigationRequestReformerAssigningRequests.Any(y => y.ReformerCitizen.AreaId == filter.AreaId));
                    litigationRequestsQuery = entitiesQueryable.Where(combinedCondition);

                }
                if (filter.CityId is not null)
                {
                    combinedCondition = combinedCondition.AndAlso(x =>
                        x.ReformerCitizen.CityId == filter.CityId
                        || x.LitigationRequestReformerAssigningRequests.Any(y => y.ReformerCitizen.CityId == filter.CityId));
                    litigationRequestsQuery = entitiesQueryable.Where(combinedCondition);

                }

            }

            litigationRequestsQuery = ascending
                ? litigationRequestsQuery.OrderBy(x => x.CreatedOn)
                : litigationRequestsQuery.OrderByDescending(x => x.CreatedOn);

            var totalCount = await litigationRequestsQuery.CountAsync(cancellationToken);

            var results = await litigationRequestsQuery
                .AsSplitQuery()
                .AsNoTracking()
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            List<LitigationRequestMircoForRequestReformerDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestReformerDto>();

            if (results != null)
            {
                foreach (var litigationRequest in results)
                {
                    LitigationRequestMircoForRequestReformerDto litigationRequestMircoDto = new LitigationRequestMircoForRequestReformerDto
                    {
                        Id = litigationRequest.Id,
                        RequestDate = litigationRequest.CreatedOn,
                        RequestCode = litigationRequest.RequestCode,
                        FirstPartyName = litigationRequest.Contract.FirstPartyCitizen == null ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName,
                        SecondPartyName = litigationRequest.DefendantCitizen == null ? litigationRequest.DefendantFacility.ArabicFacilityName : litigationRequest.DefendantCitizen.FullName,
                        RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName,
                        ReformerId = litigationRequest.ReformerCitizen?.Id,
                        ReformerName = litigationRequest.ReformerCitizen != null ? litigationRequest.ReformerCitizen.FullName : litigationRequest.LitigationRequestReformerAssigningRequests?.OrderByDescending(l => l.RequestOn).FirstOrDefault()?.ReformerCitizen?.FullName
                    };

                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }

            return new DataResponse<PagedResult<LitigationRequestMircoForRequestReformerDto>>
            {
                Data = new PagedResult<LitigationRequestMircoForRequestReformerDto>
                {
                    Results = litigationRequestMircoDtos,
                    CurrentPage = pageIndex,
                    PageCount = (int)Math.Ceiling((double)totalCount / pageSize),
                    PageSize = pageSize,
                    TotalCount = totalCount
                },
                IsDone = true
            };
        }
        #endregion
    }
}
