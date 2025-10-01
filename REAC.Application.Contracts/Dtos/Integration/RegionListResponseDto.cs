namespace REAC.Application.Contracts.Dtos.Integration
{
    public class RegionListResponseDto
    {
        public string module_name { get; set; }
        public string table_name { get; set; }
        public ModuleFields module_fields { get; set; }
        public List<object> link_fields { get; set; }
    }

    public class ModuleFields
    {
        public RejonC rejon_c { get; set; }
    }

    public class RejonC
    {
        public string name { get; set; }
        public string type { get; set; }
        public string group { get; set; }
        public string id_name { get; set; }
        public string label { get; set; }
        public int required { get; set; }
        public Dictionary<string, RegionOption> options { get; set; }
        public string related_module { get; set; }
        public bool calculated { get; set; }
        public int len { get; set; }
    }

    public class RegionOption
    {
        public string name { get; set; }
        public string value { get; set; }
    }
}
