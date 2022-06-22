using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace GrpcDotNetDemo.Server.Services
{
    public class JwtTokenAuthService : IJwtTokenAuthService
    {
        private readonly string _securityKey;

        public JwtTokenAuthService(string securityKey)
        {
            _securityKey = securityKey;
        }

        public string GenerateToken(string username, string password)
        {
            if (!(username.Equals("user1") || password.Equals("password123")))
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            var securityKeyBytes = Encoding.ASCII.GetBytes(_securityKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddMinutes(20),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(securityKeyBytes), SecurityAlgorithms.HmacSha256Signature)
            };
            
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
