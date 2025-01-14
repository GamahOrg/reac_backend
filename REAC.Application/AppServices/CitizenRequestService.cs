using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.CitizenRequests;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;

namespace REAC.Application.AppServices
{
    public class CitizenRequestService : BasicService<CitizenRequest, Guid>, ICitizenRequestService
    {
        protected IEntityRepository _repository;
        protected IMapper _mapper;
        protected ILoggerManager _loggerManager;
        private readonly IJwtReader _jwtReader;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;
        public CitizenRequestService(IEntityRepository repository,
          IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysEmployeeService employeeService, INotificationService notificationService, ISysSettingService sysSettingService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _repository = repository;
            _mapper = mapper;
            _jwtReader = jwtReader;
            _loggerManager = loggerManager;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
        }

        public async Task<BaseResponse> GetCitizenRequests(int pageIndex, int pageSize, bool isSend = false, CitizenRequestType? citizenRequestTypeId = null)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var citizenRequestsQuery = (await _repository.GetQueryable<CitizenRequest, Guid>())
                        .Include(x => x.Citizen)
                        .Include(x => x.SysCitizenRequestType)
                        .Include(x => x.Status)
                        .Include(x => x.SysArbitratorClass)
                        .Include(x => x.SysArea)
                        .Include(x => x.CitizenRequestAcademicDegreeFiles)
                        .ThenInclude(x => x.AcademicDegreeFile)
                        .Include(x => x.CitizenRequestNotes)
                        .Include(x => x.CitizenRequestFreelanceWorkDocumentFiles)
                        .ThenInclude(x => x.FreelanceWorkDocumentFile)
                        .Include(x => x.CitizenRequestExperienceCertificates)
                        .ThenInclude(x => x.CitizenRequestExperienceCertificateFiles)
                        .ThenInclude(x => x.ExperienceCertificateFile)
                        .AsNoTracking();



                var citizen = (await _repository.GetQueryable<Citizen, Guid>()).Where(x => x.Id == referenceId).SingleOrDefault();


                if (citizen != null)
                    citizenRequestsQuery = citizenRequestsQuery.Where(x => x.CitizenId == referenceId);

                if (isSend)
                {
                    citizenRequestsQuery = citizenRequestsQuery.Where(x => x.StatusId != CitizenRequestStatus.Draft && x.CitizenRequestTypeId == citizenRequestTypeId);
                }


                var citizenRequests = citizenRequestsQuery.GetPaged(pageIndex, pageSize);

                return new DataResponse<PagedResult<CitizenRequestShortDataDto>>()
                {
                    IsDone = true,
                    Data = new PagedResult<CitizenRequestShortDataDto>
                    {
                        CurrentPage = citizenRequests.CurrentPage,
                        PageSize = citizenRequests.PageSize,
                        PageCount = citizenRequests.PageCount,
                        TotalCount = citizenRequests.TotalCount,
                        Results = _mapper.Map<List<CitizenRequestShortDataDto>>(citizenRequests.Results)
                    }
                };
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<BaseResponse> GetCitizenRequestById(Guid citizenRequestId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            var citizenRequests = (await _repository.GetQueryable<CitizenRequest, Guid>())
                .Where(x => x.Id == citizenRequestId)
                .Include(x => x.SysCitizenRequestType)
                .Include(x => x.Status)
                .Include(x => x.SysArbitratorClass)
                .Include(x => x.SysArea)
                .Include(x => x.Citizen)
                .Include(x => x.CitizenRequestNotes)
                .Include(x => x.CitizenRequestAcademicDegreeFiles)
                .ThenInclude(x => x.AcademicDegreeFile)
                .Include(x => x.CitizenRequestNotes)
                .Include(x => x.CitizenRequestFreelanceWorkDocumentFiles)
                .ThenInclude(x => x.FreelanceWorkDocumentFile)
                .Include(x => x.CitizenRequestExperienceCertificates)
                .ThenInclude(x => x.CitizenRequestExperienceCertificateFiles)
                .ThenInclude(x => x.ExperienceCertificateFile)
                .AsSplitQuery().AsNoTracking().ToList();

            if (citizenRequests.Count > 0)
            {
                var fullDelegators = _mapper.Map<CitizenRequestFullDataDto>(citizenRequests.FirstOrDefault());

                return new DataResponse<CitizenRequestFullDataDto>()
                {
                    IsDone = true,
                    Data = fullDelegators
                };
            }
            return new BaseResponse()
            {
                IsDone = false,
                Message = "Not Found"
            };
        }

        public async Task<BaseResponse> SaveCitizenRequest(CitizenRequestCreateUpdateDto citizenRequestCreateUpdateDto)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);
            if (citizenRequestCreateUpdateDto.Id == Guid.Empty)
            {
                var request = _mapper.Map<CitizenRequest>(citizenRequestCreateUpdateDto);
                request.CitizenId = referenceId;
                request.Id = Guid.NewGuid();
                if (citizenRequestCreateUpdateDto.IsSend)
                    request.StatusId = CitizenRequestStatus.WaitingForCertify;
                else
                    request.StatusId = CitizenRequestStatus.Draft;

                request.RequestOn = DateTime.Now;
                await _repository.AddAsync<CitizenRequest, Guid>(request);
                var response = await _repository.SaveChangesAsync();
                if (!response.IsDone) return response;

                if (citizenRequestCreateUpdateDto.IsSend)
                    await ExecuteAction(CitizenRequestAction.Send, request.Id, null);


                return response;


            }
            else
            {
                var citizenRequests = (await _repository.GetQueryable<CitizenRequest, Guid>())
                    .Where(x => x.Id == citizenRequestCreateUpdateDto.Id)
                    .Include(x => x.SysCitizenRequestType)
                    .Include(x => x.Status)
                    .Include(x => x.SysArbitratorClass)
                    .Include(x => x.SysArea)
                    .Include(x => x.CitizenRequestAcademicDegreeFiles).ThenInclude(x => x.AcademicDegreeFile)
                    .Include(x => x.CitizenRequestExperienceCertificates)
                    .Include(x => x.CitizenRequestProgramRealEstateInstituteFiles)
                    .Include(x => x.CitizenRequestCVFiles)
                    .Include(x => x.CitizenRequestNotes)
                    .Include(x => x.CitizenRequestFreelanceWorkDocumentFiles).ThenInclude(x => x.FreelanceWorkDocumentFile)
                    .Include(x => x.CitizenRequestExperienceCertificates).ThenInclude(x => x.CitizenRequestExperienceCertificateFiles)
                    .ThenInclude(x => x.ExperienceCertificateFile)
                    .AsSplitQuery().FirstOrDefault();

                if (citizenRequests is null)
                    return new BaseResponse() { IsDone = false, Message = "Not Found" };

                #region citizenRequest

                citizenRequests.AreaId = citizenRequestCreateUpdateDto.AreaId;
                citizenRequests.AcademicDegreeName = citizenRequestCreateUpdateDto.AcademicDegreeName;
                citizenRequests.AcademicDegreeName = citizenRequestCreateUpdateDto.AcademicDegreeName;
                citizenRequests.ExperienceYears = citizenRequestCreateUpdateDto.ExperienceYears;
                citizenRequests.ExperienceYears = citizenRequestCreateUpdateDto.ExperienceYears;
                citizenRequests.ArbitratorClassId = citizenRequestCreateUpdateDto.ArbitratorClassId;
                citizenRequests.IsVolunteerApproval = citizenRequestCreateUpdateDto.IsVolunteerApproval;
                citizenRequests.Brief = citizenRequestCreateUpdateDto.Brief;

                #endregion

                var response = new BaseResponse { IsDone = true };

                #region CitizenRequestFreelanceWorkDocumentFiles

                var deleteCitizenRequestFreelanceWorkDocumentFiles = citizenRequests.CitizenRequestFreelanceWorkDocumentFiles
                    .Where(x => !citizenRequestCreateUpdateDto.CitizenRequestFreelanceWorkDocumentFiles.Any(y => y.Id == x.Id));

                _repository.RemoveRange<CitizenRequestFreelanceWorkDocumentFile, Guid>(deleteCitizenRequestFreelanceWorkDocumentFiles);
                response = await _repository.SaveChangesAsync();
                if (!response.IsDone) return response;

                foreach (var citizenRequestFreelanceWorkDocument in citizenRequestCreateUpdateDto.CitizenRequestFreelanceWorkDocumentFiles)
                {
                    if (citizenRequestFreelanceWorkDocument.Id == Guid.Empty)
                    {
                        await _repository.AddAsync<CitizenRequestFreelanceWorkDocumentFile, Guid>(new CitizenRequestFreelanceWorkDocumentFile
                        {
                            Id = Guid.NewGuid(),
                            CitizenRequestId = citizenRequests.Id,
                            FreelanceWorkDocumentFileId = citizenRequestFreelanceWorkDocument.FreelanceWorkDocumentFileId
                        });
                        response = await _repository.SaveChangesAsync();
                        if (!response.IsDone) return response;
                    }
                }


                #endregion

                #region CitizenRequestAcademicDegreeFiles

                var deleteCitizenRequestAcademicDegreeFiles = citizenRequests.CitizenRequestAcademicDegreeFiles
                    .Where(x => !citizenRequestCreateUpdateDto.CitizenRequestAcademicDegreeFiles.Any(y => y.Id == x.Id));

                _repository.RemoveRange<CitizenRequestAcademicDegreeFile, Guid>(deleteCitizenRequestAcademicDegreeFiles);
                response = await _repository.SaveChangesAsync();
                if (!response.IsDone) return response;


                foreach (var citizenRequestAcademicDegree in citizenRequestCreateUpdateDto.CitizenRequestAcademicDegreeFiles)
                {
                    if (citizenRequestAcademicDegree.Id == Guid.Empty)
                    {
                        await _repository.AddAsync<CitizenRequestAcademicDegreeFile, Guid>(new CitizenRequestAcademicDegreeFile
                        {
                            CitizenRequestId = citizenRequests.Id,
                            Id = Guid.NewGuid(),
                            AcademicDegreeFileId = citizenRequestAcademicDegree.AcademicDegreeFileId
                        });
                        response = await _repository.SaveChangesAsync();
                        if (!response.IsDone) return response;
                    }
                }

                #endregion

                #region CitizenRequestExperienceCertificates

                var deleteCitizenRequestExperienceCertificates = citizenRequests.CitizenRequestExperienceCertificates
                    .Where(x => !citizenRequestCreateUpdateDto.CitizenRequestExperienceCertificates.Any(y => y.Id == x.Id));

                _repository.RemoveRange<CitizenRequestExperienceCertificate, Guid>(deleteCitizenRequestExperienceCertificates);
                response = await _repository.SaveChangesAsync();
                if (!response.IsDone) return response;


                foreach (var citizenRequestExperienceCertificates in citizenRequestCreateUpdateDto.CitizenRequestExperienceCertificates)
                {
                    if (citizenRequestExperienceCertificates.AcquisitionDate > DateTime.Now)
                        return new BaseResponse() { IsDone = false, Message = "تأكد من سنة الحصول على شهادة الخبرة" };

                    if (citizenRequestExperienceCertificates.Id == Guid.Empty)
                    {
                        CitizenRequestExperienceCertificate citizenRequestExperienceCertificate = new CitizenRequestExperienceCertificate();
                        citizenRequestExperienceCertificate.Id = Guid.NewGuid();
                        citizenRequestExperienceCertificate.CitizenRequestId = citizenRequests.Id;
                        citizenRequestExperienceCertificate.AcquisitionDate = citizenRequestExperienceCertificates.AcquisitionDate;
                        citizenRequestExperienceCertificate.CertificateName = citizenRequestExperienceCertificates.CertificateName;
                        citizenRequestExperienceCertificate.IssuingEntityName = citizenRequestExperienceCertificates.IssuingEntityName;
                        await _repository.AddAsync<CitizenRequestExperienceCertificate, Guid>(citizenRequestExperienceCertificate);
                        response = await _repository.SaveChangesAsync();
                        if (!response.IsDone) return response;

                        foreach (var item in citizenRequestExperienceCertificates.CitizenRequestExperienceCertificateFiles)
                        {
                            await _repository.AddAsync<CitizenRequestExperienceCertificateFile, Guid>(new CitizenRequestExperienceCertificateFile()
                            {
                                CitizenRequestExperienceCertificateId = citizenRequestExperienceCertificate.Id,
                                Id = Guid.NewGuid(),
                                ExperienceCertificateFileId = item.ExperienceCertificateFileId
                            });
                            response = await _repository.SaveChangesAsync();
                            if (!response.IsDone) return response;
                        }
                    }
                }

                #endregion

                #region CitizenRequestReconciliationCenterDegreeFiles

                var deleteCitizenRequestReconciliationCenterDegreeFiles = citizenRequests.CitizenRequestReconciliationCenterDegreeFiles
                    .Where(x => !citizenRequestCreateUpdateDto.CitizenRequestReconciliationCenterDegreeFiles.Any(y => y.Id == x.Id));

                _repository.RemoveRange<CitizenRequestReconciliationCenterDegreeFile, Guid>(deleteCitizenRequestReconciliationCenterDegreeFiles);
                response = await _repository.SaveChangesAsync();
                if (!response.IsDone) return response;

                foreach (var citizenRequestReconciliationCenterDegreeDocument in citizenRequestCreateUpdateDto.CitizenRequestReconciliationCenterDegreeFiles)
                {
                    if (citizenRequestReconciliationCenterDegreeDocument.Id == Guid.Empty)
                    {
                        await _repository.AddAsync<CitizenRequestReconciliationCenterDegreeFile, Guid>(new CitizenRequestReconciliationCenterDegreeFile
                        {
                            Id = Guid.NewGuid(),
                            CitizenRequestId = citizenRequests.Id,
                            ReconciliationCenterDegreeFileId = citizenRequestReconciliationCenterDegreeDocument.ReconciliationCenterDegreeFileId
                        });
                        response = await _repository.SaveChangesAsync();
                        if (!response.IsDone) return response;
                    }
                }


                #endregion

                #region CitizenRequestRealStateProgramFiles

                var deleteRealStateProgramCenterDegreeFiles = citizenRequests.CitizenRequestProgramRealEstateInstituteFiles
                    .Where(x => !citizenRequestCreateUpdateDto.CitizenRequestProgramRealEstateInstituteFiles.Any(y => y.Id == x.Id));

                _repository.RemoveRange<CitizenRequestProgramRealEstateInstituteFile, Guid>(deleteRealStateProgramCenterDegreeFiles);
                response = await _repository.SaveChangesAsync();
                if (!response.IsDone) return response;

                foreach (var citizenRequesRealStateProgramDegreeDocument in citizenRequestCreateUpdateDto.CitizenRequestProgramRealEstateInstituteFiles)
                {
                    if (citizenRequesRealStateProgramDegreeDocument.Id == Guid.Empty)
                    {
                        await _repository.AddAsync<CitizenRequestProgramRealEstateInstituteFile, Guid>(new CitizenRequestProgramRealEstateInstituteFile
                        {
                            Id = Guid.NewGuid(),
                            CitizenRequestId = citizenRequests.Id,
                            RealEstateInstituteFileId = citizenRequesRealStateProgramDegreeDocument.ProgramRealEstateInstituteFileId
                        });
                        response = await _repository.SaveChangesAsync();
                        if (!response.IsDone) return response;
                    }
                }


                #endregion

                #region CitizenRequestCVFiles

                var deleteCVFiles = citizenRequests.CitizenRequestCVFiles
                    .Where(x => !citizenRequestCreateUpdateDto.CitizenRequestCVFiles.Any(y => y.Id == x.Id));

                _repository.RemoveRange<CitizenRequestCVFile, Guid>(deleteCVFiles);
                response = await _repository.SaveChangesAsync();
                if (!response.IsDone) return response;

                foreach (var citizenRequesCvDocument in citizenRequestCreateUpdateDto.CitizenRequestCVFiles)
                {
                    if (citizenRequesCvDocument.Id == Guid.Empty)
                    {
                        await _repository.AddAsync<CitizenRequestCVFile, Guid>(new CitizenRequestCVFile
                        {
                            Id = Guid.NewGuid(),
                            CitizenRequestId = citizenRequests.Id,
                            CVFileId = citizenRequesCvDocument.CVFileId
                        });
                        response = await _repository.SaveChangesAsync();
                        if (!response.IsDone) return response;
                    }
                }


                #endregion

                if (citizenRequestCreateUpdateDto.IsSend && citizenRequests.StatusId == CitizenRequestStatus.Draft)
                    await ExecuteAction(CitizenRequestAction.Send, citizenRequests.Id, null);
                if (citizenRequestCreateUpdateDto.IsSend && citizenRequests.StatusId == CitizenRequestStatus.SentForEdit)
                    await ExecuteAction(CitizenRequestAction.SendFromEdit, citizenRequests.Id, null);

                if (citizenRequestCreateUpdateDto.IsFeesPaid.HasValue && citizenRequestCreateUpdateDto.IsFeesPaid.Value && citizenRequests.StatusId == CitizenRequestStatus.SendToPayFees)
                    await ExecuteAction(CitizenRequestAction.FeesPaid, citizenRequests.Id, null);

                return response;

            }
        }

        public async Task<BaseResponse> ExecuteAction(CitizenRequestAction citizenRequestAction, Guid requestId, string? notes)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);



            var citizenRequests = (await _repository.GetQueryable<CitizenRequest, Guid>())
                .Include(c => c.Citizen)
                  .FirstOrDefault(x => x.Id == requestId);

            if (citizenRequests is null)
                return new BaseResponse() { IsDone = false, Message = "Not Found" };

            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                    .Where(c => c.Id == citizenRequests.CitizenId)
                    .FirstOrDefault();

            BaseResponse saveResponse = new BaseResponse() { IsDone = false, Message = "Not Found" };

            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            var deptAdminInfoReconciliation = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Reconciliation);
            var msg = "";
            var applicationNotifications = new List<SysNotificationDto>();

            switch (citizenRequestAction)
            {
                case CitizenRequestAction.Send:
                    citizenRequests.StatusId = CitizenRequestStatus.WaitingForCertify;
                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;


                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            // تم اضافة طلب جديد بالانضمام كمحكم

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_JoiningRequest).Result.Data?.Value;
                                msg = msg?.Replace("{CitizenName}", citizenInfo.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
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
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator Joining Request Added",
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
                            catch (Exception ex)
                            {

                            }


                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            // تم اضافة طلب جديد بالانضمام كمصلح


                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_JoiningRequest).Result.Data?.Value;
                                msg = msg?.Replace("{CitizenName}", citizenInfo.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer Joining Request Added",
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
                            catch (Exception ex)
                            {

                            }


                            break;
                    }
                    break;
                case CitizenRequestAction.SendFromEdit:
                    citizenRequests.StatusId = CitizenRequestStatus.SentFromEdit;
                    if (!string.IsNullOrWhiteSpace(notes))
                    {
                        CitizenRequestNote citizenRequestNote = new CitizenRequestNote();
                        citizenRequestNote.Id = Guid.NewGuid();
                        citizenRequestNote.CitizenRequestId = requestId;
                        citizenRequestNote.Notes = notes;
                        citizenRequestNote.NoteDateTime = DateTime.Now;
                        await _repository.AddAsync<CitizenRequestNote, Guid>(citizenRequestNote);
                    }
                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;
                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            // تم تعديل طلب بالانضمام كمحكم

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_JoiningRequest_Updated).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenInfo.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
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
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator Joining Request Updated",
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
                            catch (Exception ex)
                            {

                            }

                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            // تم تعديل طلب بالانضمام كمصلح

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_JoiningRequest_Updated).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenInfo.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer Joining Request Updated",
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
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                    break;
                case CitizenRequestAction.Reject:
                    citizenRequests.StatusId = CitizenRequestStatus.Rejected;
                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;
                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            // تم رفض طلب بالانضمام كمحكم

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_RejectJoiningRequest).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
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
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator Joining Request Updated",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            // تم رفض طلب بالانضمام كمصلح

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_RejectJoiningRequest).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
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
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer Joining Request Updated",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                    break;
                case CitizenRequestAction.CertifiedWithFees:
                    citizenRequests.StatusId = CitizenRequestStatus.WaitingForMembershipFeesToBePaid;
                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;
                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            // تم إعتماد طلب بالانضمام كمحكم مع وجود رسوم
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_AcceptJoiningRequest_WaitingForFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);

                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
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
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator AcceptJoiningRequest and WaitingForFees",
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
                            catch (Exception ex)
                            {

                            }


                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            // تم إعتماد طلب بالانضمام كمصلح مع وجود رسوم

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_AcceptJoiningRequest_WaitingForFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer AcceptJoiningRequest and WaitingForFees",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                    break;
                case CitizenRequestAction.CertifiedWithOutFees:
                    citizenRequests.StatusId = CitizenRequestStatus.Certified;
                    var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                         .FirstOrDefault(x => x.Id == citizenRequests.CitizenId);
                    if (citizen != null)
                    {
                        if (citizen is not null)
                        {
                            switch (citizenRequests.CitizenRequestTypeId)
                            {
                                case CitizenRequestType.Reformer:
                                    citizen.IsReformer = true;
                                    citizen.ReformerStatusId = ReformerStatus.Certified;
                                    break;
                                case CitizenRequestType.Arbitrator:
                                    citizen.IsArbitrator = true;
                                    citizen.ArbitratorStatusId = ArbitratorStatus.Certified;
                                    break;
                            }
                        }
                    }
                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;
                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            // تم إعتماد طلب بالانضمام كمحكم مع عدم وجود رسوم

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_AcceptJoiningRequest_NoFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
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
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator AcceptJoiningRequest and No Fees",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            // تم إعتماد طلب بالانضمام كمصلح مع عدم وجود رسوم

                            try
                            {
                                var t = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_AcceptJoiningRequest_NoFees);
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_AcceptJoiningRequest_NoFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer AcceptJoiningRequest and No Fees",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoReconciliation.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                    break;
                case CitizenRequestAction.SendForEdit:
                    citizenRequests.StatusId = CitizenRequestStatus.SentForEdit;
                    if (!string.IsNullOrWhiteSpace(notes))
                    {
                        CitizenRequestNote newCitizenRequestNote = new CitizenRequestNote();
                        newCitizenRequestNote.Id = Guid.NewGuid();
                        newCitizenRequestNote.CitizenRequestId = requestId;
                        newCitizenRequestNote.Notes = notes;
                        newCitizenRequestNote.NoteDateTime = DateTime.Now;
                        await _repository.AddAsync<CitizenRequestNote, Guid>(newCitizenRequestNote);
                    }

                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;
                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            // تم مراجعة طلب بالانضمام كمحكم مع وجود واعادته للتعديل لوجود ملاحظات

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_UpdateJoiningRequest_WithFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
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
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator Update Joining Request WithFees",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            // تم مراجعة طلب بالانضمام كمصلح مع وجود واعادته للتعديل لوجود ملاحظات

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_UpdateJoiningRequest_WithFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer Update JoiningRequest and No Fees",
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
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                    break;

                case CitizenRequestAction.SendToPayFees:
                    citizenRequests.StatusId = CitizenRequestStatus.SendToPayFees;
                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;

                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            // تم ارسال الطلب بالانضمام كمحكم لدفع الرسوم 

                            try
                            {

                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_UpdateJoiningRequest_ToPayFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Employee_Arbitrator_UpdateJoiningRequest_ToPayFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator Update Joining Request To Pay Fees",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            // تم ارسال الطلب بالانضمام كمصلح لدفع الرسوم 
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_UpdateJoiningRequest_ToPayFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Employee_Reformer_UpdateJoiningRequest_ToPayFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer Update JoiningRequest To Pay Fees",
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
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                    await _repository.SaveChangesAsync();
                    break;

                case CitizenRequestAction.FeesPaid:
                    citizenRequests.IsFeesPaid = true;
                    citizenRequests.StatusId = CitizenRequestStatus.FeesPaid;
                    saveResponse = await _repository.SaveChangesAsync();
                    if (!saveResponse.IsDone) return saveResponse;

                    switch (citizenRequests.CitizenRequestTypeId)
                    {
                        case CitizenRequestType.Arbitrator:
                            // Send Notification
                            //تم دفع رسوم طلب بالانضمام كمحكم 

                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Arbitrator_UpdateJoiningRequest_PaidFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenRequests.Citizen.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Arbitrator,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Employee_Arbitrator_UpdateJoiningRequest_PaidFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsArbitratorRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenRequests.Citizen.Email, deptAdminInfoArbitration.Data.Email },
                                        Subject = "Arbitrator  Update JoiningRequest and Fees Paid",
                                        Body = msg
                                    },
                                    new SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = new List<string>() { citizenRequests.Citizen.Mobile, deptAdminInfoArbitration.Data.Mobile },
                                        notification_type = "SMS",

                                    },
                                    applicationNotifications
                                    );
                            }
                            catch (Exception ex)
                            {

                            }

                            break;
                        case CitizenRequestType.Reformer:
                            // Send Notification
                            //تم دفع رسوم طلب بالانضمام كمصلح 
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reformer_UpdateJoiningRequest_PaidFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = citizenInfo.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reformer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Employee_Reformer_UpdateJoiningRequest_PaidFees).Result.Data?.Value;
                                msg = msg.Replace("{CitizenName}", citizenRequests.Citizen.FullName);
                                applicationNotifications.Add(new SysNotificationDto()
                                {
                                    ActorId = deptAdminInfoArbitration.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = citizenRequests.Id,
                                    RequestTypeId = RequestType.JoinRequestAsReformerRequest,
                                    Message = msg
                                });

                                var notificationResultUpdatedArbitrator = _notificationService.SendNotifications(
                                    new SendEmailDto()
                                    {
                                        To = new List<string>() { citizenInfo.Email, deptAdminInfoReconciliation.Data.Email },
                                        Subject = "Reformer Update JoiningRequest and Fees Paid",
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
                            catch (Exception ex)
                            {

                            }

                            break;
                    }
                    await _repository.SaveChangesAsync();
                    break;
            }
            return saveResponse;
        }
    }
}
