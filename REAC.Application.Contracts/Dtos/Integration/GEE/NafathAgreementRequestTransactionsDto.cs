using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration.GEE
{
    public class NafathAgreementRequestTransactionsDto
    {
        public Guid LitigationRequestId { get; set; }
        public string NationalId { get; set; }
        public ApproveRejectStatus AgreementStatus { get; set; }
        public DateTime AgreementStatusOn { get; set; }
        public DateTime RequestExpired { get; set; }
        public string? TransactionId { get; set; }
        public string? RandomNumber { get; set; }
    }
}
