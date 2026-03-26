using FoodApp.Models;
using FoodApp.Utilities;
using FoodApp.Repositories;

namespace FoodApp.Services
{
    public interface IUserService
    {
        User? GetUserDataById(int id);
        ExtendedUser? GetCurrentUser(int userId);
        string LoginUser(string email, string password);
        string SignUpUser(string name, string email, string password);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IRoleRepository _roleRepository;
        public UserService(IUserRepository repo, IRoleRepository roleRepo) 
        { 
            _repo = repo;
            _roleRepository = roleRepo;
        }
        
        public User? GetUserDataById(int id) => _repo.GetUserDataById(id);
        
        public ExtendedUser? GetCurrentUser(int userId)
        {
            return _repo.GetExtendedUserDataById(userId);
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

            try
            {
                _roleRepository.AddRoleToUser(user.id, Roles.user);
            }
            catch (Exception ex)
            {
                //delete user
                _repo.DeleteUser(user.id);
            }

            var token = Authentication.CreateAuthToken(user.id);
            return token;
        }
    }
}