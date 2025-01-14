using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{

    public partial class ArbitrationService

    {
        #region ArbitratorSelectionDuration
        public async Task<BaseResponse> AddArbitratorSelectionDuration(ArbitratorSelectionDurationRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                    .Include(x => x.ArbitratorCitizen)
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract)
                    .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract.FirstPartyCitizen)
                    .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract.SecondPartyCitizen)
                        .Where(x => x.Id == arbitratorCountRequestPayload.RequestId)
                        .Single();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                bool allowAdd = true;

                if (litigationRequest.ArbitratorsCount.HasValue)
                    allowAdd = false;
                if (litigationRequest.FirstPartyArbitratorsCount.HasValue & litigationRequest.ArbitratorSelectionDuration.HasValue)
                    allowAdd = false;

                if (allowAdd)
                {
                    litigationRequest.ArbitratorSelectionDuration = arbitratorCountRequestPayload.ArbitratorSelectionDuration;
                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestAddArbitratorCount;
                    litigationRequest.ArbitratorsSelectionDurationDateTime = DateTime.Now;


                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم تحديد مدة إختيار عدد المحكمين " + arbitratorCountRequestPayload.ArbitratorSelectionDuration + " ساعة"
                    });



                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;



                    // Send Notification
                    // تم تحديد مدة تحديد المحكمين
                    try
                    {
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        if (arbitrationAdmin.IsDone)
                        {
                            var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorSelectionDuration_Added).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);

                            var adminMsg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorSelectionDuration_Added_Admin).Result.Data?.Value;
                            adminMsg = adminMsg?.Replace("{LitigationCode}", litigationRequest.RequestCode);


                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                        new SysNotificationDto{
                        ActorId = litigationRequest.ApplicantCitizen.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Plaintiff,
                        Message=msg
                        }
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
                                Subject = "تحديد مدة أختيار المحكمين",
                                Body = msg
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, notifications);

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = "تحديد مدة أختيار المحكمين",
                                Body = adminMsg
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = adminMsg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, new List<SysNotificationDto>()
                            {
                                new SysNotificationDto{
                                ActorId = (Guid)arbitrationAdmin.Data?.Id,
                                RequestId=litigationRequest.Id,
                                CreatedOn= DateTime.Now,
                                RequestTypeId=RequestType.LitigationRequest,
                                ActorTypeId=NotificationActorType.Employee,
                                Message=adminMsg
                                }
                            });
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
                        Message = "لا يمكن إضافة مدة اختيار المحكمين"
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
            return await GetLitigationRequestForAddArbitratorSelectionDuration(arbitratorCountRequestPayload.RequestId, cancellationToken);
        }
        public async Task<BaseResponse> GetLitigationRequestForAddArbitratorSelectionDuration(Guid requestId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.ApplicantCitizen)
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

                         .Where(x => x.Id == requestId).AsNoTracking().ToList().FirstOrDefault();

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);


            bool isDone = false;
            bool allowAdd = true;

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };
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

                if (litigationRequest.FirstPartyArbitratorsCount.HasValue & litigationRequest.SecondPartyArbitratorsCount.HasValue)
                    allowAdd = false;

            }


            return new DataResponse<LitigationRequestForArbitratorSelectionDurationDto>()
            {

                Data = new LitigationRequestForArbitratorSelectionDurationDto()
                {
                    AllowAdd = allowAdd,
                    LitigationRequestShortDto = request,
                    ArbitratorSelectionDuration = litigationRequest.ArbitratorSelectionDuration
                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForArbitratorSelectionDuration(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)

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
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType);


            litigationRequests = entitiesQuerable.Where(x => x.RequestCode != null
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                                       & x.LitigationRequestStage == LitigationRequestStage.Arbitration)
                                       .OrderByDescending(x => x.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);





            List<LitigationRequestMircoForArbitratorsSelectionDurationDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForArbitratorsSelectionDurationDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    LitigationRequestMircoForArbitratorsSelectionDurationDto litigationRequestMircoDto = new LitigationRequestMircoForArbitratorsSelectionDurationDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.ArbitratorsSelectionDuration = litigationRequest.ArbitratorSelectionDuration;
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
                        if (litigationRequest.ArbitratorsCount.HasValue)
                        {
                            allowAdd = false;
                            isDone = true;
                        }
                        if (!litigationRequest.FirstPartyArbitratorsCount.HasValue & !litigationRequest.ArbitratorSelectionDuration.HasValue)

                        {
                            allowAdd = false;
                            isDone = true;
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
            return new DataResponse<PagedResult<LitigationRequestMircoForArbitratorsSelectionDurationDto>>
            {
                Data =
                  new PagedResult<LitigationRequestMircoForArbitratorsSelectionDurationDto>
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
