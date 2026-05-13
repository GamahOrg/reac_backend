using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.Application.ModuleServices
{
    public partial class ArbitrationService
    {


        #region Report

        public async Task<BaseResponse> AddArbitrationReport(WritingArbitrationReportRequestPayload requestPayload, CancellationToken cancellationToken)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.LitigationRequestArbitrationReports)
                        .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                        .ThenInclude(x => x.ArbitratorCitizen)
                        .Include(x => x.ArbitratorCitizen)
                        .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.FirstPartyCitizen)
                    .Include(x => x.Contract)
                    .ThenInclude(x => x.SecondPartyCitizen)
                    .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                    .Include(x => x.LitigationRequestArbitratorLists)
                    .ThenInclude(x => x.ArbitratorCitizen)
                     .FirstOrDefault(x => x.Id == requestPayload.Id & x.ArbitratorCitizenId == referenceId);

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                bool allowAdd = true;

                if (litigationRequest.LitigationRequestArbitrationReports.Count() > 0)
                    allowAdd = false;

                if (allowAdd)
                {
                    LitigationRequestArbitrationReport litigationRequestReconciliationReport = new LitigationRequestArbitrationReport()
                    {
                        Id = Guid.NewGuid(),
                        JudgmentDate = DateTime.Now,
                        Facts = requestPayload.Facts,
                        JudgmentDecisionText = requestPayload.JudgmentDecisionText,
                        MeetingLocationId = requestPayload.MeetingLocationId,
                        ModificationRequest = requestPayload.ModificationRequest,
                        Reasons = requestPayload.Reasons,
                        JudgmentDecisionPlace = requestPayload.JudgmentDecisionPlace,
                        JudgmentDecisionDate = DateTime.Now,
                        LitigationRequestId = requestPayload.Id,
                    };

                    if (litigationRequest.ArbitratorsCount == 1)
                    {
                        litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.JudgmentDecisionIssued;
                        litigationRequest.InternalStatusId = RequestStatus.JudgmentDecisionIssued;
                        litigationRequest.ExternalStatusId = RequestStatus.JudgmentDecisionIssued;
                        litigationRequest.IsFinished = true;
                        // adding publish 
                        litigationRequestReconciliationReport.IsPublished = true;
                        litigationRequestReconciliationReport.PublishedDate = DateTime.Now;

                    }
                    else
                    {
                        litigationRequest.InternalStatusId = RequestStatus.WaitingForArbitratorsApproval;
                        litigationRequest.ExternalStatusId = RequestStatus.WaitingForArbitratorsApproval;
                    }

;

                    litigationRequest.IsMinutesOfMeetingApprovalRequired = true;
                    litigationRequest.JudgmentDecisionIssuingDateTime = DateTime.Now;
                    litigationRequest.EndArbitrationDate = DateTime.Now;

                    await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationRequest.Id,
                        ShowForApplicant = false,
                        LogMessage = "تم الإنتهاء من إضافة محضر التحكيم"
                    });

                    await _repository.AddAsync<LitigationRequestArbitrationReport, Guid>(litigationRequestReconciliationReport);

                    var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                    if (!repositorySaveResponse.IsDone)
                    {
                        return repositorySaveResponse;
                    }
                    if (litigationRequest.ArbitratorsCount == 1)
                    {
                        // Send Notification
                        // تم إضافة حكم التحكيم
                        try
                        {
                            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                            if (arbitrationAdmin.IsDone)
                            {
                                var portalUrl = _settingService.GetBySettingKey(SysSettingKeyConstants.REACPortalUrl).Result.Data?.Value;

                                var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationService_JudgmentAdded).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                msg = msg?.Replace("{PortalUrl}", portalUrl);
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                string body;
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationService_JudgmentAdded).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                }


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
                                        ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.Id,
                                        RequestId = litigationRequest.Id,
                                        CreatedOn = DateTime.Now,
                                        RequestTypeId = RequestType.LitigationRequest,
                                        ActorTypeId = NotificationActorType.Plaintiff,
                                        Message = msg
                                    });
                                    emails.Add(litigationRequest.Contract.FirstPartyFacility.Citizen?.Email);
                                    recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen?.Mobile);
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
                                        ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.Id,
                                        RequestId = litigationRequest.Id,
                                        CreatedOn = DateTime.Now,
                                        RequestTypeId = RequestType.LitigationRequest,
                                        ActorTypeId = NotificationActorType.Defendant,
                                        Message = msg
                                    });
                                    emails.Add(litigationRequest.Contract.SecondPartyFacility.Citizen?.Email);
                                    recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen?.Mobile);
                                }
                                ;

                                foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorLists)
                                {
                                    notifications.Add(new SysNotificationDto
                                    {
                                        ActorId = (Guid)arbitrator.Id,
                                        RequestId = litigationRequest.Id,
                                        CreatedOn = DateTime.Now,
                                        RequestTypeId = RequestType.LitigationRequest,
                                        ActorTypeId = NotificationActorType.Arbitrator,
                                        Message = msg
                                    });
                                    emails.Add(arbitrator.ArbitratorCitizen.Email);
                                    recipients.Add(arbitrator.ArbitratorCitizen.Mobile);
                                }
                                emails = emails.Where(e => e != null).ToList();
                                recipients = recipients.Where(r => r != null).ToList();
                                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                {
                                    To = emails,
                                    Subject = "حكم هيئة التحكيم",
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
                        //// Send Notification
                        ////  يرجى الموافقه على حكم التحكيم ليتم اصداره تم إضافة حكم التحكيم
                        try
                        {
                            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                            if (arbitrationAdmin.IsDone)
                            {

                                var msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationService_JudgmentAddedForAgreement).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                string body;
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationService_JudgmentAddedForAgreement).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                }


                                List<SysNotificationDto> notifications = new List<SysNotificationDto>();

                                List<string> emails = new List<string>();

                                List<string> recipients = new List<string>();
                                foreach (var arbitrator in litigationRequest.LitigationRequestArbitratorAssigningRequests)
                                {
                                    notifications.Add(new SysNotificationDto
                                    {
                                        ActorId = (Guid)arbitrator.CitizenId,
                                        RequestId = litigationRequest.Id,
                                        CreatedOn = DateTime.Now,
                                        RequestTypeId = RequestType.LitigationRequest,
                                        ActorTypeId = NotificationActorType.Arbitrator,
                                        Message = msg
                                    });
                                    emails.Add(arbitrator.ArbitratorCitizen?.Email);
                                    recipients.Add(arbitrator.ArbitratorCitizen?.Mobile);
                                }

                                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                {
                                    To = emails,
                                    Subject = " الموافقه على حكم هيئة التحكيم",
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
            return await GetLitigationRequestForReport(requestPayload.Id, cancellationToken);
        }
        public async Task<BaseResponse> GetLitigationRequestForReport(Guid requestId, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.DefendantCitizen)
                         .Include(x => x.DefendantFacility)
                         .Include(x => x.ArbitratorCitizen)
                         .ThenInclude(x => x.SigntureFile)
                         .Include(x => x.ArbitratorCitizen.City)
                         .Include(x => x.Area)
                         .Include(x => x.City)
                         .Include(x => x.LitigationRequestSessions)
                         .Include(x => x.LitigationRequestArbitrationReports)
                         .ThenInclude(x => x.MeetingLocation)
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
                         .Include(x => x.LitigationRequestInvoice)
                         .Where(x => x.Id == requestId
                         //   & x.ArbitratorCitizenId == referenceId
                         //   | arbitrationAdmin.Data.Id == referenceId
                         //   | x.ApplicantCitizenId == referenceId & x.Id == requestId
                         //|
                         //x.Contract.FirstPartyCitizenId == referenceId & x.Id == requestId
                         //|
                         //  x.Contract.SecondPartyCitizenId == referenceId & x.Id == requestId
                         //|
                         // x.Contract.FirstPartyFacility.CitizenId == referenceId & x.Id == requestId
                         //|
                         //x.Contract.SecondPartyFacility.CitizenId == referenceId & x.Id == requestId
                         ).AsNoTracking().ToList().FirstOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            string base64String = string.Empty;

            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

            //modifiy fees of arbitration amount 
            if (request.LitigationRequestAmount != null)
            {
                request.LitigationRequestAmount.Fees = litigationRequest.LitigationRequestInvoice?.TotalAmount ?? 0;
            }
            else
            {
                var MinRequestAmount = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())
                                 .Include(x => x.LitigationRequestFees)
                               .Where(x => x.FromAmount == 0).FirstOrDefault();

                request.LitigationRequestAmount = new SysLitigationRequestAmountCreateUpdateDto()
                {
                    Fees = MinRequestAmount.LitigationRequestFees.FirstOrDefault().ArbitrationFees,

                };
            }
            if (!string.IsNullOrEmpty(litigationRequest.ArbitratorCitizen?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), litigationRequest.ArbitratorCitizen?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                if (!string.IsNullOrEmpty(inputFilePath) && File.Exists(inputFilePath))
                {
                    byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                    base64String = Convert.ToBase64String(fileBytes);
                }
            }
            if (request != null && !string.IsNullOrEmpty(base64String))
            {
                request.ArbitratorCitizen.SigntureFile = base64String;
            }
            var reports = _mapper.Map<List<LitigationRequestArbitrationReportCreateUpdateDto>>(litigationRequest?.LitigationRequestArbitrationReports);

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
                    allowAdd = false;
                else
                {
                    if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.JudgmentDecisionIssuing)
                        allowAdd = false;
                    else
                    {
                        if (litigationRequest?.LitigationRequestArbitrationReports != null)
                        {
                            if (litigationRequest.LitigationRequestArbitrationReports.Count > 0)
                            {
                                allowAdd = false;
                                reports.FirstOrDefault().MeetingLocationName = litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault()?.MeetingLocation.MeetingLocationName;
                            }
                        }
                    }
                }
                if (reports.Count > 0)
                {
                    reports.FirstOrDefault().ArbitratorCitizenName = litigationRequest.ArbitratorCitizen?.FullName;
                }
            }

            return new DataResponse<LitigationRequestForArbitrationReportDto>()
            {
                Data = new LitigationRequestForArbitrationReportDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    Report = reports.FirstOrDefault()
                },
                IsDone = true,
            };

        }
        public async Task<BaseResponse> GetLitigationRequestForReport(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
           .Include(x => x.Area).Include(x => x.City)
                                      .Include(x => x.LitigationRequestSessions)
                                      .Include(x => x.LitigationRequestArbitrationReports)
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
                                      .Include(x => x.Contract.SecondPartyCitizen).Include(x => x.Contract.LitigationRequestContractType);


            litigationRequests = entitiesQuerable.Where(x => x.ArbitratorCitizenId == referenceId &
                                      x.RequestCode != null
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
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility?.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen?.FullName;
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

                            if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.JudgmentDecisionIssued)
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
        public async Task<BaseResponse> GetLitigationRequestForReportForAdmin(LitigationRequestReportAdminFilterDto filter, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            if (!arbitrationAdmin.IsDone || arbitrationAdmin.Data?.Id != referenceId)
                return new BaseResponse { IsDone = false, Message = "غير مصرح لك بالوصول، هذه الخدمة متاحة لرئيس هيئة التحكيم فقط" };

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.ArbitratorCitizen)
                .Include(x => x.Area).Include(x => x.City)
                .Include(x => x.LitigationRequestSessions)
                .Include(x => x.LitigationRequestArbitrationReports)
                .Include(x => x.LitigationRequestArbitrationCorrectionRequests)
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
                .Include(x => x.Contract.SecondPartyCitizen);

            var query = entitiesQuerable.Where(x =>
                x.RequestCode != null
                & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                & (x.InternalStatusId == RequestStatus.JudgmentDecisionIssued || x.InternalStatusId == RequestStatus.WaitingForArbitratorsApproval)
                & x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation);

            // البحث برقم الدعوى
            if (!string.IsNullOrWhiteSpace(filter.RequestCode))
                query = query.Where(x => x.RequestCode!.Contains(filter.RequestCode));

            // البحث باسم المحكم
            if (!string.IsNullOrWhiteSpace(filter.ArbitratorName))
                query = query.Where(x => x.ArbitratorCitizen != null && x.ArbitratorCitizen.FullName.Contains(filter.ArbitratorName));

            // البحث باسم أحد أطراف الدعوى
            if (!string.IsNullOrWhiteSpace(filter.PartyName))
                query = query.Where(x =>
                    (x.Contract.FirstPartyCitizen != null && x.Contract.FirstPartyCitizen.FullName.Contains(filter.PartyName)) ||
                    (x.Contract.FirstPartyFacility != null && x.Contract.FirstPartyFacility.ArabicFacilityName.Contains(filter.PartyName)) ||
                    (x.Contract.SecondPartyCitizen != null && x.Contract.SecondPartyCitizen.FullName.Contains(filter.PartyName)) ||
                    (x.Contract.SecondPartyFacility != null && x.Contract.SecondPartyFacility.ArabicFacilityName.Contains(filter.PartyName)) ||
                    (x.DefendantCitizen != null && x.DefendantCitizen.FullName.Contains(filter.PartyName)) ||
                    (x.DefendantFacility != null && x.DefendantFacility.ArabicFacilityName.Contains(filter.PartyName)));

            // التصفية حسب تاريخ تقديم الدعوى
            if (filter.SubmissionDateFrom.HasValue)
                query = query.Where(x => x.SentOn >= filter.SubmissionDateFrom.Value);
            if (filter.SubmissionDateTo.HasValue)
                query = query.Where(x => x.SentOn <= filter.SubmissionDateTo.Value);

            // التصفية حسب تاريخ قبول الدعوى من المدقق
            if (filter.AuditorAcceptanceDateFrom.HasValue)
                query = query.Where(x => x.StartArbitrationDate >= filter.AuditorAcceptanceDateFrom.Value);
            if (filter.AuditorAcceptanceDateTo.HasValue)
                query = query.Where(x => x.StartArbitrationDate <= filter.AuditorAcceptanceDateTo.Value);

            // التصفية حسب تاريخ قبول المحكم الدعوى
            if (filter.ArbitratorAcceptanceDateFrom.HasValue)
                query = query.Where(x => x.ArbitratorsChairmanNominationListSelectionDoneDateTime >= filter.ArbitratorAcceptanceDateFrom.Value);
            if (filter.ArbitratorAcceptanceDateTo.HasValue)
                query = query.Where(x => x.ArbitratorsChairmanNominationListSelectionDoneDateTime <= filter.ArbitratorAcceptanceDateTo.Value);

            // التصفية حسب تاريخ إصدار الحكم
            if (filter.JudgmentDateFrom.HasValue)
                query = query.Where(x => x.JudgmentDecisionIssuingDateTime >= filter.JudgmentDateFrom.Value);
            if (filter.JudgmentDateTo.HasValue)
                query = query.Where(x => x.JudgmentDecisionIssuingDateTime <= filter.JudgmentDateTo.Value);

            var litigationRequests = query
                .OrderByDescending(x => x.CreatedOn)
                .AsSplitQuery()
                .AsNoTracking()
                .GetPaged(filter.PageIndex, filter.PageSize);

            var dtos = new List<LitigationRequestMircoForAdminReportDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    bool allowAdd = true;
                    bool isDone = false;

                    if (litigationRequest.IsFinished.HasValue && litigationRequest.IsFinished.Value)
                    {
                        allowAdd = false;
                        isDone = true;
                    }

                    if (!isDone)
                    {
                        if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                            allowAdd = false;
                        else if (litigationRequest?.LitigationRequestSubStage == LitigationRequestSubStage.JudgmentDecisionIssued)
                            allowAdd = false;
                        else if (litigationRequest?.LitigationRequestArbitrationReports?.Count > 0)
                            allowAdd = false;
                    }

                    dtos.Add(new LitigationRequestMircoForAdminReportDto
                    {
                        Id = litigationRequest!.Id,
                        RequestDate = litigationRequest.CreatedOn,
                        RequestCode = litigationRequest.RequestCode,
                        FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null)
                            ? litigationRequest.Contract.FirstPartyFacility?.ArabicFacilityName ?? string.Empty
                            : litigationRequest.Contract.FirstPartyCitizen?.FullName ?? string.Empty,
                        SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null)
                            ? litigationRequest.DefendantFacility?.ArabicFacilityName ?? string.Empty
                            : litigationRequest.Contract.SecondPartyCitizen?.FullName ?? string.Empty,
                        RequestTypeName = litigationRequest.Contract.LitigationRequestContractType?.TypeName ?? string.Empty,
                        ArbitratorName = litigationRequest.ArbitratorCitizen?.FullName,
                        Status = litigationRequest.InternalStatus?.RequestStatus,
                        IsDone = allowAdd ? "--" : "تم",
                        SubmissionDate = litigationRequest.SentOn,
                        AuditorAcceptanceDate = litigationRequest.StartArbitrationDate,
                        ArbitratorAcceptanceDate = litigationRequest.ArbitratorsChairmanNominationListSelectionDoneDateTime ?? litigationRequest.StartArbitrationDate,
                        JudgmentDate = litigationRequest.JudgmentDecisionIssuingDateTime,
                        CorrectionRequestStatus = litigationRequest.LitigationRequestArbitrationCorrectionRequests?
                            .OrderByDescending(x => x.CreatedOn)
                            .FirstOrDefault() is { } cr
                            ? cr.ApproveRejectStatus == null ? "تحت المراجعة"
                              : cr.ApproveRejectStatus == ApproveRejectStatus.Approve ? "مقبول"
                              : "مرفوض"
                            : null,
                    });
                }
            }

            return new DataResponse<PagedResult<LitigationRequestMircoForAdminReportDto>>
            {
                Data = new PagedResult<LitigationRequestMircoForAdminReportDto>
                {
                    Results = dtos,
                    CurrentPage = litigationRequests!.CurrentPage,
                    PageCount = litigationRequests.PageCount,
                    PageSize = litigationRequests.PageSize,
                    TotalCount = litigationRequests.TotalCount
                },
                IsDone = true
            };
        }

        #endregion

        #region Nafath
        public async Task<BaseResponse> CallNafathForArbitrationReport(Guid requestId, ApproveRejectStatus agreementRequestStatus)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                              .AsNoTracking()
                              .FirstOrDefault(x => x.Id == requestId);
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                              .AsNoTracking()
                              .FirstOrDefault(x => x.Id == referenceId);
                if (litigationRequest is not null & citizen is not null)
                {
                    var nafathReconciliationReportResponse = await _nafathService.NafathReconciliationReport(
                         new NafathRequestDto() { IsRegister = false, NationalId = citizen.IdentityNumber },
                         new NafathReconciliationReportTransactionDto()
                         {
                             LitigationRequestId = litigationRequest.Id,
                             NationalId = citizen.IdentityNumber,
                             RequestMinutesOfMeetingApprovalStatus = agreementRequestStatus
                         });
                    return nafathReconciliationReportResponse;
                }
                return new BaseResponse() { IsDone = false, Message = "خطأ في البيانات المدخله" };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }
        }
        #endregion
    }
}
