namespace ProtoBack.Repositories
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading.Tasks;
    using Dapper;
    using ProtoBack.Data;
    using ProtoBack.Models;

    public class UserRepository
    {
        private readonly DapperContext _context;

        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsersDapper()
        {
            var query = "SELECT * FROM Users";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryAsync<User>(query);
            }
        }

        public async Task<int> AddUserDapper(User user)
        {
            var query =
                "INSERT INTO Users (Nom, Passe, Groupe, Droits, Page1) VALUES (@Nom, @Passe, @Groupe, @Droits, @Page1)";
            using (var connection = _context.CreateConnection())
            {
                return await connection.ExecuteAsync(query, user);
            }
        }

        public async Task<User?> LoginUserDapper(string nom, string passe)
        {
            var query = "SELECT * FROM Users WHERE Nom = @Nom AND Passe = @Passe";
            using (var connection = _context.CreateConnection())
            {
                return await connection.QueryFirstOrDefaultAsync<User>(
                    query,
                    new { Nom = nom, Passe = passe }
                );
            }
        }
    }
}
