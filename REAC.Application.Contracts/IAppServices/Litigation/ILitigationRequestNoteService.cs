using Core.Shared.Responses;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestNoteService : IBasicService<LitigationRequestNote, Guid>
    {
        Task<BaseResponse> AddLitigationNote(LitigationRequestNoteCreateUpdateDto litigationNote);
        Task<BaseResponse> GetLitigationNotes(Guid litigationRequestId, int[] litigationRequestCategoryIds);
        Task<BaseResponse> EditLitigationNote(LitigationRequestNoteCreateUpdateDto litigationNote);

        Task<BaseResponse> GetLitigationRequestNotesForBackoffice(int pageIndex, int pageSize, int litigationRequestCategoryId);
        Task<BaseResponse> GetLitigationNotesByLitigationRequestIdForBackoffice(Guid litigationRequestId);
    }
}
