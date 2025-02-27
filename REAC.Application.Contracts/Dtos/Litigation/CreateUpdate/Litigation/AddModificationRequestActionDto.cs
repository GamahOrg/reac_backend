using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Litigation.CreateUpdate.Litigation
{
    public class AddModificationRequestActionDto
    {
        public Guid ModificationRequestId { get; set; }
        public bool IsApproved { get; set; }
    }
}
