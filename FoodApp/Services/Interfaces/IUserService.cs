using FoodApp.Models;
using FoodApp.Utilities;
using FoodApp.Repositories;

namespace FoodApp.Services.Interfaces
{
    /// <summary>
    /// Service for managing users
    /// </summary>
    public interface IUserService
    {
        User? GetUserDataById(Guid id);
        ExtendedUser? GetCurrentUser(Guid userId);

        /// <summary>
        /// Confirm signup and actiate user account
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        LoginResp LoginUser(string email, string password);

        /// <summary>
        /// Generate new auth token, and refresh token, if provided refresh token is valid
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        public LoginResp RefreshAuthToken(string refreshToken);

        /// <summary>
        /// Invalidate refresh token, and end session
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Logout(Guid userId);

        /// <summary>
        /// create new account and sent confirmation email
        /// </summary>
        /// <param name="name">user name</param>
        /// <param name="email">user email</param>
        /// <param name="password">user password</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
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
        public ExtendedUser UpdateUser(UserUpdateRequest request, Guid userId);
        /// <summary>
        /// delete user record
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool DeleteUser(Guid userId);
    }

    public struct LoginResp
    {
        public string authToken;
        public string refreshToken;
    }
}