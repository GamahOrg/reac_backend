using Core.Shared.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Core.Helpers;

public static class UserLanguage
{
    public static string GetCurrentLanguage(this IHttpContextAccessor? accessor)
    {
        var context = accessor?.HttpContext;
        if (context == null)
            return Constants.ArabicLanguage;

        var language = context.Request.Cookies[Constants.CurrentLang] ?? Constants.ArabicLanguage;
        return language;
    }

    public static void ChangeLanguage(string language, HttpResponse response)
    {
        var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
        var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language));
        response.Cookies.Append(Constants.CurrentLang, language, options);
        response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, cookieValue, options);
    }
}