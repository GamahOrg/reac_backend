using Core.Security;
using Core.Security.Contracts;
using Core.Security.Dtos;
using Core.Security.Entities;
using Core.Security.Extensions;
using Core.Security.OptionsSetup;
using Core.Security.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JWT"));

#region configure DBContexts
builder.Services.AddDbContext<IdentityContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("AppConnectionString")
    , b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));
#endregion

#region Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<IdentityContext>().AddDefaultTokenProviders();
builder.Services.ConfigureOptions<IdentityOptionsSetup>();
#endregion

// Add services to the container.
#region DI
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<IUserService, UserService>();
#endregion


builder.Services.AddControllers();
string _defaultCorsPolicyName = "localhost";

builder.Services.AddCors(
    options => options.AddPolicy(
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
            .AllowCredentials()
    )
);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors(_defaultCorsPolicyName);

app.MapControllers();

app.Run();
