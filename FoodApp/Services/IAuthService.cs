using FoodApp.Models;
using FoodApp.Utilities;
using System.Net;

namespace FoodApp.Services
{
    public interface IAuthService
    {
        /// <summary>
        /// Check if userhave permission to 
        /// </summary>
        /// <param name="token"></param>
        int CheckPermissions(HttpRequest token, Roles[] permittedRoles);
    }

    public class AuthService : IAuthService
    {
        IRoleService _roleServ;

        public AuthService(IRoleService roleServ)
        {
            _roleServ = roleServ;
        }

        public int CheckPermissions(HttpRequest token, Roles[] permittedRoles)
        {
            try
            {
                int userId = Authentication.GetUserIdFromHeader(token).Value;

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
