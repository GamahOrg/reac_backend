using Core.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts
{
    public interface INafathReconciliationReportStatus
    {
        Task ReceivedNafathReconciliationReportStatus(BaseResponse NafathReconciliationReportStatus);
    }
}
