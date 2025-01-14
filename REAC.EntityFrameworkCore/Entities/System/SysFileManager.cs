using Core.GenericRepository.Shared.BaseEntities;
using REAC.EntityFrameworkCore.Entities.Actor;
using REAC.EntityFrameworkCore.Entities.Arbitration;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.Reconciliation;
using REAC.EntityFrameworkCore.Entities.Wpf.Business;

namespace REAC.EntityFrameworkCore.Entities.System;

public partial class SysFileManager : CreationBaseEntity<Guid>
{
    public string? FileDescription { get; set; }

    public string FilePath { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public virtual ICollection<LitigationRequestArbitratorRecusalRequestFile> LitigationRequestArbitratorRecusalRequestFiles { get; set; } = new List<LitigationRequestArbitratorRecusalRequestFile>();

    public virtual ICollection<LitigationRequestArbitrationReportExplanationRequestFile> LitigationRequestArbitrationReportExplanationRequestFiles { get; set; } = new List<LitigationRequestArbitrationReportExplanationRequestFile>();

    public virtual ICollection<Citizen> IndentityCitizens { get; set; } = new List<Citizen>();
    public virtual ICollection<Citizen> LawLicenseCitizens { get; set; } = new List<Citizen>();
    public virtual ICollection<CitizenRequestExperienceCertificateFile> CitizenRequestExperienceCertificateFiles { get; set; } = new List<CitizenRequestExperienceCertificateFile>();
    public virtual ICollection<CitizenRequestFreelanceWorkDocumentFile> CitizenRequestFreelanceWorkDocumentFiles { get; set; } = new List<CitizenRequestFreelanceWorkDocumentFile>();
    public virtual ICollection<LitigationRequest> ExemptionReductionCategoryFiles { get; set; } = new List<LitigationRequest>();
    public virtual ICollection<WorkFlowProcessingLogFile> WorkFlowProcessingLogFiles { get; set; } = new List<WorkFlowProcessingLogFile>();


    public virtual ICollection<LitigationRequestContract> AssociationRegistrationCertificateContracts { get; set; } = new List<LitigationRequestContract>();
    public virtual ICollection<LitigationRequestContract> FoundingLetterContracts { get; set; } = new List<LitigationRequestContract>();
    public virtual ICollection<LitigationRequestContract> InternalRegulationsContracts { get; set; } = new List<LitigationRequestContract>();
    public virtual ICollection<CitizenRequestAcademicDegreeFile> CitizenRequestAcademicDegreeFiles { get; set; } = new List<CitizenRequestAcademicDegreeFile>();
    public virtual ICollection<Delegator> Delegators { get; set; } = new List<Delegator>();

    public virtual ICollection<DelegationRequest> DelegationRequests { get; set; } = new List<DelegationRequest>();

    public virtual ICollection<LitigationRequestContract> LitigationRequestContracts { get; set; } = new List<LitigationRequestContract>();

    public virtual ICollection<LitigationRequestCorrespondenceFile> LitigationRequestCorrespondenceFiles { get; set; } = new List<LitigationRequestCorrespondenceFile>();
    public virtual ICollection<LitigationRequestCorrespondenceReplyFile> LitigationRequestCorrespondenceReplyFiles { get; set; } = new List<LitigationRequestCorrespondenceReplyFile>();

    public virtual ICollection<LitigationRequestExtendPeriodRequestFile> LitigationRequestExtendPeriodRequestFiles { get; set; } = new List<LitigationRequestExtendPeriodRequestFile>();

    public virtual ICollection<LitigationRequestGroundFile> LitigationRequestGroundFiles { get; set; } = new List<LitigationRequestGroundFile>();

    public virtual ICollection<LitigationRequestNoteFile> LitigationRequestNoteFiles { get; set; } = new List<LitigationRequestNoteFile>();

    public virtual ICollection<LitigationRequestRequestFile> LitigationRequestRequestFiles { get; set; } = new List<LitigationRequestRequestFile>();
    public virtual ICollection<CitizenRequestCVFile> CitizenRequestCVFiles { get; set; } = new List<CitizenRequestCVFile>();
    public virtual ICollection<CitizenRequestReconciliationCenterDegreeFile> CitizenRequestReconciliationCenterDegreeFiles { get; set; } = new List<CitizenRequestReconciliationCenterDegreeFile>();
    public virtual ICollection<CitizenRequestProgramRealEstateInstituteFile> CitizenRequestProgramRealEstateInstituteFiles { get; set; } = new List<CitizenRequestProgramRealEstateInstituteFile>();
}
