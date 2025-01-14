using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Core.Security.OptionsSetup
{
    public class IdentityOptionsSetup : IConfigureOptions<IdentityOptions>
    {
        /// <summary>
        /// Configure Microsoft identity options
        /// </summary>
        /// <param name="options"></param>
        public void Configure(IdentityOptions options)
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequiredUniqueChars = 0;
            options.Password.RequiredLength = 1;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        }
    }
}
