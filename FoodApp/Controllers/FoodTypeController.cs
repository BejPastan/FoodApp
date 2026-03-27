using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API controller for managing food types. Provides endpoints to create, read, update, and delete food types.
    /// </summary>
    [ApiController]
    public class FoodTypeController(IFoodTypeService service, IAuthService authService) : Controller
    {
        private readonly IFoodTypeService _service = service;
        private readonly IAuthService _auth = authService;

        [HttpGet("api/food_type")]
        public IActionResult GetFoodTypes([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
                Authentication.ValidateToken(Request);
                var foodTypes = _service.GetFoodTypes(name, page, perPage);
                return Ok(foodTypes);
        }

        [HttpGet("api/food_type/{id}")]
        public IActionResult GetFoodType(int id)
        {
                Authentication.ValidateToken(Request);
                var foodType = _service.GetFoodTypeById(id);
                if (foodType == null) return NotFound();
                return Ok(foodType);
        }

        [HttpPost("api/food_type")]
        public IActionResult PostFoodType([FromBody] FoodType request)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                if (string.IsNullOrWhiteSpace(request.name))
                {
                    return BadRequest(new { error = "Name is required." });
                }
                var created = _service.CreateFoodType(request);
                return Ok(created);
        }
        

        [HttpPatch("api/food_type/{id}")]
        public IActionResult PatchFoodType(int id, [FromBody] FoodTypeUpdateRequest request)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                if (request.name == null) return BadRequest(new { error = "No fields provided to update." });
                var updated = _service.UpdateFoodType(id, request);
                if (updated == null) return NotFound();
                return Ok(updated);
        }

        /// <summary>
        /// Remove food type from database, if there are foods with this type, they will be set to have no type (type_id = null)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("api/food_type/{id}")]
        public IActionResult DeleteFoodType(int id)
        {
                _auth.CheckPermissions(Request, [Roles.admin]);
                var ok = _service.DeleteFoodType(id);
                return Ok(new { success = ok });
        }
    }
}
