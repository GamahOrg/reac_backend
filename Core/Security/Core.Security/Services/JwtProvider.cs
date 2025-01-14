using Core.Dtos;
using Core.Security.Authorization;
using Core.Security.Contracts;
using Core.Security.Dtos;
using Core.Security.Entities;
using Core.Shared.Responses;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Core.Security.Services
{
    public class JwtProvider : IJwtProvider
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly JwtOptions _jwtOptions;
        private readonly IConfiguration _configuration;
        public IHttpContextAccessor _httpContextAccessor { get; }

        public JwtProvider(UserManager<User> userManager, RoleManager<Role> roleManager, IOptions<JwtOptions> jwtOptions, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtOptions = jwtOptions.Value;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }
        public async Task<BaseResponse> ValidateActiveToken()
        => new DataResponse<bool> { IsDone = true, Data = await IsActiveAsync(GetCurrentAsync()) };
        
        public async Task<BaseResponse> ValidateActiveToken(string token)
        => new DataResponse<bool> { IsDone = true, Data = await IsActiveAsync(token) };

        public async Task<AuthDto> GenerateAuthenticationTokens(User user)
        {
            AuthDto authDto = new AuthDto();
            // Get user Roles and role's claims
            var UserRoles = await _userManager.GetRolesAsync(user);
            var rolesAndClaims = new List<Claim>();
            foreach (var roleName in UserRoles)
            {
                //rolesAndClaims.Add(new Claim(Permissions.RoleType, roleName));
                var role = _roleManager.FindByNameAsync(roleName).Result;
                if (role != null)
                {
                    var roleClaims = _roleManager.GetClaimsAsync(role).Result;
                    foreach (var roleClaim in roleClaims)
                        rolesAndClaims.Add(new Claim(Permissions.PolicyType, roleClaim.Value));
                }
            }
            var claimsValues = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub,user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name,user.UserName.ToString()),
                new Claim("ReferenceId",user.ReferenceId.ToString()),
            };


            var claims = claimsValues.Union(rolesAndClaims).ToList();
            bool? IsLawyer = false;
            bool? IsReformer = false;
            bool? IsArbitrator = false;
            bool? IsAdmin = false;
            int? DepartmentType = 0;
            bool? IsDraftAccount = false;
            try
            {

                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                /// External claims (Remove it to reuse the component in other systems)
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                using (IDbConnection db = new SqlConnection(_configuration.GetConnectionString("AppConnectionString")?.ToString()))
                {
                    db.Open();
                    var citizen = db.Query<dynamic?>($@"SELECT * FROM Citizen where Id = '{user.ReferenceId}'").FirstOrDefault();
                    if (citizen != null)
                    {
                        try
                        {
                            claimsValues.Add(new Claim("IsLawyer", (citizen.IsLawyer != null) ? citizen.IsLawyer.ToString() : null));
                        }
                        catch { claimsValues.Add(new Claim("IsLawyer", "")); }
                        try
                        {
                            claimsValues.Add(new Claim("IsReformer", (citizen.IsReformer != null) ? citizen.IsReformer.ToString() : null));
                        }
                        catch { claimsValues.Add(new Claim("IsReformer", "")); }
                        try
                        {
                            claimsValues.Add(new Claim("IsArbitrator", (citizen.IsArbitrator != null) ? citizen.IsArbitrator.ToString() : null));
                        }
                        catch { claimsValues.Add(new Claim("IsArbitrator", "")); }
                        try
                        {
                            claimsValues.Add(new Claim("IsDraftAccount", (citizen.IsDraftAccount != null) ? citizen.IsDraftAccount.ToString() : null));
                        }
                        catch { claimsValues.Add(new Claim("IsDraftAccount", "")); }

                        IsLawyer = citizen.IsLawyer;
                        IsReformer = citizen.IsReformer;
                        IsArbitrator = citizen.IsArbitrator;
                        IsDraftAccount = citizen.IsDraftAccount;
                    }
                    else
                    {
                        claimsValues.Add(new Claim("IsLawyer", ""));
                        claimsValues.Add(new Claim("IsReformer", ""));
                        claimsValues.Add(new Claim("IsArbitrator", ""));
                        var employee = db.Query<dynamic?>($@"SELECT * FROM SysEmployee where Id = '{user.ReferenceId}'").FirstOrDefault();
                        if (employee != null)
                        {
                            claimsValues.Add(new Claim("IsAdmin", (employee.IsAdmin != null) ? employee.IsAdmin.ToString() : null));


                            IsAdmin = employee.IsAdmin;

                            var department = db.Query<dynamic?>($@"SELECT * FROM SysDepartment where Id = '{employee.DepartmentId}'").FirstOrDefault();
                            if (department != null)
                            {
                                claimsValues.Add(new Claim("DepartmentType", (department.DepartmentType != null) ? department.DepartmentType.ToString() : null));
                                DepartmentType = (int)department?.DepartmentType;

                            }
                        }
                        else
                        {
                            claimsValues.Add(new Claim("IsAdmin", ""));
                            claimsValues.Add(new Claim("DepartmentType", ""));

                        }
                        db.Close();
                    }
                    db.Close();
                }
                ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////                
            }
            catch { }
            var symetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
            var signingCredentials = new SigningCredentials(symetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                Expires = DateTime.Now.AddHours(_jwtOptions.TokenDurationInHoures),
                SigningCredentials = signingCredentials
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            authDto.Token = new TokenModel
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = GenerateRefreshToken(),
                IsAdmin = IsAdmin,
                IsArbitrator = IsArbitrator,
                IsReformer = IsReformer,
                IsLawyer = IsLawyer,
                DepartmentType = DepartmentType,
                IsDraftAccount = IsDraftAccount

            };
            authDto.RefreshTokenExpiration = DateTime.Now.AddHours(_jwtOptions.RefreshTokenDurationInHoures);
            return authDto;
        }
        #region private methods
        private async Task<bool> IsActiveAsync(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                var isValid = ValidateToken(token);
                return isValid;
            }
            else
                return false;
        }
        private bool ValidateToken(string token)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(token) || token == "Bearer")
                {
                    return false;
                }

                var validationParameters = new TokenValidationParameters
                {

                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _jwtOptions.Issuer,
                    ValidAudience = _jwtOptions.Audience,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey))
                };

                ISecurityTokenValidator tokenValidator = new JwtSecurityTokenHandler();
                var claim = tokenValidator.ValidateToken(token, validationParameters, out var _);
                if (claim == null)
                    return false;

                var scope = claim.FindFirst(c => c.Type.ToLower() == ClaimTypes.NameIdentifier);
                if (scope == null)
                    return false;
                else
                    return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        private string GetCurrentAsync()
        {
            var authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            string refreshToken = Convert.ToBase64String(randomNumber);
            return refreshToken;
        }
        #endregion
    }

}
