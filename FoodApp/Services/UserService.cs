using FoodApp.Models;
using FoodApp.Repositories;
using FoodApp.Repositories.Interfaces;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;

namespace FoodApp.Services
{
    /// <summary>
    /// Default implementation of IUserService
    /// </summary>
    /// <inheritdoc/>
    public class UserService(IUserRepository repo, IRoleRepository roleRepo, IUserTokenRepository userTokenRepository, IEmailService emailService, IRefreshTokenRepo refreshTokenRepo) : IUserService
    {
        private readonly IUserRepository _repo = repo;
        private readonly IRoleRepository _roleRepository = roleRepo;
        private readonly IUserTokenRepository _userTokenRepository = userTokenRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IRefreshTokenRepo _refreshTokenRepo = refreshTokenRepo;

        /// <inheritdoc/>
        public User? GetUserDataById(Guid id) => _repo.GetUserDataById(id);

        /// <inheritdoc/>
        public ExtendedUser? GetCurrentUser(Guid userId)
        {
            return _repo.GetExtendedUserDataById(userId);
        }

        #region user session cycle
        /// <inheritdoc/>
        public LoginResp LoginUser(string email, string password)
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

            return GetNewTokens(user.id);
        }

        /// <inheritdoc/>
        public LoginResp RefreshAuthToken(string oldRToken)
        {
            //check if refresh token is valid
            string hashedToken = Authentication.HashToken(oldRToken);
            var userId = _refreshTokenRepo.GetUserByToken(hashedToken).id;

            if (!_refreshTokenRepo.IsRefreshTokenValid(userId, hashedToken))
            {
                throw new UnauthorizedAccessException("your refresh token is not valid");
            }
            return GetNewTokens(userId);
        }

        private LoginResp GetNewTokens(Guid userId)
        {
            var token = Authentication.CreateAuthToken(userId);

            string rToken = Authentication.GenerateRefreshToken();
            RefreshTokenData refreshToken = new RefreshTokenData()
            {
                expirationDate = DateTime.UtcNow.AddSeconds(Constants.RefreshTokenExpire),
                token = Authentication.HashToken(rToken),
                userId = userId,
                used = false
            };
            var fullRToken = _refreshTokenRepo.SetRefreshToken(refreshToken);

            var resp = new LoginResp()
            {
                authToken = token,
                refreshToken = rToken
            };

            return resp;
        }

        /// <inheritdoc/>
        public bool Logout(Guid userId)
        {
            var toUpdate = new RefreshTokenData();
            toUpdate.userId = userId;
            toUpdate.used = true;
            return true;
        }
        #endregion

        #region signup
        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public bool ConfirmSignUp(string token)
        {
            var validToken = _userTokenRepository.GetValidToken(token);
            if (validToken == null)
            {
                return false;
            }

            var user = _userTokenRepository.GetUserByToken(token);
            if (user == null || user.userStatus != UserStatus.inactive)
            {
                throw new Exception("wrong token, or account does not exist");
            }

            user = _repo.ChangeUserStatus(user.id, UserStatus.active);
            if (user == null)
            {
                throw new Exception("Internal server error");
            }
            // Mark token as used
            _userTokenRepository.MarkTokenAsUsed(validToken.id);

            return true;
        }
        #endregion

        #region password reset

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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
        #endregion

        /// <inheritdoc/>
        public ExtendedUser UpdateUser(UserUpdateRequest request, Guid userId)
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

        /// <inheritdoc/>
        public bool DeleteUser(Guid userId)
        {
            _repo.DeleteUser(userId);
            return true;
        }
    }
}
