using System.Collections.Generic;

namespace REAC.Application.Contracts.Dtos.Integration
{
    public class CreateComplaintRequestDto
    {
        public List<string> notesIds { get; set; }
        public string national_id { get; set; }
        public string? contact_id { get; set; } // Added for contact id
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string email1 { get; set; }
        public string phone_mobile { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string rejon_c { get; set; }
        public string requestclassification_c { get; set; }
    }
}
