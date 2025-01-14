using System.ComponentModel.DataAnnotations;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class LitigationReferences
    {
        public Guid? ReferenceId { get; set; }
        [MaxLength(100)]
        public string ReferenceTitle { get; set; } //عنوان الاسناد
        public List<Guid>? ReferenceAttachments { get; set; }
    }
}
