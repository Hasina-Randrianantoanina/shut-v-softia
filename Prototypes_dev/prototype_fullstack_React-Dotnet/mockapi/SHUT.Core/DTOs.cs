namespace SHUT.Core
{
    public class DTOs
    {
        public record UserDTO(string Username, string Password);
        public record UserD(string Username, string Email, string Role, string Statut);
        public record UserCredentials(string Username, string Role, string Token);
    }
}
