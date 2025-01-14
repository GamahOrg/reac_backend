namespace Core.Security.Dtos
{
    public class JwtOptions
    {
        /// <summary>
        /// To Validate issuer of token
        /// </summary>
        public string Issuer { get; init; }
        /// <summary>
        /// To Validate Audience of token
        /// </summary>
        public string Audience { get; init; }
        /// <summary>
        /// To encrypt token with this key
        /// </summary>
        public string SecretKey { get; set; }
        /// <summary>
        /// To Validate expiration date of token
        /// </summary>
        public double TokenDurationInHoures { get; set; }
        public double RefreshTokenDurationInHoures { get; set; }
    }
}
