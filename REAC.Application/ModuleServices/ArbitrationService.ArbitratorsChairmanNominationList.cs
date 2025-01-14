using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.ModuleServices
{
    public partial class ArbitrationService
    {
        #region Nomination List
        public async Task<BaseResponse> AddLitigationRequestArbitratorsChairmanNominationList(ArbitratorNominationListRequestPayload arbitratorNominationListRequestPayload)
        {
            try
            {
                if (arbitratorNominationListRequestPayload.Arbitrators.Count == 0)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لابد من إضافة مرشحى رئيس هيئة المحكمين"
                    };

                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen)
                        .Include(x => x.LitigationRequestArbitratorLists)
                        .ThenInclude(x => x.ArbitratorCitizen)
                        .Where(x => x.Id == arbitratorNominationListRequestPayload.RequestId)
                        .SingleOrDefault();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };


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
                    {
                        allowAdd = false;
                    }
                    else
                    {
                        if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNomination)
                            allowAdd = false;
                    }
                }
                if (!litigationRequest.LitigationRequestArbitratorNominationLists.Any())
                    allowAdd = false;

                if (allowAdd)
                {

                    arbitratorNominationListRequestPayload.Arbitrators.ForEach(async t =>
                    {
                        LitigationRequestArbitratorsChairmanNominationList litigationRequestArbitratorNominationList = new LitigationRequestArbitratorsChairmanNominationList();
                        litigationRequestArbitratorNominationList.ArbitratorChairmanCitizenId = t;
                        litigationRequestArbitratorNominationList.LitigationRequestId = arbitratorNominationListRequestPayload.RequestId;
                        litigationRequestArbitratorNominationList.Id = Guid.NewGuid();
                        await _repository.AddAsync<LitigationRequestArbitratorsChairmanNominationList, Guid>(litigationRequestArbitratorNominationList);
                    });
                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNominationDone;
                    litigationRequest.ArbitratorsChairmanNominationListCreationDateTime = DateTime.Now;

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;

                    // Send Notification
                    // تم إضافة قائمة مرشحى رئيس هيئة التحكيم 
                    try
                    {
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        if (arbitrationAdmin.IsDone)
                        {

                            var msg = "";
                            msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_Added).Result.Data?.Value;



                            msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
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
                                  arbitrationAdmin.Data?.Email
                                };

                            foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
                            {
                                notifications.Add(new SysNotificationDto
                                {

                                    ActorId = arbitrator.ArbitratorCitizenId,
                                    RequestId = litigationRequest.Id,
                                    CreatedOn = DateTime.Now,
                                    RequestTypeId = RequestType.LitigationRequest,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    Message = msg
                                });
                                emails.Add(arbitrator.ArbitratorCitizen.Email);
                            }
                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = "تم إضافة قائمة مرشحى رئيس هيئة التحكيم ",
                                Body = msg
                            }, null, notifications);


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
                        Message = "لا يمكن إضافة عدد رؤساء هيئة المحكمين"
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
            return await GetLitigationRequestForAddArbitratorsChairmanNominationList(arbitratorNominationListRequestPayload.RequestId);
        }
        public async Task<BaseResponse> AddLitigationRequestArbitratorsChairmandecision(Guid requestId, Guid arbitratorChairmanId)
        {
            try
            {
                if (requestId == null | requestId == Guid.Empty)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                if (arbitratorChairmanId == null | arbitratorChairmanId == Guid.Empty)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                        .Where(x => x.Id == requestId)
                        .SingleOrDefault();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };


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
                if (!allowAdd)
                    return new BaseResponse() { IsDone = false, Message = "لا يمكن إتمام العملية" };

                LitigationRequestArbitratorAssigningRequest litigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                litigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                litigationRequestArbitratorAssigningRequest.Id = Guid.NewGuid();
                litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
                litigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                litigationRequestArbitratorAssigningRequest.CitizenId = arbitratorChairmanId;
                litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;

                await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestArbitratorAssigningRequest);

                litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Where(x => x.ArbitratorChairmanCitizenId == arbitratorChairmanId).FirstOrDefault().IsSent = true;

                _repository.Update<LitigationRequest, Guid>(litigationRequest);

                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                if (!repositorySaveResponse.IsDone)
                    return repositorySaveResponse;

                // new
                // Send Notification 
                // تم تحديد رئيس هئية التحكيم للدعوة رقم ...... وإرسال طلب المهمة
                // للمحكمين - مدير إدارة التحكيم
                try
                {
                    var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

                    var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorChairman_NewTask_Admin).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);


                    List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequestArbitratorAssigningRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.ArbitratorChairmanNominationList,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        }

                        };
                    List<string> emails = new List<string>() {
                                  arbitrationAdmin.Data?.Email
                                };
                    List<string> recepiant = new List<string>() {
                                  arbitrationAdmin.Data?.Mobile
                                };

                    foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
                    {
                        notifications.Add(new SysNotificationDto
                        {

                            ActorId = arbitrator.ArbitratorCitizenId,
                            RequestId = litigationRequest.Id,
                            CreatedOn = DateTime.Now,
                            RequestTypeId = RequestType.LitigationRequest,
                            ActorTypeId = NotificationActorType.Arbitrator,
                            Message = msg
                        });
                        emails.Add(arbitrator.ArbitratorCitizen.Email);
                        recepiant.Add(arbitrator.ArbitratorCitizen.Mobile);
                    }
                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                    {
                        To = emails,
                        Subject = "تحديد رئيس هيئة التحكيم",
                        Body = msg
                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto() { message_text = msg, notification_type = "SMS", recipients = recepiant }
                    , notifications);

                }
                catch (Exception e) { }


                // يوجد طلب مهمة جديد كرئيس هئية تحكيم على الدعوى رقم ...........
                // رئيس هيئة التحكيم المرسل له المهمة
                try
                {
                    var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

                    var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorChairman_NewTask).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);

                    var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                        .FirstOrDefault(x => x.Id == litigationRequestArbitratorAssigningRequest.CitizenId);

                    List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                           new SysNotificationDto{
                        ActorId = citizenInfo.Id,
                        RequestId=litigationRequestArbitratorAssigningRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.ArbitratorChairmanNominationList,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        }

                        };
                    List<string> emails = new List<string>() {
                                  citizenInfo.Email
                                };
                    List<string> recepiant = new List<string>() {
                                  citizenInfo.Mobile
                                };


                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                    {
                        To = emails,
                        Subject = "طلب مهمة جديد كرئيس هيئة تحكيم ",
                        Body = msg
                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto() { message_text = msg, notification_type = "SMS", recipients = recepiant }
                    , notifications);

                }
                catch (Exception e) { }
            }
            catch (Exception ex)
            {
                new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return await GetLitigationRequestForAddArbitratorsChairmanNominationList(requestId);
        }

        public async Task<BaseResponse> GetLitigationRequestForAddArbitratorsChairmanNominationList(Guid requestId)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                               .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                               .Include(x => x.Area).Include(x => x.City)
                                .Include(x => x.InternalStatus)
                                .Include(x => x.ExternalStatus)
                                .Include(x => x.LitigationRequestArbitratorLists)
                                .Include(x => x.ArbitratorCitizen)
                                .ThenInclude(x => x.ArbitratorLitigationRequests)
                                .Include(x => x.ArbitratorCitizen)
                                .ThenInclude(x => x.CitizenRequests)
                                .ThenInclude(x => x.SysArbitratorClass)

                                .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                                .ThenInclude(x => x.ArbitratorChairmanCitizen)
                                .ThenInclude(x => x.ArbitratorLitigationRequests)

                                .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                                .ThenInclude(x => x.ArbitratorChairmanCitizen)
                                .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)

                                .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                                .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)

                                .Include(x => x.LitigationRequestArbitratorNominationLists)
                                .ThenInclude(x => x.ArbitratorCitizen)
                                .ThenInclude(x => x.CitizenRequests)
                                .ThenInclude(x => x.SysArbitratorClass)

                                .Include(x => x.LitigationRequestArbitratorNominationLists)
                                .ThenInclude(x => x.ArbitratorCitizen)
                                .ThenInclude(x => x.LitigationRequestArbitratorLists)


                                .Include(x => x.LitigationRequestArbitratorLists)
                                .Include(x => x.Contract)

                                .Include(x => x.Contract.LitigationRequestContractType)
                                .Include(x => x.Contract.FirstPartyFacility)
                                .Include(x => x.Contract.FirstPartyCitizen)
                                .Include(x => x.Contract.SecondPartyFacility)
                                .Include(x => x.Contract.SecondPartyCitizen)
                                .Include(x => x.Contract.LitigationRequestContractType)
                                .Where(x => x.Id == requestId).ToList().FirstOrDefault();

                var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

                bool isDone = false;
                bool allowAdd = true;
                bool AllowDecision = false;
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
                    {
                        allowAdd = false;
                    }
                    else
                    {
                        if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNomination)
                            allowAdd = false;
                    }
                }
                LitigationRequestForAllArbitratorCharmanNominationListDto litigationRequestForAllArbitratorNominationListDto = new LitigationRequestForAllArbitratorCharmanNominationListDto();
                litigationRequestForAllArbitratorNominationListDto.AllowAdd = allowAdd;
                litigationRequestForAllArbitratorNominationListDto.LitigationRequestShortDto = request;
                litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorCharmanShortStatusDtos = new List<CitizenArbitratorCharmanShortStatusDto>();

                if (litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Any())
                {
                    if (litigationRequest.IsArbitratorsChairmanDecisionRequired.HasValue &
                        litigationRequest.LitigationRequestArbitratorAssigningRequests?.Where(x => x.ArbitratorAssigningRequestTypeId
                                     == ArbitratorAssigningRequestType.ArbitratorsChairman).Count() == 0)
                    {
                        litigationRequestForAllArbitratorNominationListDto.AllowDecision = true;
                        litigationRequestForAllArbitratorNominationListDto.AllowAdd = false;
                        for (int record = 0; record <= 1; record++)
                        {
                            CitizenArbitratorCharmanShortStatusDto citizenArbitratorShortStatusChairmanDto = new CitizenArbitratorCharmanShortStatusDto();
                            citizenArbitratorShortStatusChairmanDto.Id = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizenId;
                            citizenArbitratorShortStatusChairmanDto.FullName = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen.FullName;


                            citizenArbitratorShortStatusChairmanDto.Mobile = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen.Mobile;
                            citizenArbitratorShortStatusChairmanDto.Email = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen.Email;

                            citizenArbitratorShortStatusChairmanDto.ArbitratorClass =
                                litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;

                            citizenArbitratorShortStatusChairmanDto.Brief =
                                litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            citizenArbitratorShortStatusChairmanDto.ExperienceYears =
                                litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;

                            citizenArbitratorShortStatusChairmanDto.TotalPriority = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].LitigationRequestArbitratorsChairmanNominationListValues.Sum(x => x.Priority);

                            citizenArbitratorShortStatusChairmanDto.RequestsCount = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen?.ArbitratorLitigationRequests.Count() + litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen.LitigationRequestArbitratorLists.Count;

                            citizenArbitratorShortStatusChairmanDto.IdentityNumber = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                                .OrderBy(x => x.TotalPriority).ToList()[record].ArbitratorChairmanCitizen.IdentityNumber;

                            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorCharmanShortStatusDtos.Add(citizenArbitratorShortStatusChairmanDto);

                        }
                        return new DataResponse<LitigationRequestForAllArbitratorCharmanNominationListDto>()
                        {
                            Data = litigationRequestForAllArbitratorNominationListDto,
                            IsDone = true
                        };
                    }
                    else
                    {
                        litigationRequestForAllArbitratorNominationListDto.AllowDecision = false;
                        litigationRequestForAllArbitratorNominationListDto.AllowAdd = false;
                        foreach (var litigationRequestArbitratorNominationList in litigationRequest.LitigationRequestArbitratorsChairmanNominationLists)
                        {
                            CitizenArbitratorCharmanShortStatusDto citizenArbitratorShortStatusChairmanDto = new CitizenArbitratorCharmanShortStatusDto();
                            citizenArbitratorShortStatusChairmanDto.Id = litigationRequestArbitratorNominationList.ArbitratorChairmanCitizenId;
                            citizenArbitratorShortStatusChairmanDto.FullName = litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen.FullName;
                            citizenArbitratorShortStatusChairmanDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen.Mobile;
                            citizenArbitratorShortStatusChairmanDto.Email = litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen.Email;

                            citizenArbitratorShortStatusChairmanDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;

                            citizenArbitratorShortStatusChairmanDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            citizenArbitratorShortStatusChairmanDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;

                            citizenArbitratorShortStatusChairmanDto.TotalPriority = litigationRequestArbitratorNominationList.LitigationRequestArbitratorsChairmanNominationListValues.Sum(x => x.Priority);

                            citizenArbitratorShortStatusChairmanDto.RequestsCount = litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen?.ArbitratorLitigationRequests.Count() + litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen.LitigationRequestArbitratorLists.Count;
                            citizenArbitratorShortStatusChairmanDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorChairmanCitizen.IdentityNumber;

                            var assingingRequest = litigationRequest.LitigationRequestArbitratorAssigningRequests.Where(t => t.LitigationRequestId == litigationRequest.Id
                                                                   & t.CitizenId == litigationRequestArbitratorNominationList.ArbitratorChairmanCitizenId & t.ArbitratorAssigningRequestTypeId == ArbitratorAssigningRequestType.ArbitratorsChairman);

                            if (assingingRequest != null)
                            {
                                if (assingingRequest.Any())
                                {
                                    if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve)
                                    {
                                        citizenArbitratorShortStatusChairmanDto.Status = "تم الموافقة";
                                        litigationRequestForAllArbitratorNominationListDto.AllowDecision = false;
                                    }
                                    else
                                    {
                                        if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject)
                                            citizenArbitratorShortStatusChairmanDto.Status = "تم الرفض";
                                        else
                                        {
                                            if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)
                                                citizenArbitratorShortStatusChairmanDto.Status = "تم الإلغاء";
                                            else
                                            {
                                                citizenArbitratorShortStatusChairmanDto.Status = "قيد الرد";
                                            }
                                        }
                                    }
                                }
                            }
                            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorCharmanShortStatusDtos.Add(citizenArbitratorShortStatusChairmanDto);
                        }
                        return new DataResponse<LitigationRequestForAllArbitratorCharmanNominationListDto>()
                        {
                            Data = litigationRequestForAllArbitratorNominationListDto,
                            IsDone = true
                        };
                    }
                }
                else
                {
                    if (litigationRequest.ArbitratorCitizenId.HasValue)
                    {
                        litigationRequestForAllArbitratorNominationListDto.AllowDecision = false;
                        litigationRequestForAllArbitratorNominationListDto.AllowAdd = false;
                        CitizenArbitratorCharmanShortStatusDto citizenArbitratorShortStatusChairmanDto = new CitizenArbitratorCharmanShortStatusDto();
                        citizenArbitratorShortStatusChairmanDto.FullName = litigationRequest.ArbitratorCitizen?.FullName;
                        citizenArbitratorShortStatusChairmanDto.Mobile = litigationRequest.ArbitratorCitizen.Mobile;
                        citizenArbitratorShortStatusChairmanDto.Email = litigationRequest.ArbitratorCitizen.Email;

                        citizenArbitratorShortStatusChairmanDto.ArbitratorClass =
                            litigationRequest.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;

                        citizenArbitratorShortStatusChairmanDto.Brief =
                            litigationRequest.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                        citizenArbitratorShortStatusChairmanDto.ExperienceYears =
                            litigationRequest.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;

                        citizenArbitratorShortStatusChairmanDto.TotalPriority = 0;
                        citizenArbitratorShortStatusChairmanDto.RequestsCount = litigationRequest.ArbitratorCitizen.ArbitratorLitigationRequests.Count() + litigationRequest.ArbitratorCitizen.LitigationRequestArbitratorLists.Count;
                        citizenArbitratorShortStatusChairmanDto.IdentityNumber = litigationRequest.ArbitratorCitizen.IdentityNumber;
                        citizenArbitratorShortStatusChairmanDto.Status = "تم الموافقة";
                        litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorCharmanShortStatusDtos.Add(citizenArbitratorShortStatusChairmanDto);

                        return new DataResponse<LitigationRequestForAllArbitratorCharmanNominationListDto>()
                        {
                            Data = litigationRequestForAllArbitratorNominationListDto,
                            IsDone = true
                        };
                    }
                }

                return new DataResponse<LitigationRequestForAllArbitratorCharmanNominationListDto>()
                {
                    Data = new LitigationRequestForAllArbitratorCharmanNominationListDto()
                    {
                        ListCount = (await _repository.GetQueryable<SysArbitratorNominationListCount, Guid>()).Where(x => x.ArbitratorCount == litigationRequest.ArbitratorsCount).FirstOrDefault()?.ArbitratorChairmanNominationListCount,
                        AllowAdd = allowAdd,
                        AllowDecision = AllowDecision,
                        LitigationRequestShortDto = request
                    },
                    IsDone = true,
                };
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
        }
        public async Task<BaseResponse> GetLitigationRequestForAddArbitratorsChairmanNominationList(int pageIndex, int pageSize)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                            .Include(x => x.Area).Include(x => x.City)
                            .Include(x => x.InternalStatus)
                            .Include(x => x.ExternalStatus)

                            .Include(x => x.ArbitratorCitizen)
                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.ArbitratorChairmanCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.CitizenRequests)
                            .ThenInclude(x => x.SysArbitratorClass)

                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.Contract)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .Include(x => x.Contract.FirstPartyFacility)
                            .Include(x => x.Contract.FirstPartyCitizen)
                            .Include(x => x.Contract.SecondPartyFacility)
                            .Include(x => x.Contract.SecondPartyCitizen)
                            .Include(x => x.Contract.LitigationRequestContractType);

            litigationRequests = entitiesQuerable.Where(x => x.RequestCode != null & x.ArbitratorsCount != 1 & x.LitigationRequestArbitratorLists.Count > 0
            & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation & x.LitigationRequestStage == LitigationRequestStage.Arbitration)
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
                    litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
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
                    if (litigationRequest.IsArbitratorsChairmanDecisionRequired.HasValue &
                        litigationRequest.LitigationRequestArbitratorAssigningRequests?.Where(x => x.ArbitratorAssigningRequestTypeId
                                     == ArbitratorAssigningRequestType.ArbitratorsChairman).Count() == 0)
                    { }
                    else
                    {
                        if (!isDone)
                        {
                            if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                                allowAdd = false;
                            else
                            {
                                if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNomination)
                                    allowAdd = false;
                            }
                        }
                        if (litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Count > 0)
                            allowAdd = false;
                    }

                    if (!litigationRequest.ArbitratorCitizenId.HasValue)
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
        public async Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(int pageIndex, int pageSize)
        {

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
               .Include(x => x.Area).Include(x => x.City)
                            .Include(x => x.InternalStatus)
                            .Include(x => x.ExternalStatus)

                            .Include(x => x.ArbitratorCitizen)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.ArbitratorChairmanCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.ArbitratorChairmanCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.ArbitratorChairmanCitizen)
                            .ThenInclude(x => x.CitizenRequests)
                            .ThenInclude(x => x.SysArbitratorClass)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.ArbitratorChairmanCitizen)
                            .Include(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.Contract)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .Include(x => x.Contract.FirstPartyFacility)
                            .Include(x => x.Contract.FirstPartyCitizen)
                            .Include(x => x.Contract.SecondPartyFacility)
                            .Include(x => x.Contract.SecondPartyCitizen)
                            .Include(x => x.Contract.LitigationRequestContractType);


            litigationRequests = entitiesQuerable.Where(
                                      x => x.RequestCode != null & x.LitigationRequestArbitratorLists.Count > 0
                                      & x.ArbitratorsCount != 1
                                      & x.LitigationRequestArbitratorLists.Where(t => t.ArbitratorCitizenId == referenceId).Count() > 0
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitratorNominationLists.Count > 0)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);

            List<LitigationRequestMircoDto> litigationRequestMircoDtos = new List<LitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    bool allowAdd = true;

                    LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.Status = litigationRequest.ExternalStatus.RequestStatus;

                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;

                    if (litigationRequest.LitigationRequestArbitratorsChairmanNominationLists
                        .Where(x => x.LitigationRequestArbitratorsChairmanNominationListValues.Count(x => x.ArbitratorCitizenId == referenceId) > 0).Count() > 0)
                        allowAdd = false;

                    if (litigationRequest.ArbitratorCitizenId.HasValue)
                        allowAdd = false;

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
        public async Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(Guid requestId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                            .Include(x => x.Area).Include(x => x.City)
                            .Include(x => x.InternalStatus)
                            .Include(x => x.ExternalStatus)
                            .Include(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)
                            .Include(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.CitizenRequests)
                            .ThenInclude(x => x.SysArbitratorClass)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.ArbitratorChairmanCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.ArbitratorChairmanCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)

                            .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                            .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)

                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.CitizenRequests)
                            .ThenInclude(x => x.SysArbitratorClass)

                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorLists)


                            .Include(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.Contract)

                            .Include(x => x.Contract.LitigationRequestContractType)
                            .Include(x => x.Contract.FirstPartyFacility)
                            .Include(x => x.Contract.FirstPartyCitizen)
                            .Include(x => x.Contract.SecondPartyFacility)
                            .Include(x => x.Contract.SecondPartyCitizen)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .Where(x => x.Id == requestId
                            & x.LitigationRequestArbitratorLists.Any(l => l.ArbitratorCitizenId == referenceId)
                            ).ToList().FirstOrDefault();
            
            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            bool isDone = false;
            bool allowAdd = true;

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
                {
                    allowAdd = false;
                }
                else
                {
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorChairmanListNominationDone)
                        allowAdd = false;
                }

            }
            if (litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Where(x => x.LitigationRequestArbitratorsChairmanNominationListValues
                                                     .Where(x => x.ArbitratorCitizenId == referenceId).Count() > 0).Count() > 0)
                allowAdd = false;

            LitigationRequestForAllArbitratorNominationListWithPartiesValueDto litigationRequestForAllArbitratorNominationListDto = new LitigationRequestForAllArbitratorNominationListWithPartiesValueDto();
            litigationRequestForAllArbitratorNominationListDto.LitigationRequestShortDto = request;
            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos = new List<CitizenArbitratorShortStatusWithValueDto>();
            if (litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Any())
            {
                foreach (var chairmanNominationList in litigationRequest.LitigationRequestArbitratorsChairmanNominationLists)
                {
                    CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                    citizenArbitratorShortStatusDto.Id = chairmanNominationList.Id;
                    citizenArbitratorShortStatusDto.CitizenId = chairmanNominationList.ArbitratorChairmanCitizenId;
                    citizenArbitratorShortStatusDto.FullName = chairmanNominationList.ArbitratorChairmanCitizen.FullName;
                    citizenArbitratorShortStatusDto.Brief =
                        chairmanNominationList.ArbitratorChairmanCitizen?.CitizenRequests?
                        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                        .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    citizenArbitratorShortStatusDto.Value = chairmanNominationList.LitigationRequestArbitratorsChairmanNominationListValues.Where(x => x.ArbitratorCitizenId == referenceId).FirstOrDefault()?.Priority;
                    litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                }
                litigationRequestForAllArbitratorNominationListDto.AllowAdd = allowAdd;
                return new DataResponse<LitigationRequestForAllArbitratorNominationListWithPartiesValueDto>()
                {
                    Data = litigationRequestForAllArbitratorNominationListDto,
                    IsDone = true,
                };
            }
            else
            {
                if (litigationRequest.ArbitratorCitizenId.HasValue)
                {
                    allowAdd = false;
                    CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusChairmanDto = new CitizenArbitratorShortStatusWithValueDto();
                    citizenArbitratorShortStatusChairmanDto.FullName = litigationRequest.ArbitratorCitizen?.FullName;
                    citizenArbitratorShortStatusChairmanDto.Brief =
                        litigationRequest.ArbitratorCitizen?.CitizenRequests?
                        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                        .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusChairmanDto);

                    return new DataResponse<LitigationRequestForAllArbitratorNominationListWithPartiesValueDto>()
                    {
                        Data = litigationRequestForAllArbitratorNominationListDto,
                        IsDone = true
                    };
                }
                else
                {

                }
            }
            return new BaseResponse()
            {
                IsDone = false,
                Message = "خطأ ما يرجى الرجوع لمدير النظام"
            };

        }
        public async Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsChairmanNominationListReply(ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.LitigationRequestArbitratorNominationLists)
                          .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                          .ThenInclude(x => x.ArbitratorChairmanCitizen)
                           .Include(x => x.LitigationRequestArbitratorLists)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Where(x => x.Id == arbitratorNominationListRequestReplyPayload.RequestId).ToList().FirstOrDefault();

            bool isDone = false;

            arbitratorNominationListRequestReplyPayload?.ArbitratorNominationListRequestItems.ToList().ForEach(item =>
            {
                var valueItem = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Where(x => x.Id == item.ArbitratorListId).FirstOrDefault();
                if (valueItem != null)
                {
                    _repository.AddAsync<LitigationRequestArbitratorsChairmanNominationListValue, Guid>(new LitigationRequestArbitratorsChairmanNominationListValue()
                    {
                        Id = Guid.NewGuid(),
                        ArbitratorChairmanCitizenId = valueItem.ArbitratorChairmanCitizenId,
                        LitigationRequestArbitratorsChairmanNominationListId = valueItem.Id,
                        IsSent = false,
                        ArbitratorCitizenId = referenceId,
                        Priority = item.Priority
                    });
                }
            });
            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            int total = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Count * litigationRequest.LitigationRequestArbitratorLists.Count;

            litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                                          .Include(x => x.LitigationRequestArbitratorNominationLists)
                          .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                                                    .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)
                          .Include(x => x.ApplicantCitizen)
                          .Where(x => x.Id == arbitratorNominationListRequestReplyPayload.RequestId).ToList().FirstOrDefault();

            if (litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists != null)
            {
                litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists.ToList().ForEach(chairmanNominationList =>
                {
                    total -= chairmanNominationList.LitigationRequestArbitratorsChairmanNominationListValues.Count;
                });

                litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists.ToList().ForEach(chairmanNominationList =>
                {
                    chairmanNominationList.TotalPriority = chairmanNominationList.LitigationRequestArbitratorsChairmanNominationListValues.Sum(x => x.Priority);
                });

                if (total == 0)
                {
                    litigationRequest.ArbitratorsChairmanNominationListSelectionDoneDateTime = DateTime.Now;

                    if (litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].TotalPriority == litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[1].TotalPriority)
                    {
                        litigationRequest.IsArbitratorsChairmanDecisionRequired = true;
                        repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                        if (!repositorySaveResponse.IsDone)
                            return repositorySaveResponse;

                        // Send Notification
                        // تم الإنتهاء من تحديد اولوية الاختيار لرئيس هيئة التحكيم من قبل المحكمين ولابد من اتخاذ القرار 

                        try
                        {
                            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                            if (arbitrationAdmin.IsDone)
                            {

                                var msg = "";
                                msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_MakeDecision).Result.Data?.Value;



                                msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
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
                                  arbitrationAdmin.Data?.Email
                                };


                                _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                {
                                    To = emails,
                                    Subject = "اتخاذ قرار",
                                    Body = msg
                                }, null, notifications);


                            }
                        }
                        catch (Exception e)
                        {


                        }
                    }
                    else
                    {
                        LitigationRequestArbitratorAssigningRequest litigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                        litigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                        litigationRequestArbitratorAssigningRequest.Id = Guid.NewGuid();
                        litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
                        litigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                        litigationRequestArbitratorAssigningRequest.CitizenId =
                            litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].ArbitratorChairmanCitizenId;
                        litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                        await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestArbitratorAssigningRequest);
                        repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                        if (!repositorySaveResponse.IsDone)
                            return repositorySaveResponse;

                        // Send Notification
                        //  تم الإنتهاء من تحديد اولوية الاختيار لرئيس هيئة التحكيم من قبل المحكمين و ارسال طلب المهمة للمحكم


                        try
                        {
                            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                            if (arbitrationAdmin.IsDone)
                            {

                                var msg = "";
                                msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmited).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                msg = msg?.Replace("{ArbitratorName}", litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].ArbitratorChairmanCitizen.FullName);



                                List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
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
                                  arbitrationAdmin.Data?.Email
                                };


                                _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                {
                                    To = emails,
                                    Subject = "إعلام بأرسال طلب مهمة",
                                    Body = msg
                                }, null, notifications);


                            }
                        }
                        catch (Exception e)
                        {


                        }
                        // Send Notification
                        //  يوجد طلب مهمه جديد كرئيس هية التحكيم لدعوي رقم

                        try
                        {

                            var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == litigationRequestArbitratorAssigningRequest.CitizenId);


                            var msg = "";
                            msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                        new SysNotificationDto{
                        ActorId = arbitratorInfo.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        },


                        };
                            _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = new List<string>() { arbitratorInfo.Email },
                                Subject = "طلب مهمة جديدة",
                                Body = msg
                            }, null, notifications);

                        }
                        catch (Exception e)
                        {


                        }
                    }


                }
            }

            return await GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(arbitratorNominationListRequestReplyPayload.RequestId);
        }
        #endregion
    }
}
