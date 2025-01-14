using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.Log.Application.Contracts.IAppServices;
using Core.Security.Dtos;
using Core.Security.Entities;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Delegator;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using System.Globalization;
using static REAC.Application.Contracts.IAppServices.Litigation.ILitigationRequestService;

namespace REAC.Application.AppServices
{
    public class DelegatorService : IDelegatorService
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ILoggerManager _loggerManager;
        private readonly IJwtReader _jwtReader;
        private readonly IMOJService _mOJService;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;
        private readonly UserManager<User> _userManager;

        public DelegatorService(IEntityRepository repository,
            IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, IMOJService mOJService, INotificationService notificationService, ISysSettingService sysSettingService, UserManager<User> userManager)
        {
            _repository = repository;
            _mapper = mapper;
            _jwtReader = jwtReader;
            _loggerManager = loggerManager;
            _mOJService = mOJService;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
            _userManager = userManager;
        }


        public async Task<BaseResponse> GetDelegators(int pageIndex, int pageSize, string? searchKey)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var delegators = (await _repository.GetQueryable<Delegator, Guid>())
                    .Include(x => x.Citizen).Include(x => x.Facility).Include(x => x.DelegatorCitizen)
                    .Include(x => x.DelegationFile)
                    .Include(x => x.SysDelegationRelationshipType)
                .Where(x => x.CitizenId == referenceId & x.DelegationRelationshipTypeId == DelegationRelationshipType.Agent)
                .ToList();

                var fullDelegators = _mapper.Map<List<DelegatorShortDataDto>>(delegators);

                var result = await PaginatedList<DelegatorShortDataDto>.CreateAsync(fullDelegators, pageIndex, pageSize);
                if (result.Items.Count == 0) return new BaseResponse() { IsDone = true, Message = "غير موجود" };

                return new DataResponse<PaginatedList<DelegatorShortDataDto>>()
                {
                    IsDone = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse { IsDone = false, Message = "خطأ داخلي" };

            }

        }
        public async Task<BaseResponse> GetDelegatorById(Guid delegatorId)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var delegators = (await _repository.GetQueryable<Delegator, Guid>())
                    .Include(x => x.Citizen).Include(x => x.Facility).Include(x => x.DelegatorCitizen)
                    .Include(x => x.DelegationFile)
                              .Where(x => x.Id == delegatorId)
                              .ToList();

                var fullDelegators = _mapper.Map<List<DelegatorFullDataDto>>(delegators);
                if (fullDelegators.Count > 0)
                {
                    return new DataResponse<DelegatorFullDataDto>()
                    {
                        IsDone = true,
                        Data = fullDelegators.FirstOrDefault()
                    };
                }
                else
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "غير موجود"
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }

        }

        public async Task<BaseResponse> SaveDelegator(DelegatorDto delegatorDto)
        {

            // Edit Delegator
            if (delegatorDto.Id != Guid.Empty)
            {
                return await EditAgentDelagator(delegatorDto);
            }

            // Add new Delegator
            return await AddAgentDelagator(delegatorDto);

        }

        public async Task<BaseResponse> ValidateAgentDelegator(ValidateDelegatorDto validateDelegatorDto)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                        .AsNoTracking().FirstOrDefault(x => x.Id == referenceId);

                var validationResult = await _mOJService.ExternalAttorneyInquiry(new MOJRequestDto()
                {
                    AttorneyCode = validateDelegatorDto.DelegateNumber,
                    FirstParties = citizen.IdentityNumber,
                    SecondParties = validateDelegatorDto.IdentityNumber
                });

                if (validationResult is null || !validationResult.IsDone || !validationResult.Data.IsValid || validationResult.Data?.Error is null)
                    return new BaseResponse() { IsDone = false, Message = "المحامي أو الوكيل غير صالح" };
                

                var secondParties = validationResult.Data.Data?.responseObject.Results?
                .Select(r => r.Model.SecondParties).FirstOrDefault();


                var citizenDataResult = secondParties?.FirstOrDefault(m => m.IdentityNo == validateDelegatorDto.IdentityNumber);

                return new DataResponse<PartiesDto>()
                {
                    IsDone = true,
                    Data = citizenDataResult
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }


        }

        public async Task<DataResponse<PartiesDto>> GetDelegatorCitizenData(GetDeligatorCitizenData getDelegatorDataDto)
        {

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                    .AsNoTracking().FirstOrDefault(x => x.Id == referenceId);

            var integrationDataResult = await _mOJService.ExternalAttorneyInquiry(new MOJRequestDto()
            {
                AttorneyCode = getDelegatorDataDto.DelegateNumber,
                FirstParties = citizen.IdentityNumber,
                SecondParties = getDelegatorDataDto.DelegatorIdentityNumber
            });


            if (integrationDataResult is null || !integrationDataResult.IsDone || integrationDataResult.Data?.Error is not null || !integrationDataResult.Data.IsValid)
                return new DataResponse<PartiesDto>() { IsDone = false, Message = "المحامي أو الوكيل غير صالح" };


            var secondParties = integrationDataResult.Data.Data?.responseObject.Results?
                .Select(r => r.Model.SecondParties).FirstOrDefault();


            var citizenDataResult = secondParties?.FirstOrDefault(m => m.IdentityNo == getDelegatorDataDto.DelegatorIdentityNumber);

            return new DataResponse<PartiesDto>()
            {
                IsDone = true,
                Data = citizenDataResult
            };

        }

        private async Task<BaseResponse> EditAgentDelagator(DelegatorDto delegatorDto)
        {
            try
            {
                // Get Delegator
                var delegator = (await _repository.GetQueryable<Delegator, Guid>()).Where(f => f.Id == delegatorDto.Id).FirstOrDefault();
                if (delegator is null) return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                delegator.DelegationFileId = delegatorDto.DelegationFileId;
                delegator.DelegationRelationshipTypeId = (DelegationRelationshipType)delegatorDto.DelegationRelationshipTypeId;
                delegator.FacilityId = delegatorDto.FacilityId;
                delegator.IsActive = delegatorDto.IsActive;

                // Get Delegator Citizen Data
                var result = await _repository.GetByIdAsync<Citizen, Guid>(delegator.DelegatorCitizenId);
                if (!result.IsDone) return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var delegatorCitizen = ((DataResponse<Citizen>)result).Data;

                delegatorCitizen.IsLawyer = delegatorDto.DelegationRelationshipTypeId == DelegationRelationshipType.Lawyer ? true : false;


                var saveCurrentCitizen = _repository.Update<Citizen, Guid>(delegatorCitizen);
                var saveCurrentDelegator = _repository.Update<Delegator, Guid>(delegator);

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone) return saveResponse;

                var updatedDelegator = await GetDelegatorById(delegatorDto.Id);
                if (!updatedDelegator.IsDone) return updatedDelegator;

                updatedDelegator.Message = "تم التعديل بنجاح";
                return updatedDelegator;
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }

        }

        private async Task<BaseResponse> AddAgentDelagator(DelegatorDto delegatorDto)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var sourceCitizen = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.Id == referenceId);

                if (sourceCitizen is null)
                    return new BaseResponse() { IsDone = false, Message = "المستخدم غير موجود" };

                var lawyerOrAgentCitizen = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.IdentityNumber == delegatorDto.IdentityNumber);

                if (lawyerOrAgentCitizen is null)
                {
                    var integrationCitizen = await GetDelegatorCitizenData(new GetDeligatorCitizenData()
                    {
                        DelegatorIdentityNumber = delegatorDto.IdentityNumber,
                        DelegateNumber = string.IsNullOrEmpty(delegatorDto.DelegateNumber) ? 0 : int.Parse(delegatorDto.DelegateNumber),
                        UserIdentityNumber = sourceCitizen.IdentityNumber,
                    });

                    if (integrationCitizen.Data is null || !integrationCitizen.IsDone)
                        return new BaseResponse() { IsDone = false, Message = "خطأ في رقم الهوية" };

                    CultureInfo provider = new CultureInfo("en-US");
                    CultureInfo.CurrentCulture = provider;

                    Citizen citizenFromintegration = new Citizen()
                    {
                        Id = Guid.NewGuid(),
                        FullName = integrationCitizen.Data.FullName,
                        ShortName = integrationCitizen.Data.FirstName,
                        Mobile = integrationCitizen.Data.MobileNumber,
                        IdentityNumber = delegatorDto.IdentityNumber,
                        GenderId = (integrationCitizen.Data.GenderId == Gender.Male.GetHashCode()) ? Gender.Male : Gender.Female,
                        BirthDate = DateTime.Parse(integrationCitizen.Data.BirthDate),
                        IdentityTypeId = (IdentityType?)(int)(integrationCitizen.Data.IdentityNo.StartsWith("1") ? IdentityType.NationalIdentity : integrationCitizen.Data.IdentityNo.StartsWith("2") ? IdentityType.Residence : IdentityType.GulfIdentity),
                        IsDraftAccount = true,
                        Code = await GenerateCitizenCode()
                    };


                    if (delegatorDto.DelegationRelationshipTypeId == DelegationRelationshipType.Lawyer)
                    {
                        citizenFromintegration.IsLawyer = true;
                        citizenFromintegration.LawyerLicenseNumber = delegatorDto.LawyerLicenseNumber;
                        citizenFromintegration.LawLicenseFileId = delegatorDto.LawLicenseFileId;
                    }

                    var response = await _repository.AddAsync<Citizen, Guid>(citizenFromintegration);
                    if (!response.IsDone) return new BaseResponse() { IsDone = false, Message = "خطأ في الاضافة" };

                    delegatorDto.DelegatorCitizenId = citizenFromintegration.Id;
                }
                else
                {
                    delegatorDto.DelegatorCitizenId = lawyerOrAgentCitizen.Id;
                }

                var updatedCitizen = _mapper.Map<Delegator>(delegatorDto);
                updatedCitizen.CitizenId = referenceId;

                await _repository.AddAsync<Delegator, Guid>(updatedCitizen);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone) return saveResponse;

                // Send Notification To User and Delegator

                try
                {
                    var agentInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.Id == delegatorDto.DelegatorCitizenId);

                    var citizenMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Citizen_AddAgent).Result.Data?.Value;
                    citizenMsg = citizenMsg.Replace("{CitizenName}", agentInfo.FullName);

                    var agentMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_AddAgent).Result.Data?.Value;
                    agentMsg = agentMsg.Replace("{CitizenName}", sourceCitizen.FullName);


                    _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = referenceId,
                        ActorTypeId = NotificationActorType.Applicant,
                        RequestId = agentInfo.Id,
                        RequestTypeId = RequestType.AddAgent,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = citizenMsg,
                        }
                    });


                    _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = agentInfo.Id,
                        ActorTypeId = NotificationActorType.Agent,
                        RequestId = updatedCitizen.Id,
                        RequestTypeId = RequestType.AddAgent,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = agentMsg,
                        }
                    });

                }
                catch (Exception ex)
                {
                }


                return new BaseResponse() { IsDone = true, Message = "تم الاضافة بنجاح" };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }


        }

        public async Task<string> GenerateCitizenCode()
        {
            var citizenRequestList = (await _repository.GetQueryable<Citizen, Guid>()).Where(x => x.Code != null).ToList();


            string code = "CZ-0001";

            if (citizenRequestList is null)
                code = "CZ-0001";
            else
                code = "CZ-" + (citizenRequestList.Count() + 1).ToString("D4");

            return code;
        }


        public async Task<BaseResponse> GetLawyersDelegators(int pageIndex, int pageSize, string? searchKey)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var delegators = (await _repository.GetQueryable<Delegator, Guid>())
                    .Include(x => x.Citizen).Include(x => x.Facility).Include(x => x.DelegatorCitizen)
                    .Include(x => x.DelegationFile)
                    .Include(x => x.SysDelegationRelationshipType)
                .Where(x => x.CitizenId == referenceId & x.DelegationRelationshipTypeId == DelegationRelationshipType.Lawyer & x.IsActive == true)
                .ToList();

                var fullDelegators = _mapper.Map<List<DelegatorShortDataDto>>(delegators);

                var result = await PaginatedList<DelegatorShortDataDto>.CreateAsync(fullDelegators, pageIndex, pageSize);
                if (result.Items.Count == 0) return new BaseResponse() { IsDone = true, Message = "غير موجود" };

                return new DataResponse<PaginatedList<DelegatorShortDataDto>>()
                {
                    IsDone = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse { IsDone = false, Message = "خطأ داخلي" };

            }

        }
        public async Task<BaseResponse> GetLawyerDelegatorById(Guid delegatorId)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var delegators = (await _repository.GetQueryable<Delegator, Guid>())
                    .Include(x => x.Citizen).Include(x => x.Facility).Include(x => x.DelegatorCitizen)
                    .Include(x => x.DelegationFile)
                              .Where(x => x.Id == delegatorId)
                              .ToList();

                var fullDelegators = _mapper.Map<List<DelegatorFullDataDto>>(delegators);
                if (fullDelegators.Count > 0)
                {
                    return new DataResponse<DelegatorFullDataDto>()
                    {
                        IsDone = true,
                        Data = fullDelegators.FirstOrDefault()
                    };
                }
                else
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "غير موجود"
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
            }

        }


        public async Task<BaseResponse> ValidateLawyerDelagator(ValidateLawyerDelegatorDto delegatorDto)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var sourceCitizen = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.Id == referenceId);

                if (sourceCitizen is null)
                    return new BaseResponse() { IsDone = false, Message = "المستخدم غير موجود" };

                var identityCheck = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.IdentityNumber == delegatorDto.IdentityNumber & x.IsLawyer == true);
                var licenseCheck = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.LawyerLicenseNumber == delegatorDto.LawyerLicenseNumber & x.IsLawyer == true);

                if ((identityCheck is not null & licenseCheck is null) || (identityCheck is null & licenseCheck is not null) || (identityCheck?.Id != licenseCheck?.Id))
                {
                    return new BaseResponse() { IsDone = false, Message = "البيانات المدخله خطأ" };

                }

                var lawyerCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                    .FirstOrDefault(x => x.IdentityNumber == delegatorDto.IdentityNumber & x.LawyerLicenseNumber == delegatorDto.LawyerLicenseNumber & x.IsLawyer == true);

                if (lawyerCitizen is not null)
                {

                    return new DataResponse<LawyerDelegatorDto>()
                    {
                        IsDone = true,
                        Data = new LawyerDelegatorDto()
                        {
                            Id = lawyerCitizen.Id,
                            FullName = lawyerCitizen.FullName,
                        }
                    };
                }


                return new DataResponse<LawyerDelegatorDto>()
                {
                    IsDone = true,
                    Data = null
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }


        }

        public async Task<BaseResponse> AddLawyerDelagator(LawyerDelegatorDto delegatorDto)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var sourceCitizen = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.Id == referenceId);

                if (sourceCitizen is null)
                    return new BaseResponse() { IsDone = false, Message = "المستخدم غير موجود" };

                var lawyerCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                    .FirstOrDefault(x => x.Id == delegatorDto.Id);


                if (lawyerCitizen is null)
                    return new BaseResponse() { IsDone = false, Message = " المحامي غير موجود" };

                var lawyerDelegator = (await _repository.GetQueryable<Delegator, Guid>())
                    .FirstOrDefault(x => x.DelegatorCitizenId == lawyerCitizen.Id & x.CitizenId == referenceId);

                if (lawyerDelegator is not null)
                    return new BaseResponse() { IsDone = false, Message = " المحامي موجود مسبقا" };


                var newLawyerDelegatorObject = new Delegator()
                {
                    Id = new Guid(),
                    DelegatorCitizenId = lawyerCitizen.Id,
                    IsActive = true,
                    CitizenId = referenceId,
                    DelegationRelationshipTypeId = DelegationRelationshipType.Lawyer,
                    DelegationFileId = delegatorDto.DelegationFileId
                };


                await _repository.AddAsync<Delegator, Guid>(newLawyerDelegatorObject);
                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone) return saveResponse;


                try
                {

                    var citizenMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Citizen_AddLawyer).Result.Data?.Value;
                    citizenMsg = citizenMsg.Replace("{CitizenName}", sourceCitizen.FullName);

                    var lawyerMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_AddLawyer).Result.Data?.Value;
                    lawyerMsg = lawyerMsg.Replace("{CitizenName}", lawyerCitizen.FullName);


                    _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = referenceId,
                        ActorTypeId = NotificationActorType.Applicant,
                        RequestId = lawyerCitizen.Id,
                        RequestTypeId = RequestType.AddLawyer,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = citizenMsg,
                        }
                    });


                    _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = lawyerCitizen.Id,
                        ActorTypeId = NotificationActorType.Lawyer,
                        RequestId = lawyerCitizen.Id,
                        RequestTypeId = RequestType.AddLawyer,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = lawyerMsg,
                        }
                    });

                }
                catch (Exception ex)
                {
                }


                return new BaseResponse() { IsDone = true, Message = "تم الاضافة بنجاح" };



            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }

        public async Task<BaseResponse> AddLawyerCitizenAndDelagator(LawyerDelegatorDto delegatorDto)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var sourceCitizen = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.Id == referenceId);

                if (sourceCitizen is null)
                    return new BaseResponse() { IsDone = false, Message = "المستخدم غير موجود" };

                var lawyerCitizen = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.IdentityNumber == delegatorDto.IdentityNumber);
                var lawyerUser = _userManager.Users.FirstOrDefault(u => u.UserName == delegatorDto.IdentityNumber);



                if (lawyerCitizen is null)
                {

                    CultureInfo provider = new CultureInfo("en-US");
                    CultureInfo.CurrentCulture = provider;

                    Citizen newLawyerCitizen = new Citizen()
                    {
                        Id = Guid.NewGuid(),
                        FullName = delegatorDto.FullName,
                        IdentityNumber = delegatorDto.IdentityNumber,
                        IsDraftAccount = true,
                        Code = await GenerateCitizenCode(),
                        IsLawyer = true,
                        LawyerLicenseNumber = delegatorDto.LawyerLicenseNumber,
                        Mobile = delegatorDto.MobileNumber
                    };

                    var response = await _repository.AddAsync<Citizen, Guid>(newLawyerCitizen);
                    if (!response.IsDone) return new BaseResponse() { IsDone = false, Message = "خطأ في الاضافة" };




                    delegatorDto.DelegatorCitizenId = newLawyerCitizen.Id;

                }
                else
                {
                    delegatorDto.DelegatorCitizenId = lawyerCitizen.Id;

                }


                var updatedCitizen = _mapper.Map<Delegator>(delegatorDto);
                updatedCitizen.CitizenId = referenceId;
                updatedCitizen.IsActive = true;
                await _repository.AddAsync<Delegator, Guid>(updatedCitizen);


                // Add new Asp .Net User
                if (lawyerUser is null)
                {
                    var newUser = new User { UserName = delegatorDto.IdentityNumber.Trim(), EmailConfirmed = false };
                    var result = await _userManager.CreateAsync(newUser, "123456");

                    if (result.Errors.Any())
                    {
                        throw new Exception();

                    }

                }

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone) return saveResponse;

                try
                {

                    var citizenMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Citizen_AddLawyer).Result.Data?.Value;
                    citizenMsg = citizenMsg.Replace("{CitizenName}", delegatorDto.FullName);

                    var lawyerMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_AddLawyer).Result.Data?.Value;
                    lawyerMsg = lawyerMsg.Replace("{CitizenName}", sourceCitizen.FullName);


                    _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = referenceId,
                        ActorTypeId = NotificationActorType.Applicant,
                        RequestId = (Guid) delegatorDto.DelegatorCitizenId,
                        RequestTypeId = RequestType.AddLawyer,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = citizenMsg,
                        }
                    });


                    _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = (Guid)delegatorDto.DelegatorCitizenId,
                        ActorTypeId = NotificationActorType.Lawyer,
                        RequestId = (Guid)delegatorDto.DelegatorCitizenId,
                        RequestTypeId = RequestType.AddLawyer,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = lawyerMsg,
                        }
                    });

                }
                catch (Exception ex)
                {
                }


                return new BaseResponse() { IsDone = true, Message = "تم الاضافة بنجاح" };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }

        public async Task<BaseResponse> DeleteLawyerDelegator(Guid delegatorId)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var sourceCitizen = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(x => x.Id == referenceId);

                if (sourceCitizen is null)
                    return new BaseResponse() { IsDone = false, Message = "المستخدم غير موجود" };


                var delegator = (await _repository.GetQueryable<Delegator, Guid>())
                    .Include(x => x.DelegatorCitizen)
                    .FirstOrDefault(x => x.Id == delegatorId);

                if (delegator != null)
                {
                    var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                        .Include(x => x.Contract)
                        .ThenInclude(x => x.FirstPartyCitizen)
                        .Include(x => x.Contract)
                        .ThenInclude(x => x.SecondPartyCitizen)
                        .Include(x => x.Contract)
                        .ThenInclude(x => x.FirstPartyFacility)
                        .ThenInclude(x => x.Citizen)
                        .Include(x => x.Contract)
                        .ThenInclude(x => x.SecondPartyFacility)
                        .ThenInclude(x => x.Citizen)
                        .Where(x => (x.ApplicantCitizenId == delegator.DelegatorCitizenId) ||
                        (x.Contract.FirstPartyCitizen != null && x.Contract.FirstPartyCitizenId == delegator.DelegatorCitizenId) ||
                        (x.Contract.FirstPartyFacility != null && x.Contract.FirstPartyFacility.Citizen.Id == delegator.DelegatorCitizenId)
                        )
                        .Any();

                    if (!(bool)litigationRequest)
                    {

                        var delegatorResult = _repository.Remove<Delegator, Guid>(delegator);
                    }
                    else
                    {
                        delegator.IsActive = false;
                    }



                    var saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;

                    // Send Notification

                    try
                    {

                        var citizenMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Citizen_DeleteLawyer).Result.Data?.Value;
                        citizenMsg = citizenMsg.Replace("{CitizenName}", sourceCitizen.FullName);

                        var lawyerMsg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_DeleteLawyer).Result.Data?.Value;
                        lawyerMsg = lawyerMsg.Replace("{CitizenName}", delegator.DelegatorCitizen.FullName);


                        _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = referenceId,
                        ActorTypeId = NotificationActorType.Applicant,
                        RequestId = delegator.DelegatorCitizen.Id,
                        RequestTypeId = RequestType.DeleteLawyer,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = citizenMsg,
                        }
                    });


                        _notificationService.SendNotifications(null, null, new List<Contracts.Dtos.Notification.SysNotificationDto>()
                    {
                        new Contracts.Dtos.Notification.SysNotificationDto()
                        {
                             Id = Guid.NewGuid(),
                        ActorId = delegator.DelegatorCitizen.Id,
                        ActorTypeId = NotificationActorType.Lawyer,
                        RequestId = delegator.DelegatorCitizen.Id,
                        RequestTypeId = RequestType.DeleteLawyer,
                        IsViewed = false,
                        ViewedOn = null,
                        CreatedOn = DateTime.Now,
                        Message = lawyerMsg,
                        }
                    });

                    }
                    catch (Exception ex)
                    {
                    }

                    return new BaseResponse() { IsDone = true, Message = "تم الحذف بنجاح" };

                }

                return new BaseResponse() { IsDone = false, Message = "غير موجود" };


            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }
    }
}
