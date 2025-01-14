using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Core.Security.OptionsSetup
{
    public static class  JwtBearerOptionsSetup
    {
        /// <summary>
        /// configure jwt bearer setup for validations
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        public static void Configure(this JwtBearerOptions options, ConfigurationManager configuration)
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = false;
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                ClockSkew = TimeSpan.Zero,

                SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                {
                    var jwt = new JwtSecurityToken(token);

                    return jwt;
                },
            };
        }
        //public static void Configure(this JwtBearerOptions options, ConfigurationManager configuration)
        //{
        //    options.RequireHttpsMetadata = false;
        //    options.SaveToken = false;
        //    options.TokenValidationParameters = new()
        //    {
        //        ValidateIssuer = true,
        //        ValidateAudience = true,
        //        ValidateLifetime = true,
        //        ValidateIssuerSigningKey = true,
        //        ValidIssuer = configuration["JWT:Issuer"],
        //        ValidAudience = configuration["JWT:Audience"],
        //        ClockSkew = TimeSpan.Zero,
        //        RequireExpirationTime = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]))
        //    };
        //}
    }
}
