using FoodApp.Models;
using FoodApp.Utilities;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json.Linq;
using System.Security.Authentication.ExtendedProtection;

namespace FoodApp.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Create new user as inactive
        /// </summary>
        /// <param name="username"></param>
        /// <param name="hashedPassword"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        User SignUpUser(string username, string hashedPassword, string email);
        /// <summary>
        /// set new status to user
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        User ChangeUserStatus(Guid id, UserStatus newStatus);
        /// <summary>
        /// find user by email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        User? GetUserByEmail(string email);
        /// <summary>
        /// set user data in database to be as logged in user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        User LoginUser(Guid userId);
        /// <summary>
        /// Find user by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User? GetUserDataById(Guid id);
        /// <summary>
        /// get full user data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ExtendedUser? GetExtendedUserDataById(Guid id);
        /// <summary>
        /// Delete user record from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteUser(Guid id);
        User UpdateUser(Guid userId, string? name = null, string? password = null, string? email = null);
    }

    public class UserRepository : IUserRepository
    {
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public User? GetUserByEmail(string email)
        {
            var sql = "SELECT * FROM users WHERE email = @email;";
            return DBConnector.QueryDatabase<User>(sql, new { email = email ?? string.Empty }).FirstOrDefault();
        }

        /// <inheritdoc/>
        public User LoginUser(Guid userId)
        {
            var updateSql = "UPDATE users SET lastLogin = @lastLogin WHERE id = @id;";
            try
            {
                DBConnector.QueryDatabase<Guid>(updateSql, new { lastLogin = DateTime.Now, id = userId }).ToList();
            }
            catch
            {
                Console.WriteLine("error updating last login");
            }

            var fetchSql = "SELECT id, name, email, lastLogin FROM users WHERE id = @id AND userStatus = 'active';";
            var user = DBConnector.QueryDatabase<User>(fetchSql, new { id = userId }).FirstOrDefault();
            if (user == null)
            {
                throw new AccessViolationException("user does not exist or is not active");
            }
            user.password = string.Empty;
            return user;
        }

        /// <summary>
        /// Return user with given ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User? GetUserDataById(Guid id)
        {
            var sql = "SELECT id, name, email FROM users WHERE id = @id;";
            var user = DBConnector.QueryDatabase<User>(sql, new { id = id }).FirstOrDefault();
            if (user == null) return null;
            user.password = string.Empty;
            user.lastLogin = null;
            return user;
        }

        /// <inheritdoc/>
        public bool DeleteUser(Guid id)
        {
            var sql = "DELETE FROM users WHERE user_id = @userId";
            DBConnector.QueryDatabase<Guid>(sql, new {userId = id}).FirstOrDefault();
            return true;
        }

        /// <inheritdoc/>
        public ExtendedUser? GetExtendedUserDataById(Guid id)
        {
            var sql = "SELECT users.*, r.name as role FROM users LEFT JOIN user_roles ur ON ur.user_id = users.id LEFT JOIN role r ON r.id = ur.role_id WHERE users.id = @id";
            var user = DBConnector.QueryDatabase<ExtendedUser>(sql, new { id }).FirstOrDefault();
            if (user == null) return null;
            user.roleName = user.role.GetDisplayName();
            user.password = string.Empty;
            return user;
        }

        /// <inheritdoc/>
        public User UpdateUser(Guid userId, string? name = null, string? password = null, string? email = null)
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

        /// <inheritdoc/>
        public User ChangeUserStatus(Guid userId, UserStatus newStatus)
        {
            var sql = "UPDATE users SET userStatus = @newStatus OUTPUT INSERTED.* WHERE id = @userId";
            var parameters = new { newStatus = newStatus.ToString(), userId = userId};
            var result = DBConnector.QueryDatabase<User>(sql,parameters);
            return result.FirstOrDefault();
        }

    }
}
