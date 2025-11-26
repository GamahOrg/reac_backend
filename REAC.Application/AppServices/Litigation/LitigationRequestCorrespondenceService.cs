using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.IdentityModel.Tokens.Jwt;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationRequestCorrespondenceService : BasicService<LitigationRequestCorrespondence, Guid>, ILitigationRequestCorrespondenceService
    {
        private readonly IJwtReader _jwtReader;
        private readonly ISysEmployeeService _employeeService;
        protected ISysSettingService _sysSettingService;
        private readonly INotificationService _notificationService;
        private readonly REACDBContext _dbContext;

        public LitigationRequestCorrespondenceService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, INotificationService notificationService, ISysEmployeeService employeeService, ISysSettingService sysSettingService, REACDBContext dbContext) : base(repository, mapper, loggerManager, jwtReader)
        {
            _jwtReader = jwtReader;
            _notificationService = notificationService;
            _employeeService = employeeService;
            _sysSettingService = sysSettingService;
            _dbContext = dbContext;
        }

        #region For Reformer / Arbitrator

        public async Task<BaseResponse> AddLitigationCorrespondence(LitigationRequestCorrespondenceCreateUpdateDto litigationCorrespondence)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);


                if (litigationCorrespondence == null || referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                                .Include(x => x.Contract)
                                .ThenInclude(x => x.FirstPartyCitizen)
                                .Include(x => x.Contract)
                                 .ThenInclude(x => x.SecondPartyCitizen)
                                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.ReformerCitizen)//المصلح مواطن
                                .Include(x => x.ApplicantCitizen)
                                .AsNoTracking()
                                .FirstOrDefault(x => x.Id == litigationCorrespondence.LitigationRequestId);

                var correspondanceObject = new LitigationRequestCorrespondence()
                {
                    Id = new Guid(),
                    CreatedBy = referenceId,
                    CreatedOn = DateTime.Now,
                    SerialCode = await GenerateCorrespondenceCode(litigationCorrespondence.LitigationRequestId),
                    LitigationRequestId = litigationCorrespondence.LitigationRequestId,
                    Subject = litigationCorrespondence.Subject,
                    Message = litigationCorrespondence.Message,
                    LitigationRequestCategoryId = litigationRequest.LitigationRequestStage == LitigationRequestStage.Reconciliation ? LitigationRequestCategory.Reconciliation
                    : litigationRequest.LitigationRequestStage == LitigationRequestStage.Arbitration ? LitigationRequestCategory.Arbitration : litigationCorrespondence.LitigationRequestCategoryId,
                    LitigationRequestCorrespondenceStatusId = CorrespondenceStatus.NotRepliedOn,
                    LitigationRequestCorrespondenceDestinationId = (LitigationRequestCorrespondenceDestination)litigationCorrespondence.LitigationRequestCorrespondenceDestinationId,
                };

                var result = await _repository.AddAsync<LitigationRequestCorrespondence, Guid>(correspondanceObject);

                if (result.IsDone && litigationCorrespondence.CorrespondenceFileId != Guid.Empty && litigationCorrespondence.CorrespondenceFileId != null)
                {
                    var fileAddResult = await _repository.AddAsync<LitigationRequestCorrespondenceFile, Guid>(new LitigationRequestCorrespondenceFile()
                    {
                        Id = new Guid(),
                        CreatedBy = referenceId != Guid.Empty ? referenceId : null,
                        CreatedOn = DateTime.Now,
                        LitigationRequestCorrespondenceId = correspondanceObject.Id,
                        FileManagerId = (Guid)litigationCorrespondence.CorrespondenceFileId
                    });

                    if (!fileAddResult.IsDone)
                        return fileAddResult;

                    var logAddResult = await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                    {
                        Id = Guid.NewGuid(),
                        CreatedOn = DateTime.Now,
                        RequestId = litigationCorrespondence.LitigationRequestId,
                        ShowForApplicant = true,
                        LogMessage = $"تم اضافة مراسلة رقم {correspondanceObject.SerialCode} علي الدعوي"
                    });

                    if (!logAddResult.IsDone)
                        return fileAddResult;
                }
                ;

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notifications

                if (litigationRequest != null)
                {
                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequestCorrespondenceAdded).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    msg = msg?.Replace("{CorrespondenceCode}", correspondanceObject?.SerialCode);
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body;
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequestCorrespondenceAdded).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                        msg = msg?.Replace("{CorrespondenceCode}", correspondanceObject?.SerialCode);
                    }
                    await SendNotificationMS(body, msg, litigationRequest, "Message Added", NotificationActorType.Applicant, RequestType.LitigationRequestCorrespondence, null, true, false, false, true, true);
                }



                var response = await GetCorrespondencesByLitigationRequestId(litigationCorrespondence.LitigationRequestId, (int)litigationCorrespondence.LitigationRequestCategoryId);

                return new DataResponse<List<LitigationRequestCorrespondenceDto>>() { IsDone = true, Message = "تم الاضافه بنجاح", Data = response.Data };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }

        public async Task<BaseResponse> GetLitigationCorrespondences(Guid litigationRequestId, int[] litigationRequestCategoryIds)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.LitigationRequestCorrespondences.Where(l => litigationRequestCategoryIds.Contains((int)l.LitigationRequestCategoryId)).OrderByDescending(l => l.CreatedOn))
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestCorrespondences.Where(l => litigationRequestCategoryIds.Contains((int)l.LitigationRequestCategoryId)).OrderByDescending(l => l.CreatedOn))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceStatus)
                          .Include(x => x.LitigationRequestCorrespondences.Where(l => litigationRequestCategoryIds.Contains((int)l.LitigationRequestCategoryId)).OrderByDescending(l => l.CreatedOn))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.LitigationRequestCorrespondences.Where(l => litigationRequestCategoryIds.Contains((int)l.LitigationRequestCategoryId)).OrderByDescending(l => l.CreatedOn))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReply)
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReplyFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.LitigationRequestCorrespondences.Where(l => litigationRequestCategoryIds.Contains((int)l.LitigationRequestCategoryId)).OrderByDescending(l => l.CreatedOn))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReply)
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.Area)
                          .Include(x => x.City)
                          .Include(x => x.LitigationRequestReconciliationReports)
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
                          .Include(x => x.Contract.ContractFile)
                          .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                          .Include(x => x.Contract.FoundingLetterFile)
                          .Include(x => x.Contract.InternalRegulationsFile)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.Contract.LitigationRequestContractType)
                          .AsNoTracking()
                          .FirstOrDefault(x => x.Id == litigationRequestId);




                if (litigationRequest == null)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var allowAdd = true;

                var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

                var correspondences = _mapper.Map<List<LitigationRequestCorrespondenceDto>>(litigationRequest?.LitigationRequestCorrespondences);

                return new DataResponse<LitigationRequestForCorrespondencesDto>()
                {
                    Data = new LitigationRequestForCorrespondencesDto()
                    {
                        LitigationRequestShortDto = request,
                        AllowAdd = allowAdd,
                        Correspondences = (correspondences.Count > 0) ? correspondences : null,
                    },
                    IsDone = true,
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestForCorrespondencesDto>>() { IsDone = false, Message = "خطأ داخلي" };

            }
        }

        private async Task<DataResponse<List<LitigationRequestCorrespondenceDto>>> GetCorrespondencesByLitigationRequestId(Guid litigationRequestId, int litigationRequestCategoryId)
        {
            try
            {
                var result = (await _repository.GetQueryable<LitigationRequestCorrespondence, Guid>())
                    .Include(x => x.Citizen)
                    .Include(x => x.LitigationRequestCorrespondenceFiles)
                    .ThenInclude(x => x.FileManager)
                    .Include(x => x.LitigationRequestCorrespondenceStatus)
                    .Include(x => x.LitigationRequestCorrespondenceReply)
                    .ThenInclude(x => x.LitigationRequestCorrespondenceReplyFiles)
                    .ThenInclude(x => x.FileManager)
                    .Include(x => x.LitigationRequestCorrespondenceReply)
                    .ThenInclude(x => x.Citizen)
                    .Where(x => x.LitigationRequestId == litigationRequestId && (int)x.LitigationRequestCategoryId == litigationRequestCategoryId)
                    .OrderByDescending(x => x.CreatedOn)
                    .AsNoTracking()
                    .ToList();


                if (result is null)
                    return new DataResponse<List<LitigationRequestCorrespondenceDto>>() { IsDone = false, Message = " المراسلة غير موجودة" };

                var data = _mapper.Map<List<LitigationRequestCorrespondenceDto>>(result);

                return new DataResponse<List<LitigationRequestCorrespondenceDto>>()
                {
                    IsDone = true,
                    Message = "",
                    Data = data

                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestCorrespondenceDto>>() { IsDone = false, Message = "خطأ داخلي" };
            }


        }

        #endregion


        #region Correspondences Reply For Defendant/plaintiff

        public async Task<BaseResponse> GetLitigationRequestsForCitizinCorrespondenceReply(int pageIndex, int pageSize, string? searchKey, LitigationListDto model)
        {
            PagedResult<LitigationRequest> litigationRequests = null;

            var litigations = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.DefendantCitizen)
                    .Include(x => x.DefendantFacility)
                    .Include(x => x.Contract)
                    .Include(x => x.Contract.LitigationRequestContractType)
                    .Include(x => x.Contract.FirstPartyCitizen)
                    .Include(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.Contract.FirstPartyFacility)
                    .Include(x => x.Contract.SecondPartyFacility)
                    .Include(x => x.LitigationRequestCorrespondences)
                    .Where(x => x.LitigationRequestCorrespondences.Any())
                    .AsNoTracking();

            if (!string.IsNullOrEmpty(searchKey))
            {
                litigations = litigations.Where(l => l.RequestCode.Contains(searchKey) || l.Contract.SecondPartyName.Contains(searchKey));
            }

            if (model.InternalStatusId.HasValue)
            {
                litigations = litigations.Where(l => l.InternalStatusId == model.InternalStatusId.Value);
            }

            if (model.LitigationRequestContractTypeId.HasValue)
            {
                litigations = litigations.Where(l => l.Contract.LitigationRequestContractTypeId == model.LitigationRequestContractTypeId.Value);
            }

            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);

            litigationRequests = litigations.Where(x =>
                                               // x.Contract.FirstPartyCitizenId == citizenId & x.LitigationRequestStage == LitigationRequestStage.Reconciliation & x.LitigationRequestCorrespondences.Any(l => l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.Plaintiff | l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                                               //| 
                                               x.Contract.SecondPartyCitizenId == citizenId & x.LitigationRequestStage == LitigationRequestStage.Reconciliation
                                               | x.ApplicantCitizenId == citizenId & x.LitigationRequestStage == LitigationRequestStage.Reconciliation 

                                               //|x.Contract.FirstPartyCitizenId == citizenId & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                               | x.Contract.SecondPartyCitizenId == citizenId & x.LitigationRequestStage == LitigationRequestStage.Arbitration
                                               | x.ApplicantCitizenId == citizenId & x.LitigationRequestStage == LitigationRequestStage.Arbitration

                                               )
                                               .OrderByDescending(x => x.CreatedOn)
                                               .GetPaged(pageIndex, pageSize);


            var list = _mapper.Map<List<LitigationListDto>>(litigationRequests.Results);

            return new DataResponse<PagedResult<LitigationListDto>>
            {
                Data =
                  new PagedResult<LitigationListDto>
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

        public async Task<BaseResponse> GetLitigationCorrespondencesByLitigationRequestId(Guid litigationRequestId)
        {
            try
            {

                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var destination = GetCorrespondenceDestination(litigationRequestId, referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.LitigationRequestCorrespondences
                          .Where(l =>
                          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation )
                          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                          ))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReply)
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReplyFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.LitigationRequestCorrespondences
                          .Where(l =>
                          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation )
                          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                          ))
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestCorrespondences
                          .Where(l =>
                          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation)
                          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                          ))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.Area)
                          .Include(x => x.City)
                          .Include(x => x.LitigationRequestReconciliationReports)
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
                          .Include(x => x.Contract.ContractFile)
                          .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                          .Include(x => x.Contract.FoundingLetterFile)
                          .Include(x => x.Contract.InternalRegulationsFile)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.Contract.LitigationRequestContractType)
                          .FirstOrDefault(x => x.Id == litigationRequestId);


                if (litigationRequest == null)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var allowAdd = true;



                var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

                var correspondences = _mapper.Map<List<LitigationRequestCorrespondenceDto>>(litigationRequest?.LitigationRequestCorrespondences);

                foreach (var correspondence in correspondences)
                {
                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.Plaintiff & (litigationRequest?.ApplicantCitizenId == referenceId || litigationRequest?.Contract?.FirstPartyFacilityId == referenceId|| litigationRequest?.ApplicantCitizenId == referenceId))
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.Defendant & (litigationRequest?.Contract?.SecondPartyCitizenId == referenceId || litigationRequest?.Contract?.SecondPartyFacilityId == referenceId ))
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.Plaintiff & (litigationRequest?.Contract?.FirstPartyCitizenId == referenceId || litigationRequest?.Contract?.FirstPartyFacilityId == referenceId|| litigationRequest?.ApplicantCitizenId == referenceId))
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.Defendant & (litigationRequest?.Contract?.SecondPartyCitizenId == referenceId || litigationRequest?.Contract?.SecondPartyCitizenId == referenceId))
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                        correspondence.AllowAddReply = true;
                }

                // Only proceed if the list isn't null or empty
                if (correspondences != null && correspondences.Count > 0)
                {
                    // 1. Sort the PARENT list (correspondences)
                    var sortedCorrespondences = correspondences
                        .OrderByDescending(s => s.CreatedOn)
                        .ToList();

                    // 2. Now, loop through each parent and sort its CHILD list
                    foreach (var correspondence in sortedCorrespondences)
                    {
                        // Check if the child list exists before trying to sort it
                        if (correspondence.LitigationRequestCorrespondenceReply != null)
                        {
                            // Re-assign the child list with its newly sorted version
                            correspondence.LitigationRequestCorrespondenceReply = correspondence.LitigationRequestCorrespondenceReply
                                .OrderByDescending(r => r.CreatedOn)
                                .ToList();
                        }
                    }

                    // 3. Assign the fully sorted list to your final variable
                    correspondences = sortedCorrespondences;
                }
                else
                {
                    correspondences = null; // Or 'new List<Correspondence>()'
                }


                return new DataResponse<LitigationRequestForCorrespondencesDto>()
                {
                    Data = new LitigationRequestForCorrespondencesDto()
                    {
                        LitigationRequestShortDto = request,
                        AllowAdd = allowAdd,
                        //Correspondences = (correspondences.Count > 0) ? correspondences?.OrderByDescending(s => s.CreatedOn).ThenByDescending(s => s.LitigationRequestCorrespondenceReply.OrderByDescending(r => r.CreatedOn)). ToList() : null,
                    Correspondences = correspondences
                    },
                    IsDone = true,
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestForCorrespondencesDto>>() { IsDone = false, Message = "خطأ داخلي" };

            }
        }

        public async Task<BaseResponse> AddLitigationCorrespondenceReply(LitigationRequestCorrespondenceReplyCreateUpdateDto litigationCorrespondenceReply)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                                .Include(x => x.LitigationRequestCorrespondences.Where(l => l.Id == litigationCorrespondenceReply.LitigationRequestCorrespondenceId))
                                .Include(x => x.Contract)
                                .ThenInclude(x => x.FirstPartyCitizen)
                                .Include(x => x.Contract)
                                 .ThenInclude(x => x.SecondPartyCitizen)
                                .Include(x => x.Contract).ThenInclude(x => x.FirstPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.Contract).ThenInclude(x => x.SecondPartyFacility).ThenInclude(x => x.Citizen)
                                .Include(x => x.ReformerCitizen)//المصلح مواطن
                                .Include(x => x.ApplicantCitizen)
                                .Include(x => x.ArbitratorCitizen)
                                .AsNoTracking()
                                .FirstOrDefault(x => x.LitigationRequestCorrespondences.Any(l => l.Id == litigationCorrespondenceReply.LitigationRequestCorrespondenceId));


                if (litigationCorrespondenceReply == null || referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var correspondanceReplyObject = new LitigationRequestCorrespondenceReply()
                {
                    Id = new Guid(),
                    CreatedBy = referenceId,
                    CreatedOn = DateTime.Now,
                    ReplyMessage = litigationCorrespondenceReply.ReplyMessage,
                    LitigationRequestCorrespondenceId = litigationCorrespondenceReply.LitigationRequestCorrespondenceId,
                };

                var result = await _repository.AddAsync<LitigationRequestCorrespondenceReply, Guid>(correspondanceReplyObject);

                if (result.IsDone && litigationCorrespondenceReply.CorrespondenceReplyFileId != Guid.Empty && litigationCorrespondenceReply.CorrespondenceReplyFileId != null)
                {
                    var fileAddResult = await _repository.AddAsync<LitigationRequestCorrespondenceReplyFile, Guid>(new LitigationRequestCorrespondenceReplyFile()
                    {
                        Id = new Guid(),
                        CreatedBy = referenceId != Guid.Empty ? referenceId : null,
                        CreatedOn = DateTime.Now,
                        LitigationRequestCorrespondenceReplyId = correspondanceReplyObject.Id,
                        FileManagerId = (Guid)litigationCorrespondenceReply.CorrespondenceReplyFileId
                    });

                    if (litigationRequest is not null)
                    {
                        var logAddResult = await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = true,
                            LogMessage = $"تم اضافة رد للمراسلة رقم {litigationRequest.LitigationRequestCorrespondences.First().SerialCode} علي الدعوي"
                        });

                        if (!logAddResult.IsDone)
                            return fileAddResult;
                    }

                }
                ;

                var updateCorrespondenceResult = _dbContext.Set<LitigationRequestCorrespondence>()
                    .Where(x => x.Id == litigationCorrespondenceReply.LitigationRequestCorrespondenceId)
                    .ExecuteUpdate(setters => setters.SetProperty(b => b.LitigationRequestCorrespondenceStatusId, CorrespondenceStatus.RepliedOn));

                if (updateCorrespondenceResult != 1)
                    return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };


                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                // Send Notifications
                if (litigationRequest != null)
                {
                    var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequestCorrespondenceReplyAdded).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    msg = msg?.Replace("{CorrespondenceCode}", litigationRequest.LitigationRequestCorrespondences.First()?.SerialCode);
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    string body;
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequestCorrespondenceReplyAdded).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                        body = body?.Replace("{CorrespondenceCode}", litigationRequest.LitigationRequestCorrespondences.First()?.SerialCode);
                    }

                    if (litigationRequest.LitigationRequestStage == LitigationRequestStage.Reconciliation)
                    {

                        await SendNotificationMS(body, msg, litigationRequest, "Message Reply Added", NotificationActorType.Reformer, RequestType.LitigationRequestCorrespondence, null, true, false, false, true, true);
                    }
                    else
                    {
                        await SendNotificationMS(body, msg, litigationRequest, "Message Reply Added", NotificationActorType.Arbitrator, RequestType.LitigationRequestCorrespondence, null, true, true, false, true, true);

                    }
                    var saveResponse2 = await _repository.SaveChangesAsync();
                    if (!saveResponse2.IsDone)
                        return saveResponse2;
                }


                var response = await GetCorrespondenceRepliesByCorrespondenceId(litigationCorrespondenceReply.LitigationRequestCorrespondenceId);

                return new DataResponse<List<LitigationRequestCorrespondenceDto>>() { IsDone = true, Message = "تم الاضافه بنجاح", Data = response.Data };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }
        }


        private async Task<DataResponse<List<LitigationRequestCorrespondenceDto>>> GetCorrespondenceRepliesByCorrespondenceId(Guid correspondenceId)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequestId = (await _repository.GetQueryable<LitigationRequestCorrespondence, Guid>())
                    .AsNoTracking()
                    .First(x => x.Id == correspondenceId).LitigationRequestId;

                var destination = GetCorrespondenceDestination(litigationRequestId, referenceId);


                //var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                //          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                //          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                //          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                //          ))
                //          .ThenInclude(x => x.LitigationRequestCorrespondenceReply)
                //          .ThenInclude(x => x.LitigationRequestCorrespondenceReplyFiles)
                //          .ThenInclude(x => x.FileManager)
                //          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                //          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                //          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                //          ))
                //          .ThenInclude(x => x.LitigationRequestCorrespondenceReply)
                //          .ThenInclude(x => x.Citizen)
                //          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                //          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                //          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                //          ))
                //          .ThenInclude(x => x.Citizen)
                //          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                //          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                //          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                //          ))
                //          .ThenInclude(x => x.LitigationRequestCorrespondenceFiles)
                //          .ThenInclude(x => x.FileManager)
                //          .Include(x => x.Contract.FirstPartyFacility)
                //          .Include(x => x.Contract.FirstPartyCitizen)
                //          .Include(x => x.Contract.SecondPartyFacility)
                //          .Include(x => x.Contract.SecondPartyCitizen)
                //          .AsNoTracking()
                //          .FirstOrDefault(x => x.Id == litigationRequestId);
                
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                          ))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReply)
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReplyFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                          ))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceReply)
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                          ))
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestCorrespondences.Where(l =>
                          (l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation && l.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem || l.LitigationRequestCorrespondenceDestinationId == (LitigationRequestCorrespondenceDestination)destination.Result)
                          || (l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration)
                          ))
                          .ThenInclude(x => x.LitigationRequestCorrespondenceFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .AsNoTracking()
                          .FirstOrDefault(x => x.Id == litigationRequestId);

                if (litigationRequest is null)
                    return new DataResponse<List<LitigationRequestCorrespondenceDto>>() { IsDone = false, Message = " غير موجود" };

                var correspondences = _mapper.Map<List<LitigationRequestCorrespondenceDto>>(litigationRequest?.LitigationRequestCorrespondences);

                foreach (var correspondence in correspondences)
                {
                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.Plaintiff & (litigationRequest?.ApplicantCitizenId == referenceId || litigationRequest?.Contract?.FirstPartyFacilityId == referenceId))
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.Defendant & (litigationRequest?.Contract?.SecondPartyCitizenId == referenceId || litigationRequest?.Contract?.SecondPartyFacilityId == referenceId))
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration & correspondence.LitigationRequestCorrespondenceDestinationId == LitigationRequestCorrespondenceDestination.BothOfThem)
                        correspondence.AllowAddReply = true;

                    if (correspondence.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation)
                        correspondence.AllowAddReply = true;
                }

                return new DataResponse<List<LitigationRequestCorrespondenceDto>>()
                {
                    IsDone = true,
                    Message = "",
                    Data = correspondences

                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestCorrespondenceDto>>() { IsDone = false, Message = "خطأ داخلي" };
            }


        }


        #endregion


        #region Backoffice
        public async Task<BaseResponse> GetLitigationRequestArbitrationCorrespondence(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequestArbitratorAssigningRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                         .Include(x => x.ArbitratorCitizen)
                         .Include(x => x.LitigationRequest)
                         .Include(x => x.LitigationRequest.DefendantCitizen)
                         .Include(x => x.LitigationRequest.DefendantFacility)
                         .Include(x => x.LitigationRequest.InternalStatus)
                         .Include(x => x.LitigationRequest.ExternalStatus)
                         .Include(x => x.LitigationRequest.LitigationRequestCategory)
                         .Include(x => x.LitigationRequest.Contract)
                         .Include(x => x.LitigationRequest.Contract.ContractFile)
                         .Include(x => x.LitigationRequest.Contract.AssociationRegistrationCertificateFile)
                         .Include(x => x.LitigationRequest.Contract.FoundingLetterFile)
                         .Include(x => x.LitigationRequest.Contract.InternalRegulationsFile)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.LitigationRequestContractType)
                         .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.LitigationRequestCorrespondences.OrderByDescending(x => x.CreatedOn))
                         .Where(x => x.LitigationRequest.LitigationRequestCorrespondences.Any() & x.LitigationRequest.LitigationRequestStage == LitigationRequestStage.Arbitration)
                          .OrderByDescending(x => x.LitigationRequest.CreatedOn)
                         .GroupBy(x => x.LitigationRequest.RequestCode)
                         .Select(x => x.First()).AsNoTracking();





            litigationRequests = entitiesQuerable
                                       .GetPaged(pageIndex, pageSize);



            List<CustomLitigationRequestMircoArbitratorAllDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoArbitratorAllDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    CustomLitigationRequestMircoArbitratorAllDto litigationRequestMircoDto = new CustomLitigationRequestMircoArbitratorAllDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestId = litigationRequest.LitigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.LitigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.LitigationRequest.RequestCode;
                    litigationRequestMircoDto.ArbitratorAssigningRequestType = litigationRequest.ArbitratorAssigningRequestTypeId;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.LitigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.LitigationRequest.DefendantCitizen == null) ? litigationRequest.LitigationRequest.DefendantFacility?.ArabicFacilityName : litigationRequest.LitigationRequest.DefendantCitizen?.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.LitigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.RequestTypeId = litigationRequest.LitigationRequest.Contract.LitigationRequestContractType.Id.GetHashCode();
                    litigationRequestMircoDto.Response = litigationRequest.Response;
                    litigationRequestMircoDto.ResponseOn = litigationRequest.ResponseOn;
                    litigationRequestMircoDto.ArbitratorName =
                        (litigationRequest.ArbitratorAssigningRequestTypeId ==
                        ArbitratorAssigningRequestType.ArbitratorsChairman) ? litigationRequest.ArbitratorCitizen?.FullName :
                        litigationRequest.ArbitratorCitizen?.FullName;

                    bool allowAdd = true;


                    if (litigationRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Approve |
                        litigationRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Cancelled |
                        litigationRequest.Response == LitigationRequestArbitratorAssigningRequestStatus.Reject)
                    {
                        allowAdd = false;
                    }


                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest.LitigationRequest.ExternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
                litigationRequestMircoDtos = litigationRequestMircoDtos
                            .OrderByDescending(x => x.RequestDate)
                            .ToList();
            }


            return new DataResponse<PagedResult<CustomLitigationRequestMircoArbitratorAllDto>>
            {
                Data =
                              new PagedResult<CustomLitigationRequestMircoArbitratorAllDto>
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

        public async Task<BaseResponse> GetLitigationRequestReconciliationCorrespondence(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequestReformerAssigningRequest> litigationRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())
                          .Include(x => x.ReformerCitizen)
                         .Include(x => x.LitigationRequest)

                         .Include(x => x.LitigationRequest.ApplicantCitizen)
                         .Include(x => x.LitigationRequest.DefendantCitizen)
                         .Include(x => x.LitigationRequest.DefendantFacility)
                         .Include(x => x.LitigationRequest.InternalStatus)
                         .Include(x => x.LitigationRequest.ExternalStatus)

                         .Include(x => x.LitigationRequest.LitigationRequestCategory)
                          .Include(x => x.LitigationRequest.Contract)
                          .Include(x => x.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.LitigationRequestContractType)
                         .Include(x => x.LitigationRequest)
                         .ThenInclude(x => x.LitigationRequestCorrespondences.OrderByDescending(x => x.CreatedBy))
                         .Where(x => x.LitigationRequest.LitigationRequestCorrespondences.Any()
                       & (x.LitigationRequest.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation
                       || x.LitigationRequest.LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration)
                       )
                          .OrderByDescending(x => x.LitigationRequest.CreatedOn)
                         .GroupBy(x => x.LitigationRequest.RequestCode)
                         .Select(x => x.First()).AsNoTracking();


            litigationRequests = entitiesQuerable
                                       .GetPaged(pageIndex, pageSize);


            List<CustomLitigationRequestMircoAllDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoAllDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    CustomLitigationRequestMircoAllDto litigationRequestMircoDto = new CustomLitigationRequestMircoAllDto();
                    litigationRequestMircoDto.Id = litigationRequest.LitigationRequest.Id;
                    litigationRequestMircoDto.RequestId = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.LitigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.LitigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.LitigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.LitigationRequest.DefendantCitizen == null) ? litigationRequest.LitigationRequest.DefendantFacility.ArabicFacilityName : litigationRequest.LitigationRequest.DefendantCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.LitigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.Response = litigationRequest.Response;
                    litigationRequestMircoDto.ReformerName = litigationRequest.ReformerCitizen?.FullName;
                    litigationRequestMircoDto.ResponseOn = litigationRequest.ResponseOn;

                    bool allowAdd = true;


                    if (litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Approve |
                        litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Cancelled |
                        litigationRequest.Response == LitigationRequestReformerAssigningRequestStatus.Reject)
                    {
                        allowAdd = false;
                    }


                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

                    litigationRequestMircoDto.Status = litigationRequest.LitigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }

                litigationRequestMircoDtos = litigationRequestMircoDtos
                        .OrderByDescending(x => x.RequestDate)
                        .ToList();
            }
            return new DataResponse<PagedResult<CustomLitigationRequestMircoAllDto>>
            {
                Data =
                              new PagedResult<CustomLitigationRequestMircoAllDto>
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

        #endregion

        #region Helpers
        private async Task<string> GenerateCorrespondenceCode(Guid litigationRequestId)
        {
            var correspondenceList = (await _repository.GetQueryable<LitigationRequestCorrespondence, Guid>())
                .Where(x => !string.IsNullOrEmpty(x.SerialCode) && x.LitigationRequestId == litigationRequestId)
                .ToList();


            string code = "MS-0001";

            if (correspondenceList is null)
                code = "MS-0001";
            else
                code = "MS-" + (correspondenceList.Count() + 1).ToString("D4");

            return code;
        }

        private async Task<int> GetCorrespondenceDestination(Guid litigationRequestId, Guid referenceId)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                .FirstOrDefault(x => x.Id == litigationRequestId);

            if (litigationRequest is null)
                return 0;

            var destination = litigationRequest.ApplicantCitizenId == referenceId | litigationRequest.Contract.FirstPartyFacilityId == referenceId ? 1
                : litigationRequest.Contract.SecondPartyCitizenId == referenceId | litigationRequest.Contract.SecondPartyFacilityId == referenceId ? 2 : 0;

            return destination;
        }
        #endregion

        #region Notifications
        private async Task SendNotificationMS(string mailMsg, string msg, LitigationRequest litigationRequest, string subject, NotificationActorType notificationActorType, RequestType requestType, LitigationRequestArbitratorList? arbitrator = null, bool notifyApplicant = false, bool notifyArbitrators = false, bool notifyDeptAdmin = false, bool notifyFirstParty = false, bool notifySecondParty = false)
        {
            var deptAdminInfoArbitration = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
                Subject = subject,
                To = new List<string>()
            };

            var objSms = new Contracts.Dtos.Integration.GEE.SMSRequestDto()
            {
                message_text = msg,
                recipients = new List<string>(),
                notification_type = "SMS",
            };

            var lstNotification = new List<SysNotificationDto>();

            // Notify Applicant Citizen
            if (notifyApplicant && litigationRequest.ApplicantCitizen != null)
            {
                objEmail.To.Add(litigationRequest.ApplicantCitizen.Email);
                objSms.recipients.Add(litigationRequest.ApplicantCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.ApplicantCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify Arbitrator Citizen
            if (notifyArbitrators && litigationRequest?.ArbitratorCitizen is not null)
            {
                objEmail.To.Add(litigationRequest.ArbitratorCitizen.Email);
                objSms.recipients.Add(litigationRequest.ArbitratorCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = (Guid)litigationRequest?.ArbitratorCitizen?.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }



            // Notify Department Admin
            if (notifyDeptAdmin && deptAdminInfoArbitration != null)
            {
                objEmail.To.Add(deptAdminInfoArbitration.Data.Email);
                objSms.recipients.Add(deptAdminInfoArbitration.Data.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = deptAdminInfoArbitration.Data.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }

            // Notify First Party Citizen
            if (notifyFirstParty && litigationRequest.Contract?.FirstPartyCitizen != null && litigationRequest.Contract.FirstPartyCitizen.Id != litigationRequest.ApplicantCitizen.Id)
            {
                objEmail.To.Add(litigationRequest.Contract.FirstPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.FirstPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.FirstPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }
            // Notify First Party Facility
            //if (notifyFirstParty && litigationRequest.Contract?.FirstPartyFacility != null && litigationRequest.Contract.FirstPartyFacility.Citizen.Id != litigationRequest.ApplicantCitizen.Id)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.FirstPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.FirstPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.FirstPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}



            // Notify Second Party Citizen
            if (notifySecondParty && litigationRequest.Contract?.SecondPartyCitizen != null)
            {
                objEmail.To.Add(litigationRequest.Contract.SecondPartyCitizen.Email);
                objSms.recipients.Add(litigationRequest.Contract.SecondPartyCitizen.Mobile);
                lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
                {
                    ActorId = litigationRequest.Contract.SecondPartyCitizen.Id,
                    CreatedOn = DateTime.Now,
                    ActorTypeId = notificationActorType,
                    IsViewed = false,
                    ViewedOn = null,
                    RequestId = litigationRequest.Id,
                    RequestTypeId = requestType,
                    Message = msg,
                });
            }
            // Notify Second Party Facility
            //if (notifySecondParty && litigationRequest.Contract?.SecondPartyFacility != null)
            //{
            //    objEmail.To.Add(litigationRequest.Contract.SecondPartyFacility.Email);
            //    objSms.recipients.Add(litigationRequest.Contract.SecondPartyFacility.Citizen.Mobile);
            //    lstNotification.Add(new Contracts.Dtos.Notification.SysNotificationDto()
            //    {
            //        ActorId = (Guid)litigationRequest.Contract.SecondPartyFacility.Citizen.Id,
            //        CreatedOn = DateTime.Now,
            //        ActorTypeId = notificationActorType,
            //        IsViewed = false,
            //        ViewedOn = null,
            //        RequestId = litigationRequest.Id,
            //        RequestTypeId = requestType,
            //        Message = msg,
            //    });
            //}

            await _notificationService.SendNotifications(objEmail, objSms, lstNotification);
        }


        #endregion

    }
}
