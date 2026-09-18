using FoodApp.Models;

namespace FoodApp.Repositories.Interfaces
{
    /// <summary>
    /// Interface for managing refresh token
    /// </summary>
    public interface IRefreshTokenRepo
    {
        /// <summary>
        /// Check if refresh token is valid
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="hashedToken"></param>
        /// <returns></returns>
        bool IsRefreshTokenValid(Guid userId, string hashedToken);
        /// <summary>
        /// Set refresh token for user in database, if token for this user already exist, change data
        /// </summary>
        /// <param name="toAdd"></param>
        /// <returns></returns>
        FullRefreshToken SetRefreshToken(RefreshTokenData toAdd);
        User GetUserByToken(string refreshToken);
    }
}
