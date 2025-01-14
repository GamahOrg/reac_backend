
using REAC.Application.Contracts.Dtos.Integration.GEE;

namespace REAC.Application.Contracts.IAppServices.Integration
{
    public interface INafathReconciliationReportService
    {
        void Add(NafathReconciliationReportTransactionDto reconciliationReportTransaction);
        void Remove(NafathReconciliationReportTransactionDto reconciliationReportTransaction);
        List<NafathReconciliationReportTransactionDto> GetAll();
    }
}