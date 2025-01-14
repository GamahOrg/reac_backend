using REAC.Application.Contracts.Dtos.Integration.GEE;
using REAC.Application.Contracts.IAppServices.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace REAC.Application.AppServices.Integration.GEE
{
    public class NafathReconciliationReportService : INafathReconciliationReportService
    {
        private List<NafathReconciliationReportTransactionDto> cachedReportRequestsList = new List<NafathReconciliationReportTransactionDto>();
        public void Add(NafathReconciliationReportTransactionDto reconciliationReportTransaction)
        {
            cachedReportRequestsList.Add(reconciliationReportTransaction);
        }
        public void Remove(NafathReconciliationReportTransactionDto reconciliationReportTransaction)
        {
            cachedReportRequestsList.Remove(reconciliationReportTransaction);
        }
        public List<NafathReconciliationReportTransactionDto> GetAll()
        {
            return cachedReportRequestsList;
        }
    }
}