using AutoMapper;
using Core.Contracts;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.System;
using Wafi.Application.Hubs.Contracts;
using Wafi.Application.Hubs;
using REAC.Domain.Shared.Enum;
using REAC.Application.Contracts.IAppServices.Notification;

namespace REAC.Application.AppServices
{
    public class CitizenBankAccountService : BasicService<CitizenBankAccount, Guid>, ICitizenBankAccountService
    {
        private readonly IMapper _mapper;
        private readonly IJwtReader _jwtReader;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;
        private readonly ISysEmployeeService _employeeService;



        public CitizenBankAccountService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, IMailService emailService, ISMSService smsService, ISysSettingService sysSetting, IHubContext<NotificationHub, INotificationClient> notificationHub, INotificationService notificationService, ISysSettingService sysSettingService, ISysEmployeeService employeeService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _mapper = mapper;
            _jwtReader = jwtReader;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
            _employeeService = employeeService;
        }
        public async Task<BaseResponse> GetCitizenBankAccount()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var citizenBankAccount = (await _repository.GetQueryable<CitizenBankAccount, Guid>())
                    .Include(x => x.SysBank)
                    .AsNoTracking().Where(x => x.CitizenId == referenceId)
                              .ToList();

            if (citizenBankAccount.ToList().Count == 0)
                return new DataResponse<CitizenBankAccountCreateUpdateDto>()
                {
                    IsDone = true,
                    Data = new CitizenBankAccountCreateUpdateDto()
                };

            var citizenBankAccountdto = _mapper.Map<CitizenBankAccountCreateUpdateDto>(citizenBankAccount.FirstOrDefault());
            return new DataResponse<CitizenBankAccountCreateUpdateDto>()
            {
                IsDone = true,
                Data = citizenBankAccountdto
            };
        }

        public async Task<BaseResponse> UpdateCitizenBankAccount(CitizenBankAccountCreateUpdateDto citizenBankAccountCreateUpdateDto)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                    .Where(c => c.Id == referenceId)
                    .FirstOrDefault();
            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);

            if (citizenBankAccountCreateUpdateDto.Id == Guid.Empty)
            {
                CitizenBankAccount citizenBankAccount = new CitizenBankAccount();
                citizenBankAccount.Id = Guid.NewGuid();
                citizenBankAccount.CitizenId = referenceId;
                citizenBankAccount.BankId = citizenBankAccountCreateUpdateDto.BankId;
                citizenBankAccount.IBan = citizenBankAccountCreateUpdateDto.IBan;
                citizenBankAccount.AccountOwnerName = citizenBankAccountCreateUpdateDto.AccountOwnerName;
                await _repository.AddAsync<CitizenBankAccount, Guid>(citizenBankAccount);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إضافة بيانات حساب البنك 
                try
                {
                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Citizen_AddBankAccount).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizenInfo.FullName);

                    var applicationNotifications = new List<SysNotificationDto>();

                    if (citizenInfo.IsReformer != null)
                    {
                        if ((bool)citizenInfo.IsReformer)
                        {
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Reformer,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                Message = msg
                            });

                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoReconciliation.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                Message = msg
                            });
                        }

                        var notificationResultReformer = _notificationService.SendNotifications(
                            new SendEmailDto()
                            {
                                To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                Subject = "Bank Account Added",
                                Body = msg
                            },
                            new SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                notification_type = "SMS",

                            },
                            applicationNotifications
                            );


                    }
                    if (citizenInfo.IsArbitrator != null)
                    {
                        if ((bool)citizenInfo.IsArbitrator)
                        {
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Arbitrator,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                Message = msg
                            });

                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoArbitration.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                Message = msg
                            });

                            var notificationResultReformer = _notificationService.SendNotifications(
                            new SendEmailDto()
                            {
                                To = new List<string>() { citizenInfo.Email, deptAdminInfoArbitration.Data.Email },
                                Subject = "Bank Account Added",
                                Body = msg
                            },
                            new SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                notification_type = "SMS",

                            },
                            applicationNotifications
                            );
                        }
                    }
                    
                }
                catch (Exception ex) { 
                
                
                }
                
                return await GetCitizenBankAccount();
            }
            else
            {
                var response = (await _repository.GetQueryable<CitizenBankAccount, Guid>()).AsNoTracking()
                    .FirstOrDefault(c => c.CitizenId == referenceId);

                if (response == null)
                {
                    CitizenBankAccount citizenBankAccount = new CitizenBankAccount();
                    citizenBankAccount.Id = Guid.NewGuid();
                    citizenBankAccount.CitizenId = referenceId;
                    citizenBankAccount.BankId = citizenBankAccountCreateUpdateDto.BankId;
                    citizenBankAccount.IBan = citizenBankAccountCreateUpdateDto.IBan;
                    citizenBankAccount.AccountOwnerName = citizenBankAccountCreateUpdateDto.AccountOwnerName;
                    await _repository.AddAsync<CitizenBankAccount, Guid>(citizenBankAccount);
                    var saveNewResponse = await _repository.SaveChangesAsync();
                    if (!saveNewResponse.IsDone)
                        return saveNewResponse;

                    return await GetCitizenBankAccount();
                }

                var newCitizenBankAccount = _mapper.Map<CitizenBankAccount>(citizenBankAccountCreateUpdateDto);
                newCitizenBankAccount.CitizenId = referenceId;
                var saveResponse = _repository.Update<CitizenBankAccount, Guid>(newCitizenBankAccount);
                if (saveResponse.IsDone != true)
                    return new DataResponse<CitizenBankAccount> { IsDone = true, Data = response };

                var saveExistResponse = await _repository.SaveChangesAsync();
                if (!saveExistResponse.IsDone)
                    return saveExistResponse;

                // Send Notification
                // تم تعديل بيانات حساب البنك 

                try
                {
                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Citizen_EditBankAccount).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizenInfo.FullName);

                    var applicationNotifications = new List<SysNotificationDto>();

                    if (citizenInfo.IsReformer != null)
                    {
                        if ((bool)citizenInfo.IsReformer)
                        {
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Reformer,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                Message = msg
                            });

                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoReconciliation.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                Message = msg
                            });
                        }

                        var notificationResultReformer = _notificationService.SendNotifications(
                            new SendEmailDto()
                            {
                                To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                Subject = "Bank Account Updated",
                                Body = msg
                            },
                            new SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                notification_type = "SMS",

                            },
                            applicationNotifications
                            );


                    }
                    if (citizenInfo.IsArbitrator != null)
                    {
                        if ((bool)citizenInfo.IsArbitrator)
                        {
                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = citizenInfo.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Arbitrator,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                Message = msg
                            });

                            applicationNotifications.Add(new SysNotificationDto()
                            {
                                ActorId = deptAdminInfoArbitration.Data.Id,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Employee,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = citizenInfo.Id,
                                RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                Message = msg
                            });

                            var notificationResultReformer = _notificationService.SendNotifications(
                            new SendEmailDto()
                            {
                                To = new List<string>() { citizenInfo.Email, deptAdminInfoArbitration.Data.Email },
                                Subject = "Bank Account Updated",
                                Body = msg
                            },
                            new SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = new List<string>() { citizenInfo.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                notification_type = "SMS",

                            },
                            applicationNotifications
                            );
                        }
                    }
                   
                }
                catch (Exception ex)
                {


                }

                return await GetCitizenBankAccount();
            }
        }
    }
}
