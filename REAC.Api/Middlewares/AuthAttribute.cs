using Microsoft.AspNetCore.Mvc.Filters;

namespace REAC.Api.Middlewares
{
    public class AuthAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            throw new NotImplementedException();
        }
    }
}
