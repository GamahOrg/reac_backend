using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration;

namespace REAC.Application.Contracts.IAppServices.Integration
{
    public interface IComplaintsService
    {
        Task<DataResponse<RegionListResponseDto>> GetRegionListAsync();
        Task<DataResponse<ContactResponseDto>> GetContactAsync(string nationalId);
        Task<BaseResponse> UploadComplaintFileAsync(UploadComplaintFileNoteDto input);
        Task<BaseResponse> CreateComplaintAsync(CreateComplaintRequestDto request);
        Task<DataResponse<CasesByContactResponseDto>> GetCasesByContactAsync(string contactId);
    }
}
