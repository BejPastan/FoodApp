using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing kitchen roles
    /// </summary>
    [ApiController]
    [Route("api/kitchen-role")]
    public class KitchenRoleController : ControllerBase
    {
        private readonly IKitchenRoleService _roleService;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of KitchenRoleController
        /// </summary>
        public KitchenRoleController(IKitchenRoleService roleService, IAuthService authService)
        {
            _roleService = roleService;
            _authService = authService;
        }

        /// <summary>
        /// Retrieves all available kitchen roles
        /// </summary>
        /// <response code="200">List of kitchen roles</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            Authentication.ValidateToken(Request);
            var roles = _roleService.GetAllRoles();
            return Ok(roles);
        }

        /// <summary>
        /// Retrieves a kitchen role by ID
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <response code="200">The requested role</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Role not found</response>
        [HttpGet("{id}")]
        public IActionResult GetRoleById([FromRoute] Guid id)
        {
            Authentication.ValidateToken(Request);
            var role = _roleService.GetRoleById(id);
            if (role == null)
            {
                return NotFound(new ErrorResponse("Kitchen role not found"));
            }
            return Ok(role);
        }

        /// <summary>
        /// Creates a new kitchen role (requires system admin)
        /// </summary>
        /// <param name="request">Role creation data</param>
        /// <response code="200">Role created</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - system admin required</response>
        [HttpPost]
        public IActionResult CreateRole([FromBody] KitchenRoleCreateRequest request)
        {
            _authService.CheckPermissions(Request, [Roles.admin]);
            var created = _roleService.CreateRole(request);
            return Ok(created);
        }

        /// <summary>
        /// Updates a kitchen role (requires system admin)
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <param name="request">Update data</param>
        /// <response code="200">Role updated</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - system admin required</response>
        /// <response code="404">Role not found</response>
        [HttpPatch("{id}")]
        public IActionResult UpdateRole([FromRoute] Guid id, [FromBody] KitchenRoleUpdateRequest request)
        {
            _authService.CheckPermissions(Request, [Roles.admin]);
            var updated = _roleService.UpdateRole(id, request);
            if (updated == null)
            {
                return NotFound(new ErrorResponse("Kitchen role not found"));
            }
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a kitchen role (requires system admin)
        /// </summary>
        /// <param name="id">Role unique identifier</param>
        /// <response code="200">Role deleted</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden - system admin required</response>
        [HttpDelete("{id}")]
        public IActionResult DeleteRole([FromRoute] Guid id)
        {
            _authService.CheckPermissions(Request, [Roles.admin]);
            _roleService.DeleteRole(id);
            return Ok(new DeletionResponse(true, "Kitchen role deleted successfully"));
        }
    }
}
