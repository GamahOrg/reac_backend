using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Litigation.Create.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Application.Contracts.Dtos.Litigation.List;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;

namespace REAC.Application.Contracts.IMobileAppServices.Litigation
{
    public interface IMobileLitigationRequestService
    {


        #region pagination
        public class PaginatedList<T>
        {
            public List<T> Items { get; private set; }
            public int PageIndex { get; private set; }
            public int TotalPages { get; private set; }

            public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
            {
                PageIndex = pageIndex;
                TotalPages = (int)Math.Ceiling(count / (double)pageSize);
                Items = items;
            }

            public bool HasPreviousPage => PageIndex > 1;
            public bool HasNextPage => PageIndex < TotalPages;

            public static async Task<PaginatedList<T>> CreateAsync(IList<T> source, int pageIndex, int pageSize)
            {
                var count = source.Count();
                var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
                return new PaginatedList<T>(items, count, pageIndex, pageSize);
            }
        }
        #endregion


        Task<BaseResponse> List(int pageIndex, int pageSize, string? searchKey, List<int> statuses, List<int> types);

        Task<BaseResponse> GetById(Guid litigationId);
        Task<BaseResponse> GetLitigationRequestEvents(Guid litigationId);

        #region Agrrement
        Task<BaseResponse> GetCitizenLitigationRequestForAgreementRequestReply(int pageIndex, int pageSize);
        Task<BaseResponse> GetCitizenLitigationRequestForAgreementRequestReply(Guid requestId);
        Task<BaseResponse> AddCitizenLitigationRequestForAgreementRequestReply(Guid requestId, ApproveRejectStatus agreementRequestStatus);
        #endregion

    }
}
