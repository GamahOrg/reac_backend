using Microsoft.EntityFrameworkCore;
namespace REAC.EntityFrameworkCore.Entities.Integration
{
    public class NafathTransaction
    {
        [Comment("Unique identifier of the national")]
        public string? NationalId { get; set; }
        [Comment("Transaction identifier for Nafath authentication")]
        public string TransactionId { get; set; }
        [Comment("Random number for authentication verification")]
        public string? RandomNumber { get; set; }
        [Comment("Expiration date and time of the authentication request")]
        public DateTime? RequestExpired { get; set; }
        [Comment("Access token for API authentication")]
        public string? AccessToken { get; set; }
        [Comment("Refresh token for renewing access")]
        public string? RefreshToken { get; set; }
        [Comment("Indicates whether this is a draft account")]
        public bool IsDraftAccount { get; set; } = false;
        [Comment("Indicates whether the person is an arbitrator")]
        public bool IsArbitrator { get; set; } = false;
        [Comment("Indicates whether the person is a reformer")]
        public bool IsReformer { get; set; } = false;
        [Comment("Indicates whether the person is a lawyer")]
        public bool IsLawye { get; set; } = false;
    }
}


