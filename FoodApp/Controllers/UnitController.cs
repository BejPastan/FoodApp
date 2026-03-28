using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing measurement units. Provides endpoints to create, read, update, and delete
    /// measurement units used for ingredients (e.g., cups, grams, tablespoons). Also provides unit conversion
    /// functionality for recipe scaling and measurement system compatibility.
    /// </summary>
    [ApiController]
    public class UnitController(IUnitService service, IAuthService auth) : Controller
    {
        private readonly IUnitService _service = service;
        private readonly IAuthService _auth = auth;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitController"/> class.
        /// </summary>
        /// <param name="service">The unit service for data operations.</param>
        /// <param name="auth">The authentication service for permission checks.</param>

        /// <summary>
        /// Retrieves a list of units with optional filtering and pagination.
        /// </summary>
        /// <param name="ids">Optional array of unit IDs to filter by. If provided, only units with these IDs are returned.</param>
        /// <param name="search">Optional search text to filter units by name (partial match, case-insensitive).</param>
        /// <param name="page">Page number for pagination (default: 1).</param>
        /// <param name="perPage">Number of items per page (default: 25, max: 100).</param>
        /// <returns>A list of units matching the search criteria.</returns>
        /// <response code="200">Returns the list of matching units.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <remarks>
        /// Requires user or admin authentication. Supports filtering by specific IDs or search text.
        /// If both filters are provided, results must match both criteria.
        /// Units are used to specify measurement quantities for recipe ingredients.
        /// Common units include "cup", "gram", "tablespoon", "teaspoon", etc.
        /// </remarks>
        [HttpGet("api/units")]
        public IActionResult GetUnits([FromQuery] int[] ids, [FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            _auth.CheckPermissions(Request, [Roles.user, Roles.admin]);
            Console.WriteLine(ids.Length);
            return Ok(_service.GetUnits(search, ids, page, perPage));
        }

        /// <summary>
        /// Retrieves a specific unit by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the unit.</param>
        /// <returns>The unit with its details if found; otherwise returns 404 Not Found.</returns>
        /// <response code="200">Returns the requested unit.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - unit with the specified ID does not exist.</response>
        /// <remarks>
        /// Requires user or admin authentication. Returns detailed information about a single unit
        /// including its name, volume equivalent for conversion calculations, and description.
        /// </remarks>
        [HttpGet("api/units/{id}")]
        public IActionResult GetUnit(int id)
        {
            _auth.CheckPermissions(Request, [Roles.user, Roles.admin]);
            var item = _service.GetUnitById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        /// <summary>
        /// Creates a new measurement unit.
        /// </summary>
        /// <param name="request">The unit data to create. Must include name and volumeEquivalent.</param>
        /// <returns>The created unit with its assigned ID.</returns>
        /// <response code="200">Returns the created unit.</response>
        /// <response code="400">Bad Request - invalid unit data or missing required fields.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The unit name must be unique within the system.
        /// The volumeEquivalent is used for conversion calculations between different units.
        /// For example, if 1 cup = 236.588 ml, then volumeEquivalent for cup would be 236.588.
        /// This allows the system to convert between units for recipe scaling and ingredient calculations.
        /// </remarks>
        [HttpPost("api/units")]
        public IActionResult PostUnit([FromBody] UnitCreateRequest request)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateUnit(request);
            return Ok(created);
        }

        /// <summary>
        /// Updates an existing measurement unit.
        /// </summary>
        /// <param name="id">The ID of the unit to update.</param>
        /// <param name="request">The updated unit data. Only provided fields will be updated.</param>
        /// <returns>The updated unit, or 400 Bad Request if no valid fields to update or unit not found.</returns>
        /// <response code="200">Returns the updated unit.</response>
        /// <response code="400">Bad Request - no valid fields provided for update or unit not found.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. The updated name must be unique within the system.
        /// The volumeEquivalent can be updated for conversion accuracy improvements.
        /// Returns 400 if no fields are provided for update.
        /// </remarks>
        [HttpPatch("api/units/{id}")]
        public IActionResult PatchUnit(int id, [FromBody] UnitUpdateRequest request)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            var updated = _service.UpdateUnit(id, request);
            if (updated == null)
            {
                return BadRequest(new { error = "No fields or not found" });
            }
            return Ok(updated);
        }

        /// <summary>
        /// Deletes a measurement unit by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the unit to delete.</param>
        /// <returns>Success status of the deletion operation.</returns>
        /// <response code="200">Returns deletion status (true if successful, false if not found).</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="403">Forbidden - user does not have admin role required for this operation.</response>
        /// <remarks>
        /// Requires admin role authentication. This operation performs a cascading update:
        /// - The unit is permanently removed from the database
        /// - All ingredients using this unit will have their unit_id set to null
        /// - This is a destructive operation that affects recipe ingredients
        /// Consider updating the unit instead of deleting it to preserve recipe integrity.
        /// </remarks>
        [HttpDelete("api/units/{id}")]
        public IActionResult DeleteUnit(int id)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteUnit(id);
            return Ok(new { deleted = true });
        }

        /// <summary>
        /// Converts an amount from one unit to another using volume equivalents.
        /// </summary>
        /// <param name="oldId">The ID of the unit to convert from.</param>
        /// <param name="newId">The ID of the unit to convert to.</param>
        /// <param name="originalAmount">The amount in the original unit to convert.</param>
        /// <returns>The converted amount in the new unit, along with the unit details.</returns>
        /// <response code="200">Returns the conversion result with new unit and amount.</response>
        /// <response code="401">Unauthorized - valid authentication token required.</response>
        /// <response code="404">Not Found - one or both unit IDs do not exist.</response>
        /// <remarks>
        /// Requires user or admin authentication. This endpoint performs unit conversion based on
        /// the volume equivalent values stored for each unit. The conversion uses the formula:
        /// newAmount = (originalAmount * oldUnit.volumeEquivalent) / newUnit.volumeEquivalent
        /// This is useful for recipe scaling, measurement system conversion, and ingredient calculations.
        /// </remarks>
        [HttpGet("api/units/convert")]
        public IActionResult ConvertUnit([FromQuery] string oldId, [FromQuery] string newId, [FromQuery] float originalAmount)
        {
            _auth.CheckPermissions(Request, [Roles.user, Roles.admin]);

            int oldUnitId = int.Parse(oldId);
            int newUnitId = int.Parse(newId);

            return Ok(_service.ConvertUnit(oldUnitId, newUnitId, originalAmount));
        }
    }
}
