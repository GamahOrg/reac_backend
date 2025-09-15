using AutoMapper;
using Azure.Core;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.Dto;
using Core.Log.Application.Contracts.IAppServices;
using Core.Log.Domain.Shared.Enums;
using Core.Security.Entities;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using REAC.Application.Contracts.Dtos.Citizen;
using REAC.Application.Contracts.Dtos.Delegator;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Application.Contracts.Dtos.Integration.Rent;
using REAC.Application.Contracts.Dtos.Integration.Wafi;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.Get;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Notification;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.Dtos.Wpf.Business;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Integration;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.Notification;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Application.Contracts.IAppServices.Wpf.System;
using REAC.Application.Contracts.IModuleServices;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.Domain.Shared.Helpers;
using REAC.EntityFrameworkCore.Entities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;
using REAC.EntityFrameworkCore.Seed.Integration;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static REAC.Application.Contracts.IAppServices.Litigation.ILitigationRequestService;



namespace REAC.Application.AppServices.Litigation
{
    public partial class LitigationRequestService : BasicService<LitigationRequest, Guid>, ILitigationRequestService
    {
        private readonly REACDBContext _context;
        private readonly IJwtReader _jwtReader;
        private readonly IServiceProvider _serviceProvider;
        private readonly IFacilityService _facilityService;
        private readonly UserManager<User> _userManager;
        private readonly ISysEmployeeService _employeeService;
        private readonly INotificationService _notificationService;
        private readonly ISysSettingService _sysSettingService;
        private readonly IGEELoginService _gEELoginService;
        private readonly INafathService _nafathService;
        private readonly IRENTService _rentService;

        public LitigationRequestService(REACDBContext context, UserManager<User> userManager, IFacilityService facilityService, IServiceProvider service,
              IEntityRepository repository,
            IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysEmployeeService employeeService, INotificationService notificationService, ISysSettingService sysSettingService, IGEELoginService gEELoginService, INafathService nafathService, IRENTService rentService) : base(repository, mapper, loggerManager, jwtReader)

        {
            _userManager = userManager;
            _facilityService = facilityService;
            _context = context;
            _jwtReader = jwtReader;

            _serviceProvider = service;
            _employeeService = employeeService;
            _notificationService = notificationService;
            _sysSettingService = sysSettingService;
            _gEELoginService = gEELoginService;
            _nafathService = nafathService;
            _rentService = rentService;
        }

        public async Task<BaseResponse> AddLitigation(LitigationCreateUpdateDto model)
        {
            return await Add(model);
        }
        public async Task<BaseResponse> Add(LitigationCreateUpdateDto model)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.LitigationRequestContractType)
                          .Include(x => x.LitigationRequestRequests)
                          .Include(x => x.LitigationRequestGrounds)
                          .ThenInclude(x => x.LitigationRequestGroundFiles)
                          .Include(x => x.LitigationRequestAmount)
                          .Where(x => x.Contract.Id == model.ContractId).ToList().FirstOrDefault();

                litigationRequest.Title = model.Title;
                litigationRequest.Description = model.Description;
                litigationRequest.IsFinancialLitigation = model.IsFinancialLitigation;

                if (model.LitigationRequestAmountId != Guid.Empty)
                    litigationRequest.LitigationRequestAmountId = model.LitigationRequestAmountId;
                else
                {
                    var lowestFeesId = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())
                        .AsNoTracking().OrderBy(x => x.Fees).First().Id;

                    litigationRequest.LitigationRequestAmountId = lowestFeesId;

                }


                if (model.AreaId != Guid.Empty)
                    litigationRequest.AreaId = model.AreaId;

                if (model.CityId != Guid.Empty)
                    litigationRequest.CityId = model.CityId;

                if (model.ExemptionReductionCategory != null)
                    litigationRequest.IsExemptionReduction = model.ExemptionReductionCategory.IsExemptionReductionCategory;

                if (model.ExemptionReductionCategory.AttachmentId != Guid.Empty)
                    litigationRequest.ExemptionReductionFileId = model.ExemptionReductionCategory.AttachmentId;


                if (model.ExemptionReductionCategory.Id != Guid.Empty)
                    litigationRequest.ExemptionReductionCategoryId = model.ExemptionReductionCategory.Id;

                var lastCategoryId = litigationRequest.LitigationRequestCategoryId;
                litigationRequest.LitigationRequestCategoryId = model.LitigationRequestCategoryId;

                if (model.Requests != null)
                {
                    foreach (var request in model.Requests)
                    {
                        if (!litigationRequest.LitigationRequestRequests.Any(l => l.Id == request.RequestId))
                        {
                            litigationRequest.LitigationRequestRequests.Add(new EntityFrameworkCore.Entities.Litigation.LitigationRequestRequest()
                            {
                                Id = Guid.NewGuid(),
                                LitigationRequestId = litigationRequest.Id,
                                RequestContent = request.LitigationPurpose
                            });
                        }

                    }
                }
                if (model.References != null)
                {
                    foreach (var reference in model.References)
                    {
                        if (!litigationRequest.LitigationRequestGrounds.Any(l => l.Id == reference.ReferenceId))
                        {

                            LitigationRequestGround litigationRequestGround = new LitigationRequestGround();
                            litigationRequestGround.Id = Guid.NewGuid();
                            litigationRequestGround.LitigationRequestId = litigationRequest.Id;
                            litigationRequestGround.Title = reference.ReferenceTitle;
                            if (reference.ReferenceAttachments != null)
                            {
                                foreach (var file in reference.ReferenceAttachments)
                                {
                                    litigationRequestGround.LitigationRequestGroundFiles.Add(new LitigationRequestGroundFile()
                                    {
                                        LitigationRequestGroundId = litigationRequestGround.Id,
                                        Id = Guid.NewGuid(),
                                        FileManagerId = file
                                    });
                                }
                            }
                            litigationRequest.LitigationRequestGrounds.Add(litigationRequestGround);
                        }

                        if (litigationRequest.LitigationRequestGrounds.Any(l => l.Id == reference.ReferenceId)
                            & litigationRequest.LitigationRequestGrounds.Any(l => l.LitigationRequestGroundFiles == null || l.LitigationRequestGroundFiles.Count == 0)
                            & reference.ReferenceAttachments is not null
                            )
                        {
                            foreach (var file in reference.ReferenceAttachments)
                            {
                                litigationRequest.LitigationRequestGrounds.FirstOrDefault(l => l.Id == reference.ReferenceId).LitigationRequestGroundFiles.Add(new LitigationRequestGroundFile()
                                {
                                    LitigationRequestGroundId = litigationRequest.LitigationRequestGrounds.FirstOrDefault(l => l.Id == reference.ReferenceId).Id,
                                    Id = Guid.NewGuid(),
                                    FileManagerId = file
                                });
                            }
                        }

                    }
                }


                if (model.IsSent)
                {
                    var litigationRequestList = (await _repository.GetQueryable<LitigationRequest, Guid>())
                     .Where(x => x.RequestCode != null & x.Contract.LitigationRequestContractType == litigationRequest.Contract.LitigationRequestContractType).ToList();
                    string requestCode = "";

                    if (litigationRequest.RequestCode == null)
                    {
                        switch (litigationRequest.Contract.LitigationRequestContractTypeId)
                        {
                            case LitigationRequestContractType.Wafi:
                                if (litigationRequestList == null)
                                    requestCode = "W00001-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                else
                                    requestCode = "W" + (litigationRequestList.Count() + 1).ToString("D5") + "-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                break;
                            case LitigationRequestContractType.Rent:
                                if (litigationRequestList == null)
                                    requestCode = "R00001-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                else
                                    requestCode = "R" + (litigationRequestList.Count() + 1).ToString("D5") + "-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                break;
                            case LitigationRequestContractType.Properties:
                                if (litigationRequestList == null)
                                    requestCode = "O00001-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                else
                                    requestCode = "O" + (litigationRequestList.Count() + 1).ToString("D5") + "-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                break;
                            case LitigationRequestContractType.RealEstateBrokerage:
                                if (litigationRequestList == null)
                                    requestCode = "B00001-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                else
                                    requestCode = "B" + (litigationRequestList.Count() + 1).ToString("D5") + "-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US")); ;
                                break;
                            case LitigationRequestContractType.OtherServices:
                                if (litigationRequestList == null)
                                    requestCode = "N00001-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US"));
                                else
                                    requestCode = "N" + (litigationRequestList.Count() + 1).ToString("D5") + "-" + DateTime.Now.Date.ToString("yy", new CultureInfo("en-US")); ;
                                break;
                        }

                        // Request Code Check

                        requestCode = await EnsureUniqueRequestCodeAsync(requestCode, litigationRequest);

                        litigationRequest.IsSent = model.IsSent;
                        litigationRequest.SentOn = DateTime.Now;
                        litigationRequest.RequestCode = requestCode;
                        litigationRequest.ExternalStatusId = RequestStatus.New;
                        litigationRequest.InternalStatusId = RequestStatus.New;
                        litigationRequest.LitigationRequestStage = LitigationRequestStage.Review;
                    }
                }





                var saveResponse = _repository.SaveChangesAsync().Result;
                if (!saveResponse.IsDone)
                    return saveResponse;
                if (model.IsSent)
                {
                    var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                    var _stepService = _serviceProvider.GetRequiredService<IWorkFlowStepService>();

                    var processingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())
                        .Include(t => t.WorkFlowStep)
                        .Include(t => t.WorkFlowStep.WorkFlowStepEvents)
                        .AsNoTracking()
                        .Where(x => x.RequestId == litigationRequest.Id).ToList();

                    if (processingLog.Count > 0)
                    {

                        var lastAmountLog = (await _repository.GetQueryable<LitigationRequestAmountLog, Guid>())
                                                          .Include(x => x.Amounts)
                                                          .Where(log => log.LitigationRequest.Id == litigationRequest.Id)
                                                          .OrderByDescending(log => log.CreatedOn)
                                                          .FirstOrDefault();

                        var currentAmount = (await _repository.GetQueryable<SysLitigationRequestAmount, Guid>())
                                                  .Where(s => s.Id == litigationRequest.LitigationRequestAmountId).FirstOrDefault();


                        //check amount change
                        if (lastAmountLog != null && currentAmount.Fees > lastAmountLog.Amounts.Fees)
                        {
                            await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog
                            {
                                Id = Guid.NewGuid(),
                                CreatedOn = DateTime.Now,
                                RequestId = litigationRequest.Id,
                                ShowForApplicant = false,
                                LogMessage = $"تم تغيير رسوم التسجيل من {lastAmountLog.Amounts.Fees} الى {currentAmount.Fees}."
                            });

                            litigationRequest.IsSent = model.IsSent;
                            litigationRequest.SentOn = DateTime.Now;
                            litigationRequest.ExternalStatusId = RequestStatus.New;
                            litigationRequest.InternalStatusId = RequestStatus.New;
                            litigationRequest.LitigationRequestStage = LitigationRequestStage.Review;
                            litigationRequest.FeesDifference = currentAmount.Fees - lastAmountLog.Amounts.Fees;
                            litigationRequest.IsLitigationFeesHaveBeenCollected = false;


                            var Response = await _repository.SaveChangesAsync();
                            if (Response.IsDone)
                                await _bpfService.ExcuteWorkFlow(litigationRequest.Id, default, null, true);

                            // Set the latest processing log IsFinished = true

                            var lastUnfinishedWorkflow = processingLog.FirstOrDefault(x => x.IsFinished != true);
                            if (lastUnfinishedWorkflow != null)
                            {
                                lastUnfinishedWorkflow.IsFinished = true;
                                lastUnfinishedWorkflow.FinishedOn = DateTime.Now;
                                _repository.Update<WorkFlowProcessingLog, Guid>(lastUnfinishedWorkflow);
                            }

                        }

                        else
                        {
                            var currentProcessingLog = processingLog.Where(t => t.IsFinished == false).OrderBy(t => t.CreatedOn).ToList();
                            if (processingLog.Count > 0 && currentProcessingLog.Count > 0)
                            {

                                ProcessRequestPayload processRequestPayload = new ProcessRequestPayload();
                                processRequestPayload.RequestId = litigationRequest.Id;
                                processRequestPayload.ProcessingLogId = currentProcessingLog.LastOrDefault().Id;
                                processRequestPayload.WorkflowStepEventId = currentProcessingLog.LastOrDefault().WorkFlowStep.WorkFlowStepEvents.FirstOrDefault().Id;
                                processRequestPayload.Notes = "";
                                processRequestPayload.UploadedFilesIds = new List<Guid>();
                                if (lastCategoryId != model.LitigationRequestCategoryId)
                                {
                                    processRequestPayload.CategoryTypeChanged = true;
                                    processRequestPayload.CategoryType = model.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration ? "دعوة تحكيم" : "دعوة صلح";
                                }
                                await _bpfService.ProcessWorkFlow(processRequestPayload, default);
                                return new DataResponse<LitigationCreateUpdateDto>()
                                {
                                    Data = model,
                                    IsDone = true,
                                    Message = ""
                                };
                            }
                        }



                    }
                    else
                        await _bpfService.ExcuteWorkFlow(litigationRequest.Id, default, null);



                    #region billing 
                    // billing comparing last amount with current amount 
                    // adding new litigation amountlog 
                    // comparing amounts if new amount is greater than last adding new step log 
                    if (litigationRequest.IsFinancialLitigation == true)
                    {
                        var newAmountLog = new LitigationRequestAmountLog
                        {
                            Id = Guid.NewGuid(),
                            LitigationRequestId = litigationRequest.Id,
                            AmountId = (Guid)litigationRequest.LitigationRequestAmountId,
                            CreatedOn = DateTime.Now
                        };
                        await _repository.AddAsync<LitigationRequestAmountLog, Guid>(newAmountLog);
                        var Response = await _repository.SaveChangesAsync();
                    }

                    #endregion
                    return new DataResponse<LitigationCreateUpdateDto>()
                    {
                        Data = model,
                        IsDone = true,
                        Message = "0844444444"
                    };
                }

                return new DataResponse<LitigationCreateUpdateDto>()
                {
                    Data = model,
                    IsDone = true,
                    Message = "0111111111"
                };

            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                    IsDone = false,

                };

            }

        }

        private async Task<string> EnsureUniqueRequestCodeAsync(string requestCode, LitigationRequest litigationRequest)
        {
            var requestCodeExistCheck = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .AsNoTracking()
                .Any(x => x.RequestCode == requestCode);

            if (requestCodeExistCheck)
            {
                var lastRequestCode = (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Where(x => x.RequestCode != null && x.Contract.LitigationRequestContractType == litigationRequest.Contract.LitigationRequestContractType)
                    .AsNoTracking()
                    .OrderByDescending(x => x.RequestCode)
                    .Select(x => x.RequestCode)
                    .FirstOrDefault();

                return IncrementGenericFormatString(lastRequestCode);
            }

            return requestCode;
        }

        private static string IncrementGenericFormatString(string input)
        {

            Match match = Regex.Match(input, @"^([A-Za-z])(\d+)(-.*)$");

            if (match.Success)
            {
                string prefix = match.Groups[1].Value;      // e.g., "B", "N", "X"
                string numberString = match.Groups[2].Value; // e.g., "00007", "12345"
                string suffix = match.Groups[3].Value;     // e.g., "-25", "-XYZ" (includes the dash)

                if (int.TryParse(numberString, out int currentNumber))
                {
                    int nextNumber = currentNumber + 1;

                    string nextNumberString = nextNumber.ToString().PadLeft(numberString.Length, '0');

                    // Reconstruct the string using the captured prefix, the new number, and the original suffix
                    return $"{prefix}{nextNumberString}{suffix}";
                }
            }
            return null; // Format doesn't match or the captured number part is not a valid integer
        }

        public async Task<BaseResponse> ApproveArbitrationFeesPaid(Guid litigationId, CancellationToken cancellationToken)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Where(x => x.Id == litigationId).ToList().FirstOrDefault();

            if (litigationRequest == null)
                return new ExceptionResponse()
                {
                    Exception = "لم يتم العثور على الدعوى",
                    IsDone = false,
                };

            litigationRequest.IsArbitrationFeesHaveBeenCollected = true;
            litigationRequest.ExternalStatusId = RequestStatus.WaitingForArbitratorsSelection;
            litigationRequest.InternalStatusId = RequestStatus.WaitingForArbitratorsSelection;

            return _repository.SaveChangesAsync().Result;
            //if (!saveResponse.IsDone)
            //    return saveResponse;

        }


        public async Task<BaseResponse> SendLitigationFeesRequestPaid(Guid litigationId)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Where(x => x.Id == litigationId).ToList().FirstOrDefault();

                if (litigationRequest == null)
                    return new ExceptionResponse()
                    {
                        Exception = "لم يتم العثور على الدعوى",
                        IsDone = false,
                    };

                litigationRequest.IsLitigationFeesHaveBeenCollected = true;
                litigationRequest.InternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;
                litigationRequest.ExternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;

                var saveResponse = _repository.SaveChangesAsync().Result;
                if (!saveResponse.IsDone)
                    return saveResponse;

                var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                var _stepService = _serviceProvider.GetRequiredService<IWorkFlowStepService>();

                var processingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())
                    .Include(t => t.WorkFlowStep)
                    .Include(t => t.WorkFlowStep.WorkFlowStepEvents)
                    .AsNoTracking()
                    .Where(x => x.RequestId == litigationRequest.Id).ToList();

                if (processingLog.Count > 0)
                {
                    var currentProcessingLog = processingLog.Where(t => t.IsFinished == false).OrderBy(t => t.CreatedOn).ToList();
                    if (processingLog.Count > 0)
                    {
                        ProcessRequestPayload processRequestPayload = new ProcessRequestPayload();
                        processRequestPayload.RequestId = litigationRequest.Id;
                        processRequestPayload.ProcessingLogId = currentProcessingLog.LastOrDefault().Id;
                        processRequestPayload.WorkflowStepEventId = currentProcessingLog.LastOrDefault().WorkFlowStep.WorkFlowStepEvents.FirstOrDefault().Id;
                        processRequestPayload.Notes = "";
                        processRequestPayload.UploadedFilesIds = new List<Guid>();
                        await _bpfService.ProcessWorkFlow(processRequestPayload, default);
                        return new DataResponse<LitigationCreateUpdateDto>()
                        {
                            IsDone = true,
                            Message = ""
                        };
                    }
                }
                return new DataResponse<LitigationCreateUpdateDto>()
                {
                    IsDone = true,
                    Message = ""
                };

            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                    IsDone = false,

                };

            }

        }

        public async Task<BaseResponse> SendReconciliationFeesRequestPaid(Guid litigationId)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Where(x => x.Id == litigationId).ToList().FirstOrDefault();

                if (litigationRequest == null)
                    return new ExceptionResponse()
                    {
                        Exception = "لم يتم العثور على الدعوى",
                        IsDone = false,
                    };

                litigationRequest.IsReconciliationFeesHaveBeenCollected = true;


                var saveResponse = _repository.SaveChangesAsync().Result;
                if (!saveResponse.IsDone)
                    return saveResponse;

                var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                var _stepService = _serviceProvider.GetRequiredService<IWorkFlowStepService>();

                var processingLog = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())
                    .Include(t => t.WorkFlowStep)
                     .Include(t => t.WorkFlowStep.WorkFlowStepEvents)
                .AsNoTracking()
                      .Where(x => x.RequestId == litigationRequest.Id).ToList();

                if (processingLog.Count > 0)
                {
                    var currentProcessingLog = processingLog.Where(t => t.IsFinished == false).OrderBy(t => t.CreatedOn).ToList();
                    if (processingLog.Count > 0)
                    {
                        ProcessRequestPayload processRequestPayload = new ProcessRequestPayload();
                        processRequestPayload.RequestId = litigationRequest.Id;
                        processRequestPayload.ProcessingLogId = currentProcessingLog.LastOrDefault().Id;
                        processRequestPayload.WorkflowStepEventId = currentProcessingLog.LastOrDefault().WorkFlowStep.WorkFlowStepEvents.FirstOrDefault().Id;
                        processRequestPayload.Notes = "";
                        processRequestPayload.UploadedFilesIds = new List<Guid>();
                        await _bpfService.ProcessWorkFlow(processRequestPayload, default);
                        return new DataResponse<LitigationCreateUpdateDto>()
                        {
                            IsDone = true,
                            Message = ""
                        };
                    }
                }
                return new DataResponse<LitigationCreateUpdateDto>()
                {
                    IsDone = true,
                    Message = ""
                };

            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    Exception = ex.Message + "\n" + ex.ToString(),
                    IsDone = false,

                };

            }

        }
        public async Task<Guid> GetAddedLitigation()
        {
            var quarable = await GetQueryableAsync();
            var latestAdded = quarable.OrderByDescending(x => x.CreatedOn).First();
            return latestAdded.Id;
        }

        public async Task<BaseResponse> List(int pageIndex, int pageSize, string? searchKey, LitigationListDto model)
        {
            PagedResult<LitigationRequest> litigationRequests = null;

            var litigations = await GetQueryableAsync();
            litigations = litigations.Include(x => x.Contract);
            litigations = litigations.Include(x => x.Contract.LitigationRequestContractType);
            litigations = litigations.Include(x => x.Contract.FirstPartyCitizen);
            litigations = litigations.Include(x => x.Contract.SecondPartyCitizen);
            litigations = litigations.Include(x => x.Contract.FirstPartyFacility);
            litigations = litigations.Include(x => x.Contract.SecondPartyFacility);
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
                                               //x.Contract.FirstPartyCitizenId == citizenId
                                               //| 
                                               x.Contract.SecondPartyCitizenId == citizenId
                                               & x.InternalStatusId != RequestStatus.SaveAsDraft
                                               & x.InternalStatusId != RequestStatus.New
                                               & x.InternalStatusId != RequestStatus.SentBackForEdit
                                               & x.InternalStatusId != RequestStatus.Edited
                                               & x.InternalStatusId != RequestStatus.Rejected
                                               & x.InternalStatusId != RequestStatus.CollectionOfLitigationFeesRequestIsBeingSent
                                               & x.InternalStatusId != RequestStatus.LitigationFeesHaveBeenCollected
                                               & x.InternalStatusId != RequestStatus.LitigationApprovedCollectionOfReconciliationFeesRequestIsBeingSend
                                               & x.InternalStatusId != RequestStatus.CollectionOfReconciliationFeesRequestIsBeingSent
                                               & x.InternalStatusId != RequestStatus.ReconciliationFeesHaveBeenCollected
                                               | x.ApplicantCitizenId == citizenId)
                                               .OrderByDescending(x => x.CreatedOn)
                                               .AsSplitQuery()
                                               .AsNoTracking()
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

            //var result = await PaginatedList<LitigationListDto>.CreateAsync(list, pageIndex, pageSize);
            //return new DataResponse<PaginatedList<LitigationListDto>>()
            //{
            //    Data = result,
            //    IsDone = true
            //};
        }
        public async Task<BaseResponse> ReformerList(int pageIndex, int pageSize, string? searchKey, LitigationListDto model)
        {

            var litigations = await GetQueryableAsync();
            litigations = litigations.Include(x => x.Contract);
            litigations = litigations.Include(x => x.Contract.LitigationRequestContractType);
            litigations = litigations.Include(x => x.Contract.FirstPartyCitizen);
            litigations = litigations.Include(x => x.Contract.SecondPartyCitizen);
            litigations = litigations.Include(x => x.Contract.FirstPartyFacility);
            litigations = litigations.Include(x => x.Contract.SecondPartyFacility);

            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);
            litigations = litigations.Where(x => x.ReformerCitizenId == citizenId);

            litigations = litigations.OrderByDescending(x => x.CreatedOn);
            var list = _mapper.Map<List<LitigationListDto>>(litigations);



            var result = await PaginatedList<LitigationListDto>.CreateAsync(list, pageIndex, pageSize);
            return new DataResponse<PaginatedList<LitigationListDto>>()
            {
                Data = result,
                IsDone = true
            };
        }


        public async Task<BaseResponse> AuditngList(int pageIndex, int pageSize, string? searchKey, AuditingListDto model)
        {
            //var logdenUser = Guid.Parse("1c1b6018-69c8-43f5-b5b1-72cea64d0f9f");/*_jwtReader.GetUserId();*/
            PagedResult<LitigationRequest> litigationRequests = null;

            var litigations = await GetQueryableAsync();
            litigations = litigations.Include(x => x.Contract);
            litigations = litigations.Include(x => x.Contract.LitigationRequestContractType);
            litigations = litigations.Include(x => x.Contract.FirstPartyCitizen);
            litigations = litigations.Include(x => x.Contract.SecondPartyCitizen);
            litigations = litigations.Include(x => x.Contract.FirstPartyFacility);
            litigations = litigations.Include(x => x.Contract.SecondPartyFacility);
            litigations = litigations.Include(x => x.InternalStatus);



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

            Guid employeeId = Guid.Empty;
            _jwtReader.GetReferenceId(ref employeeId);

            var employee = (await _repository.GetQueryable<SysEmployee, Guid>())?.Where(x => x.Id == employeeId).FirstOrDefault();
            if (employee == null)
            {
                return new BaseResponse() { IsDone = false, Message = "موظف غير معرف" };
            }

            if (!employee.IsAdmin)
            {
                litigations = litigations.Where(x => x.WorkFlowProcessingLogs.Where(t => t.EmployeeId == employeeId).Count() > 0);
            }

            litigations = litigations.OrderByDescending(x => x.CreatedOn);
            litigations = litigations.Where(x => x.LitigationRequestStage == LitigationRequestStage.Review && x.InternalStatusId != RequestStatus.SaveAsDraft && x.InternalStatusId != RequestStatus.AcceptanceOfTheReconciliationReport && x.InternalStatusId != RequestStatus.ReconciliationWasNotMadeAndMoveToArbitration);

            litigationRequests = litigations.OrderByDescending(x => x.CreatedOn)
                                            .AsSplitQuery()
                                            .AsNoTracking()
                                            .GetPaged(pageIndex, pageSize);
            if (litigationRequests is not null)
            {
                int reviewPeriod = 0;
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Review_Period).Result.Data?.Value, out reviewPeriod);


                foreach (var litigation in litigationRequests.Results)
                {
                    int daysPassed = (DateTime.Now - litigation.CreatedOn).Days;

                    litigation.PretenderRating = Math.Max(0, reviewPeriod - daysPassed);
                }
            }

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

        public async Task<BaseResponse> GetLitigationRequestForFullAllReviewer(int pageIndex, int pageSize, CancellationToken cancellationToken)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequest> litigationRequests = null;
            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequest, Guid>())

                                      .Include(x => x.Area)
                                      .Include(x => x.City)
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
                                      .Include(x => x.Contract.SecondPartyCitizen)
                                      .Include(x => x.Contract.LitigationRequestContractType)
                                      .Include(x => x.WorkFlowProcessingLogs.OrderByDescending(c => c.CreatedOn))
                                      .ThenInclude(x => x.Employee);

            litigationRequests = entitiesQuerable.Where(x =>
                x.RequestCode != null & x.LitigationRequestStage == LitigationRequestStage.Review & x.InternalStatusId != RequestStatus.Rejected)

                .OrderByDescending(x => x.CreatedOn)
              .AsSplitQuery()
              .AsNoTracking()
              .GetPaged(pageIndex, pageSize);


            List<LitigationRequestMircoForRequestReviewerDto> litigationRequestMircoDtos = new List<LitigationRequestMircoForRequestReviewerDto>();
            if (litigationRequests != null)
            {

                foreach (var litigationRequest in litigationRequests.Results)
                {
                    if (litigationRequest.Contract.FirstPartyCitizenId == null && litigationRequest.Contract.FirstPartyFacilityId == null)
                    {
                        continue;
                    }
                    LitigationRequestMircoForRequestReviewerDto litigationRequestMircoDto = new LitigationRequestMircoForRequestReviewerDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.CreatedOn;
                    litigationRequestMircoDto.RequestCode = litigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName;
                    litigationRequestMircoDto.RequestTypeName = litigationRequest.Contract.LitigationRequestContractType.TypeName;
                    litigationRequestMircoDto.EmployeeId = litigationRequest.WorkFlowProcessingLogs.First(z => z.EmployeeId != null).EmployeeId;
                    litigationRequestMircoDto.EmployeeName = litigationRequest.WorkFlowProcessingLogs.First(z => z.EmployeeId != null).Employee.EmployeeName;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);
                }
            }

            return new DataResponse<PagedResult<LitigationRequestMircoForRequestReviewerDto>>
            {
                Data =
                   new PagedResult<LitigationRequestMircoForRequestReviewerDto>
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

        public async Task<BaseResponse> GetReviewersLookup(Guid requestId)
        {

            var Reviewers = (await _repository.GetQueryable<SysEmployee, Guid>())
                .Include(x => x.WorkFlowProcessingLogs.Where(y => y.LitigationRequest.IsFinished == false && y.LitigationRequest.LitigationRequestStage == LitigationRequestStage.Review))
                .ThenInclude(x => x.LitigationRequest)
                             .Where(x => x.Department.DepartmentType == DepartmentType.Review & x.IsAdmin != true).ToList();

            List<EmployeeReviewerDto> employeeReviewers = new List<EmployeeReviewerDto>();

            foreach (var reviewr in Reviewers)
            {

                var reqCount = reviewr.WorkFlowProcessingLogs
                    .GroupBy(x => x.LitigationRequest.Id)
                    .Select(x => x.Key)
                    .Count();

                employeeReviewers.Add(new EmployeeReviewerDto()
                {
                    EmployeeName = reviewr.EmployeeName,
                    Id = reviewr.Id,
                    RequestsCount = reqCount
                });

            }


            return new DataResponse<List<EmployeeReviewerDto>>()
            {
                IsDone = true,
                Data = employeeReviewers.ToList()
            };
        }

        public async Task<BaseResponse> Assign(AssignReviewerRequestPayload assignRequestPayload, CancellationToken cancellationToken)
        {
            var msg = "";
            string body;
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.WorkFlowProcessingLogs.Where(z => z.RequestId == assignRequestPayload.RequestId && !z.IsFinished))
                          .FirstOrDefault(x => x.Id == assignRequestPayload.RequestId);
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);
            var deptEmployeeInfoReview = (await _repository.GetQueryable<SysEmployee, Guid>())
                             .Include(e => e.Department)
                             .Where(x => x.Department.DepartmentType == DepartmentType.Review & x.IsAdmin == false && x.Id == litigationRequest.WorkFlowProcessingLogs.First().EmployeeId)
                             .FirstOrDefault();
            var deptNewEmployeeInfoReview = (await _repository.GetQueryable<SysEmployee, Guid>())
                             .Include(e => e.Department)
                             .Where(x => x.Department.DepartmentType == DepartmentType.Review & x.IsAdmin == false & x.Id == assignRequestPayload.EmployeeId)
                             .FirstOrDefault();
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
                var admin = (await _repository.GetQueryable<SysEmployee, Guid>())?
                        .Where(x => x.Id == Guid.Parse("17840AB6-B134-4C5F-9273-5C97345010A3")).SingleOrDefault();

                if (admin == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير معروف"
                    };

                if (!admin.AllowAssign)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "موظف غير مسموح له"
                    };

                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())?
                      .Where(x => x.Id == assignRequestPayload.RequestId).Single();

                if (litigationRequest == null)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "طلب دعوى غير موجود"
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

                if (litigationRequest.LitigationRequestStage != LitigationRequestStage.Review)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "الدعوى ليست بمرحلة التدقيق"
                    };



                if (litigationRequest.WorkFlowProcessingLogs.Any())
                {
                    var WorkFlowProcessingLogs = (await _repository.GetQueryable<WorkFlowProcessingLog, Guid>())?
                      .Where(x => x.RequestId == assignRequestPayload.RequestId);

                    foreach (var request in WorkFlowProcessingLogs.ToList())
                    {
                        if (request.RequestId == assignRequestPayload.RequestId)
                        {
                            request.EmployeeId = assignRequestPayload.EmployeeId;
                        }
                    }

                    foreach (var request in litigationRequest.WorkFlowProcessingLogs.ToList())
                    {
                        if (request.RequestId == assignRequestPayload.RequestId)
                        {
                            request.EmployeeId = assignRequestPayload.EmployeeId;
                        }

                        await _repository.AddAsync<WorkFlowLog, Guid>(new WorkFlowLog()
                        {
                            Id = Guid.NewGuid(),
                            CreatedOn = DateTime.Now,
                            RequestId = litigationRequest.Id,
                            ShowForApplicant = false,
                            LogMessage = "تم إعادة تعيين المدقق " + request.Employee?.EmployeeName
                        });

                        var saveResponse = await _repository.SaveChangesAsync(new CancellationToken());
                        if (!saveResponse.IsDone)
                            return saveResponse;

                        // Send Notification
                        // تم إلغاء المهمة (دعوى تدقيق)
                        if (deptEmployeeInfoReview is not null)
                        {
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Review_Cancelled).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Review_Cancelled).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                }
                                var currentReviewerEmail = deptEmployeeInfoReview.Email;
                                var currentReviewerMobile = deptEmployeeInfoReview.Mobile;


                                await _notificationService.SendNotifications(
                                new SendEmailDto()
                                {
                                    To = new List<string>() { currentReviewerEmail },
                                    Subject = "Review cancelled",
                                    Body = body
                                },
                                new SMSRequestDto()
                                {
                                    message_text = msg,
                                    recipients = new List<string>() { currentReviewerMobile },
                                    notification_type = "SMS",
                                },
                                new List<SysNotificationDto>()
                                {
                               new SysNotificationDto()
                               {
                                    ActorId = deptEmployeeInfoReview.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reviewer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = request.Id,
                                    RequestTypeId = RequestType.LitigationRequestCorrespondence,
                                    Message = msg,
                               },
                                }
                                );
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                        if (deptNewEmployeeInfoReview is not null)
                        {
                            try
                            {
                                msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_NewReviewerEmployee).Result.Data?.Value;
                                msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                                using (StreamReader streamReader = File.OpenText(pathToFile))
                                {
                                    body = await streamReader.ReadToEndAsync();
                                    body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.SLA_AssignLitigation_NewReviewerEmployee).Result.Data?.Value);
                                    body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                                }
                                var newReviewerEmail = deptNewEmployeeInfoReview.Email;
                                var newReviewerMobile = deptNewEmployeeInfoReview.Mobile;


                                await _notificationService.SendNotifications(
                                new SendEmailDto()
                                {
                                    To = new List<string>() { newReviewerEmail },
                                    Subject = "Review cancelled",
                                    Body = body
                                },
                                new SMSRequestDto()
                                {
                                    message_text = msg,
                                    recipients = new List<string>() { newReviewerMobile },
                                    notification_type = "SMS",
                                },
                                new List<SysNotificationDto>()
                                {
                               new SysNotificationDto()
                               {
                                    ActorId = deptNewEmployeeInfoReview.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Reviewer,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = request.Id,
                                    RequestTypeId = RequestType.LitigationRequestCorrespondence,
                                    Message = msg,
                               },

                                }
                                );
                            }
                            catch (Exception ex)
                            {

                            }
                        }


                    }
                }


                // Send Notification
                // تم إرسال مهمة مدقق الى مدقق أخر
                try
                {
                    msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Review_ReAssigned).Result.Data?.Value;
                    msg = msg?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                    using (StreamReader streamReader = File.OpenText(pathToFile))
                    {
                        body = await streamReader.ReadToEndAsync();
                        body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Review_ReAssigned).Result.Data?.Value);
                        body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                    }

                    await _notificationService.SendNotifications(
                           new SendEmailDto()
                           {
                               To = new List<string>() { deptAdminInfoReview.Data.Email },
                               Subject = "Reviewer ReAssigned",
                               Body = msg
                           },
                           new Contracts.Dtos.Integration.GEE.SMSRequestDto()
                           {
                               message_text = msg,
                               recipients = new List<string>() { deptAdminInfoReview.Data.Mobile },
                               notification_type = "SMS",
                           },
                           new List<SysNotificationDto>()
                           {

                               new SysNotificationDto()
                               {
                                    ActorId = deptAdminInfoReview.Data.Id,
                                    CreatedOn = DateTime.Now,
                                    ActorTypeId = NotificationActorType.Employee,
                                    IsViewed = false,
                                    ViewedOn = null,
                                    RequestId = litigationRequest.Id,
                                    RequestTypeId = RequestType.ReformerTaskrequests,
                                    Message = msg,
                               }
                           }
                           );
                }
                catch (Exception ex)
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



        public async Task<BaseResponse> AuditngListArbitration(int pageIndex, int pageSize, string? searchKey, AuditingListDto model)
        {
            //var logdenUser = Guid.Parse("1c1b6018-69c8-43f5-b5b1-72cea64d0f9f");/*_jwtReader.GetUserId();*/

            var litigations = await GetQueryableAsync();
            litigations = litigations.Include(x => x.Contract);
            litigations = litigations.Include(x => x.Contract.LitigationRequestContractType);
            litigations = litigations.Include(x => x.Contract.FirstPartyCitizen);
            litigations = litigations.Include(x => x.Contract.SecondPartyCitizen);
            litigations = litigations.Include(x => x.Contract.FirstPartyFacility);
            litigations = litigations.Include(x => x.Contract.SecondPartyFacility);
            litigations = litigations.Include(x => x.InternalStatus);


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
            litigations = litigations.Where(l => l.LitigationRequestCategoryId == LitigationRequestCategory.Arbitration & l.LitigationRequestStage == LitigationRequestStage.Arbitration | l.LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration & l.LitigationRequestStage == LitigationRequestStage.Arbitration);

            Guid employeeId = Guid.Empty;
            _jwtReader.GetReferenceId(ref employeeId);
            litigations = litigations.Where(x => x.WorkFlowProcessingLogs.Where(t => t.EmployeeId == employeeId).Count() > 0);








            /*   if (model.AuditngRemaningDays.HasValue)
               {
                   litigations = litigations.Where(l => l.AuditngRemaningDays == model.AuditngRemaningDays.Value);
               }
            */
            litigations = litigations.OrderByDescending(x => x.CreatedOn);
            var list = _mapper.Map<List<AuditingListDto>>(litigations).Where(x => x.InternalStatusId != RequestStatus.SaveAsDraft).ToList();
            var result = await PaginatedList<AuditingListDto>.CreateAsync(list, pageIndex, pageSize);
            return new DataResponse<PaginatedList<AuditingListDto>>()
            {
                Data = result,
                IsDone = true
            };

        }
        public async Task<BaseResponse> AuditngListReconciliation(int pageIndex, int pageSize, string? searchKey, AuditingListDto model)
        {
            //var logdenUser = Guid.Parse("1c1b6018-69c8-43f5-b5b1-72cea64d0f9f");/*_jwtReader.GetUserId();*/

            var litigations = await GetQueryableAsync();
            litigations = litigations.Include(x => x.Contract);
            litigations = litigations.Include(x => x.Contract.LitigationRequestContractType);
            litigations = litigations.Include(x => x.Contract.FirstPartyCitizen);
            litigations = litigations.Include(x => x.Contract.SecondPartyCitizen);
            litigations = litigations.Include(x => x.Contract.FirstPartyFacility);
            litigations = litigations.Include(x => x.Contract.SecondPartyFacility);
            litigations = litigations.Include(x => x.InternalStatus);
            litigations = litigations.Include(x => x.WorkFlowProcessingLogs);


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
            litigations = litigations.Where(l => l.LitigationRequestCategoryId == LitigationRequestCategory.Reconciliation & l.LitigationRequestStage != LitigationRequestStage.Review | l.LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration & l.LitigationRequestStage != LitigationRequestStage.Review);

            Guid employeeId = Guid.Empty;
            _jwtReader.GetReferenceId(ref employeeId);
            litigations = litigations.Where(x => x.WorkFlowProcessingLogs.Where(t => t.EmployeeId == employeeId).Count() > 0);








            /*   if (model.AuditngRemaningDays.HasValue)
               {
                   litigations = litigations.Where(l => l.AuditngRemaningDays == model.AuditngRemaningDays.Value);
               }
            */
            litigations = litigations.OrderByDescending(x => x.CreatedOn);
            var list = _mapper.Map<List<AuditingListDto>>(litigations).Where(x => x.InternalStatusId != RequestStatus.SaveAsDraft).ToList();
            var result = await PaginatedList<AuditingListDto>.CreateAsync(list, pageIndex, pageSize);
            return new DataResponse<PaginatedList<AuditingListDto>>()
            {
                Data = result,
                IsDone = true
            };

        }

        public async Task<BaseResponse> Get(Guid litigationId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            if (referenceId == Guid.Empty)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };



            var quarble = await GetQueryableAsync();
            var result = quarble
                .Include(x => x.SysExemptionReductionCategory)
                .Include(x => x.ExemptionReductionFile)
                .Include(x => x.LitigationRequestAmount)
                .Include(x => x.LitigationRequestExtendPeriodRequests)
                .Include(x => x.LitigationRequestSessions)
                .Include(x => x.LitigationRequestReformerAssigningRequests)
                .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                .Include(x => x.Contract)

                .Include(x => x.LitigationRequestCategory)
                .Include(x => x.Area)
                .Include(x => x.City)
                .Include(x => x.ApplicantCitizen)
                .Include(x => x.Contract.SecondPartyCitizen)
                .Include(x => x.Contract.SecondPartyCitizen.Area)
                .Include(x => x.Contract.SecondPartyCitizen.City)
                .Include(x => x.Contract.SecondPartyCitizen.Nationality)
                .Include(x => x.Contract.FirstPartyCitizen)
                .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                .Include(x => x.Contract.FirstPartyFacility)
                .Include(x => x.Contract.SecondPartyFacility)
                .Include(x => x.Contract.ContractFile)
                .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                .Include(x => x.Contract.FoundingLetterFile)
                .Include(x => x.Contract.InternalRegulationsFile)
                .Include(x => x.WorkFlowProcessingLogs)
                .ThenInclude(x => x.Employee)
                .Include(x => x.WorkFlowProcessingLogs.Where(y => !string.IsNullOrEmpty(y.Notes)).OrderByDescending(h => h.CreatedOn))
                .ThenInclude(x => x.WorkFlowProcessingLogFiles).ThenInclude(x => x.FileManager)
                .Include(x => x.LitigationRequestGrounds)
                .ThenInclude(x => x.LitigationRequestGroundFiles)
                .ThenInclude(x => x.FileManager)

                .Include(x => x.LitigationRequestRequests)
                .FirstOrDefault(x => x.Id == litigationId &&
                x.ApplicantCitizenId == referenceId
                      |
                      x.Contract.FirstPartyCitizenId == referenceId & x.Id == litigationId
                      |
                        x.Contract.SecondPartyCitizenId == referenceId & x.Id == litigationId
                      |
                       x.Contract.FirstPartyFacility.CitizenId == referenceId & x.Id == litigationId
                      |
                      x.Contract.SecondPartyFacility.CitizenId == referenceId & x.Id == litigationId
                      | x.LitigationRequestReformerAssigningRequests.Any(l => l.CitizenId == referenceId) & x.Id == litigationId
                      | x.LitigationRequestArbitratorAssigningRequests.Any(l => l.CitizenId == referenceId) & x.Id == litigationId
                );

            if (result is null)
                return new ExceptionResponse { IsDone = false, Message = "غير مصرح بعرض الدعوي" };


            var reconciliationFees = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Fees).Result.Data?.Value;

            result.ReconciliationFees = decimal.Parse(reconciliationFees);

            return new DataResponse<LitigationRequest> { Data = result, IsDone = true };
        }


        public async Task<BaseResponse> GetLitigationByIdForEmployee(Guid litigationId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            if (referenceId == Guid.Empty)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };



            var quarble = await GetQueryableAsync();
            var result = quarble
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
                .Include(x => x.WorkFlowProcessingLogs)
                .ThenInclude(x => x.Employee)
                .Include(x => x.WorkFlowProcessingLogs.Where(y => !string.IsNullOrEmpty(y.Notes)).OrderByDescending(h => h.CreatedOn))
                .ThenInclude(x => x.WorkFlowProcessingLogFiles).ThenInclude(x => x.FileManager)
                .Include(x => x.LitigationRequestGrounds)
                .ThenInclude(x => x.LitigationRequestGroundFiles)
                .ThenInclude(x => x.FileManager)
                .Include(x => x.LitigationRequestRequests)

                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.Citizen)
                 .ThenInclude(x => x.Nationality)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.Facility)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.DelegationFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.DecisionFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.CommercialRegisterationFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.CustodyFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.RepresentationFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.EstablishmentFile)
                .FirstOrDefault(x => x.Id == litigationId);

            return new DataResponse<LitigationRequest> { Data = result, IsDone = true };
        }

        public async Task<BaseResponse> GetLitigationByIdForBackoffice(Guid litigationId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            if (referenceId == Guid.Empty)
                return new BaseResponse() { IsDone = false, Message = "غير مصرح" };



            var quarble = await GetQueryableAsync();
            var result = quarble
                .Include(x => x.SysExemptionReductionCategory)
                .Include(x => x.ExemptionReductionFile)
                .Include(x => x.LitigationRequestAmount)
                .Include(x => x.LitigationRequestExtendPeriodRequests)
                .Include(x => x.LitigationRequestSessions)
                .Include(x => x.LitigationRequestReformerAssigningRequests)
                .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                .Include(x => x.Contract)

                .Include(x => x.LitigationRequestCategory)
                .Include(x => x.Area)
                .Include(x => x.City)
                .Include(x => x.ApplicantCitizen)
                .Include(x => x.Contract.SecondPartyCitizen)
                .Include(x => x.Contract.SecondPartyCitizen.Area)
                .Include(x => x.Contract.SecondPartyCitizen.City)
                .Include(x => x.Contract.SecondPartyCitizen.Nationality)
                .Include(x => x.Contract.FirstPartyCitizen)
                .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                .Include(x => x.Contract.FirstPartyFacility)
                .Include(x => x.Contract.SecondPartyFacility)
                .Include(x => x.Contract.ContractFile)
                .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                .Include(x => x.Contract.FoundingLetterFile)
                .Include(x => x.Contract.InternalRegulationsFile)
                .Include(x => x.WorkFlowProcessingLogs)
                .ThenInclude(x => x.Employee)
                .Include(x => x.WorkFlowProcessingLogs.Where(y => !string.IsNullOrEmpty(y.Notes)).OrderByDescending(h => h.CreatedOn))
                .ThenInclude(x => x.WorkFlowProcessingLogFiles).ThenInclude(x => x.FileManager)
                .Include(x => x.LitigationRequestGrounds)
                .ThenInclude(x => x.LitigationRequestGroundFiles)
                .ThenInclude(x => x.FileManager)

                .Include(x => x.LitigationRequestRequests)
                .FirstOrDefault(x => x.Id == litigationId);


            if (result is null)
                return new ExceptionResponse { IsDone = false, Message = "دعوي غير موجودة" };

            return new DataResponse<LitigationRequest> { Data = result, IsDone = true };
        }

        public async Task<BaseResponse> GetLitigationArbitrationTaskAcceptanceOrRejectDocumentData(Guid litigationId, Guid? requestId)
        {
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.ArbitratorCitizen)
                         .ThenInclude(x => x.SigntureFile)
                         .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                         .ThenInclude(x => x.ArbitratorCitizen)
                         .ThenInclude(x => x.SigntureFile)
                         .Include(x => x.LitigationRequestAmount)
                         .ThenInclude(x => x.LitigationRequestFees)
                         .Include(x => x.InternalStatus)
                         .Include(x => x.ExternalStatus)
                         .Include(x => x.LitigationRequestCategory)
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
                         .FirstOrDefault(l => l.Id == litigationId)
                         ;

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "Not Found" };

            var assigningRequest = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault(l => l.Id == requestId);

            string base64String = string.Empty;
            if (!string.IsNullOrEmpty(assigningRequest?.ArbitratorCitizen?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), assigningRequest?.ArbitratorCitizen?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                base64String = Convert.ToBase64String(fileBytes);
            }
            CultureInfo provider = new CultureInfo("en-US");
            CultureInfo.CurrentCulture = provider;

            var response = new LitigationArbitrationTaskAcceptOrRejectDocumentDto()
            {
                ArbitratorName = assigningRequest.ArbitratorAssigningRequestTypeId ==
                        (ArbitratorAssigningRequestType.ArbitratorsChairman) ? litigationRequest.ArbitratorCitizen?.FullName :
                        assigningRequest.ArbitratorCitizen?.FullName,
                AssociationName = litigationRequest.Contract.AssociationName,
                ContractNumber = litigationRequest.Contract.ContractKey,
                ContractReleaseDate = litigationRequest.CreatedOn.ToString()?.Replace("بعد الهجرة", "هجرياً"),
                LitigationNumber = litigationRequest.RequestCode,
                LitigationType = litigationRequest.Contract.LitigationRequestContractType.TypeName,
                Subject = "100",
                ArbitratorAssignerName = "مركز التحكيم العقاري",
                ArbitratorAssigningDate = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault(l => l.Id == requestId).ResponseOn.ToString(),
                ArbitratorAssigningCost = litigationRequest.LitigationRequestAmount?.LitigationRequestFees.Where(x => x.ArbitratorsCount == litigationRequest.ArbitratorsCount).FirstOrDefault()?.ArbitrationFees != null ? litigationRequest.LitigationRequestAmount?.LitigationRequestFees.Where(x => x.ArbitratorsCount == litigationRequest.ArbitratorsCount).FirstOrDefault()?.ArbitrationFees.ToString("G29") : litigationRequest.LitigationRequestAmount?.Fees.ToString(),
                Response = litigationRequest.LitigationRequestArbitratorAssigningRequests.FirstOrDefault(l => l.Id == requestId).Response,
                RequestTypeId = litigationRequest.Contract.LitigationRequestContractType.Id.GetHashCode(),
                FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName,
                SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName,
                RejectReason = assigningRequest.RejectReason,
                SigntureFile = base64String
            };

            return new DataResponse<LitigationArbitrationTaskAcceptOrRejectDocumentDto> { Data = response, IsDone = true };
        }

        public async Task<BaseResponse> GetLitigationArbitrationTaskAcceptanceOrRejectDocumentData(Guid litigationId)
        {

            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.ArbitratorCitizen)
                         .ThenInclude(x => x.SigntureFile)
                         .Include(x => x.LitigationRequestArbitratorAssigningRequests)
                         .Include(x => x.LitigationRequestAmount)
                         .ThenInclude(x => x.LitigationRequestFees)
                         .Include(x => x.InternalStatus)
                         .Include(x => x.ExternalStatus)
                         .Include(x => x.LitigationRequestCategory)
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
                         .Include(x => x.LitigationRequestInvoice)
                         .FirstOrDefault(l => l.Id == litigationId)
                         ;

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "Not Found" };

            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                .Include(x => x.SigntureFile)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == citizenId);

            string base64String = string.Empty;
            if (!string.IsNullOrEmpty(citizenInfo?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), citizenInfo?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                base64String = Convert.ToBase64String(fileBytes);
            }

            var response = new LitigationArbitrationTaskAcceptOrRejectDocumentDto()
            {
                ArbitratorName = citizenInfo != null ? citizenInfo.FullName : null,
                AssociationName = litigationRequest.Contract.AssociationName,
                ContractNumber = litigationRequest.Contract.ContractKey,
                ContractReleaseDate = litigationRequest.CreatedOn.ToLongDateString()?.Replace("بعد الهجرة", "هجرياً"),
                ArbitratorAssigningDate = litigationRequest.ArbitratorsNominationListSelectionDoneDateTime?.ToLongDateString()?.Replace("بعد الهجرة", "هجرياً"),
                LitigationNumber = litigationRequest.RequestCode,
                LitigationType = litigationRequest.Contract.LitigationRequestContractType.TypeName,
                Subject = "100",
                ArbitratorAssignerName = "مركز التحكيم العقاري",
                ArbitratorAssigningCost = litigationRequest.LitigationRequestAmount?.LitigationRequestFees.Where(x => x.ArbitratorsCount == litigationRequest.ArbitratorsCount).FirstOrDefault()?.ArbitrationFees != null ? litigationRequest.LitigationRequestAmount?.LitigationRequestFees.Where(x => x.ArbitratorsCount == litigationRequest.ArbitratorsCount).FirstOrDefault()?.ArbitrationFees.ToString("G29") : litigationRequest.LitigationRequestAmount?.Fees.ToString(),
                RequestTypeId = litigationRequest.Contract.LitigationRequestContractType.Id.GetHashCode(),
                FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName,
                SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName,
                SigntureFile = base64String

            };
            return new DataResponse<LitigationArbitrationTaskAcceptOrRejectDocumentDto> { Data = response, IsDone = true };
        }
        public async Task<BaseResponse> GetLitigationReconciliationTaskAcceptanceOrRejectDocumentData(Guid litigationId, Guid? requestId)
        {
            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.ReformerCitizen)
                         .ThenInclude(x => x.SigntureFile)
                         .Include(x => x.LitigationRequestReformerAssigningRequests)
                         .ThenInclude(x => x.ReformerCitizen)
                         .Include(x => x.LitigationRequestAmount)
                         .Include(x => x.InternalStatus)
                         .Include(x => x.ExternalStatus)
                         .Include(x => x.LitigationRequestCategory)
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
                         .FirstOrDefault(l => l.Id == litigationId)
                         ;



            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "Not Found" };

            var assigningRequest = litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault(l => l.Id == requestId);

            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                .Include(x => x.SigntureFile)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == assigningRequest.CitizenId);

            string base64String = string.Empty;
            if (!string.IsNullOrEmpty(citizenInfo?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), citizenInfo?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                base64String = Convert.ToBase64String(fileBytes);
            }


            var response = new LitigationReconciliationTaskAcceptOrRejectDocumentDto()
            {
                ReformerName = assigningRequest.ReformerCitizen !=
                        null ? assigningRequest.ReformerCitizen.FullName :
                        litigationRequest.ReformerCitizen?.FullName,
                AssociationName = litigationRequest.Contract.AssociationName,
                ContractNumber = litigationRequest.Contract.ContractKey,
                ContractReleaseDate = litigationRequest.CreatedOn.ToString()?.Replace("بعد الهجرة", "هجرياً"),
                LitigationNumber = litigationRequest.RequestCode,
                LitigationType = litigationRequest.Contract.LitigationRequestContractType.TypeName,
                Subject = "100",
                ReformerAssignerName = "مركز التحكيم العقاري",
                ReformerAssigningDate = litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault(l => l.Id == requestId).ResponseOn.ToString(),
                ReformerAssigningCost = "100",
                Response = litigationRequest.LitigationRequestReformerAssigningRequests.FirstOrDefault(l => l.Id == requestId).Response,
                RequestTypeId = litigationRequest.Contract.LitigationRequestContractType.Id.GetHashCode(),
                FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName,
                SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName,
                RejectReason = assigningRequest.RejectReason,
                SigntureFile = base64String

            };

            return new DataResponse<LitigationReconciliationTaskAcceptOrRejectDocumentDto> { Data = response, IsDone = true };
        }

        public async Task<BaseResponse> GetLitigationReconciliationTaskAcceptanceOrRejectDocumentData(Guid litigationId)
        {
            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);

            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                         .Include(x => x.ArbitratorCitizen)
                         .Include(x => x.LitigationRequestReformerAssigningRequests)
                         .Include(x => x.LitigationRequestAmount)
                         .Include(x => x.InternalStatus)
                         .Include(x => x.ExternalStatus)
                         .Include(x => x.LitigationRequestCategory)
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
                         .FirstOrDefault(l => l.Id == litigationId)
                         ;

            if (litigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "Not Found" };

            var citizenInfo = (await _repository.GetQueryable<Citizen, Guid>())
                .Include(x => x.SigntureFile)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == citizenId);

            string base64String = string.Empty;
            if (!string.IsNullOrEmpty(citizenInfo?.SigntureFile?.FilePath))
            {
                string inputFilePath = Path.Combine(Directory.GetCurrentDirectory(), citizenInfo?.SigntureFile?.FilePath);
                // Read file and convert to Base64
                byte[] fileBytes = File.ReadAllBytes(inputFilePath);
                base64String = Convert.ToBase64String(fileBytes);
            }

            var response = new LitigationReconciliationTaskAcceptOrRejectDocumentDto()
            {
                ReformerName = citizenInfo != null ? citizenInfo.FullName : null,
                AssociationName = litigationRequest.Contract.AssociationName,
                ContractNumber = litigationRequest.Contract.ContractKey,
                ContractReleaseDate = litigationRequest.CreatedOn.ToString()?.Replace("بعد الهجرة", "هجرياً"),
                LitigationNumber = litigationRequest.RequestCode,
                LitigationType = litigationRequest.Contract.LitigationRequestContractType.TypeName,
                Subject = "100",
                ReformerAssignerName = "مركز التحكيم العقاري",
                ReformerAssigningCost = "100",
                RequestTypeId = litigationRequest.Contract.LitigationRequestContractType.Id.GetHashCode(),
                FirstPartyName = (litigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.Contract.FirstPartyCitizen.FullName,
                SecondPartyName = (litigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.Contract.SecondPartyCitizen.FullName,
                SigntureFile = base64String
            };

            return new DataResponse<LitigationReconciliationTaskAcceptOrRejectDocumentDto> { Data = response, IsDone = true };
        }

        public async Task<int> GetLitigationsCountByType(LitigationRequestContractType contractType)
        {
            var quarble = await GetQueryableAsync();
            int count = 0;

            switch (contractType)
            {
                case LitigationRequestContractType.Rent:
                    count = quarble.Where(x => x.RequestCode.StartsWith('R')).Count();
                    break;

                case LitigationRequestContractType.Properties:
                    count = quarble.Where(x => x.RequestCode.StartsWith('O')).Count();
                    break;

                case LitigationRequestContractType.Wafi:
                    count = quarble.Where(x => x.RequestCode[0] == 'W').Count();
                    break;

                case LitigationRequestContractType.RealEstateBrokerage:
                    count = quarble.Where(x => x.RequestCode[0] == 'B').Count();
                    break;

                case LitigationRequestContractType.OtherServices:
                    count = quarble.Where(x => x.RequestCode[0] == 'N').Count();
                    break;
            }
            return count;
        }

        public async Task<BaseResponse> GetLitigationPartiesDetails(Guid litigationId)
        {
            try
            {
                var quarble = await GetQueryableAsync();
                var result = quarble.Include(x => x.Contract)
                    .Include(x => x.LitigationRequestCategory)
                    .Include(x => x.Area)
                    .Include(x => x.City)
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract.SecondPartyCitizen)
                                    .Include(x => x.Contract.SecondPartyCitizen.Nationality)

                    .Include(x => x.Contract.FirstPartyCitizen)
                                    .Include(x => x.Contract.FirstPartyCitizen.Nationality)

                    .Include(x => x.Contract.SecondPartyFacility)
                    .Include(x => x.Contract.FirstPartyFacility)
                    .Include(x => x.Contract.AssociationRegistrationCertificateFile)
                    .Include(x => x.Contract.FoundingLetterFile)
                    .Include(x => x.Contract.InternalRegulationsFile)
                    .Include(x => x.Contract.ContractFile)
                    .Include(x => x.LitigationRequestRequests)
                    .Include(x => x.SysExemptionReductionCategory)
                    .Include(x => x.LitigationRequestGrounds)
                    .ThenInclude(x => x.LitigationRequestGroundFiles)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.Citizen)
                    .ThenInclude(x => x.Nationality)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.Facility)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.DelegationFile)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.DecisionFile)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.CommercialRegisterationFile)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.CustodyFile)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.RepresentationFile)
                    .Include(x => x.Delegator)
                    .ThenInclude(x => x.EstablishmentFile)
                    .FirstOrDefault(x => x.Id == litigationId);

                if (result == null)
                    return new BaseResponse() { IsDone = false, Message = "Not Found" };

                LitigationPartiesDetailsDto litigationPartiesDetailsDto = new LitigationPartiesDetailsDto();
                litigationPartiesDetailsDto.FirstPartyDto = new PartyDTO();
                litigationPartiesDetailsDto.SecondPartyDto = new PartyDTO();
                litigationPartiesDetailsDto.LitigationRequestStatus = result.ExternalStatusId;
                if (result.Contract.FirstPartyCitizenId.HasValue)
                {

                    litigationPartiesDetailsDto.FirstPartyDto.Nationality = result.Contract.FirstPartyCitizen.Nationality.NationalityName;
                    litigationPartiesDetailsDto.FirstPartyDto.FullName = result.Contract.FirstPartyCitizen.FullName;

                    litigationPartiesDetailsDto.FirstPartyDto.Mobile = result.Contract.FirstPartyCitizen.Mobile;
                    litigationPartiesDetailsDto.FirstPartyDto.Email = result.Contract.FirstPartyCitizen.Email;
                    litigationPartiesDetailsDto.FirstPartyDto.FullName = result.Contract.FirstPartyCitizen.FullName;
                    litigationPartiesDetailsDto.FirstPartyDto.AgencyNumber = "1234567890";

                    litigationPartiesDetailsDto.FirstPartyDto.CitizenId = result.Contract.FirstPartyCitizen.Id;
                    litigationPartiesDetailsDto.FirstPartyDto.BirthDate = result.Contract.FirstPartyCitizen.BirthDate;
                    litigationPartiesDetailsDto.FirstPartyDto.IdentityNumber = result.Contract.FirstPartyCitizen.IdentityNumber;
                    litigationPartiesDetailsDto.FirstPartyDto.IsPretender = true;
                    litigationPartiesDetailsDto.FirstPartyDto.Capacity = result.Contract.FirstPartyCapacity;
                    litigationPartiesDetailsDto.FirstPartyDto.NationalAddress = result.Contract.FirstPartyCitizen.NationalAddress;

                    litigationPartiesDetailsDto.FirstPartyDto.AgancyAttachment = result.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    litigationPartiesDetailsDto.FirstPartyDto.LawLicenseAttachment = result.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? ((result.ApplicantCitizen.LawLicenseFile != null) ? new SysFileManagerCreateUpdateDto { Id = result.ApplicantCitizen.LawLicenseFile.Id, FileDescription = result.ApplicantCitizen.LawLicenseFile.FileDescription, FilePath = result.ApplicantCitizen.LawLicenseFile.FilePath, FileType = result.ApplicantCitizen.LawLicenseFile.FileType } : null) : null;
                    litigationPartiesDetailsDto.FirstPartyDto.EstablishmentContract = result.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                }
                if (result.Contract.FirstPartyFacilityId.HasValue)
                {
                    litigationPartiesDetailsDto.FirstPartyDto.IsPretender = true;
                    litigationPartiesDetailsDto.FirstPartyDto.LawLicenseExpirationDate = result.ApplicantCitizen.LawyerLicenseExpirationDate;
                    litigationPartiesDetailsDto.FirstPartyDto.LawLicenseNumber = result.ApplicantCitizen.LawyerLicenseNumber;
                    litigationPartiesDetailsDto.FirstPartyDto.Capacity = result.Contract.FirstPartyCapacity;
                    litigationPartiesDetailsDto.FirstPartyDto.FacilityName = result.Contract?.FirstPartyFacility?.ArabicFacilityName;
                    litigationPartiesDetailsDto.FirstPartyDto.FacilityNumber = result.Contract?.FirstPartyFacility?.Facility700Number;
                    litigationPartiesDetailsDto.FirstPartyDto.CommercialRegistrationNo = result.Contract.FirstPartyFacility.CommercialRegisterNumber;
                    litigationPartiesDetailsDto.FirstPartyDto.AgencyNumber = "1234567890";
                }


                if (result.Contract.SecondPartyCitizenId.HasValue)
                {
                    litigationPartiesDetailsDto.SecondPartyDto.Nationality = result.Contract.SecondPartyCitizen?.Nationality?.NationalityName;
                    litigationPartiesDetailsDto.SecondPartyDto.FullName = result.Contract.SecondPartyCitizen.FullName;

                    litigationPartiesDetailsDto.SecondPartyDto.Mobile = result.Contract.SecondPartyCitizen.Mobile;
                    litigationPartiesDetailsDto.SecondPartyDto.Email = result.Contract.SecondPartyCitizen.Email;
                    litigationPartiesDetailsDto.SecondPartyDto.FullName = result.Contract.SecondPartyCitizen.FullName;
                    litigationPartiesDetailsDto.SecondPartyDto.AgencyNumber = "1234567890";

                    litigationPartiesDetailsDto.SecondPartyDto.CitizenId = result.Contract.SecondPartyCitizen.Id;
                    litigationPartiesDetailsDto.SecondPartyDto.BirthDate = result.Contract.SecondPartyCitizen.BirthDate;
                    litigationPartiesDetailsDto.SecondPartyDto.IdentityNumber = result.Contract.SecondPartyCitizen.IdentityNumber;
                    litigationPartiesDetailsDto.SecondPartyDto.IsPretender = false;
                    litigationPartiesDetailsDto.SecondPartyDto.Capacity = result.Contract.SecondPartyCapacity;
                    litigationPartiesDetailsDto.SecondPartyDto.NationalAddress = result.Contract.SecondPartyCitizen.NationalAddress;

                    litigationPartiesDetailsDto.SecondPartyDto.AgancyAttachment = result.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;
                    litigationPartiesDetailsDto.SecondPartyDto.LawLicenseAttachment = result.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? ((result.ApplicantCitizen.LawLicenseFile != null) ? new SysFileManagerCreateUpdateDto { Id = result.ApplicantCitizen.LawLicenseFile.Id, FileDescription = result.ApplicantCitizen.LawLicenseFile.FileDescription, FilePath = result.ApplicantCitizen.LawLicenseFile.FilePath, FileType = result.ApplicantCitizen.LawLicenseFile.FileType } : null) : null;
                    litigationPartiesDetailsDto.SecondPartyDto.EstablishmentContract = result.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null;

                }
                if (result.Contract.SecondPartyFacilityId.HasValue)
                {
                    litigationPartiesDetailsDto.FirstPartyDto.IsPretender = false;
                    litigationPartiesDetailsDto.SecondPartyDto.LawLicenseExpirationDate = result.ApplicantCitizen.LawyerLicenseExpirationDate;
                    litigationPartiesDetailsDto.SecondPartyDto.LawLicenseNumber = result.ApplicantCitizen.LawyerLicenseNumber;
                    litigationPartiesDetailsDto.SecondPartyDto.Capacity = result.Contract.SecondPartyCapacity;
                    litigationPartiesDetailsDto.SecondPartyDto.FacilityName = result.Contract?.SecondPartyFacility?.ArabicFacilityName;
                    litigationPartiesDetailsDto.SecondPartyDto.FacilityNumber = result.Contract.SecondPartyFacility.Facility700Number;
                    litigationPartiesDetailsDto.SecondPartyDto.CommercialRegistrationNo = result.Contract.SecondPartyFacility.CommercialRegisterNumber;
                    litigationPartiesDetailsDto.SecondPartyDto.AgencyNumber = "1234567890";
                }

                litigationPartiesDetailsDto.Key = result.Contract.ContractKey;
                litigationPartiesDetailsDto.SubKey = result.Contract.VersionNumber;


                litigationPartiesDetailsDto.ContractAddress = result.ContractAddress;
                litigationPartiesDetailsDto.AssociationName = result.Contract.AssociationName;
                litigationPartiesDetailsDto.Id = result.ContractId;

                litigationPartiesDetailsDto.LitigationRequestOwnerType = result.LitigationRequestOwnerTypeId;
                if (result.Contract.ContractFile != null)
                {
                    litigationPartiesDetailsDto.ContractFile = new SysFileManagerCreateUpdateDto()
                    {
                        Id = result.Contract.ContractFile.Id,
                        FileType = result.Contract.ContractFile.FileType,
                        FileDescription = result.Contract.ContractFile.FileDescription,
                        FilePath = result.Contract.ContractFile.FilePath,
                    };
                }
                if (result.Contract.AssociationRegistrationCertificateFile != null)
                {
                    litigationPartiesDetailsDto.AssociationRegistrationCertificateFile = new SysFileManagerCreateUpdateDto()
                    {
                        Id = result.Contract.AssociationRegistrationCertificateFile.Id,
                        FileType = result.Contract.AssociationRegistrationCertificateFile.FileType,
                        FileDescription = result.Contract.AssociationRegistrationCertificateFile.FileDescription,
                        FilePath = result.Contract.AssociationRegistrationCertificateFile.FilePath,
                    };
                }
                if (result.Contract.FoundingLetterFile != null)
                {
                    litigationPartiesDetailsDto.FoundingLetterFile = new SysFileManagerCreateUpdateDto()
                    {
                        Id = result.Contract.FoundingLetterFile.Id,
                        FileType = result.Contract.FoundingLetterFile.FileType,
                        FileDescription = result.Contract.FoundingLetterFile.FileDescription,
                        FilePath = result.Contract.FoundingLetterFile.FilePath,
                    };
                }
                if (result.Contract.InternalRegulationsFile != null)
                {
                    litigationPartiesDetailsDto.InternalRegulationsFile = new SysFileManagerCreateUpdateDto()
                    {
                        Id = result.Contract.InternalRegulationsFile.Id,
                        FileType = result.Contract.InternalRegulationsFile.FileType,
                        FileDescription = result.Contract.InternalRegulationsFile.FileDescription,
                        FilePath = result.Contract.InternalRegulationsFile.FilePath,
                    };
                }


                foreach (var item in result.LitigationRequestRequests)
                {
                    litigationPartiesDetailsDto.Requests.Add(item);
                }
                foreach (var item in result.LitigationRequestGrounds)
                {
                    litigationPartiesDetailsDto.References.Add(item);
                }

                // Delegation Data
                if (result.LitigationRequestOwnerTypeId != LitigationRequestOwnerType.Personal && result.Delegator is not null)
                {
                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto = new LitigationRequestDelegationDataDto();
                    CultureInfo provider = new CultureInfo("en-US");
                    CultureInfo.CurrentCulture = provider;

                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.DelegationNumber = result.Delegator.DelegateNumber;
                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.DelegationFile = _mapper.Map<SysFileManagerCreateUpdateDto>(result.Delegator.DelegationFile);
                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.DecisionFile = _mapper.Map<SysFileManagerCreateUpdateDto>(result.Delegator.DecisionFile);
                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.RepresentationFile = _mapper.Map<SysFileManagerCreateUpdateDto>(result.Delegator.RepresentationFile);
                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.CustodyFile = _mapper.Map<SysFileManagerCreateUpdateDto>(result.Delegator.CustodyFile);
                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.CommercialRegisterationFile = _mapper.Map<SysFileManagerCreateUpdateDto>(result.Delegator.CommercialRegisterationFile);
                    litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.EstablishmentFile = _mapper.Map<SysFileManagerCreateUpdateDto>(result.Delegator.EstablishmentFile);


                    if (result.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual && result.Delegator.Citizen is not null)
                    {

                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.DelegationRelationshipTypeId = DelegationRelationshipType.Individual;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualBirthdate = DateTime.ParseExact(result.Delegator?.Citizen?.BirthDate.ToString(), "M/d/yyyy h:mm:ss tt", new CultureInfo("en-US")).ToString();
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualEmail = result.Delegator.Citizen?.Email;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualCapacity = result.Contract.FirstPartyCapacity;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualFullName = result.Delegator.Citizen.FullName;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualIdentityNumber = result.Delegator.Citizen.IdentityNumber;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualMobile = result.Delegator.Citizen.Mobile;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualNationalAddress = result.Delegator.Citizen.NationalAddress;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.IndividualNationality = result.Delegator.Citizen.Nationality.NationalityName;
                    }
                    else if (result.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Facility && result.Delegator.Facility is not null)
                    {
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.DelegationRelationshipTypeId = DelegationRelationshipType.Facility;

                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.Facility700Number = result.Delegator.Facility.Facility700Number;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.ArabicFacilityName = result.Delegator.Facility.ArabicFacilityName;
                        litigationPartiesDetailsDto.LitigationRequestDelegationDataDto.FacilityCommercialRegisterationNumber = result.Delegator.Facility.CommercialRegisterNumber;
                    }
                }

                return new DataResponse<LitigationPartiesDetailsDto>() { Data = litigationPartiesDetailsDto, IsDone = true };
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
        }

        public async Task<BaseResponse> GetLitigationDetails(Guid litigationId)
        {
            try
            {
                var quarble = await GetQueryableAsync();
                var result = quarble.Include(x => x.Contract)
                    .Include(x => x.LitigationRequestCategory)
                    .Include(x => x.LitigationRequestSessions)

                    .Include(x => x.Area)
                    .Include(x => x.City)
                    .Include(x => x.ExemptionReductionFile)
                    .Include(x => x.ApplicantCitizen)
                    .Include(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.SysExemptionReductionCategory)

                    .Include(x => x.ExemptionReductionFile)
                    .Include(x => x.Contract.FirstPartyCitizen).Include(x => x.LitigationRequestRequests)
                    .Include(x => x.LitigationRequestGrounds).ThenInclude(x => x.LitigationRequestGroundFiles).ThenInclude(x => x.FileManager)
                    .FirstOrDefault(x => x.Id == litigationId);

                var mapped = _mapper.Map<LitigationDetailsDto>(result);
                if (result.LitigationRequestRequests != null)
                {
                    foreach (var item in result.LitigationRequestRequests)
                    {
                        mapped.Requests.Add(new LitigationRequestRequestCreateUpdateDto()
                        {
                            Id = item.Id,
                            RequestContent = item.RequestContent
                        });

                    }
                }
                mapped.References = new List<LitigationRequestGroundCreateUpdateDto>();
                if (result.LitigationRequestGrounds != null)
                {
                    foreach (var item in result.LitigationRequestGrounds)
                    {
                        LitigationRequestGroundCreateUpdateDto litigationRequestGroundCreateUpdateDto = new LitigationRequestGroundCreateUpdateDto();
                        litigationRequestGroundCreateUpdateDto.Id = item.Id;
                        litigationRequestGroundCreateUpdateDto.Title = item.Title;
                        litigationRequestGroundCreateUpdateDto.ReferenceFiles = new List<SysFileManagerCreateUpdateDto>();
                        if (item.LitigationRequestGroundFiles != null)
                        {
                            foreach (var fileItem in item.LitigationRequestGroundFiles)
                            {
                                if (fileItem.FileManager != null)
                                {
                                    litigationRequestGroundCreateUpdateDto.ReferenceFiles.Add(new SysFileManagerCreateUpdateDto()
                                    {
                                        Id = fileItem.FileManager.Id,
                                        FileDescription = fileItem.FileManager.FileDescription,
                                        FileType = fileItem.FileManager.FileType,
                                        FilePath = fileItem.FileManager.FilePath,
                                    });
                                }
                            }
                        }
                        mapped.References.Add(litigationRequestGroundCreateUpdateDto);
                    }
                }
                if (result.SysExemptionReductionCategory != null)
                {
                    mapped.ExemptionReductionCategory = new Contracts.Dtos.System.LitigationRequestCategoryDtos.SysExemptionReductionCategoryCreateUpdateDto()
                    {
                        Id = result.SysExemptionReductionCategory.Id,
                        ExemptionReductionCategoryName = result.SysExemptionReductionCategory.ExemptionReductionCategoryName
                    };
                }

                if (result.ExemptionReductionFile != null)
                {
                    mapped.ExemptionReductionFile = new SysFileManagerCreateUpdateDto()
                    {
                        Id = result.ExemptionReductionFile.Id,
                        FileType = result.ExemptionReductionFile.FileType,
                        FileDescription = result.ExemptionReductionFile.FileDescription,
                        FilePath = result.ExemptionReductionFile.FilePath,
                    };
                }

                return new DataResponse<LitigationDetailsDto> { Data = mapped, IsDone = true };
            }
            catch (Exception ex) { return new ExceptionResponse() { Exception = ex.Message + "\b" + ex.ToString(), IsDone = false }; }
            ;
        }

        public async Task<BaseResponse> CreateLitigationRequest(LitigationRequestDto litigationRequestDto)
        {
            Guid citizenId = Guid.Empty;
            _jwtReader.GetReferenceId(ref citizenId);
            switch (litigationRequestDto.ContractType)
            {
                case LitigationRequestContractType.Rent:
                    return await ValidateRentContract(litigationRequestDto, citizenId);
                case LitigationRequestContractType.Properties:
                    return await ValidatePropertiesContract(litigationRequestDto, citizenId);
                case LitigationRequestContractType.Wafi:
                    return await ValidateWafiContract(litigationRequestDto, citizenId);
                case LitigationRequestContractType.RealEstateBrokerage:
                    return await ValidateRealEstateBrokerageContract(litigationRequestDto, citizenId);
                case LitigationRequestContractType.OtherServices:
                    return await ValidateOtherServicesContract(litigationRequestDto, citizenId);
                default:
                    throw new NotImplementedException();
            }

        }

        public async Task<BaseResponse> ValidateRentContract(LitigationRequestDto litigationRequestDto, Guid userId)
        {
            var contracts = ContractIntegration.GetContracts().Where(t => t.ContractKey == litigationRequestDto.key && t.VersionNumber == litigationRequestDto.SubKey && t.LitigationRequestContractTypeId == LitigationRequestContractType.Rent).ToList();
            if (!contracts.Any())
                return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };

            LitigationRequestContract litigationRequestContract = new LitigationRequestContract()
            {
                Id = Guid.NewGuid(),
                LitigationRequestContractTypeId = LitigationRequestContractType.Rent,
                ContractKey = litigationRequestDto.key,
                VersionNumber = litigationRequestDto.SubKey,

            };
            if (!(await ValidateContractParties(litigationRequestDto, userId, contracts, litigationRequestContract)).IsDone)
                return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };

            LitigationRequest litigationRequest = new LitigationRequest()
            {
                Id = Guid.NewGuid(),
                ContractId = litigationRequestContract.Id,
                ApplicantCitizenId = userId,
                LitigationRequestOwnerTypeId = litigationRequestDto.LitigationRequestOwnerType,
                InternalStatusId = RequestStatus.SaveAsDraft,
                ExternalStatusId = RequestStatus.SaveAsDraft,
                Fees = 0,
                IsPaid = false,
                CreatedOn = DateTime.Now,
                IsSent = false,
                ContractAddress = "شارع القصر - الرياض"
            };
            await _repository.AddAsync<LitigationRequestContract, Guid>(litigationRequestContract);

            await _repository.AddAsync<LitigationRequest, Guid>(litigationRequest);

            var saveResponse = await _repository.SaveChangesAsync();
            if (saveResponse.IsDone)
            {
                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.ApplicantCitizen.LawLicenseFile)
                          .Include(x => x.ApplicantCitizen.IdentityFile)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                          .Include(x => x.Contract.FirstPartyCitizen.FromDelegators)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.Contract.SecondPartyCitizen.Nationality)

                          .Include(x => x.Contract.SecondPartyCitizen.FromDelegators)

                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyFacility.Delegators)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyFacility.Delegators)

                          .AsNoTracking()
                          .Where(x => x.Id == litigationRequest.Id).ToList().FirstOrDefault();

                return await ReturnLitigationRequestDto(litigationRequest);
            }
            else
                return saveResponse;
        }

        private async Task<BaseResponse> ReturnLitigationRequestDto(LitigationRequest litigationRequest, string SecondPartyNationalId = "")
        {

            RentIntegrateDto rentIntegrateDto = new RentIntegrateDto()
            {
                FirstPartyDto = new PartyDTO()
                {

                    IdentityNumber = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.IdentityNumber,
                    NationalAddress = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.NationalAddress,
                    CommercialRegistrationNo = (litigationRequest.Contract.FirstPartyFacility != null) ? litigationRequest.Contract.FirstPartyFacility.CommercialRegisterNumber : null,
                    FacilityName = (litigationRequest.Contract.FirstPartyFacility == null) ? null : litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName,
                    Nationality = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.Nationality?.NationalityName,
                    FullName = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.FullName,
                    Mobile = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.Mobile,
                    Email = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.Email,
                    FacilityNumber = (litigationRequest.Contract.FirstPartyFacility != null) ? litigationRequest.Contract.FirstPartyFacility.Facility700Number : null,
                    CommercialRegisterNumber = (litigationRequest.Contract.FirstPartyFacility != null) ? litigationRequest.Contract.FirstPartyFacility.CommercialRegisterNumber : null,

                    BirthDate = (litigationRequest.Contract.FirstPartyCitizen != null) ? litigationRequest.Contract.FirstPartyCitizen.BirthDate : null,
                    Capacity = litigationRequest.Contract.FirstPartyCapacity,
                    IsPretender = true,
                    LawLicenseNumber = litigationRequest.ApplicantCitizen.LawyerLicenseNumber,
                    LawLicenseExpirationDate = litigationRequest.ApplicantCitizen.LawyerLicenseExpirationDate,
                    AgencyNumber = "1234567890", // (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative) || (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.AgentForIndividual ) ? litigationRequest.Contract. : null,
                    AgancyAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                    LawLicenseAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? ((litigationRequest.ApplicantCitizen.LawLicenseFile != null) ? new SysFileManagerCreateUpdateDto { Id = litigationRequest.ApplicantCitizen.LawLicenseFile.Id, FileDescription = litigationRequest.ApplicantCitizen.LawLicenseFile.FileDescription, FilePath = litigationRequest.ApplicantCitizen.LawLicenseFile.FilePath, FileType = litigationRequest.ApplicantCitizen.LawLicenseFile.FileType } : null) : null,
                    EstablishmentContract = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                },
                SecondPartyDto = new PartyDTO()
                {
                    IdentityNumber = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.IdentityNumber,
                    NationalAddress = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.NationalAddress,
                    CommercialRegistrationNo = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.CommercialRegisterNumber : null,
                    FacilityName = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : null,
                    Nationality = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : (litigationRequest.Contract.SecondPartyCitizen.Nationality == null) ? null : litigationRequest.Contract.SecondPartyCitizen.Nationality.NationalityName,
                    FullName = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.FullName,
                    Mobile = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.Mobile,
                    Email = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.Email,
                    FacilityNumber = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.Facility700Number : null,
                    CommercialRegisterNumber = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.CommercialRegisterNumber : null,

                    BirthDate = (litigationRequest.Contract.SecondPartyCitizen != null) ? litigationRequest.Contract.SecondPartyCitizen.BirthDate : null,
                    Capacity = litigationRequest.Contract.SecondPartyCapacity,
                    IsPretender = false,
                    LawLicenseNumber = litigationRequest.ApplicantCitizen.LawyerLicenseNumber,
                    LawLicenseExpirationDate = litigationRequest.ApplicantCitizen.LawyerLicenseExpirationDate,
                    AgencyNumber = "1234567890", // (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative) || (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.AgentForIndividual ) ? litigationRequest.Contract. : null,
                    AgancyAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                    LawLicenseAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? ((litigationRequest.ApplicantCitizen.LawLicenseFile != null) ? new SysFileManagerCreateUpdateDto { Id = litigationRequest.ApplicantCitizen.LawLicenseFile.Id, FileDescription = litigationRequest.ApplicantCitizen.LawLicenseFile.FileDescription, FilePath = litigationRequest.ApplicantCitizen.LawLicenseFile.FilePath, FileType = litigationRequest.ApplicantCitizen.LawLicenseFile.FileType } : null) : null,
                    EstablishmentContract = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                },
                Key = litigationRequest.Contract.ContractKey,
                SubKey = litigationRequest.Contract.VersionNumber,
                LitigationRequestOwnerType = litigationRequest.LitigationRequestOwnerTypeId,
                ContractAddress = litigationRequest.ContractAddress,
                Id = litigationRequest.ContractId
            };

            if (litigationRequest.Contract.LitigationRequestContractTypeId == LitigationRequestContractType.Wafi)
            {
                //WafiGetUserInfo take wafi number only (show project details by wafi number only)
                var wafiResult = await WafiGetUserInfo(litigationRequest.Contract.ContractKey);

                if (wafiResult is not null)
                {
                    if (wafiResult.IsDone)
                    {
                        rentIntegrateDto.Project = wafiResult.Data;
                    }
                }

            }
            if (litigationRequest.Contract.LitigationRequestContractTypeId == LitigationRequestContractType.Wafi)
            {
                //(GetWafiData) get wafi for defandent
                var wafiData = await GetWafiData(litigationRequest.Contract.ContractKey, SecondPartyNationalId);

                if (wafiData is not null)
                {
                    if (wafiData.IsDone)
                    {
                        rentIntegrateDto.Wafiresponse = wafiData?.Data?.data;
                    }
                }

                //var licenseNumber = litigationRequest.Contract.ContractKey;
                //var identityNumber = citizen.IdentityNumber;

                //DataResponse<WafiDataDto> Wafiresponse = await GetWafiData(licenseNumber, identityNumber);
                //rentIntegrateDto.Wafiresponse = Wafiresponse.Data;

            }
            return new DataResponse<RentIntegrateDto>()
            {
                IsDone = true,
                Data = rentIntegrateDto
            };
        }

        private async Task<BaseResponse> ReturnEjarLitigationRequestDto(LitigationRequest litigationRequest, CreateLitigationRequestNoIntegrationDto litigationRequestNoIntegrationDto, PlaintiffDefendantSharedEjarDto plaintiffDefendantSharedEjarData)
        {

            RentIntegrateDto rentIntegrateDto = new RentIntegrateDto()
            {
                FirstPartyDto = new PartyDTO()
                {

                    IdentityNumber = plaintiffDefendantSharedEjarData.PlaintiffEjarNationalId,
                    NationalAddress = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.NationalAddress,
                    CommercialRegistrationNo = (litigationRequest.Contract.FirstPartyFacility != null) ? litigationRequest.Contract.FirstPartyFacility.CommercialRegisterNumber : null,
                    FacilityName = (litigationRequest.Contract.FirstPartyFacility == null) ? null : litigationRequest.Contract.FirstPartyFacility.ArabicFacilityName,
                    Nationality = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.Nationality.NationalityName,
                    FullName = plaintiffDefendantSharedEjarData.PlaintiffEjarName,
                    Mobile = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.Mobile,
                    Email = (litigationRequest.Contract.FirstPartyCitizen == null) ? null : litigationRequest.Contract.FirstPartyCitizen.Email,
                    FacilityNumber = (litigationRequest.Contract.FirstPartyFacility != null) ? litigationRequest.Contract.FirstPartyFacility.Facility700Number : null,
                    CommercialRegisterNumber = (litigationRequest.Contract.FirstPartyFacility != null) ? litigationRequest.Contract.FirstPartyFacility.CommercialRegisterNumber : null,

                    BirthDate = (litigationRequest.Contract.FirstPartyCitizen != null) ? litigationRequest.Contract.FirstPartyCitizen.BirthDate : null,
                    Capacity = plaintiffDefendantSharedEjarData.PlaintiffEjarCapacity,
                    IsPretender = true,
                    LawLicenseNumber = litigationRequest.ApplicantCitizen.LawyerLicenseNumber,
                    LawLicenseExpirationDate = litigationRequest.ApplicantCitizen.LawyerLicenseExpirationDate,
                    AgencyNumber = "1234567890", // (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative) || (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.AgentForIndividual ) ? litigationRequest.Contract. : null,
                    AgancyAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                    LawLicenseAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? ((litigationRequest.ApplicantCitizen.LawLicenseFile != null) ? new SysFileManagerCreateUpdateDto { Id = litigationRequest.ApplicantCitizen.LawLicenseFile.Id, FileDescription = litigationRequest.ApplicantCitizen.LawLicenseFile.FileDescription, FilePath = litigationRequest.ApplicantCitizen.LawLicenseFile.FilePath, FileType = litigationRequest.ApplicantCitizen.LawLicenseFile.FileType } : null) : null,
                    EstablishmentContract = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                },
                SecondPartyDto = new PartyDTO()
                {
                    IdentityNumber = plaintiffDefendantSharedEjarData.DefendantEjarNationalId,
                    NationalAddress = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.NationalAddress,
                    CommercialRegistrationNo = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.CommercialRegisterNumber : null,
                    FacilityName = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : null,
                    Nationality = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : (litigationRequest.Contract.SecondPartyCitizen.Nationality == null) ? null : litigationRequest.Contract.SecondPartyCitizen.Nationality.NationalityName,
                    FullName = plaintiffDefendantSharedEjarData.DefendantEjarName,
                    Mobile = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.Mobile,
                    Email = (litigationRequest.Contract.SecondPartyCitizen == null) ? null : litigationRequest.Contract.SecondPartyCitizen.Email,
                    FacilityNumber = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.Facility700Number : null,
                    CommercialRegisterNumber = (litigationRequest.Contract.SecondPartyFacility != null) ? litigationRequest.Contract.SecondPartyFacility.CommercialRegisterNumber : null,

                    BirthDate = (litigationRequest.Contract.SecondPartyCitizen != null) ? litigationRequest.Contract.SecondPartyCitizen.BirthDate : null,
                    Capacity = plaintiffDefendantSharedEjarData.DefendantEjarCapacity,
                    IsPretender = false,
                    LawLicenseNumber = litigationRequest.ApplicantCitizen.LawyerLicenseNumber,
                    LawLicenseExpirationDate = litigationRequest.ApplicantCitizen.LawyerLicenseExpirationDate,
                    AgencyNumber = "1234567890", // (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative) || (litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.AgentForIndividual ) ? litigationRequest.Contract. : null,
                    AgancyAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                    LawLicenseAttachment = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.IndividualRepresentative ? ((litigationRequest.ApplicantCitizen.LawLicenseFile != null) ? new SysFileManagerCreateUpdateDto { Id = litigationRequest.ApplicantCitizen.LawLicenseFile.Id, FileDescription = litigationRequest.ApplicantCitizen.LawLicenseFile.FileDescription, FilePath = litigationRequest.ApplicantCitizen.LawLicenseFile.FilePath, FileType = litigationRequest.ApplicantCitizen.LawLicenseFile.FileType } : null) : null,
                    EstablishmentContract = litigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.CompanyRepresentative ? new SysFileManagerCreateUpdateDto { Id = Guid.Parse("e86953a3-c747-49f4-b817-21592334321f"), FileDescription = "FileDescription", FilePath = "reac/e86953a3-c747-49f4-b817-21592334321f.png", FileType = "Pdf" } : null,
                },
                Property = plaintiffDefendantSharedEjarData.EjarProperty,
                Key = litigationRequest.Contract.ContractKey,
                SubKey = litigationRequest.Contract.VersionNumber,
                LitigationRequestOwnerType = litigationRequest.LitigationRequestOwnerTypeId,
                ContractAddress = litigationRequest.ContractAddress,
                Id = litigationRequest.ContractId
            };


            return new DataResponse<RentIntegrateDto>()
            {
                IsDone = true,
                Data = rentIntegrateDto
            };
        }

        private async Task<DataResponse<EjarPartiesDto>> ReturnEjarPlaintiffAndDefendantData(EjarApiResponse input, CreateLitigationRequestNoIntegrationDto litigationRequestNoIntegrationDto, Guid plaintiffReferenceId, string? defendantNationalId = null)
        {
            try
            {
                // For Testing
                var plaintiffNationalId = "101021547";

                if (litigationRequestNoIntegrationDto.FirstPartyIndividualId != Guid.Empty)
                {
                    plaintiffNationalId = (await _repository.GetQueryable<Citizen, Guid>())
                                                .FirstOrDefault(x => x.Id == litigationRequestNoIntegrationDto.FirstPartyIndividualId).IdentityNumber;

                }
                else
                {
                    plaintiffNationalId = (await _repository.GetQueryable<Citizen, Guid>())
                                                   .FirstOrDefault(x => x.Id == plaintiffReferenceId).IdentityNumber;

                }


                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Debug,
                           Message = $"Plaintiff National Id ======> {plaintiffNationalId}"
                       });

                var partiesDto = new EjarPartiesDto();

                foreach (var data in input.Body.Included)
                {
                    if (data.Type == "contract_properties")
                    {
                        partiesDto.Property = new PropertyAttributes()
                        {
                            City = data.Attributes.city,
                            Province = data.Attributes.province,
                            Region = data.Attributes.regions,
                            StreetName = data.Attributes.street_name,
                            BuildingNumber = data.Attributes.building_no,
                            PostCost = data.Attributes.post_cost,
                            AdditionalNumber = data.Attributes.additional_number,
                            Coordiantes = data.Attributes.coordiantes,
                        };
                    }

                    if (data.Type == "lessor_individuals" && data.Attributes.lessor_id_number == plaintiffNationalId)
                    {
                        partiesDto.Plaintiff = new LessorAttributes()
                        {
                            ContractId = data.Attributes.contract_id,
                            PartyRole = data.Attributes.party_role,
                            FullLessorName = data.Attributes.full_lessor_name,
                            LessorIdNumber = data.Attributes.lessor_id_number,
                            LessorIdType = data.Attributes.lessor_id_type,
                            PartyType = data.Attributes.party_type,
                        };


                        partiesDto.Defendant = new TenantAttributes()
                        {
                            ContractId = input.Body.Included.FirstOrDefault(a => a.Type == "tenant_individuals").Attributes.contract_id,
                            PartyRole = input.Body.Included.FirstOrDefault(a => a.Type == "tenant_individuals").Attributes.party_role,
                            FullTenantName = input.Body.Included.FirstOrDefault(a => a.Type == "tenant_individuals").Attributes.full_name_of_tenant,
                            TenantIdNumber = input.Body.Included.FirstOrDefault(a => a.Type == "tenant_individuals").Attributes.tenant_id_number,
                            TenantIdType = input.Body.Included.FirstOrDefault(a => a.Type == "tenant_individuals").Attributes.tenant_id_type,
                            PartyType = input.Body.Included.FirstOrDefault(a => a.Type == "tenant_individuals").Attributes.party_type,
                            TenantDobHijri = input.Body.Included.FirstOrDefault(a => a.Type == "tenant_individuals").Attributes.tenant_dob_hijri,
                        };
                    }
                    else if (data.Type == "tenant_individuals" && data.Attributes.tenant_id_number == plaintiffNationalId)
                    {

                        partiesDto.Plaintiff = new TenantAttributes()
                        {
                            ContractId = data.Attributes.contract_id,
                            PartyRole = data.Attributes.party_role,
                            FullTenantName = data.Attributes.full_name_of_tenant,
                            TenantIdNumber = data.Attributes.tenant_id_number,
                            TenantIdType = data.Attributes.tenant_id_type,
                            PartyType = data.Attributes.party_type,
                            TenantDobHijri = data.Attributes.tenant_dob_hijri,
                        }; ;

                        if (string.IsNullOrEmpty(defendantNationalId))
                        {

                            partiesDto.Defendant = new LessorAttributes()
                            {
                                ContractId = input.Body.Included.FirstOrDefault(a => a.Type == "lessor_individuals").Attributes.contract_id,
                                PartyRole = input.Body.Included.FirstOrDefault(a => a.Type == "lessor_individuals").Attributes.party_role,
                                FullLessorName = input.Body.Included.FirstOrDefault(a => a.Type == "lessor_individuals").Attributes.full_lessor_name,
                                LessorIdNumber = input.Body.Included.FirstOrDefault(a => a.Type == "lessor_individuals").Attributes.lessor_id_number,
                                LessorIdType = input.Body.Included.FirstOrDefault(a => a.Type == "lessor_individuals").Attributes.lessor_id_type,
                                PartyType = input.Body.Included.FirstOrDefault(a => a.Type == "lessor_individuals").Attributes.party_type,
                            };
                        }
                        else
                        {
                            partiesDto.Defendant = new LessorAttributes()
                            {
                                ContractId = input.Body.Included.Where(a => a.Type == "lessor_individuals" && a.Attributes.lessor_id_number == defendantNationalId).Select(l => l.Attributes.contract_id.Value).Distinct().First(),
                                PartyRole = input.Body.Included.Where(a => a.Type == "lessor_individuals" && a.Attributes.lessor_id_number == defendantNationalId).Select(l => l.Attributes.party_role).Distinct().First(),
                                FullLessorName = input.Body.Included.Where(a => a.Type == "lessor_individuals" && a.Attributes.lessor_id_number == defendantNationalId).Select(l => l.Attributes.full_lessor_name).Distinct().First(),
                                LessorIdNumber = input.Body.Included.Where(a => a.Type == "lessor_individuals" && a.Attributes.lessor_id_number == defendantNationalId).Select(l => l.Attributes.lessor_id_number).Distinct().First(),
                                LessorIdType = input.Body.Included.Where(a => a.Type == "lessor_individuals" && a.Attributes.lessor_id_number == defendantNationalId).Select(l => l.Attributes.lessor_id_type).Distinct().First(),
                                PartyType = input.Body.Included.Where(a => a.Type == "lessor_individuals" && a.Attributes.lessor_id_number == defendantNationalId).Select(l => l.Attributes.party_type).Distinct().First(),
                            };
                        }

                    }
                }

                if (partiesDto.Plaintiff is null)
                    return new DataResponse<EjarPartiesDto>() { IsDone = false, Message = "مقدم الطلب ليس طرفا من اطراف العقد" };



                if (partiesDto.Plaintiff is null || partiesDto.Property is null)
                    return new DataResponse<EjarPartiesDto>() { IsDone = false, Message = "خطأ في استخراج بيانات ايجار" };


                return new DataResponse<EjarPartiesDto>() { IsDone = true, Data = partiesDto };
            }
            catch (Exception ex)
            {
                return new DataResponse<EjarPartiesDto>() { IsDone = false, Message = "خطأ داخلي في استخراج بيانات ايجار" };
            }

        }

        private async Task<DataResponse<MultipleLessors>> CatchMultipleLessors(EjarApiResponse input, CreateLitigationRequestNoIntegrationDto litigationRequestNoIntegrationDto, Guid plaintiffReferenceId)
        {
            var plaintiffNationalId = "";

            if (litigationRequestNoIntegrationDto.FirstPartyIndividualId != Guid.Empty)
            {
                plaintiffNationalId = (await _repository.GetQueryable<Citizen, Guid>())
                                            .FirstOrDefault(x => x.Id == litigationRequestNoIntegrationDto.FirstPartyIndividualId).IdentityNumber;

            }
            else
            {
                plaintiffNationalId = (await _repository.GetQueryable<Citizen, Guid>())
                                               .FirstOrDefault(x => x.Id == plaintiffReferenceId).IdentityNumber;

            }

            foreach (var data in input.Body.Included.Where(x => x.Type == "tenant_individuals"))
            {

                if (data.Attributes.tenant_id_number == plaintiffNationalId)
                {
                    var lessorsCount = input.Body.Included
                            .Where(l => l.Type == "lessor_individuals" && l.Attributes.party_role.en == "Lessor")
                            .Select(l => l.Attributes.lessor_id_number)
                            .Distinct()
                            .Count();

                    if (lessorsCount > 1)
                    {
                        var lessors = input.Body.Included
                            .Where(l => l.Type == "lessor_individuals" && l.Attributes.party_role.en == "Lessor")
                            .GroupBy(l => l.Attributes.lessor_id_number)
                            .Select(g => g.First())
                            .ToList();

                        var lessorsDto = new MultipleLessors();

                        var lessorsList = new List<LessorAttributes>();

                        foreach (var lessor in lessors)
                        {
                            lessorsList.Add(new LessorAttributes
                            {
                                FullLessorName = lessor.Attributes.full_lessor_name.Value,
                                LessorIdNumber = lessor.Attributes.lessor_id_number.Value,
                            });

                        }

                        lessorsDto.Lessors = lessorsList;
                        lessorsDto.Multiple = true;

                        return new DataResponse<MultipleLessors>() { IsDone = false, Message = "برجاء اختيار المدعي عليه", Data = lessorsDto };
                    }


                }


            }
            return new DataResponse<MultipleLessors>() { IsDone = true };
        }

        private async Task<DataResponse<Data>> WafiGetUserInfo(string licenseNumber)
        {
            try
            {
                var token = (await _gEELoginService.Login()).Data?.Access_token;
                if (token is null)
                    _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                           Message = "get access token from API GEE error"
                       });

                var url = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Wafi_URL).Result.Data?.Value;

                if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(url))
                    return new DataResponse<Data>() { IsDone = false, Message = "" };

                url = string.Format(url, licenseNumber);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("Authorization", $"Bearer {token}");
                var response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    var userInfo = JsonSerializer.Deserialize<WafiProjectDetails>(content);

                    return new DataResponse<Data> { IsDone = true, Data = userInfo.data };
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                    return new DataResponse<Data> { IsDone = false, Message = "" };
                else
                    return new DataResponse<Data> { IsDone = false, Message = "" };
            }
            catch (Exception e)
            {
                return new DataResponse<Data>() { IsDone = false, Message = e.Message + e.InnerException };
            }
        }

        private async Task<BaseResponse> ValidateContractParties(LitigationRequestDto litigationRequestDto, Guid userId, List<Contract> contracts, LitigationRequestContract litigationRequestContract)
        {
            switch (litigationRequestDto.LitigationRequestOwnerType)
            {
                case LitigationRequestOwnerType.Personal:

                    if (contracts.FirstOrDefault()?.FirstPartyCitizenId == userId)
                    {
                        litigationRequestContract.FirstPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                        litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;
                        litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                        if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                            litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                        else
                            litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;
                    }
                    else
                    {
                        if (contracts.FirstOrDefault()?.SecondPartyCitizenId == userId)
                        {
                            litigationRequestContract.FirstPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                            litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;
                            litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;
                            if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                            else
                                litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                        }
                        else
                            return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };
                    }
                    break;
                case LitigationRequestOwnerType.AgentForIndividual:
                    bool goOn1 = true;
                    if (contracts.FirstOrDefault().FirstPartyCitizenId.HasValue)
                    {
                        var firstCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                            .Include(x => x.FromDelegators)
                            .AsNoTracking()
                            .Where(x => x.Id == contracts.FirstOrDefault().FirstPartyCitizenId).ToList();
                        foreach (var delegator in firstCitizen.FirstOrDefault().FromDelegators)
                        {
                            if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                            {
                                litigationRequestContract.FirstPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                                litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;
                                litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                                if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                    litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                                else
                                    litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;

                                goOn1 = false;
                            }
                        }

                    }
                    if (goOn1)
                    {
                        if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                        {
                            var secondCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                            .Include(x => x.FromDelegators)
                            .AsNoTracking()
                            .Where(x => x.Id == contracts.FirstOrDefault().SecondPartyCitizenId).ToList();

                            foreach (var delegator in secondCitizen.FirstOrDefault().FromDelegators)
                            {
                                if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                                {
                                    litigationRequestContract.FirstPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                                    litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;
                                    litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                                    if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                        litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                                    else
                                        litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;
                                }
                            }
                        }
                    }
                    if (goOn1)
                    {
                        if (contracts.FirstOrDefault().FirstPartyFacilityId.HasValue)
                        {
                            var firstFacility = (await _repository.GetQueryable<Facility, Guid>())
                                .Include(x => x.Delegators)
                                .AsNoTracking()
                                .Where(x => x.Id == contracts.FirstOrDefault().FirstPartyFacilityId).ToList();

                            foreach (var delegator in firstFacility.FirstOrDefault().Delegators)
                            {
                                if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                                {
                                    litigationRequestContract.FirstPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;
                                    litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;
                                    litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                                    if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                        litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                                    else
                                        litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;

                                    goOn1 = false;
                                }
                            }

                        }
                    }
                    if (goOn1)
                    {
                        if (contracts.FirstOrDefault().SecondPartyFacilityId.HasValue)
                        {
                            var secondFacility = (await _repository.GetQueryable<Facility, Guid>())
                                .Include(x => x.Delegators)
                                .AsNoTracking()
                                .Where(x => x.Id == contracts.FirstOrDefault().SecondPartyFacilityId).ToList();

                            foreach (var delegator in secondFacility.FirstOrDefault().Delegators)
                            {
                                if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                                {
                                    litigationRequestContract.FirstPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;
                                    litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;
                                    litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;

                                    if (contracts.FirstOrDefault().FirstPartyCitizenId.HasValue)
                                        litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                                    else
                                        litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;

                                    goOn1 = false;
                                }
                            }
                        }
                    }
                    if (goOn1)
                        return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };
                    break;
                case LitigationRequestOwnerType.IndividualRepresentative:
                    bool goOn2 = true;
                    if (contracts.FirstOrDefault().FirstPartyCitizenId.HasValue)
                    {
                        var firstCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                            .Include(x => x.FromDelegators)
                            .AsNoTracking()
                            .Where(x => x.Id == contracts.FirstOrDefault().FirstPartyCitizenId).ToList();
                        foreach (var delegator in firstCitizen.FirstOrDefault().FromDelegators)
                        {
                            if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                            {
                                litigationRequestContract.FirstPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                                litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;
                                litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                                if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                    litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                                else
                                    litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;

                                goOn2 = false;
                            }
                        }

                    }
                    if (goOn2)
                    {
                        if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                        {
                            var secondCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                            .Include(x => x.FromDelegators)
                            .AsNoTracking()
                            .Where(x => x.Id == contracts.FirstOrDefault().SecondPartyCitizenId).ToList();

                            foreach (var delegator in secondCitizen.FirstOrDefault().FromDelegators)
                            {
                                if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                                {
                                    litigationRequestContract.FirstPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                                    litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;
                                    litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                                    if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                        litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                                    else
                                        litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;

                                    goOn2 = false;
                                }
                            }
                        }
                    }
                    if (goOn2)
                    {
                        if (contracts.FirstOrDefault().FirstPartyFacilityId.HasValue)
                        {
                            var firstFacility = (await _repository.GetQueryable<Facility, Guid>())
                                .Include(x => x.Delegators)
                                .AsNoTracking()
                                .Where(x => x.Id == contracts.FirstOrDefault().FirstPartyFacilityId).ToList();

                            foreach (var delegator in firstFacility.FirstOrDefault().Delegators)
                            {
                                if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                                {
                                    litigationRequestContract.FirstPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;
                                    litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;
                                    litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                                    if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                        litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                                    else
                                        litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;

                                    goOn2 = false;
                                }
                            }

                        }
                    }
                    if (goOn2)
                    {
                        if (contracts.FirstOrDefault().SecondPartyFacilityId.HasValue)
                        {
                            var secondFacility = (await _repository.GetQueryable<Facility, Guid>())
                                .Include(x => x.Delegators)
                                .AsNoTracking()
                                .Where(x => x.Id == contracts.FirstOrDefault().SecondPartyFacilityId).ToList();

                            foreach (var delegator in secondFacility.FirstOrDefault().Delegators)
                            {
                                if (delegator.DelegatorCitizenId == userId && delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                                {
                                    litigationRequestContract.FirstPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;
                                    litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;
                                    litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;

                                    if (contracts.FirstOrDefault().FirstPartyCitizenId.HasValue)
                                        litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                                    else
                                        litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;

                                    goOn1 = false;
                                }
                            }
                        }
                    }
                    if (goOn2)
                        return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };
                    break;
                case LitigationRequestOwnerType.CompanyRepresentative:
                    bool goOn3 = true;
                    if (contracts.FirstOrDefault().FirstPartyFacilityId.HasValue)
                    {
                        var firstFacility = (await _repository.GetQueryable<Facility, Guid>())
                            .AsNoTracking()
                            .Where(x => x.Id == contracts.FirstOrDefault().FirstPartyFacilityId).ToList();

                        if (firstFacility.FirstOrDefault().CitizenId == userId)
                        {
                            litigationRequestContract.FirstPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;
                            litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;
                            litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;

                            if (contracts.FirstOrDefault().SecondPartyCitizenId.HasValue)
                                litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.SecondPartyCitizenId;
                            else
                                litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;

                            goOn3 = false;
                        }

                    }
                    if (goOn3)
                    {
                        if (contracts.FirstOrDefault().SecondPartyFacilityId.HasValue)
                        {
                            var secondFacility = (await _repository.GetQueryable<Facility, Guid>())
                                .Include(x => x.Delegators)
                                .AsNoTracking()
                                .Where(x => x.Id == contracts.FirstOrDefault().SecondPartyFacilityId).ToList();

                            foreach (var delegator in secondFacility.FirstOrDefault().Delegators)
                            {
                                if (secondFacility.FirstOrDefault()?.CitizenId == userId)
                                {
                                    litigationRequestContract.FirstPartyFacilityId = contracts.FirstOrDefault()?.SecondPartyFacilityId;
                                    litigationRequestContract.FirstPartyCapacity = contracts.FirstOrDefault()?.SecondPartyCapacity;
                                    litigationRequestContract.SecondPartyCapacity = contracts.FirstOrDefault()?.FirstPartyCapacity;

                                    if (contracts.FirstOrDefault().FirstPartyCitizenId.HasValue)
                                        litigationRequestContract.SecondPartyCitizenId = contracts.FirstOrDefault()?.FirstPartyCitizenId;
                                    else
                                        litigationRequestContract.SecondPartyFacilityId = contracts.FirstOrDefault()?.FirstPartyFacilityId;

                                    goOn3 = false;
                                }
                            }
                        }
                    }
                    if (goOn3)
                        return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };
                    break;
                default:
                    return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };
            }
            return new BaseResponse() { IsDone = true };

        }

        public async Task<BaseResponse> ValidatePropertiesContract(LitigationRequestDto litigationRequestDto, Guid userId)
        {
            var contracts = ContractIntegration.GetContracts().Where(t => t.ContractKey == litigationRequestDto.key &&
                    t.LitigationRequestContractTypeId == LitigationRequestContractType.Properties).ToList();
            if (!contracts.Any())
                return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };

            LitigationRequestContract litigationRequestContract = new LitigationRequestContract()
            {
                Id = Guid.NewGuid(),
                LitigationRequestContractTypeId = LitigationRequestContractType.Properties,
                ContractKey = litigationRequestDto.key,
                VersionNumber = litigationRequestDto.SubKey
            };
            await ValidateContractParties(litigationRequestDto, userId, contracts, litigationRequestContract);

            LitigationRequest litigationRequest = new LitigationRequest()
            {
                Id = Guid.NewGuid(),
                ContractId = litigationRequestContract.Id,
                ApplicantCitizenId = userId,
                LitigationRequestOwnerTypeId = litigationRequestDto.LitigationRequestOwnerType,
                InternalStatusId = RequestStatus.SaveAsDraft,
                ExternalStatusId = RequestStatus.SaveAsDraft,
                Fees = 0,
                IsPaid = false,
                CreatedOn = DateTime.Now,
                IsSent = false,
                IsAllowEdit = true,
                IsActive = true,
                IsFinished = false,
                ContractAddress = "شارع القصر - الرياض"
            };
            await _repository.AddAsync<LitigationRequestContract, Guid>(litigationRequestContract);

            await _repository.AddAsync<LitigationRequest, Guid>(litigationRequest);

            var saveResponse = await _repository.SaveChangesAsync();
            if (saveResponse.IsDone)
            {
                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.ApplicantCitizen.LawLicenseFile)
                          .Include(x => x.ApplicantCitizen.IdentityFile)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                          .Include(x => x.Contract.FirstPartyCitizen.FromDelegators)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.Contract.SecondPartyCitizen.Nationality)

                          .Include(x => x.Contract.SecondPartyCitizen.FromDelegators)

                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyFacility.Delegators)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyFacility.Delegators)

                          .AsNoTracking()
                          .Where(x => x.Id == litigationRequest.Id).ToList().FirstOrDefault();

                return await ReturnLitigationRequestDto(litigationRequest);
            }
            else
                return saveResponse;
        }

        public async Task<BaseResponse> ValidateWafiContract(LitigationRequestDto litigationRequestDto, Guid userId)
        {
            var contracts = ContractIntegration.GetContracts().Where(t => t.ContractKey == litigationRequestDto.key && t.LitigationRequestContractTypeId == LitigationRequestContractType.Wafi).ToList();
            if (!contracts.Any())
                return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };

            LitigationRequestContract litigationRequestContract = new LitigationRequestContract()
            {
                Id = Guid.NewGuid(),
                LitigationRequestContractTypeId = LitigationRequestContractType.Wafi,
                ContractKey = litigationRequestDto.key,
                VersionNumber = litigationRequestDto.SubKey
            };
            var validationResponse =
                await ValidateContractParties(litigationRequestDto, userId, contracts, litigationRequestContract);

            if (!validationResponse.IsDone)
                return validationResponse;

            LitigationRequest litigationRequest = new LitigationRequest()
            {
                Id = Guid.NewGuid(),
                ContractId = litigationRequestContract.Id,
                ApplicantCitizenId = userId,
                LitigationRequestOwnerTypeId = litigationRequestDto.LitigationRequestOwnerType,
                InternalStatusId = RequestStatus.SaveAsDraft,
                ExternalStatusId = RequestStatus.SaveAsDraft,
                Fees = 0,
                IsPaid = false,
                CreatedOn = DateTime.Now,
                IsSent = false,
                IsAllowEdit = true,
                IsActive = true,
                IsFinished = false,
                ContractAddress = "شارع القصر - الرياض"
            };
            await _repository.AddAsync<LitigationRequestContract, Guid>(litigationRequestContract);

            await _repository.AddAsync<LitigationRequest, Guid>(litigationRequest);

            var saveResponse = await _repository.SaveChangesAsync();
            if (saveResponse.IsDone)
            {
                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.ApplicantCitizen.LawLicenseFile)
                          .Include(x => x.ApplicantCitizen.IdentityFile)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                          .Include(x => x.Contract.FirstPartyCitizen.FromDelegators)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.Contract.SecondPartyCitizen.Nationality)

                          .Include(x => x.Contract.SecondPartyCitizen.FromDelegators)

                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyFacility.Delegators)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyFacility.Delegators)

                          .AsNoTracking()
                          .Where(x => x.Id == litigationRequest.Id).ToList().FirstOrDefault();

                return await ReturnLitigationRequestDto(litigationRequest);
            }
            else
                return saveResponse;
        }

        public async Task<BaseResponse> ValidateRealEstateBrokerageContract(LitigationRequestDto litigationRequestDto, Guid userId)
        {
            var contracts = ContractIntegration.GetContracts().Where(t => t.ContractKey == litigationRequestDto.key &&
                                t.LitigationRequestContractTypeId == LitigationRequestContractType.RealEstateBrokerage).ToList();
            if (!contracts.Any())
                return new BaseResponse() { IsDone = false, Message = "بيانات العقد غير صحيحة" };

            LitigationRequestContract litigationRequestContract = new LitigationRequestContract()
            {
                Id = Guid.NewGuid(),
                LitigationRequestContractTypeId = LitigationRequestContractType.RealEstateBrokerage,
                ContractKey = litigationRequestDto.key,
                VersionNumber = litigationRequestDto.SubKey
            };
            await ValidateContractParties(litigationRequestDto, userId, contracts, litigationRequestContract);

            LitigationRequest litigationRequest = new LitigationRequest()
            {
                Id = Guid.NewGuid(),
                ContractId = litigationRequestContract.Id,
                ApplicantCitizenId = userId,
                LitigationRequestOwnerTypeId = litigationRequestDto.LitigationRequestOwnerType,
                InternalStatusId = RequestStatus.SaveAsDraft,
                ExternalStatusId = RequestStatus.SaveAsDraft,
                Fees = 0,
                IsPaid = false,
                CreatedOn = DateTime.Now,
                IsSent = false,
                IsAllowEdit = true,
                IsActive = true,
                IsFinished = false,
                ContractAddress = "شارع القصر - الرياض"
            };
            await _repository.AddAsync<LitigationRequestContract, Guid>(litigationRequestContract);

            await _repository.AddAsync<LitigationRequest, Guid>(litigationRequest);

            var saveResponse = await _repository.SaveChangesAsync();
            if (saveResponse.IsDone)
            {
                litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.ApplicantCitizen)
                          .Include(x => x.ApplicantCitizen.LawLicenseFile)
                          .Include(x => x.ApplicantCitizen.IdentityFile)
                          .Include(x => x.Contract)
                          .Include(x => x.Contract.FirstPartyCitizen)
                          .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                          .Include(x => x.Contract.FirstPartyCitizen.FromDelegators)
                          .Include(x => x.Contract.SecondPartyCitizen)
                          .Include(x => x.Contract.SecondPartyCitizen.Nationality)

                          .Include(x => x.Contract.SecondPartyCitizen.FromDelegators)

                          .Include(x => x.Contract.FirstPartyFacility)
                          .Include(x => x.Contract.FirstPartyFacility.Delegators)
                          .Include(x => x.Contract.SecondPartyFacility)
                          .Include(x => x.Contract.SecondPartyFacility.Delegators)

                          .AsNoTracking()
                          .Where(x => x.Id == litigationRequest.Id).ToList().FirstOrDefault();

                return await ReturnLitigationRequestDto(litigationRequest);
            }
            else
                return saveResponse;
        }

        public async Task<BaseResponse> ValidateOtherServicesContract(LitigationRequestDto litigationRequestDto, Guid userId)
        {
            throw new NotImplementedException();
        }

        private async Task<BaseResponse> ValidateDelegatorData(CreateLitigationRequestNoIntegrationDto litigationRequestDto)
        {
            var existLitigationRequestDelegationData = (await _repository.GetQueryable<LitigationRequest, Guid>()).
                    Include(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.Contract.SecondPartyFacility)
                    .Include(x => x.Delegator.Citizen)
                    .Include(x => x.Delegator.Facility)
                    .Any(x => x.IsFinished != true && (x.ExternalStatusId != RequestStatus.Cancelled && x.InternalStatusId != RequestStatus.Cancelled || x.ExternalStatusId != RequestStatus.Rejected && x.InternalStatusId != RequestStatus.Rejected)
                    && x.Contract.ContractKey == litigationRequestDto.key
                    && (x.Contract.SecondPartyCitizen.IdentityNumber == litigationRequestDto.SecondPartyNationalId || x.Contract.SecondPartyFacility.CommercialRegisterNumber == litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo)
                    && (x.Delegator.Citizen != null && x.Delegator.Citizen.IdentityNumber == litigationRequestDto.litigationRequestDelegationDataDto.IndividualIdentityNumber)
                    || (x.Delegator.Facility != null && x.Delegator.Facility.Facility700Number == litigationRequestDto.litigationRequestDelegationDataDto.Facility700Number)
                    );

            if (existLitigationRequestDelegationData)
            {
                return new BaseResponse()
                {
                    IsDone = false,
                    Message = "يوجد دعوى قائمة بالفعل بنفس رقم الجمعيه وهوية المدعي و هوية المدعي عليه"
                };
            }

            return new BaseResponse()
            {
                IsDone = true,
                Message = "تم التحقق من بيانات التفويض بنجاح"
            };
        }

        private async Task<DataResponse<Delegator>> AddDelegationData(LitigationRequestDelegationDataDto input, LitigationRequest litigationRequest)
        {

            if (input == null)
                return new DataResponse<Delegator> { IsDone = false, Message = "خطأ في إضافة بيانات التفويض" };

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            if (input.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
            {
                var addDraftCitizen = await AddDraftCitizen(input);

                if (!addDraftCitizen.IsDone)
                    return new DataResponse<Delegator> { IsDone = false, Message = "خطأ في إضافة بيانات المدعي" };

                var delegator = new Delegator()
                {
                    Id = Guid.NewGuid(),
                    DelegatorCitizenId = referenceId,
                    CitizenId = addDraftCitizen.Data?.Id,
                    DelegateNumber = input.DelegationNumber,
                    DelegationFileId = input.DelegationFileId,
                    DelegationRelationshipTypeId = DelegationRelationshipType.Individual,
                    CommercialRegisterationFileId = input.CommercialRegisterationFileId,
                    CustodyFileId = input.CustodyFileId,
                    RepresentationFileId = input.RepresentationFileId,
                    DecisionFileId = input.DecisionFileId,
                    EstablishmentFileId = input.EstablishmentFileId,
                    IsActive = true
                };

                var addDelegator = await _repository.AddAsync<Delegator, Guid>(delegator);


                if (!addDelegator.IsDone)
                    return new DataResponse<Delegator> { IsDone = false, Message = "خطأ في إضافة بيانات التفويض للفرد" };

                var delegationRequest = new DelegationRequest()
                {
                    Id = Guid.NewGuid(),
                    DelegationParty = LitigationRequestParty.Plaintiff,
                    DelegatorId = delegator.Id,
                    RequestedOn = DateTime.Now,
                    LitigationRequestId = litigationRequest.Id,
                    LitigationRequestOwnerTypeId = litigationRequest.LitigationRequestOwnerTypeId,
                    IsActive = true,
                    IsApproved = true,
                    ApprovedOn = DateTime.Now
                };

                var addDelegationRequest = await _repository.AddAsync<DelegationRequest, Guid>(delegationRequest);

                if (!addDelegationRequest.IsDone)
                    return new DataResponse<Delegator> { IsDone = false, Message = "خطأ في إضافة طلب التفويض للفرد" };

                return new DataResponse<Delegator> { IsDone = true, Message = "تم إضافة بيانات التفويض بنجاح", Data = delegator };

            }
            else if (input.DelegationRelationshipTypeId == DelegationRelationshipType.Facility)
            {
                var addFacility = await AddFacility(input);

                if (!addFacility.IsDone)
                    return new DataResponse<Delegator> { IsDone = false, Message = "خطأ في إضافة بيانات الشركة" };

                var delegator = new Delegator()
                {
                    Id = Guid.NewGuid(),
                    FacilityId = addFacility.Data?.Id,
                    DelegationRelationshipTypeId = DelegationRelationshipType.Facility,
                    DelegatorCitizenId = referenceId,
                    DelegateNumber = input.DelegationNumber,
                    DelegationFileId = input.DelegationFileId,
                    CommercialRegisterationFileId = input.CommercialRegisterationFileId,
                    CustodyFileId = input.CustodyFileId,
                    RepresentationFileId = input.RepresentationFileId,
                    DecisionFileId = input.DecisionFileId,
                    EstablishmentFileId = input.EstablishmentFileId,
                    IsActive = true
                };

                var addDelegator = await _repository.AddAsync<Delegator, Guid>(delegator);

                if (!addDelegator.IsDone)
                    return new DataResponse<Delegator> { IsDone = false, Message = "خطأ في إضافة بيانات التفويض للمنشأة" };


                var delegationRequest = new DelegationRequest()
                {
                    Id = Guid.NewGuid(),
                    DelegationParty = LitigationRequestParty.Plaintiff,
                    DelegatorId = delegator.Id,
                    RequestedOn = DateTime.Now,
                    LitigationRequestId = litigationRequest.Id,
                    LitigationRequestOwnerTypeId = litigationRequest.LitigationRequestOwnerTypeId,
                    IsActive = true,
                    IsApproved = true,
                    ApprovedOn = DateTime.Now
                };

                var addDelegationRequest = await _repository.AddAsync<DelegationRequest, Guid>(delegationRequest);


                if (!addDelegationRequest.IsDone)
                    return new DataResponse<Delegator> { IsDone = false, Message = "خطأ في إضافة طلب التفويض للمنشأة" };

                return new DataResponse<Delegator> { IsDone = true, Message = "تم إضافة بيانات التفويض بنجاح", Data = delegator };
            }

            return new DataResponse<Delegator> { IsDone = false, Message = "خطأ داخلي" };


        }

        private async Task<DataResponse<DelegationRequest>> AddDelegationRequestData(AddDelegationRequest input)
        {

            if (input == null)
                return new DataResponse<DelegationRequest> { IsDone = false, Message = "خطأ في إضافة بيانات التفويض" };

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var existDelegationRequest = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .AsNoTracking()
                .Any(x => x.LitigationRequestId == input.LitigationRequestId && x.Delegator.DelegatorCitizenId == referenceId && x.DelegationParty == input.LitigationRequestParty && (x.IsApproved == true || x.IsApproved == null));



            if (existDelegationRequest)
                return new DataResponse<DelegationRequest> { IsDone = false, Message = "يوجد طلب تفويض حاليا بنفس رقم الهوية" };

            var litigationData = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.Delegator)
                .Include(x => x.Contract.FirstPartyCitizen)
                .Include(x => x.Contract.SecondPartyCitizen)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == input.LitigationRequestId);


            if (input.LitigationRequestDelegationData.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
            {


                var delegator = new Delegator()
                {
                    Id = Guid.NewGuid(),
                    DelegatorCitizenId = referenceId,
                    CitizenId = input.LitigationRequestParty == LitigationRequestParty.Plaintiff ? litigationData.Contract?.FirstPartyCitizenId : litigationData.Contract?.SecondPartyCitizenId,
                    DelegateNumber = input.LitigationRequestDelegationData.DelegationNumber,
                    DelegationFileId = input.LitigationRequestDelegationData.DelegationFileId,
                    DelegationRelationshipTypeId = DelegationRelationshipType.Individual,
                    CommercialRegisterationFileId = input.LitigationRequestDelegationData.CommercialRegisterationFileId,
                    CustodyFileId = input.LitigationRequestDelegationData.CustodyFileId,
                    RepresentationFileId = input.LitigationRequestDelegationData.RepresentationFileId,
                    DecisionFileId = input.LitigationRequestDelegationData.DecisionFileId,
                    EstablishmentFileId = input.LitigationRequestDelegationData.EstablishmentFileId,
                    IsActive = false
                };



                var addDelegator = await _repository.AddAsync<Delegator, Guid>(delegator);

                var delegationRequest = new DelegationRequest()
                {
                    Id = Guid.NewGuid(),
                    DelegationParty = input.LitigationRequestParty,
                    DelegatorId = delegator.Id,
                    RequestedOn = DateTime.Now,
                    LitigationRequestId = input.LitigationRequestId,
                    LitigationRequestOwnerTypeId = input.LitigationRequestOwnerType
                };

                var addDelegationRequest = await _repository.AddAsync<DelegationRequest, Guid>(delegationRequest);

                var saveAll = await _repository.SaveChangesAsync();

                if (!saveAll.IsDone)
                    return new DataResponse<DelegationRequest> { IsDone = false, Message = "خطأ في إضافة طلب التفويض للفرد" };


                // Notification
                if (litigationData.Contract.FirstPartyCitizen is not null)
                {
                    try
                    {


                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.DelegationRequest_Added).Result.Data?.Value;
                        var body = string.Empty;
                        msg = msg?.Replace("{LitigationCode}", litigationData?.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.DelegationRequest_Added).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", litigationData?.RequestCode);
                        }

                        await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { litigationData.Contract.FirstPartyCitizen.Email },
                            Subject = "طلب تفويض جديد",
                            Body = body
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { litigationData.Contract.FirstPartyCitizen.Mobile },
                            notification_type = "SMS",
                        },
                        new List<SysNotificationDto>()
                        {
                    new SysNotificationDto()
                    {
                         ActorId = (Guid)litigationData.Contract.FirstPartyCitizenId,
                         CreatedOn = DateTime.Now,
                         ActorTypeId = NotificationActorType.Reviewer,
                         IsViewed = false,
                        ViewedOn = null,
                         RequestId = litigationData.Id,
                         RequestTypeId = RequestType.LitigationRequest,
                         Message = msg,
                    },
                        }
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }


                return new DataResponse<DelegationRequest> { IsDone = true, Message = "تم إضافة طلب التفويض بنجاح", Data = delegationRequest };

            }
            else if (input.LitigationRequestDelegationData.DelegationRelationshipTypeId == DelegationRelationshipType.Facility)
            {

                var delegator = new Delegator()
                {
                    Id = Guid.NewGuid(),
                    FacilityId = input.LitigationRequestParty == LitigationRequestParty.Plaintiff ? litigationData.Contract?.FirstPartyFacilityId : litigationData.Contract?.SecondPartyFacilityId,
                    DelegationRelationshipTypeId = DelegationRelationshipType.Facility,
                    DelegatorCitizenId = referenceId,
                    DelegateNumber = input.LitigationRequestDelegationData.DelegationNumber,
                    DelegationFileId = input.LitigationRequestDelegationData.DelegationFileId,
                    CommercialRegisterationFileId = input.LitigationRequestDelegationData.CommercialRegisterationFileId,
                    CustodyFileId = input.LitigationRequestDelegationData.CustodyFileId,
                    RepresentationFileId = input.LitigationRequestDelegationData.RepresentationFileId,
                    DecisionFileId = input.LitigationRequestDelegationData.DecisionFileId,
                    EstablishmentFileId = input.LitigationRequestDelegationData.EstablishmentFileId,
                    IsActive = false
                };

                var addDelegator = await _repository.AddAsync<Delegator, Guid>(delegator);

                var delegationRequest = new DelegationRequest()
                {
                    Id = Guid.NewGuid(),
                    DelegationParty = input.LitigationRequestParty,
                    DelegatorId = delegator.Id,
                    RequestedOn = DateTime.Now,
                    LitigationRequestId = input.LitigationRequestId,
                    LitigationRequestOwnerTypeId = input.LitigationRequestOwnerType

                };

                var addDelegationRequest = await _repository.AddAsync<DelegationRequest, Guid>(delegationRequest);

                var saveAll = await _repository.SaveChangesAsync();

                if (!saveAll.IsDone)
                    return new DataResponse<DelegationRequest> { IsDone = false, Message = "خطأ في إضافة طلب التفويض للمنشأة" };

                return new DataResponse<DelegationRequest> { IsDone = true, Message = "تم إضافة طلب التفويض بنجاح", Data = delegationRequest };
            }



            return new DataResponse<DelegationRequest> { IsDone = false, Message = "خطأ داخلي" };


        }

        private async Task<DataResponse<Citizen>> AddDraftCitizen(LitigationRequestDelegationDataDto input)
        {
            // Check if citizen exists
            var existCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                .AsNoTracking()
                .FirstOrDefault(x => x.IdentityNumber == input.IndividualIdentityNumber);

            if (existCitizen != null)
                return new DataResponse<Citizen>() { IsDone = true, Message = "المواطن موجود مسبقاً", Data = existCitizen };

            // Get Nationality
            var nationality = (await _repository.GetQueryable<SysNationality, Guid>())
               .AsNoTracking();

            // Add Draft Citizen
            var citizen = new Citizen()
            {
                Id = Guid.NewGuid(),
                FullName = input.IndividualFullName,
                Mobile = input.IndividualMobile,
                IdentityNumber = input.IndividualIdentityNumber,
                BirthDate = DateTime.ParseExact(input.IndividualBirthdate, "yyyy-MM-dd", CultureInfo.InvariantCulture),
                NationalAddress = input.IndividualNationalAddress,
                Email = input.IndividualEmail,
                NationalityId = nationality.FirstOrDefault(x => x.NationalityName == input.IndividualNationality)?.Id ?? nationality.FirstOrDefault(x => x.NationalityCode == 113)?.Id,
                IsDraftAccount = true
            };

            var addCitizen = await _repository.AddAsync<Citizen, Guid>(citizen);

            User user = new User { UserName = input.IndividualIdentityNumber.Trim(), ReferenceId = citizen.Id, Email = null, EmailConfirmed = true };
            var passwordHasher = new PasswordHasher<Core.Security.Entities.User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "123456");

            var identityUser = await _context.Users.AddAsync(user);

            if (!identityUser.IsKeySet)
                return new DataResponse<Citizen>() { IsDone = false, Message = "خطأ في إضافة المواطن" };



            if (!addCitizen.IsDone)
                return new DataResponse<Citizen>() { IsDone = false, Message = "خطأ في إضافة المواطن" };

            return new DataResponse<Citizen>() { IsDone = true, Message = "تم إضافة المواطن بنجاح", Data = citizen };
        }

        private async Task<DataResponse<Facility>> AddFacility(LitigationRequestDelegationDataDto input)
        {
            if (input == null)
                return new DataResponse<Facility> { IsDone = false, Message = "خطأ في إضافة بيانات المنشأة" };

            // Add Facility
            var facility = new Facility()
            {
                Id = Guid.NewGuid(),
                CommercialRegisterNumber = input.FacilityCommercialRegisterationNumber,
                ArabicFacilityName = input.ArabicFacilityName,
                EnglishFacilityName = input.ArabicFacilityName,
                Facility700Number = input.Facility700Number,
            };
            var addFacility = await _repository.AddAsync<Facility, Guid>(facility);
            //var addFacilityResponse = await _repository.SaveChangesAsync();
            if (!addFacility.IsDone)
                return new DataResponse<Facility> { IsDone = false, Message = "خطأ في إضافة بيانات المنشأة" };
            return new DataResponse<Facility> { IsDone = true, Message = "تم إضافة بيانات المنشأة بنجاح", Data = facility };
        }
        public async Task<BaseResponse> CreateLitigationRequestManual(CreateLitigationRequestNoIntegrationDto litigationRequestDto)
        {
            try
            {

                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false };
                if (litigationRequestDto.SecondPartyNationalId.IsNullOrEmpty())
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لابد من إدخال رقم الهوية"
                    };
                }



                var contract = (await _repository.GetQueryable<LitigationRequestContract, Guid>())
                    .Include(x => x.LitigationRequests)
                    .Where(x => x.ContractKey == litigationRequestDto.key & x.LitigationRequestContractTypeId == litigationRequestDto.ContractType)
                    .ToList();

                if (contract != null)
                {
                    if (contract.Any())
                    {
                        if (contract.FirstOrDefault()?.LitigationRequests.FirstOrDefault()?.IsFinished != true)
                        {
                            return new BaseResponse()
                            {
                                IsDone = false,
                                Message = "يوجد دعوى قائمة بالفعل على العقد"
                            };
                        }
                    }
                }


            TryGetCitizenAgian:

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                                                .Where(x => x.IdentityNumber == litigationRequestDto.SecondPartyNationalId)
                                                .ToList();

                if (!citizen.Any() && (string.IsNullOrEmpty(litigationRequestDto.SecondPartyName) && string.IsNullOrEmpty(litigationRequestDto.SecondPartyPhone)) && litigationRequestDto.ContractType != LitigationRequestContractType.Wafi)
                {

                    return new DataResponse<RentIntegrateDto>()
                    {
                        IsDone = false,
                        Data = new RentIntegrateDto() { IsDefendantRegistered = false },
                        Message = "من فضلك أدخل بيانات المدعي عليه"
                    };
                }
                else if (!citizen.Any() && (string.IsNullOrEmpty(litigationRequestDto.SecondPartyName) && string.IsNullOrEmpty(litigationRequestDto.SecondPartyPhone)) && litigationRequestDto.ContractType == LitigationRequestContractType.Wafi)
                {
                    var addDefendantFromWafi = await AddDefendantFromWafi(litigationRequestDto);

                    if (!addDefendantFromWafi.IsDone)
                        return addDefendantFromWafi;

                    goto TryGetCitizenAgian;
                }


                if (!citizen.Any())
                {

                    User user = new User { UserName = litigationRequestDto.SecondPartyNationalId.Trim(), Email = null, EmailConfirmed = true };
                    var identityUser = await _userManager.CreateAsync(user, "123456");
                    if (!identityUser.Succeeded || identityUser.Errors.Any())
                        return new BaseResponse() { IsDone = false, Message = "" };

                    citizen.Add(new Citizen()
                    {
                        Id = Guid.NewGuid(),
                        FullName = litigationRequestDto.SecondPartyName,
                        Mobile = litigationRequestDto.SecondPartyPhone,
                        IdentityNumber = litigationRequestDto.SecondPartyNationalId,
                        IsDraftAccount = true,
                        UserId = user.Id
                    });
                    await _repository.AddAsync<Citizen, Guid>(citizen.FirstOrDefault());

                    var addCitizenResponse = await _repository.SaveChangesAsync();
                    if (!addCitizenResponse.IsDone)
                        return addCitizenResponse;

                    user.ReferenceId = citizen.FirstOrDefault()?.Id;
                    var updateUser = await _userManager.UpdateAsync(user);
                    if (!identityUser.Succeeded || identityUser.Errors.Any())
                        return new BaseResponse() { IsDone = false, Message = "" };
                    goto TryGetCitizenAgian;
                }

                if (citizen.SingleOrDefault()?.Id == referenceId)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن ان يكون طرفى الدعوى لنفس الشخص"
                    };

                TryGetFacilityAgian:
                Facility? facility = null;

                if (!string.IsNullOrWhiteSpace(litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo))
                {
                    facility = (await _repository.GetQueryable<Facility, Guid>())
                        .FirstOrDefault(x => x.CommercialRegisterNumber == litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo);

                    if (facility == null)
                    {
                        var addNewFacilityFromIntegrationResponse = await _facilityService.ValidateAndAddFacility(litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo, citizen.FirstOrDefault().Id, false);
                        if (!addNewFacilityFromIntegrationResponse.IsDone)
                            return addNewFacilityFromIntegrationResponse;

                        //goto TryGetFacilityAgian;
                    }
                }



                LitigationRequestContract litigationRequestContract = new LitigationRequestContract()
                {
                    Id = Guid.NewGuid(),

                    LitigationRequestContractTypeId = litigationRequestDto.ContractType,
                    ContractKey = litigationRequestDto.key,
                    VersionNumber = litigationRequestDto.SubKey,
                    IncorporationContractFileId = litigationRequestDto.IncorporationContractFileId,
                    FirstPartyCitizenId = (litigationRequestDto.FirstPartyIndividualId == Guid.Empty) ? referenceId : litigationRequestDto.FirstPartyIndividualId,
                    FirstPartyFacilityId = (litigationRequestDto.FirstPartyFacilityId != Guid.Empty) ? litigationRequestDto.FirstPartyFacilityId : null,
                    FirstPartyCapacity = litigationRequestDto.FirstPartyCapacity,
                    SecondPartyCitizenId = (facility == null) ? citizen.FirstOrDefault()?.Id : null,
                    SecondPartyFacilityId = (facility == null) ? null : facility.Id,
                    SecondPartyCapacity = litigationRequestDto.SecondPartyCapacity
                };



                LitigationRequest litigationRequest = new LitigationRequest()
                {
                    Id = Guid.NewGuid(),
                    LitigationRequestStage = LitigationRequestStage.Review,
                    ContractId = litigationRequestContract.Id,
                    ApplicantCitizenId = referenceId,
                    LitigationRequestOwnerTypeId = litigationRequestDto.LitigationRequestOwnerType,
                    InternalStatusId = RequestStatus.SaveAsDraft,
                    ExternalStatusId = RequestStatus.SaveAsDraft,
                    Fees = 0,
                    IsPaid = false,
                    CreatedOn = DateTime.Now,
                    IsSent = false,
                    IsAllowEdit = true,
                    IsActive = true,
                    IsFinished = false,
                    ContractAddress = litigationRequestDto.ContractAddress,
                    OtherCategories = litigationRequestDto.OtherCategories
                };

                if (litigationRequestDto.LitigationRequestOwnerType != LitigationRequestOwnerType.Personal && litigationRequestDto.litigationRequestDelegationDataDto is not null)
                {
                    var addDelegationDataResponse = await AddDelegationData(litigationRequestDto.litigationRequestDelegationDataDto, litigationRequest);

                    if (!addDelegationDataResponse.IsDone)
                        return addDelegationDataResponse;

                    litigationRequest.DelegatorId = addDelegationDataResponse?.Data?.Id;
                    litigationRequestContract.FirstPartyCitizenId = addDelegationDataResponse?.Data?.CitizenId != null ? addDelegationDataResponse?.Data?.CitizenId : litigationRequestContract.FirstPartyCitizenId;
                    litigationRequestContract.FirstPartyFacilityId = addDelegationDataResponse?.Data?.FacilityId != null ? addDelegationDataResponse?.Data?.FacilityId : litigationRequestContract.FirstPartyFacilityId;
                }

                await _repository.AddAsync<LitigationRequestContract, Guid>(litigationRequestContract);

                await _repository.AddAsync<LitigationRequest, Guid>(litigationRequest);

                var saveResponse = await _repository.SaveChangesAsync();
                if (saveResponse.IsDone)
                {
                    await _context.SaveChangesAsync();

                    var newlitigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                              .Include(x => x.ApplicantCitizen)
                              .Include(x => x.ApplicantCitizen.LawLicenseFile)
                              .Include(x => x.ApplicantCitizen.IdentityFile)
                              .Include(x => x.Contract)
                              .Include(x => x.Contract.LitigationRequestContractType)
                              .Include(x => x.Contract.FirstPartyCitizen)
                              .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                              .Include(x => x.Contract.FirstPartyCitizen.FromDelegators)
                              .Include(x => x.Contract.SecondPartyCitizen)
                              .Include(x => x.Contract.SecondPartyCitizen.Nationality)
                              .Include(x => x.Contract.SecondPartyCitizen.FromDelegators)
                              .Include(x => x.Contract.FirstPartyFacility)
                              .Include(x => x.Contract.FirstPartyFacility.Delegators)
                              .Include(x => x.Contract.SecondPartyFacility)
                              .Include(x => x.Contract.SecondPartyFacility.Delegators)
                              .Where(x => x.Id == litigationRequest.Id).ToList().FirstOrDefault();

                    newlitigationRequest.ReconciliationPeriod = newlitigationRequest.Contract.LitigationRequestContractType.ReconciliationPeriod;
                    newlitigationRequest.PretenderRating = 10;
                    var newSaveResponse = await _repository.SaveChangesAsync();
                    if (saveResponse.IsDone)
                    {
                        return await ReturnLitigationRequestDto(newlitigationRequest, litigationRequestDto.SecondPartyNationalId.Trim());
                    }
                    else
                        return saveResponse;
                }
                else
                    return saveResponse;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
        }

        public async Task<BaseResponse> CreatePropertiesLitigationRequestManual(CreateLitigationRequestNoIntegrationDto litigationRequestDto)
        {
            try
            {

                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false };
                if (litigationRequestDto.SecondPartyNationalId.IsNullOrEmpty())
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لابد من إدخال رقم الهوية"
                    };
                }

                if (litigationRequestDto.LitigationRequestOwnerType != LitigationRequestOwnerType.Personal && litigationRequestDto.litigationRequestDelegationDataDto is not null)
                {

                    var validation = await ValidateDelegatorData(litigationRequestDto);

                    if (!validation.IsDone)
                        return validation;
                }

                // Properties Validation
                var existLitigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>()).
                    Include(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.Contract.SecondPartyFacility)
                    .FirstOrDefault(x => x.IsFinished != true && (x.ExternalStatusId != RequestStatus.Cancelled && x.InternalStatusId != RequestStatus.Cancelled || x.ExternalStatusId != RequestStatus.Rejected && x.InternalStatusId != RequestStatus.Rejected)
                    && x.Contract.ContractKey == litigationRequestDto.key
                    && (x.Contract.SecondPartyCitizen.IdentityNumber == litigationRequestDto.SecondPartyNationalId || x.Contract.SecondPartyFacility.CommercialRegisterNumber == litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo));

                if (existLitigationRequest != null)
                {
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "يوجد دعوى قائمة بالفعل بنفس رقم الجمعيه وهوية المدعي عليه"
                    };
                }


                var contract = (await _repository.GetQueryable<LitigationRequestContract, Guid>())
                    .Include(x => x.LitigationRequests)
                    .Where(x => x.ContractKey == litigationRequestDto.key & x.LitigationRequestContractTypeId == litigationRequestDto.ContractType)
                    .ToList();

            TryGetCitizenAgian:

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                                                .Where(x => x.IdentityNumber == litigationRequestDto.SecondPartyNationalId)
                                                .ToList();

                if (!citizen.Any() && (string.IsNullOrEmpty(litigationRequestDto.SecondPartyName) && string.IsNullOrEmpty(litigationRequestDto.SecondPartyPhone)) && litigationRequestDto.ContractType != LitigationRequestContractType.Wafi)
                {

                    return new DataResponse<RentIntegrateDto>()
                    {
                        IsDone = false,
                        Data = new RentIntegrateDto() { IsDefendantRegistered = false },
                        Message = "من فضلك أدخل بيانات المدعي عليه"
                    };
                }
                else if (!citizen.Any() && (string.IsNullOrEmpty(litigationRequestDto.SecondPartyName) && string.IsNullOrEmpty(litigationRequestDto.SecondPartyPhone)) && litigationRequestDto.ContractType == LitigationRequestContractType.Wafi)
                {
                    var addDefendantFromWafi = await AddDefendantFromWafi(litigationRequestDto);

                    if (!addDefendantFromWafi.IsDone)
                        return addDefendantFromWafi;

                    goto TryGetCitizenAgian;
                }


                if (!citizen.Any())
                {

                    User user = new User { UserName = litigationRequestDto.SecondPartyNationalId.Trim(), Email = null, EmailConfirmed = true };
                    var identityUser = await _userManager.CreateAsync(user, "123456");
                    if (!identityUser.Succeeded || identityUser.Errors.Any())
                        return new BaseResponse() { IsDone = false, Message = "" };

                    citizen.Add(new Citizen()
                    {
                        Id = Guid.NewGuid(),
                        FullName = litigationRequestDto.SecondPartyName,
                        Mobile = litigationRequestDto.SecondPartyPhone,
                        IdentityNumber = litigationRequestDto.SecondPartyNationalId,
                        IsDraftAccount = true,
                        UserId = user.Id
                    });
                    await _repository.AddAsync<Citizen, Guid>(citizen.FirstOrDefault());

                    var addCitizenResponse = await _repository.SaveChangesAsync();
                    if (!addCitizenResponse.IsDone)
                        return addCitizenResponse;

                    user.ReferenceId = citizen.FirstOrDefault()?.Id;
                    var updateUser = await _userManager.UpdateAsync(user);
                    if (!identityUser.Succeeded || identityUser.Errors.Any())
                        return new BaseResponse() { IsDone = false, Message = "" };
                    goto TryGetCitizenAgian;
                }

                if (citizen.SingleOrDefault()?.Id == referenceId)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن ان يكون طرفى الدعوى لنفس الشخص"
                    };

                TryGetFacilityAgian:
                Facility? facility = null;

                if (!string.IsNullOrWhiteSpace(litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo))
                {
                    facility = (await _repository.GetQueryable<Facility, Guid>())
                        .FirstOrDefault(x => x.CommercialRegisterNumber == litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo);

                    if (facility == null)
                    {
                        var addNewFacilityFromIntegrationResponse = await _facilityService.ValidateAndAddFacility(litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo, citizen.FirstOrDefault().Id, false);
                        if (!addNewFacilityFromIntegrationResponse.IsDone)
                            return addNewFacilityFromIntegrationResponse;

                        goto TryGetFacilityAgian;
                    }

                }

                LitigationRequestContract litigationRequestContract = new LitigationRequestContract()
                {
                    Id = Guid.NewGuid(),

                    LitigationRequestContractTypeId = litigationRequestDto.ContractType,
                    ContractKey = litigationRequestDto.key,
                    AssociationName = litigationRequestDto.AssociationName,
                    IncorporationContractFileId = litigationRequestDto.IncorporationContractFileId,
                    FirstPartyCitizenId = (litigationRequestDto.FirstPartyIndividualId == Guid.Empty) ? referenceId : litigationRequestDto.FirstPartyIndividualId,
                    FirstPartyFacilityId = (litigationRequestDto.FirstPartyFacilityId != Guid.Empty) ? litigationRequestDto.FirstPartyFacilityId : null,
                    FirstPartyCapacity = litigationRequestDto.FirstPartyCapacity,
                    SecondPartyCitizenId = (facility == null) ? citizen.FirstOrDefault()?.Id : null,
                    SecondPartyFacilityId = (facility == null) ? null : facility.Id,
                    SecondPartyCapacity = litigationRequestDto.SecondPartyCapacity
                };



                LitigationRequest litigationRequest = new LitigationRequest()
                {
                    Id = Guid.NewGuid(),
                    LitigationRequestStage = LitigationRequestStage.Review,
                    ContractId = litigationRequestContract.Id,
                    ApplicantCitizenId = referenceId,
                    LitigationRequestOwnerTypeId = litigationRequestDto.LitigationRequestOwnerType,
                    InternalStatusId = RequestStatus.SaveAsDraft,
                    ExternalStatusId = RequestStatus.SaveAsDraft,
                    Fees = 0,
                    IsPaid = false,
                    CreatedOn = DateTime.Now,
                    IsSent = false,
                    IsAllowEdit = true,
                    IsActive = true,
                    IsFinished = false,
                    ContractAddress = litigationRequestDto.ContractAddress,
                    OtherCategories = litigationRequestDto.OtherCategories
                };

                if (litigationRequestDto.LitigationRequestOwnerType != LitigationRequestOwnerType.Personal && litigationRequestDto.litigationRequestDelegationDataDto is not null)
                {


                    var addDelegationDataResponse = await AddDelegationData(litigationRequestDto.litigationRequestDelegationDataDto, litigationRequest);

                    if (!addDelegationDataResponse.IsDone)
                        return addDelegationDataResponse;

                    litigationRequest.DelegatorId = addDelegationDataResponse?.Data?.Id;
                    litigationRequestContract.FirstPartyCitizenId = addDelegationDataResponse?.Data?.CitizenId != null ? addDelegationDataResponse?.Data?.CitizenId : litigationRequestContract.FirstPartyCitizenId;
                    litigationRequestContract.FirstPartyFacilityId = addDelegationDataResponse?.Data?.FacilityId != null ? addDelegationDataResponse?.Data?.FacilityId : litigationRequestContract.FirstPartyFacilityId;

                }

                await _repository.AddAsync<LitigationRequestContract, Guid>(litigationRequestContract);

                await _repository.AddAsync<LitigationRequest, Guid>(litigationRequest);

                var saveResponse = await _repository.SaveChangesAsync();
                if (saveResponse.IsDone)
                {
                    await _context.SaveChangesAsync();

                    var newlitigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                              .Include(x => x.ApplicantCitizen)
                              .Include(x => x.ApplicantCitizen.LawLicenseFile)
                              .Include(x => x.ApplicantCitizen.IdentityFile)
                              .Include(x => x.Contract)
                              .Include(x => x.Contract.LitigationRequestContractType)
                              .Include(x => x.Contract.FirstPartyCitizen)
                              .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                              .Include(x => x.Contract.FirstPartyCitizen.FromDelegators)
                              .Include(x => x.Contract.SecondPartyCitizen)
                              .Include(x => x.Contract.SecondPartyCitizen.Nationality)
                              .Include(x => x.Contract.SecondPartyCitizen.FromDelegators)
                              .Include(x => x.Contract.FirstPartyFacility)
                              .Include(x => x.Contract.FirstPartyFacility.Delegators)
                              .Include(x => x.Contract.SecondPartyFacility)
                              .Include(x => x.Contract.SecondPartyFacility.Delegators)
                              .Where(x => x.Id == litigationRequest.Id).ToList().FirstOrDefault();

                    newlitigationRequest.ReconciliationPeriod = newlitigationRequest.Contract.LitigationRequestContractType.ReconciliationPeriod;
                    newlitigationRequest.PretenderRating = 10;
                    var newSaveResponse = await _repository.SaveChangesAsync();
                    if (saveResponse.IsDone)
                    {
                        return await ReturnLitigationRequestDto(newlitigationRequest, litigationRequestDto.SecondPartyNationalId.Trim());
                    }
                    else
                        return saveResponse;
                }
                else
                    return saveResponse;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
        }

        public async Task<BaseResponse> CreateEjarLitigationRequestManual(CreateLitigationRequestNoIntegrationDto litigationRequestDto)
        {
            try
            {
                bool plaintiffCitizenExist;
                bool defendantCitizenExist;


                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                if (referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false };


                var contract = (await _repository.GetQueryable<LitigationRequestContract, Guid>())
                    .Include(x => x.LitigationRequests)
                    .Where(x => x.ContractKey == litigationRequestDto.key & x.LitigationRequestContractTypeId == litigationRequestDto.ContractType).ToList();

                if (contract != null)
                {
                    if (contract.Any())
                    {
                        if (contract.FirstOrDefault()?.LitigationRequests.FirstOrDefault()?.IsFinished != true)
                        {
                            return new BaseResponse()
                            {
                                IsDone = false,
                                Message = "يوجد دعوى قائمة بالفعل على العقد"
                            };
                        }
                    }
                }


                // Get Ejar Data
                var ejarData = await _rentService.GetRentContractDetails(long.Parse(litigationRequestDto.key));

                if (!ejarData.IsDone)
                    return ejarData;

                if (string.IsNullOrEmpty(litigationRequestDto.SecondPartyNationalId))
                {

                    // Catch Multiple Defendant lessors scenario
                    var multipleLessors = await CatchMultipleLessors(ejarData.Data, litigationRequestDto, referenceId);

                    if (!multipleLessors.IsDone)
                        return multipleLessors;
                }

                // Get Plaintiff and Defendant Data from Ejar Response
                var ejarPlaintiffAndDefendantData = await ReturnEjarPlaintiffAndDefendantData(ejarData.Data, litigationRequestDto, referenceId, litigationRequestDto.SecondPartyNationalId);

                if (!ejarPlaintiffAndDefendantData.IsDone)
                    return ejarPlaintiffAndDefendantData;


                var plaintiffDefendantSharedData = await GetEjarPlaintiffAndDefendantData(ejarPlaintiffAndDefendantData.Data);

                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Debug,
                           Message = $" Ejar Data For Process =====> {JsonSerializer.Serialize(plaintiffDefendantSharedData)}"
                       });

                // Check if defendant exist ? if not insert his data Mobile, Email

                defendantCitizenExist = (await _repository.GetQueryable<Citizen, Guid>())
                                                .Any(x => x.IdentityNumber == plaintiffDefendantSharedData.DefendantEjarNationalId);

                if (!defendantCitizenExist && string.IsNullOrEmpty(litigationRequestDto.SecondPartyPhone))
                {

                    return new DataResponse<RentIntegrateDto>()
                    {
                        IsDone = false,
                        Data = new RentIntegrateDto() { IsDefendantRegistered = false },
                        Message = "من فضلك أدخل بيانات المدعي عليه"
                    };
                }



            TryGetDefendantCitizenAgain:

                var defendantCitizen = (await _repository.GetQueryable<Citizen, Guid>())
                                                .Where(x => x.IdentityNumber == plaintiffDefendantSharedData.DefendantEjarNationalId)
                                                .ToList();


                if (!defendantCitizen.Any())
                {

                    User user = new User { UserName = plaintiffDefendantSharedData.DefendantEjarNationalId, Email = litigationRequestDto.SecondPartyEmail is not null ? litigationRequestDto.SecondPartyEmail : null, EmailConfirmed = true };
                    var identityUser = await _userManager.CreateAsync(user, "123456");
                    if (!identityUser.Succeeded || identityUser.Errors.Any())
                        return new BaseResponse() { IsDone = false, Message = "" };

                    defendantCitizen.Add(new Citizen()
                    {
                        Id = Guid.NewGuid(),
                        FullName = plaintiffDefendantSharedData.DefendantEjarName,
                        Mobile = litigationRequestDto.SecondPartyPhone,
                        Email = litigationRequestDto.SecondPartyEmail is not null ? litigationRequestDto.SecondPartyEmail : null,
                        IdentityNumber = plaintiffDefendantSharedData.DefendantEjarNationalId,
                        IsDraftAccount = true,
                        UserId = user.Id
                    });
                    await _repository.AddAsync<Citizen, Guid>(defendantCitizen.FirstOrDefault());

                    var addCitizenResponse = await _repository.SaveChangesAsync();
                    if (!addCitizenResponse.IsDone)
                        return addCitizenResponse;

                    user.ReferenceId = defendantCitizen.FirstOrDefault()?.Id;
                    var updateUser = await _userManager.UpdateAsync(user);
                    if (!identityUser.Succeeded || identityUser.Errors.Any())
                        return new BaseResponse() { IsDone = false, Message = "" };
                    goto TryGetDefendantCitizenAgain;
                }

                if (defendantCitizen.SingleOrDefault()?.Id == referenceId)
                    return new BaseResponse()
                    {
                        IsDone = false,
                        Message = "لا يمكن ان يكون طرفى الدعوى لنفس الشخص"
                    };


                TryGetFacilityAgian:
                Facility? facility = null;

                if (!string.IsNullOrWhiteSpace(litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo))
                {
                    facility = (await _repository.GetQueryable<Facility, Guid>())
                        .FirstOrDefault(x => x.CommercialRegisterNumber == litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo);

                    if (facility == null)
                    {
                        var addNewFacilityFromIntegrationResponse = await _facilityService.ValidateAndAddFacility(litigationRequestDto.SecondPartyFacilityCommercialRegistrationNo, defendantCitizen.FirstOrDefault().Id, false);
                        if (!addNewFacilityFromIntegrationResponse.IsDone)
                            return addNewFacilityFromIntegrationResponse;

                        goto TryGetFacilityAgian;
                    }
                }

                LitigationRequestContract litigationRequestContract = new LitigationRequestContract()
                {
                    Id = Guid.NewGuid(),

                    LitigationRequestContractTypeId = litigationRequestDto.ContractType,
                    ContractKey = litigationRequestDto.key,
                    VersionNumber = litigationRequestDto.SubKey,
                    FirstPartyCitizenId = (litigationRequestDto.FirstPartyIndividualId == Guid.Empty) ? referenceId : litigationRequestDto.FirstPartyIndividualId,
                    FirstPartyFacilityId = (litigationRequestDto.FirstPartyFacilityId != Guid.Empty) ? litigationRequestDto.FirstPartyFacilityId : null,
                    FirstPartyCapacity = plaintiffDefendantSharedData.PlaintiffEjarCapacity,
                    SecondPartyCitizenId = (facility == null) ? defendantCitizen.FirstOrDefault()?.Id : null,
                    SecondPartyFacilityId = (facility == null) ? null : facility.Id,
                    SecondPartyCapacity = plaintiffDefendantSharedData.DefendantEjarCapacity,
                };



                LitigationRequest litigationRequest = new LitigationRequest()
                {
                    Id = Guid.NewGuid(),
                    LitigationRequestStage = LitigationRequestStage.Review,
                    ContractId = litigationRequestContract.Id,
                    ApplicantCitizenId = referenceId,
                    LitigationRequestOwnerTypeId = litigationRequestDto.LitigationRequestOwnerType,
                    InternalStatusId = RequestStatus.SaveAsDraft,
                    ExternalStatusId = RequestStatus.SaveAsDraft,
                    Fees = 0,
                    IsPaid = false,
                    CreatedOn = DateTime.Now,
                    IsSent = false,
                    IsAllowEdit = true,
                    IsActive = true,
                    IsFinished = false,
                    //ContractAddress = litigationRequestDto.ContractAddress,
                    ContractAddress = $"{plaintiffDefendantSharedData.EjarProperty.BuildingNumber},حي {plaintiffDefendantSharedData.EjarProperty.Province},شارع {plaintiffDefendantSharedData.EjarProperty.StreetName}, مدينة {plaintiffDefendantSharedData.EjarProperty.City}",
                    OtherCategories = litigationRequestDto.OtherCategories
                };

                if (litigationRequestDto.LitigationRequestOwnerType != LitigationRequestOwnerType.Personal && litigationRequestDto.litigationRequestDelegationDataDto is not null)
                {
                    var addDelegationDataResponse = await AddDelegationData(litigationRequestDto.litigationRequestDelegationDataDto, litigationRequest);

                    if (!addDelegationDataResponse.IsDone)
                        return addDelegationDataResponse;

                    litigationRequest.DelegatorId = addDelegationDataResponse?.Data?.Id;
                    litigationRequestContract.FirstPartyCitizenId = addDelegationDataResponse?.Data?.CitizenId != null ? addDelegationDataResponse?.Data?.CitizenId : litigationRequestContract.FirstPartyCitizenId;
                    litigationRequestContract.FirstPartyFacilityId = addDelegationDataResponse?.Data?.FacilityId != null ? addDelegationDataResponse?.Data?.FacilityId : litigationRequestContract.FirstPartyFacilityId;

                }

                await _repository.AddAsync<LitigationRequestContract, Guid>(litigationRequestContract);

                await _repository.AddAsync<LitigationRequest, Guid>(litigationRequest);

                var saveResponse = await _repository.SaveChangesAsync();
                if (saveResponse.IsDone)
                {
                    await _context.SaveChangesAsync();

                    var newlitigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                              .Include(x => x.ApplicantCitizen)
                              .Include(x => x.ApplicantCitizen.LawLicenseFile)
                              .Include(x => x.ApplicantCitizen.IdentityFile)
                              .Include(x => x.Contract)
                              .Include(x => x.Contract.LitigationRequestContractType)
                              .Include(x => x.Contract.FirstPartyCitizen)
                              .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                              .Include(x => x.Contract.FirstPartyCitizen.FromDelegators)
                              .Include(x => x.Contract.SecondPartyCitizen)
                              .Include(x => x.Contract.SecondPartyCitizen.Nationality)
                              .Include(x => x.Contract.SecondPartyCitizen.FromDelegators)
                              .Include(x => x.Contract.FirstPartyFacility)
                              .Include(x => x.Contract.FirstPartyFacility.Delegators)
                              .Include(x => x.Contract.SecondPartyFacility)
                              .Include(x => x.Contract.SecondPartyFacility.Delegators)
                              .Where(x => x.Id == litigationRequest.Id).ToList().FirstOrDefault();

                    newlitigationRequest.ReconciliationPeriod = newlitigationRequest.Contract.LitigationRequestContractType.ReconciliationPeriod;
                    newlitigationRequest.PretenderRating = 10;
                    var newSaveResponse = await _repository.SaveChangesAsync();
                    if (saveResponse.IsDone)
                    {
                        return await ReturnEjarLitigationRequestDto(newlitigationRequest, litigationRequestDto, plaintiffDefendantSharedData);
                    }
                    else
                        return saveResponse;
                }
                else
                    return saveResponse;
            }
            catch (Exception ex)
            {
                return new ExceptionResponse()
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
        }

        private async Task<PlaintiffDefendantSharedEjarDto> GetEjarPlaintiffAndDefendantData(EjarPartiesDto input)
        {
            var plaintiffDefendantSharedData = new PlaintiffDefendantSharedEjarDto();


            plaintiffDefendantSharedData.EjarProperty = new EjarPropertyDto()
            {

                City = input.Property.City.ar,
                Province = input.Property.Province.ar,
                Region = input.Property.Region.ar,
                StreetName = input.Property.StreetName.Value,
                BuildingNumber = input.Property.BuildingNumber.Value,
                PostCost = input.Property.PostCost.Value,
                AdditionalNumber = input.Property.AdditionalNumber.Value,
                Coordiantes = input.Property.Coordiantes.Value
            };


            if (input.Plaintiff is LessorAttributes lessorPlaintiff)
            {

                plaintiffDefendantSharedData.PlaintiffEjarNationalId = lessorPlaintiff.LessorIdNumber.Value;
                plaintiffDefendantSharedData.PlaintiffEjarName = lessorPlaintiff.FullLessorName.Value;
                plaintiffDefendantSharedData.PlaintiffEjarCapacity = lessorPlaintiff.PartyRole.ar;
                plaintiffDefendantSharedData.PlaintiffEjarType = lessorPlaintiff.PartyType.ar;
            }
            else if (input.Plaintiff is TenantAttributes tenantPlaintiff)
            {

                plaintiffDefendantSharedData.PlaintiffEjarNationalId = tenantPlaintiff.TenantIdNumber.Value;
                plaintiffDefendantSharedData.PlaintiffEjarName = tenantPlaintiff.FullTenantName.Value;
                plaintiffDefendantSharedData.PlaintiffEjarCapacity = tenantPlaintiff.PartyRole.ar;
                plaintiffDefendantSharedData.PlaintiffEjarType = tenantPlaintiff.PartyType.ar;
                plaintiffDefendantSharedData.PlaintiffDOBHijri = tenantPlaintiff.TenantDobHijri.Value;
            }

            if (input.Defendant is LessorAttributes lessorDefendant)
            {

                plaintiffDefendantSharedData.DefendantEjarNationalId = lessorDefendant.LessorIdNumber.Value;
                plaintiffDefendantSharedData.DefendantEjarName = lessorDefendant.FullLessorName.Value;
                plaintiffDefendantSharedData.DefendantEjarCapacity = lessorDefendant.PartyRole.ar;
                plaintiffDefendantSharedData.DefendantEjarType = lessorDefendant.PartyType.ar;
            }
            else if (input.Defendant is TenantAttributes tenantDefendant)
            {

                plaintiffDefendantSharedData.DefendantEjarNationalId = tenantDefendant.TenantIdNumber.Value;
                plaintiffDefendantSharedData.DefendantEjarName = tenantDefendant.FullTenantName.Value;
                plaintiffDefendantSharedData.DefendantEjarCapacity = tenantDefendant.PartyRole.ar;
                plaintiffDefendantSharedData.DefendantEjarType = tenantDefendant.PartyType.ar;
                plaintiffDefendantSharedData.DefendnatDOBHijri = tenantDefendant.TenantDobHijri.Value;

            }

            return plaintiffDefendantSharedData;
        }

        private async Task<BaseResponse> AddDefendantFromWafi(CreateLitigationRequestNoIntegrationDto litigationRequestDto)
        {
            try
            {
                var wafiData = await GetWafiData(litigationRequestDto.key, litigationRequestDto.SecondPartyNationalId);

                if (!wafiData.IsDone)
                    return new BaseResponse() { IsDone = false, Message = "حدث خطأ في التكامل مع وافي" };

                if (wafiData?.Data?.data is null)
                    return new DataResponse<RentIntegrateDto>()
                    {
                        IsDone = false,
                        Data = new RentIntegrateDto() { IsDefendantRegistered = false },
                        Message = "من فضلك أدخل بيانات المدعي عليه"
                    };


                if (litigationRequestDto.SecondPartyCapacityId is not null)
                {
                    switch (litigationRequestDto.SecondPartyCapacityId)
                    {
                        case (int)WafiRequestOwnerType.Accountant:

                            if (wafiData?.Data?.data?.accountant is null)
                                return new BaseResponse() { IsDone = false, Message = "حدث خطأ في التكامل مع وافي" };

                            User accountantUser = new User { UserName = litigationRequestDto.SecondPartyNationalId.Trim(), Email = null, EmailConfirmed = true };
                            var accountantIdentityUser = await _userManager.CreateAsync(accountantUser, "123456");
                            if (!accountantIdentityUser.Succeeded || accountantIdentityUser.Errors.Any())
                                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

                            var accountant = new Citizen()
                            {
                                Id = Guid.NewGuid(),
                                FullName = wafiData.Data.data.accountant.fullName,
                                Mobile = wafiData.Data.data.accountant.mobile,
                                IdentityNumber = litigationRequestDto.SecondPartyNationalId,
                                IsDraftAccount = true,
                                UserId = accountantUser.Id
                            };
                            await _repository.AddAsync<Citizen, Guid>(accountant);
                            await _repository.SaveChangesAsync();
                            accountantUser.ReferenceId = accountant.Id;
                            var updateAccountantUser = await _userManager.UpdateAsync(accountantUser);
                            if (!updateAccountantUser.Succeeded || updateAccountantUser.Errors.Any())
                                return new BaseResponse() { IsDone = false };

                            return new BaseResponse() { IsDone = true };


                        case (int)WafiRequestOwnerType.EngineeringConsultant:

                            if (wafiData?.Data?.data?.engineeringConsultant is null)
                                return new BaseResponse() { IsDone = false, Message = "حدث خطأ في التكامل مع وافي" };

                            User consultantUser = new User { UserName = litigationRequestDto.SecondPartyNationalId.Trim(), Email = null, EmailConfirmed = true };
                            var consultantIdentityUser = await _userManager.CreateAsync(consultantUser, "123456");
                            if (!consultantIdentityUser.Succeeded || consultantIdentityUser.Errors.Any())
                                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

                            var consultant = new Citizen()
                            {
                                Id = Guid.NewGuid(),
                                FullName = wafiData.Data.data.engineeringConsultant.fullName,
                                Mobile = wafiData.Data.data.engineeringConsultant.mobile,
                                IdentityNumber = litigationRequestDto.SecondPartyNationalId,
                                IsDraftAccount = true,
                                UserId = consultantUser.Id
                            };

                            await _repository.AddAsync<Citizen, Guid>(consultant);

                            await _repository.SaveChangesAsync();
                            consultantUser.ReferenceId = consultant.Id;
                            var consultantUpdateUser = await _userManager.UpdateAsync(consultantUser);
                            if (!consultantUpdateUser.Succeeded || consultantUpdateUser.Errors.Any())
                                return new BaseResponse() { IsDone = false };

                            return new BaseResponse() { IsDone = true };


                        case (int)WafiRequestOwnerType.RealStateDeveloper:

                            if (wafiData?.Data?.data?.developer is null)
                                return new BaseResponse() { IsDone = false, Message = "حدث خطأ في التكامل مع وافي" };

                            User developerUser = new User { UserName = litigationRequestDto.SecondPartyNationalId.Trim(), Email = null, EmailConfirmed = true };
                            var identityUser3 = await _userManager.CreateAsync(developerUser, "123456");
                            if (!identityUser3.Succeeded || identityUser3.Errors.Any())
                                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

                            var developer = new Citizen()
                            {
                                Id = Guid.NewGuid(),
                                FullName = wafiData.Data.data.developer.fullName,
                                Mobile = wafiData.Data.data.developer.mobile,
                                IdentityNumber = litigationRequestDto.SecondPartyNationalId,
                                IsDraftAccount = true,
                                UserId = developerUser.Id
                            };

                            await _repository.AddAsync<Citizen, Guid>(developer);

                            await _repository.SaveChangesAsync();
                            developerUser.ReferenceId = developer.Id;
                            var developerUpdateUser = await _userManager.UpdateAsync(developerUser);
                            if (!developerUpdateUser.Succeeded || developerUpdateUser.Errors.Any())
                                return new BaseResponse() { IsDone = false };

                            return new BaseResponse() { IsDone = true };


                    }
                }
                else
                {
                    if (wafiData?.Data?.data?.developer is null)
                        return new BaseResponse() { IsDone = false, Message = "حدث خطأ في التكامل مع وافي" };

                    User developerUser = new User { UserName = litigationRequestDto.SecondPartyNationalId.Trim(), Email = null, EmailConfirmed = true };
                    var identityUser3 = await _userManager.CreateAsync(developerUser, "123456");
                    if (!identityUser3.Succeeded || identityUser3.Errors.Any())
                        return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

                    var developer = new Citizen()
                    {
                        Id = Guid.NewGuid(),
                        FullName = wafiData.Data.data.developer.fullName,
                        Mobile = wafiData.Data.data.developer.mobile,
                        IdentityNumber = litigationRequestDto.SecondPartyNationalId,
                        IsDraftAccount = true,
                        UserId = developerUser.Id
                    };

                    await _repository.AddAsync<Citizen, Guid>(developer);

                    await _repository.SaveChangesAsync();
                    developerUser.ReferenceId = developer.Id;
                    var developerUpdateUser = await _userManager.UpdateAsync(developerUser);
                    if (!developerUpdateUser.Succeeded || developerUpdateUser.Errors.Any())
                        return new BaseResponse() { IsDone = false };

                    return new BaseResponse() { IsDone = true };

                }

                return new BaseResponse() { IsDone = false, Message = "خطأ في البيانات المدخله" };


            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }


        }

        public async Task<BaseResponse> GetLitigationArbitrators(Guid requestId)
        {
            var newlitigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.LitigationRequestArbitratorLists)
                .ThenInclude(c => c.ArbitratorCitizen)
                .Where(x => x.Id == requestId).FirstOrDefault();

            List<LitigationRequestArbitratorListDto> citizenArbirators = new List<LitigationRequestArbitratorListDto>();
            newlitigationRequest.LitigationRequestArbitratorLists?.ToList().ForEach(t =>
            {
                citizenArbirators.Add(new LitigationRequestArbitratorListDto()
                {
                    FullName = t.ArbitratorCitizen.FullName,
                    Id = t.ArbitratorCitizen.Id,
                });
            });
            return new DataResponse<List<LitigationRequestArbitratorListDto>>()
            {
                IsDone = true,
                Data = citizenArbirators
            };
        }

        #region Agreement Request
        public async Task<BaseResponse> GetCitizenLitigationRequestForAgreementRequestReply()
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>()).Include(x => x.Area).Include(x => x.City)
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
                                       .OrderByDescending(x => x.CreatedOn).ToList();

            List<LitigationRequestMircoDto> litigationRequestMircoDtos = new List<LitigationRequestMircoDto>();
            if (litigationRequests != null)
            {
                foreach (var litigationRequest in litigationRequests)
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

                    litigationRequestMircoDto.Status = litigationRequest.InternalStatus.RequestStatus;
                    litigationRequestMircoDtos.Add(litigationRequestMircoDto);

                }
            }

            return new DataResponse<List<LitigationRequestMircoDto>>()
            {
                Data = litigationRequestMircoDtos,
                IsDone = true
            };
        }
        public async Task<BaseResponse> GetCitizenLitigationRequestForAgreementRequestReply(Guid requestId)
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
                         .Include(x => x.Contract.FirstPartyCitizen.Nationality)
                         .Include(x => x.Contract.SecondPartyFacility)
                         .Include(x => x.Contract.SecondPartyCitizen.Nationality)
                         .Include(x => x.Contract.LitigationRequestContractType)
                         .Include(x => x.LitigationRequestRequests)

                         .Where(x =>
                         x.ApplicantCitizenId == referenceId & x.Id == requestId
                      |
                      x.Contract.FirstPartyCitizenId == referenceId & x.Id == requestId
                      |
                        x.Contract.SecondPartyCitizenId == referenceId & x.Id == requestId
                      |
                       x.Contract.FirstPartyFacility.CitizenId == referenceId & x.Id == requestId
                      |
                      x.Contract.SecondPartyFacility.CitizenId == referenceId & x.Id == requestId
                         ).AsNoTracking().ToList().FirstOrDefault();
            if (litigationRequest == null)
            {
                return new BaseResponse { IsDone = false, Message = "غير مصرح بعرض الدعوي" };
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

        public async Task<BaseResponse> CallNafathForAgreementRequestReply(Guid requestId, ApproveRejectStatus agreementRequestStatus)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                              .AsNoTracking()
                              .FirstOrDefault(x => x.Id == requestId);

                var citizen = (await _repository.GetQueryable<Citizen, Guid>())
                              .AsNoTracking()
                              .FirstOrDefault(x => x.Id == referenceId);

                if (litigationRequest is not null & citizen is not null)
                {
                    var nafathAgreementResponse = await _nafathService.NafathAgreementRequest(
                         new NafathRequestDto() { IsRegister = false, NationalId = citizen.IdentityNumber },
                         new NafathAgreementRequestTransactionsDto()
                         {
                             LitigationRequestId = litigationRequest.Id,
                             NationalId = citizen.IdentityNumber,
                             AgreementStatus = agreementRequestStatus
                         });


                    return nafathAgreementResponse;


                }
                return new BaseResponse() { IsDone = false, Message = "خطأ في البيانات المدخله" };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };
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
                    // if the type is arbitiration and reconciliation we will continue on reconciliatrion only
                    if (litigationRequest.LitigationRequestCategoryId == LitigationRequestCategory.ReconciliationAndArbitration)
                    {
                        var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                        await _bpfService.ExcuteWorkFlow(litigationRequest.Id, default, 2);
                        litigationRequest.InternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;
                        litigationRequest.ExternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;

                        //  Send Notification 
                        // تم رفض طلب المشارطة على الدعوى وجارى استكمال الصلح ..........
                        // المدعى - المدعى عليه - مدير ادارة التدقيق - موظف التدقيق
                        try
                        {
                            var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Reject_Agreement).Result.Data?.Value;
                            msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Reject_Agreement).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            await SendNotificationMSWithDefendant(body, msg, litigationRequest, RequestType.LitigationAgreementRequest, " رفض طلب المشارطة وجارى استكمال مرحلة الصلح");
                        }
                        catch { }
                    }
                    // else "arbitiration" do old logic
                    else
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
                            var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                            string body;
                            using (StreamReader streamReader = File.OpenText(pathToFile))
                            {
                                body = await streamReader.ReadToEndAsync();
                                body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Reject_Agreement).Result.Data?.Value);
                                body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                            }
                            await SendNotificationMSWithDefendant(body, msg, litigationRequest, RequestType.LitigationAgreementRequest, "رفض طلب المشارطة");
                        }
                        catch { }
                    }
                }
                else
                {
                    var _bpfService = _serviceProvider.GetRequiredService<IBpfService>();
                    await _bpfService.ExcuteWorkFlow(litigationRequest.Id, default, 2);
                    litigationRequest.InternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;
                    litigationRequest.ExternalStatusId = RequestStatus.LitigationFeesHaveBeenCollected;

                    //  Send Notification 
                    // تم الموافقة على طلب المشارطة على الدعوى رقم........
                    // المدعى - المدعى عليه - مدير ادارة التدقيق - موظف التدقيق
                    try
                    {
                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Accept_Agreement).Result.Data?.Value;
                        msg = msg.Replace("{LitigationCode}", litigationRequest.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        string body;
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.LitigationRequest_Accept_Agreement).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", litigationRequest?.RequestCode);
                        }
                        await SendNotificationMSWithDefendant(body, msg, litigationRequest, RequestType.LitigationAgreementRequest, "قبول طلب المشارطة");
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

        private async Task SendNotificationMSWithDefendant(string mailMsg, string msg, LitigationRequest litigationRequest, RequestType requestTypeId, string subject)
        {
            var deptAdminInfoReview = await _employeeService.GetAdminsByDepartmentType(DepartmentType.Review);

            //ApplicantCitizen
            var objEmail = new SendEmailDto()
            {
                Body = mailMsg,
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

        public async Task<DataResponse<WafiDataDto>> GetWafiData(string licenseNumber, string identityNumber)
        {
            var accessToken = (await _gEELoginService.Login()).Data?.Access_token;


            //string accessToken = await GetAccessToken();

            //HttpClient httpclient = new HttpClient();
            try
            {
                //httpclient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


                var apiUrl = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.WafiData_URL).Result.Data?.Value;
                apiUrl = string.Format(apiUrl, licenseNumber, identityNumber);

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
                request.Headers.Add("Authorization", $"Bearer {accessToken}");
                var response = await client.SendAsync(request);


                //var apiUrl = $"https://wafiservicesapi.rega.gov.sa/api/REAC/GetLicenseProjectPartiesData?licenseNumber={Uri.EscapeDataString(licenseNumber)}&identityNumber={identityNumber}";
                // var apiUrl=" https://wafiservicesapi.rega.gov.sa/api/REAC/GetLicenseProjectPartiesData?licenseNumber=2/أ&identityNumber=1003643689";
                //httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);


                //var response = await httpclient.GetAsync(apiUrl);


                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var _wafiData = JsonSerializer.Deserialize<WafiDataDto>(responseContent);

                    return new DataResponse<WafiDataDto>() { Data = _wafiData, IsDone = true };
                }
                else
                {
                    return new DataResponse<WafiDataDto> { IsDone = true, Message = "خطأ داخلي" };
                }
            }
            catch (Exception ex)
            {
                _loggerManager.Log(Core.Log.Domain.Shared.Enums.LoggerType.Database
                       , new LogDto()
                       {
                           Type = Core.Log.Domain.Shared.Enums.LogType.Exception,
                           Message = $"GetWafiData Exception ====> {ex.ToString()}"
                       });
                return null;
            }
        }

        private async Task<string> GetAccessToken()
        {
            string userName = "REACIntegrationUser";
            string password = "P@ssw0rd123";
            int userType = 3;

            var accessToken = await Loginin(userName, password, userType);

            return accessToken;
        }
        private static async Task<string> Loginin(string userName, string password, int userType)
        {
            var loginData = new
            {
                userName = userName,
                password = password,
                userType = userType
            };

            using (HttpClient client = new HttpClient())
            {
                var LoginUrl = "https://wafisecurity.rega.gov.sa/api/Account/Login";
                var content = new StringContent(JsonSerializer.Serialize(loginData), Encoding.UTF8, "application/json");

                try
                {
                    var response = await client.PostAsync(LoginUrl, content);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response: {responseBody}");

                    if (response.IsSuccessStatusCode)
                    {

                        var token = JsonSerializer.Deserialize<WafiLoginDto>(responseBody);

                        return token != null ? token.data.accessToken : "";
                    }
                    else
                    {

                        throw new Exception($"Login failed: {response.StatusCode}, Response: {responseBody}");
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                    throw;
                }
            }
        }

        public async Task<BaseResponse> DeleteByIdAsync(Guid id)
        {
            try
            {
                // Validate request exists and can be deleted
                var entity = await _context.LitigationRequests
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity == null)
                    return new BaseResponse { IsDone = false, Message = "دعوي غير موجودة" };

                // Check if request can be deleted based on status
                if (entity.InternalStatusId != RequestStatus.SaveAsDraft)
                    return new BaseResponse { IsDone = false, Message = "لا يمكن مسح هذه الدعوي" };

                try
                {
                    // Execute stored procedure directly
                    var result = await _repository.ExecuteSqlCommandAsync(
                         "EXEC sp_DeleteLitigationRequest @Id",
                         new[] { new SqlParameter("@Id", id) });
                    if (!result.IsDone)
                        return new BaseResponse { IsDone = false, Message = "خطأ عند مسح الدعوي" };

                    return new BaseResponse { IsDone = true, Message = "تم مسح الدعوي بنجاح" };

                }
                catch (SqlException sqlEx)
                {
                    _loggerManager.Log(LoggerType.Database, new LogDto
                    {
                        Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                        Message = $"SQL Error deleting litigation request: {sqlEx.Message}"
                    });

                    return new ExceptionResponse
                    {
                        IsDone = false,
                        Exception = "خطأ داخلي"
                    };
                }

            }
            catch (Exception ex)
            {
                _loggerManager.Log(LoggerType.Database, new LogDto
                {
                    Type = Core.Log.Domain.Shared.Enums.LogType.Error,
                    Message = $"Error in DeleteByIdAsync: {ex.Message}\n{ex}"
                });

                return new ExceptionResponse
                {
                    IsDone = false,
                    Exception = ex.Message + "\n" + ex.ToString()
                };
            }
        }


        #endregion



        #region Delegation
        public async Task<BaseResponse> GetLitigationRequestDataForDelegation(string requestCode, string identityNumber, LitigationRequestParty party)
        {
            var existLitigationRequest = party == LitigationRequestParty.Plaintiff ? (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.Contract.FirstPartyCitizen)
                    .Include(x => x.Contract.FirstPartyFacility)
                    .Include(x => x.Delegator)
                    .FirstOrDefault(x => x.InternalStatusId != RequestStatus.Cancelled && x.IsFinished != true && x.RequestCode == requestCode && (x.Contract.FirstPartyCitizen.IdentityNumber == identityNumber || x.Contract.FirstPartyFacility.Facility700Number == identityNumber))
                    : (await _repository.GetQueryable<LitigationRequest, Guid>())
                    .Include(x => x.Contract.SecondPartyCitizen)
                    .Include(x => x.Contract.SecondPartyFacility)
                    .Include(x => x.Delegator)
                    .FirstOrDefault(x => x.InternalStatusId != RequestStatus.Cancelled && x.IsFinished != true && x.RequestCode == requestCode && (x.Contract.SecondPartyCitizen.IdentityNumber == identityNumber || x.Contract.SecondPartyFacility.Facility700Number == identityNumber));

            if (existLitigationRequest == null)
                return new BaseResponse() { IsDone = false, Message = "دعوي غير موجودة أو منتهية" };

            LitigationShortDetailsDto litigationShortDetailsDto = new LitigationShortDetailsDto()
            {
                Id = existLitigationRequest.Id,
                RequestCode = existLitigationRequest.RequestCode,
                PartyName = party == LitigationRequestParty.Plaintiff ? (existLitigationRequest.Contract.FirstPartyFacility != null ? existLitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : existLitigationRequest.Contract.FirstPartyCitizen.FullName) : (existLitigationRequest.Contract.SecondPartyCitizen != null ? existLitigationRequest.Contract.SecondPartyCitizen.FullName : existLitigationRequest.Contract.SecondPartyFacility.ArabicFacilityName),
                PartyIdentity = identityNumber,
                PartyCapacity = party == LitigationRequestParty.Plaintiff ? existLitigationRequest.Contract.FirstPartyCapacity : existLitigationRequest.Contract.SecondPartyCapacity,
            };

            if (party == LitigationRequestParty.Plaintiff)
            {
                litigationShortDetailsDto.DelegationRelationshipTypeId = existLitigationRequest.Delegator != null ? existLitigationRequest.Delegator.DelegationRelationshipTypeId : existLitigationRequest.LitigationRequestOwnerTypeId == LitigationRequestOwnerType.Personal ? DelegationRelationshipType.Individual : DelegationRelationshipType.Facility;

            }
            else if (party == LitigationRequestParty.Defendant)
            {
                // Check type of defendant
                if (existLitigationRequest.Contract.SecondPartyFacilityId.HasValue)
                {
                    // Facility
                    litigationShortDetailsDto.DelegationRelationshipTypeId = DelegationRelationshipType.Facility;
                }
                else if (existLitigationRequest.Contract.SecondPartyCitizenId.HasValue)
                {
                    // Citizen
                    litigationShortDetailsDto.DelegationRelationshipTypeId = DelegationRelationshipType.Individual;
                }

            }

            return new DataResponse<LitigationShortDetailsDto>()
            {
                IsDone = true,
                Data = litigationShortDetailsDto
            };


        }

        public async Task<BaseResponse> AddDelegationRequest(AddDelegationRequest addDelegationRequest)
        {
            var addDelegation = await AddDelegationRequestData(addDelegationRequest);

            return addDelegation;

        }

        public async Task<BaseResponse> GetDelegationRequests(int pageIndex, int pageSize)
        {

            var delegationRequests = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.LitigationRequest)
                .Where(x => x.IsActive == null && x.IsApproved == null && x.ModifiedBy == null)
                .OrderByDescending(x => x.RequestedOn)
                .AsSplitQuery()
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);

            var mappedRequests = _mapper.Map<List<GetDelegationRequest>>(delegationRequests.Results);

            return new DataResponse<PagedResult<GetDelegationRequest>>
            {
                Data =
                   new PagedResult<GetDelegationRequest>
                   {
                       Results = mappedRequests,
                       CurrentPage = delegationRequests.CurrentPage,
                       PageCount = delegationRequests.PageCount,
                       PageSize = delegationRequests.PageSize,
                       TotalCount = delegationRequests.TotalCount
                   },
                IsDone = true
            };

        }


        public async Task<DataResponse<DelegationRequest>> GetDelegationRequestById(Guid delegationRequestId)
        {
            var delegationRequest = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator)
                 .ThenInclude(x => x.Citizen)
                 .ThenInclude(x => x.Nationality)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.Facility)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.DelegationFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.DecisionFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.CommercialRegisterationFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.CustodyFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.RepresentationFile)
                 .Include(x => x.Delegator)
                 .ThenInclude(x => x.EstablishmentFile)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == delegationRequestId);

            if (delegationRequest == null)
                return new DataResponse<DelegationRequest> { IsDone = false, Message = "طلب التفويض غير موجود" };

            return new DataResponse<DelegationRequest>
            {
                IsDone = true,
                Data = delegationRequest
            };

        }


        public async Task<BaseResponse> ApproveRejectDelegationRequest(Guid delegationRequestId, bool isApproved, string? reason = null)
        {


            var delegationRequest = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator)
                .ThenInclude(x => x.DelegatorCitizen)
                .Include(x => x.LitigationRequest)
                .ThenInclude(x => x.Contract.FirstPartyCitizen)
                .FirstOrDefault(x => x.Id == delegationRequestId);

            if (delegationRequest == null)
                return new BaseResponse { IsDone = false, Message = "طلب التفويض غير موجود" };

            if (delegationRequest.IsApproved != null || delegationRequest.ModifiedBy != null)
                return new BaseResponse { IsDone = false, Message = "تم اتخاذ اجراء علي طلب التفويض من قبل" };

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            delegationRequest.IsApproved = isApproved;
            delegationRequest.ModifiedBy = referenceId;
            delegationRequest.ModifiedOn = DateTime.Now;

            if (isApproved)
            {
                var existActiveDelegationRequests = (await _repository.GetQueryable<DelegationRequest, Guid>())
                    .AsNoTracking()
                    .Any(x => x.LitigationRequestId == delegationRequest.LitigationRequestId && x.DelegationParty == delegationRequest.DelegationParty && x.IsActive == true && x.IsApproved == true);

                var existNonActiveDelegationRequests = (await _repository.GetQueryable<DelegationRequest, Guid>())
                    .AsNoTracking()
                    .Any(x => x.LitigationRequestId == delegationRequest.LitigationRequestId && x.DelegationParty == delegationRequest.DelegationParty && x.IsActive == false && x.IsApproved == null);

                if (existActiveDelegationRequests || existNonActiveDelegationRequests)
                {
                    delegationRequest.IsActive = false;
                }
                else
                {
                    delegationRequest.IsActive = true;
                    delegationRequest.ApprovedOn = DateTime.Now;

                    delegationRequest.LitigationRequest.DelegatorId = delegationRequest.DelegatorId;
                    delegationRequest.LitigationRequest.LitigationRequestOwnerTypeId = delegationRequest.LitigationRequestOwnerTypeId;

                    if (delegationRequest.DelegationParty == LitigationRequestParty.Plaintiff)
                    {
                        delegationRequest.LitigationRequest.ApplicantCitizenId = delegationRequest.Delegator.DelegatorCitizenId != null ? (Guid)delegationRequest.Delegator.DelegatorCitizenId : delegationRequest.LitigationRequest.ApplicantCitizenId;

                        if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                        {
                            delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId = delegationRequest.Delegator.CitizenId;
                            delegationRequest.LitigationRequest.Contract.FirstPartyFacilityId = null;
                        }
                        else if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Facility)
                        {
                            delegationRequest.LitigationRequest.Contract.FirstPartyFacilityId = delegationRequest.Delegator.FacilityId;
                            delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId = delegationRequest.Delegator.CitizenId;
                        }
                    }
                    else if (delegationRequest.DelegationParty == LitigationRequestParty.Defendant)
                    {
                        if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                        {
                            delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId = delegationRequest.Delegator.DelegatorCitizenId;
                            delegationRequest.LitigationRequest.Contract.SecondPartyFacilityId = null;
                        }
                        else if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Facility)
                        {
                            delegationRequest.LitigationRequest.Contract.SecondPartyFacilityId = delegationRequest.Delegator.FacilityId;
                            delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId = delegationRequest.Delegator.DelegatorCitizenId;
                        }
                    }

                }

                // Notification
                if (delegationRequest.LitigationRequest.Contract.FirstPartyCitizen is not null)
                {
                    try
                    {


                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.DelegationRequest_Accepted).Result.Data?.Value;
                        var body = string.Empty;
                        msg = msg?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.DelegationRequest_Added).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                        }

                        await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Email },
                            Subject = "قبول طلب تفويض جديد",
                            Body = body
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Mobile },
                            notification_type = "SMS",
                        },
                        new List<SysNotificationDto>()
                        {
                    new SysNotificationDto()
                    {
                         ActorId = (Guid)delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId,
                         CreatedOn = DateTime.Now,
                         ActorTypeId = NotificationActorType.Reviewer,
                         IsViewed = false,
                        ViewedOn = null,
                         RequestId = delegationRequest.LitigationRequest.Id,
                         RequestTypeId = RequestType.LitigationRequest,
                         Message = msg,
                    },
                        }
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
            else
            {
                delegationRequest.Reason = reason;
                delegationRequest.IsActive = false;

                // Notification
                if (delegationRequest.LitigationRequest.Contract.FirstPartyCitizen is not null)
                {
                    try
                    {


                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.DelegationRequest_Rejected).Result.Data?.Value;
                        var body = string.Empty;
                        msg = msg?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                        msg = msg?.Replace("{RejectReason}", reason);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.DelegationRequest_Added).Result.Data?.Value);
                            body = body?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                            body = body?.Replace("{RejectReason}", reason);
                        }

                        await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Email, delegationRequest.Delegator.DelegatorCitizen.Email },
                            Subject = "رفض طلب تفويض جديد",
                            Body = body
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Mobile, delegationRequest.Delegator.DelegatorCitizen.Mobile },
                            notification_type = "SMS",
                        },
                        new List<SysNotificationDto>()
                        {
                        new SysNotificationDto()
                        {
                             ActorId = (Guid)delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId,
                             CreatedOn = DateTime.Now,
                             ActorTypeId = NotificationActorType.Plaintiff,
                             IsViewed = false,
                             ViewedOn = null,
                             RequestId = delegationRequest.LitigationRequest.Id,
                             RequestTypeId = RequestType.LitigationRequest,
                             Message = msg,
                        },
                        new SysNotificationDto()
                        {
                             ActorId = (Guid)delegationRequest.Delegator.DelegatorCitizenId,
                             CreatedOn = DateTime.Now,
                             ActorTypeId = NotificationActorType.Applicant,
                             IsViewed = false,
                             ViewedOn = null,
                             RequestId = delegationRequest.LitigationRequest.Id,
                             RequestTypeId = RequestType.LitigationRequest,
                             Message = msg,
                        }
                        }
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }

            var saveResult = await _repository.SaveChangesAsync(new CancellationToken());
            if (!saveResult.IsDone)
                return saveResult;
            return new BaseResponse { IsDone = true, Message = isApproved ? "تم قبول طلب التفويض" : "تم رفض طلب التفويض" };
        }

        public async Task<BaseResponse> GetActiveDelegatorsForAdmin(int pageIndex, int pageSize)
        {
            var delegationRequests = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest)
                .Where(x => x.IsActive == true && x.LitigationRequest.RequestCode != null)
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);

            var mappedDelegators = _mapper.Map<List<GetDelegationRequestForChangeOrStop>>(delegationRequests.Results);

            return new DataResponse<PagedResult<GetDelegationRequestForChangeOrStop>>
            {
                Data =
                   new PagedResult<GetDelegationRequestForChangeOrStop>
                   {
                       Results = mappedDelegators,
                       CurrentPage = delegationRequests.CurrentPage,
                       PageCount = delegationRequests.PageCount,
                       PageSize = delegationRequests.PageSize,
                       TotalCount = delegationRequests.TotalCount
                   },
                IsDone = true
            };
        }


        public async Task<BaseResponse> GetInActiveDelegatorsForAdmin(Guid litigationRequestId, LitigationRequestParty delegationParty)
        {
            var delegationRequests = delegationParty != LitigationRequestParty.None ? (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest)
                .Where(x => x.LitigationRequestId == litigationRequestId && x.IsActive != true && x.DelegationParty == delegationParty)
                .AsNoTracking()
                : (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest)
                .Where(x => x.LitigationRequestId == litigationRequestId && x.IsActive != true)
                .AsNoTracking();


            if (!delegationRequests.Any())
                return new BaseResponse { IsDone = false, Message = "لا يوجد مفوضين غير مفعلين علي هذه الدعوي" };

            var mappedDelegators = _mapper.Map<List<GetDelegationRequestForChangeOrStop>>(delegationRequests);

            return new DataResponse<List<GetDelegationRequestForChangeOrStop>>
            {
                Data = mappedDelegators
                  ,
                IsDone = true
            };
        }
        
        public async Task<BaseResponse> GetInActiveDelegatorsForUser(Guid litigationRequestId, LitigationRequestParty delegationParty)
        {
            var referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            // Check the party of user
            var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.Contract)
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == litigationRequestId);

            delegationParty = litigationRequest.ApplicantCitizenId == referenceId || litigationRequest.Contract.FirstPartyCitizenId == referenceId ? LitigationRequestParty.Plaintiff : litigationRequest.Contract.SecondPartyCitizenId == referenceId ? LitigationRequestParty.Defendant : LitigationRequestParty.None;


            var delegationRequests = delegationParty != LitigationRequestParty.None ? (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest)
                .Where(x => x.LitigationRequestId == litigationRequestId && x.IsActive != true && x.DelegationParty == delegationParty && x.IsApproved == true)
                .AsNoTracking()
                : (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest)
                .Where(x => x.LitigationRequestId == litigationRequestId && x.IsActive != true && x.IsApproved == true)
                .AsNoTracking();


            if (!delegationRequests.Any())
                return new BaseResponse { IsDone = false, Message = "لا يوجد مفوضين غير مفعلين علي هذه الدعوي" };

            var mappedDelegators = _mapper.Map<List<GetDelegationRequestForChangeOrStop>>(delegationRequests);

            return new DataResponse<List<GetDelegationRequestForChangeOrStop>>
            {
                Data = mappedDelegators
                  ,
                IsDone = true
            };
        }

        public async Task<BaseResponse> StopDelegator(Guid delegationRequestId)
        {
            var delegationRequest = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.Citizen)
                .Include(x => x.Delegator.Facility)
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                .FirstOrDefault(x => x.Id == delegationRequestId);

            if (delegationRequest == null)
                return new BaseResponse { IsDone = false, Message = "طلب التفويض غير موجود" };

            if (delegationRequest.IsActive != true || delegationRequest.IsApproved != true)
                return new BaseResponse { IsDone = false, Message = "لا يمكن ايقاف هذا التفويض" };

            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            delegationRequest.IsActive = false;
            delegationRequest.ModifiedBy = referenceId;
            delegationRequest.ModifiedOn = DateTime.Now;

            if (delegationRequest.Delegator is not null && delegationRequest.LitigationRequest is not null)
            {

                // if the party is citizen and not draft account 
                if (delegationRequest.Delegator.Citizen is not null && delegationRequest.Delegator.Citizen.IsDraftAccount != true)
                {
                    if (delegationRequest.DelegationParty == LitigationRequestParty.Plaintiff && delegationRequest.LitigationRequest.Contract.FirstPartyCitizen is not null)
                    {
                        delegationRequest.LitigationRequest.ApplicantCitizenId = delegationRequest.Delegator.Citizen.Id;
                        delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId = delegationRequest.Delegator.Citizen.Id;
                        delegationRequest.LitigationRequest.DelegatorId = delegationRequest.DelegatorId;
                    }
                    else if (delegationRequest.DelegationParty == LitigationRequestParty.Defendant && delegationRequest.LitigationRequest.Contract.SecondPartyCitizen is not null)
                    {
                        delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId = delegationRequest.Delegator.Citizen.Id;
                    }

                }

                // if the party is citizen and draft account or facility
                if ((delegationRequest.Delegator.Citizen is not null && delegationRequest.Delegator.Citizen.IsDraftAccount == true) || (delegationRequest.Delegator.Facility is not null))
                {
                    if (delegationRequest.DelegationParty == LitigationRequestParty.Plaintiff)
                    {
                        delegationRequest.LitigationRequest.ApplicantCitizenId = null;
                        delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId = null;
                        delegationRequest.LitigationRequest.Contract.FirstPartyFacilityId = null;
                        delegationRequest.LitigationRequest.DelegatorId = null;

                    }
                    else if (delegationRequest.DelegationParty == LitigationRequestParty.Defendant)
                    {
                        delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId = null;
                        delegationRequest.LitigationRequest.Contract.SecondPartyFacilityId = null;
                    }


                }

                var saveChanges = await _repository.SaveChangesAsync();

                if (!saveChanges.IsDone)
                    return saveChanges;

                // Notification
                if(delegationRequest.DelegationParty == LitigationRequestParty.Plaintiff && delegationRequest.LitigationRequest.Contract.FirstPartyCitizen != null && delegationRequest.Delegator.DelegatorCitizen != null)
                {
                    try
                    {


                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Stopped).Result.Data?.Value;
                        var body = string.Empty;
                        msg = msg?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        msg = msg?.Replace("{Party}", "المدعي");
                        msg = msg?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Stopped).Result.Data?.Value);
                            body = body?.Replace("{Party}", "المدعي");
                            body = body?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                            body = body?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        }

                        await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Email , delegationRequest.Delegator.DelegatorCitizen.Email },
                            Subject = "ايقاف المفوض",
                            Body = body
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Mobile, delegationRequest.Delegator.DelegatorCitizen.Mobile },
                            notification_type = "SMS",
                        },
                        new List<SysNotificationDto>()
                        {
                    new SysNotificationDto()
                    {
                         ActorId = (Guid)delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId,
                         CreatedOn = DateTime.Now,
                         ActorTypeId = NotificationActorType.Applicant,
                         IsViewed = false,
                        ViewedOn = null,
                         RequestId = delegationRequest.LitigationRequest.Id,
                         RequestTypeId = RequestType.LitigationRequest,
                         Message = msg,
                    },new SysNotificationDto()
                    {
                         ActorId = (Guid)delegationRequest.Delegator.DelegatorCitizenId,
                         CreatedOn = DateTime.Now,
                         ActorTypeId = NotificationActorType.Applicant,
                         IsViewed = false,
                        ViewedOn = null,
                         RequestId = delegationRequest.LitigationRequest.Id,
                         RequestTypeId = RequestType.LitigationRequest,
                         Message = msg,
                    },
                        }
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if(delegationRequest.DelegationParty == LitigationRequestParty.Defendant && delegationRequest.LitigationRequest.Contract.SecondPartyCitizen != null && delegationRequest.Delegator.DelegatorCitizen != null)
                {
                    try
                    {


                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Stopped).Result.Data?.Value;
                        var body = string.Empty;
                        msg = msg?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        msg = msg?.Replace("{Party}", "المدعي عليه");
                        msg = msg?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Stopped).Result.Data?.Value);
                            body = body?.Replace("{party}", "المدعي عليه");
                            body = body?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                            body = body?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        }

                        await _notificationService.SendNotifications(
                        new SendEmailDto()
                        {
                            To = new List<string>() { delegationRequest.LitigationRequest.Contract.SecondPartyCitizen.Email,  delegationRequest.Delegator.DelegatorCitizen.Email},
                            Subject = "ايقاف المفوض",
                            Body = body
                        },
                        new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { delegationRequest.LitigationRequest.Contract.SecondPartyCitizen.Mobile , delegationRequest.Delegator.DelegatorCitizen.Mobile },
                            notification_type = "SMS",
                        },
                        new List<SysNotificationDto>()
                        {
                    new SysNotificationDto()
                    {
                         ActorId = (Guid)delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId,
                         CreatedOn = DateTime.Now,
                         ActorTypeId = NotificationActorType.Reviewer,
                         IsViewed = false,
                        ViewedOn = null,
                         RequestId = delegationRequest.LitigationRequest.Id,
                         RequestTypeId = RequestType.LitigationRequest,
                         Message = msg,
                    },new SysNotificationDto()
                    {
                         ActorId = (Guid)delegationRequest.Delegator.DelegatorCitizenId,
                         CreatedOn = DateTime.Now,
                         ActorTypeId = NotificationActorType.Reviewer,
                         IsViewed = false,
                        ViewedOn = null,
                         RequestId = delegationRequest.LitigationRequest.Id,
                         RequestTypeId = RequestType.LitigationRequest,
                         Message = msg,
                    },
                        }
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }


                    return new BaseResponse { IsDone = true, Message = "تم ايقاف التفويض" };

            }

            return new BaseResponse { IsDone = false, Message = "حدث خطأ اثناء ايقاف التفويض" };
        }

        public async Task<BaseResponse> ChangeDelegator(Guid delegationRequestId)
        {

            var delegationRequest = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.Citizen)
                .Include(x => x.Delegator.Facility)
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                .FirstOrDefault(x => x.Id == delegationRequestId);

            if (delegationRequest is not null)
            {
                if (delegationRequest.DelegationParty == LitigationRequestParty.Plaintiff)
                {
                    delegationRequest.LitigationRequest.ApplicantCitizenId = delegationRequest.Delegator.DelegatorCitizenId != null ? (Guid)delegationRequest.Delegator.DelegatorCitizenId : delegationRequest.LitigationRequest.ApplicantCitizenId;

                    if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                    {
                        delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId = delegationRequest.Delegator.CitizenId;
                        delegationRequest.LitigationRequest.Contract.FirstPartyFacilityId = null;
                    }
                    else if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Facility)
                    {
                        delegationRequest.LitigationRequest.Contract.FirstPartyFacilityId = delegationRequest.Delegator.FacilityId;
                        delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId = delegationRequest.Delegator.CitizenId;
                    }
                }
                else if (delegationRequest.DelegationParty == LitigationRequestParty.Defendant)
                {
                    if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Individual)
                    {
                        delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId = delegationRequest.Delegator.CitizenId;
                        delegationRequest.LitigationRequest.Contract.SecondPartyFacilityId = null;
                    }
                    else if (delegationRequest.Delegator.DelegationRelationshipTypeId == DelegationRelationshipType.Facility)
                    {
                        delegationRequest.LitigationRequest.Contract.SecondPartyFacilityId = delegationRequest.Delegator.FacilityId;
                        delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId = delegationRequest.Delegator.CitizenId;
                    }
                }


                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);

                delegationRequest.IsActive = true;
                delegationRequest.IsApproved = true;
                delegationRequest.ModifiedBy = referenceId;
                delegationRequest.ModifiedOn = DateTime.Now;
                delegationRequest.ApprovedOn = DateTime.Now;
                delegationRequest.LitigationRequest.DelegatorId = delegationRequest.DelegatorId;
                delegationRequest.LitigationRequest.LitigationRequestOwnerTypeId = delegationRequest.LitigationRequestOwnerTypeId;

                // set other delegation requests as inActive
                var otherDelegationRequests = (await _repository.GetQueryable<DelegationRequest, Guid>())
                    .Include(x => x.Delegator.DelegatorCitizen)
                    .Where(x => x.LitigationRequestId == delegationRequest.LitigationRequestId && x.DelegationParty == delegationRequest.DelegationParty && x.IsActive == true)
                    .ToList();

                if (otherDelegationRequests.Any())
                {
                    foreach (var item in otherDelegationRequests)
                    {
                        item.IsActive = false;
                        item.IsApproved = false;

                        item.ModifiedBy = referenceId;
                        item.ModifiedOn = DateTime.Now;
                    }
                }

                var saveChanges = await _repository.SaveChangesAsync();

                if (!saveChanges.IsDone)
                    return saveChanges;

                // Notification
                if (delegationRequest.DelegationParty == LitigationRequestParty.Plaintiff && delegationRequest.LitigationRequest.Contract.FirstPartyCitizen != null)
                {
                    try
                    {


                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Modified).Result.Data?.Value;
                        var body = string.Empty;
                        msg = msg?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        msg = msg?.Replace("{Party}", "المدعي");
                        msg = msg?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Modified).Result.Data?.Value);
                            body = body?.Replace("{Party}", "المدعي");
                            body = body?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                            body = body?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        }

                        var emails = new SendEmailDto()
                        {
                            To = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Email },
                            Subject = "ايقاف المفوض",
                            Body = body
                        };
                        var sms = new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { delegationRequest.LitigationRequest.Contract.FirstPartyCitizen.Mobile },
                            notification_type = "SMS",
                        };

                        var notifications = new List<SysNotificationDto>()
                        {
                            new SysNotificationDto()
                            {
                                 ActorId = (Guid)delegationRequest.LitigationRequest.Contract.FirstPartyCitizenId,
                                 CreatedOn = DateTime.Now,
                                 ActorTypeId = NotificationActorType.Applicant,
                                 IsViewed = false,
                                ViewedOn = null,
                                 RequestId = delegationRequest.LitigationRequest.Id,
                                 RequestTypeId = RequestType.LitigationRequest,
                                 Message = msg,
                            }
                        };

                        if (otherDelegationRequests.FirstOrDefault() != null)
                        {
                            emails.To.Add(otherDelegationRequests.FirstOrDefault().Delegator.DelegatorCitizen.Email);
                            sms.recipients.Add(otherDelegationRequests.FirstOrDefault().Delegator.DelegatorCitizen.Mobile);

                            notifications.Add(new SysNotificationDto()
                            {
                                ActorId = (Guid)otherDelegationRequests.FirstOrDefault().Delegator.DelegatorCitizenId,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Applicant,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = delegationRequest.LitigationRequest.Id,
                                RequestTypeId = RequestType.LitigationRequest,
                                Message = msg,
                            });
                        }

                        await _notificationService.SendNotifications(
                        emails,
                        sms,
                        notifications
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }
                else if (delegationRequest.DelegationParty == LitigationRequestParty.Defendant && delegationRequest.LitigationRequest.Contract.SecondPartyCitizen != null && otherDelegationRequests.FirstOrDefault() != null)
                {
                    try
                    {


                        var msg = _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Stopped).Result.Data?.Value;
                        var body = string.Empty;
                        msg = msg?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        msg = msg?.Replace("{party}", "المدعي عليه");
                        msg = msg?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                        var pathToFile = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplete", "mail.html");
                        using (StreamReader streamReader = File.OpenText(pathToFile))
                        {
                            body = await streamReader.ReadToEndAsync();
                            body = body?.Replace("{content}", _sysSettingService.GetBySettingKey(SysSettingKeyConstants.Delegator_Stopped).Result.Data?.Value);
                            body = body?.Replace("{party}", "المدعي عليه");
                            body = body?.Replace("{LitigationCode}", delegationRequest.LitigationRequest?.RequestCode);
                            body = body?.Replace("{CitizenName}", delegationRequest.Delegator?.DelegatorCitizen.FullName);
                        }

                        var emails = new SendEmailDto()
                        {
                            To = new List<string>() { delegationRequest.LitigationRequest.Contract.SecondPartyCitizen.Email },
                            Subject = "ايقاف المفوض",
                            Body = body
                        };
                        var sms = new SMSRequestDto()
                        {
                            message_text = msg,
                            recipients = new List<string>() { delegationRequest.LitigationRequest.Contract.SecondPartyCitizen.Mobile },
                            notification_type = "SMS",
                        };

                        var notifications = new List<SysNotificationDto>()
                        {
                            new SysNotificationDto()
                            {
                                 ActorId = (Guid)delegationRequest.LitigationRequest.Contract.SecondPartyCitizenId,
                                 CreatedOn = DateTime.Now,
                                 ActorTypeId = NotificationActorType.Applicant,
                                 IsViewed = false,
                                ViewedOn = null,
                                 RequestId = delegationRequest.LitigationRequest.Id,
                                 RequestTypeId = RequestType.LitigationRequest,
                                 Message = msg,
                            }
                        };

                        if (otherDelegationRequests.FirstOrDefault() != null)
                        {
                            emails.To.Add(otherDelegationRequests.FirstOrDefault().Delegator.DelegatorCitizen.Email);
                            sms.recipients.Add(otherDelegationRequests.FirstOrDefault().Delegator.DelegatorCitizen.Mobile);

                            notifications.Add(new SysNotificationDto()
                            {
                                ActorId = (Guid)otherDelegationRequests.FirstOrDefault().Delegator.DelegatorCitizenId,
                                CreatedOn = DateTime.Now,
                                ActorTypeId = NotificationActorType.Applicant,
                                IsViewed = false,
                                ViewedOn = null,
                                RequestId = delegationRequest.LitigationRequest.Id,
                                RequestTypeId = RequestType.LitigationRequest,
                                Message = msg,
                            });
                        }

                        await _notificationService.SendNotifications(
                        emails,
                        sms,
                        notifications
                        );
                    }
                    catch (Exception ex)
                    {

                    }
                }



                return new BaseResponse { IsDone = true, Message = "تم تغيير المفوض" };
            }

            return new BaseResponse { IsDone = false, Message = "حدث خطأ اثناء تغيير المفوض" };
        }

        public async Task<BaseResponse> GetLitigationRequestsWithoutDelegatorsForAdmin(int pageIndex, int pageSize)
        {
            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.LitigationDelegationRequests)
                .Include(x => x.Contract.FirstPartyCitizen)
                .Include(x => x.Contract.SecondPartyCitizen)
                .Where(x => x.InternalStatusId != RequestStatus.Cancelled && x.IsFinished != true && x.LitigationDelegationRequests.Any(x => x.IsActive != true && x.IsApproved == true) && x.RequestCode != null)
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);
            var mappedRequests = _mapper.Map<List<GetLitigationRequestWithoutDelegator>>(litigationRequests.Results);
            return new DataResponse<PagedResult<GetLitigationRequestWithoutDelegator>>
            {
                Data =
                   new PagedResult<GetLitigationRequestWithoutDelegator>
                   {
                       Results = mappedRequests,
                       CurrentPage = litigationRequests.CurrentPage,
                       PageCount = litigationRequests.PageCount,
                       PageSize = litigationRequests.PageSize,
                       TotalCount = litigationRequests.TotalCount
                   },
                IsDone = true
            };
        }

        public async Task<BaseResponse> GetActiveDelegatorsForUser(int pageIndex, int pageSize)
        {
            var referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var delegationRequests = (await _repository.GetQueryable<DelegationRequest, Guid>())
                .Include(x => x.Delegator.DelegatorCitizen)
                .Include(x => x.LitigationRequest)
                .Where(x => x.IsActive == true && x.LitigationRequest.RequestCode != null && x.Delegator.CitizenId == referenceId)
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);

            var mappedDelegators = _mapper.Map<List<GetDelegationRequestForChangeOrStop>>(delegationRequests.Results);

            return new DataResponse<PagedResult<GetDelegationRequestForChangeOrStop>>
            {
                Data =
                   new PagedResult<GetDelegationRequestForChangeOrStop>
                   {
                       Results = mappedDelegators,
                       CurrentPage = delegationRequests.CurrentPage,
                       PageCount = delegationRequests.PageCount,
                       PageSize = delegationRequests.PageSize,
                       TotalCount = delegationRequests.TotalCount
                   },
                IsDone = true
            };
        }


        public async Task<BaseResponse> GetLitigationRequestsWithoutDelegatorsForUser(int pageIndex, int pageSize)
        {
            var referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            var litigationRequests = (await _repository.GetQueryable<LitigationRequest, Guid>())
                .Include(x => x.LitigationDelegationRequests)
                .ThenInclude(x => x.Delegator)
                .Include(x => x.Contract.FirstPartyCitizen)
                .Include(x => x.Contract.SecondPartyCitizen)
                .Where(x => x.InternalStatusId != RequestStatus.Cancelled && x.IsFinished != true && x.LitigationDelegationRequests.Any(x => x.IsActive != true && x.IsApproved == true)
                && x.LitigationDelegationRequests.Any(x => x.Delegator.CitizenId == referenceId)
                && x.RequestCode != null
                && (x.ApplicantCitizenId == referenceId || x.Contract.SecondPartyCitizenId == referenceId))
                .AsNoTracking()
                .GetPaged(pageIndex, pageSize);

            var mappedRequests = _mapper.Map<List<GetLitigationRequestWithoutDelegator>>(litigationRequests.Results);
            
            foreach (var item in mappedRequests)
            {
                
            }

            return new DataResponse<PagedResult<GetLitigationRequestWithoutDelegator>>
            {
                Data =
                   new PagedResult<GetLitigationRequestWithoutDelegator>
                   {
                       Results = mappedRequests,
                       CurrentPage = litigationRequests.CurrentPage,
                       PageCount = litigationRequests.PageCount,
                       PageSize = litigationRequests.PageSize,
                       TotalCount = litigationRequests.TotalCount
                   },
                IsDone = true
            };
        }
        #endregion

    }

}