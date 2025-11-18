using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.Invoice
{
   public  class UpdateInvoiceDto
    {
        [Required(ErrorMessage = "ReferenceId is required")]
        public string? ReferecnceId { get; set; }
        [Required(ErrorMessage = "InvoiceId is required")]
        public string? InvoiceId { get; set; }
        [Required(ErrorMessage = "InvoiceStatus is required")]
        public int InvoiceStatus { get; set; }

    }
}
