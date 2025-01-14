using Core.Helpers;
using Core.Shared.Constants;
using Microsoft.AspNetCore.Http;

namespace Core.Middlewares
{
    public class LanguageMiddleware
    {
        private readonly RequestDelegate _next;

        // Dependency Injection
        public LanguageMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var languageCookie = context.Request.Cookies[Constants.CurrentLang];
            if (string.IsNullOrEmpty(languageCookie))
            {
                UserLanguage.ChangeLanguage(Constants.ArabicLanguage, context.Response);
            }

            await _next(context);
        }
    }
}