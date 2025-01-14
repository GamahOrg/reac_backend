namespace Core.Log.Application.Contracts.Dto
{
    public class EmailLoggerOptions
    {
        public bool Trace { get; set; } = true;
        public bool Debug { get; set; } = true;
        public bool Warning { get; set; } = true;
        public bool Information { get; set; } = true;
        public bool Error { get; set; } = true;
        public bool Critical { get; set; } = true;
        public bool Exception { get; set; } = true;
        public string To { get; set; }
    }
}
