namespace REAC.Application.Contracts.Dtos.Integration
{
    public class CreateContactResponseDto
    {
        public string id { get; set; }
        public ContactEntryList entry_list { get; set; }
    }

    public class ContactEntryList
    {
        public NameValue id { get; set; }
        public NameValue national_id_c { get; set; }
        public NameValue first_name { get; set; }
        public NameValue last_name { get; set; }
        public NameValue email1 { get; set; }
        public NameValue phone_mobile { get; set; }
    }

    //public class NameValue
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //}
}
