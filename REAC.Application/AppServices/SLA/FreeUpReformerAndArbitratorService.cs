using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.AppServices.Notification;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities;
using Core.Log.Application.Contracts.Dto;


namespace REAC.Application.AppServices.SLA
{
    public interface IFreeUpReformerAndArbitratorService
    {
        Task Execute();
    }

    public class FreeUpReformerAndArbitratorService : IFreeUpReformerAndArbitratorService
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ISysSettingService _sysSettingService;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ILoggerManager _loggerManager;
        public FreeUpReformerAndArbitratorService(IEntityRepository repository, IMapper mapper, ISysSettingService sysSettingService, ISysEmployeeService employeeService, INotificationService notificationService, ILoggerManager loggerManager)
        {
            _repository = repository;
            _mapper = mapper;
            _sysSettingService = sysSettingService;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _loggerManager = loggerManager;
        }




        public async Task Execute()
        {

            // Auto Activate Reformers

            var citizens = (await _repository.GetQueryable<Citizen, Guid>())
                      .Select(x => new
                      {
                          Citizen = x,
                          LatestDecision = x.ReformerStopFreeUpDecisions
                              .OrderByDescending(y => y.DecisionDate)
                              .FirstOrDefault()
                      })
                      .ToList();

            foreach(var citizen in citizens)
            {
                if(citizen.LatestDecision is null || (citizen.LatestDecision.ReformerStatusId == ReformerStatus.Certified & citizen.LatestDecision.DecisionTo is null) || citizen.LatestDecision.DecisionTo > DateTime.Now)
                    continue;

                citizen.Citizen.ReformerStatusId = ReformerStatus.Certified;

                ReformerStopFreeUpDecision ReformerStopFreeUpDecision = new ReformerStopFreeUpDecision();
                ReformerStopFreeUpDecision.CitizenId = citizen.Citizen.Id;
                ReformerStopFreeUpDecision.ReformerStatusId = ReformerStatus.Certified;
                ReformerStopFreeUpDecision.DecisionDate = DateTime.Now;
                ReformerStopFreeUpDecision.CreatedByRole = EntityFrameworkCore.Seed.Roles.SuperAdmin;
                await _repository.AddAsync<ReformerStopFreeUpDecision, int>(ReformerStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                        , new LogDto()
                                        {
                                            Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                            Message = saveResponse.Message
                                        });

                // Send Notification
                // تم إعادة تحرير المصلح

                try
                {
                    var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);

                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_Activate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.Citizen.FullName);
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body;

                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_Activate).Result.Data?.Value);
                        body = body?.Replace("{CitizenName}", citizen.Citizen.FullName);
                    }

                    var applicationNotifications = new List<SysNotificationDto>();

                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Reformer,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpReformer,
                        Message = msg
                    });

                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = deptAdminInfoReconciliation.Data.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpReformer,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Citizen.Email, deptAdminInfoReconciliation.Data.Email },
                            Subject = "Reformer Activated",
                            Body = body
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                                        , new LogDto()
                                        {
                                            Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                                            Message = ex.ToString()
                                        });
                }
            }
        }
    }
}
