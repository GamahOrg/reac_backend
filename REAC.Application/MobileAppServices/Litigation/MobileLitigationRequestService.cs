using AutoMapper;using Core.GenericRepository.BaseRepository;using Core.GenericRepository.Shared.Pagging;using Core.Log.Application.Contracts.IAppServices;using Core.Shared.Responses;using Microsoft.EntityFrameworkCore;using Microsoft.Extensions.DependencyInjection;using REAC.Application.Contracts.Dtos.Integration.GEE;using REAC.Application.Contracts.Dtos.Notification;using REAC.Application.Contracts.Dtos.Reconciliation.Process;using REAC.Application.Contracts.Dtos.Wpf.Business.WorkFlowLog;using REAC.Application.Contracts.IAppServices;using REAC.Application.Contracts.IAppServices.Notification;using REAC.Application.Contracts.IAppServices.System;using REAC.Application.Contracts.IMobileAppServices.Litigation;using REAC.Application.Contracts.IModuleServices;using REAC.Application.Contracts.MobileDtos.Litigation.List;using REAC.Application.Contracts.MobileDtos.Litigation.ShortDto;using REAC.Application.Contracts.MobileDtos.Wpf;using REAC.Domain.Shared.Constants;using REAC.Domain.Shared.Enum;using REAC.EntityFrameworkCore.Entities.Litigation;using REAC.EntityFrameworkCore.Entities.System;using REAC.EntityFrameworkCore.Entities.Wpf.Business;namespace REAC.Application.MobileAppServices.Litigation{    public partial class MobileLitigationRequestService : BasicService<LitigationRequest, Guid>, IMobileLitigationRequestService    {        private readonly REACDBContext _context;
        private readonly IJwtReader _jwtReader;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ISysSettingService _sysSettingService;        public MobileLitigationRequestService(REACDBContext context,
              IEntityRepository repository,            IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysEmployeeService employeeService, INotificationService notificationService, ISysSettingService sysSettingService, IServiceProvider serviceProvider)
            : base(repository, mapper, loggerManager, jwtReader)        {            _context = context;            _jwtReader = jwtReader;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
            _serviceProvider = serviceProvider;
        }


        public async Task<BaseResponse> List(int pageIndex, int pageSize, string? searchKey, List<int> statuses, List<int> types)        {
            try
            {

                PagedResult<LitigationRequest> litigationRequests = null;
                var litigations = await GetQueryableAsync();
                litigations = litigations.Include(x => x.Contract);
                litigations = litigations.Include(x => x.ApplicantCitizen);
                litigations = litigations.Include(x => x.Contract.LitigationRequestContractType);
                litigations = litigations.Include(x => x.Contract.FirstPartyCitizen);
                litigations = litigations.Include(x => x.Contract.SecondPartyCitizen);
                litigations = litigations.Include(x => x.Contract.FirstPartyFacility);
                litigations = litigations.Include(x => x.Contract.SecondPartyFacility);
                if (!string.IsNullOrEmpty(searchKey))
                {
                    litigations = litigations.Where(l => l.RequestCode.Contains(searchKey) || l.Contract.FirstPartyCitizen.FullName.Contains(searchKey) || l.Contract.SecondPartyCitizen.FullName.Contains(searchKey));
                }
                if (statuses != null & statuses.Count() > 0)
                {
                    litigations = litigations.Where(s => statuses.Contains(((int)s.ExternalStatusId)));
                }
                if (types != null & types.Count() > 0)
                {
                    litigations = litigations.Where(s => types.Contains(((int)s.Contract.LitigationRequestContractTypeId)));
                }



                Guid citizenId = Guid.Empty;
                _jwtReader.GetReferenceId(ref citizenId);

                litigationRequests = litigations.Where(x => x.Contract.FirstPartyCitizenId == citizenId
                                                   | x.Contract.SecondPartyCitizenId == citizenId
                                                   | x.ApplicantCitizenId == citizenId)
                                                   .OrderByDescending(x => x.CreatedOn)
                                                   .AsSplitQuery()
                                                   .AsNoTracking()
                                                   .GetPaged(pageIndex, pageSize);


                var list = _mapper.Map<List<LitigationListMobileDto>>(litigationRequests.Results);

                return new DataResponse<PagedResult<LitigationListMobileDto>>
                {
                    Data =
                      new PagedResult<LitigationListMobileDto>
                      {
                          Results = list,
                          CurrentPage = litigationRequests.CurrentPage,
                          PageCount = litigationRequests.PageCount,
                          PageSize = litigationRequests.PageSize,
                          TotalCount = litigationRequests.TotalCount
                      },
                    IsDone = true
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = ex.Message };

            }



        }

        public async Task<BaseResponse> GetById(Guid litigationId)        {
            //var quarble = await GetQueryableAsync();
            var result = (await _repository.GetQueryable<LitigationRequest, Guid>())
.Include(x => x.SysExemptionReductionCategory)
.Include(x => x.ExemptionReductionFile)
.Include(x => x.LitigationRequestAmount)
.Include(x => x.LitigationRequestExtendPeriodRequests)
.Include(x => x.LitigationRequestSessions)
.Include(x => x.Contract)
.Include(x => x.LitigationRequestCategory)
.Include(x => x.Area)
.Include(x => x.City)
.Include(x => x.ApplicantCitizen)
.Include(x => x.Contract.SecondPartyCitizen)
.Include(x => x.Contract.SecondPartyCitizen.Nationality)
.Include(x => x.Contract.FirstPartyCitizen)
.Include(x => x.Contract.FirstPartyCitizen.Nationality)
.Include(x => x.Contract.FirstPartyFacility)
.Include(x => x.Contract.SecondPartyFacility)
.Include(x => x.Contract.ContractFile)
.Include(x => x.Contract.AssociationRegistrationCertificateFile)
.Include(x => x.Contract.FoundingLetterFile)
.Include(x => x.Contract.InternalRegulationsFile)

.Include(x => x.LitigationRequestGrounds)
.ThenInclude(x => x.LitigationRequestGroundFiles)
.ThenInclude(x => x.FileManager)

.Include(x => x.LitigationRequestRequests)
.FirstOrDefault(x => x.Id == litigationId);

            //var request = _mapper.Map<LitigationRequestShortMobileDto>(result);

            return new DataResponse<LitigationRequest> { Data = result, IsDone = true };        }

        public async Task<BaseResponse> GetLitigationRequestEvents(Guid litigationId)
        {
            try
            {
                List<WorkFlowLog>? workFlowLog = null;

                workFlowLog = (await _repository.GetQueryable<WorkFlowLog, Guid>())?
                .Where(x => x.RequestId == litigationId)
                .Include(x => x.Translations)
                .OrderBy(x => x.CreatedOn)
                .ToList();

                var result = _mapper.Map<List<WorkFlowLogMobileDto>>(workFlowLog);

                return new DataResponse<List<WorkFlowLogMobileDto>>()
                {
                    IsDone = true,
                    Data = result
                };
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }

        #region Agreement Request
        public async Task<BaseResponse> GetCitizenLitigationRequestForAgreementRequestReply(int pageIndex, int pageSize)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;


            litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
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
                                      .Where(
                                        x => x.RequestCode != null & x.ApplicantCitizenId == referenceId & x.IsAgreementRequestRequired == true
                                      | x.RequestCode != null & x.Contract.FirstPartyCitizenId == referenceId & x.IsAgreementRequestRequired == true
                                      | x.RequestCode != null & x.Contract.SecondPartyCitizenId == referenceId & x.IsAgreementRequestRequired == true
                                      | x.RequestCode != null & x.Contract.FirstPartyFacility.CitizenId == referenceId & x.IsAgreementRequestRequired == true
                                      | x.RequestCode != null & x.Contract.SecondPartyFacility.CitizenId == referenceId & x.IsAgreementRequestRequired == true)
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
                        if (litigationRequest?.LitigationRequestSubStage != LitigationRequestSubStage.LitigationRequestAgreementRequestRequired)
                            allowAdd = false;
                        else
                        {

                            if (litigationRequest.ApplicantCitizenId == referenceId)
                            {
                                if (litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                                    allowAdd = false;

                            }
                            if (litigationRequest.Contract.FirstPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyCitizenId == referenceId)
                                {
                                    if (litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                                        allowAdd = false;
                                }
                            }
                            if (litigationRequest.Contract.SecondPartyCitizenId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyCitizenId == referenceId)
                                {
                                    if (litigationRequest.DefendantAgreementRequestResponse.HasValue)
                                        allowAdd = false;
                                }
                            }
                            if (litigationRequest.Contract.FirstPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.FirstPartyFacility?.CitizenId == referenceId)
                                {
                                    if (litigationRequest.PlaintiffAgreementRequestResponse.HasValue)
                                        allowAdd = false;
                                }
                            }
                            if (litigationRequest.Contract.SecondPartyFacilityId.HasValue)
                            {
                                if (litigationRequest.Contract.SecondPartyFacility?.CitizenId == referenceId)
                                {
                                    if (litigationRequest.DefendantAgreementRequestResponse.HasValue)
                                        allowAdd = false;
                                }
                            }
                        }
                    }

                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    //  litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDto.Status = litigationRequest.InternalStatusId.GetHashCode().ToString();
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
        public async Task<BaseResponse> GetCitizenLitigationRequestForAgreementRequestReply(Guid requestId)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

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
                             .Include(x => x.Contract.FirstPartyCitizen)
                             .Include(x => x.Contract.SecondPartyFacility)
                             .Include(x => x.Contract.SecondPartyCitizen)
                             .Include(x => x.Contract.LitigationRequestContractType)

                             .Where(x => x.Id == requestId).ToList().FirstOrDefault();

                if (litigationRequest is null)
                    return new BaseResponse() { IsDone = false, Message = "دعوي غير موجوده" };

                var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);



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
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false , Message = ex.ToString() };
            }

        }
        public async Task<BaseResponse> AddCitizenLitigationRequestForAgreementRequestReply(Guid requestId, ApproveRejectStatus agreementRequestStatus)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
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
            if (litigationRequest != null)
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

                        SendNotificationMSWithDefendant(msg, litigationRequest, RequestType.LitigationAgreementRequest, "رفض طلب المشارطة");
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

            return await GetCitizenLitigationRequestForAgreementRequestReply(requestId);

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

            _notificationService.SendNotifications(objEmail, objSms, lstNotification);

        }
        #endregion
    }
}

