namespace REAC.EntityFrameworkCore.Entities.Integration
{
    public class NafathTransaction
    {
        public string? NationalId { get; set; }
        public string TransactionId { get; set; }
        public string? RandomNumber { get; set; }
        public DateTime? RequestExpired { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public bool IsDraftAccount { get; set; } = false;
        public bool IsArbitrator { get; set; } = false;
        public bool IsReformer { get; set; } = false;
        public bool IsLawye { get; set; } = false;
    }
}
