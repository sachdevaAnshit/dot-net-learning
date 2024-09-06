using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace AngularApp.Server
{
    public class UserCustomMiddleWare
    {
        private readonly RequestDelegate _next;
        public readonly IConfiguration _config;

        public UserCustomMiddleWare(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }
        
        public string? ValidateToken(string token)
        {
            System.Diagnostics.Debug.WriteLine("validation entered");
            System.Diagnostics.Debug.WriteLine("_config[Jwt: Key]");
            System.Diagnostics.Debug.WriteLine(_config["Jwt:Key"]);
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userName= (jwtToken.Claims.First(x => x.Type == "username").Value);

                // System.Diagnostics.Debug.WriteLine("userName ==> ");
                // System.Diagnostics.Debug.WriteLine(userName);

                System.Diagnostics.Debug.WriteLine("validated token ==> ");
                System.Diagnostics.Debug.WriteLine(JsonSerializer.Serialize(jwtToken));
                System.Diagnostics.Debug.WriteLine(JsonSerializer.Serialize(jwtToken.Claims));
                
                System.Diagnostics.Debug.WriteLine("userName");
                System.Diagnostics.Debug.WriteLine(userName);

                return userName;
            }
            catch
            {
                // return null if validation fails
                System.Diagnostics.Debug.WriteLine("validation catch block");
                return null;
            }
        }

        public async Task InvokeAsync(HttpContext context)
        {

            var handler = new JwtSecurityTokenHandler();
            string authHeader = context.Request.Headers["Authorization"];
            if (authHeader != null)
            {

                authHeader = authHeader.Replace("Bearer ", "");
                var jsonToken = handler.ReadToken(authHeader);
                System.Diagnostics.Debug.WriteLine("jsonToken ==> ", jsonToken);
                var decodedToken = handler.ReadToken(authHeader) as JwtSecurityToken;

                System.Diagnostics.Debug.WriteLine("decodedToken ==> ");
                System.Diagnostics.Debug.WriteLine(JsonSerializer.Serialize(decodedToken));

                string? userName = ValidateToken(authHeader);

                System.Diagnostics.Debug.WriteLine("output of ValidateToken() == ", userName);

                if (ValidateToken(authHeader) == null)
                {
                    context.Response.StatusCode = (int)StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync("{'message': 'User API is not passed!!!'}");
                    return;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("before context.Items == ", JsonSerializer.Serialize(context.Items));
                    context.Items["Username"] = userName;
                    System.Diagnostics.Debug.WriteLine("after context.Items == ", JsonSerializer.Serialize(context.Items));
                    await _next(context);
                }
            }
            else
            {
                // Call the next delegate/middleware in the pipeline.
                System.Diagnostics.Debug.WriteLine("call next middleware");
                await _next(context);
            }
        }
    }

    public static class UserCustomMiddleWareExtensions
    {
        public static IApplicationBuilder UseUserCustomMiddleWare(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserCustomMiddleWare>();
        }
    }

}
