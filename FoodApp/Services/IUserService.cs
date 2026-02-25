using FoodApp.Models;
using FoodApp.Utilities;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IUserService
    {
        User? GetUserDataById(int id);
        User? GetCurrentUser(string authorization);
        string LoginUser(string email, string password);
        string SignUpUser(string name, string email, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        public UserService(IUserRepository repo) { _repo = repo; }
        
        public User? GetUserDataById(int id) => _repo.GetUserDataById(id);
        
        public User? GetCurrentUser(string authorization)
        {
            if (string.IsNullOrWhiteSpace(authorization) || !authorization.StartsWith("Bearer "))
            {
                return null;
            }

            var token = authorization.Substring(7); // Remove "Bearer " prefix
            var userId = Authentication.GetUserIdFromToken(token);
            Console.WriteLine($"userId: {userId}");
            
            if (userId == null)
            {
                return null;
            }

            return _repo.GetUserDataById(userId.Value);
        }

        public string LoginUser(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                throw new ArgumentException("email and password are required");
            }
            
            var user = _repo.GetUserByEmail(email);
            if (user == null || !Authentication.CheckPassword(password, user.password))
            {
                throw new UnauthorizedAccessException("invalid email or password");
            }
            
            user = _repo.LoginUser(user.id);
            var token = Authentication.CreateAuthToken(user.id);
            return token;
        }

        public string SignUpUser(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("email, password and name are required");
            }
            
            var hashed = Authentication.HashPassword(password);
            var user = _repo.SignUpUser(name, hashed, email);
            var token = Authentication.CreateAuthToken(user.id);
            return token;
        }
    }
}