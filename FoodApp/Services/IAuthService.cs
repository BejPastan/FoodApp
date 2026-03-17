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
        void CheckPermissions(string token, Roles[] permittedRoles);
    }

    public class AuthService : IAuthService
    {
        IRoleService _roleServ;

        public AuthService(IRoleService roleServ)
        {
            _roleServ = roleServ;
        }

        public void CheckPermissions(string token, Roles[] permittedRoles)
        {
            if (string.IsNullOrWhiteSpace(token) || !token.StartsWith("Bearer "))
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }

            token = token.Substring(7); // Remove "Bearer " prefix
            var userId = Authentication.GetUserIdFromToken(token);

            if (userId == null)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }


            Role userRole = _roleServ.GetRoleByUserId(userId.Value);
            if(!permittedRoles.Contains(userRole.name))
            { 
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
        }
    }
}
