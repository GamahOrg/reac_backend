using System.Collections.Generic;

namespace REAC.Application.Contracts.Dtos.Integration
{
    public class LinkNoteToCaseRequestDto
    {
        public string module_name { get; set; }
        public string module_id { get; set; }
        public string link_field_name { get; set; }
        public List<string> related_ids { get; set; }
    }
}
