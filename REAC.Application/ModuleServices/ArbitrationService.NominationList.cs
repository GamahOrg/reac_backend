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
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Globalization;

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
                        await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                        {
                            To = emails,
                            Subject = "تم إضافة قائمة تعيين هيئة التحكيم برجاء الاختيار",
                            Body = msg
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
                                Body = bossMsg
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

                            .Include(x => x.ArbitratorCitizen)
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
                            .Where(x => x.Id == requestId).AsNoTracking().ToList().FirstOrDefault();

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
                                      | x.RequestCode != null & x.ArbitratorsCount.HasValue
                                      & x.Contract.FirstPartyCitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitratorNominationLists.Count > 0
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

                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
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
                            .Include(x => x.ArbitratorCitizen)
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
                    else
                    {
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
                    }
                }

            }

            LitigationRequestForAllArbitratorNominationListWithPartiesValueDto litigationRequestForAllArbitratorNominationListDto = new LitigationRequestForAllArbitratorNominationListWithPartiesValueDto();
            litigationRequestForAllArbitratorNominationListDto.LitigationRequestShortDto = request;
            litigationRequestForAllArbitratorNominationListDto.CitizenArbitratorShortStatusDtos = new List<CitizenArbitratorShortStatusWithValueDto>();

            if (litigationRequest.LitigationRequestArbitratorNominationLists.Any())
            {
                foreach (var litigationRequestArbitratorNominationList in litigationRequest.LitigationRequestArbitratorNominationLists)
                {
                    if (allowAdd)
                    {
                        CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                        citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                        citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                        citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;

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
                    else
                    {
                        if (litigationRequest.LitigationRequestArbitratorLists.Count(x => x.ArbitratorCitizenId == litigationRequestArbitratorNominationList.ArbitratorCitizenId) > 0)
                        {
                            CitizenArbitratorShortStatusWithValueDto citizenArbitratorShortStatusDto = new CitizenArbitratorShortStatusWithValueDto();
                            citizenArbitratorShortStatusDto.Id = litigationRequestArbitratorNominationList.Id;
                            citizenArbitratorShortStatusDto.CitizenId = litigationRequestArbitratorNominationList.ArbitratorCitizenId;
                            citizenArbitratorShortStatusDto.FullName = litigationRequestArbitratorNominationList.ArbitratorCitizen.FullName;

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
        public async Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsNominationListReply(ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.LitigationRequestArbitratorNominationLists).ThenInclude(x => x.ArbitratorCitizen)
                          .Include(x => x.ArbitratorCitizen)
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

                foreach (var item in litigationRequest?.LitigationRequestArbitratorNominationLists.ToList())
                {
                    switch (litigationRequestParty)
                    {
                        case LitigationRequestParty.Plaintiff:
                            item.PlaintiffPriority = arbitratorNominationListRequestReplyPayload.ArbitratorNominationListRequestItems.Where(x => x.ArbitratorListId == item.Id).FirstOrDefault()?.Priority;
                            if (item.DefendantPriority.HasValue)
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
            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            if (litigationRequest.IsPlaintiffArbitratorNominationListDone.HasValue & litigationRequest.IsDefendantArbitratorNominationListDone.HasValue)
            {
                litigationRequest.ArbitratorsNominationListSelectionDoneDateTime = DateTime.Now;
                var arbitratorsList = litigationRequest.LitigationRequestArbitratorNominationLists.OrderBy(x => x.TotalPriority).ThenBy(x => x.DefendantPriority).ToList();

                switch (litigationRequest.ArbitratorsCount)
                {
                    case null:
                        break;
                    case 0:
                        break;
                    case 1:
                        LitigationRequestArbitratorAssigningRequest litigationRequestOneArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                        litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                        litigationRequestOneArbitratorAssigningRequest.RequestOn = DateTime.Now;
                        litigationRequestOneArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
                        litigationRequestOneArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                        litigationRequestOneArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = arbitratorsList[0].Id;
                        litigationRequestOneArbitratorAssigningRequest.CitizenId = arbitratorsList[0].ArbitratorCitizenId;
                        litigationRequestOneArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                        await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestOneArbitratorAssigningRequest);
                        arbitratorsList[0].IsSent = true;
                        break;
                    default:
                        for (int record = 0; record < litigationRequest.ArbitratorsCount - 1; record++)
                        {
                            if (record == arbitratorsList.Count)
                                break;
                            LitigationRequestArbitratorAssigningRequest litigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                            litigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                            litigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                            litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
                            litigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                            litigationRequestArbitratorAssigningRequest.LitigationRequestArbitratorNominationListId = arbitratorsList[record].Id;
                            litigationRequestArbitratorAssigningRequest.CitizenId = arbitratorsList[record].ArbitratorCitizenId;
                            litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                            await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestArbitratorAssigningRequest);
                            arbitratorsList[record].IsSent = true;

                            // Send Notification
                            // يوجد طلب مهمه جديده كمحكم علي الدعوي رقم
                            try
                            {
                                var Fmsg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value;
                                Fmsg = Fmsg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                                                    .Where(c => c.Id == litigationRequestArbitratorAssigningRequest.CitizenId)
                                                    .FirstOrDefault();


                                List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = litigationRequestArbitratorAssigningRequest.CitizenId,
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
                                    Body = Fmsg
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
                        break;
                }
                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                {
                    Id = Guid.NewGuid(),
                    CreatedOn = DateTime.Now,
                    RequestId = litigationRequest.Id,
                    ShowForApplicant = false,
                    LogMessage = "تم الإنتهاء من تعيين هيئة التحكيم وإرسال طلبات المهام"
                });

                repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                if (!repositorySaveResponse.IsDone)
                    return repositorySaveResponse;

                // Send Notification
                // تم تعيين هيئة التحكيم وإرسال طلبات المهام
                try
                {
                    var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsNominationList_AssignedAndSent).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
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
                            Body = msg
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


            return await GetCitizenLitigationRequestForArbitratorsNominationListReply(arbitratorNominationListRequestReplyPayload.RequestId, cancellationToken);

        }
        #endregion
    }
}
