using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;

namespace REAC.Application.AppServices
{

    public class CitizenService : BasicService<Citizen, Guid>, ICitizenService
    {
        private readonly IMapper _mapper;
        private readonly IJwtReader _jwtReader;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;

        public CitizenService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysSettingService sysSetting, ISysEmployeeService employeeService, INotificationService notificationService, ISysSettingService sysSettingService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _mapper = mapper;
            _jwtReader = jwtReader;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
        }
        public async Task<DateTime?> GetIdentityExpirationDate(Guid? userId)
        {
            var quarable = await GetQueryableAsync();
            var identityExpirationDate = quarable.FirstOrDefault(x => x.Id == userId)?.IdentityExpiryDate;
            return identityExpirationDate;
        }
        public async Task<BaseResponse> GetDelegatedPersonsLawyer()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var delegatorPersons = (await _repository.GetQueryable<Delegator, Guid>())
                            .Include(x => x.Citizen)
                            .Include(x => x.Facility)
                         .Where(x => x.DelegatorCitizenId == referenceId
                         & x.DelegationRelationshipTypeId == DelegationRelationshipType.Lawyer
                         & x.CitizenId.HasValue == true).ToList();

            List<DelegatorItem> persons = new List<DelegatorItem>();


            foreach (var delegatorPerson in delegatorPersons)
            {
                if (delegatorPerson.Facility == null)
                {
                    if (delegatorPerson.CitizenId.HasValue)
                        persons.Add(new DelegatorItem() { Id = delegatorPerson.CitizenId.Value, Name = delegatorPerson.Citizen.FullName });
                }
            }

            return new DataResponse<List<DelegatorItem>>()
            {
                IsDone = true,
                Data = persons
            };
        }
        public async Task<BaseResponse> GetDelegatedPersonsAgent()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var delegatorPersons = (await _repository.GetQueryable<Delegator, Guid>())
                            .Include(x => x.Citizen)
                            .Include(x => x.Facility)
                         .Where(x => x.DelegatorCitizenId == referenceId
                         & x.CitizenId.HasValue == true).ToList();

            List<DelegatorItem> persons = new List<DelegatorItem>();

            foreach (var delegatorPerson in delegatorPersons)
            {
                if (delegatorPerson.Facility == null)
                {
                    if (delegatorPerson.CitizenId.HasValue)
                        persons.Add(new DelegatorItem() { Id = delegatorPerson.CitizenId.Value, Name = delegatorPerson.Citizen.FullName });
                }
            }

            return new DataResponse<List<DelegatorItem>>()
            {
                IsDone = true,
                Data = persons
            };
        }
        public async Task<BaseResponse> GetDelegatedFacilitiesAgent()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var delegatorPersons = (await _repository.GetQueryable<Delegator, Guid>())
                            .Include(x => x.Citizen)
                            .Include(x => x.Facility)
                         .Where(x => x.DelegatorCitizenId == referenceId
                         & x.DelegationRelationshipTypeId == DelegationRelationshipType.Agent
                         & x.FacilityId.HasValue == true).ToList();


            List<DelegatorItem> facilities = new List<DelegatorItem>();

            foreach (var delegatorPerson in delegatorPersons)
            {
                if (delegatorPerson.Facility != null)
                {
                    facilities.Add(new DelegatorItem() { Id = delegatorPerson.FacilityId.Value, Name = delegatorPerson.Facility.ArabicFacilityName });
                }
            }
            return new DataResponse<List<DelegatorItem>>()
            {
                IsDone = true,
                Data = facilities,
                Message = "agent"
            };
        }

        public async Task<BaseResponse> GetDelegatedFacilitiesLawyer()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var delegatorPersons = (await _repository.GetQueryable<Delegator, Guid>())
                            .Include(x => x.Citizen)
                            .Include(x => x.Facility)
                         .Where(x => x.DelegatorCitizenId == referenceId
                         & x.DelegationRelationshipTypeId == DelegationRelationshipType.Lawyer
                        & x.FacilityId.HasValue == true).ToList();


            List<DelegatorItem> facilities = new List<DelegatorItem>();

            foreach (var delegatorPerson in delegatorPersons)
            {
                if (delegatorPerson.Facility != null)
                {
                    facilities.Add(new DelegatorItem() { Id = delegatorPerson.FacilityId.Value, Name = delegatorPerson.Facility.ArabicFacilityName });
                }
            }
            return new DataResponse<List<DelegatorItem>>()
            {
                IsDone = true,
                Data = facilities,
                Message = "Lawyer"
            };
        }

        public async Task<BaseResponse> GetOwnerFacilities()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var faclities = (await _repository.GetQueryable<Facility, Guid>())
                        .Where(x => x.CitizenId == referenceId).ToList();

            List<DelegatorItem> facilities = new List<DelegatorItem>();

            foreach (var faclity in faclities)
            {
                facilities.Add(new DelegatorItem() { Id = faclity.Id, Name = faclity.ArabicFacilityName });
            }

            return new DataResponse<List<DelegatorItem>>()
            {
                IsDone = true,
                Data = facilities,
                Message = "owner"
            };
        }
        public async Task<ICollection<Facility>> GetRepresentativeCitizens(Guid? userId) // بجيب هنا كل المدعي عليهم اللي انا ممثل عنهم
        {
            var quarable = await GetQueryableAsync();
            var result = quarable.Where(x => x.LitigationCompanyRepresentativeId == userId).SelectMany(x => x.Facilities).ToList();
            return result;
        }
        public async Task<(Guid, bool)> CheckCitizenExist(string identityNumber)
        {
            var quarable = await GetQueryableAsync();
            Guid citizenId = new Guid("00000000-0000-0000-0000-000000000000");
            bool citizenExist = true;
            var exist = quarable.Where(x => x.IdentityNumber == identityNumber).FirstOrDefault();
            if (exist != null)
            {
                citizenId = exist.Id;
            }
            else
                citizenExist = false;

            return (citizenId, citizenExist);
        }
        public async Task<BaseResponse> GetCitizenBasicData()
        {

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);


            var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                    .Include(x => x.Gender).Include(x => x.City)
                    .Include(x => x.Area)
                    .Include(x => x.IdentityFile)
                    .Include(x => x.SigntureFile)
                    .Include(x => x.LawLicenseFile)
                    .Include(x => x.Nationality)
                    .AsNoTracking().Where(x => x.Id == referenceId)
                              .ToList();


            if (citizen.ToList().Count == 0) return new BaseResponse() { IsDone = false, Message = "Not Found" };


            var citizendto = _mapper.Map<GetCitizenBasicDataProfileDto>(citizen.FirstOrDefault());
            return new DataResponse<GetCitizenBasicDataProfileDto>()
            {
                IsDone = true,
                Data = citizendto
            };

        }
        public async Task<BaseResponse> UpdateCitizenBasicData(SaveCitizenBasicDataProfileDto citizenBasicDataProfileDto)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var response = await _repository.GetByIdAsync<Citizen, Guid>(referenceId, default);

            if (response.IsDone != true)
                return response;

            var citizen = ((DataResponse<Citizen>)response).Data;
            citizen.NationalAddress = citizenBasicDataProfileDto.NationalAddress;
            citizen.ShortName = citizenBasicDataProfileDto.ShortName;
            citizen.Email = citizenBasicDataProfileDto.Email;

            citizen.Employer = citizenBasicDataProfileDto.Employer;
            citizen.Mobile = citizenBasicDataProfileDto.Mobile;
            citizen.Fax = citizenBasicDataProfileDto.Fax;
            citizen.Email = citizenBasicDataProfileDto.Email;
            citizen.AreaId = citizenBasicDataProfileDto.AreaId;
            citizen.CityId = citizenBasicDataProfileDto.CityId;
            citizen.District = citizenBasicDataProfileDto.District;
            citizen.Street = citizenBasicDataProfileDto.Street;
            citizen.PostalCode = citizenBasicDataProfileDto.PostalCode;
            citizen.SubNumber = citizenBasicDataProfileDto.SubNumber;
            citizen.BuildingNumber = citizenBasicDataProfileDto.BuildingNumber;
            citizen.UnitNumber = citizenBasicDataProfileDto.UnitNumber;
            citizen.IsLawyer = citizenBasicDataProfileDto.IsLawyer;
            citizen.LawyerLicenseNumber = citizenBasicDataProfileDto.LawyerLicenseNumber;
            citizen.LawyerLicenseExpirationDate = citizenBasicDataProfileDto.LawyerLicenseExpirationDate;
            citizen.LawLicenseFileId = citizenBasicDataProfileDto.LawLicenseFileId;

            citizen.IdentityFileId = citizenBasicDataProfileDto.IdentityFileId;
            citizen.SigntureFileId = citizenBasicDataProfileDto.SigntureFileId;
            citizen.HomeTelephone = citizenBasicDataProfileDto.HomeTelephone;
            citizen.WorkTelephone = citizenBasicDataProfileDto.WorkTelephone;

            var saveResponse = _repository.SaveChangesAsync().Result;
            if (saveResponse.IsDone != true)
                return response;

            var updatedResponse = await _repository.GetByIdAsync<Citizen, Guid>(referenceId, default);
            if (updatedResponse.IsDone != true)
                return response;

            return await GetCitizenBasicData();
        }
        public async Task<BaseResponse> GetReformersLookup(Guid? citizenId, Guid requestId)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
              .Include(x => x.Contract)
              .Where(x => x.Id == requestId).SingleOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            var citizens = (await _repository.GetQueryable<Citizen, Guid>())
                             .Include(x => x.ReformerLitigationRequests)
                             .Where(x => x.IsReformer == true & x.ReformerStatusId == ReformerStatus.Certified & x.Id != citizenId).ToList();
            List<CitizenReformer> citizenReformers = new List<CitizenReformer>();
            citizens.ForEach(t =>
            {
                if (t.Id != litigationRequest.ApplicantCitizenId & t.Id != litigationRequest.Contract.FirstPartyCitizenId & t.Id != litigationRequest.Contract.SecondPartyCitizenId)
                {
                    citizenReformers.Add(new CitizenReformer()
                    {
                        FullName = t.FullName,
                        Id = t.Id,
                        RequestsCount = t.ReformerLitigationRequests.Where(x => x.IsFinished == false).Count()
                    });
                }
            });
            return new DataResponse<List<CitizenReformer>>()
            {
                IsDone = true,
                Data = citizenReformers.OrderBy(x => x.RequestsCount).ToList()
            };
        }
        public async Task<BaseResponse> GetArbitratorsLookup(Guid? citizenId, Guid requestId)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.Contract)
                .Where(x => x.Id == requestId).SingleOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            var citizens = (await _repository.GetQueryable<Citizen, Guid>())
                         .Include(x => x.ArbitratorLitigationRequests)
                         .Include(x => x.LitigationRequestArbitratorLists)
                         .Where(x => x.IsArbitrator == true & x.ArbitratorStatusId == ArbitratorStatus.Certified & x.Id != citizenId).ToList();

            List<CitizenArbirator> citizenArbirators = new List<CitizenArbirator>();
            citizens.ForEach(t =>
            {
                if (t.Id != litigationRequest.ApplicantCitizenId & t.Id != litigationRequest.Contract.FirstPartyCitizenId & t.Id != litigationRequest.Contract.SecondPartyCitizenId)
                {
                    citizenArbirators.Add(new CitizenArbirator()
                    {
                        FullName = t.FullName,
                        Id = t.Id,
                        RequestsCount = t.ArbitratorLitigationRequests.Where(x => x.IsFinished == false).Count() + t.LitigationRequestArbitratorLists.Count()
                    });
                }
            });
            return new DataResponse<List<CitizenArbirator>>()
            {
                IsDone = true,
                Data = citizenArbirators.OrderBy(x => x.RequestsCount).ToList()
            };

        }
        public async Task<BaseResponse> GetArbitratorsFullLookup(Guid? citizenId, Guid requestId)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.LitigationRequestArbitratorLists)
                .Include(x => x.Contract)
                .Where(x => x.Id == requestId).SingleOrDefault();

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "طلب غير موجود" };

            var citizens = (await _repository.GetQueryable<Citizen, Guid>())
                         .Include(x => x.ArbitratorLitigationRequests)
                         .Include(x => x.LitigationRequestArbitratorLists)
                         .Include(x => x.Area)
                         .Include(x => x.CitizenRequests)
                         .Where(x => x.IsArbitrator == true & x.ArbitratorStatusId == ArbitratorStatus.Certified & x.Id != citizenId).ToList();
            List<CitizenFullLookupArbirator> citizenArbirators = new List<CitizenFullLookupArbirator>();
            citizens.ForEach(t =>
            {
                if (t.Id != litigationRequest.ApplicantCitizenId & t.Id != litigationRequest.Contract.FirstPartyCitizenId & t.Id != litigationRequest.Contract.SecondPartyCitizenId)
                {
                    if (t.Id != litigationRequest.ArbitratorCitizenId)
                    {
                        if (litigationRequest.LitigationRequestArbitratorLists?.Count(x => x.ArbitratorCitizenId == t.Id) == 0)
                        {
                            citizenArbirators.Add(new CitizenFullLookupArbirator()
                            {
                                ArbitratorClass = t.CitizenRequests?.FirstOrDefault()?.SysArbitratorClass?.ArbitratorClassName,
                                AreaName = t.CitizenRequests?.FirstOrDefault()?.SysArea?.AreaName,
                                Brief = t.CitizenRequests?.FirstOrDefault()?.Brief,
                                FullName = t.FullName,
                                Id = t.Id,
                                RequestsCount = t.ArbitratorLitigationRequests.Where(x => x.IsFinished == false).Count() + t.LitigationRequestArbitratorLists.Count()
                            });
                        }
                    }
                }
            });
            return new DataResponse<List<CitizenFullLookupArbirator>>()
            {
                IsDone = true,
                Data = citizenArbirators.OrderBy(x => x.RequestsCount).ToList()
            };

        }


        #region Backoffice

        public async Task<BaseResponse> AddArbitratorFreeUpDecision(ArbitratorFreeUpDecisionCreateDto arbitratorFreeUpDecisionCreateDto)
        {
            try
            {

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                     .Where(x => x.Id == arbitratorFreeUpDecisionCreateDto.CitizenId)
                     .SingleOrDefault();
                var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();

                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                citizen.ArbitratorStatusId = ArbitratorStatus.Certified;

                ArbitratorStopFreeUpDecision arbitratorStopFreeUpDecision = new ArbitratorStopFreeUpDecision();
                arbitratorStopFreeUpDecision.CitizenId = arbitratorFreeUpDecisionCreateDto.CitizenId;
                arbitratorStopFreeUpDecision.ArbitratorStatusId = ArbitratorStatus.Certified;
                arbitratorStopFreeUpDecision.DecisionDate = arbitratorFreeUpDecisionCreateDto.DecisionDate;
                await _repository.AddAsync<ArbitratorStopFreeUpDecision, int>(arbitratorStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إعادة تحرير المحكم

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_Activate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Arbitrator,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = deptAdminInfoArbitration.Data.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoArbitration.Data.Email },
                            Subject = "Arbitrator Activated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }

                return await GetArbitratorsDecisions(arbitratorFreeUpDecisionCreateDto.CitizenId);
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.ToString(),
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponse> AddArbitratorStopDecision(ArbitratorStopDecisionCreateDto arbitratorStopDecisionCreateDto)
        {
            try
            {
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                      .Where(x => x.Id == arbitratorStopDecisionCreateDto.CitizenId)
                      .SingleOrDefault();
                var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();
                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                citizen.ArbitratorStatusId = ArbitratorStatus.Stopped;


                ArbitratorStopFreeUpDecision arbitratorStopFreeUpDecision = new ArbitratorStopFreeUpDecision();
                arbitratorStopFreeUpDecision.CitizenId = citizen.Id;
                arbitratorStopFreeUpDecision.ArbitratorStatusId = ArbitratorStatus.Stopped;
                arbitratorStopFreeUpDecision.DecisionDate = arbitratorStopDecisionCreateDto.DecisionDate;
                arbitratorStopFreeUpDecision.LateLitigationRequestNumber = arbitratorStopDecisionCreateDto.LateLitigationRequestNumber;
                arbitratorStopFreeUpDecision.StoppingReason = arbitratorStopDecisionCreateDto.StoppingReason;
                await _repository.AddAsync<ArbitratorStopFreeUpDecision, int>(arbitratorStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إقاف تحرير المحكم

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_DeActivate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Arbitrator,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = deptAdminInfoArbitration.Data.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoArbitration.Data.Email },
                            Subject = "Arbitrator DeActivated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }


                return await GetArbitratorsDecisions(arbitratorStopDecisionCreateDto.CitizenId);
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.ToString(),
                    Message = ex.Message
                };
            }


        }
        public async Task<BaseResponse> AddReformerFreeUpDecision(ReformerFreeUpDecisionCreateDto reformerFreeUpDecisionCreateDto)
        {
            try
            {
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                    .Where(x => x.Id == reformerFreeUpDecisionCreateDto.CitizenId)
                    .SingleOrDefault();
                var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();
                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                citizen.ReformerStatusId = ReformerStatus.Certified;

                ReformerStopFreeUpDecision ReformerStopFreeUpDecision = new ReformerStopFreeUpDecision();
                ReformerStopFreeUpDecision.CitizenId = reformerFreeUpDecisionCreateDto.CitizenId;
                ReformerStopFreeUpDecision.ReformerStatusId = ReformerStatus.Certified;
                ReformerStopFreeUpDecision.DecisionDate = reformerFreeUpDecisionCreateDto.DecisionDate;
                await _repository.AddAsync<ReformerStopFreeUpDecision, int>(ReformerStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إعادة تحرير المصلح

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_Activate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Reformer,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
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
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpReformer,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoReconciliation.Data.Email },
                            Subject = "Reformer Activated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }

                return await GetReformersDecisions(reformerFreeUpDecisionCreateDto.CitizenId);
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.ToString(),
                    Message = ex.Message
                };
            }
        }
        public async Task<BaseResponse> AddReformerStopDecision(ReformerStopDecisionCreateDto reformerStopDecisionCreateDto)
        {
            try
            {
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                    .Where(x => x.Id == reformerStopDecisionCreateDto.CitizenId)
                    .SingleOrDefault();

                if (!string.IsNullOrEmpty(reformerStopDecisionCreateDto.LateLitigationRequestNumber))
                {
                    var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Any(l => l.RequestCode == reformerStopDecisionCreateDto.LateLitigationRequestNumber);

                    if (!litigationRequest)
                        return new BaseResponse() { IsDone = false, Message = "لا يوجد دعوي بهذا الرقم" };

                }


                var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();
                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                citizen.ReformerStatusId = ReformerStatus.Stopped;

                ReformerStopFreeUpDecision ReformerStopFreeUpDecision = new ReformerStopFreeUpDecision();
                ReformerStopFreeUpDecision.CitizenId = reformerStopDecisionCreateDto.CitizenId;
                ReformerStopFreeUpDecision.ReformerStatusId = ReformerStatus.Stopped;
                ReformerStopFreeUpDecision.DecisionDate = reformerStopDecisionCreateDto.DecisionDate;
                ReformerStopFreeUpDecision.LateLitigationRequestNumber = reformerStopDecisionCreateDto.LateLitigationRequestNumber;
                ReformerStopFreeUpDecision.StoppingReason = reformerStopDecisionCreateDto.StoppingReason;
                await _repository.AddAsync<ReformerStopFreeUpDecision, int>(ReformerStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إيقاف المصلح

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_DeActivate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Reformer,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
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
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoReconciliation.Data.Email },
                            Subject = "Reformer DeActivated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }

                return await GetReformersDecisions(reformerStopDecisionCreateDto.CitizenId);
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.ToString(),
                    Message = ex.Message
                };
            }

        }


        #endregion

        public async Task<BaseResponse> ActivateArbitratorAccount(ArbitratorFreeUpDecisionCreateDto arbitratorFreeUpDecisionCreateDto)
        {
            try
            {

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                     .Where(x => x.Id == arbitratorFreeUpDecisionCreateDto.CitizenId)
                     .SingleOrDefault();
                var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();

                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                citizen.ArbitratorStatusId = ArbitratorStatus.Certified;

                ArbitratorStopFreeUpDecision arbitratorStopFreeUpDecision = new ArbitratorStopFreeUpDecision();
                arbitratorStopFreeUpDecision.CitizenId = arbitratorFreeUpDecisionCreateDto.CitizenId;
                arbitratorStopFreeUpDecision.ArbitratorStatusId = ArbitratorStatus.Certified;
                arbitratorStopFreeUpDecision.DecisionDate = arbitratorFreeUpDecisionCreateDto.DecisionDate;
                await _repository.AddAsync<ArbitratorStopFreeUpDecision, int>(arbitratorStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إعادة تحرير المحكم

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_Activate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Arbitrator,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = deptAdminInfoArbitration.Data.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoArbitration.Data.Email },
                            Subject = "Arbitrator Activated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }

                return new DataResponse<CitizenArbitratorDetailShortDto>() { IsDone = true, Data = new CitizenArbitratorDetailShortDto() { ArbitratorStatusId = ArbitratorStatus.Certified }, Message = " تم تفعيل الحساب كمحكم" };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = true, Message = "خطأ داخلي" };

            }
        }

        public async Task<BaseResponse> DeActivateArbitratorAccount(ArbitratorStopDecisionCreateDto arbitratorStopDecisionCreateDto)
        {
            try
            {
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                      .Where(x => x.Id == arbitratorStopDecisionCreateDto.CitizenId)
                      .SingleOrDefault();
                var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();
                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "محكم غير موجود"
                    };

                var litigationRequestArbitratorAssigningRequest = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                    .Include(x => x.LitigationRequest)
                .Any(l => l.CitizenId == arbitratorStopDecisionCreateDto.CitizenId && l.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve 
                && l.LitigationRequest.LitigationRequestStage == LitigationRequestStage.Arbitration && l.LitigationRequest.LitigationRequestSubStage != LitigationRequestSubStage.JudgmentDecisionIssued);

                if (litigationRequestArbitratorAssigningRequest)
                    return new BaseResponse() { IsDone = false, Message = "لا يمكن قبول الطلب لوجود دعوي لم تنته بعد من فترة التحكيم" };

                citizen.ArbitratorStatusId = ArbitratorStatus.Stopped;


                ArbitratorStopFreeUpDecision arbitratorStopFreeUpDecision = new ArbitratorStopFreeUpDecision();
                arbitratorStopFreeUpDecision.CitizenId = citizen.Id;
                arbitratorStopFreeUpDecision.ArbitratorStatusId = ArbitratorStatus.Stopped;
                arbitratorStopFreeUpDecision.DecisionDate = arbitratorStopDecisionCreateDto.DecisionDate;
                arbitratorStopFreeUpDecision.DecisionFrom = arbitratorStopDecisionCreateDto.DecisionFrom;
                arbitratorStopFreeUpDecision.DecisionTo = arbitratorStopDecisionCreateDto.DecisionTo;
                arbitratorStopFreeUpDecision.LateLitigationRequestNumber = arbitratorStopDecisionCreateDto.LateLitigationRequestNumber;
                arbitratorStopFreeUpDecision.StoppingReason = arbitratorStopDecisionCreateDto.StoppingReason;
                await _repository.AddAsync<ArbitratorStopFreeUpDecision, int>(arbitratorStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إقاف تحرير المحكم

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_DeActivate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Arbitrator,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = deptAdminInfoArbitration.Data.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Employee,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoArbitration.Data.Email },
                            Subject = "Arbitrator DeActivated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }


                return new DataResponse<CitizenArbitratorDetailShortDto>() { IsDone = true, Data = new CitizenArbitratorDetailShortDto() { ArbitratorStatusId = ArbitratorStatus.Stopped }, Message = " تم الغاء تفعيل الحساب كمحكم" };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }


        }


        public async Task<BaseResponse> DeActivateReformerAccount(ReformerStopDecisionCreateDto reformerStopDecisionCreateDto)
        {
            try
            {
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                    .Where(x => x.Id == reformerStopDecisionCreateDto.CitizenId)
                    .SingleOrDefault();

                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "مصلح غير موجود"
                    };

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Any(l => (l.ExternalStatusId != RequestStatus.Cancelled | l.InternalStatusId != RequestStatus.Cancelled) 
                & l.LitigationRequestStage == LitigationRequestStage.Reconciliation & l.ReformerCitizenId == reformerStopDecisionCreateDto.CitizenId);

                if (litigationRequest)
                    return new BaseResponse() { IsDone = false, Message = "لا يمكن قبول الطلب لوجود دعوي لم تنته بعد من فترة الصُلح" };

                var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();
                

                citizen.ReformerStatusId = ReformerStatus.Stopped;

                ReformerStopFreeUpDecision ReformerStopFreeUpDecision = new ReformerStopFreeUpDecision();
                ReformerStopFreeUpDecision.CitizenId = reformerStopDecisionCreateDto.CitizenId;
                ReformerStopFreeUpDecision.ReformerStatusId = ReformerStatus.Stopped;
                ReformerStopFreeUpDecision.DecisionDate = DateTime.Now;
                ReformerStopFreeUpDecision.LateLitigationRequestNumber = reformerStopDecisionCreateDto.LateLitigationRequestNumber;
                ReformerStopFreeUpDecision.StoppingReason = reformerStopDecisionCreateDto.StoppingReason;
                ReformerStopFreeUpDecision.DecisionFrom = reformerStopDecisionCreateDto.DecisionFrom;
                ReformerStopFreeUpDecision.DecisionTo = reformerStopDecisionCreateDto.DecisionTo;
                await _repository.AddAsync<ReformerStopFreeUpDecision, int>(ReformerStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إيقاف المصلح

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_DeActivate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Reformer,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
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
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpArbitrator,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoReconciliation.Data.Email },
                            Subject = "Reformer DeActivated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }

                return new DataResponse<CitizenReformerDetailShortDto>() { IsDone = true , Data = new CitizenReformerDetailShortDto() { ReformerStatusId = ReformerStatus.Stopped}, Message = " تم إلغاء تفعيل الحساب كمصلح" };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }

        }

        public async Task<BaseResponse> ActivateReformerAccount(ReformerFreeUpDecisionCreateDto reformerFreeUpDecisionCreateDto)
        {
            try
            {
                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                    .Where(x => x.Id == reformerFreeUpDecisionCreateDto.CitizenId)
                    .SingleOrDefault();
                var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
                var msg = "";
                var applicationNotifications = new List<SysNotificationDto>();
                if (citizen == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "مصلح غير موجود"
                    };

                citizen.ReformerStatusId = ReformerStatus.Certified;

                ReformerStopFreeUpDecision ReformerStopFreeUpDecision = new ReformerStopFreeUpDecision();
                ReformerStopFreeUpDecision.CitizenId = reformerFreeUpDecisionCreateDto.CitizenId;
                ReformerStopFreeUpDecision.ReformerStatusId = ReformerStatus.Certified;
                ReformerStopFreeUpDecision.DecisionDate = reformerFreeUpDecisionCreateDto.DecisionDate;
                await _repository.AddAsync<ReformerStopFreeUpDecision, int>(ReformerStopFreeUpDecision);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notification
                // تم إعادة تحرير المصلح

                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_Activate).Result.Data?.Value;
                    msg = msg.Replace("{CitizenName}", citizen.FullName);
                    applicationNotifications.Add(new SysNotificationDto()
                    {
                        ActorId = citizen.Id,
                        CreatedOn = DateTime.Now,
                        ActorTypeId = NotificationActorType.Reformer,
                        IsViewed = false,
                        ViewedOn = null,
                        RequestId = citizen.Id,
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
                        RequestId = citizen.Id,
                        RequestTypeId = RequestType.StopFreeUpReformer,
                        Message = msg
                    });

                    var notificationResultUpdatedArbitrator = await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { citizen.Email, deptAdminInfoReconciliation.Data.Email },
                            Subject = "Reformer Activated",
                            Body = msg
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                            notification_type = "SMS",

                        },
                        applicationNotifications
                        );
                }
                catch (Exception ex)
                {

                }

                return new DataResponse<CitizenReformerDetailShortDto>() { IsDone = true, Data = new CitizenReformerDetailShortDto() { ReformerStatusId = ReformerStatus.Certified }, Message = "تم تفعيل الحساب كمصلح" };


            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = true, Message = "خطأ داخلي" };

            }
        }


        public async Task<BaseResponse> GetReformersDecisions(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            PagedResult<Citizen> citizens = null;

            var entitiesQuerable = (await _repository.GetQueryable<Citizen, Guid>())
                                     .Include(x => x.ReformerLitigationRequests)
                         .Include(x => x.ReformerStopFreeUpDecisions)
                         .Include(x => x.SysReformerStatus);


            citizens = entitiesQuerable.Where(x => x.IsReformer == true)
                                       .OrderByDescending(x => x.FullName)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);

            List<CitizenReformerShortDto> citizenReformers = new List<CitizenReformerShortDto>();

            foreach (var citizen in citizens.Results)
            {
                CitizenReformerShortDto citizenReformer = new CitizenReformerShortDto();
                citizenReformer.Id = citizen.Id;
                citizenReformer.FullName = citizen.FullName;
                citizenReformer.Mobile = citizen.Mobile;
                citizenReformer.IdentityNumber = citizen.IdentityNumber;
                citizenReformer.ReformerStatus = citizen.SysReformerStatus?.StatusName;
                citizenReformer.Email = citizen.Email;
                citizenReformer.RequestsCount = citizen.ReformerLitigationRequests.Count(x => x.IsFinished == false);
                citizenReformer.ReformerStatusDate = citizen.ReformerStopFreeUpDecisions?.OrderBy(x => x.DecisionDate).LastOrDefault()?.DecisionDate;
                citizenReformers.Add(citizenReformer);

            }

            return new DataResponse<PagedResult<CitizenReformerShortDto>>
            {
                Data =
                   new PagedResult<CitizenReformerShortDto>
                   {
                       Results = citizenReformers,
                       CurrentPage = citizens.CurrentPage,
                       PageCount = citizens.PageCount,
                       PageSize = citizens.PageSize,
                       TotalCount = citizens.TotalCount
                   },
                IsDone = true
            };

        }
        public async Task<BaseResponse> GetReformersDecisions(Guid citizenId)
        {
            var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                                         .Include(x => x.SysReformerStatus)
                         .Include(x => x.ReformerLitigationRequests)
                         .Include(x => x.ReformerStopFreeUpDecisions)
                         .ThenInclude(x => x.SysReformerStatus)
                         .Include(x => x.CitizenRequests)
                         .Where(x => x.Id == citizenId).SingleOrDefault();

            if (citizen == null)
                return new BaseResponse() { IsDone = false, Message = "حساب غير موجود" };

            CitizenReformerDetailShortDto citizenReformer = new CitizenReformerDetailShortDto();
            citizenReformer.Id = citizen.Id;
            citizenReformer.FullName = citizen.FullName;
            citizenReformer.Mobile = citizen.Mobile;
            citizenReformer.AcademicDegreeName = citizen.CitizenRequests?
                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Reformer)?
                .OrderBy(x => x.RequestOn).LastOrDefault()?.AcademicDegreeName;
            citizenReformer.IsVolunteerApproval = citizen.CitizenRequests?
               .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Reformer)?
               .OrderBy(x => x.RequestOn).LastOrDefault()?.IsVolunteerApproval;
            citizenReformer.Fax = citizen.Fax;
            citizenReformer.HomeTelephone = citizen.HomeTelephone;
            citizenReformer.WorkTelephone = citizen.WorkTelephone;
            citizenReformer.IdentityNumber = citizen.IdentityNumber;
            citizenReformer.ReformerStatus = citizen.SysReformerStatus?.StatusName;
            citizenReformer.ReformerStatusId = citizen.ReformerStatusId;
            citizenReformer.Email = citizen.Email;
            citizenReformer.RequestsCount = citizen.ReformerLitigationRequests.Count(x => x.IsFinished == false);
            citizenReformer.ReformerStatusDate = citizen.ReformerStopFreeUpDecisions?.OrderBy(x => x.DecisionDate).LastOrDefault()?.DecisionDate;
            if (citizen.ReformerStopFreeUpDecisions != null)
            {
                foreach (var decision in citizen.ReformerStopFreeUpDecisions)
                {
                    ReformerStopFreeUpDetailDecisionDto reformerStopFreeUpDetailDecisionDto = new ReformerStopFreeUpDetailDecisionDto();
                    reformerStopFreeUpDetailDecisionDto.DecisionDate = decision.DecisionDate;
                    reformerStopFreeUpDetailDecisionDto.ReformerStatus = decision.SysReformerStatus?.StatusName;
                    reformerStopFreeUpDetailDecisionDto.StoppingReason = decision.StoppingReason;
                    reformerStopFreeUpDetailDecisionDto.LateLitigationRequestNumber = decision.LateLitigationRequestNumber;
                    citizenReformer.ReformerStopFreeUpDetailDecisionDtos.Add(reformerStopFreeUpDetailDecisionDto);
                }
            }
            return new DataResponse<CitizenReformerDetailShortDto>()
            {
                IsDone = true,
                Data = citizenReformer
            };

        }

        public async Task<BaseResponse> GetArbitratorsDecisions(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            PagedResult<Citizen> citizens = null;

            var entitiesQuerable = (await _repository.GetQueryable<Citizen, Guid>())
                                      .Include(x => x.ArbitratorStopFreeUpDecisions)
                                      .Include(x => x.ArbitratorLitigationRequests)
                                      .Include(x => x.LitigationRequestArbitratorLists)
                       .Include(x => x.SysArbitratorStatus);


            citizens = entitiesQuerable.Where(x => x.IsArbitrator == true)
                                        .OrderByDescending(x => x.FullName)
                                        .AsSplitQuery()
                                        .AsNoTracking()
                                        .GetPaged(pageIndex, pageSize);


            List<CitizenArbitratorShortDto> citizenArbitrators = new List<CitizenArbitratorShortDto>();

            foreach (var citizen in citizens.Results)
            {
                CitizenArbitratorShortDto citizenArbitrator = new CitizenArbitratorShortDto();
                citizenArbitrator.Id = citizen.Id;
                citizenArbitrator.FullName = citizen.FullName;
                citizenArbitrator.Mobile = citizen.Mobile;
                citizenArbitrator.IdentityNumber = citizen.IdentityNumber;
                citizenArbitrator.ArbitratorStatus = citizen.SysArbitratorStatus?.StatusName;
                citizenArbitrator.ArbitratorClass = citizen.CitizenRequests?.OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
                citizenArbitrator.ExperienceYears = citizen.CitizenRequests?.OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
                citizenArbitrator.Email = citizen.Email;
                citizenArbitrator.RequestsCount = citizen.ArbitratorLitigationRequests.Count + citizen.LitigationRequestArbitratorLists.Count;
                citizenArbitrator.ArbitratorStatusDate = citizen.ArbitratorStopFreeUpDecisions?.OrderBy(x => x.DecisionDate).LastOrDefault()?.DecisionDate;
                citizenArbitrators.Add(citizenArbitrator);

            }


            return new DataResponse<PagedResult<CitizenArbitratorShortDto>>
            {
                Data =
                   new PagedResult<CitizenArbitratorShortDto>
                   {
                       Results = citizenArbitrators,
                       CurrentPage = citizens.CurrentPage,
                       PageCount = citizens.PageCount,
                       PageSize = citizens.PageSize,
                       TotalCount = citizens.TotalCount
                   },
                IsDone = true
            };






            return new DataResponse<List<CitizenArbitratorShortDto>>()
            {
                IsDone = true,
                Data = citizenArbitrators.OrderBy(x => x.RequestsCount).ToList()
            };

        }
        public async Task<BaseResponse> GetArbitratorsDecisions(Guid citizenId)
        {
            var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                .Include(x => x.ArbitratorLitigationRequests)
                .Include(x => x.LitigationRequestArbitratorLists)
                         .Include(x => x.SysArbitratorStatus)
                         .Include(x => x.ArbitratorStopFreeUpDecisions)
                        .ThenInclude(x => x.SysArbitratorStatus)
                         .Include(x => x.CitizenRequests)
                         .ThenInclude(x => x.SysArea)
                            .Include(x => x.CitizenRequests)
                         .ThenInclude(x => x.SysArbitratorClass)
                         .Where(x => x.Id == citizenId).SingleOrDefault();

            if (citizen == null)
                return new BaseResponse() { IsDone = false, Message = "حساب غير موجود" };

            CitizenArbitratorDetailShortDto citizenArbitrator = new CitizenArbitratorDetailShortDto();
            citizenArbitrator.Id = citizen.Id;
            citizenArbitrator.FullName = citizen.FullName;
            citizenArbitrator.Mobile = citizen.Mobile;
            citizenArbitrator.AcademicDegreeName = citizen.CitizenRequests?
                .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator)?
                .OrderBy(x => x.RequestOn).LastOrDefault()?.AcademicDegreeName;
            citizenArbitrator.IsVolunteerApproval = citizen.CitizenRequests?
               .Where(x => x.CitizenRequestTypeId == CitizenRequestType.Arbitrator)?
               .OrderBy(x => x.RequestOn).LastOrDefault()?.IsVolunteerApproval;
            citizenArbitrator.Fax = citizen.Fax;
            citizenArbitrator.HomeTelephone = citizen.HomeTelephone;
            citizenArbitrator.AreaName = citizen.CitizenRequests?.OrderBy(x => x.RequestOn).LastOrDefault()?.SysArea?.AreaName;
            citizenArbitrator.ArbitratorClass = citizen.CitizenRequests?.OrderBy(x => x.RequestOn).LastOrDefault()?.SysArbitratorClass?.ArbitratorClassName;
            citizenArbitrator.ExperienceYears = citizen.CitizenRequests?.OrderBy(x => x.RequestOn).LastOrDefault()?.ExperienceYears;
            citizenArbitrator.Brief = citizen.CitizenRequests?.OrderBy(x => x.RequestOn).LastOrDefault()?.Brief;
            citizenArbitrator.WorkTelephone = citizen.WorkTelephone;
            citizenArbitrator.IdentityNumber = citizen.IdentityNumber;
            citizenArbitrator.ArbitratorStatus = citizen.SysArbitratorStatus?.StatusName;
            citizenArbitrator.ArbitratorStatusId = citizen.ArbitratorStatusId;
            citizenArbitrator.Email = citizen.Email;
            citizenArbitrator.RequestsCount = citizen.ArbitratorLitigationRequests.Count + citizen.LitigationRequestArbitratorLists.Count;
            citizenArbitrator.ArbitratorStatusDate = citizen.ArbitratorStopFreeUpDecisions?.OrderBy(x => x.DecisionDate).LastOrDefault()?.DecisionDate;
            if (citizen.ArbitratorStopFreeUpDecisions != null)
            {
                foreach (var decision in citizen.ArbitratorStopFreeUpDecisions)
                {
                    ArbitratorStopFreeUpDetailDecisionDto ArbitratorStopFreeUpDetailDecisionDto = new ArbitratorStopFreeUpDetailDecisionDto();
                    ArbitratorStopFreeUpDetailDecisionDto.DecisionDate = decision.DecisionDate;
                    ArbitratorStopFreeUpDetailDecisionDto.ArbitratorStatus = decision.SysArbitratorStatus?.StatusName;
                    ArbitratorStopFreeUpDetailDecisionDto.StoppingReason = decision.StoppingReason;
                    ArbitratorStopFreeUpDetailDecisionDto.LateLitigationRequestNumber = decision.LateLitigationRequestNumber;
                    citizenArbitrator.ArbitratorStopFreeUpDetailDecisionDtos.Add(ArbitratorStopFreeUpDetailDecisionDto);
                }
            }
            return new DataResponse<CitizenArbitratorDetailShortDto>()
            {
                IsDone = true,
                Data = citizenArbitrator
            };

        }


        public async Task<BaseResponse> GetFacilities(RelationShipType relationShipId)
        {
            switch (relationShipId)
            {
                case RelationShipType.LawyerForCompany:
                    return await GetDelegatedFacilitiesLawyer();
                case RelationShipType.AgentForCompany:
                    return await GetDelegatedFacilitiesAgent();
                case RelationShipType.CompanyRepresentative:
                    return await GetOwnerFacilities();
                default:
                    return new DataResponse<List<DelegatorItem>>()
                    {
                        IsDone = true,
                        Data = null
                    };
            }
        }
        public async Task<BaseResponse> GetPersons(RelationShipType relationShipId)
        {
            switch (relationShipId)
            {
                case RelationShipType.LawyerForIndividual:
                    return await GetDelegatedPersonsLawyer();
                case RelationShipType.AgentForIndividual:
                    return await GetDelegatedPersonsAgent();
                default:
                    return new DataResponse<List<DelegatorItem>>()
                    {
                        IsDone = true,
                        Data = null
                    };
            }
        }

    }
}
