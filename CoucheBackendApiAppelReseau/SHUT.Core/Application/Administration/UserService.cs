using SHUT.Core.Data;
using Dapper;
using SHUT.Core.Domain.Administration;

namespace SHUT.Core.Application.Administration
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            try
            {
                var query = @"
                    SELECT u.id, u.email, u.nom, u.profil_id AS ProfilId, u.actif,
                        p.profil AS ProfilName
                    FROM administration.utilisateurs u
                    LEFT JOIN administration.profils p ON u.profil_id = p.id";

                using var connection = _context.CreateConnection();

                var users = await connection.QueryAsync<User>(query);

                return users;
            }
            catch (Exception ex)
            {
                throw new Exception("Erreur lors de la récupération des utilisateurs", ex);
            }
        }

        public async Task<int> AddUser(User user)
        {
            try
            {
                var query = @"
                    INSERT INTO administration.utilisateurs (email, nom, profil_id, actif)
                    VALUES (@Email, @Nom, @ProfilId, @Actif)
                    RETURNING id";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(query, new
                {
                    user.Email,
                    user.Nom,
                    user.ProfilId,
                    user.Actif
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'ajout de l'utilisateur {user.Nom}", ex);
            }
        }

        public async Task<int> UpdateUser(User user)
        {
            try
            {
                var query = @"
                    UPDATE administration.utilisateurs
                    SET email = @Email, nom = @Nom, profil_id = @ProfilId, actif = @Actif
                    WHERE id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, new
                {
                    user.Id,
                    user.Email,
                    user.Nom,
                    user.ProfilId,
                    user.Actif
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la mise à jour de l'utilisateur {user.Nom}", ex);
            }
        }

        public async Task<User> GetUserById(int userId)
        {
            try
            {
                var query = @"
                    SELECT u.id, u.email, u.nom, u.profil_id AS ProfilId, u.actif,
                        p.profil AS ProfilName
                    FROM administration.utilisateurs u
                    LEFT JOIN administration.profils p ON u.profil_id = p.id
                    WHERE u.id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = userId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la récupération de l'utilisateur avec l'ID {userId}", ex);
            }
        }

        public async Task<int> DeleteUser(int userId)
        {
            try
            {
                var query = "DELETE FROM administration.utilisateurs WHERE id = @Id";
                using var connection = _context.CreateConnection();
                return await connection.ExecuteAsync(query, new { Id = userId });
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la suppression de l'utilisateur avec l'ID {userId}", ex);
            }
        }
    }
}