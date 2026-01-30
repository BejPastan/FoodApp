using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class MealController : Controller
    {
        private readonly IMealService _service;
        public MealController(IMealService service) { _service = service; }

        [HttpGet("api/meals")]
        public IActionResult GetMeals([FromQuery] string name="")
        {
            Console.WriteLine("Get call");
            try
            {
                var response = _service.GetMeals(name, null);
                Console.WriteLine(response.Length);
                return Ok(response);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpGet("api/meals/{id}")]
        public IActionResult GetMeal(int id)
        {
            try 
            { 
                var item = _service.GetMealById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpPost("api/meals")]
        public IActionResult PostMeal([FromBody] Meal mealRequest)
        {
            try { var created = _service.CreateMeal(mealRequest); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPatch("api/meals/{id}")]
        public IActionResult PatchMeal(int id, [FromQuery] string? name = null)
        {
            try { var updated = _service.UpdateMeal(id, name); if (updated == null) return BadRequest(new { error = "No fields or not found" }); return Ok(updated); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/meals/{id}")]
        public IActionResult DeleteMeal(int id)
        {
            try { _service.DeleteMeal(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}
