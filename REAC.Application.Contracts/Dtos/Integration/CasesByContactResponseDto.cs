using System.Collections.Generic;

namespace REAC.Application.Contracts.Dtos.Integration
{
    public class CasesByContactResponseDto
    {
        public int result_count { get; set; }
        public string total_count { get; set; }
        public int next_offset { get; set; }
        public List<CaseEntry> entry_list { get; set; }
        public List<object> relationship_list { get; set; }
    }

    public class CaseEntry
    {
        public string id { get; set; }
        public string module_name { get; set; }
        public CaseNameValueList name_value_list { get; set; }
    }

    public class CaseNameValueList
    {
        public NameValue id { get; set; }
        public NameValue rejon_c { get; set; }
        public NameValue requestclassification_c { get; set; }
        public NameValue program_c { get; set; }
        public NameValue reference_number_c { get; set; }
        public NameValue request_number_c { get; set; }
        public NameValue name { get; set; }
        public NameValue description { get; set; }
        public NameValue date_entered { get; set; }
        public NameValue case_number { get; set; }
        public NameValue status { get; set; }
    }

    
}
