using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// API group for managing food items, including searching, retrieving, creating, updating, and deleting food records.
    /// </summary>
    [ApiController]
    public class FoodController : Controller
    {
        private readonly IFoodService _service;
        private readonly IAuthService _auth;
        public FoodController(IFoodService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/food")]
        public IActionResult SearchFood(
            [FromQuery] int? typeId,
            [FromQuery] string name = "",
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10
        )
        {
                Authentication.ValidateToken(Request);
                var foods = _service.GetFoods(typeId, name, page, perPage);
                return Ok(foods);
        }

        /// <summary>
        /// Return food record based on given id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("api/food/{id}")]
        public IActionResult GetFood(int id)
        {
                Authentication.ValidateToken(Request);
                var food = _service.GetFoodById(id);
                if (food == null) return NotFound();
                return Ok(food);
        }

        /// <summary>
        /// Crete new food record. Required admin role
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost("api/food")]
        public IActionResult PostFood([FromBody] Food request)
        {
                var userId = _auth.CheckPermissions(Request,  [Roles.admin]);
                if(request.foodTypeId ==0 && (request.foodType == null || string.IsNullOrEmpty(request.foodType.name)))
                {
                    throw new Exception("food type id or food type name required");
                }
                var created = _service.CreateFood(request);
                return Ok(created);
        }

        /// <summary>
        /// Update food record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="food"></param>
        /// <returns></returns>
        [HttpPatch("api/food/{id}")]
        public IActionResult PatchFood(
            int id,
            [FromBody] Food food
        )
        {
                var userId = _auth.CheckPermissions(Request,  [Roles.admin]);
                food.id = id;
                var updated = _service.UpdateFood(food);
                if (updated == null) return BadRequest(new { error = "No fields or not found" });
                return Ok(updated);
        }

        [HttpDelete("api/food/{id}")]
        public IActionResult DeleteFood(int id)
        {
                var userId = _auth.CheckPermissions(Request,  [Roles.admin]);
                var ok = _service.DeleteFood(id);
                return Ok(new { deleted = ok });
        }
    }
}
