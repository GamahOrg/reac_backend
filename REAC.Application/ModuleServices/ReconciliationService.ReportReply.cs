using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{
    public partial class ReconciliationService
    {
        #region Report Reply
        public async Task<BaseResponse> GetCitizenLitigationRequestForReportReply(CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions)
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
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType)
                                      .Where(x => x.RequestCode != null
                                      & x.ApplicantCitizenId == referenceId & x.LitigationRequestReconciliationReports.Count > 0

                                      |

                                      x.LitigationRequestReconciliationReports.Count > 0
                                      & x.RequestCode != null & x.Contract.FirstPartyCitizenId == referenceId
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration

                                      |

                                      x.LitigationRequestReconciliationReports.Count > 0
                                      & x.RequestCode != null & x.Contract.SecondPartyCitizenId == referenceId
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration

                                      |

                                      x.LitigationRequestReconciliationReports.Count > 0
                                      & x.RequestCode != null & x.Contract.FirstPartyFacility.CitizenId == referenceId
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration

                                      |

                                      x.LitigationRequestReconciliationReports.Count > 0
                                      & x.RequestCode != null & x.Contract.SecondPartyFacility.CitizenId == referenceId
                                      & x.LitigationRequestCategoryId != LitigationRequestCategory.Arbitration).
                                      OrderByDescending(x => x.CreatedOn).ToList();

            List<LitigationRequestMircoDto> litigationRequestMircoDtos = new List<LitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests)
                {
                    LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    bool allowAdd = true;
                    bool IsDone = false;
                    if (litigationRequest.IsFinished.HasValue)
                    {
                        if (litigationRequest.IsFinished.Value)
                        {
                            allowAdd = false;
                            IsDone = true;
                        }
                    }
                    if (!IsDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                            allowAdd = false;
                        else
                        {

                            if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestPartiesDecisionAdded)
                                allowAdd = false;
                            else
                            {

                                if (litigationRequest.ApplicantCitizenId == referenceId)
                                {
                                    if (litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                                        allowAdd = false;

                                }
                                if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                                {
                                    if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                    {
                                        if (litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                                            allowAdd = false;
                                    }
                                }
                                if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                                {
                                    if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                    {
                                        if (litigationRequest.MintuesOfMeetingApprovalDefendantStatus.HasValue)
                                            allowAdd = false;
                                    }
                                }
                                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                                {
                                    if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                                    {
                                        if (litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                                            allowAdd = false;
                                    }
                                }
                                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                                {
                                    if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                                    {
                                        if (litigationRequest.MintuesOfMeetingApprovalDefendantStatus.HasValue)
                                            allowAdd = false;
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

            return new DataResponse<List<LitigationRequestMircoDto>>()
            {
                Data = litigationRequestMircoDtos,
                IsDone = true
            };
        }
        public async Task<BaseResponse> GetCitizenLitigationRequestForReportReply(Guid requestId, CancellationToken cancellationToken)
        {

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.ReformerCitizen)
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestSessions)
                         .Include(x => x.LitigationRequestReconciliationReports)
                         .ThenInclude(x => x.LitigationRequestMeetingReconciliationItems)
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

                         .Where(x => x.Id == requestId & x.LitigationRequestReconciliationReports.Count > 0).ToList().FirstOrDefault();
            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };
            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            var reports = _mapper.Map<List<LitigationRequestReconciliationReportCreateUpdateDto>>(litigationRequest?.LitigationRequestReconciliationReports);

            LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();

            bool allowAdd = true;
            bool IsDone = false;
            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    allowAdd = false;
                    IsDone = true;
                }
            }
            if (!IsDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Reconciliation)
                    allowAdd = false;
                else
                {

                    if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestPartiesDecisionAdded)
                        allowAdd = false;
                    else
                    {

                        if (litigationRequest.ApplicantCitizenId == referenceId)
                        {
                            if (litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                                allowAdd = false;

                        }
                        if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                        {
                            if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                            {
                                if (litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                                    allowAdd = false;
                            }
                        }
                        if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                        {
                            if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                            {
                                if (litigationRequest.MintuesOfMeetingApprovalDefendantStatus.HasValue)
                                    allowAdd = false;
                            }
                        }
                        if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                        {
                            if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                            {
                                if (litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                                    allowAdd = false;
                            }
                        }
                        if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                        {
                            if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                            {
                                if (litigationRequest.MintuesOfMeetingApprovalDefendantStatus.HasValue)
                                    allowAdd = false;
                            }
                        }

                        if (litigationRequest.InternalStatus.Id == REAC.Domain.Shared.Enum.RequestStatus.WaiverOfRight || litigationRequest.InternalStatus.Id == REAC.Domain.Shared.Enum.RequestStatus.WaiverOfTheLitigation)
                        {
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                {
                                    litigationRequest.DefendantAgreementRequestResponse = ApproveRejectStatus.Approve;
                                    allowAdd = false;
                                }
                            }
                        };

                    }
                }
            }
            if (litigationRequest.LitigationRequestReconciliationReports.Count == 0)
            {
                allowAdd = false;
            }
            return new DataResponse<LitigationRequestForReportDto>()
            {
                Data = new LitigationRequestForReportDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    Report = reports.FirstOrDefault()
                },
                IsDone = true,
            };
        }
        public async Task<BaseResponse> AddCitizenLitigationRequestForReportReply(Guid requestId, ApproveRejectStatus requestMinutesOfMeetingApprovalStatus, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            var msg = "";
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.ReformerCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Where(x => x.Id == requestId).ToList().FirstOrDefault();

            bool isDone = false;
            if (litigationRequest != null)
            {
                if (litigationRequest.ApplicantCitizen != null)
                {
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        if (!litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                        {
                            litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus = requestMinutesOfMeetingApprovalStatus;
                            litigationRequest.MintuesOfMeetingApprovalPlaintiffStatusOn = DateTime.Now;
                            isDone = true;

                            if (requestMinutesOfMeetingApprovalStatus == ApproveRejectStatus.Approve)
                            {
                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم قبول محضر الصلح من قبل المدعى"
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
                                    LogMessage = "تم رفض محضر الصلح من قبل المدعى"
                                });
                            }
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.FirstPartyCitizen != null)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            if (!litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                            {
                                litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus = requestMinutesOfMeetingApprovalStatus;
                                litigationRequest.MintuesOfMeetingApprovalPlaintiffStatusOn = DateTime.Now;
                                isDone = true;
                                if (requestMinutesOfMeetingApprovalStatus == ApproveRejectStatus.Approve)
                                {
                                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                    {
                                        Id = Guid.NewGuid(),
                                        CreatedOn = DateTime.Now,
                                        RequestId = litigationRequest.Id,
                                        ShowForApplicant = false,
                                        LogMessage = "تم قبول محضر الصلح من قبل المدعى"
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
                                        LogMessage = "تم رفض محضر الصلح من قبل المدعى"
                                    });
                                }
                            }
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.SecondPartyCitizen != null)
                    {
                        if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                        {
                            if (!litigationRequest.MintuesOfMeetingApprovalDefendantStatus.HasValue)
                            {
                                litigationRequest.MintuesOfMeetingApprovalDefendantStatus = requestMinutesOfMeetingApprovalStatus;
                                litigationRequest.MintuesOfMeetingApprovalDefendantStatusOn = DateTime.Now;
                                isDone = true;
                                if (requestMinutesOfMeetingApprovalStatus == ApproveRejectStatus.Approve)
                                {
                                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                    {
                                        Id = Guid.NewGuid(),
                                        CreatedOn = DateTime.Now,
                                        RequestId = litigationRequest.Id,
                                        ShowForApplicant = false,
                                        LogMessage = "تم قبول محضر الصلح من قبل المدعى عليه"
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
                                        LogMessage = "تم رفض محضر الصلح من قبل المدعى عليه"
                                    });
                                }
                            }
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.FirstPartyFacility != null)
                    {
                        if (litigationRequest.Contract.FirstPartyFacility.CitizenId == referenceId)
                        {
                            if (!litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
                            {
                                litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus = requestMinutesOfMeetingApprovalStatus;
                                litigationRequest.MintuesOfMeetingApprovalPlaintiffStatusOn = DateTime.Now;
                                isDone = true;
                                if (requestMinutesOfMeetingApprovalStatus == ApproveRejectStatus.Approve)
                                {
                                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                    {
                                        Id = Guid.NewGuid(),
                                        CreatedOn = DateTime.Now,
                                        RequestId = litigationRequest.Id,
                                        ShowForApplicant = false,
                                        LogMessage = "تم قبول محضر الصلح من قبل المدعى"
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
                                        LogMessage = "تم رفض محضر الصلح من قبل المدعى"
                                    });
                                }
                            }
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.SecondPartyFacility != null)
                    {
                        if (litigationRequest.Contract.SecondPartyFacility.CitizenId == referenceId)
                        {
                            if (!litigationRequest.MintuesOfMeetingApprovalDefendantStatus.HasValue)
                            {
                                litigationRequest.MintuesOfMeetingApprovalDefendantStatus = requestMinutesOfMeetingApprovalStatus;
                                litigationRequest.MintuesOfMeetingApprovalDefendantStatusOn = DateTime.Now;
                                isDone = true;
                                if (requestMinutesOfMeetingApprovalStatus == ApproveRejectStatus.Approve)
                                {
                                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                    {
                                        Id = Guid.NewGuid(),
                                        CreatedOn = DateTime.Now,
                                        RequestId = litigationRequest.Id,
                                        ShowForApplicant = false,
                                        LogMessage = "تم قبول محضر الصلح من قبل المدعى عليه"
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
                                        LogMessage = "تم رفض محضر الصلح من قبل المدعى عليه"
                                    });
                                }
                            }
                        }
                    }
                }
            }


            if (litigationRequest.MintuesOfMeetingApprovalDefendantStatus.HasValue & litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus.HasValue)
            {
                if (litigationRequest.MintuesOfMeetingApprovalDefendantStatus == ApproveRejectStatus.Approve & litigationRequest.MintuesOfMeetingApprovalPlaintiffStatus == ApproveRejectStatus.Approve)
                {
                    litigationRequest.InternalStatusId = RequestStatus.ReconciliationWasMade;
                    litigationRequest.ExternalStatusId = RequestStatus.ReconciliationWasMade;
                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.LitigationRequestPartiesDecisionAdded;

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم قبول محصر الصلح"
                    });

                    // Send Notification
                    // تم قبول الصلح
                    try
                    {
                        msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasMade).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        await SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Added");
                    }
                    catch (Exception e)
                    {


                    }


                }
                else
                {
                    litigationRequest.InternalStatusId = RequestStatus.ReconciliationWasNotMade;
                    litigationRequest.ExternalStatusId = RequestStatus.ReconciliationWasNotMade;
                    if (litigationRequest.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation)
                    {
                        litigationRequest.LitigationRequestStage = LitigationRequestStage.Arbitration;
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.ArbitratorSelectionDuration;

                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم رفض الصلح والانتقال إلى التحكيم"
                        });

                        // Send Notification
                        // تم رفض الصلح والانتقال إلى التحكيم
                        try
                        {
                            bool isAdminArbitrator = true;
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasNotMadeAndMoveToArbitration).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            await SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Refused", isAdminArbitrator);
                        }
                        catch (Exception e)
                        {

                        }

                    }
                    else
                    {
                        // Send Notification
                        // تم رفض الصلح
                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم رفض الصلح"
                        });

                        try
                        {
                            msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Decision_ReconciliationWasNotMade).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            await SendNotificationMSWithAllTypes(msg, litigationRequest, "Citizen Litigation Request For Report Reply Refused");

                        }
                        catch (Exception e)
                        {


                        }
                    }
                }
            }

            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            return await GetCitizenLitigationRequestForReportReply(requestId, cancellationToken);

        }

        private async Task SendNotificationMSWithAllTypes(string msg, LitigationRequest litigationRequest, string Subject, bool? isAdminArbitrator = null)
        {
            var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

            //ApplicantCitizen
            var objEmail = new SendEmailDto()
            {
                Body = msg,
                Subject = Subject,
                To = new List<string>() { litigationRequest.ApplicantCitizen.Email }

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
                RequestTypeId = RequestType.ReformationReport,
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
                    RequestTypeId = RequestType.ReformationReport,
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
                    RequestTypeId = RequestType.ReformationReport,
                    Message = msg,
                });
            }

            //مدير ادارة التحكيم
            if (isAdminArbitrator is not null)
            {
                if ((bool)isAdminArbitrator)
                {
                    objEmail.To.Add(deptAdminInfoArbitration?.Data?.Email);
                    objSms.recipients.Add(deptAdminInfoArbitration?.Data?.Mobile);

                    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                    {
                        ActorId = deptAdminInfoArbitration.Data.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = litigationRequest.Id,
                        RequestTypeId = RequestType.ReformationReport,
                        Message = msg,
                    });
                }

            }

            if (litigationRequest?.Contract?.FirstPartyCitizen != null && litigationRequest?.Contract?.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyCitizen.Mobile);

                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationReport,
                    Message = msg,
                });
            }
            if (litigationRequest?.Contract?.SecondPartyCitizen != null)
            {
                objEmail.To.Add(litigationRequest?.Contract?.SecondPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest?.Contract?.SecondPartyCitizen.Mobile);

                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Defendant,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationReport,
                    Message = msg,
                });
            }
            //litigationRequest.Contract.FirstPartyFacility.Citizen
            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyFacility.Email);
                objSms.recipients.Add(litigationRequest.Contract.FirstPartyFacility?.Citizen.Mobile);

                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationReport,
                    Message = msg,
                });
            }
            if (litigationRequest?.Contract?.SecondPartyFacility != null)
            {
                objEmail.To.Add(litigationRequest?.Contract?.SecondPartyFacility.Email);
                objSms.recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility?.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Defendant,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = RequestType.ReformationReport,
                    Message = msg,
                });

            }

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
        }

        #endregion

    }
}
