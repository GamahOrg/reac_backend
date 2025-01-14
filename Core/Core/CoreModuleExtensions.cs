using System.Globalization;
using Core.Shared.Constants;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace Core;

public static class CoreModuleExtensions
{
    public static void AddCultureLocalizations(this IServiceCollection services)
    {
        services.Configure<RequestLocalizationOptions>(options =>
        {
            var supportedCultures = new[]
            {
                new CultureInfo(Constants.ArabicLanguage),
                new CultureInfo(Constants.EnglishLanguage)
            };

            options.DefaultRequestCulture = new RequestCulture(Constants.ArabicLanguage);
            options.SetDefaultCulture(Constants.ArabicLanguage);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider()
            };
        });
    }
}