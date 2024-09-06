using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AngularApp.Server.DBConnection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Net;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AngularApp.Server.Controllers
{
    [ApiController]
    [Route("/authenticateUser")]
    public class AuthenticationController : ControllerBase
    {

        public readonly IConfiguration _config;

        public AuthenticationController(IConfiguration config)
            // , IUserService userService
        {
            _config = config;
            // string? customData = _config["Jwt:Issuer"];
            // string? customData = _config["Jwt:Key"];
            // System.Diagnostics.Debug.WriteLine("customData ", JsonSerializer.Serialize(customData));
        }

        private string GenerateAccessToken(IConfiguration config, AuthenticationInput authenticationInput )
        {
            string? securityKey = _config["Jwt:Key"];
            
            var symmsecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var credentials = new SigningCredentials(symmsecurityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim("username", authenticationInput.UserName),
                new Claim("email", "anshit.sachdeva@cognizant.com"),
                new Claim("role", "END_USER")
            };

            var token = new JwtSecurityToken(
              issuer: _config["Jwt:Issuer"],
              audience: _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        // Either it returns the authenticated user Token or Error

        public ActionResult AuthenticateUser([FromBody] AuthenticationInput authenticationInput)
        {
            System.Diagnostics.Debug.WriteLine("authentication ", JsonSerializer.Serialize(authenticationInput));

            DatabaseConnect dbConnection = new();

            string dbQuery = $"SELECT * from employees WHERE FIRST_NAME = '{authenticationInput.UserName}'";
            System.Diagnostics.Debug.WriteLine("<== dbQuery ==>", (dbQuery));

            // Find any matching user present in DB
            List<User> userData = dbConnection.GetUserDetails(dbQuery);
            System.Diagnostics.Debug.WriteLine("<== userData ==>", JsonSerializer.Serialize(userData));

            if(Convert.ToBoolean(userData.Count))
            {
                var tokenString = GenerateAccessToken(_config, authenticationInput);

                System.Diagnostics.Debug.WriteLine("tokenString output ", JsonSerializer.Serialize(tokenString));

                var response = new AuthenticationResponse
                {
                    message = "User is authenticated",
                    token = tokenString
                };
                return StatusCode(StatusCodes.Status200OK, response);
            } 
            else {
                var response = new AuthenticationResponse
                {
                    message = "No such user exists"
                };
                return StatusCode(StatusCodes.Status500InternalServerError, response);
            }
            
        }

    }
}
