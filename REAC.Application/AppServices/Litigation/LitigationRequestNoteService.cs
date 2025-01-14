using AutoMapper;
using Azure.Core;
using Core.GenericRepository.BaseRepository;
using Core.GenericRepository.Shared.Pagging;
using Core.Log.Application.Contracts.IAppServices;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Reconciliation.Process;
using REAC.Application.Contracts.IAppServices;
using REAC.Application.Contracts.IAppServices.Litigation;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using System.Buffers.Text;
using System.IdentityModel.Tokens.Jwt;

namespace REAC.Application.AppServices.Litigation
{
    public class LitigationRequestNoteService : BasicService<LitigationRequestNote, Guid>, ILitigationRequestNoteService
    {
        private readonly IJwtReader _jwtReader;
        private readonly ISysSettingService _sysSettingService;

        public LitigationRequestNoteService(IEntityRepository repository, IMapper mapper, ILoggerManager loggerManager, IJwtReader jwtReader, ISysSettingService sysSettingService) : base(repository, mapper, loggerManager, jwtReader)
        {
            _jwtReader = jwtReader;
            _sysSettingService = sysSettingService;
        }


        #region Portal
        public async Task<BaseResponse> AddLitigationNote(LitigationRequestNoteCreateUpdateDto litigationNote)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);


                if (litigationNote == null || referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var noteObject = new LitigationRequestNote()
                {
                    Id = new Guid(),
                    CreatedBy = referenceId,
                    CreatedOn = DateTime.Now,
                    LitigationRequestId = litigationNote.LitigationRequestId,
                    Notes = litigationNote.Notes,
                    Title = litigationNote.Title,
                    LitigationRequestCategoryId = litigationNote.LitigationRequestCategoryId,
                };

                var result = await _repository.AddAsync<LitigationRequestNote, Guid>(noteObject);

                if (result.IsDone && litigationNote.NotesFileId != Guid.Empty && litigationNote.NotesFileId != null)
                {
                    var fileAddResult = await _repository.AddAsync<LitigationRequestNoteFile, Guid>(new LitigationRequestNoteFile()
                    {
                        Id = new Guid(),
                        CreatedBy = referenceId != Guid.Empty ? referenceId : null,
                        CreatedOn = DateTime.Now,
                        NoteId = noteObject.Id,
                        FileManagerId = (Guid)litigationNote.NotesFileId
                    });
                };

                var saveResponse = await _repository.SaveChangesAsync();
                if (!saveResponse.IsDone)
                    return saveResponse;

                var response = await GetNotesByLitigationRequestId(litigationNote.LitigationRequestId);

                return new DataResponse<List<LitigationRequestNotesDto>>() { IsDone = true, Message = "تم الاضافه بنجاح", Data = response.Data };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }

        }

        public async Task<BaseResponse> EditLitigationNote(LitigationRequestNoteCreateUpdateDto requestLitigationNote)
        {
            try
            {
                Guid referenceId = Guid.Empty;
                _jwtReader.GetReferenceId(ref referenceId);


                if (requestLitigationNote == null || referenceId == Guid.Empty)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };

                var dbNote = (await _repository.GetQueryable<LitigationRequestNote, Guid>())
                    .Include(x => x.LitigationRequestNoteFiles)
                    .FirstOrDefault(x => x.Id == requestLitigationNote.Id);

                if (dbNote == null)
                    return new BaseResponse() { IsDone = false, Message = "غير موجود" };



                dbNote.UpdatedOn = DateTime.Now;
                dbNote.UpdatedBy = referenceId;
                dbNote.Title = requestLitigationNote.Title;
                dbNote.Notes = requestLitigationNote.Notes;


                var updateResult = _repository.Update<LitigationRequestNote, Guid>(dbNote);


                if (!updateResult.IsDone)
                    return updateResult;


                if (updateResult.IsDone && requestLitigationNote.NotesFileId != Guid.Empty && requestLitigationNote.NotesFileId != null)
                {


                    if (dbNote.LitigationRequestNoteFiles is not null)
                    {
                        foreach (var file in dbNote.LitigationRequestNoteFiles)
                        {
                            file.UpdatedOn = DateTime.Now;
                            file.UpdatedBy = referenceId;
                            file.FileManagerId = (Guid)requestLitigationNote.NotesFileId;
                        }

                    }
                    else
                    {
                        var fileAddResult = _repository.Update<LitigationRequestNoteFile, Guid>(new LitigationRequestNoteFile()
                        {
                            Id = new Guid(),
                            CreatedBy = referenceId != null ? referenceId : null,
                            CreatedOn = DateTime.Now,
                            NoteId = requestLitigationNote.Id,
                            FileManagerId = (Guid)requestLitigationNote.NotesFileId
                        });
                    }


                };

                var result = await _repository.SaveChangesAsync();
                if (!result.IsDone)
                    return result;

                var response = await GetNotesByLitigationRequestId(requestLitigationNote.LitigationRequestId);

                return new DataResponse<List<LitigationRequestNotesDto>>() { IsDone = true, Message = "تم التعديل بنجاح", Data = response.Data };

            }
            catch (Exception ex)
            {
                return new BaseResponse() { IsDone = false, Message = "خطأ داخلي" };

            }

        }

        public async Task<BaseResponse> GetLitigationNotes(Guid litigationRequestId, int[] litigationRequestCategoryIds)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.LitigationRequestNotes.Where(l => litigationRequestCategoryIds.Contains((int)l.LitigationRequestCategoryId)))
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestNotes.Where(l => litigationRequestCategoryIds.Contains((int)l.LitigationRequestCategoryId)))
                          .ThenInclude(x => x.LitigationRequestNoteFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.Area)
                          .Include(x => x.City)
                          .Include(x => x.LitigationRequestReconciliationReports)
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
                          .Where(x => x.Id == litigationRequestId).AsNoTracking().ToList().FirstOrDefault();


                if (litigationRequest == null)
                    return new BaseResponse() { IsDone = false, Message = "Not Found" };

                var allowAdd = true;

                var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

                var notes = _mapper.Map<List<LitigationRequestNotesDto>>(litigationRequest?.LitigationRequestNotes);

                return new DataResponse<LitigationRequestForNotes>()
                {
                    Data = new LitigationRequestForNotes()
                    {
                        LitigationRequestShortDto = request,
                        AllowAdd = allowAdd,
                        Notes = (notes.Count > 0) ? notes : null,
                    },
                    IsDone = true,
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestForNotes>>() { IsDone = false, Message = "خطأ داخلي" };

            }

        }

        private async Task<DataResponse<List<LitigationRequestNotesDto>>> GetNotesByLitigationRequestId(Guid litigationRequestId)
        {
            try
            {
                var result = (await _repository.GetQueryable<LitigationRequestNote, Guid>())
                    .Include(x => x.Citizen)
                    .Include(x => x.LitigationRequestNoteFiles)
                    .ThenInclude(x => x.FileManager)
                    .Where(x => x.LitigationRequestId == litigationRequestId)
                    .OrderByDescending(x => x.CreatedOn)
                    .AsNoTracking()
                    .ToList();


                if (result is null)
                    return new DataResponse<List<LitigationRequestNotesDto>>() { IsDone = false, Message = "طلب التعديل غير موجود" };

                var data = _mapper.Map<List<LitigationRequestNotesDto>>(result);

                return new DataResponse<List<LitigationRequestNotesDto>>()
                {
                    IsDone = true,
                    Message = "",
                    Data = data

                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestNotesDto>>() { IsDone = false, Message = "خطأ داخلي" };
            }


        }
        #endregion

        #region BackOffice

        public async Task<BaseResponse> GetLitigationRequestNotesForBackoffice(int pageIndex, int pageSize,int litigationRequestCategoryId)
        {
            Guid referenceId = Guid.Empty;
            _jwtReader.GetReferenceId(ref referenceId);

            PagedResult<LitigationRequestReformerAssigningRequest> litigationRequestsReformerAssigningRequests = null;

            var entitiesQuerable = (await _repository.GetQueryable<LitigationRequestReformerAssigningRequest, Guid>())
                          .Include(x => x.ReformerCitizen)
                          .Include(x => x.LitigationRequest.LitigationRequestNotes)
                         .Include(x => x.LitigationRequest)

                         .Include(x => x.LitigationRequest.ApplicantCitizen)
                         .Include(x => x.LitigationRequest.InternalStatus)
                         .Include(x => x.LitigationRequest.ExternalStatus)

                         .Include(x => x.LitigationRequest.LitigationRequestCategory)
                          .Include(x => x.LitigationRequest.Contract)
                          .Include(x => x.LitigationRequest.Contract.FirstPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.FirstPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyFacility)
                         .Include(x => x.LitigationRequest.Contract.SecondPartyCitizen)
                         .Include(x => x.LitigationRequest.Contract.LitigationRequestContractType);


            litigationRequestsReformerAssigningRequests = entitiesQuerable
                .Where(x => x.LitigationRequest.LitigationRequestNotes.Any(l => l.LitigationRequestCategoryId == (LitigationRequestCategory)litigationRequestCategoryId))
                                       .OrderByDescending(x => x.LitigationRequest.CreatedOn)
                                       .AsSplitQuery()
                                       .AsNoTracking()
                                       .GetPaged(pageIndex, pageSize);


            List<CustomLitigationRequestMircoAllDto> litigationRequestMircoDtos = new List<CustomLitigationRequestMircoAllDto>();
            if (litigationRequestsReformerAssigningRequests != null)
            {

                int reconciliationPeriod = 0;
                int.TryParse(_sysSettingService.GetBySettingKey(SysSettingKeyConstants.Reconciliation_Period).Result.Data?.Value, out reconciliationPeriod);


                foreach (var litigationRequestReformerAssigningRequest in litigationRequestsReformerAssigningRequests.Results)
                {
                    int daysPassed = (DateTime.Now - (DateTime)litigationRequestReformerAssigningRequest.RequestOn).Days;
                    litigationRequestReformerAssigningRequest.LitigationRequest.ReconciliationPeriod = Math.Max(0, reconciliationPeriod - daysPassed);
                }
                foreach (var litigationRequest in litigationRequestsReformerAssigningRequests.Results)
                {
                    CustomLitigationRequestMircoAllDto litigationRequestMircoDto = new CustomLitigationRequestMircoAllDto();
                    litigationRequestMircoDto.Id = litigationRequest.Id;
                    litigationRequestMircoDto.RequestId = litigationRequest.Id;
                    litigationRequestMircoDto.RequestDate = litigationRequest.LitigationRequest.CreatedOn;
                    litigationRequestMircoDto.ReconciliationPeriod = litigationRequest.LitigationRequest.ReconciliationPeriod;
                    litigationRequestMircoDto.RequestCode = litigationRequest.LitigationRequest.RequestCode;
                    litigationRequestMircoDto.FirstPartyName = (litigationRequest.LitigationRequest.Contract.FirstPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.FirstPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.FirstPartyCitizen.FullName;
                    litigationRequestMircoDto.SecondPartyName = (litigationRequest.LitigationRequest.Contract.SecondPartyCitizen == null) ? litigationRequest.LitigationRequest.Contract.SecondPartyFacility.ArabicFacilityName : litigationRequest.LitigationRequest.Contract.SecondPartyCitizen.FullName;
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
            }
            return new DataResponse<PagedResult<CustomLitigationRequestMircoAllDto>>
            {
                Data =
                              new PagedResult<CustomLitigationRequestMircoAllDto>
                              {
                                  Results = litigationRequestMircoDtos,
                                  CurrentPage = litigationRequestsReformerAssigningRequests.CurrentPage,
                                  PageCount = litigationRequestsReformerAssigningRequests.PageCount,
                                  PageSize = litigationRequestsReformerAssigningRequests.PageSize,
                                  TotalCount = litigationRequestsReformerAssigningRequests.TotalCount
                              },
                IsDone = true
            };
        }

        public async Task<BaseResponse> GetLitigationNotesByLitigationRequestIdForBackoffice(Guid litigationRequestId)
        {
            try
            {
                var litigationRequest = (await _repository.GetQueryable<LitigationRequest, Guid>())
                          .Include(x => x.LitigationRequestNotes)
                          .ThenInclude(x => x.Citizen)
                          .Include(x => x.LitigationRequestNotes)
                          .ThenInclude(x => x.LitigationRequestNoteFiles)
                          .ThenInclude(x => x.FileManager)
                          .Include(x => x.Area)
                          .Include(x => x.City)
                          .Include(x => x.LitigationRequestReconciliationReports)
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
                          .Include(x => x.Contract.LitigationRequestContractType).AsNoTracking()
                          .FirstOrDefault(x => x.Id == litigationRequestId);


                if (litigationRequest == null)
                    return new BaseResponse() { IsDone = false, Message = "Not Found" };

                var allowAdd = true;

                var request = _mapper.Map<LitigationRequestShortDto>(litigationRequest);

                var notes = _mapper.Map<List<LitigationRequestNotesDto>>(litigationRequest?.LitigationRequestNotes);

                return new DataResponse<LitigationRequestForNotes>()
                {
                    Data = new LitigationRequestForNotes()
                    {
                        LitigationRequestShortDto = request,
                        AllowAdd = allowAdd,
                        Notes = (notes.Count > 0) ? notes : null,
                    },
                    IsDone = true,
                };
            }
            catch (Exception ex)
            {
                return new DataResponse<List<LitigationRequestForNotes>>() { IsDone = false, Message = "خطأ داخلي" };

            }

        }
        #endregion

    }
}
