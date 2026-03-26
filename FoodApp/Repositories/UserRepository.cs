using FoodApp.Utilities;
using FoodApp.Models;
using System.Security.Authentication.ExtendedProtection;
using Microsoft.OpenApi.Extensions;

namespace FoodApp.Repositories
{
    public interface IUserRepository
    {
        User SignUpUser(string username, string hashedPassword, string email);
        User? GetUserByEmail(string email);
        User LoginUser(int userId);
        User? GetUserDataById(int id);
        ExtendedUser? GetExtendedUserDataById(int id);
        bool DeleteUser(int id);
    }

    public class UserRepository : IUserRepository
    {
        public User SignUpUser(string username, string hashedPassword, string email)
        {
            var sql = "INSERT INTO users (name, password, email) OUTPUT INSERTED.* VALUES (@Username, @Password, @Email);";
            var list = DBConnector.QueryDatabase<User>(sql, new { Username = username, Password = hashedPassword, Email = email }).ToList();
            if (list.Count > 0)
            {
                var inserted = list[0];
                return new User
                {
                    id = inserted.id,
                    name = inserted.name,
                    email = inserted.email,
                    password = string.Empty,
                    last_login = inserted.last_login
                };
            }
            throw new InvalidOperationException("Failed to insert user.");
        }

        public User? GetUserByEmail(string email)
        {
            var sql = "SELECT * FROM users WHERE email = @email;";
            return DBConnector.QueryDatabase<User>(sql, new { email = email ?? string.Empty }).FirstOrDefault();
        }

        public User LoginUser(int userId)
        {
            var updateSql = "UPDATE users SET last_login = @last_login WHERE id = @id;";
            try
            {
                DBConnector.QueryDatabase<int>(updateSql, new { last_login = DateTime.Now, id = userId }).ToList();
            }
            catch
            {
                Console.WriteLine("error updating last login");
            }

            var fetchSql = "SELECT id, name, email, last_login FROM users WHERE id = @id;";
            var user = DBConnector.QueryDatabase<User>(fetchSql, new { id = userId }).FirstOrDefault();
            if (user == null)
            {
                throw new InvalidOperationException("Failed to retrieve user after login.");
            }
            user.password = string.Empty;
            return user;
        }

        public User? GetUserDataById(int id)
        {
            var sql = "SELECT id, name, email FROM users WHERE id = @id;";
            var user = DBConnector.QueryDatabase<User>(sql, new { id = id }).FirstOrDefault();
            if (user == null) return null;
            user.password = string.Empty;
            user.last_login = null;
            return user;
        }

        public bool DeleteUser(int id)
        {
            var sql = "DELETE FROM users WHERE user_id = @userId";
            DBConnector.QueryDatabase<int>(sql, new {userId = id}).FirstOrDefault();
            return true;
        }

        public ExtendedUser? GetExtendedUserDataById(int id)
        {
            var sql = "SELECT users.*, r.name as role FROM users LEFT JOIN user_roles ur ON ur.user_id = users.id LEFT JOIN role r ON r.id = ur.role_id;";
            var user = DBConnector.QueryDatabase<ExtendedUser>(sql, new { id = id }).FirstOrDefault();
            user.roleName = user.role.GetDisplayName();
            if (user == null) return null;
            user.password = string.Empty;
            return user;
        }
    }
}
