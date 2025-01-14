using Core.Shared.Responses;
using REAC.Application.Contracts.Dtos.Integration.GEE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Integration
{
    public interface INafathService
    {
        Task<DataResponse<NafathResponseDto>> Nafath(NafathRequestDto nafathRequestDto, CancellationToken cancellationToken);
        Task CheckNafathStatus();

        Task<BaseResponse> CheckStatusForMobile(string transactionId);

        Task<DataResponse<NafathResponseDto>> NafathAgreementRequest(NafathRequestDto nafathRequestDto, NafathAgreementRequestTransactionsDto listInput);

        Task CheckNafathAgreementRequestStatus();

        Task<DataResponse<NafathResponseDto>> NafathReconciliationReport(NafathRequestDto nafathRequestDto, NafathReconciliationReportTransactionDto listInput);

        Task CheckNafathReconciliationReportStatus();
        Task<BaseResponse> CheckNafathReconciliationReportStatusForMobile(string transactionId);
    }
}
