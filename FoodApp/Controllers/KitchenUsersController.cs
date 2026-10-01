using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing members/users of a kitchen
    /// </summary>
    [ApiController]
    [Route("api/kitchen/")]
    public class KitchenUsersController : ControllerBase
    {
        private readonly IKitchenUsersService _kitchenUsersService;

        /// <summary>
        /// Initializes a new instance of KitchenUsersController
        /// </summary>
        public KitchenUsersController(IKitchenUsersService kitchenUsersService)
        {
            _kitchenUsersService = kitchenUsersService;
        }

        /// <summary>
        /// Retrieves all users/members belonging to this kitchen
        /// </summary>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <response code="200">List of kitchen members</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("{kitchenId}/users")]
        public IActionResult GetKitchenUsers([FromRoute] Guid kitchenId, [FromQuery] Guid? requestedUserId)
        {
            var userId = Authentication.GetUserIdFromHeader(Request);
            if (userId == null)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
            if (!_kitchenUsersService.CheckPermission(userId.Value, kitchenId, KitchenRoles.inspector))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }

            var users = _kitchenUsersService.GetKitchenUsers(kitchenId, requestedUserId.HasValue? requestedUserId.Value:Guid.Empty, Guid.Empty);
            return Ok(users);
        }

        /// <summary>
        /// Adds a user to the kitchen. Required access code, to locate kitchen and grant access as inspector to it
        /// </summary>
        /// <param name="request">User and role assignment data</param>
        /// <response code="200"></response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("users")]
        public IActionResult AddUserToKitchen([FromBody] KitchenUserAddRequest request)
        {
            var requestingUserId = Authentication.GetUserIdFromHeader(Request)
                ?? throw new UnauthorizedAccessException("Authentication required");

            if(request.AccessCode == "")
            {
                throw new ArgumentException("access code cannot be empty");
            }

            var added = _kitchenUsersService.JoinKitchen(request.UserId, request.AccessCode);
            return Ok(added);
        }

        /// <summary>
        /// Updates a member's role in the kitchen. Requires admin or owner role.
        /// </summary>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="userId">Member user ID</param>
        /// <param name="request">New role</param>
        /// <response code="200">Role updated</response>
        /// <response code="401">Unauthorized</response>
        [HttpPatch("{kitchenId}/users/{userId}/role")]
        public IActionResult ChangeUserRole([FromRoute] Guid kitchenId, [FromRoute] Guid userId, [FromBody] KitchenUserRoleUpdateRequest request)
        {
            var requester = Authentication.GetUserIdFromHeader(Request);
            if (requester == null)
            {
                throw new UnauthorizedAccessException("You don't have permission to do this");
            }
            if (!_kitchenUsersService.CheckPermission(requester.Value, kitchenId, KitchenRoles.admin))
            {
                throw new UnauthorizedAccessException("You don't have access to do this");
            }


            var updated = _kitchenUsersService.ChangeRole(userId, kitchenId, request.Role);
            return Ok(updated);
        }

        /// <summary>
        /// Removes a user from the kitchen. Requires admin or owner role, or self-removal.
        /// </summary>
        /// <param name="kitchenId">Kitchen ID</param>
        /// <param name="userId">Member user ID</param>
        /// <response code="200">User removed from kitchen</response>
        /// <response code="401">Unauthorized</response>
        [HttpDelete("{kitchenId}/users/{userId}")]
        public IActionResult RemoveUserFromKitchen([FromRoute] Guid kitchenId, [FromRoute] Guid userId)
        {
            var requestingUserId = Authentication.GetUserIdFromHeader(Request)
                ?? throw new UnauthorizedAccessException("Authentication required");
            if (requestingUserId != userId && !_kitchenUsersService.CheckPermission(userId, kitchenId, KitchenRoles.owner))
            {
                throw new UnauthorizedAccessException("You don't have permission to access this kitchen");
            }

            _kitchenUsersService.RemoveUserFromKitchen(userId, kitchenId);
            return Ok(new DeletionResponse(true, "User removed from kitchen successfully"));
        }
    }
}
