using Microsoft.AspNetCore.Http;
using REAC.Domain.Shared.Enum;

namespace REAC.Application.Contracts.Dtos.Litigation.Create.Contract
{
    public class ContractDocuments
    {
        public Guid ContractId { get; set; }
        public Guid? ContractFileId { get; set; }
        public Guid? AssociationRegistrationCertificateFileId { get; set; }
        public Guid? FoundingLetterFileId { get; set; }
        public Guid? InternalRegulationsFileId { get; set; }
    }
    public class CreateUpdateContractDocuments
    {
        public Guid LitigationRequestId { get; set; }
        public IFormFileCollection? Files { get; set; }
        public PropertiesDocumnetType? PropertiesDocumnetType { get; set; }
        public IFormFileCollection? SupportsFiles { get; set; }
        

    }

}
