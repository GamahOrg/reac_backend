using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.Invoice
{
    public class SysInvoiceTypeTranslation
    {
        [Comment("Description for Name")]
        public string Name { get; set; } = null!;
        [Comment("Description for Language Code")]
        public string LanguageCode { get; set; } = null!;
    }
}

