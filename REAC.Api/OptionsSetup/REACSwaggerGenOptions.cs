using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using REAC.Domain.Shared.Constants;

namespace REAC.Api.OptionsSetup
{
    public class REACSwaggerGenOptions : IConfigureOptions<Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions>
    {
        /// <summary>
        /// add jwt Swagger Generation options
        /// </summary>
        /// <param name="options"></param>
        public void Configure(Swashbuckle.AspNetCore.SwaggerGen.SwaggerGenOptions options)
        {
            options.SwaggerDoc(APISections.Security, new OpenApiInfo { Title = $"{APISections.Security} v1.0", Version = $"{APISections.Security} API V1" });
            options.SwaggerDoc(APISections.System, new OpenApiInfo { Title = $"{APISections.System} v1.0", Version = $"{APISections.System} API V1" });
            options.SwaggerDoc(APISections.Litigation, new OpenApiInfo { Title = $"{APISections.Litigation} v1.0", Version = $"{APISections.Litigation} API V1" });
            options.SwaggerDoc(APISections.Reconciliation, new OpenApiInfo { Title = $"{APISections.Reconciliation} v1.0", Version = $"{APISections.Reconciliation} API V1" });
            options.SwaggerDoc(APISections.WpfBusiness, new OpenApiInfo { Title = $"{APISections.WpfBusiness} v1.0", Version = $"{APISections.WpfBusiness} API V1" });
            options.SwaggerDoc(APISections.WpfSystem, new OpenApiInfo { Title = $"{APISections.WpfSystem} v1.0", Version = $"{APISections.WpfSystem} API V1" });
            options.SwaggerDoc(APISections.Integration, new OpenApiInfo { Title = $"{APISections.Integration} v1.0", Version = $"{APISections.Integration} API V1" });
            options.SwaggerDoc(APISections.Public, new OpenApiInfo { Title = $"{APISections.Public} v1.0", Version = $"{APISections.Public} API V1" });
            options.SwaggerDoc(APISections.Arbitration, new OpenApiInfo { Title = $"{APISections.Arbitration} v1.0", Version = $"{APISections.Arbitration} API V1" });



            // Include all paths
            options.DocInclusionPredicate((name, api) => true);

            // Filter endpoints based on consumer
            options.DocumentFilter<SwaggerDocumentFilter>();


            // Take first description on any conflict
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());



            #region JWT Config
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme.ToString(), new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT token"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme.ToString()
                        }
                    },
                    new string[]{}
                }
            });
            #endregion

        }
    }

}
