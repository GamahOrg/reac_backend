using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class LitigationRequests
    {
        [MaxLength(200)]
        public string LitigationPurpose { get; set; }
        public Guid? RequestId { get; set; }
    }
}
