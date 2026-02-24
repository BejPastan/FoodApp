using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class FoodController : Controller
    {
        private readonly IFoodService _service;
        public FoodController(IFoodService service)
        {
            _service = service;
        }

        [HttpGet("api/food")]
        public IActionResult SearchFood(
            [FromQuery] int? typeId,
            [FromQuery] string name = "",
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10
        )
        {
            try
            {
                var foods = _service.GetFoods(typeId, name, page, perPage);
                return Ok(foods);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("api/food/{id}")]
        public IActionResult GetFood(int id)
        {
            try
            {
                var food = _service.GetFoodById(id);
                if (food == null) return NotFound();
                return Ok(food);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPost("api/food")]
        public IActionResult PostFood([FromBody] Food request)
        {
            Console.WriteLine(request);
            try
            {
                if(request.foodTypeId ==0 && (request.foodType == null || string.IsNullOrEmpty(request.foodType.name)))
                {
                    throw new Exception("food type id or food type name required");
                }
                var created = _service.CreateFood(request);
                return Ok(created);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }


        [HttpPatch("api/food/{id}")]
        public IActionResult PatchFood(
            int id,
            [FromBody] Food food
        )
        {
            try
            {
                food.id = id;
                var updated = _service.UpdateFood(food);
                if (updated == null) return BadRequest(new { error = "No fields or not found" });
                return Ok(updated);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpDelete("api/food/{id}")]
        public IActionResult DeleteFood(int id)
        {
            try
            {
                var ok = _service.DeleteFood(id);
                return Ok(new { deleted = ok });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
