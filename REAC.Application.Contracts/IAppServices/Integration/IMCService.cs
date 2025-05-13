using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;

namespace REAC.Application.Contracts.IAppServices.Integration
{
    public interface IMCService
    {

        Task<DataResponse<CRInformationByCRNumberUpdatedDto>> GetCRInformationByCRNumber(string crNumber);
        Task<DataResponse<CRInformationByCROwnersUpdatedDto>> GetCRInformationByCROwners(string crNumber);
        Task<DataResponse<CRInformationByCRNumberDto>> GetCRNationalNumberInformation(string CRNationalNumber);
        Task<BaseResponse> ValidateRelatedCRs(string crNumber, string identifierTypeID, string identifierNo, string nationalityID);
    }
}
