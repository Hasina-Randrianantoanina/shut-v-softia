using SHUT.Core.Domain;
using SHUT.Core.Data;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SHUT.Core.Application
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
            var query = "SELECT * FROM users";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<User>(query);
        }

        public async Task<int> AddUser(User user)
        {
            var query =
                "INSERT INTO users (Nom, Passe, Groupe, Droits, Page1) VALUES (@Nom, @Passe, @Groupe, @Droits, @Page1)";
            using var connection = _context.CreateConnection();
            return await connection.ExecuteAsync(query, user);
        }
    }
}
