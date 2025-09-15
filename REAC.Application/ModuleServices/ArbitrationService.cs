using AutoMapper;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Shared.Responses;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using REAC.Application.AppServices.System;
using REAC.Application.Contracts.Dtos.Arbitration;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices.UserManage;
using REAC.Application.Contracts.IAppServices.Wpf.Business;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using System.Linq;
using Wafi.Application.Hubs;
using Wafi.Application.Hubs.Contracts;

namespace REAC.Application.ModuleServices
{
    public interface IArbitrationService
    {
        #region ArbitratorsCount
        Task<BaseResponse> AddArbitratorsCount(ArbitratorCountRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorsCount(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorsCount(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsCountReply(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsCountReply(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsCountReply(ArbitratorCountRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken);
        #endregion

        #region SelectionDuration
        Task<BaseResponse> AddArbitratorSelectionDuration(ArbitratorSelectionDurationRequestPayload arbitratorCountRequestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorSelectionDuration(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForArbitratorSelectionDuration(int pageIndex, int pageSize, CancellationToken cancellationToken);
        #endregion

        #region ExtendPeriod
        Task<BaseResponse> AddArbitrationExtendPeriod(LitigationRequestExtendPeriodRequestCreateUpdateDto requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> AddArbitrationExtendPeriodForEmployee(LitigationRequestExtendPeriodRequestCreateUpdateEmployeeDto requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriod(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriodForEmployee(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriod(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestExtendPeriodForEmployee(int pageIndex, int pageSize, CancellationToken cancellationToken);
        #endregion

        #region Session
        Task<BaseResponse> AddArbitrationSession(ScheduleSessionProcessRequestPayload requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForSession(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForSession(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAllSessionForArbitrator(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForFullAllSession(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForFullAllArbitrator(int pageIndex, int pageSize, CancellationToken cancellationToken);
        #endregion

        #region Report
        Task<BaseResponse> AddArbitrationReport(WritingArbitrationReportRequestPayload requestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForReport(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForReport(int pageIndex, int pageSize, CancellationToken cancellationToken);
        #endregion

        #region ExplanationRequest
        Task<BaseResponse> AddArbitrationReportExplanationRequest(LitigationRequestArbitrationReportExplanationRequestCreateDto requestPayload);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequest(Guid requestId);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequest(int pageIndex, int pageSize);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequestForArbitrator(int pageIndex, int pageSize);
        Task<BaseResponse> GetLitigationRequestForReportExplanationRequestForArbitrator(Guid requestId);
        Task<BaseResponse> AddArbitrationReportExplanationRequestForArbitrator(LitigationRequestArbitrationReportExplanationResponseDto requestPayload);
        #endregion

        #region NominationList
        Task<BaseResponse> AddLitigationRequestArbitratorsNominationList(ArbitratorNominationListRequestPayload arbitratorNominationListRequestPayload, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorsNominationList(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorsNominationList(int pageIndex, int pageSize, CancellationToken cancellationToken);

        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsNominationListReply(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsNominationListReply(Guid requestId, CancellationToken cancellationToken);
        Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsNominationListReply(ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload, CancellationToken cancellationToken);
        Task<BaseResponse> AddLitigationRequestArbitratorsdecision(Guid requestId, Guid arbitratorId);
        #endregion

        #region ChairmanNominationList
        Task<BaseResponse> AddLitigationRequestArbitratorsChairmandecision(Guid requestId, Guid arbitratorChairmanId);

        Task<BaseResponse> AddLitigationRequestArbitratorsChairmanNominationList(ArbitratorNominationListRequestPayload arbitratorNominationListRequestPayload);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorsChairmanNominationList(Guid requestId);
        Task<BaseResponse> GetLitigationRequestForAddArbitratorsChairmanNominationList(int pageIndex, int pageSize);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(int pageIndex, int pageSize);
        Task<BaseResponse> GetCitizenLitigationRequestForArbitratorsChairmanNominationListReply(Guid requestId);
        Task<BaseResponse> AddCitizenLitigationRequestForReportArbitratorsChairmanNominationListReply(ArbitratorNominationListRequestReplyPayload arbitratorNominationListRequestReplyPayload);
        #endregion

        #region RecusalRequest
        Task<BaseResponse> AddArbitrationArbitratorRecusalRequest(LitigationRequestArbitrationArbitratorRecusalRequestCreateDto requestPayload);
        Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequest(Guid requestId);
        Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequest(int pageIndex, int pageSize);

        // Backoffice
        Task<BaseResponse> AddArbitrationReportArbitratorRecusalForArbitrator(LitigationRequestArbitrationRecusalRequestResponseDto requestPayload);
        Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequestForArbitratorByRequestId(Guid litigationRequestId);
        Task<BaseResponse> GetLitigationRequestForArbitratorRecusalRequestForArbitrator(int pageIndex, int pageSize);

        #endregion


        Task<BaseResponse> Assign(AssignArbitratorRequestPayload assignRequestPayload);
        Task<BaseResponse> AssignAfterRecusal(AssignArbitratorRequestPayload assignRequestPayload);
        Task<BaseResponse> GetAllArbitrationLitigationRequest(int pageIndex, int pageSize, CancellationToken cancellationToken);

    }

    public partial class ArbitrationService : IArbitrationService
    {
        private readonly IMapper _mapper;
        private readonly ISysInvoiceService _sysInvoiceService;
        private readonly IEntityRepository _repository;
        private readonly IJwtReader _jwtReader;
        private readonly ISysSettingService _settingService;
        protected readonly INotificationService _notificationService;
        private readonly ISysEmployeeService _sysEmployeeService;
        private readonly IZoomService _zoomService;
        protected readonly IHubContext<NotificationHub, INotificationClient> _notificationHub;
        public ArbitrationService(IEntityRepository repository, ISysSettingService settingService,
                          IWorkFlowProcessingLogService workFlowProcessingLogService,
                          IMapper mapper, ISysInvoiceService sysInvoiceService,
                          IJwtReader jwtReader, ISysEmployeeService sysEmployeeService, INotificationService notificationService, IZoomService zoomService)
        {
            _mapper = mapper;
            _sysInvoiceService = sysInvoiceService;
            _repository = repository;
            _jwtReader = jwtReader;
            _settingService = settingService;
            _sysEmployeeService = sysEmployeeService;
            _notificationService = notificationService;
            _zoomService = zoomService;
        }

        public async Task<BaseResponse> GetLitigationRequestForFullAllArbitrator(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                      .Include(x => x.ArbitratorCitizen)
                                      .Include(x => x.LitigationRequestArbitratorNominationLists)
                                      .Include(x => x.LitigationRequestArbitratorLists)
                                      .Include(x => x.Contract)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.Contract.FirstPartyFacility)
                                      .Include(x => x.Contract.FirstPartyCitizen)
                                      .Include(x => x.Contract.SecondPartyFacility)
                                      .Include(x => x.Contract.SecondPartyCitizen)
                                      .Include(x => x.Contract.LitigationRequestContractType);

            litigationRequests = entitiesQuerable.Where(x => x.LitigationRequestCategoryId
            != LitigationRequestCategory.Reconciliation & x.IsFinished != true
            & x.LitigationRequestStage == LitigationRequestStage.Arbitration)
              .OrderByDescending(x => x.LitigationRequestSessions.OrderBy(x => x.CreatedOn)
              .LastOrDefault().SessionDatetime)
              .AsSplitQuery()
            .AsNoTracking()
            .GetPaged(pageIndex, pageSize);


            List<LitigationRequestMircoDto> litigationRequestMircoDtos = new List<LitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests.Results)
                {
                    bool allowAdd = true;

                    if (!litigationRequest.LitigationRequestArbitratorNominationLists.Any())
                        allowAdd = false;

                    LitigationRequestMircoDto litigationRequestMircoDto = new LitigationRequestMircoDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;


                    if (allowAdd)
                        litigationRequestMircoDto.IsDone = "--";
                    else
                        litigationRequestMircoDto.IsDone = "تم";

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

        //public async Task<BaseResponse> GetAllArbitrationLitigationRequest(int pageIndex, int pageSize, CancellationToken cancellationToken)
        //{
        //    Guid referenceId = Guid.Empty;
        //    _jwtReader.GetReferenceId(ref referenceId);

        //    PagedResult<LitigationRequest> litigationRequests = null;
        //    var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())
        //                              .Include(x => x.ArbitratorCitizen)
        //                              .Include(x => x.LitigationRequestArbitratorLists)
        //                              .ThenInclude(x => x.ArbitratorCitizen)
        //                              .Include(x => x.Contract)
        //                              .Include(x => x.Contract.LitigationRequestContractType)
        //                              .Include(x => x.Contract.FirstPartyFacility)
        //                              .Include(x => x.Contract.FirstPartyCitizen)
        //                              .Include(x => x.Contract.SecondPartyFacility)
        //                              .Include(x => x.Contract.SecondPartyCitizen)
        //                              .Include(x => x.Contract.LitigationRequestContractType);

        //    litigationRequests = entitiesQuerable.Where(x => x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation & x.IsFinished != true
        //    & x.LitigationRequestStage == LitigationRequestStage.Arbitration)
        //      .OrderByDescending(x => x.CreatedOn)
        //      .AsSplitQuery()
        //    .AsNoTracking()
        //    .GetPaged(pageIndex, pageSize);


        //    List<LitigationRequestMircoArbitratorDto> litigationRequestMircoDtos = new List<LitigationRequestMircoArbitratorDto>();
        //    if (litigationRequests != null)
        //    {
        //        foreach (var litigationRequest in litigationRequests.Results)
        //        {
        //            bool allowAdd = true;



        //            if (litigationRequest.ArbitratorCitizen != null)
        //            {
        //                LitigationRequestMircoArbitratorDto litigationRequestMircoDto = new LitigationRequestMircoArbitratorDto();
        //                litigationRequestMircoDto.Id = litigationRequest.Id;
        //                litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
        //                litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
        //                litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
        //                litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
        //                litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
        //                litigationRequestMircoDto.ArbitratorName = litigationRequest.ArbitratorCitizen?.FullName;
        //                litigationRequestMircoDto.ArbitratoId = litigationRequest.ArbitratorCitizen?.Id;
        //                litigationRequestMircoDto.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
        //                litigationRequestMircoDtos.Add(litigationRequestMircoDto);
        //            }
        //            litigationRequest.LitigationRequestArbitratorLists.ToList().ForEach(t =>
        //            {
        //                LitigationRequestMircoArbitratorDto litigationRequestMircoDto = new LitigationRequestMircoArbitratorDto();
        //                litigationRequestMircoDto.Id = litigationRequest.Id;
        //                litigationRequestMircoDto.LitigationRequestArbitratorListId = t.Id;
        //                litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
        //                litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
        //                litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
        //                litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
        //                litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
        //                litigationRequestMircoDto.ArbitratorName = t.ArbitratorCitizen?.FullName;
        //                litigationRequestMircoDto.ArbitratoId = t.ArbitratorCitizen?.Id;

        //                litigationRequestMircoDto.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
        //                litigationRequestMircoDtos.Add(litigationRequestMircoDto);
        //            });
        //        }
        //    }

        //    return new DataResponse<PagedResult<LitigationRequestMircoArbitratorDto>>
        //    {
        //        Data =
        //           new PagedResult<LitigationRequestMircoArbitratorDto>
        //           {
        //               Results = litigationRequestMircoDtos,
        //               CurrentPage = litigationRequests.CurrentPage,
        //               PageCount = litigationRequests.PageCount,
        //               PageSize = litigationRequests.PageSize,
        //               TotalCount = litigationRequests.TotalCount
        //           },
        //        IsDone = true
        //    };

        //}
        public async Task<BaseResponse> GetAllArbitrationLitigationRequest(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            // Ensure at least 10 items per page
            if (pageSize < 10)
                pageSize = 10;


            var entitiesQueryable = (await _repository.GetQueryable<LitigationRequest, Guid>())
                                   .Include(x => x.ArbitratorCitizen)
                                   .Include(x => x.LitigationRequestArbitratorLists)
                                   .ThenInclude(x => x.ArbitratorCitizen)
                                     .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                                     .ThenInclude(x=>x.ArbitratorCitizen)
                                   .Include(x => x.Contract)
                                   .Include(x => x.Contract.LitigationRequestContractType)
                                   .Include(x => x.Contract.FirstPartyFacility)
                                   .Include(x => x.Contract.FirstPartyCitizen)
                                   .Include(x => x.Contract.SecondPartyFacility)
                                   .Include(x => x.Contract.SecondPartyCitizen);

            var filteredQuery = entitiesQueryable
                .Where(x => x.LitigationRequestCategoryId != LitigationRequestCategory.Reconciliation
                           && x.IsFinished != true
                           && x.LitigationRequestStage == LitigationRequestStage.Arbitration
                           && x.InternalStatusId >= RequestStatus.WaitingForDetermineArbitratorsNumber
                          )

                .OrderByDescending(x => x.CreatedOn)
                .AsSplitQuery()
                .AsNoTracking();

            // Get total count for pagination
            var totalCount = await filteredQuery.CountAsync(cancellationToken);

            // Apply pagination to the query
            var litigationRequests = await filteredQuery
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            List<LitigationRequestMircoArbitratorDto> litigationRequestMircoDtos = new List<LitigationRequestMircoArbitratorDto>();

            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests)
                {
                    // Add ArbitratorCitizen (Chairman) if exists
                    if (litigationRequest.ArbitratorCitizen != null)
                    {
                        LitigationRequestMircoArbitratorDto litigationRequestMircoDto = new LitigationRequestMircoArbitratorDto();
                        litigationRequestMircoDto.Id = litigationRequest.Id;
                        litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                        litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                        litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                        litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                        litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                        litigationRequestMircoDto.ArbitratorName = litigationRequest.ArbitratorCitizen?.FullName;
                        litigationRequestMircoDto.ArbitratoId = litigationRequest.ArbitratorCitizen?.Id;
                        litigationRequestMircoDto.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.ArbitratorsChairman;
                        litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                    }
                    //if (litigationRequest.LitigationRequestSubStage == LitigationRequestSubStage.LitigationRequestAddArbitratorListNominationDone && (litigationRequest.IsDefendantArbitratorNominationListDone != true || litigationRequest.IsPlaintiffArbitratorNominationListDone != true))
                    //{

                    //}
                    if (litigationRequest.LitigationRequestArbitratorLists.Any())
                    {                         // Add all arbitrators from the list
                        litigationRequest.LitigationRequestArbitratorLists.ToList().ForEach(t =>
                        {
                            LitigationRequestMircoArbitratorDto litigationRequestMircoDto = new LitigationRequestMircoArbitratorDto();
                            litigationRequestMircoDto.Id = litigationRequest.Id;
                            litigationRequestMircoDto.LitigationRequestArbitratorListId = t.Id;
                            litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                            litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                            litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                            litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                            litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                            litigationRequestMircoDto.ArbitratorName = t.ArbitratorCitizen?.FullName;
                            litigationRequestMircoDto.ArbitratoId = t.ArbitratorCitizen?.Id;
                            litigationRequestMircoDto.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
                            litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                        });
                    }
                    else
                    {
                        LitigationRequestMircoArbitratorDto litigationRequestMircoDto = new LitigationRequestMircoArbitratorDto();
                        litigationRequestMircoDto.Id = litigationRequest.Id;
                        litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                        litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                        litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                        litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                        litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                        litigationRequestMircoDto.ArbitratorAssigningRequestTypeId = ArbitratorAssigningRequestType.Arbitrator;
                        litigationRequestMircoDto.ArbitratorName = litigationRequest.LitigationRequestArbitratorAssigningRequests?.OrderByDescending(s=>s.RequestOn)?.FirstOrDefault()?.ArbitratorCitizen?.FullName;
                        litigationRequestMircoDto.ArbitratoId = litigationRequest.LitigationRequestArbitratorAssigningRequests?.OrderByDescending(s => s.RequestOn)?.FirstOrDefault()?.ArbitratorCitizen?.Id;
                        litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                    }
                }
            }

            //// Remove duplicates based on Id but keep the original order
            //litigationRequestMircoDtos = litigationRequestMircoDtos
            //    .DistinctBy(s => new { s.Id, s.LitigationRequestArbitratorListId, s.ArbitratorAssigningRequestTypeId })
            //    .OrderByDescending(x => x.RequestDate)
            //    .ToList();

            // Calculate pagination info
            var pageCount = (int)Math.Ceiling((double)totalCount / pageSize);

            return new DataResponse<PagedResult<LitigationRequestMircoArbitratorDto>>
            {
                Data = new PagedResult<LitigationRequestMircoArbitratorDto>
                {
                    Results = litigationRequestMircoDtos,
                    CurrentPage = pageIndex,
                    PageCount = pageCount,
                    PageSize = pageSize,
                    TotalCount = totalCount
                },
                IsDone = true
            };
        }


        public async Task<BaseResponse> Assign(AssignArbitratorRequestPayload assignRequestPayload)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (referenceId == Guid.Empty)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير معروف"
                    };
                var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?
                        .Where(x => x.Id == referenceId).Single();

                if (employee == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير معروف"
                    };

                if (!employee.AllowAssign)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير مسموح له"
                    };

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                       .Include(x => x.ArbitratorCitizen)
                       .Include(x => x.LitigationRequestArbitratorLists)
                       .Include(x => x.ArbitratorCitizen)
                       .Where(x => x.Id == assignRequestPayload.RequestId).SingleOrDefault();


                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب غير موجود"
                    };

                if (litigationRequest.IsFinished.HasValue)
                {
                    if (litigationRequest.IsFinished.Value)
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "دعوى مغلقة"
                        };
                }

                if (litigationRequest.ArbitratorCitizenId.HasValue)
                {
                    if (!employee.AllowReAssign)
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "موظف غير مسموح له"
                        };
                }

                if (litigationRequest.LitigationRequestArbitratorLists.Any())
                {
                    if (litigationRequest.LitigationRequestArbitratorLists.Where(x => x.ArbitratorCitizenId == assignRequestPayload.ArbitratorId).Any())
                    {
                        return new BaseResponse()
                        {
                            IsDone = false,
                            Message = "المحكم بالفعل عضو بهيئة المحكمين بالدعوى"
                        };
                    }
                }

                var litigationRequestArbitratorAssigningRequests = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())?
                     .Where(x => x.LitigationRequestId == assignRequestPayload.RequestId & x.CitizenId == assignRequestPayload.ArbitratorId & x.Response == LitigationRequestArbitratorAssigningRequestStatus.None);

                if (litigationRequestArbitratorAssigningRequests.Any())
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "يوجد طلب مهمه سابق لم يتم الرد عليه حتى الان من قبل المحكم"
                    };
                }
                var litigationRequestArbitratorList = (await _repository.GetQueryable<LitigationRequestArbitratorList, Guid>())?
                           .Include(x => x.ArbitratorCitizen)
                            .Where(x => x.Id == assignRequestPayload.LitigationRequestArbitratorListId).SingleOrDefault();
                try
                {

                    if (assignRequestPayload.LitigationRequestArbitratorListId != Guid.Empty)
                    {
                        switch (assignRequestPayload.ArbitratorAssigningRequestTypeId)
                        {
                            case ArbitratorAssigningRequestType.Arbitrator:

                                var updateAssigingRequest = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                                    .Where(x => x.CitizenId == litigationRequestArbitratorList.ArbitratorCitizenId)
                                    .ExecuteUpdateAsync(upd => upd.SetProperty(cr => cr.Response, LitigationRequestArbitratorAssigningRequestStatus.Cancelled));

                                _repository.Remove<LitigationRequestArbitratorList, Guid>(litigationRequestArbitratorList);


                                // Send Notification
                                // تم إلغاء تعيينك كمحكم على الدعوى رقم كذا

                                var msg = "";
                                msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitrator_Reject).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                string body;
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitrator_Reject).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                }

                                var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);


                                if (arbitrationAdmin.IsDone)
                                {
                                    List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        },
                           new SysNotificationDto{
                        ActorId = (Guid)litigationRequestArbitratorList.ArbitratorCitizenId,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        }

                        };
                                    List<string> emails = new List<string>() {
                                  arbitrationAdmin.Data?.Email,
                                  litigationRequestArbitratorList.ArbitratorCitizen.Email
                                };
                                    List<string> recipients = new List<string>() {
                                  arbitrationAdmin.Data?.Mobile,
                                  litigationRequestArbitratorList.ArbitratorCitizen.Mobile
                                };

                                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                    {
                                        To = emails,
                                        Subject = "إلغاء مهمتك كمحكم على الدعوى",
                                        Body = body
                                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                    {
                                        message_text = msg,
                                        recipients = recipients,
                                        notification_type = "SMS",

                                    }, notifications);

                                }

                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم إلغاء تعيين المحكم " + litigationRequestArbitratorList.ArbitratorCitizen.FullName
                                });
                                break;
                            case ArbitratorAssigningRequestType.ArbitratorsChairman:
                                litigationRequest.ArbitratorCitizenId = null;
                                // Send Notification
                                // تم إلغاء تعيينك كرئيس هيئة تحكيم على الدعوى رقم كذا

                                var boosMsg = "";
                                boosMsg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitratorChairman_Rejected).Result.Data?.Value;
                                boosMsg = boosMsg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitratorChairman_Rejected).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                }

                                var admin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);


                                if (admin.IsDone)
                                {
                                    List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                           new SysNotificationDto{
                        ActorId = (Guid)admin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=boosMsg
                        },
                           new SysNotificationDto{
                        ActorId = (Guid)litigationRequest.ArbitratorCitizen.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=boosMsg
                        }

                        };
                                    List<string> emails = new List<string>() {
                                  admin.Data?.Email,
                                  litigationRequest.ArbitratorCitizen.Email
                                };
                                    List<string> recipients = new List<string>() {
                                  admin.Data?.Mobile,
                                  litigationRequest.ArbitratorCitizen.Mobile
                                };

                                    await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                    {
                                        To = emails,
                                        Subject = "إلغاء مهمتك كرأيس محكمين على الدعوى",
                                        Body = body
                                    }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                    {
                                        message_text = boosMsg,
                                        recipients = recipients,
                                        notification_type = "SMS",

                                    }, notifications);

                                }



                                await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                                {
                                    Id = Guid.NewGuid(),
                                    CreatedOn = DateTime.Now,
                                    RequestId = litigationRequest.Id,
                                    ShowForApplicant = false,
                                    LogMessage = "تم إلغاء تعيين كرئيس هيئة المحكمين  للمحكم " + litigationRequest?.ArbitratorCitizen?.FullName
                                });
                                break;
                        }

                    }
                }
                catch (Exception e)
                {


                }

                var newArbitrator = (await _repository.GetQueryable<Citizen, Guid>())?
                            .Where(x => x.Id == assignRequestPayload.ArbitratorId).SingleOrDefault();


                LitigationRequestArbitratorAssigningRequest litigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                litigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                litigationRequestArbitratorAssigningRequest.CitizenId = assignRequestPayload.ArbitratorId;
                litigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = assignRequestPayload.ArbitratorAssigningRequestTypeId;
                litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestArbitratorAssigningRequest);

                var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == litigationRequestArbitratorAssigningRequest.CitizenId);
                try
                {
                    switch (assignRequestPayload.ArbitratorAssigningRequestTypeId)
                    {


                        case ArbitratorAssigningRequestType.Arbitrator:
                            // Send Notification
                            // يوجد طلب مهمه جديد كمحكم على الدعوى رقم {LitigationCode}

                            var msg = "";
                            msg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }


                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = (Guid)arbitratorInfo.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        }

                        };
                            List<string> emails = new List<string>() {

                                  arbitratorInfo.Email
                                };
                            List<string> recipients = new List<string>() {

                                  arbitratorInfo.Mobile
                                };

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = " طلب مهمه جديد كمحكم على الدعوي",
                                Body = body
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, notifications);






                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إرسال طلب مهمة كمحكم للمحكم " + litigationRequestArbitratorList?.ArbitratorCitizen.FullName
                            });
                            break;
                        case ArbitratorAssigningRequestType.ArbitratorsChairman:

                            var updateOldLitigationRequestArbitratorAssigningRequests = await _repository.ExecuteUpdateAsync<LitigationRequestArbitratorAssigningRequest, Guid>(
                                x => x.ArbitratorAssigningRequestTypeId == ArbitratorAssigningRequestType.ArbitratorsChairman
                              , y => y.SetProperty(z => z.Response, LitigationRequestArbitratorAssigningRequestStatus.Cancelled)
                              , default);

                            if (!updateOldLitigationRequestArbitratorAssigningRequests.IsDone)
                            {
                                return new BaseResponse()
                                {
                                    IsDone = false,
                                    Message = "خطأ داخلي"
                                };
                            }
                            // Send Notification
                            // تم ارسال طلب مهمة كرئيس هيئة تحكيم على الدعوى رقم كذا

                            var bossMsg = "";
                            bossMsg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitratorsChairmanNominationList_TaskSubmitedToArbitrator).Result.Data?.Value;
                            bossMsg = bossMsg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ArbitraorChairman_Assigned).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }

                            List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = assignRequestPayload.ArbitratorId,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=bossMsg
                        }

                        };
                            List<string> emailList = new List<string>() {

                                  newArbitrator.Email
                                };
                            List<string> recipientList = new List<string>() {

                                  newArbitrator.Mobile
                                };

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emailList,
                                Subject = " طلب مهمه جديد كرأيس هيئة محكمين على الدعوي",
                                Body = body
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = bossMsg,
                                recipients = recipientList,
                                notification_type = "SMS",

                            }, notificationList);

                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إرسال طلب مهمة كرئيس هيئة المحكمين للمحكم " + newArbitrator.FullName
                            });
                            break;
                    }
                }
                catch (Exception e)
                {


                }


                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                return repositorySaveResponse;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }

        public async Task<BaseResponse> AssignAfterRecusal(AssignArbitratorRequestPayload assignRequestPayload)
        {
            try
            {

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                       .Include(x => x.LitigationRequestArbitratorLists)
                       .Include(x => x.ArbitratorCitizen)
                       .Where(x => x.Id == assignRequestPayload.RequestId).SingleOrDefault();


                var litigationRequestArbitratorAssigningRequests = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())?
                     .Where(x => x.LitigationRequestId == assignRequestPayload.RequestId & x.CitizenId == assignRequestPayload.ArbitratorId & x.Response == LitigationRequestArbitratorAssigningRequestStatus.None);


                var litigationRequestArbitratorList = (await _repository.GetQueryable<LitigationRequestArbitratorList, Guid>())?
                           .Include(x => x.ArbitratorCitizen)
                            .Where(x => x.Id == assignRequestPayload.LitigationRequestArbitratorListId).Single();
                try
                {
                    switch (assignRequestPayload.ArbitratorAssigningRequestTypeId)
                    {
                        case ArbitratorAssigningRequestType.Arbitrator:


                            var updateAssigingRequest = (await _repository.GetQueryable<LitigationRequestArbitratorAssigningRequest, Guid>())
                                     .Where(x => x.CitizenId == litigationRequestArbitratorList.ArbitratorCitizenId)
                                     .ExecuteUpdateAsync(upd => upd.SetProperty(cr => cr.Response, LitigationRequestArbitratorAssigningRequestStatus.Cancelled));

                            _repository.Remove<LitigationRequestArbitratorList, Guid>(litigationRequestArbitratorList);
                            // Send Notification
                            // تم إلغاء تعيينك كمحكم على الدعوى رقم كذا

                            var msg = "";
                            msg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitrator_Reject).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitrator_Reject).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }

                            var arbitrationAdmin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);


                            if (arbitrationAdmin.IsDone)
                            {
                                List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                           new SysNotificationDto{
                        ActorId = (Guid)arbitrationAdmin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=msg
                        },
                           new SysNotificationDto{
                        ActorId = (Guid)litigationRequestArbitratorList.ArbitratorCitizenId,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        }

                        };
                                List<string> emails = new List<string>() {
                                  arbitrationAdmin.Data?.Email,
                                  litigationRequestArbitratorList.ArbitratorCitizen.Email
                                };
                                List<string> recipients = new List<string>() {
                                  arbitrationAdmin.Data?.Mobile,
                                  litigationRequestArbitratorList.ArbitratorCitizen.Mobile
                                };

                                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                {
                                    To = emails,
                                    Subject = "إلغاء مهمتك كمحكم على الدعوى",
                                    Body = body
                                }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                {
                                    message_text = msg,
                                    recipients = recipients,
                                    notification_type = "SMS",

                                }, notifications);

                            }

                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إلغاء تعيين المحكم " + litigationRequestArbitratorList.ArbitratorCitizen.FullName
                            });
                            break;
                        case ArbitratorAssigningRequestType.ArbitratorsChairman:
                            litigationRequest.ArbitratorCitizenId = null;
                            // Send Notification
                            // تم إلغاء تعيينك كرئيس هيئة تحكيم على الدعوى رقم كذا

                            var boosMsg = "";
                            boosMsg = _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitratorChairman_Rejected).Result.Data?.Value;
                            boosMsg = boosMsg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.ArbitraorRecusal_InformArbitratorChairman_Rejected).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }

                            var admin = await _sysEmployeeService.GetAdminsByDepartmentType(DepartmentType.Arbitration);


                            if (admin.IsDone)
                            {
                                List<SysNotificationDto> notifications = new List<SysNotificationDto>() {
                           new SysNotificationDto{
                        ActorId = (Guid)admin.Data?.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Employee,
                        Message=boosMsg
                        },
                           new SysNotificationDto{
                        ActorId = (Guid)litigationRequest.ArbitratorCitizen.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=boosMsg
                        }

                        };
                                List<string> emails = new List<string>() {
                                  admin.Data?.Email,
                                  litigationRequest.ArbitratorCitizen.Email
                                };
                                List<string> recipients = new List<string>() {
                                  admin.Data?.Mobile,
                                  litigationRequest.ArbitratorCitizen.Mobile
                                };

                                await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                                {
                                    To = emails,
                                    Subject = "إلغاء مهمتك كرأيس محكمين على الدعوى",
                                    Body = body
                                }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                                {
                                    message_text = boosMsg,
                                    recipients = recipients,
                                    notification_type = "SMS",

                                }, notifications);

                            }



                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إلغاء تعيين كرئيس هيئة المحكمين  للمحكم " + litigationRequestArbitratorList?.ArbitratorCitizen.FullName
                            });
                            break;
                    }

                }
                catch (Exception e)
                {


                }

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())?
                            .Where(x => x.Id == assignRequestPayload.ArbitratorId).Single();

                LitigationRequestArbitratorAssigningRequest litigationRequestArbitratorAssigningRequest = new LitigationRequestArbitratorAssigningRequest();
                litigationRequestArbitratorAssigningRequest.RequestOn = DateTime.Now;
                litigationRequestArbitratorAssigningRequest.CitizenId = assignRequestPayload.ArbitratorId;
                litigationRequestArbitratorAssigningRequest.LitigationRequestId = litigationRequest.Id;
                litigationRequestArbitratorAssigningRequest.ArbitratorAssigningRequestTypeId = assignRequestPayload.ArbitratorAssigningRequestTypeId;
                litigationRequestArbitratorAssigningRequest.Response = LitigationRequestArbitratorAssigningRequestStatus.None;
                await _repository.AddAsync<LitigationRequestArbitratorAssigningRequest, Guid>(litigationRequestArbitratorAssigningRequest);

                var arbitratorInfo = (await _repository.GetQueryable<Citizen, Guid>()).FirstOrDefault(c => c.Id == litigationRequestArbitratorAssigningRequest.CitizenId);
                try
                {
                    switch (assignRequestPayload.ArbitratorAssigningRequestTypeId)
                    {


                        case ArbitratorAssigningRequestType.Arbitrator:
                            // Send Notification
                            // يوجد طلب مهمه جديد كمحكم على الدعوى رقم {LitigationCode}

                            var msg = "";
                            msg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value;
                            msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_Arbitraor_Assigned).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }


                            List<SysNotificationDto> notifications = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = (Guid)arbitratorInfo.Id,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=msg
                        }

                        };
                            List<string> emails = new List<string>() {

                                  arbitratorInfo.Email
                                };
                            List<string> recipients = new List<string>() {

                                  arbitratorInfo.Mobile
                                };

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emails,
                                Subject = " طلب مهمه جديد كمحكم على الدعوي",
                                Body = body
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = msg,
                                recipients = recipients,
                                notification_type = "SMS",

                            }, notifications);






                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إرسال طلب مهمة كمحكم للمحكم " + litigationRequestArbitratorList?.ArbitratorCitizen.FullName
                            });
                            break;
                        case ArbitratorAssigningRequestType.ArbitratorsChairman:
                            // Send Notification
                            // تم قبول كرئيس هيئة تحكيم على الدعوى رقم كذا

                            var bossMsg = "";
                            bossMsg = _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ArbitraorChairman_Assigned).Result.Data?.Value;
                            bossMsg = bossMsg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _settingService.GetBySettingKey(SysSettingKeyConstants.Arbitration_ArbitraorChairman_Assigned).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }

                            List<SysNotificationDto> notificationList = new List<SysNotificationDto>() {

                           new SysNotificationDto{
                        ActorId = (Guid)litigationRequest.ArbitratorCitizenId,
                        RequestId=litigationRequest.Id,
                        CreatedOn= DateTime.Now,
                        RequestTypeId=RequestType.LitigationRequest,
                        ActorTypeId=NotificationActorType.Arbitrator,
                        Message=bossMsg
                        }

                        };
                            List<string> emailList = new List<string>() {

                                  litigationRequest.ArbitratorCitizen.Email
                                };
                            List<string> recipientList = new List<string>() {

                                  litigationRequest.ArbitratorCitizen.Mobile
                                };

                            await _notificationService.SendNotifications(new Contracts.Dtos.Notification.SendEmailDto()
                            {
                                To = emailList,
                                Subject = " طلب مهمه جديد كرأيس هيئة محكمين على الدعوي",
                                Body = body
                            }, new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                            {
                                message_text = bossMsg,
                                recipients = recipientList,
                                notification_type = "SMS",

                            }, notificationList);

                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = "تم إرسال طلب مهمة كرئيس هيئة المحكمين للمحكم " + litigationRequestArbitratorList?.ArbitratorCitizen.FullName
                            });
                            break;
                    }
                }
                catch (Exception e)
                {


                }


                var repositorySaveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                return repositorySaveResponse;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                };
            }
        }
    }
}
