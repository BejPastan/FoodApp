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
        User UpdateUser(int userId, string? name = null, string? password = null, string? email = null);
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
                    lastLogin = inserted.lastLogin
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
            var updateSql = "UPDATE users SET lastLogin = @lastLogin WHERE id = @id;";
            try
            {
                DBConnector.QueryDatabase<int>(updateSql, new { lastLogin = DateTime.Now, id = userId }).ToList();
            }
            catch
            {
                Console.WriteLine("error updating last login");
            }

            var fetchSql = "SELECT id, name, email, lastLogin FROM users WHERE id = @id;";
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
            user.lastLogin = null;
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
            var sql = "SELECT users.*, r.name as role FROM users LEFT JOIN user_roles ur ON ur.user_id = users.id LEFT JOIN role r ON r.id = ur.role_id WHERE users.id = @id";
            var user = DBConnector.QueryDatabase<ExtendedUser>(sql, new { id }).FirstOrDefault();
            if (user == null) return null;
            user.roleName = user.role.GetDisplayName();
            user.password = string.Empty;
            return user;
        }

        public User UpdateUser(int userId, string? name = null, string? password = null, string? email = null)
        {
            var sql = @"UPDATE users
                        SET 
                            name = COALESCE(@Name, name),
                            password = COALESCE(@Password, password),
                            email = COALESCE(@Email, email)
                        OUTPUT INSERTED.*
                        WHERE id = @UserId;";

            Console.WriteLine(password);

            var parameters = new 
            { 
                UserId = userId, 
                Name = name, 
                Password = password, 
                Email = email 
            };

            var results = DBConnector.QueryDatabase<User>(sql, parameters);
            return results.FirstOrDefault();
        }
    }
}
