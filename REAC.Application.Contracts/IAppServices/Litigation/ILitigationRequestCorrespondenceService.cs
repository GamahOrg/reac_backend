using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Application.Contracts.Dtos.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;

namespace REAC.Application.Contracts.IAppServices.Litigation
{
    public interface ILitigationRequestCorrespondenceService : IBasicService<LitigationRequestCorrespondence, Guid>
    {
        #region For Reformer/Arbitrator
        Task<BaseResponse> AddLitigationCorrespondence(LitigationRequestCorrespondenceCreateUpdateDto litigationCorrespondence);
        Task<BaseResponse> GetLitigationCorrespondences(Guid litigationRequestId, int[] litigationRequestCategoryIds);
        #endregion


        #region Correspondences Reply For Defendant/plaintiff

        Task<BaseResponse> GetLitigationRequestsForCitizinCorrespondenceReply(int pageIndex, int pageSize, string? searchKey, LitigationListDto model);
        Task<BaseResponse> GetLitigationCorrespondencesByLitigationRequestId(Guid litigationRequestId);
        Task<BaseResponse> AddLitigationCorrespondenceReply(LitigationRequestCorrespondenceReplyCreateUpdateDto litigationCorrespondenceReply);
        #endregion

        #region Backoffice
        Task<BaseResponse> GetLitigationRequestArbitrationCorrespondence(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<BaseResponse> GetLitigationRequestReconciliationCorrespondence(int pageIndex, int pageSize, CancellationToken cancellationToken);
        #endregion
    }
}
