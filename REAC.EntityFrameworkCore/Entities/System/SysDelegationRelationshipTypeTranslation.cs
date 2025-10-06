using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysDelegationRelationshipTypeTranslation
    {
        [Comment("Type of the name")]
        public string DelegationRelationshipTypeName { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

