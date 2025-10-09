using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Security.Entities;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.EntityFrameworkCore.Entities.Integration;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.Arbitration;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationRequestArbitrationReportService : ILitigationRequestArbitrationReportService
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ILoggerManager _loggerManager;
        private readonly IJwtReader _jwtReader;
        private readonly ISysSettingService _sysSettingService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private UserManager<User> _userManager;

        public LitigationRequestArbitrationReportService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysSettingService sysSettingService, IServiceProvider serviceProvider, ISysEmployeeService employeeService, INotificationService notificationService, UserManager<User> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _loggerManager = loggerManager;
            _jwtReader = jwtReader;
            _sysSettingService = sysSettingService;
            _serviceProvider = serviceProvider;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<BaseResponse> AddArbitrationMinutesReply(Guid requestId, string nationalId, ApproveRejectStatus requestMinutesOfMeetingApprovalStatus)
        {
            try
            {
                Guid? referenceId = _userManager.Users.FirstOrDefault(u => u.UserName == nationalId)?.ReferenceId;


                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                                      .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                                      .ThenInclude(x => x.ArbitratorCitizen)
                                      .Include(x => x.ApplicantCitizen)
                                      .Include(x => x.Contract)
                                      .ThenInclude(x => x.FirstPartyCitizen)
                                      .Include(x => x.Contract)
                                      .ThenInclude(x => x.SecondPartyCitizen)
                                      .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                                      .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                                      .Include(x => x.LitigationRequestArbitrationReports)
                                      .Where(x => x.Id == requestId).FirstOrDefault();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };
                var litigationRequestAssign = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault(x => x.CitizenId == referenceId);

                if (litigationRequestAssign != null)
                {
                    litigationRequestAssign.ResponseArbitrationMinutes = requestMinutesOfMeetingApprovalStatus == ApproveRejectStatus.Approve ? true : false;
                    litigationRequestAssign.ArbitrationMinutesOn = DateTime.Now;
                }

                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                if (!repositorySaveResponse.IsDone)
                    return repositorySaveResponse;

                var ApprovalCounts = litigationRequest.LitigationRequestArbitratorAssigningRequests.Where(s => s.ResponseArbitrationMinutes == true).Count();
                var estimationCount = Math.Floor((decimal)litigationRequest.ArbitratorsCount / 2);
                if (estimationCount <= ApprovalCounts)
                {
                    litigationRequest.LitigationRequestSubStage = LitigationRequestSubStage.JudgmentDecisionIssued;
                    litigationRequest.InternalStatusId = RequestStatus.JudgmentDecisionIssued;
                    litigationRequest.ExternalStatusId = RequestStatus.JudgmentDecisionIssued;

                    var LitigationReport = litigationRequest.LitigationRequestArbitrationReports.FirstOrDefault();
                    if (LitigationReport != null)
                    {
                        LitigationReport.IsPublished = true;
                        LitigationReport.PublishedDate = DateTime.Now;
                    }
                    // Send Notification
                    // تم إضافة حكم التحكيم
                    try
                    {
                        var arbitrationAdmin = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                        if (arbitrationAdmin.IsDone)
                        {
                            var portalUrl = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.REACPortalUrl).Result.Data?.Value;

                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationService_JudgmentAdded).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            msg = msg?.Replace("{PortalUrl}", portalUrl);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.ArbitrationService_JudgmentAdded).Result.Data?.Value);
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
                                emails.Add(litigationRequest.Contract.FirstPartyCitizen?.Email);
                                recipients.Add(litigationRequest.Contract.FirstPartyCitizen?.Mobile);


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
                                emails.Add(litigationRequest.Contract.SecondPartyCitizen?.Email);
                                recipients.Add(litigationRequest.Contract.SecondPartyCitizen?.Mobile);
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
                var SaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                if (!SaveResponse.IsDone)
                    return SaveResponse;

            }
            catch (Exception ex)
            {
                new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
            return await GetLitigationRequestForArbitratorRecusalRequest(requestId, nationalId);
        }

        public async Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequest(Guid requestId, string nationalId)
        {
            Guid? referenceId = _userManager.Users.FirstOrDefault(u => u.UserName == nationalId)?.ReferenceId;

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                     .Include(x => x.LitigationRequestArbitratorRecusalRequests)
                         .ThenInclude(x => x.ArbitratorCitizen)
                     .Include(x => x.LitigationRequestArbitratorRecusalRequests)
                         .ThenInclude(x => x.LitigationRequestArbitratorRecusalRequestFiles)
                         .ThenInclude(x => x.RecusalRequestFile)
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

            var requestArbitrationReportExplanationRequest = litigationRequest?.LitigationRequestArbitratorRecusalRequests
                            .Where(x => x.RequestParty == litigationRequestArbitrationReportExplanationRequestParty).FirstOrDefault();

            if (!isDone)
            {
                if (litigationRequest?.LitigationRequestStage != LitigationRequestStage.Arbitration)
                    allowAdd = false;

                if (requestArbitrationReportExplanationRequest is not null)
                    allowAdd = false;
            }
            var reportExplanationRequest = _mapper.Map<LitigationRequestArbitratorRecusalRequestGetDto>(requestArbitrationReportExplanationRequest);



            return new DataResponse<LitigationRequestForArbitratorRecusalRequestDto>()
            {
                Data = new LitigationRequestForArbitratorRecusalRequestDto()
                {
                    LitigationRequestShortDto = request,
                    AllowAdd = allowAdd,
                    request = reportExplanationRequest
                },
                IsDone = true,
            };

        }

        private async Task SendNotificationMSWithAllTypes(string mailMsg, string msg, LitigationRequest litigationRequest, string Subject, bool? isAdminArbitrator = null)
        {
            var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);

            //ApplicantCitizen
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
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

    }
}
