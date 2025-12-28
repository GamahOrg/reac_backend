using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using REAC.EntityFrameworkCore.Migrations;
using System.Diagnostics.Eventing.Reader;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Threading;

namespace REAC.Application.ModuleServices
{
    public partial class ArbitrationService
    {
        #region Nomination List
        public async Task<BaseResponse> AddLitigationRequestArbitratorsNominationList(ArbitratorNominationListRequestPayload arbitratorNominationListRequestPayload, CancellationToken cancellationToken)
        {
            try
            {
                if (arbitratorNominationListRequestPayload.Arbitrators.Count == 0)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لابد من إضافة إعضاء هيئة المحكمين"
                    };

                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitratorNominationLists)
                        .Include(x => x.ArbitratorCitizen)
                         .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.FirstPartyCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.SecondPartyCitizen)
                    .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen).
                     FirstOrDefault(x => x.Id == arbitratorNominationListRequestPayload.RequestId);

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
                        if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorListNomination)
                            allowAdd = false;
                    }
                }
                if (litigationRequest.LitigationRequestArbitratorNominationLists.Any())
                    allowAdd = false;

                if (allowAdd)
                {

                    arbitratorNominationListRequestPayload.Arbitrators.ForEach(async t =>
                    {
                        LitigationRequestArbitratorNominationList litigationRequestArbitratorNominationList = new LitigationRequestArbitratorNominationList();
                        litigationRequestArbitratorNominationList.ArbitratorCitizenId = t;
                        litigationRequestArbitratorNominationList.LitigationRequestId = arbitratorNominationListRequestPayload.RequestId;
                        litigationRequestArbitratorNominationList.Id = Guid.NewGuid();
                        await _repository.AddAsync<LitigationRequestArbitratorNominationList, Guid>(litigationRequestArbitratorNominationList);
                    });
                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorListNominationDone;
                    litigationRequest.ArbitratorsNominationListCreationDateTime = DateTime.Now;

                    litigationRequest.ExternalStatusId = RequestStatus.ArbitratorsNominationListIsSent;
                    litigationRequest.InternalStatusId = RequestStatus.ArbitratorsNominationListIsSent;

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم إضافة قائمة تعيين هيئة التحكيم"
                    });

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;

                    // Send Notification //
                    // تم إضافة قائمة تعيين هيئة التحكيم برجاء الاختيار
                    try
                    {
                        var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_Added).Result.Data?.Value;
                        msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        var expirationDateHours = int.Parse(_settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExpirationDateHours).Result.Data?.Value);
                        msg = msg?.Replace("{ExpirationDate}", DateTime.Now.AddHours(expirationDateHours).ToString("MM/dd/yyyy hh:mm tt", new CultureInfo("en-US")));
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        string body;
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_Added).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            body = body?.Replace("{ExpirationDate}", DateTime.Now.AddHours(expirationDateHours).ToString("MM/dd/yyyy hh:mm tt", new CultureInfo("en-US")));
                        }
                        List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                        new SysNotificationDto{
                        ActorId = litigationRequest.ApplicantCitizen.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Plaintiff,
                        Message=msg
                        },
                        };
                        List<string> emails = new List<string>() {
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
                        }
                        ;
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
                        }
                        ;
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
                        }
                        ;
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
                        }
                        ;
                        await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                        {
                            To = emails,
                            Subject = "تم إضافة قائمة تعيين هيئة التحكيم برجاء الاختيار",
                            Body = body
                        }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = recipients,
                            notification_type = "SMS",
                        }, notifications);
                        // Send Notification //
                        // تم إضافة قائمة تعيين هيئة التحكيم
                        var bossMsg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AddedForAdmin).Result.Data?.Value;
                        bossMsg = bossMsg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        string bossBody;
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            bossBody = await streamReader.ReadToEndAsync();
                            bossBody = bossBody?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AddedForAdmin).Result.Data?.Value);
                            bossBody = bossBody?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
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
                        Message=bossMsg
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
                                Subject = "إضافة قائمة تعيين هيئة التحكيم",
                                Body = bossBody
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = bossMsg,
                                recipients = recipientList,
                                notification_type = "SMS",
                            }, notificationList);
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
                        Message = "لا يمكن إضافة عدد المحكمين"
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
            return await GetLitigationRequestForAddArbitratorsNominationList(arbitratorNominationListRequestPayload.RequestId, cancellationToken);
        }
        public async Task<BaseResponse> GetLitigationRequestForAddArbitratorsNominationList(Guid requestId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                            .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.Area).Include(x => x.City)

                            .Include(x => x.LitigationRequestCategory)
                            .Include(x => x.ArbitratorCitizen)
                            .Include(x => x.DefendantCitizen)
                            .Include(x => x.DefendantFacility)
                            .Include(x => x.InternalStatus)
                            .Include(x => x.ExternalStatus)

                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)
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
                            .Where(x => x.Id == requestId)
                            .AsSplitQuery()
                            .AsNoTracking().FirstOrDefault();

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
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorListNomination)
                        allowAdd = false;
                }
            }
            LitigationRequestForAllArbitratorNominationListDto litigationRequestForAllArbitratorNominationListDto = new LitigationRequestForAllArbitratorNominationListDto();
            litigationRequestForAllArbitratorNominationListDto.LitigationRequestShortDto = request;
            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos = new List<CitizenArbitratorShortStatusDto>();

            if (litigationRequest.LitigationRequestArbitratorNominationLists.Any())
            {
                //// case : we have one arbitrator will take at lease 2 arbitrator which isEscalated (min priortiy)
                if (litigationRequest.LitigationRequestArbitratorNominationLists.Count(x => x.IsEscalated == true && x.IsSent != true) > 1 && !litigationRequest.LitigationRequestArbitratorNominationLists.Any(x => x.IsResponse != true && x.IsSent == true) && (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue & litigationRequest.IsDefendantArbitratorNominationListDone.HasValue))
                {
                    litigationRequestForAllArbitratorNominationListDto.AllowDecision = true;
                    foreach (var litigationRequestArbitratorNominationList in litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsEscalated == true && x.IsSent != true))
                    {
                        CitizenArbitratorShortStatusDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusDto();
                        citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                        citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                        citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                        citizenArbitratorShortStatusDto.PlaintiffPriority = litigationRequestArbitratorNominationList.PlaintiffPriority;
                        citizenArbitratorShortStatusDto.DefendantPriority = litigationRequestArbitratorNominationList.DefendantPriority;
                        citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                        citizenArbitratorShortStatusDto.TotalPriority = litigationRequestArbitratorNominationList.PlaintiffPriority + litigationRequestArbitratorNominationList.DefendantPriority;


                        citizenArbitratorShortStatusDto.ArbitratorClass =
                            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;

                        citizenArbitratorShortStatusDto.Brief =
                            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                        citizenArbitratorShortStatusDto.ExperienceYears =
                            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                        citizenArbitratorShortStatusDto.RequestsCount = litigationRequestArbitratorNominationList.ArbitratorCitizen?.ArbitratorLitigationRequests.Count() + litigationRequestArbitratorNominationList.ArbitratorCitizen.LitigationRequestArbitratorLists.Count;
                        citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;

                        var assingingRequest = litigationRequest.LitigationRequestArbitratorAssigningRequests.Where(t => t.LitigationRequestId == litigationRequest.Id
                                                                & t.CitizenId == litigationRequestArbitratorNominationList.ArbitratorCitizenId & t.ArbitratorAssigningRequestTypeId == ArbitratorAssigningRequestType.Arbitrator);

                        if (assingingRequest != null)
                        {
                            if (assingingRequest.Any())
                            {
                                if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve)
                                    citizenArbitratorShortStatusDto.Status = "تم الموافقة";
                                else
                                {
                                    if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject)
                                        citizenArbitratorShortStatusDto.Status = "تم الرفض";
                                    else
                                    {
                                        if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)
                                            citizenArbitratorShortStatusDto.Status = "تم الإلغاء";
                                        else
                                        {
                                            citizenArbitratorShortStatusDto.Status = "قيد الرد";
                                        }
                                    }
                                }
                            }
                        }
                        litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    }

                }
                else
                {
                    allowAdd = false;
                    foreach (var litigationRequestArbitratorNominationList in litigationRequest.LitigationRequestArbitratorNominationLists)
                    {
                        CitizenArbitratorShortStatusDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusDto();
                        citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                        citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                        citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                        citizenArbitratorShortStatusDto.PlaintiffPriority = litigationRequestArbitratorNominationList.PlaintiffPriority;
                        citizenArbitratorShortStatusDto.DefendantPriority = litigationRequestArbitratorNominationList.DefendantPriority;

                        citizenArbitratorShortStatusDto.ArbitratorClass =
                            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;

                        citizenArbitratorShortStatusDto.Brief =
                            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                        citizenArbitratorShortStatusDto.ExperienceYears =
                            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                            .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                        citizenArbitratorShortStatusDto.RequestsCount = litigationRequestArbitratorNominationList.ArbitratorCitizen?.ArbitratorLitigationRequests.Count() + litigationRequestArbitratorNominationList.ArbitratorCitizen.LitigationRequestArbitratorLists.Count;
                        citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;

                        var assingingRequest = litigationRequest.LitigationRequestArbitratorAssigningRequests.Where(t => t.LitigationRequestId == litigationRequest.Id
                                                                & t.CitizenId == litigationRequestArbitratorNominationList.ArbitratorCitizenId & t.ArbitratorAssigningRequestTypeId == ArbitratorAssigningRequestType.Arbitrator);

                        if (assingingRequest != null)
                        {
                            if (assingingRequest.Any())
                            {
                                if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve)
                                    citizenArbitratorShortStatusDto.Status = "تم الموافقة";
                                else
                                {
                                    if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject)
                                        citizenArbitratorShortStatusDto.Status = "تم الرفض";
                                    else
                                    {
                                        if (assingingRequest.LastOrDefault()?.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)
                                            citizenArbitratorShortStatusDto.Status = "تم الإلغاء";
                                        else
                                        {
                                            citizenArbitratorShortStatusDto.Status = "قيد الرد";
                                        }
                                    }
                                }
                            }
                        }
                        litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    }

                }
                return new DataResponse<LitigationRequestForAllArbitratorNominationListDto>()
                {
                    Data = litigationRequestForAllArbitratorNominationListDto,
                    IsDone = true,
                };
            }

            return new DataResponse<LitigationRequestForAllArbitratorNominationListDto>()
            {
                Data = new LitigationRequestForAllArbitratorNominationListDto()
                {
                    ListCount = (await _repository.GetQueryable<SysArbitratorNominationListCount, Guid>()).Where(x => x.ArbitratorCount == litigationRequest.ArbitratorsCount).FirstOrDefault()?.ArbitratorNominationListCount,
                    AllowAdd = allowAdd,
                    LitigationRequestShortDto = request
                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForAddArbitratorsNominationList(int pageIndex, int pageSize, CancellationToken cancellationToken)
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
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)
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


            litigationRequests = entitiesQuerable.Where(x => x.RequestCode != null & x.ArbitratorsCount.HasValue
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
                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else
                        {

                            if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorListNomination)
                            {
                                allowAdd = false;
                            }
                        }
                    }
                    if (litigationRequest.LitigationRequestArbitratorNominationLists.Count > 0)
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

        public async Task<BaseResponse> GetDueDateLitigationRequestForAssignArbitrators(int pageIndex, int pageSize, CancellationToken cancellationToken)
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

                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .Include(x => x.LitigationRequestArbitratorLists)

                            .Include(x => x.Contract)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .Include(x => x.Contract.FirstPartyFacility)
                            .Include(x => x.Contract.FirstPartyCitizen)
                            .Include(x => x.Contract.SecondPartyFacility)
                            .Include(x => x.Contract.SecondPartyCitizen)
                            .Include(x => x.Contract.LitigationRequestContractType);


            litigationRequests = entitiesQuerable.Where(x =>
            x.RequestCode != null
            && x.ArbitratorsCount.HasValue
            && x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
            && x.LitigationRequestStage == LitigationRequestStage.Arbitration
            && x.IsArbitratorNominationListDecisionPeriodSent.HasValue
            && x.IsArbitratorNominationListDecisionPeriodSent == true
            && !x.LitigationRequestArbitratorLists.Any()
            && (
            // Case 1: Only one IsDefendantSelected == true, all others IsPlaintiffSelected == null
             (
                 x.LitigationRequestArbitratorNominationLists.Count(x => x.IsDefendantSelected == true) >= 1 &&
                 x.LitigationRequestArbitratorNominationLists.All(x => x.IsPlaintiffSelected == null /*&& x.IsDefendantSelected == true*/)
             )
             ||
             //// Case 2: Only one IsPlaintiffSelected == true, all others IsDefendantSelected == null
             (
                 x.LitigationRequestArbitratorNominationLists.Count(x => x.IsPlaintiffSelected == true) >= 1 &&
                 x.LitigationRequestArbitratorNominationLists.All(x => x.IsDefendantSelected == null /*&& x.IsPlaintiffSelected == true*/)
             )
             ||
             (
             x.LitigationRequestArbitratorNominationLists.Count(x => x.IsDefendantSelected == true) == 0 && x.LitigationRequestArbitratorNominationLists.Count(x => x.IsPlaintiffSelected == true) == 0
             ))

            )
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
                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else
                        {

                            if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorListNomination)
                            {
                                allowAdd = false;
                            }
                        }
                    }
                    if (litigationRequest.LitigationRequestArbitratorNominationLists.Count > 0)
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


        public async Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsNominationListReply(int pageIndex, int pageSize, CancellationToken cancellationToken)
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
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)
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


            litigationRequests = entitiesQuerable.Where(x => x.RequestCode != null & x.ArbitratorsCount.HasValue
                                      & x.ApplicantCitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitratorNominationLists.Count > 0
                                      //| x.RequestCode != null & x.ArbitratorsCount.HasValue
                                      //& x.Contract.FirstPartyCitizenId == referenceId
                                      //& x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      //& x.LitigationRequestArbitratorNominationLists.Count > 0
                                      | x.RequestCode != null & x.ArbitratorsCount.HasValue
                                      & x.Contract.SecondPartyCitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitratorNominationLists.Count > 0
                                      | x.RequestCode != null & x.ArbitratorsCount.HasValue
                                      & x.Contract.FirstPartyFacility.CitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitratorNominationLists.Count > 0
                                      | x.RequestCode != null & x.ArbitratorsCount.HasValue
                                      & x.Contract.SecondPartyFacility.CitizenId == referenceId
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

                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue)
                            allowAdd = false;

                    }
                    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue)
                                allowAdd = false;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            if (litigationRequest.IsDefendantArbitratorNominationListDone.HasValue)
                                allowAdd = false;
                        }
                    }
                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                        {
                            if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue)
                                allowAdd = false;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                        {
                            if (litigationRequest.IsDefendantArbitratorNominationListDone.HasValue)
                                allowAdd = false;
                        }
                    }

                    LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDto.ArbitratorsCount = litigationRequest.ArbitratorsCount;

                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.DefendantCitizen == null) ? litigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.DefendantCitizen?.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;

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

        public async Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsNominationListReply(Guid requestId, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                            .Include(x => x.LitigationRequestCategory)
                            .Include(x => x.ArbitratorCitizen)
                            .Include(x => x.DefendantCitizen.Nationality)
                            .Include(x => x.DefendantFacility)
                            .Include(x => x.InternalStatus)
                            .Include(x => x.ExternalStatus)
                            .Include(x => x.City)
                            .Include(x => x.Area)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.CitizenRequests)
                            .ThenInclude(x => x.SysArbitratorClass)

                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.Contract)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .Include(x => x.Contract.FirstPartyFacility)
                            .Include(x => x.Contract.FirstPartyCitizen)
                            .Include(x => x.Contract.SecondPartyFacility)
                            .Include(x => x.Contract.SecondPartyCitizen)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .AsNoTracking()
                            .AsSplitQuery()
                            .FirstOrDefault(x => x.Id == requestId
                            &&
                x.ApplicantCitizenId == referenceId
                      //|
                      //x.Contract.FirstPartyCitizenId == referenceId & x.Id == requestId
                      |
                        x.Contract.SecondPartyCitizenId == referenceId & x.Id == requestId
                      |
                       x.Contract.FirstPartyFacility.CitizenId == referenceId & x.Id == requestId
                      |
                      x.Contract.SecondPartyFacility.CitizenId == referenceId & x.Id == requestId
                            );

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
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorListNominationDone)
                        allowAdd = false;

                    else if (litigationRequest?.ArbitratorsCount > 1)
                    {
                        if (litigationRequest.ApplicantCitizenId == referenceId)
                        {
                            if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true)
                                allowAdd = false;
                        }
                        if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                        {
                            if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                            {
                                if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true)
                                    allowAdd = false;
                            }
                        }
                        if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                        {
                            if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                            {
                                if (litigationRequest.IsDefendantArbitratorNominationListDone.HasValue && litigationRequest.IsDefendantArbitratorNominationListDone == true)
                                    allowAdd = false;
                            }
                        }
                        if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                        {
                            if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                            {
                                if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true)
                                    allowAdd = false;
                            }
                        }
                        if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                        {
                            if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                            {
                                if (litigationRequest.IsDefendantArbitratorNominationListDone.HasValue && litigationRequest.IsDefendantArbitratorNominationListDone == true)
                                    allowAdd = false;
                            }
                        }
                    }

                }

            }

            LitigationRequestForAllArbitratorNominationListWithPartiesValueDto litigationRequestForAllArbitratorNominationListDto = new LitigationRequestForAllArbitratorNominationListWithPartiesValueDto();
            litigationRequestForAllArbitratorNominationListDto.LitigationRequestShortDto = request;
            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos = new List<CitizenArbitratorShortStatusWithValueDto>();

            if (litigationRequest.LitigationRequestArbitratorNominationLists.Any())
            {
                if (litigationRequest?.ArbitratorsCount > 1)
                {
                    if (allowAdd)
                    {
                        var nominationArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsArbitratorAdminSelected != true && x.IsResponse == null && x.IsDefendantSelected != true && x.IsPlaintiffSelected != true);
                        var RejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsArbitratorAdminSelected != true && x.LitigationRequestArbitratorAssigningRequests.Any(s => s.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject || s.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)).ToList();
                        var rejectedIds = new HashSet<Guid>(RejectedArbitrators.Select(x => x.Id));
                        var selectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => ((x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) || (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true)) && !rejectedIds.Contains(x.Id));
                        var selectedUserArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => ((x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) || (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true)) && !rejectedIds.Contains(x.Id));
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            selectedArbitrators = selectedArbitrators.Where(x => (x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) && !rejectedIds.Contains(x.Id));
                            selectedUserArbitrators = selectedUserArbitrators.Where(x => (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true) && !rejectedIds.Contains(x.Id));
                        }
                        else
                        {
                            selectedArbitrators = selectedArbitrators.Where(x => (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true) && !rejectedIds.Contains(x.Id));
                            selectedUserArbitrators = selectedUserArbitrators.Where(x => (x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) && !rejectedIds.Contains(x.Id));
                        }
                        foreach (var litigationRequestArbitratorNominationList in nominationArbitrators)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                            citizenArbitratorShortStatusDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                            citizenArbitratorShortStatusDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                            citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                            citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                            citizenArbitratorShortStatusDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                            }
                            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                        }
                        foreach (var litigationRequestArbitratorNominationList in RejectedArbitrators)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                            citizenArbitratorShortStatusDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                            citizenArbitratorShortStatusDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                            citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                            citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                            citizenArbitratorShortStatusDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                            }
                            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            litigationRequestForAllArbitratorNominationListDto.CitizenRejectedArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                        }
                        foreach (var litigationRequestArbitratorNominationList in selectedArbitrators)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                            citizenArbitratorShortStatusDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                            citizenArbitratorShortStatusDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;


                            citizenArbitratorShortStatusDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                            }
                            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            litigationRequestForAllArbitratorNominationListDto.CitizenSelectedArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                        }
                        foreach (var litigationRequestArbitratorNominationList in selectedUserArbitrators)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                            citizenArbitratorShortStatusDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                            citizenArbitratorShortStatusDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                            citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                            citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                            citizenArbitratorShortStatusDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                            }
                            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            litigationRequestForAllArbitratorNominationListDto.CitizenSelectedUserArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                        }
                    }
                    else
                    {
                        var RejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.LitigationRequestArbitratorAssigningRequests.Any(s => s.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject || s.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)).ToList();
                        var rejectedIds = new HashSet<Guid>(RejectedArbitrators.Select(x => x.Id));
                        var LitigationRequestArbitratorNominationLists = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => (x.IsDefendantSelected == true || x.IsPlaintiffSelected == true) && !rejectedIds.Contains(x.Id)).ToList();
                        //if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        //{
                        //    LitigationRequestArbitratorNominationLists = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsPlaintiffSelected == true).ToList();
                        //}
                        //else
                        //{
                        //    LitigationRequestArbitratorNominationLists = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsDefendantSelected == true).ToList();
                        //}

                        foreach (var litigationRequestArbitratorNominationList in LitigationRequestArbitratorNominationLists)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                            citizenArbitratorShortStatusDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                            citizenArbitratorShortStatusDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                            citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                            citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                            citizenArbitratorShortStatusDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                            }
                            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                        }
                        foreach (var litigationRequestArbitratorNominationList in RejectedArbitrators)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                            citizenArbitratorShortStatusDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                            citizenArbitratorShortStatusDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                            citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                            citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                            citizenArbitratorShortStatusDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                            }
                            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            litigationRequestForAllArbitratorNominationListDto.CitizenRejectedArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                        }

                    }
                }
                else
                {
                    foreach (var litigationRequestArbitratorNominationList in litigationRequest.LitigationRequestArbitratorNominationLists)
                    {
                        if (allowAdd)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                            citizenArbitratorShortStatusDto.ArbitratorClass =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                            citizenArbitratorShortStatusDto.ExperienceYears =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;


                            citizenArbitratorShortStatusDto.Brief =
                                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            }

                            //if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            //{
                            //    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                            //}
                            //if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            //{
                            //    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                            //}

                            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                        }
                        else
                        {
                            if (litigationRequest.LitigationRequestArbitratorLists.Count(x => x.ArbitratorCitizenId == litigationRequestArbitratorNominationList.ArbitratorCitizenId) > 0)
                            {
                                CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                                citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                                citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                                citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                                citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                                citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                                citizenArbitratorShortStatusDto.ArbitratorClass =
                                    litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                                citizenArbitratorShortStatusDto.ExperienceYears =
                                    litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                                citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;


                                citizenArbitratorShortStatusDto.Brief =
                                    litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                                if (litigationRequest.ApplicantCitizenId == referenceId)
                                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                                if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                                {
                                    if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                                }
                                if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                                {
                                    if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                                }

                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                {
                                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                                }
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                {
                                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                                }
                                litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                            }
                        }
                    }
                }
                if ((litigationRequest.IsDefendantArbitratorNominationListDone.HasValue && litigationRequest.IsDefendantArbitratorNominationListDone == true) && (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true))
                {
                    allowAdd = false;
                }
                litigationRequestForAllArbitratorNominationListDto.AllowAdd = allowAdd;
                return new DataResponse<LitigationRequestForAllArbitratorNominationListWithPartiesValueDto>()
                {
                    Data = litigationRequestForAllArbitratorNominationListDto,
                    IsDone = true,
                };
            }

            return new BaseResponse()
            {
                IsDone = false,
                Message = "خطأ ما يرجى الرجوع لمدير النظام"
            };

        }

        public async Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsNominationListReplyForCoAdmin(Guid requestId, CancellationToken cancellationToken)
        {
            //Guid referenceId = Guid.Empty;
            //_jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                            .Include(x => x.LitigationRequestCategory)
                            .Include(x => x.ArbitratorCitizen)
                            .Include(x => x.DefendantCitizen.Nationality)
                            .Include(x => x.DefendantFacility)
                            .Include(x => x.InternalStatus)
                            .Include(x => x.ExternalStatus)
                            .Include(x => x.City)
                            .Include(x => x.Area)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.ArbitratorLitigationRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.CitizenRequests)
                            .ThenInclude(x => x.SysArbitratorClass)

                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.ArbitratorCitizen)
                            .ThenInclude(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.LitigationRequestArbitratorNominationLists)
                            .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                            .Include(x => x.LitigationRequestArbitratorLists)
                            .Include(x => x.Contract)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .Include(x => x.Contract.FirstPartyFacility)
                            .Include(x => x.Contract.FirstPartyCitizen)
                            .Include(x => x.Contract.SecondPartyFacility)
                            .Include(x => x.Contract.SecondPartyCitizen)
                            .Include(x => x.Contract.LitigationRequestContractType)
                            .AsSplitQuery()
                            .AsNoTracking()
                            .FirstOrDefault(x => x.Id == requestId);

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };
            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            //bool isDone = false;
            //bool allowAdd = true;

            //if (litigationRequest.IsFinished.HasValue)
            //{
            //    if (litigationRequest.IsFinished.Value)
            //    {
            //        allowAdd = false;
            //        isDone = true;
            //    }
            //}
            //if (!isDone)
            //{
            //    if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
            //    {
            //        allowAdd = false;
            //    }
            //    else
            //    {
            //        if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorListNominationDone)
            //            allowAdd = false;

            //        else if (litigationRequest?.ArbitratorsCount > 1)
            //        {
            //            if (litigationRequest.ApplicantCitizenId == referenceId)
            //            {
            //                if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true)
            //                    allowAdd = false;
            //            }
            //            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
            //            {
            //                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
            //                {
            //                    if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true)
            //                        allowAdd = false;
            //                }
            //            }
            //            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
            //            {
            //                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
            //                {
            //                    if (litigationRequest.IsDefendantArbitratorNominationListDone.HasValue && litigationRequest.IsDefendantArbitratorNominationListDone == true)
            //                        allowAdd = false;
            //                }
            //            }
            //            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
            //            {
            //                if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
            //                {
            //                    if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true)
            //                        allowAdd = false;
            //                }
            //            }
            //            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
            //            {
            //                if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
            //                {
            //                    if (litigationRequest.IsDefendantArbitratorNominationListDone.HasValue && litigationRequest.IsDefendantArbitratorNominationListDone == true)
            //                        allowAdd = false;
            //                }
            //            }
            //        }

            //    }

            //}

            LitigationRequestForAllArbitratorNominationListWithPartiesValueDto litigationRequestForAllArbitratorNominationListDto = new LitigationRequestForAllArbitratorNominationListWithPartiesValueDto();
            litigationRequestForAllArbitratorNominationListDto.LitigationRequestShortDto = request;
            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos = new List<CitizenArbitratorShortStatusWithValueDto>();

            if (litigationRequest.LitigationRequestArbitratorNominationLists.Any())
            {
                //if (litigationRequest?.ArbitratorsCount > 1)
                //{
                //    if (allowAdd)
                //    {
                //var nominationArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsDefendantSelected != true && x.IsPlaintiffSelected != true);
                var nominationArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists;
                var RejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.LitigationRequestArbitratorAssigningRequests.Any(s => s.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject || s.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)).ToList();
                var rejectedIds = new HashSet<Guid>(RejectedArbitrators.Select(x => x.Id));
                var selectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => ((x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) || (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true)) && !rejectedIds.Contains(x.Id));
                //var selectedUserArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => ((x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) || (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true)) && !rejectedIds.Contains(x.Id));
                //if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                //{
                //    selectedArbitrators = selectedArbitrators.Where(x => (x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) && !rejectedIds.Contains(x.Id));
                //    selectedUserArbitrators = selectedUserArbitrators.Where(x => (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true) && !rejectedIds.Contains(x.Id));
                //}
                //else
                //{
                //    selectedArbitrators = selectedArbitrators.Where(x => (x.IsPlaintiffSelected.HasValue && x.IsPlaintiffSelected.Value == true) && !rejectedIds.Contains(x.Id));
                //    selectedUserArbitrators = selectedUserArbitrators.Where(x => (x.IsDefendantSelected.HasValue && x.IsDefendantSelected.Value == true) && !rejectedIds.Contains(x.Id));
                //}
                foreach (var litigationRequestArbitratorNominationList in nominationArbitrators)
                {
                    CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                    citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                    citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                    citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                    citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                    citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                    citizenArbitratorShortStatusDto.ArbitratorClass =
                        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                        .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                    citizenArbitratorShortStatusDto.ExperienceYears =
                        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                        .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                    citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                    citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                    citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;
                    citizenArbitratorShortStatusDto.IsArbitratorAdminSelected = litigationRequestArbitratorNominationList.IsArbitratorAdminSelected;
                    citizenArbitratorShortStatusDto.IsSent = litigationRequestArbitratorNominationList.IsSent;

                    citizenArbitratorShortStatusDto.Brief =
                        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                        .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    //if (litigationRequest.ApplicantCitizenId == referenceId)
                    //    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                    //if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    //{
                    //    if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                    //}
                    //if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    //{
                    //    if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //}

                    //if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //{
                    //    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                    //}
                    //if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //{
                    //    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //}

                    litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    // }
                    //foreach (var litigationRequestArbitratorNominationList in RejectedArbitrators)
                    //{
                    //    CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                    //    citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                    //    citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                    //    citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                    //    citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                    //    citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                    //    citizenArbitratorShortStatusDto.ArbitratorClass =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                    //    citizenArbitratorShortStatusDto.ExperienceYears =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                    //    citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                    //    citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                    //    citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                    //    citizenArbitratorShortStatusDto.Brief =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    //    if (litigationRequest.ApplicantCitizenId == referenceId)
                    //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                    //    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                    //    }
                    //    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //    }

                    //    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                    //    }
                    //    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //    }

                    //    litigationRequestForAllArbitratorNominationListDto.CitizenRejectedArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    //}
                    //foreach (var litigationRequestArbitratorNominationList in selectedArbitrators)
                    //{
                    //    CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                    //    citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                    //    citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                    //    citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                    //    citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                    //    citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                    //    citizenArbitratorShortStatusDto.ArbitratorClass =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                    //    citizenArbitratorShortStatusDto.ExperienceYears =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                    //    citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;


                    //    citizenArbitratorShortStatusDto.Brief =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    //    if (litigationRequest.ApplicantCitizenId == referenceId)
                    //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                    //    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                    //    }
                    //    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //    }

                    //    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                    //    }
                    //    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //    }

                    //    litigationRequestForAllArbitratorNominationListDto.CitizenSelectedArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    //}
                    //foreach (var litigationRequestArbitratorNominationList in selectedUserArbitrators)
                    //{
                    //    CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                    //    citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                    //    citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                    //    citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                    //    citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                    //    citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                    //    citizenArbitratorShortStatusDto.ArbitratorClass =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                    //    citizenArbitratorShortStatusDto.ExperienceYears =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                    //    citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                    //    citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                    //    citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                    //    citizenArbitratorShortStatusDto.Brief =
                    //        litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //        .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //        .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    //    if (litigationRequest.ApplicantCitizenId == referenceId)
                    //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                    //    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                    //    }
                    //    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //    }

                    //    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                    //    }
                    //    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //    {
                    //        if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //    }

                    //    litigationRequestForAllArbitratorNominationListDto.CitizenSelectedUserArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    //}
                    //}
                    //else
                    //{
                    //    var RejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.LitigationRequestArbitratorAssigningRequests.Any(s => s.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject || s.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)).ToList();
                    //    var rejectedIds = new HashSet<Guid>(RejectedArbitrators.Select(x => x.Id));
                    //    var LitigationRequestArbitratorNominationLists = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => (x.IsDefendantSelected == true || x.IsPlaintiffSelected == true) && !rejectedIds.Contains(x.Id)).ToList();
                    //    //if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    //    //{
                    //    //    LitigationRequestArbitratorNominationLists = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsPlaintiffSelected == true).ToList();
                    //    //}
                    //    //else
                    //    //{
                    //    //    LitigationRequestArbitratorNominationLists = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsDefendantSelected == true).ToList();
                    //    //}

                    //    foreach (var litigationRequestArbitratorNominationList in LitigationRequestArbitratorNominationLists)
                    //    {
                    //        CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                    //        citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                    //        citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                    //        citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                    //        citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                    //        citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                    //        citizenArbitratorShortStatusDto.ArbitratorClass =
                    //            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //            .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                    //        citizenArbitratorShortStatusDto.ExperienceYears =
                    //            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //            .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                    //        citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                    //        citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                    //        citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                    //        citizenArbitratorShortStatusDto.Brief =
                    //            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //            .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    //        if (litigationRequest.ApplicantCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                    //        if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                    //        }
                    //        if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //        }

                    //        if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                    //        }
                    //        if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //        }

                    //        litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    //    }
                    //    foreach (var litigationRequestArbitratorNominationList in RejectedArbitrators)
                    //    {
                    //        CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                    //        citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                    //        citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                    //        citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                    //        citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                    //        citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                    //        citizenArbitratorShortStatusDto.ArbitratorClass =
                    //            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //            .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                    //        citizenArbitratorShortStatusDto.ExperienceYears =
                    //            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //            .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                    //        citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;
                    //        citizenArbitratorShortStatusDto.IsPlaintiffSelected = litigationRequestArbitratorNominationList.IsPlaintiffSelected;
                    //        citizenArbitratorShortStatusDto.IsDefendantSelected = litigationRequestArbitratorNominationList.IsDefendantSelected;

                    //        citizenArbitratorShortStatusDto.Brief =
                    //            litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                    //            .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                    //            .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                    //        if (litigationRequest.ApplicantCitizenId == referenceId)
                    //            citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                    //        if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                    //        }
                    //        if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //        }

                    //        if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                    //        }
                    //        if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //        {
                    //            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                    //        }

                    //        litigationRequestForAllArbitratorNominationListDto.CitizenRejectedArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                    //    }

                    //}
                }
                //else
                //{
                //    foreach (var litigationRequestArbitratorNominationList in litigationRequest.LitigationRequestArbitratorNominationLists)
                //    {
                //        if (allowAdd)
                //        {
                //            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                //            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                //            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                //            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                //            citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                //            citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                //            citizenArbitratorShortStatusDto.ArbitratorClass =
                //                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                //                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                //                .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                //            citizenArbitratorShortStatusDto.ExperienceYears =
                //                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                //                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                //                .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                //            citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;


                //            citizenArbitratorShortStatusDto.Brief =
                //                litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                //                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                //                .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                //            if (litigationRequest.ApplicantCitizenId == referenceId)
                //                citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                //            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                //            {
                //                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                //                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                //            }
                //            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                //            {
                //                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                //                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                //            }

                //            //if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                //            //{
                //            //    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                //            //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                //            //}
                //            //if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                //            //{
                //            //    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                //            //        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                //            //}

                //            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                //        }
                //        else
                //        {
                //            if (litigationRequest.LitigationRequestArbitratorLists.Count(x => x.ArbitratorCitizenId == litigationRequestArbitratorNominationList.ArbitratorCitizenId) > 0)
                //            {
                //                CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                //                citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                //                citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                //                citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;
                //                citizenArbitratorShortStatusDto.Mobile = litigationRequestArbitratorNominationList.ArbitratorCitizen.Mobile;
                //                citizenArbitratorShortStatusDto.Email = litigationRequestArbitratorNominationList.ArbitratorCitizen.Email;
                //                citizenArbitratorShortStatusDto.ArbitratorClass =
                //                    litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                //                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                //                    .OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                //                citizenArbitratorShortStatusDto.ExperienceYears =
                //                    litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                //                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                //                    .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                //                citizenArbitratorShortStatusDto.IdentityNumber = litigationRequestArbitratorNominationList.ArbitratorCitizen.IdentityNumber;


                //                citizenArbitratorShortStatusDto.Brief =
                //                    litigationRequestArbitratorNominationList.ArbitratorCitizen?.CitizenRequests?
                //                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                //                    .OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;

                //                if (litigationRequest.ApplicantCitizenId == referenceId)
                //                    citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;


                //                if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                //                {
                //                    if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                //                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;
                //                }
                //                if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                //                {
                //                    if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                //                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                //                }

                //                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                //                {
                //                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                //                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.PlaintiffPriority;

                //                }
                //                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                //                {
                //                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                //                        citizenArbitratorShortStatusDto.Value = litigationRequestArbitratorNominationList.DefendantPriority;
                //                }
                //                litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos.Add(citizenArbitratorShortStatusDto);
                //            }
                //        }
                //    }
                //}
                //if ((litigationRequest.IsDefendantArbitratorNominationListDone.HasValue && litigationRequest.IsDefendantArbitratorNominationListDone == true) && (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue && litigationRequest.IsPlaintiffArbitratorNominationListDone == true))
                //{
                //    allowAdd = false;
                //}
                litigationRequestForAllArbitratorNominationListDto.AllowAdd = true;
                return new DataResponse<LitigationRequestForAllArbitratorNominationListWithPartiesValueDto>()
                {
                    Data = litigationRequestForAllArbitratorNominationListDto,
                    IsDone = true,
                };
            }

            return new BaseResponse()
            {
                IsDone = false,
                Message = "خطأ ما يرجى الرجوع لمدير النظام"
            };

        }

        public async Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsNominationListReply(ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.CitizenRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.LitigationRequestArbitratorLists)
                             .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.ArbitratorLitigationRequests)
                             .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                          .Include(x => x.ArbitratorCitizen)
                          .ThenInclude(x => x.CitizenRequests)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.SecondPartyCitizen)
                           .Include(x => x.LitigationRequestArbitratorLists)
                          .FirstOrDefault(x => x.Id == arbitratorNominationListRequestReplyPayload.RequestId);

            bool isDone = false;
            LitigationRequestParty litigationRequestParty = LitigationRequestParty.None;
            if (litigationRequest != null)
            {
                if (litigationRequest.ApplicantCitizen != null)
                {
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                        litigationRequestParty = LitigationRequestParty.Plaintiff;
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.FirstPartyCitizen != null)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                            litigationRequestParty = LitigationRequestParty.Plaintiff;
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.SecondPartyCitizen != null)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                            litigationRequestParty = LitigationRequestParty.Defendant;
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.FirstPartyFacility != null)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility.CitizenId == referenceId)
                            litigationRequestParty = LitigationRequestParty.Plaintiff;
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.SecondPartyFacility != null)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility.CitizenId == referenceId)
                            litigationRequestParty = LitigationRequestParty.Defendant;
                    }
                }
            }

            if (litigationRequest?.LitigationRequestArbitratorNominationLists != null)
            {
                switch (litigationRequestParty)
                {
                    case LitigationRequestParty.Plaintiff:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إضافة أولوية إختيار قائمة تعيين هيئة التحكيم من قبل المدعى"
                        });
                        litigationRequest.IsPlaintiffArbitratorNominationListDone = true;
                        break;
                    case LitigationRequestParty.Defendant:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إضافة أولوية إختيار قائمة تعيين هيئة التحكيم من قبل المدعى عليه"
                        });
                        litigationRequest.IsDefendantArbitratorNominationListDone = true;
                        break;
                }
                if (litigationRequest.ArbitratorsCount == 1)
                {
                    foreach (var item in litigationRequest?.LitigationRequestArbitratorNominationLists.ToList())
                    {
                        switch (litigationRequestParty)
                        {
                            case LitigationRequestParty.Plaintiff:
                                item.PlaintiffPriority = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems.Where(x => x.ArbitratorListId == item.Id).FirstOrDefault()?.Priority;
                                if (item.PlaintiffPriority.HasValue)
                                    item.TotalPriority = item.PlaintiffPriority + item.DefendantPriority;
                                break;
                            case LitigationRequestParty.Defendant:
                                item.DefendantPriority = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems.Where(x => x.ArbitratorListId == item.Id).FirstOrDefault()?.Priority;
                                if (item.DefendantPriority.HasValue)
                                    item.TotalPriority = item.DefendantPriority + item.PlaintiffPriority;
                                break;
                        }
                    }
                }
                //handle the selected arbatrator one for multiple arbitrator 
                if (litigationRequest.ArbitratorsCount > 1)
                {
                    var Counter = 0;
                    //get the selected one and update isSelected to didnot show again
                    var SelectedArbitratorList = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems;
                    foreach (var item in SelectedArbitratorList.ToList())
                    {
                        switch (litigationRequestParty)
                        {
                            case LitigationRequestParty.Plaintiff:
                                var LitigationNominationItem = litigationRequest?.LitigationRequestArbitratorNominationLists.Where(x => x.Id == item.ArbitratorListId).FirstOrDefault();
                                if (LitigationNominationItem != null)
                                    LitigationNominationItem.PlaintiffPriority = Counter + item.Priority;
                                LitigationNominationItem.TotalPriority = Counter + item.Priority;
                                LitigationNominationItem.IsPlaintiffSelected = true;
                                break;
                            case LitigationRequestParty.Defendant:
                                var LitigationNominationItemDefendant = litigationRequest?.LitigationRequestArbitratorNominationLists.Where(x => x.Id == item.ArbitratorListId).FirstOrDefault();
                                if (LitigationNominationItemDefendant != null)
                                    LitigationNominationItemDefendant.DefendantPriority = Counter + item.Priority;
                                LitigationNominationItemDefendant.TotalPriority = Counter + item.Priority;
                                LitigationNominationItemDefendant.IsDefendantSelected = true;
                                break;
                        }
                    }
                    //var Counter = 0;
                    //switch (litigationRequestParty)
                    //{
                    //    case LitigationRequestParty.Plaintiff:
                    //        var DefendantSelectedArbitrators = SelectedArbitrators.Where(s => s.DefendantPriority >= 1).ToList();
                    //        Counter = DefendantSelectedArbitrators?.Count ?? 0;
                    //        SelectedArbitrators = SelectedArbitrators.Where(s => s.PlaintiffPriority >= 1).ToList();
                    //        foreach (var item in SelectedArbitrators)
                    //        {
                    //           var SelectedArbitrators = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems.Where(x => x.ArbitratorListId == item.Id).FirstOrDefault()?.Priority; ;
                    //            item.PlaintiffPriority = Counter + item.PlaintiffPriority;
                    //            item.TotalPriority = Counter + item.PlaintiffPriority;
                    //            item.IsPlaintiffSelected = true;
                    //        }
                    //        break;
                    //    case LitigationRequestParty.Defendant:
                    //        var PlaintiffSelectedArbitrators = SelectedArbitrators.Where(s => s.PlaintiffPriority >= 1).ToList();
                    //        Counter = PlaintiffSelectedArbitrators?.Count ?? 0;
                    //        SelectedArbitrators = SelectedArbitrators.Where(s => s.DefendantPriority >= 1).ToList();
                    //        foreach (var item in SelectedArbitrators)
                    //        {
                    //            item.DefendantPriority = Counter + item.DefendantPriority;
                    //            item.TotalPriority = Counter + item.DefendantPriority;
                    //            item.IsDefendantSelected = true;
                    //        }
                    //        break;
                    //}
                }
            }
            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue & litigationRequest.IsDefendantArbitratorNominationListDone.HasValue)
            {
                litigationRequest.ArbitratorsNominationListSelectionDoneDateTime = DateTime.Now;
                litigationRequest.ExternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;
                litigationRequest.InternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;

                var arbitratorsList = litigationRequest.LitigationRequestArbitratorNominationLists.OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                var RejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.LitigationRequestArbitratorAssigningRequests.Any(s => s.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject || s.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)).ToList();

                var rejectedIds = new HashSet<Guid>(RejectedArbitrators.Select(x => x.Id));


                switch (litigationRequestParty)
                {
                    case LitigationRequestParty.Plaintiff:
                        var DefendantSelectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsDefendantSelected == true && x.IsSent == true);
                        foreach (var id in DefendantSelectedArbitrators.Select(x => x.Id))
                        {
                            rejectedIds.Add(id);
                        }
                        break;
                    case LitigationRequestParty.Defendant:
                        var PlaintiffSelectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsPlaintiffSelected == true && x.IsSent == true);
                        foreach (var id in PlaintiffSelectedArbitrators.Select(x => x.Id))
                        {
                            rejectedIds.Add(id);
                        }
                        break;
                }
                var arbitratorsListForMultiple = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.TotalPriority.HasValue && !rejectedIds.Contains(x.Id)).OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                var AssignArbitrator = false;
                switch (litigationRequest.ArbitratorsCount)
                {
                    case null:
                        break;
                    case 0:
                        break;
                    case 1:
                        AssignArbitrator = await ProcessArbitratorSelectionForSingle(litigationRequest, arbitratorsList);
                        break;
                    default:

                        AssignArbitrator = await ProcessArbitratorSelectionForMultiple(litigationRequest, arbitratorsListForMultiple);
                        break;
                }
                if (AssignArbitrator)
                {

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم الإنتهاء من تعيين هيئة التحكيم وإرسال طلبات المهام"
                    });

                    // Send Notification
                    // تم تعيين هيئة التحكيم وإرسال طلبات المهام
                    try
                    {
                        var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AssignedAndSent).Result.Data?.Value;
                        msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
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
                                Subject = "تم تعيين هيئة التحكيم وإرسال طلبات المهام",
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
            repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;




            return await GetCitizenLitigationRequestForArbitratorsNominationListReply(arbitratorNominationListRequestReplyPayload.RequestId, cancellationToken);

        }
        public async Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsNominationListReplyForCoAdmin(ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload, CancellationToken cancellationToken)
        {
            //Guid referenceId = Guid.Empty;
            //_jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.CitizenRequests)
                            .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.LitigationRequestArbitratorLists)
                             .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen).ThenInclude(x => x.ArbitratorLitigationRequests)
                             .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                          .Include(x => x.ArbitratorCitizen)
                          .ThenInclude(x => x.CitizenRequests)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                          .Include(x => x.Contract.SecondPartyCitizen)
                           .Include(x => x.LitigationRequestArbitratorLists)
                          .FirstOrDefault(x => x.Id == arbitratorNominationListRequestReplyPayload.RequestId);

            if (litigationRequest == null)
            {
                return new BaseResponse()
                {
                    IsDone = false,
                    Message = "طلب غير موجود"
                };
            }


            var litigationRequestParty = (litigationRequest.LitigationRequestArbitratorNominationLists
                .Count(x => x.IsPlaintiffSelected == true) >= 1
                && litigationRequest.LitigationRequestArbitratorNominationLists.All(x => x.IsDefendantSelected == null /*|| x.IsPlaintiffSelected == true*/))
                ? LitigationRequestParty.Defendant
                : (litigationRequest.LitigationRequestArbitratorNominationLists
                .Count(x => x.IsDefendantSelected == true) >= 1
                && litigationRequest.LitigationRequestArbitratorNominationLists.All(x => x.IsPlaintiffSelected == null /*|| x.IsPlaintiffSelected == true*/))
                ? LitigationRequestParty.Plaintiff : LitigationRequestParty.None;

            if (litigationRequest?.LitigationRequestArbitratorNominationLists != null)
            {
                switch (litigationRequestParty)
                {
                    case LitigationRequestParty.Plaintiff:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إختيار المحكمين من قبل المدير للمدعى"
                        });
                        litigationRequest.IsPlaintiffArbitratorNominationListDone = true;
                        break;
                    case LitigationRequestParty.Defendant:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إختيار المحكمين من قبل المدير للمدعى عليه"
                        });
                        litigationRequest.IsDefendantArbitratorNominationListDone = true;
                        break;
                    case LitigationRequestParty.None:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إختيار المحكمين من قبل المدير للمدعي و للمدعى عليه"
                        });
                        litigationRequest.IsPlaintiffArbitratorNominationListDone = true;
                        litigationRequest.IsDefendantArbitratorNominationListDone = true;
                        break;
                }
                if (litigationRequest.ArbitratorsCount == 1)
                {
                    var nominationIds = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems.Select(x => x.ArbitratorListId).ToList();
                    var nominationList = litigationRequest?.LitigationRequestArbitratorNominationLists?.Where(x => nominationIds.Contains(x.Id)).ToList();
                    if (nominationList == null || nominationList.Count == 0)
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "قائمة المحكمين المختارة غير موجودة في  الطلب"
                        };
                    }
                    foreach (var item in nominationList)
                    {
                        switch (litigationRequestParty)
                        {
                            //case LitigationRequestParty.Plaintiff:
                            //    item.PlaintiffPriority = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems.Where(x => x.ArbitratorListId == item.Id).FirstOrDefault()?.Priority;
                            //    if (item.PlaintiffPriority.HasValue)
                            //        item.TotalPriority = item.PlaintiffPriority + item.DefendantPriority;
                            //    break;
                            //case LitigationRequestParty.Defendant:
                            //    item.DefendantPriority = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems.Where(x => x.ArbitratorListId == item.Id).FirstOrDefault()?.Priority;
                            //    if (item.DefendantPriority.HasValue)
                            //        item.TotalPriority = item.DefendantPriority + item.PlaintiffPriority;
                            //    break;
                            case LitigationRequestParty.None:
                                item.PlaintiffPriority = 1;
                                item.DefendantPriority = 1;
                                item.IsDefendantSelected = true;
                                item.IsPlaintiffSelected = true;
                                item.IsArbitratorAdminSelected = true;
                                item.TotalPriority = 2;
                                break;
                        }
                    }
                }
                //handle the selected arbatrator one for multiple arbitrator 
                if (litigationRequest.ArbitratorsCount > 1)
                {
                    var Counter = 0;
                    //get the selected one and update isSelected to didnot show again
                    var SelectedArbitratorList = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems;
                    for (int i = 0; i < SelectedArbitratorList.Count; i++)
                    {
                        var item = SelectedArbitratorList[i];
                        switch (litigationRequestParty)
                        {
                            case LitigationRequestParty.Plaintiff:
                                var LitigationNominationItem = litigationRequest?.LitigationRequestArbitratorNominationLists
                                    .Where(x => x.Id == item.ArbitratorListId).FirstOrDefault();
                                if (LitigationNominationItem != null)
                                {
                                    LitigationNominationItem.PlaintiffPriority = Counter + item.Priority;
                                    LitigationNominationItem.TotalPriority = Counter + item.Priority;
                                    LitigationNominationItem.IsPlaintiffSelected = true;
                                    LitigationNominationItem.IsArbitratorAdminSelected = true;

                                }
                                break;
                            case LitigationRequestParty.Defendant:
                                var LitigationNominationItemDefendant = litigationRequest?.LitigationRequestArbitratorNominationLists
                                    .Where(x => x.Id == item.ArbitratorListId).FirstOrDefault();
                                if (LitigationNominationItemDefendant != null)
                                {
                                    LitigationNominationItemDefendant.DefendantPriority = Counter + item.Priority;
                                    LitigationNominationItemDefendant.TotalPriority = Counter + item.Priority;
                                    LitigationNominationItemDefendant.IsDefendantSelected = true;
                                    LitigationNominationItemDefendant.IsArbitratorAdminSelected = true;

                                }
                                break;
                            case LitigationRequestParty.None:
                                var LitigationNominationItemNone = litigationRequest?.LitigationRequestArbitratorNominationLists
                                    .Where(x => x.Id == item.ArbitratorListId).FirstOrDefault();
                                if (LitigationNominationItemNone != null)
                                {
                                    int half = SelectedArbitratorList.Count / 2;
                                    if (i < half)
                                    {
                                        LitigationNominationItemNone.IsPlaintiffSelected = true;
                                    }
                                    else
                                    {
                                        LitigationNominationItemNone.IsDefendantSelected = true;
                                    }
                                    LitigationNominationItemNone.IsArbitratorAdminSelected = true;
                                    LitigationNominationItemNone.TotalPriority = Counter + 1;
                                }
                                Counter++;
                                break;
                        }
                    }

                }
            }
            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue & litigationRequest.IsDefendantArbitratorNominationListDone.HasValue)
            {
                litigationRequest.ArbitratorsNominationListSelectionDoneDateTime = DateTime.Now;
                litigationRequest.ExternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;
                litigationRequest.InternalStatusId = RequestStatus.ArbitratorsSelectionIsDone;

                var arbitratorsList = litigationRequest.LitigationRequestArbitratorNominationLists.OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                var RejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.LitigationRequestArbitratorAssigningRequests.Any(s => s.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject || s.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled)).ToList();

                var rejectedIds = new HashSet<Guid>(RejectedArbitrators.Select(x => x.Id));


                switch (litigationRequestParty)
                {
                    case LitigationRequestParty.Plaintiff:
                        var DefendantSelectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsDefendantSelected == true && x.IsSent == true);
                        foreach (var id in DefendantSelectedArbitrators.Select(x => x.Id))
                        {
                            rejectedIds.Add(id);
                        }
                        break;
                    case LitigationRequestParty.Defendant:
                        var PlaintiffSelectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsPlaintiffSelected == true && x.IsSent == true);
                        foreach (var id in PlaintiffSelectedArbitrators.Select(x => x.Id))
                        {
                            rejectedIds.Add(id);
                        }
                        break;
                }
                var arbitratorsListForMultiple = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.TotalPriority.HasValue && !rejectedIds.Contains(x.Id)).OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();
                var AssignArbitrator = false;
                switch (litigationRequest.ArbitratorsCount)
                {
                    case null:
                        break;
                    case 0:
                        break;
                    case 1:
                        AssignArbitrator = await ProcessArbitratorSelectionForSingle(litigationRequest, arbitratorsList);
                        break;
                    default:

                        AssignArbitrator = await ProcessArbitratorSelectionForMultiple(litigationRequest, arbitratorsListForMultiple);
                        break;
                }
                if (AssignArbitrator)
                {

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم الإنتهاء من تعيين هيئة التحكيم وإرسال طلبات المهام"
                    });

                    // Send Notification
                    // تم تعيين هيئة التحكيم وإرسال طلبات المهام
                    try
                    {
                        var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AssignedAndSent).Result.Data?.Value;
                        msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
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
                                Subject = "تم تعيين هيئة التحكيم وإرسال طلبات المهام",
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
            repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            return new BaseResponse()
            {
                IsDone = true,
                Message = "تم الحفظ بنجاح"
            };


            //return await GetCitizenLitigationRequestForArbitratorsNominationListReply(arbitratorNominationListRequestReplyPayload.RequestId, cancellationToken);

        }

        public async Task<BaseResponse> AddLitigationRequestArbitratorsdecision(Guid requestId, Guid arbitratorId)
        {
            try
            {
                if (requestId == null | requestId == Guid.Empty)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                if (arbitratorId == null | arbitratorId == Guid.Empty)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitratorNominationLists)
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

                var selectedArbitrator = litigationRequest.LitigationRequestArbitratorNominationLists.Where(x => x.IsEscalated == true && x.Id == arbitratorId).FirstOrDefault();

                if (selectedArbitrator != null)
                {
                    selectedArbitrator.IsSelectedFromManager = true;
                    await AddSingleNominationAssigningArbitrator(litigationRequest, selectedArbitrator);
                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;
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
            return await GetLitigationRequestForAddArbitratorsNominationList(requestId, new CancellationToken());

        }

        #endregion
        #region Extention Method for Add Multiple Nomination Assigning Arbitrator
        public async Task<bool> ProcessArbitratorSelectionForMultiple(LitigationRequest litigationRequest, List<LitigationRequestArbitratorNominationList>? arbitratorsList)
        {
            //Remove Duplicate 
            var LitigatioinRequestAssignedBeforeIds = litigationRequest.LitigationRequestArbitratorAssigningRequests.Select(s => s.LitigationRequestArbitratorNominationListId).ToList();
            arbitratorsList = arbitratorsList?.Where(x => !LitigatioinRequestAssignedBeforeIds.Contains(x.Id)).ToList();
            foreach (var arbitrator in arbitratorsList)
            {
                await AddSingleNominationAssigningArbitrator(litigationRequest, arbitrator);
            }
            return true;
        }
        #endregion
        #region Extention method for Add Single Nomination Assigning Arbitrator
        public async Task<bool> ProcessArbitratorSelectionForSingle(LitigationRequest litigationRequest, List<LitigationRequestArbitratorNominationList>? arbitratorsList)
        {
            // Calculate total priority scores for each arbitrator
            var arbitratorScores = new List<ArbitratorSelectionScore>();

            foreach (var arbitrator in arbitratorsList)
            {
                var score = new ArbitratorSelectionScore
                {
                    Arbitrator = arbitrator,
                    TotalScore = arbitrator.TotalPriority,
                    ExperienceYears = arbitrator.ArbitratorCitizen?.CitizenRequests?
                                                                                    .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator & x.StatusId == CitizenRequestStatus.Certified)?
                                                                                    .OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears ?? 0,
                    RequestsCount = arbitrator.ArbitratorCitizen?.ArbitratorLitigationRequests.Count() + arbitrator.ArbitratorCitizen?.LitigationRequestArbitratorLists.Count,
                    GeographicalPriority = await CompareGeographicalDistance(arbitrator.ArbitratorCitizen, litigationRequest)
                };
                arbitratorScores.Add(score);
            }

            // Apply score & weight calculation criteria
            var selectedArbitrator = await ApplySelectionCriteria(litigationRequest, arbitratorsList, arbitratorScores);

            if (selectedArbitrator != null)
            {
                await AddSingleNominationAssigningArbitrator(litigationRequest, selectedArbitrator);
                return true;
            }

            return false;
        }

        private async Task<LitigationRequestArbitratorNominationList> ApplySelectionCriteria(LitigationRequest litigationRequest, List<LitigationRequestArbitratorNominationList> arbitratorsList, List<ArbitratorSelectionScore> arbitratorScores)
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
            await EscalateToManager(litigationRequest, arbitratorsList, geographicalCandidates);
            return null;
        }
        private async Task<decimal> CompareGeographicalDistance(Citizen arbitratorCitizen, LitigationRequest litigationRequest)
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
                var Fmsg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value;
                Fmsg = Fmsg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                string body;
                using (StreamReader streamReader = File.OpenText(pathToFile))
                {
                    body = await streamReader.ReadToEndAsync();
                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value);
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
        #region Get Nomination List 
        private Guid GetReferenceId()
        {
            var referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            return referenceId;
        }

        private async Task<LitigationRequest> GetLitigationRequestWithIncludes(Guid requestId, Guid referenceId)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                           .Include(x => x.ArbitratorCitizen)
                           .Include(x => x.InternalStatus)
                           .Include(x => x.ExternalStatus)
                           .Include(x => x.City)
                           .Include(x => x.Area)
                           .Include(x => x.LitigationRequestArbitratorNominationLists)
                           .ThenInclude(x => x.ArbitratorCitizen)
                           .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                           .Include(x => x.LitigationRequestArbitratorNominationLists)
                           .ThenInclude(x => x.ArbitratorCitizen)
                           .ThenInclude(x => x.ArbitratorLitigationRequests)
                           .Include(x => x.LitigationRequestArbitratorNominationLists)
                           .ThenInclude(x => x.ArbitratorCitizen)
                           .ThenInclude(x => x.CitizenRequests)
                           .ThenInclude(x => x.SysArbitratorClass)

                               .Include(x => x.LitigationRequestArbitratorNominationLists)
                               .ThenInclude(x => x.ArbitratorCitizen)
                               .ThenInclude(x => x.LitigationRequestArbitratorLists)
                               .Include(x => x.LitigationRequestArbitratorNominationLists)
                               .ThenInclude(x => x.LitigationRequestArbitratorAssigningRequests)
                               .Include(x => x.LitigationRequestArbitratorLists)
                               .Include(x => x.Contract)
                               .Include(x => x.Contract.LitigationRequestContractType)
                               .Include(x => x.Contract.FirstPartyFacility)
                               .Include(x => x.Contract.FirstPartyCitizen)
                               .Include(x => x.Contract.SecondPartyFacility)
                               .Include(x => x.Contract.SecondPartyCitizen)
                               .Include(x => x.Contract.LitigationRequestContractType)
                               .Where(x => x.Id == requestId
                               &&
                   x.ApplicantCitizenId == referenceId
                         |
                         x.Contract.FirstPartyCitizenId == referenceId & x.Id == requestId
                         |
                           x.Contract.SecondPartyCitizenId == referenceId & x.Id == requestId
                         |
                          x.Contract.FirstPartyFacility.CitizenId == referenceId & x.Id == requestId
                         |
                         x.Contract.SecondPartyFacility.CitizenId == referenceId & x.Id == requestId
                               ).AsNoTracking().ToList().FirstOrDefault();

            return litigationRequest;
        }

        private static bool IsUserAuthorizedForRequest(LitigationRequest request, Guid referenceId)
        {
            return request.ApplicantCitizenId == referenceId ||
                   request.Contract.FirstPartyCitizenId == referenceId ||
                   request.Contract.SecondPartyCitizenId == referenceId ||
                   request.Contract.FirstPartyFacility.CitizenId == referenceId ||
                   request.Contract.SecondPartyFacility.CitizenId == referenceId;
        }

        private bool CanAddArbitrator(LitigationRequest litigationRequest, Guid referenceId)
        {
            if (litigationRequest.IsFinished == true)
                return false;

            if (litigationRequest.LitigationRequestStage != LitigationRequestStage.Arbitration)
                return false;

            if (litigationRequest.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAddArbitratorListNominationDone)
                return false;

            if (litigationRequest.ArbitratorsCount <= 1)
                return true;

            var userRole = GetUserRole(litigationRequest, referenceId);
            return userRole switch
            {
                UserRole.Plaintiff => litigationRequest.IsPlaintiffArbitratorNominationListDone != true,
                UserRole.Defendant => litigationRequest.IsDefendantArbitratorNominationListDone != true,
                _ => false
            };
        }

        private UserRole GetUserRole(LitigationRequest litigationRequest, Guid referenceId)
        {
            if (litigationRequest.ApplicantCitizenId == referenceId ||
                litigationRequest.Contract.FirstPartyCitizenId == referenceId ||
                litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                return UserRole.Plaintiff;

            if (litigationRequest.Contract.SecondPartyCitizenId == referenceId ||
                litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                return UserRole.Defendant;

            return UserRole.Unknown;
        }

        private LitigationRequestForAllArbitratorNominationListWithPartiesValueDto BuildArbitratorNominationListResponse(
            LitigationRequest litigationRequest, Guid referenceId, bool allowAdd)
        {
            var response = new LitigationRequestForAllArbitratorNominationListWithPartiesValueDto
            {
                LitigationRequestShortDto = _mapper.Map<LitigationRequestShortDto>(litigationRequest),
                CitizenArbitratorShortStatusDtos = new List<CitizenArbitratorShortStatusWithValueDto>(),
                AllowAdd = allowAdd
            };

            if (!litigationRequest.LitigationRequestArbitratorNominationLists.Any())
                return response;

            var rejectedIds = GetRejectedArbitratorIds(litigationRequest);
            var userRole = GetUserRole(litigationRequest, referenceId);

            if (litigationRequest.ArbitratorsCount > 1)
            {
                ProcessMultipleArbitratorsCase(litigationRequest, referenceId, allowAdd, rejectedIds, userRole, response);
            }
            else
            {
                ProcessSingleArbitratorCase(litigationRequest, referenceId, allowAdd, response);
            }

            return response;
        }

        private HashSet<Guid> GetRejectedArbitratorIds(LitigationRequest litigationRequest)
        {
            var rejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists
                .Where(x => x.LitigationRequestArbitratorAssigningRequests.Any(s =>
                    s.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject ||
                    s.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled));

            return new HashSet<Guid>(rejectedArbitrators.Select(x => x.Id));
        }

        private void ProcessMultipleArbitratorsCase(LitigationRequest litigationRequest, Guid referenceId,
            bool allowAdd, HashSet<Guid> rejectedIds, UserRole userRole,
            LitigationRequestForAllArbitratorNominationListWithPartiesValueDto response)
        {
            if (allowAdd)
            {
                var nominationArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists
                    .Where(x => x.IsDefendantSelected != true && x.IsPlaintiffSelected != true);

                response.CitizenArbitratorShortStatusDtos.AddRange(
                    nominationArbitrators.Select(x => MapArbitratorToDto(x, litigationRequest, referenceId)));

                var selectedByOtherParty = GetSelectedByOtherParty(litigationRequest, userRole, rejectedIds);
                response.CitizenSelectedArbitratorShortStatusDtos.AddRange(
                    selectedByOtherParty.Select(x => MapArbitratorToDto(x, litigationRequest, referenceId)));

                var selectedByUser = GetSelectedByUser(litigationRequest, userRole, rejectedIds);
                response.CitizenSelectedUserArbitratorShortStatusDtos.AddRange(
                    selectedByUser.Select(x => MapArbitratorToDto(x, litigationRequest, referenceId)));

                var rejectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists
                    .Where(x => rejectedIds.Contains(x.Id));
                response.CitizenRejectedArbitratorShortStatusDtos.AddRange(
                    rejectedArbitrators.Select(x => MapArbitratorToDto(x, litigationRequest, referenceId)));
            }
            else
            {
                var selectedArbitrators = litigationRequest.LitigationRequestArbitratorNominationLists
                    .Where(x => (x.IsDefendantSelected == true || x.IsPlaintiffSelected == true) && !rejectedIds.Contains(x.Id));

                response.CitizenArbitratorShortStatusDtos.AddRange(
                    selectedArbitrators.Select(x => MapArbitratorToDto(x, litigationRequest, referenceId)));
            }
        }

        private void ProcessSingleArbitratorCase(LitigationRequest litigationRequest, Guid referenceId,
            bool allowAdd, LitigationRequestForAllArbitratorNominationListWithPartiesValueDto response)
        {
            foreach (var nomination in litigationRequest.LitigationRequestArbitratorNominationLists)
            {
                if (allowAdd || IsArbitratorAssigned(litigationRequest, nomination.ArbitratorCitizenId))
                {
                    response.CitizenArbitratorShortStatusDtos.Add(
                        MapArbitratorToDto(nomination, litigationRequest, referenceId));
                }
            }
        }

        private IEnumerable<LitigationRequestArbitratorNominationList> GetSelectedByOtherParty(
            LitigationRequest litigationRequest, UserRole userRole, HashSet<Guid> rejectedIds)
        {
            return userRole switch
            {
                UserRole.Plaintiff => litigationRequest.LitigationRequestArbitratorNominationLists
                    .Where(x => x.IsDefendantSelected == true && !rejectedIds.Contains(x.Id)),
                UserRole.Defendant => litigationRequest.LitigationRequestArbitratorNominationLists
                    .Where(x => x.IsPlaintiffSelected == true && !rejectedIds.Contains(x.Id)),
                _ => Enumerable.Empty<LitigationRequestArbitratorNominationList>()
            };
        }

        private IEnumerable<LitigationRequestArbitratorNominationList> GetSelectedByUser(
            LitigationRequest litigationRequest, UserRole userRole, HashSet<Guid> rejectedIds)
        {
            return userRole switch
            {
                UserRole.Plaintiff => litigationRequest.LitigationRequestArbitratorNominationLists
                    .Where(x => x.IsPlaintiffSelected == true && !rejectedIds.Contains(x.Id)),
                UserRole.Defendant => litigationRequest.LitigationRequestArbitratorNominationLists
                    .Where(x => x.IsDefendantSelected == true && !rejectedIds.Contains(x.Id)),
                _ => Enumerable.Empty<LitigationRequestArbitratorNominationList>()
            };
        }

        private bool IsArbitratorAssigned(LitigationRequest litigationRequest, Guid arbitratorCitizenId)
        {
            return litigationRequest.LitigationRequestArbitratorLists
                .Any(x => x.ArbitratorCitizenId == arbitratorCitizenId);
        }

        private CitizenArbitratorShortStatusWithValueDto MapArbitratorToDto(
            LitigationRequestArbitratorNominationList nomination,
            LitigationRequest litigationRequest,
            Guid referenceId)
        {
            var certifiedRequest = nomination.ArbitratorCitizen?.CitizenRequests?
                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator &&
                           x.StatusId == CitizenRequestStatus.Certified)
                .OrderBy(x => x.RequestOn)
                .LastOrDefault();

            var userRole = GetUserRole(litigationRequest, referenceId);
            var priority = GetPriorityForUser(nomination, userRole);

            return new CitizenArbitratorShortStatusWithValueDto
            {
                Id = nomination.Id,
                CitizenId = nomination.ArbitratorCitizenId,
                FullName = nomination.ArbitratorCitizen.FullName,
                Mobile = nomination.ArbitratorCitizen.Mobile,
                Email = nomination.ArbitratorCitizen.Email,
                IdentityNumber = nomination.ArbitratorCitizen.IdentityNumber,
                ArbitratorClass = certifiedRequest?.SysArbitratorClass?.ArbitratorClassName,
                ExperienceYears = certifiedRequest?.ExperienceYears,
                Brief = certifiedRequest?.Brief,
                IsPlaintiffSelected = nomination.IsPlaintiffSelected,
                IsDefendantSelected = nomination.IsDefendantSelected,
                Value = priority
            };
        }

        private int? GetPriorityForUser(LitigationRequestArbitratorNominationList nomination, UserRole userRole)
        {
            return userRole switch
            {
                UserRole.Plaintiff => nomination.PlaintiffPriority,
                UserRole.Defendant => nomination.DefendantPriority,
                _ => null
            };
        }

        private enum UserRole
        {
            Unknown,
            Plaintiff,
            Defendant
        }
        #endregion
    }
}
