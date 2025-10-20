namespace REAC.Application.Contracts.Dtos.Integration
{
    public class ContactResponseDto
    {
        public int result_count { get; set; }
        public string total_count { get; set; }
        public int next_offset { get; set; }
        public List<ContactEntry> entry_list { get; set; }
        public List<object> relationship_list { get; set; }
    }

    public class ContactEntry
    {
        public string id { get; set; }
        public string module_name { get; set; }
        public NameValueList name_value_list { get; set; }
    }

    public class NameValueList
    {
        public NameValue id { get; set; }
        public NameValue first_name { get; set; }
        public NameValue last_name { get; set; }
        public NameValue email1 { get; set; }
        public NameValue national_id_c { get; set; }
        public NameValue phone_mobile { get; set; }
    }

    
}
