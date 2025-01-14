using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{


    public partial class ArbitrationService
    {
        #region ExplanationRequest
        public async Task<BaseResponse> AddArbitrationReportExplanationRequest(LitigationRequestArbitrationReportExplanationRequestCreateDto requestPayload)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())

                    .Include(x => x.ArbitratorCitizen)
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Area)
                    .Include(x => x.City)
                                      .Include(x => x.LitigationRequestArbitrationReports)
                                      .ThenInclude(x => x.LitigationRequestArbitrationReportExplanationRequests)
                                      .Include(x => x.Contract)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.Contract.ContractFile)
                                      .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                                      .Include(x => x.Contract.FoundingLetterFile)
                                      .Include(x => x.Contract.InternalRegulationsFile)
                                      .Include(x => x.Contract.FirstPartyFacility).ThenInclude(x => x.Citizen)
                                      .Include(x => x.Contract.FirstPartyCitizen)
                                      .Include(x => x.Contract.SecondPartyFacility).ThenInclude(x => x.Citizen)
                                      .Include(x => x.Contract.SecondPartyCitizen)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.ApplicantCitizen)
                                      .Where(x => x.Id == requestPayload.LitigationRequestId).SingleOrDefault();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                int LitigationRequestArbitrationReportExplanationRequestPeriod = 0;
                int.TryParse(_settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_LitigationRequestArbitrationReportExplanationRequestPeriod).Result.Data?.Value, out LitigationRequestArbitrationReportExplanationRequestPeriod);
                if (LitigationRequestArbitrationReportExplanationRequestPeriod > 0)
                {
                    var periodHours = (DateTime.Now - litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault()?.JudgmentDate).Value.TotalHours;
                    if (periodHours >= LitigationRequestArbitrationReportExplanationRequestPeriod)
                    {
                        return new BaseResponse() { IsDone = false, Message = "لا يمكن إضافة طلب التفسير لتجاوز المهله المتاحة لذلك" };
                    }
                }

                bool allowAdd = true;

                LitigationRequestParty litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.None;

                if (litigationRequest.LitigationRequestArbitrationReports.Count() == 0)
                    allowAdd = false;
                else
                {
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                    }
                    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }
                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }
                    if (litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault()?.LitigationRequestArbitrationReportExplanationRequests != null)
                    {
                        if (litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault()?.LitigationRequestArbitrationReportExplanationRequests.Count(x => x.RequestArbitrationReportExplanationRequestParty == litigationRequestArbitrationReportExplanationRequestParty) > 0)
                            allowAdd = false;

                    }
                }

                if (allowAdd)
                {
                    if (litigationRequestArbitrationReportExplanationRequestParty == LitigationRequestParty.None)
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "غير متاح"
                        };
                    }
                    LitigationRequestArbitrationReportExplanationRequest litigationRequestArbitrationReportExplanationRequest = new LitigationRequestArbitrationReportExplanationRequest()
                    {
                        Id = Guid.NewGuid(),
                        ExplanationRequestDate = DateTime.Now,
                        ExplanationRequests = requestPayload.ExplanationRequests,
                        RequestArbitrationReportExplanationRequestParty = litigationRequestArbitrationReportExplanationRequestParty,
                        LitigationRequestArbitrationReportId = litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault().Id,
                    };
                    switch (litigationRequestArbitrationReportExplanationRequestParty)
                    {
                        case LitigationRequestParty.Plaintiff:
                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إضافة طلب استفسار على حكم التحكيم من قبل المدعى"
                            });
                            break;
                        case LitigationRequestParty.Defendant:
                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إضافة طلب استفسار على حكم التحكيم من قبل المدعى عليه"
                            });
                            break;
                    }

                    await _repository.AddAsync<LitigationRequestArbitrationReportExplanationRequest, Guid>(litigationRequestArbitrationReportExplanationRequest);

                    var saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone)
                        return saveResponse;

                    requestPayload.LitigationRequestArbitrationReportExplanationRequestFiles.ForEach(t =>
                    {
                        _repository.AddAsync<LitigationRequestArbitrationReportExplanationRequestFile, Guid>(new LitigationRequestArbitrationReportExplanationRequestFile()
                        {
                            Id = Guid.NewGuid(),
                            ExplanationRequestFileId = t,
                            ArbitrationReportExplanationRequestId = litigationRequestArbitrationReportExplanationRequest.Id
                        });
                    });

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;

                    litigationRequest.InternalStatusId = RequestStatus.WaitingForExplanation;
                    litigationRequest.ExternalStatusId = RequestStatus.WaitingForExplanation;
                    litigationRequest.IsMinutesOfMeetingApprovalRequired = true;


                    repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                        return repositorySaveResponse;

                    // Send Notification  
                    // تم اضافة طلب تفسير على حكم التحكيم
                    try
                    {
                        var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        var msg = "";
                        msg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExplanationReport_Added).Result.Data?.Value;
                        msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);

                        if (arbitrationAdmin.IsDone)
                        {


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
                 ActorId = (Guid)arbitrationAdmin.Data?.Id,
                 RequestId=litigationRequest.Id,
                 CreatedOn= DateTime.Now,
                 RequestTypeId=RequestType.LitigationRequest,
                 ActorTypeId=NotificationActorType.Employee,
                 Message=msg
                 }

                 };
                            List<string> emails = new List<string>() {
                      litigationRequest.ArbitratorCitizen.Email,
                         litigationRequest.ApplicantCitizen.Email,
                           arbitrationAdmin.Data?.Email
                         };

                            List<string> recipients = new List<string>()
                     {
                     litigationRequest.ArbitratorCitizen.Mobile,
                         litigationRequest.ApplicantCitizen.Mobile,
                           arbitrationAdmin.Data?.Mobile
                     };
                            if (litigationRequestArbitrationReportExplanationRequest.RequestArbitrationReportExplanationRequestParty == LitigationRequestParty.Plaintiff)
                            {
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
                            }

                            if (litigationRequestArbitrationReportExplanationRequest.RequestArbitrationReportExplanationRequestParty == LitigationRequestParty.Defendant)
                            {
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
                            }

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = "اضافة طلب تفسير على حكم التحكيم",
                                Body = msg
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, notifications);
                        }

                    }
                    catch (Exception)
                    {

                        throw;
                    }


                }
                else
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن إضافة محضر جديد"
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
            return await GetLitigationRequestForReportExplanationRequest(requestPayload.LitigationRequestId);
        }
        public async Task<BaseResponse> GetLitigationRequestForReportExplanationRequest(Guid requestId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestArbitrationReports)
                         .ThenInclude(x => x.LitigationRequestArbitrationReportExplanationRequests)
                                                  .ThenInclude(x => x.LitigationRequestArbitrationReportExplanationRequestFiles)
                                                                           .ThenInclude(x => x.ExplanationRequestFile)


                         .Include(x => x.ApplicantCitizen)
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

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            bool isDone = false;
            bool allowAdd = true;

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            LitigationRequestParty litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.None;
            if (litigationRequest.ApplicantCitizenId == referenceId)
            {
                litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
            }
            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
            {
                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                }
            }
            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
            {
                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                }
            }
            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
            {
                if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                }
            }
            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
            {
                if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                {
                    litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                }
            }

            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    isDone = true;
                }
            }

            var requestArbitrationReportExplanationRequest = litigationRequest?.LitigationRequestArbitrationReports.FirstOrDefault()?
                .LitigationRequestArbitrationReportExplanationRequests
                .Where(x => x.RequestArbitrationReportExplanationRequestParty == litigationRequestArbitrationReportExplanationRequestParty).FirstOrDefault();

            if (!isDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                    allowAdd = false;
                else
                {
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.JudgmentDecisionIssued)
                        allowAdd = false;
                    else
                    {
                        if (requestArbitrationReportExplanationRequest != null)
                            allowAdd = false;
                    }
                }
            }
            var reportExplanationRequest = _mapper.Map<LitigationRequestArbitrationReportExplanationRequestGetDto>(requestArbitrationReportExplanationRequest);

            return new DataResponse<LitigationRequestForArbitrationReportExplanationRequestDto>()
            {
                Data = new LitigationRequestForArbitrationReportExplanationRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    Report = reportExplanationRequest
                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForReportExplanationRequest(int pageIndex, int pageSize)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                                     .Include(x => x.Area).Include(x => x.City)
                                     .Include(x => x.Area)
                                     .Include(x => x.City)
                                     .Include(x => x.LitigationRequestArbitrationReports)
                                     .ThenInclude(x => x.LitigationRequestArbitrationReportExplanationRequests)
                                     .Include(x => x.ApplicantCitizen)
                                     .Include(x => x.InternalStatus)
                                     .Include(x => x.ExternalStatus)
                                     .Include(x => x.Contract)
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
                .Where(x => x.RequestCode != null
                                      & x.ApplicantCitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitrationReports.Count > 0
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                                      | x.RequestCode != null
                                      & x.Contract.FirstPartyCitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitrationReports.Count > 0
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                                      | x.RequestCode != null
                                      & x.Contract.SecondPartyCitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitrationReports.Count > 0
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                                      | x.RequestCode != null
                                      & x.Contract.FirstPartyFacility.CitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitrationReports.Count > 0
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                                      | x.RequestCode != null
                                      & x.Contract.SecondPartyFacility.CitizenId == referenceId
                                      & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                      & x.LitigationRequestArbitrationReports.Count > 0
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

                    LitigationRequestParty litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.None;
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                    }
                    if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }
                    if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Plaintiff;
                        }
                    }
                    if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                        {
                            litigationRequestArbitrationReportExplanationRequestParty = LitigationRequestParty.Defendant;
                        }
                    }

                    if (litigationRequest.IsFinished.HasValue)
                    {
                        if (litigationRequest.IsFinished.Value)
                        {
                            allowAdd = false;
                            isDone = true;
                        }
                    }
                    var requestArbitrationReportExplanationRequest = litigationRequest?.LitigationRequestArbitrationReports.FirstOrDefault()?
                         .LitigationRequestArbitrationReportExplanationRequests
                         .Where(x => x.RequestArbitrationReportExplanationRequestParty == litigationRequestArbitrationReportExplanationRequestParty).FirstOrDefault();

                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else
                        {
                            if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.JudgmentDecisionIssued)
                                allowAdd = false;
                            else
                            {
                                if (requestArbitrationReportExplanationRequest != null)
                                    allowAdd = false;
                            }
                        }
                    }
                    if (requestArbitrationReportExplanationRequest == null)
                    {
                        litigationRequestMircoDto.IsDone = "--";
                    }
                    else
                    {
                        if (allowAdd)
                            litigationRequestMircoDto.IsDone = "لم يتم الرد";
                        else
                            litigationRequestMircoDto.IsDone = "تم الرد";
                    }

                    litigationRequestMircoDto.Status = litigationRequest.ExternalStatus.RequestStatus;
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
        public async Task<BaseResponse> GetLitigationRequestForReportExplanationRequestForArbitrator(int pageIndex, int pageSize)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequestArbitrationReportExplanationRequest> litigationRequestArbitrationReportExplanationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequestArbitrationReportExplanationRequest, Guid>())
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.InternalStatus)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.ExternalStatus)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Area)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.City)
                         .Include(x => x.LitigationRequestArbitrationReport)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.ApplicantCitizen)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.ContractFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.AssociationRegistrationCertificateFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.FoundingLetterFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.InternalRegulationsFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.LitigationRequestContractType);

            litigationRequestArbitrationReportExplanationRequests = entitiesQuerable
                                        .OrderByDescending(x => x.LitigationRequestArbitrationReport.LitigationRequest.CreatedOn)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);

            List<LitigationRequestMircoExplanationRequestDto> litigationRequestMircoDtos = new List<LitigationRequestMircoExplanationRequestDto>();
            if (litigationRequestArbitrationReportExplanationRequests != null)
            {
                foreach (var litigationRequestArbitrationReportExplanationRequest in litigationRequestArbitrationReportExplanationRequests.Results)
                {

                    LitigationRequestMircoExplanationRequestDto litigationRequestMircoDto = new LitigationRequestMircoExplanationRequestDto();
                    litigationRequestMircoDto.Id = litigationRequestArbitrationReportExplanationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.RequestParty = litigationRequestArbitrationReportExplanationRequest.RequestArbitrationReportExplanationRequestParty;

                    if (string.IsNullOrEmpty(litigationRequestArbitrationReportExplanationRequest.Response))
                        litigationRequestMircoDto.IsDone = "لم يتم الرد";
                    else
                        litigationRequestMircoDto.IsDone = "تم الرد";
                    litigationRequestMircoDto.Status = litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }
            return new DataResponse<PagedResult<LitigationRequestMircoExplanationRequestDto>>
            {
                Data =
                  new PagedResult<LitigationRequestMircoExplanationRequestDto>
                  {
                      Results = litigationRequestMircoDtos,
                      CurrentPage = litigationRequestArbitrationReportExplanationRequests.CurrentPage,
                      PageCount = litigationRequestArbitrationReportExplanationRequests.PageCount,
                      PageSize = litigationRequestArbitrationReportExplanationRequests.PageSize,
                      TotalCount = litigationRequestArbitrationReportExplanationRequests.TotalCount
                  },
                IsDone = true
            };


        }
        public async Task<BaseResponse> GetLitigationRequestForReportExplanationRequestForArbitrator(Guid requestId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequestArbitrationReportExplanationRequest = (await _repository.GetQueryable<LitigationRequestArbitrationReportExplanationRequest, Guid>())
                         .Include(x => x.LitigationRequestArbitrationReportExplanationRequestFiles)
                         .ThenInclude(x => x.ExplanationRequestFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Area)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.City)
                         .Include(x => x.LitigationRequestArbitrationReport)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.ApplicantCitizen)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.ContractFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.AssociationRegistrationCertificateFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.FoundingLetterFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.InternalRegulationsFile)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequestArbitrationReport.LitigationRequest.Contract.LitigationRequestContractType).AsNoTracking()
                         .Where(x => x.Id == requestId).ToList().FirstOrDefault();

            if (litigationRequestArbitrationReportExplanationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            bool allowAdd = true;

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequestArbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest);

            if (string.IsNullOrEmpty(litigationRequestArbitrationReportExplanationRequest.Response))
                allowAdd = true;
            else
                allowAdd = false;

            var reportExplanationRequest = _mapper.Map<LitigationRequestArbitrationReportExplanationRequestGetDto>(litigationRequestArbitrationReportExplanationRequest);

            return new DataResponse<LitigationRequestForArbitrationReportExplanationRequestDto>()
            {
                Data = new LitigationRequestForArbitrationReportExplanationRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    Report = reportExplanationRequest
                },
                IsDone = true,
            };
        }
        public async Task<BaseResponse> AddArbitrationReportExplanationRequestForArbitrator(LitigationRequestArbitrationReportExplanationResponseDto requestPayload)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var arbitrationReportExplanationRequest = (await _repository.GetQueryable<LitigationRequestArbitrationReportExplanationRequest, Guid>())
                    .Include(x => x.LitigationRequestArbitrationReport)
                    .ThenInclude(x => x.LitigationRequest)
                    .ThenInclude(x => x.ApplicantCitizen)
                    .Include(x => x.LitigationRequestArbitrationReport)
                    .ThenInclude(x => x.LitigationRequest)
                    .ThenInclude(x => x.ArbitratorCitizen)
                     .Include(x => x.LitigationRequestArbitrationReport)
                    .ThenInclude(x => x.LitigationRequest)
                    .ThenInclude(x => x.Contract)
                    .ThenInclude(x => x.FirstPartyCitizen)
                     .Include(x => x.LitigationRequestArbitrationReport)
                    .ThenInclude(x => x.LitigationRequest)
                    .ThenInclude(x => x.Contract)
                    .ThenInclude(x => x.SecondPartyCitizen)
                     .Include(x => x.LitigationRequestArbitrationReport)
                    .ThenInclude(x => x.LitigationRequest)
                    .ThenInclude(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                     .Include(x => x.LitigationRequestArbitrationReport)
                    .ThenInclude(x => x.LitigationRequest)
                    .ThenInclude(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                    .FirstOrDefault(x => x.Id == requestPayload.Id);

                if (arbitrationReportExplanationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };
                arbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.InternalStatusId = RequestStatus.ExplanationResponsed;
                arbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest.ExternalStatusId = RequestStatus.ExplanationResponsed;

                arbitrationReportExplanationRequest.Response = requestPayload.Response;
                arbitrationReportExplanationRequest.ResponseOn = DateTime.Now;

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                switch (arbitrationReportExplanationRequest.RequestArbitrationReportExplanationRequestParty)
                {
                    case LitigationRequestParty.Plaintiff:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = arbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequestId,
                            ShowForApplicant = false,
                            LogMessage = "تم الرد على طلب استفسار على حكم التحكيم من قبل المدعى"
                        });
                        break;
                    case LitigationRequestParty.Defendant:
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = arbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequestId,
                            ShowForApplicant = false,
                            LogMessage = "تم الرد على طلب استفسار على حكم التحكيم من قبل المدعى عليه"
                        });
                        break;
                }


                // Send Notification
                // تم الرد على طلب تفسير حكم التحكيم
                try
                {
                    var litigationRequest = arbitrationReportExplanationRequest.LitigationRequestArbitrationReport.LitigationRequest;
                    var msg = "";
                    msg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ExplanationReport_RepliedOn).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

                    if (arbitrationAdmin.IsDone)
                    {


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
                 ActorId = (Guid)arbitrationAdmin.Data?.Id,
                 RequestId=litigationRequest.Id,
                 CreatedOn= DateTime.Now,
                 RequestTypeId=RequestType.LitigationRequest,
                 ActorTypeId=NotificationActorType.Employee,
                 Message=msg
                 }

                 };
                        List<string> emails = new List<string>() {
                      litigationRequest.ArbitratorCitizen.Email,
                         litigationRequest.ApplicantCitizen.Email,
                           arbitrationAdmin.Data?.Email
                         };

                        List<string> recipients = new List<string>()
                     {
                     litigationRequest.ArbitratorCitizen.Mobile,
                         litigationRequest.ApplicantCitizen.Mobile,
                           arbitrationAdmin.Data?.Mobile
                     };
                        if (arbitrationReportExplanationRequest.RequestArbitrationReportExplanationRequestParty == LitigationRequestParty.Plaintiff)
                        {
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
                        }

                        if (arbitrationReportExplanationRequest.RequestArbitrationReportExplanationRequestParty == LitigationRequestParty.Defendant)
                        {
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
                        }

                        _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                        {
                            To = emails,
                            Subject = "اضافة طلب تفسير على حكم التحكيم",
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
            catch (Exception ex)
            {
                new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return await GetLitigationRequestForReportExplanationRequestForArbitrator(requestPayload.LitigationRequestId);
        }
        #endregion
    }
}
