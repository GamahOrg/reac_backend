using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.IAppServices.Integration.Hubs.Contracts
{
    public class LitigationRequestAgreementStatus
    {
        public bool IsValid { get; set; }
        public string TransactionId { get; set; }
        public bool AllowAdd { get; set; }
        public ApproveRejectStatus ApproveRejectStatus { get; set; }
    }
}
