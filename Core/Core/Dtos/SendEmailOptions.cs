namespace Core.Dtos
{
    public class SendEmailOptions
    {
        public string From { get; set; }
        public string Passowrd { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public bool UseDefaultCredentials { get; set; }
        public bool SendEmail { get; set; }
    }
}
