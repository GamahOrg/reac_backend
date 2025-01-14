
using Core.Log.EntityFrameworkCore;
using Core.Security;
using Core.Security.Contracts;
using Core.Security.Dtos;
using Core.Security.Entities;
using Core.Security.Extensions;
using Core.Security.OptionsSetup;
using Core.Security.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using REAC.Api;
using REAC.Api.Middlewares;
using REAC.Api.OptionsSetup;
using REAC.Application.AppServices.Common;
using REAC.Application.AppServices.Integration.GEE;
using REAC.Application.AppServices.Integration.Hubs;
using REAC.Application.AppServices.SLA;
using REAC.Application.Contracts.IAppServices.Common;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;
using REAC.EntityFrameworkCore.Seed;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Globalization;
using System.Text.Json.Serialization;
using Wafi.Application.Common.Localization;
using Wafi.Application.Hubs;

var builder = WebApplication.CreateBuilder(args);

#region configure DBContexts

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<REACDBContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("AppConnectionString")
    , sqlServerOptions => sqlServerOptions.CommandTimeout(180)
));

builder.Services.AddDbContext<IdentityContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("AppConnectionString")
    , b => b.MigrationsAssembly(typeof(REACDBContext).Assembly.FullName)));

builder.Services.AddDbContext<LogDbContext>(option => option.UseSqlServer(
    builder.Configuration.GetConnectionString("AppConnectionString")
    , b => b.MigrationsAssembly(typeof(REACDBContext).Assembly.FullName)));

builder.Services.AddGenericRepository<REACDBContext>();


builder.Services.AddHostedService<NafathBackgroundTaskService>();
builder.Services.AddHostedService<SLABackgroundTaskService>();

#endregion
// Add services to the container.
string _defaultCorsPolicyName = "localhost";
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
        _defaultCorsPolicyName,
        corsbuilder => corsbuilder
            .WithOrigins(
                // App:CorsOrigins in appsettings.json can contain more than one address separated by comma.
                builder.Configuration["App:CorsOrigins"]
                    .Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(o => o.RemovePostFix("/"))
                    .ToArray()
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
        .AllowCredentials());
        options.AddPolicy("CorsPolicy",
         builder =>
         {
             builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .SetIsOriginAllowed((host) => true)
                    .AllowCredentials();
         });
    }
);
#region Identity

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();
builder.Services.ConfigureOptions<IdentityOptionsSetup>();

#endregion 

#region JWT Configurations

builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));
builder.Services.AddAuthentication().AddJwtBearer(o => o.Configure(builder.Configuration));
builder.Services.ConfigureOptions<AuthenticationOptionsSetup>();
//builder.Services.ConfigureOptions<MvcOptionsSetup>();

#endregion

#region String Localization
builder.Services.AddScoped<IJsonStringLocalizer, JsonStringLocalizer>();

builder.Services.AddLocalization();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
builder.Services.AddMvc()
    .AddDataAnnotationsLocalization(options =>
    {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(JsonStringLocalizerFactory));
    });
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[]
    {
        new CultureInfo("en-US"),
        new CultureInfo("ar-SA"),
    };
    options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0]);
    options.SupportedCultures = supportedCultures;
});
#endregion

builder.Services.AddAppServices();

//builder.Services.AddControllers();
builder.Services.AddControllers()
           .AddJsonOptions(options =>
           {
               options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
               options.JsonSerializerOptions.MaxDepth = 64; // Increase the maximum depth as needed
           });



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<REACSwaggerGenOptions>();


//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//    var supportedCultures = new[]
//    {
//        new CultureInfo("en-US"),
//        new CultureInfo("ar-SA"),
//    };

//    options.DefaultRequestCulture = new RequestCulture(culture: supportedCultures[0]);
//    options.SupportedCultures = supportedCultures;
//});

#region SignalR

builder.Services.AddSignalR();

#endregion

var app = builder.Build();


// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.EnableDeepLinking();

    // Build a swagger endpoint for each API version and consumer
    c.SwaggerEndpoint($"/swagger/{APISections.Security}/swagger.json", $"{APISections.Security} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.System}/swagger.json", $"{APISections.System} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.Litigation}/swagger.json", $"{APISections.Litigation} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.Reconciliation}/swagger.json", $"{APISections.Reconciliation} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.WpfBusiness}/swagger.json", $"{APISections.WpfBusiness} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.WpfSystem}/swagger.json", $"{APISections.WpfSystem} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.Integration}/swagger.json", $"{APISections.Integration} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.Public}/swagger.json", $"{APISections.Public} API V1");
    c.SwaggerEndpoint($"/swagger/{APISections.Arbitration}/swagger.json", $"{APISections.Arbitration} API V1");



    c.DocExpansion(DocExpansion.List);
});
//}



app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"reac")),
    RequestPath = new PathString("/reac")
});
app.UseStaticFiles(new StaticFileOptions()
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources")),
    RequestPath = new PathString("/Resources")
});


app.UseAuthentication();
app.UseAuthorization();
app.UseCors(_defaultCorsPolicyName);

app.UseMiddleware<AuthMiddleware>();


app.MapControllers();

app.Use(async (context, next) =>
{
    var cultureQuery = context.Request.Headers["culture"];
    if (!string.IsNullOrWhiteSpace(cultureQuery))
    {
        var culture = new CultureInfo(cultureQuery);

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }
    await next(context);
});

// Seed Data
await app.Services.Seed();
app.MapHub<IdentityHub>("/identity-hub");
app.MapHub<LitigationRequestAgreementHub>("/litigation-request-agreement-hub");
app.MapHub<NafathReconciliationReportHub>("/reconciliation-report-hub");
app.MapHub<NotificationHub>("/notification-hub");

app.Run();
