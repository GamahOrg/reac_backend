using Microsoft.OpenApi.Models;
using REAC.Domain.Shared.Constants;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace REAC.Api.OptionsSetup
{
    public class SwaggerDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            var pathsPerConsumer = new OpenApiPaths();
            var currentConsumer = GetConsumer(swaggerDoc.Info.Title);

            var paths = context.ApiDescriptions.Where(x => x.GroupName == currentConsumer).Select(x => '/' + x.RelativePath).ToList();

            var keysForRemove = swaggerDoc.Paths.Where(x => !paths.Contains(x.Key)).Select(x => x.Key).ToList();

            foreach (var key in keysForRemove)
            {
                swaggerDoc.Paths.Remove(key);
            }
        }
        private string GetConsumer(string path)
        {
            if (path.Contains(APISections.WpfSystem))
            {
                return APISections.WpfSystem;
            }
            else if (path.Contains(APISections.Litigation))
            {
                return APISections.Litigation;
            }
            else if (path.Contains(APISections.Integration))
            {
                return APISections.Integration;
            }
            else if (path.Contains(APISections.Reconciliation))
            {
                return APISections.Reconciliation;
            }
            else if (path.Contains(APISections.WpfBusiness))
            {
                return APISections.WpfBusiness;
            }
            else if (path.Contains(APISections.System))
            {
                return APISections.System;
            }
            else if (path.Contains(APISections.Public))
            {
                return APISections.Public;
            }
            else if (path.Contains(APISections.Security))
            {
                return APISections.Security;
            }
            else if (path.Contains(APISections.Arbitration))
            {
                return APISections.Arbitration;
            }
            else if (path.Contains("v1"))
            {
                return "v1";
            }

            return string.Empty;
        }

    }
}