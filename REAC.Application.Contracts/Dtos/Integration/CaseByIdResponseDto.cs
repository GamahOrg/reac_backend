using REAC.EntityFrameworkCore.Entities.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace REAC.Application.Contracts.Dtos.Integration
{


    public class CaseByIdResponseDto
    {
        public int result_count { get; set; }
        public string total_count { get; set; }
        public int next_offset { get; set; }
        public List<CaseEntry> entry_list { get; set; }
        public List<object> relationship_list { get; set; }

        public ICollection<SysFileManagerCreateUpdateDto> ComplaintFiles { get; set; } = null!;

    }


}
