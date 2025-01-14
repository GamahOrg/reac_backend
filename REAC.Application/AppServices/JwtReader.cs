using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using REAC.Application.Contracts.IAppServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace REAC.Application.AppServices
{
    public class JwtReader : IJwtReader
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtReader(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public bool GetCitizenId(ref Guid customerId)
        {
            return GetClaimGuidValue("CustomerId", ref customerId);
        }
        private bool GetClaimGuidValue(string key, ref Guid value)
        {
            StringValues authorizationHeader = "";
            try
            {
                if (_httpContextAccessor.HttpContext != null)
                {
                    var endPointt = _httpContextAccessor.HttpContext.GetEndpoint();
                    //if (endPointt?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
                    //{
                    //    return true;
                    //}

                    if (_httpContextAccessor.HttpContext.Request.Headers.ContainsKey("authorization"))
                    {
                        authorizationHeader = _httpContextAccessor.HttpContext.Request.Headers["authorization"];
                    }
                }
            }
            catch (Exception exc)
            {
                authorizationHeader = "";
            }
            if (string.IsNullOrEmpty(authorizationHeader))
                return false;

            authorizationHeader = authorizationHeader.FirstOrDefault().Split(" ").Last();
            var handler = new JwtSecurityTokenHandler();
            var tokenS = handler.ReadToken(authorizationHeader) as JwtSecurityToken;
            var claim = tokenS.Claims.First(claim => claim.Type == key);

            if (string.IsNullOrEmpty(claim.Value))
                return false;

            Guid.TryParse(tokenS.Claims.First(claim => claim.Type == key).Value, out value);
            return true;
        }
        public bool GetEmployeeId(ref Guid employeeId)
        {
            return GetClaimGuidValue("EmployeeId", ref employeeId);
        }
        public bool GetUserId(ref Guid? userId)
        {
            bool result = false;
            Guid sub = Guid.Empty;
            result = GetClaimGuidValue("sub", ref sub);
            if (result && sub != Guid.Empty)
            {
                userId = sub;
            }
            return result;
        }
        public Guid? GetUserId()
        {
            var context = _httpContextAccessor.HttpContext;
            var id = context?.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Sid)?.Value;
            var isValid = Guid.TryParse(id, out var userId);
            return isValid ? userId : null;
        }

        public bool GetReferenceId(ref Guid referenceId)
        {
            return GetClaimGuidValue("ReferenceId", ref referenceId);
        }
    }
}
