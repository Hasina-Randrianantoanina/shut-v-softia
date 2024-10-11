using Dapper;
using SHUT.Core.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using SHUT.Core.Domain.Administration;
using static SHUT.Core.DTOs;
using Microsoft.Extensions.Logging;

namespace SHUT.Core.Application.Administration
{
    public class AuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly JWTSettings _tokenSettings;
        private readonly ILogger<AuthenticationService> _logger;
        // private readonly LDapIdentifier _ldapIdentifier;

        // A décommenter pour activer l'authentification LDAP
        /*
        public AuthenticationService(AppDbContext context, LDapIdentifier ldapIdentifier,
            IOptions<JWTSettings> tokenSettings, ILogger<AuthenticationService> logger)
        {
            _context = context;
            _ldapIdentifier = ldapIdentifier;
            _tokenSettings = tokenSettings.Value;
            _logger = logger;
        }
        */

        public AuthenticationService(AppDbContext context, IOptions<JWTSettings> tokenSettings, ILogger<AuthenticationService> logger)
        {
            _context = context;
            _tokenSettings = tokenSettings.Value;
            _logger = logger;
        }

        // Méthode pour developpement sans LDAP
        public async Task<User> ResolveUserIdentity(string mail, string password)
        {
            try
            {
                _logger.LogInformation($"Tentative d'authentification simplifiée pour l'utilisateur: {mail}");

var query = @"
    SELECT u.id, u.email, u.nom, u.profil_id AS ProfilId, u.actif,
        p.profil AS ProfilName, p.code AS ProfilCode
    FROM administration.utilisateurs u
    LEFT JOIN administration.profils p ON u.profil_id = p.id
    WHERE u.email = @Email";
                using var connection = _context.CreateConnection();
                var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { Email = mail });

                if (user == null)
                {
                    _logger.LogWarning($"Aucun utilisateur trouvé pour l'email: {mail}");
                    return null;
                }

                _logger.LogInformation($"Utilisateur trouvé et authentifié: {user.Nom}");
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors de l'authentification de l'utilisateur: {mail}");
                throw;
            }
        }

        // A décommenter pour activer l'authentification LDAP
        //  public async Task<User> ResolveUserIdentity(string mail, string password)
        // {
        //     try
        //     {
        //         _logger.LogInformation($"Tentative d'authentification pour l'utilisateur: {mail}");
        //         Dictionary<string, string> ldapData = _ldapIdentifier.GetLDAPDataIdentifiers(mail);

        //         if (!ldapData.ContainsKey("mail"))
        //         {
        //             _logger.LogWarning("Aucun email trouvé dans les données LDAP");
        //             return null;
        //         }

        //         string email = ldapData["mail"].ToLower();
        //         string ldapCn = ldapData["cn"];

        //         if (!_ldapIdentifier.ValidateConnect(ldapCn, password))
        //         {
        //             _logger.LogWarning("Mauvais identifiants LDAP");
        //             return null;
        //         }

        //         _logger.LogInformation($"Recherche de l'utilisateur dans la base de données avec l'email: {email}");

        //         var query = @"
        //             SELECT u.id, u.email, u.nom, u.profil_id, u.actif,
        //                 p.profil AS ProfilName
        //             FROM administration.utilisateurs u
        //             LEFT JOIN administration.profils p ON u.profil_id = p.id
        //             WHERE u.email = @Email";

        //         using var connection = _context.CreateConnection();
        //         var user = await connection.QuerySingleOrDefaultAsync<User>(query, new { Email = email });

        //         if (user == null)
        //         {
        //             _logger.LogWarning($"Aucun utilisateur trouvé pour l'email: {email}");
        //         }
        //         else
        //         {
        //             _logger.LogInformation($"Utilisateur trouvé: {user.Nom}");
        //         }

        //         return user;
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError(ex, $"Erreur lors de l'authentification de l'utilisateur: {mail}");
        //         throw;
        //     }
        // }

        public async Task<UserCredentials> GenerateToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.SecretKey));

            var expiryMin = 1440.0;
            double.TryParse(_tokenSettings.ExpirationMinutes, out expiryMin);
            var expiryTime = DateTime.UtcNow.AddMinutes(expiryMin);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Nom),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.ProfilName),
                    new Claim("ProfilId", user.ProfilId.ToString()),
                    new Claim("ProfilName", user.ProfilName),
new Claim("ProfilCode", user.ProfilCode.ToString()),
                }
                ),
                Issuer = _tokenSettings.Issuer,
                Audience = _tokenSettings.Audience,
                Expires = expiryTime,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            string tokenString = tokenHandler.WriteToken(token);

            return new UserCredentials(user.Nom, user.ProfilId, user.ProfilName, user.ProfilCode, tokenString);

        }
    }
}