using SHUT.Core.Domain.Administration;

namespace SHUT.Core
{
    public class DTOs
    {
        public record UserDTO(string Mail, string Password);

        public record UserD(int Id, string Email, string Nom, int ProfilId, string ProfilName, int ProfilCode, bool Actif)
        {
            public static UserD FromUser(User user) => new UserD(
                user.Id,
                user.Email,
                user.Nom,
                user.ProfilId,
                user.ProfilName,
                user.ProfilCode,
                user.Actif
            );
        }

        public record UserCredentials(string Nom, int ProfilId, string ProfilName, int ProfilCode, string Token);
    }
}