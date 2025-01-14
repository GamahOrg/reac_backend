using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Core.Security.OptionsSetup
{
    public class AuthenticationOptionsSetup : IConfigureOptions<AuthenticationOptions>
    {
        /// <summary>
        /// add jwt autentication options
        /// </summary>
        /// <param name="options"></param>
        public void Configure(AuthenticationOptions options)
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }
    }
}
