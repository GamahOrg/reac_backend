namespace Core.Log.Application.Contracts.Dto
{
    public class FileLoggerOptions
    {
        public int MaxSize { get; set; }
        public string Path { get; set; }
        public bool Trace { get; set; } = true;
        public bool Debug { get; set; } = true;
        public bool Information { get; set; } = true;
        public bool Warning { get; set; } = true;
        public bool Error { get; set; } = true;
        public bool Critical { get; set; } = true;
        public bool Exception { get; set; } = true;
    }
}
