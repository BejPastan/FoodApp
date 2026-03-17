using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodApp.Controllers
{
    [ApiController]
    public class FoodTypeController : Controller
    {
        private readonly IFoodTypeService _service;
        private readonly IAuthService _auth;

        public FoodTypeController(IFoodTypeService service, IAuthService auth)
        {
            _service = service;
            _auth = auth;
        }

        [HttpGet("api/food_type")]
        public IActionResult GetFoodTypes([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            try
            {
                var foodTypes = _service.GetFoodTypes(name, page, perPage);
                return Ok(foodTypes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("api/food_type/{id}")]
        public IActionResult GetFoodType(int id)
        {
            try
            {
                var foodType = _service.GetFoodTypeById(id);
                if (foodType == null) return NotFound();
                return Ok(foodType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("api/food_type")]
        public IActionResult PostFoodType([FromBody] FoodType request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try
            {
                if (string.IsNullOrWhiteSpace(request.name))
                {
                    return BadRequest(new { error = "Name is required." });
                }
                var created = _service.CreateFoodType(request);
                return Ok(created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        

        [HttpPatch("api/food_type/{id}")]
        public IActionResult PatchFoodType(int id, [FromBody] FoodType request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try
            {
                if (request.name == null) return BadRequest(new { error = "No fields provided to update." });
                var updated = _service.UpdateFoodType(request);
                if (updated == null) return NotFound();
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("api/food_type/{id}")]
        public IActionResult DeleteFoodType(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try
            {
                var ok = _service.DeleteFoodType(id);
                return Ok(new { success = ok });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
