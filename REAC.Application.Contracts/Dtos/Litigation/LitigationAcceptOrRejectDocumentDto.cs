using REAC.Domain.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation
{
    public class LitigationArbitrationTaskAcceptOrRejectDocumentDto
    {

        public string ArbitratorName { get; set; }
        public string AssociationName { get; set; }
        public string ContractNumber { get; set; }
        public string ContractReleaseDate { get; set; }
        public string LitigationNumber { get; set; }
        public string LitigationType { get; set; }
        public string Subject { get; set; }
        public string ArbitratorAssignerName { get; set; }
        public string ArbitratorAssigningDate { get; set; }
        public string ArbitratorAssigningCost { get; set; }
        public LitigationRequestArbitratorAssigningRequestStatus Response { get; set; }
        public int RequestTypeId { get; set; }
        public string FirstPartyName { get; set; }
        public string SecondPartyName { get; set; }
        public string? RejectReason { get; set; }
        public string? SigntureFile { get; set; }
    }
    
    public class LitigationReconciliationTaskAcceptOrRejectDocumentDto
    {

        public string ReformerName { get; set; }
        public string AssociationName { get; set; }
        public string ContractNumber { get; set; }
        public string ContractReleaseDate { get; set; }
        public string LitigationNumber { get; set; }
        public string LitigationType { get; set; }
        public string Subject { get; set; }
        public string ReformerAssignerName { get; set; }
        public string ReformerAssigningDate { get; set; }
        public string ReformerAssigningCost { get; set; }
        public LitigationRequestReformerAssigningRequestStatus Response { get; set; }
        public int RequestTypeId { get; set; }
        public string FirstPartyName { get; set; }
        public string SecondPartyName { get; set; }
        public string? RejectReason { get; set; }
        public string? SigntureFile { get; set; }

    }
}
