using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.Extensions.DependencyInjection;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IModuleServices;
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
using REAC.Application.Contracts.IAppServices.Litigation;
using Core.Security.Entities;
using Microsoft.AspNetCore.Identity;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationRequestAgreementRequestService : ILitigationRequestAgreementRequestService
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

        public LitigationRequestAgreementRequestService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysSettingService sysSettingService, IServiceProvider serviceProvider, ISysEmployeeService employeeService, INotificationService notificationService, UserManager<User> userManager)
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



        #region Agreement Request
        public async Task<BaseResponse> AddCitizenLitigationRequestForAgreementRequestReply(Guid requestId,string nationalId, ApproveRejectStatus agreementRequestStatus)
        {
            

            Guid? referenceId = _userManager.Users.FirstOrDefault(u => u.UserName == nationalId).ReferenceId;

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.Area)
                .Include(x => x.City)
                          .Include(x => x.WorkFlowProcessingLogs)
                          .ThenInclude(x => x.Employee)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Where(x => x.Id == requestId).ToList().FirstOrDefault();

            bool isDone = false;
            if (litigationRequest != null && referenceId != null)
            {
                if (litigationRequest.ApplicantCitizen != null)
                {
                    if (litigationRequest.ApplicantCitizenId == referenceId)
                    {
                        if (!litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                        {
                            litigationRequest.PlaintiffAgreementRequestResponse = agreementRequestStatus;
                            litigationRequest.PlaintiffAgreementRequestResponseOn = DateTime.Now;
                            isDone = true;

                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = agreementRequestStatus == ApproveRejectStatus.Approve ? "تم قبول المشارطة من قبل مقدم الطلب" : "تم رفض المشارطة من قبل مقدم الطلب"
                            });
                        }
                    }
                }

                if (!isDone)
                {
                    if (litigationRequest.Contract.FirstPartyCitizen != null)
                    {
                        if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                        {
                            if (!litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                            {
                                litigationRequest.PlaintiffAgreementRequestResponse = agreementRequestStatus;
                                litigationRequest.PlaintiffAgreementRequestResponseOn = DateTime.Now;
                                isDone = true;

                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = agreementRequestStatus == ApproveRejectStatus.Approve ? "تم قبول المشارطة من قبل المدعي" : "تم رفض المشارطة من قبل المدعي"
                                });
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
                            if (!litigationRequest.DefendantAgreementRequestResponse.HasValue)
                            {
                                litigationRequest.DefendantAgreementRequestResponse = agreementRequestStatus;
                                litigationRequest.DefendantAgreementRequestResponseOn = DateTime.Now;
                                isDone = true;

                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = agreementRequestStatus == ApproveRejectStatus.Approve ? "تم قبول المشارطة من قبل المدعي عليه" : "تم رفض المشارطة من قبل المدعي عليه"
                                });
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
                            if (!litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                            {
                                litigationRequest.PlaintiffAgreementRequestResponse = agreementRequestStatus;
                                litigationRequest.PlaintiffAgreementRequestResponseOn = DateTime.Now;
                                isDone = true;

                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = agreementRequestStatus == ApproveRejectStatus.Approve ? "تم قبول المشارطة من قبل المدعي" : "تم رفض المشارطة من قبل المدعي"
                                });
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
                            if (!litigationRequest.DefendantAgreementRequestResponse.HasValue)
                            {
                                litigationRequest.DefendantAgreementRequestResponse = agreementRequestStatus;
                                litigationRequest.DefendantAgreementRequestResponseOn = DateTime.Now;
                                isDone = true;

                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = agreementRequestStatus == ApproveRejectStatus.Approve ? "تم قبول المشارطة من قبل المدعي عليه" : "تم رفض المشارطة من قبل المدعي عليه"
                                });
                            }
                        }
                    }
                }
            }

            if (litigationRequest.PlaintiffAgreementRequestResponse.HasValue & litigationRequest.DefendantAgreementRequestResponse.HasValue)
            {
                if (litigationRequest.PlaintiffAgreementRequestResponse != ApproveRejectStatus.Approve | litigationRequest.DefendantAgreementRequestResponse != ApproveRejectStatus.Approve)
                {
                    litigationRequest.InternalStatusId = RequestStatus.AgreementRequestHasBeenRejected;
                    litigationRequest.ExternalStatusId = RequestStatus.AgreementRequestHasBeenRejected;
                    litigationRequest.IsFinished = true;
                    litigationRequest.IsActive = false;
                    //  Send Notification 
                    // تم رفض طلب المشارطة على الدعوى رقم..........
                    // المدعى - المدعى عليه - مدير ادارة التدقيق - موظف التدقيق
                    try
                    {
                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Reject_Agreement).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);

                        await SendNotificationMSWithDefendant(msg, litigationRequest, RequestType.LitigationAgreementRequest, "رفض طلب المشارطة");
                    }
                    catch { }
                }
                else
                {
                    var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                    await _bpfService.ExcuteWorkFlow(litigationRequest.Id, default);
                    litigationRequest.InternalStatusId = RequestStatus.New;
                    litigationRequest.ExternalStatusId = RequestStatus.New;

                    //  Send Notification 
                    // تم الموافقة على طلب المشارطة على الدعوى رقم........
                    // المدعى - المدعى عليه - مدير ادارة التدقيق - موظف التدقيق
                    try
                    {
                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Accept_Agreement).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);

                        await SendNotificationMSWithDefendant(msg, litigationRequest, RequestType.LitigationAgreementRequest, "قبول طلب المشارطة");
                    }
                    catch { }
                }

            }
            else
            {
                //var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                //await _bpfService.ExcuteWorkFlow(litigationRequest.Id, default);
                //litigationRequest.InternalStatusId = RequestStatus.New;
                //litigationRequest.ExternalStatusId = RequestStatus.New;
            }


            var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
            if (!repositorySaveResponse.IsDone)
                return repositorySaveResponse;

            return await GetCitizenLitigationRequestForAgreementRequestReply(requestId,nationalId);

        }

        private async Task<BaseResponse> GetCitizenLitigationRequestForAgreementRequestReply(Guid requestId, string nationalId)
        {

            
            Guid? referenceId = _userManager.Users.FirstOrDefault(u => u.UserName == nationalId).ReferenceId;


            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
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
                         .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                         .Include(x => x.Contract.SecondPartyFacility)
                         .Include(x => x.Contract.SecondPartyCitizen.Nationality)
                         .Include(x => x.Contract.LitigationRequestContractType)
                         .Include(x => x.LitigationRequestRequests)

                         .Where(x => x.Id == requestId).AsNoTracking().ToList().FirstOrDefault();
            if (litigationRequest == null)
            {
                return new BaseResponse { IsDone = false, Message = "No Litigation for this id " };
            }
            var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);


            LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();

            LitigationRequestForAgreementRequestDto litigationRequestForAgreementRequestDto = new LitigationRequestForAgreementRequestDto();
            litigationRequestForAgreementRequestDto.AllowAdd = true;


            bool IsDone = false;
            if (litigationRequest.IsFinished.HasValue)
            {
                if (litigationRequest.IsFinished.Value)
                {
                    litigationRequestForAgreementRequestDto.AllowAdd = false;
                    IsDone = true;
                }
            }
            litigationRequestForAgreementRequestDto.LitigationRequestShortDto = request;
            if (!IsDone)
            {
                if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAgreementRequestRequired)
                    litigationRequestForAgreementRequestDto.AllowAdd = false;

                if (litigationRequest.ApplicantCitizenId == referenceId)
                {
                    if (litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                    {
                        litigationRequestForAgreementRequestDto.AllowAdd = false;
                        litigationRequestForAgreementRequestDto.ApproveRejectStatus = litigationRequest.PlaintiffAgreementRequestResponse.Value;
                    }

                }
                if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                {
                    if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                    {
                        if (litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                        {
                            litigationRequestForAgreementRequestDto.AllowAdd = false;
                            litigationRequestForAgreementRequestDto.ApproveRejectStatus = litigationRequest.PlaintiffAgreementRequestResponse.Value;
                        }
                    }
                }
                if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                {
                    if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                    {
                        if (litigationRequest.DefendantAgreementRequestResponse.HasValue)
                        {
                            litigationRequestForAgreementRequestDto.AllowAdd = false;
                            litigationRequestForAgreementRequestDto.ApproveRejectStatus = litigationRequest.DefendantAgreementRequestResponse.Value;
                        }
                    }
                }
                if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                {
                    if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                    {
                        if (litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                        {
                            litigationRequestForAgreementRequestDto.AllowAdd = false;
                            litigationRequestForAgreementRequestDto.ApproveRejectStatus = litigationRequest.PlaintiffAgreementRequestResponse.Value;
                        }
                    }
                }
                if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                {
                    if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                    {
                        if (litigationRequest.DefendantAgreementRequestResponse.HasValue)
                        {
                            litigationRequestForAgreementRequestDto.AllowAdd = false;
                            litigationRequestForAgreementRequestDto.ApproveRejectStatus = litigationRequest.DefendantAgreementRequestResponse.Value;
                        }
                    }
                }

            }
            return new DataResponse<LitigationRequestForAgreementRequestDto>()
            {
                Data = litigationRequestForAgreementRequestDto,
                IsDone = true,
            };
        }

        #endregion

        #region Notifications
        private async Task SendNotificationMSWithDefendant(string msg, LitigationRequest litigationRequest, RequestType requestTypeId, string subject)
        {
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);

            //ApplicantCitizen
            var objEmail = new SendEmailDto()
            {
                Body = msg,
                Subject = subject,
                To = new List<string>() { litigationRequest.ApplicantCitizen?.Email }

            };
            var objSms = new SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>() { litigationRequest.ApplicantCitizen.Mobile },
                notification_type = "SMS",
            };
            var lstNotification = new List<SysNotificationDto>();
            lstNotification.Add(new SysNotificationDto()
            {
                ActorId = litigationRequest.ApplicantCitizen.Id,
                CreatedOn = DateTime.Now,
                ActorTypeId = NotificationActorType.Plaintiff,
                IsViewed = false,
                ViewedOn = null,
                RequestId = litigationRequest.Id,
                RequestTypeId = requestTypeId,
                Message = msg,
            });

            //مدير ادارة التدقيق
            if (deptAdminInfoReview is not null)
            {
                objEmail.To.Add(deptAdminInfoReview?.Data?.Email);
                objSms.recipients.Add(deptAdminInfoReview?.Data?.Mobile);

                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = deptAdminInfoReview.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Employee,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
            }

            //موظف التدقيق
            if (litigationRequest.WorkFlowProcessingLogs.Any())
            {
                var Employee = litigationRequest.WorkFlowProcessingLogs.Where(w => w.EmployeeId != null)
                    .OrderBy(x => x.CreatedOn).LastOrDefault().Employee;
                if (Employee is not null)
                {
                    objEmail.To.Add(Employee.Email);
                    objSms.recipients.Add(Employee.Mobile);

                    lstNotification.Add(new SysNotificationDto()
                    {
                        ActorId = Employee.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = litigationRequest.Id,
                        RequestTypeId = requestTypeId,
                        Message = msg,
                    });
                }

            }


            //المدعين
            if (litigationRequest?.Contract?.FirstPartyCitizen != null && litigationRequest?.Contract?.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyCitizen.Email);
                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyCitizen.Mobile);


            }
            if (litigationRequest?.Contract?.FirstPartyFacility != null && litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest?.Contract?.FirstPartyFacility.Email);
                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest?.Contract?.FirstPartyFacility?.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Plaintiff,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.FirstPartyFacility?.Citizen?.Mobile);


            }

            //المدعى عليه
            if (litigationRequest?.Contract?.SecondPartyCitizen != null)
            {
                objEmail.To.Add(litigationRequest?.Contract?.SecondPartyCitizen.Email);
                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Defendant,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.SecondPartyCitizen.Mobile);

            }


            if (litigationRequest?.Contract?.SecondPartyFacility != null)
            {
                objEmail.To.Add(litigationRequest?.Contract?.SecondPartyFacility.Email);
                lstNotification.Add(new SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility?.Citizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = NotificationActorType.Defendant,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestTypeId,
                    Message = msg,
                });
                objSms.recipients.Add(litigationRequest?.Contract?.SecondPartyFacility?.Citizen?.Mobile);

            }

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);

        }
        #endregion
    }
}
