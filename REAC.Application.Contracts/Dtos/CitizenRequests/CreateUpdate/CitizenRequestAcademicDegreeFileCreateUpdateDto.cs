using Core.GenericRepository.Shared.BaseEntities;

namespace REAC.Application.Contracts.Dtos.CitizenRequests
{
    public partial class CitizenRequestAcademicDegreeFileCreateUpdateDto : BaseEntity<Guid>
    {
        public Guid AcademicDegreeFileId { get; set; }

    }
}
