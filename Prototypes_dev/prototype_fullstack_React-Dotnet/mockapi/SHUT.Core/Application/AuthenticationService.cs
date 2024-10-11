using Dapper;
using InfluxDB.Client.Api.Domain;
using Novell.Directory.Ldap;
using SHUT.Core.Data;
using SHUT.Core.Domain;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static SHUT.Core.DTOs;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;

namespace SHUT.Core.Application
{
    public class AuthenticationService
    {
        private readonly LDapIdentifier _ldapIdentifier;
        private readonly AppDbContext _context;
        private readonly JWTSettings _tokenSettings;

        string tokenString = "Not authenticated";

        public AuthenticationService(AppDbContext context, LDapIdentifier ldapIdentifier, IOptions<JWTSettings> tokenSettings)
        {
            _context = context;
            _ldapIdentifier = ldapIdentifier;
            _tokenSettings = tokenSettings.Value;
        }


        public async Task<UserD> resolveUserIdentity(UserDTO userD)
        {
            Dictionary<string, string> ldapData = _ldapIdentifier.GetLDAPDataIdentifiers(
                userD.Username,
                userD.Password
            );

            string name = ldapData["sn"].ToLower();
            string email = ldapData["mail"].ToLower();
            var query = $"SELECT * FROM users WHERE email='{email}'";
            using var connection = _context.CreateConnection();
            var users = await connection.QueryAsync<Domain.User>(query);
            Domain.User user = users.FirstOrDefault();
            if (user is null)
                return null;
            return new UserD(user.Nom, user.Email, user.Role, user.Statut);
        }

        public async Task<UserCredentials> generateToken(UserD user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_tokenSettings.SecretKey)
            );

            var expiryMin = 1440.0;
            double.TryParse(_tokenSettings.ExpirationMinutes, out expiryMin);
            var expiryTime = DateTime.UtcNow.AddMinutes(expiryMin);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                    }
                ),
                Issuer = _tokenSettings.Issuer,
                Audience = _tokenSettings.Audience,
                Expires = expiryTime, // Token expiration time
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return new UserCredentials(user.Username, user.Role, tokenString);
        }




    }
}
