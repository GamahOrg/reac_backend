using Core.GenericRepository.Shared.BaseEntities;
using REAC.Application.Contracts.Dtos.Integration.Party;
using REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation;
using REAC.Domain.Shared.Enum;
using REAC.EntityFrameworkCore.Entities.Litigation;
using REAC.EntityFrameworkCore.Entities.System;
using System.ComponentModel.DataAnnotations;

namespace REAC.Application.Contracts.Dtos.Litigation.Get
{
    public partial class LitigationPartiesDetailsDto : BaseEntity<Guid>
    {
        public PartyDTO? FirstPartyDto { get; set; }
        public PartyDTO? SecondPartyDto { get; set; }
        public LitigationRequestDelegationDataDto? LitigationRequestDelegationDataDto { get; set; }

        public string? Key { get; set; }
        [MaxLength(4)]
        public string SubKey { get; set; } = null!;
        public string? AssociationName { get; set; }
        public LitigationRequestOwnerType LitigationRequestOwnerType { get; set; }
        public string? ContractAddress { get; set; }
        public RequestStatus LitigationRequestStatus { get; set; }
        public SysFileManagerCreateUpdateDto AssociationRegistrationCertificateFile { get; set; } = null!;
        public SysFileManagerCreateUpdateDto FoundingLetterFile { get; set; } = null!;
        public SysFileManagerCreateUpdateDto InternalRegulationsFile { get; set; } = null!;

        public SysFileManagerCreateUpdateDto ContractFile { get; set; } = null!;

        public ICollection<EntityFrameworkCore.Entities.Litigation.LitigationRequestRequest> Requests { get; set; } = new List<EntityFrameworkCore.Entities.Litigation.LitigationRequestRequest>();
        public virtual ICollection<LitigationRequestGround> References { get; set; } = new List<LitigationRequestGround>();



    }
}
