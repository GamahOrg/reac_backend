using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;

namespace Core.Security.OptionsSetup
{
    public class MvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        /// <summary>
        /// add authorize attribute for all controllers
        /// </summary>
        /// <param name="options"></param>
        public void Configure(MvcOptions options)
        {
            var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build();
            options.Filters.Add(new AuthorizeFilter(policy));
        }
    }
}
