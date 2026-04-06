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

                if (litigationRequest.IsArbitrationFeesHaveBeenCollected != true)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "يرجي استكمال دفع رسوم التحكيم لهذه الدعوي"
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
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_Added).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }

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
                                Body = body
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
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body;
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorChairman_NewTask_Admin).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    }


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
                        Body = body
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
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body;
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorChairman_NewTask).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    }

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
                        Body = body
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
    // Basic entity properties
    .Include(x => x.Area)
    .Include(x => x.City)
    .Include(x => x.InternalStatus)
    .Include(x => x.ExternalStatus)
    .Include(x => x.DefendantCitizen)
    .Include(x => x.DefendantFacility)

    // Main arbitrator with citizen requests and classes
    .Include(x => x.ArbitratorCitizen)
        .ThenInclude(x => x.CitizenRequests)
        .ThenInclude(x => x.SysArbitratorClass)

    // Arbitrator assigning requests
    .Include(x => x.LitigationRequestArbitratorAssigningRequests)

    // Arbitrator lists (consolidated duplicate)
    .Include(x => x.LitigationRequestArbitratorLists)

    // Chairman nomination lists with complete navigation
    .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
        .ThenInclude(x => x.ArbitratorChairmanCitizen)
        .ThenInclude(x => x.ArbitratorLitigationRequests)
    .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
        .ThenInclude(x => x.ArbitratorChairmanCitizen)
        .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
    .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
        .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)

    // Arbitrator nomination lists with complete navigation
    .Include(x => x.LitigationRequestArbitratorNominationLists)
        .ThenInclude(x => x.ArbitratorCitizen)
        .ThenInclude(x => x.CitizenRequests)
        .ThenInclude(x => x.SysArbitratorClass)
    .Include(x => x.LitigationRequestArbitratorNominationLists)
        .ThenInclude(x => x.ArbitratorCitizen)
        .ThenInclude(x => x.LitigationRequestArbitratorLists)

    // Contract with all parties and type
    .Include(x => x.Contract)
        .ThenInclude(x => x.LitigationRequestContractType)
    .Include(x => x.Contract.FirstPartyFacility)
    .Include(x => x.Contract.FirstPartyCitizen)
    .Include(x => x.Contract.SecondPartyFacility)
    .Include(x => x.Contract.SecondPartyCitizen)

    // Query execution options
    .AsSplitQuery() // This splits into multiple queries to avoid cartesian products
    .AsNoTracking()
    .FirstOrDefault(x => x.Id == requestId);
                if (litigationRequest == null)
                    return new BaseResponse { IsDone = false, Message = "طلب غير موجود" };

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


                        var ChairmanNominationLists = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Where(s => s.IsEscalated == true)
                                .OrderBy(x => x.TotalPriority).ToList();
                        for (int record = 0; record <= ChairmanNominationLists.Count() - 1; record++)
                        {
                            CitizenArbitratorCharmanShortStatusDto citizenArbitratorShortStatusChairmanDto = new CitizenArbitratorCharmanShortStatusDto();
                            citizenArbitratorShortStatusChairmanDto.Id = ChairmanNominationLists[record].ArbitratorChairmanCitizenId;
                            citizenArbitratorShortStatusChairmanDto.FullName = ChairmanNominationLists[record].ArbitratorChairmanCitizen.FullName;


                            citizenArbitratorShortStatusChairmanDto.Mobile = ChairmanNominationLists[record].ArbitratorChairmanCitizen.Mobile;
                            citizenArbitratorShortStatusChairmanDto.Email = ChairmanNominationLists[record].ArbitratorChairmanCitizen.Email;

                            citizenArbitratorShortStatusChairmanDto.ArbitratorClass =
                                ChairmanNominationLists[record].ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;

                            citizenArbitratorShortStatusChairmanDto.Brief =
                               ChairmanNominationLists[record].ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            citizenArbitratorShortStatusChairmanDto.ExperienceYears =
                               ChairmanNominationLists[record].ArbitratorChairmanCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;

                            citizenArbitratorShortStatusChairmanDto.TotalPriority = ChairmanNominationLists[record].LitigationRequestArbitratorsChairmanNominationListValues.Sum(x => x.Priority);

                            citizenArbitratorShortStatusChairmanDto.RequestsCount = ChairmanNominationLists[record].ArbitratorChairmanCitizen?.ArbitratorLitigationRequests.Count() + ChairmanNominationLists[record].ArbitratorChairmanCitizen.LitigationRequestArbitratorLists.Count;

                            citizenArbitratorShortStatusChairmanDto.IdentityNumber = ChairmanNominationLists[record].ArbitratorChairmanCitizen.IdentityNumber;

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
                            .Include(x => x.DefendantCitizen)
                            .Include(x => x.DefendantFacility)
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
                            .Include(x => x.DefendantCitizen)
                            .Include(x => x.DefendantFacility)

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

                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility?.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen?.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
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
    // Filter FIRST before loading related data
    .Where(x => x.Id == requestId
        && x.LitigationRequestArbitratorLists.Any(l => l.ArbitratorCitizenId == referenceId))

    // Basic lookups
    .Include(x => x.Area)
    .Include(x => x.City)
    .Include(x => x.InternalStatus)
    .Include(x => x.ExternalStatus)

    // Arbitrator
    .Include(x => x.ArbitratorCitizen)
        .ThenInclude(x => x.CitizenRequests)
            .ThenInclude(x => x.SysArbitratorClass)

    // Arbitrator Lists
    .Include(x => x.LitigationRequestArbitratorLists)

    // Chairman Nomination Lists (one Include chain per ThenInclude path)
    .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
        .ThenInclude(x => x.ArbitratorChairmanCitizen)
            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
    .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
        .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)

    // Arbitrator Nomination Lists
    .Include(x => x.LitigationRequestArbitratorNominationLists)
        .ThenInclude(x => x.ArbitratorCitizen)
            .ThenInclude(x => x.CitizenRequests)
                .ThenInclude(x => x.SysArbitratorClass)
    .Include(x => x.LitigationRequestArbitratorNominationLists)
        .ThenInclude(x => x.ArbitratorCitizen)
            .ThenInclude(x => x.LitigationRequestArbitratorLists)

    // Contract
    .Include(x => x.Contract)
        .ThenInclude(x => x.LitigationRequestContractType)
    .Include(x => x.Contract)
        .ThenInclude(x => x.FirstPartyFacility)
    .Include(x => x.Contract)
        .ThenInclude(x => x.FirstPartyCitizen)
    .Include(x => x.Contract)
        .ThenInclude(x => x.SecondPartyFacility)
    .Include(x => x.Contract)
        .ThenInclude(x => x.SecondPartyCitizen)

    .AsSplitQuery()     // Split to avoid cartesian explosion
    .AsNoTracking()
    .FirstOrDefault();

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

            litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                                          .Include(x => x.LitigationRequestArbitratorNominationLists)
                                          .Include(x => x.LitigationRequestArbitratorsChairmanNominationLists)
                                          .ThenInclude(x => x.LitigationRequestArbitratorsChairmanNominationListValues)
                                          .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.LitigationRequestArbitratorLists)
                                          .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.ArbitratorLitigationRequests)
                                          .Include(x => x.ApplicantCitizen)
                                          .Where(x => x.Id == arbitratorNominationListRequestReplyPayload.RequestId).FirstOrDefault();

            int? total = (litigationRequest.ArbitratorsCount - 1) * litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Count();
            if (litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists != null)
            {

                litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists.ToList().ForEach(chairmanNominationList =>
                {
                    chairmanNominationList.TotalPriority = chairmanNominationList.LitigationRequestArbitratorsChairmanNominationListValues.Sum(x => x.Priority);
                });
                var valuesCount = litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists
                                                                                                       .SelectMany(s => s.LitigationRequestArbitratorsChairmanNominationListValues)
                                                                                                       .Count() ?? 0;
                if (total == valuesCount)
                {
                    litigationRequest.ArbitratorsChairmanNominationListSelectionDoneDateTime = DateTime.Now;

                    await ProcessArbitratorChairmanSelectionForSingle(litigationRequest, litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists.ToList());

                }
            }

            #region old flow
            //int total = litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.Count * litigationRequest.LitigationRequestArbitratorLists.Count;


            //if (litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists != null)
            //{
            //    litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists.ToList().ForEach(chairmanNominationList =>
            //    {
            //        total -= chairmanNominationList.LitigationRequestArbitratorsChairmanNominationListValues.Count;
            //    });

            //    litigationRequest?.LitigationRequestArbitratorsChairmanNominationLists.ToList().ForEach(chairmanNominationList =>
            //    {
            //        chairmanNominationList.TotalPriority = chairmanNominationList.LitigationRequestArbitratorsChairmanNominationListValues.Sum(x => x.Priority);
            //    });

            //    if (total == 0)
            //    {
            //        litigationRequest.ArbitratorsChairmanNominationListSelectionDoneDateTime = DateTime.Now;

            //        if (litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].TotalPriority == litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[1].TotalPriority)
            //        {
            //            litigationRequest.IsArbitratorsChairmanDecisionRequired = true;
            //            repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            //            if (!repositorySaveResponse.IsDone)
            //                return repositorySaveResponse;

            //            // Send Notification
            //            // تم الإنتهاء من تحديد اولوية الاختيار لرئيس هيئة التحكيم من قبل المحكمين ولابد من اتخاذ القرار 

            //            try
            //            {
            //                var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            //                if (arbitrationAdmin.IsDone)
            //                {

            //                    var msg = "";
            //                    msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_MakeDecision).Result.Data?.Value;
            //                    msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
            //                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
            //                    string body;
            //                    using (StreamReader streamReader = File.OpenText(pathToFile))
            //                    {
            //                        body = await streamReader.ReadToEndAsync();
            //                        body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_MakeDecision).Result.Data?.Value);
            //                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
            //                    }
            //                    List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
            //               new SysNotificationDto{
            //            ActorId = (Guid)arbitrationAdmin.Data?.Id,
            //            RequestId=litigationRequest.Id,
            //            CreatedOn= DateTime.Now,
            //            RequestTypeId=RequestType.LitigationRequest,
            //            ActorTypeId=NotificationActorType.Employee,
            //            Message=msg
            //            }

            //            };
            //                    List<string> emails = new List<string>() {
            //                      arbitrationAdmin.Data?.Email
            //                    };


            //                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
            //                    {
            //                        To = emails,
            //                        Subject = "اتخاذ قرار",
            //                        Body = body
            //                    }, null, notifications);


            //                }
            //            }
            //            catch (Exception e)
            //            {


            //            }
            //        }
            //        else
            //        {
            //            LitigationRequestArbitratorAssigningRequest litigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
            //            litigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
            //            litigationRequestArbitratorAssigningRequest.Id = Guid.NewGuid();
            //            litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
            //            litigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            //            litigationRequestArbitratorAssigningRequest.CitizenId =
            //                litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].ArbitratorChairmanCitizenId;
            //            litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
            //            litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].IsSent = true;
            //            await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestArbitratorAssigningRequest);
            //            repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            //            if (!repositorySaveResponse.IsDone)
            //                return repositorySaveResponse;

            //            // Send Notification
            //            //  تم الإنتهاء من تحديد اولوية الاختيار لرئيس هيئة التحكيم من قبل المحكمين و ارسال طلب المهمة للمحكم


            //            try
            //            {
            //                var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            //                if (arbitrationAdmin.IsDone)
            //                {

            //                    var msg = "";
            //                    msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmited).Result.Data?.Value;
            //                    msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
            //                    msg = msg?.Replace("{ArbitratorName}", litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].ArbitratorChairmanCitizen.FullName);
            //                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
            //                    string body;
            //                    using (StreamReader streamReader = File.OpenText(pathToFile))
            //                    {
            //                        body = await streamReader.ReadToEndAsync();
            //                        body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmited).Result.Data?.Value);
            //                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
            //                        body = body?.Replace("{ArbitratorName}", litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].ArbitratorChairmanCitizen.FullName);
            //                    }

            //                    List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
            //               new SysNotificationDto{
            //            ActorId = (Guid)arbitrationAdmin.Data?.Id,
            //            RequestId=litigationRequest.Id,
            //            CreatedOn= DateTime.Now,
            //            RequestTypeId=RequestType.LitigationRequest,
            //            ActorTypeId=NotificationActorType.Employee,
            //            Message=msg
            //            }

            //            };
            //                    List<string> emails = new List<string>() {
            //                      arbitrationAdmin.Data?.Email
            //                    };


            //                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
            //                    {
            //                        To = emails,
            //                        Subject = "إعلام بأرسال طلب مهمة",
            //                        Body = body
            //                    }, null, notifications);


            //                }
            //            }
            //            catch (Exception e)
            //            {


            //            }
            //            // Send Notification
            //            //  يوجد طلب مهمه جديد كرئيس هية التحكيم لدعوي رقم

            //            try
            //            {

            //                var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == litigationRequestArbitratorAssigningRequest.CitizenId);


            //                var msg = "";
            //                msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value;
            //                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
            //                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
            //                string body;
            //                using (StreamReader streamReader = File.OpenText(pathToFile))
            //                {
            //                    body = await streamReader.ReadToEndAsync();
            //                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value);
            //                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
            //                }

            //                List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
            //            new SysNotificationDto{
            //            ActorId = arbitratorInfo.Id,
            //            RequestId=litigationRequest.Id,
            //            CreatedOn= DateTime.Now,
            //            RequestTypeId=RequestType.LitigationRequest,
            //            ActorTypeId=NotificationActorType.Arbitrator,
            //            Message=msg
            //            },


            //            };
            //                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
            //                {
            //                    To = new List<string>() { arbitratorInfo.Email },
            //                    Subject = "طلب مهمة جديدة",
            //                    Body = body
            //                }, null, notifications);

            //            }
            //            catch (Exception e)
            //            {


            //            }
            //        }


            //}
            //}
            #endregion

            repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;
            return await GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(arbitratorNominationListRequestReplyPayload.RequestId);
        }
        #endregion


        #region Extention method for Add Single Nomination Chairman Assigning Arbitrator
        public async Task<bool> ProcessArbitratorChairmanSelectionForSingle(LitigationRequest litigationRequest, List<LitigationRequestArbitratorsChairmanNominationList>? arbitratorsList)
        {
            // Calculate total priority scores for each arbitrator
            var arbitratorScores = new List<ArbitratorChairmanSelectionScore>();

            foreach (var arbitrator in arbitratorsList)
            {
                var score = new ArbitratorChairmanSelectionScore
                {
                    Arbitrator = arbitrator,
                    TotalScore = arbitrator.TotalPriority,
                    ExperienceYears = arbitrator.ArbitratorChairmanCitizen?.CitizenRequests?
                                                                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears ?? 0,
                    RequestsCount = arbitrator.ArbitratorChairmanCitizen?.ArbitratorLitigationRequests.Count() + arbitrator.ArbitratorChairmanCitizen?.LitigationRequestArbitratorLists.Count,
                    GeographicalPriority = await CompareGeographicalDistanceChairman(arbitrator.ArbitratorChairmanCitizen, litigationRequest)
                };
                arbitratorScores.Add(score);
            }

            // Apply score & weight calculation criteria
            var selectedArbitrator = await ApplySelectionCriteriaChairman(litigationRequest, arbitratorsList, arbitratorScores);

            if (selectedArbitrator != null)
            {
                await AddSingleNominationChairmanAssigningArbitrator(litigationRequest, selectedArbitrator);
                return true;
            }

            return true;
        }

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
            //// Priority: 0 = same area, 1 = different area
            //if (arbitratorCitizenRequest.AreaId == litigationRequest.AreaId)
            //{
            //    return 0; // Same area - highest priority
            //}
            //else
            //{
            //    return 1; // Different area - lowest priority
            //}
        }

        public async Task AddSingleNominationChairmanAssigningArbitrator(LitigationRequest litigationRequest, LitigationRequestArbitratorsChairmanNominationList selectedArbitrator)
        {

            LitigationRequestArbitratorAssigningRequest litigationRequestOneArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.RequestOn = DateTime.Now;
            litigationRequestOneArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
            litigationRequestOneArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault(s => s.CitizenId == selectedArbitrator.ArbitratorChairmanCitizenId)?.LitigationRequestArbitratorNominationListId ?? null;
            litigationRequestOneArbitratorAssigningRequest.CitizenId = selectedArbitrator.ArbitratorChairmanCitizenId;
            litigationRequestOneArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
            await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestOneArbitratorAssigningRequest);
            selectedArbitrator.IsSent = true;

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
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body;
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmited).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                        body = body?.Replace("{ArbitratorName}", litigationRequest.LitigationRequestArbitratorsChairmanNominationLists.OrderBy(x => x.TotalPriority).ToList()[0].ArbitratorChairmanCitizen.FullName);
                    }

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


                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                    {
                        To = emails,
                        Subject = "إعلام بأرسال طلب مهمة",
                        Body = body
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

                var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == litigationRequestOneArbitratorAssigningRequest.CitizenId);


                var msg = "";
                msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value;
                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }

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
                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                {
                    To = new List<string>() { arbitratorInfo.Email },
                    Subject = "طلب مهمة جديدة",
                    Body = body
                }, null, notifications);

            }
            catch (Exception e)
            {


            }


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
                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AssignedAndSent).Result.Data?.Value);
                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                }
                var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
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
    }
}
