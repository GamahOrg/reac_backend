using Core.Security.Attributes;
using Core.Security.Contracts;
using Core.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Primitives;
using REAC.Application.Contracts.IAppServices.System;
using REAC.Domain.Shared.Constants;
using REAC.EntityFrameworkCore.Entities.System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace REAC.Api.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate next;
        public AuthMiddleware(RequestDelegate next)
        {
            this.next = next;
        }
        /// <summary>
        /// Validate JWT Token From Security Api
        /// </summary>
        /// <param name="context"></param>
        /// <param name="jwtProvider"></param>
        /// <param name="settingService"></param>
        /// <returns></returns>
        //public async Task InvokeAsync(HttpContext context, IJwtProvider jwtProvider, ISysSettingService settingService, IHttpContextAccessor httpContextAccessor)
        //{
        //    var endPointt = context.GetEndpoint();
        //    if (endPointt?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
        //    {
        //        await next(context);
        //        return;
        //    }


        //    var urlResult = settingService.GetBySettingKey(SysSettingKeyConstants.SecurityApiUrl).Result;

        //    if (urlResult.IsDone)
        //    {
        //        string? url = (urlResult as DataResponse<SysSetting>).Data?.Value;

        //        using (HttpClient client = new HttpClient())
        //        {
        //            string token = GetCurrentTokeAsync(context);
        //            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //            HttpResponseMessage response = new HttpResponseMessage();
        //            try
        //            {
        //                response = await client.GetAsync(url.TrimEnd('/') + "/api/Account/ValidateActiveToken");
        //            }
        //            catch (Exception)
        //            {
        //                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //                return;
        //            }
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var isValidTokenResponse = await response.Content.ReadAsAsync<DataResponse<bool>>();
        //                if (isValidTokenResponse.IsDone)
        //                {
        //                    var isValidToken = isValidTokenResponse.Data;
        //                    if (isValidToken)
        //                    {
        //                        var mainPermissions =
        //                        httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>()?
        //                        .ControllerTypeInfo.AsType().GetCustomAttributes(typeof(PermissionRequiredAttribute), true)
        //                        .Cast<PermissionRequiredAttribute>().Select(x => x.Permission).ToList();

        //                        var subPermissions =
        //                        httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>()?
        //                        .ControllerTypeInfo.AsType().GetMethod(httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>().ActionName)?
        //                        .GetCustomAttributes(typeof(PermissionRequiredAttribute), true)
        //                        .Cast<PermissionRequiredAttribute>().Select(x => x.Permission).ToList();

        //                        List<string> permissions = new List<string>();

        //                        foreach (var mainPermission in mainPermissions)
        //                        {
        //                            permissions.Add(mainPermission);
        //                            foreach (var subPermission in subPermissions)
        //                            {
        //                                permissions.Add(subPermission);
        //                                permissions.Add(mainPermission + "." + subPermission);
        //                            }
        //                        }

        //                        var handler = new JwtSecurityTokenHandler();
        //                        var jwtSecurityToken = handler.ReadJwtToken(token);

        //                        foreach (var permission in permissions)
        //                        {
        //                            if (permission != null && jwtSecurityToken.Claims.Where(x => x.Value == permission).FirstOrDefault() == null)
        //                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //                        }
        //                        await next(context);
        //                        return;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        //}

        public async Task InvokeAsync(HttpContext context, IJwtProvider jwtProvider, ISysSettingService settingService, IHttpContextAccessor httpContextAccessor)
        {
            var endPointt = context.GetEndpoint();
            if (endPointt?.Metadata?.GetMetadata<IAllowAnonymous>() is object)
            {
                await next(context);
                return;
            }

            string token = GetCurrentTokeAsync(context);

            var isValidTokenResponse = await jwtProvider.ValidateActiveToken(token) as DataResponse<bool>;

            if (isValidTokenResponse.IsDone)
            {
                var isValidToken = isValidTokenResponse.Data;
                if (isValidToken)
                {
                    var mainPermissions =
                    httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>()?
                    .ControllerTypeInfo.AsType().GetCustomAttributes(typeof(PermissionRequiredAttribute), true)
                    .Cast<PermissionRequiredAttribute>().Select(x => x.Permission).ToList();

                    var subPermissions =
                    httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>()?
                    .ControllerTypeInfo.AsType().GetMethod(httpContextAccessor.HttpContext?.GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>().ActionName)?
                    .GetCustomAttributes(typeof(PermissionRequiredAttribute), true)
                    .Cast<PermissionRequiredAttribute>().Select(x => x.Permission).ToList();

                    List<string> permissions = new List<string>();

                    if (mainPermissions != null)
                    {

                        foreach (var mainPermission in mainPermissions)
                        {
                            permissions.Add(mainPermission);

                            if (subPermissions != null)
                            {

                                foreach (var subPermission in subPermissions)
                                {
                                    permissions.Add(subPermission);
                                    permissions.Add(mainPermission + "." + subPermission);
                                }
                            }
                        }
                    }

                    var handler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = handler.ReadJwtToken(token);

                    foreach (var permission in permissions)
                    {
                        if (permission != null && jwtSecurityToken.Claims.Where(x => x.Value == permission).FirstOrDefault() == null)
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    }
                    await next(context);
                    return;
                }

                //if (isValidToken)
                //{
                //    var subPermissions =
                //    httpContextAccessor.HttpContext?
                //    .GetEndpoint()?.Metadata
                //    .GetMetadata<ControllerActionDescriptor>()?.ControllerTypeInfo.AsType()
                //    .GetMethod(httpContextAccessor.HttpContext?
                //    .GetEndpoint()?.Metadata.GetMetadata<ControllerActionDescriptor>().ActionName)?
                //    .GetCustomAttributes(typeof(PermissionRequiredAttribute), true)
                //    .Cast<PermissionRequiredAttribute>().Select(x => x.Permission).ToList();

                //    var handler = new JwtSecurityTokenHandler();
                //    var jwtSecurityToken = handler.ReadJwtToken(token);

                //    if (subPermissions != null)
                //    {
                //        foreach (var permission in subPermissions)
                //        {
                //            if (permission != null && jwtSecurityToken.Claims.All(x => x.Value != permission.ToString()))
                //            {
                //                // If permission not found in claims, return Unauthorized
                //                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                //                return;
                //            }
                //        }
                //    }

                //    await next(context);
                //    return;
                //}

            }



            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        }

        /// <summary>
        /// Get JWT Token From Request Header
        /// </summary>
        /// <param name="context"></param>  
        /// <returns></returns>
        private string GetCurrentTokeAsync(HttpContext context)
        {
            var authorizationHeader = context.Request.Headers["authorization"];

            return authorizationHeader == StringValues.Empty ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }
    }
}
