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
        bool SignUpUser(string name, string email, string password);
        /// <summary>
        /// check if user accound exist, if yes, generate token, and send email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public bool StartPasswordReset(string email);
        /// <summary>
        /// get user  from token, and change it's password
        /// </summary>
        /// <param name="token"></param>
        /// <param name="newPass"></param>
        /// <returns></returns>
        public bool ConfirmPasswordReset(string token, string newPass);
        /// <summary>
        /// Confirm user sign up using token, change user state to active
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool ConfirmSignUp(string token);
        /// <summary>
        /// Update user name, and/or email, when updating email, check if such email is not occupied yet
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public ExtendedUser UpdateUser(UserUpdateRequest request, int userId);

        /// <summary>
        /// delete user record
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteUser(int userId);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _repo;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IEmailService _emailService;
        
        public UserService(IUserRepository repo, IRoleRepository roleRepo, IUserTokenRepository userTokenRepository, IEmailService emailService) 
        { 
            _repo = repo;
            _roleRepository = roleRepo;
            _userTokenRepository = userTokenRepository;
            _emailService = emailService;
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

        /// <summary>
        /// create new account and sent confirmation email
        /// </summary>
        /// <param name="name">user name</param>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public bool SignUpUser(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("email, password and name are required");
            }
            
            var hashed = Authentication.HashPassword(password);
            var user = _repo.SignUpUser(name, hashed, email);

            try
            {
                _roleRepository.AddRoleToUser(user.id, Roles.admin);
                
                // Generate confirmation token
                var confirmationCode = Authentication.GenerateSixDigitCode();
                var tokenRequest = new CreateUserTokenRequest
                {
                    token = confirmationCode,
                    expirationDate = DateTime.UtcNow.AddHours(24),
                    used = false,
                    userId = user.id
                };
                
                _userTokenRepository.AddToken(tokenRequest);
                
                // Send confirmation email
                var placeholders = new Dictionary<string, string>
                {
                    { "{{Name}}", name },
                    { "{{ConfirmationCode}}", confirmationCode }
                };
                
                _emailService.SendEmailFromTemplate(null, "AccountConfirmation", email, placeholders);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _repo.DeleteUser(user.id);
                throw;
            }

            return true;
        }

        public bool ConfirmSignUp(string token)
        {
            var validToken = _userTokenRepository.GetValidToken(token);
            if (validToken == null)
            {
                return false;
            }

            var user = _userTokenRepository.GetUserByToken(token);
            if(user == null || user.userStatus != UserStatus.inactive)
            {
                throw new Exception("wrong token, or account does not exist");
            }

            user = _repo.ChangeUserStatus(user.id, UserStatus.active);
            if(user==null)
            {
                throw new Exception("Internal server error");
            }
            // Mark token as used
            _userTokenRepository.MarkTokenAsUsed(validToken.id);

            return true;
        }


        public bool StartPasswordReset(string email)
        {
            var user = _repo.GetUserByEmail(email);
            if (user == null)
            {
                return true;
            }

            var resetCode = Authentication.GenerateSixDigitCode();
            var tokenRequest = new CreateUserTokenRequest
            {
                token = resetCode,
                expirationDate = DateTime.UtcNow.AddHours(1),
                used = false,
                userId = user.id
            };
            
            _userTokenRepository.AddToken(tokenRequest);
            
            var placeholders = new Dictionary<string, string>
            {
                { "{{Name}}", user.name },
                { "{{ResetCode}}", resetCode }
            };
            
            try
            {
                _emailService.SendEmailFromTemplate(null, "PasswordReset", email, placeholders);
            }
            catch
            {
                
            }

            return true;
        }

        public bool ConfirmPasswordReset(string token, string newPass)
        {
            var validToken = _userTokenRepository.GetValidToken(token);
            if (validToken == null)
            {
                return false;
            }

            // Hash new password
            var hashedPassword = Authentication.HashPassword(newPass);
            
            // Update user password
            _repo.UpdateUser(validToken.userId, password: hashedPassword);
            
            // Mark token as used
            _userTokenRepository.MarkTokenAsUsed(validToken.id);

            return true;
        }

        public ExtendedUser UpdateUser(UserUpdateRequest request, int userId)
        {
            var existingUser = _repo.GetUserDataById(userId);
            if (existingUser == null)
            {
                throw new ArgumentException("User not found");
            }

            // Check if email is already in use if updating email
            if (!string.IsNullOrEmpty(request.email) && request.email != existingUser.email)
            {
                var existingEmailUser = _repo.GetUserByEmail(request.email);
                if (existingEmailUser != null)
                {
                    throw new InvalidOperationException("Email address is already in use");
                }
            }

            // Update user with provided fields
            _repo.UpdateUser(userId, name: request.name, email: request.email);

            // Return updated user with extended info
            return _repo.GetExtendedUserDataById(userId);
        }

        public bool DeleteUser(int userId)
        {
            _repo.DeleteUser(userId);
            return true;
        }
    }
}