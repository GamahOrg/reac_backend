using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.System
{
    public class SysDepartmentTranslation
    {
        [Comment("Name of the department")]
        public string DepartmentName { get; set; } = null!;

        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;

    }
}

