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
}
