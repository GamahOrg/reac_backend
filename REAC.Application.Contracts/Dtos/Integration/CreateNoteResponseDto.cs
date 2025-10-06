namespace REAC.Application.Contracts.Dtos.Integration
{
    public class CreateNoteResponseDto
    {
        public string id { get; set; }
        public EntryList entry_list { get; set; }
    }

    public class EntryList
    {
        public NameValue name { get; set; }
    }

    //public class NameValue
    //{
    //    public string name { get; set; }
    //    public string value { get; set; }
    //}
}
