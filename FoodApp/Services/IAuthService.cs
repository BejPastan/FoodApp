using FoodApp.Models;
using FoodApp.Utilities;
using System.Net;

namespace FoodApp.Services
{
    /// <summary>
    /// interface for checking permissions
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Check if userhave permission to 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="permittedRoles"></param>
        Guid CheckPermissions(HttpRequest token, Roles[] permittedRoles);
    }

    /// <summary>
    /// Default implementation of IAuthService
    /// </summary>
    public class AuthService(IRoleService roleServ) : IAuthService
    {
        readonly IRoleService _roleServ = roleServ;

        /// <summary>
        /// Check if user have permission for given resource
        /// </summary>
        /// <param name="token"></param>
        /// <param name="permittedRoles"></param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>
        public Guid CheckPermissions(HttpRequest token, Roles[] permittedRoles)
        {
            try
            {
                Guid userId = Authentication.GetUserIdFromHeader(token).Value;

                Role userRole = _roleServ.GetRoleByUserId(userId);
                if(!permittedRoles.Contains(userRole.name))
                { 
                    throw new UnauthorizedAccessException("You don't have permission to do this");
                }
                return userId;
            }
            catch(Exception ex)
            {
                throw new UnauthorizedAccessException(ex.Message);
            }
        }
    }
}
