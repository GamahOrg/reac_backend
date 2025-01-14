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
                requestPayload.SessionDateTime = requestPayload.SessionDateTime.ToLocalTime();
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration))
                        .Include(x => x.ArbitratorCitizen)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.LitigationRequestArbitratorLists)
                          .ThenInclude(x => x.ArbitratorCitizen)
                        .FirstOrDefault(x => x.Id == requestPayload.RequestId);

                if (requestPayload.SessionDateTime < litigationRequest.SentOn)
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة جلسة قبل تاريخ تقديم طلب الدعوى"
                    };
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
                    var _zoomMeeting = await _zoomService.GenerateMeeting($"{litigationRequest.RequestCode}_{sessionDateGreg.ToString()}",sessionDateGreg);
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
                            msg = msg?.Replace("{SessionLink}", sessionUrl);


                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                                  new SysNotificationDto{
                        ActorId = litigationRequest.ArbitratorCitizen.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        },
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


                            };
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
                            };
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
                            };
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
                            };

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
                                Body = msg
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
                          .Include(x => x.Contract.LitigationRequestContractType).AsNoTracking()
                          .Where(x => x.Id == requestId
                          & x.ArbitratorCitizenId == referenceId
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


            litigationRequests = entitiesQuerable.Where(x => x.ArbitratorCitizenId == referenceId
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
                                      .Include(x => x.LitigationRequestSessions.Where(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration))
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
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) > 0
                & x.ApplicantCitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

                |
                x.RequestCode != null & x.InternalStatusId != RequestStatus.Rejected
                & x.LitigationRequestSessions.Count(x => x.LitigationRequestSessionRequestTypeId == LitigationRequestSessionRequestType.Arbitration) > 0 & x.RequestCode != null
                & x.Contract.FirstPartyCitizenId == referenceId
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation

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
        // عرض جلسات للمحكم
        public async Task<BaseResponse> GetLitigationRequestForAllSessionForArbitrator(int pageIndex, int pageSize, CancellationToken cancellationToken)
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

                    LitigationRequestMircoForRequestSessionDto litigationRequestMircoDto = new LitigationRequestMircoForRequestSessionDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
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

        #endregion
    }
}
