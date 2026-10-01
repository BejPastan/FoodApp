using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Services.Interfaces;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing kitchens (CRUD)
    /// </summary>
    [ApiController]
    [Route("api/kitchen")]
    public class KitchenController : ControllerBase
    {
        private readonly IKitchenService _kitchenService;
        private readonly ILogger<KitchenController> _logger;
        private readonly IKitchenUsersService _kitchenUserService;

        /// <summary>
        /// Initializes a new instance of KitchenController
        /// </summary>
        public KitchenController(IKitchenService kitchenService, ILogger<KitchenController> logger, IKitchenUsersService kitchenUsersService)
        {
            _kitchenService = kitchenService;
            _logger = logger;
            _kitchenUserService = kitchenUsersService;
        }

        /// <summary>
        /// Return kitchens accessible by this user
        /// </summary>
        /// <param name="name">name filter for searching kitchens</param>
        /// <returns></returns>
        /// <exception cref="UnauthorizedAccessException"></exception>

        [HttpGet]
        public IActionResult GetUserKitchens([FromQuery] string name="")
        {
            var userId = Authentication.GetUserIdFromHeader(Request)
                ?? throw new UnauthorizedAccessException("Authentication required");

            var kitchens = _kitchenService.GetKitchensForUser(userId, name);
            return Ok(kitchens);
        }

        /// <summary>
        /// Retrieves details of a specific kitchen by ID
        /// </summary>
        /// <param name="id">Kitchen unique identifier</param>
        /// <response code="200">Returns the kitchen</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Kitchen not found</response>
        [HttpGet("{id}")]
        public IActionResult GetKitchenById([FromRoute] Guid id)
        {
            var userId = Authentication.GetUserIdFromHeader(Request)
                ?? throw new UnauthorizedAccessException("Authentication required");

            if (!_kitchenUserService.CheckPermission(userId, id, KitchenRoles.inspector))
            {
                throw new UnauthorizedAccessException("You don't have permission to access this kitchen");
            }

            var kitchen = _kitchenService.GetKitchenById(id);
            if (kitchen == null)
            {
                return NotFound(new ErrorResponse("Kitchen not found"));
            }

            return Ok(kitchen);
        }

        /// <summary>
        /// Creates a new kitchen for the authenticated user
        /// </summary>
        /// <param name="request">Kitchen creation data</param>
        /// <response code="200">Kitchen created successfully</response>
        /// <response code="400">Invalid kitchen data</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost]
        public IActionResult CreateKitchen([FromBody] KitchenCreateRequest request)
        {
            var userId = Authentication.GetUserIdFromHeader(Request)
                ?? throw new UnauthorizedAccessException("Authentication required");

            var created = _kitchenService.CreateKitchen(request, userId);
            return Ok(created);
        }

        /// <summary>
        /// Updates an existing kitchen name. Requires admin or owner role on the kitchen.
        /// </summary>
        /// <param name="id">Kitchen unique identifier</param>
        /// <param name="request">Update data</param>
        /// <response code="200">Kitchen updated successfully</response>
        /// <response code="400">Invalid update data</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Kitchen not found</response>
        [HttpPatch("{id}")]
        public IActionResult UpdateKitchen([FromRoute] Guid id, [FromBody] KitchenUpdateRequest request)
        {
            var userId = Authentication.GetUserIdFromHeader(Request)
                ?? throw new UnauthorizedAccessException("Authentication required");

            if (!_kitchenUserService.CheckPermission(userId, id, KitchenRoles.admin))
            {
                throw new UnauthorizedAccessException("You don't have permission to access this kitchen");
            }

            var updated = _kitchenService.UpdateKitchen(id, request);
            if (updated == null)
            {
                return NotFound(new ErrorResponse("Kitchen not found"));
            }

            return Ok(updated);
        }

        /// <summary>
        /// Deletes a kitchen. Only the kitchen owner can delete it.
        /// </summary>
        /// <param name="id">Kitchen unique identifier</param>
        /// <response code="200">Kitchen deleted successfully</response>
        /// <response code="401">Unauthorized or not an owner</response>
        [HttpDelete("{id}")]
        public IActionResult DeleteKitchen([FromRoute] Guid id)
        {
            var userId = Authentication.GetUserIdFromHeader(Request)
                ?? throw new UnauthorizedAccessException("Authentication required");

            if (!_kitchenUserService.CheckPermission(userId, id, KitchenRoles.owner))
            {
                throw new UnauthorizedAccessException("You don't have permission to access this kitchen");
            }

            _kitchenService.DeleteKitchen(id);
            return Ok(new DeletionResponse(true, "Kitchen deleted successfully"));
        }
    }
}
