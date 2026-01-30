using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class StepController : Controller
    {
        private readonly IStepService _service;
        public StepController(IStepService service) { _service = service; }

        [HttpGet("api/steps")]
        public IActionResult GetStepsAPI([FromQuery] int? recipeId = null)
        {
            try 
            { 
                return Ok(_service.GetSteps(recipeId)); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpGet("api/steps/{id}")]
        public IActionResult GetStep(int id)
        {
            try { var item = _service.GetStepById(id); if (item == null) return NotFound(); return Ok(item); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPost("api/steps")]
        public IActionResult PostStep([FromBody] Step request)
        {
            try { var created = _service.CreateStep(request); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPatch("api/steps/{id}")]
        public IActionResult PatchStep(int id, [FromBody] Step stepRequest)
        {
            try 
            { 
                var updated = _service.UpdateStep(stepRequest); 
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                } 
                return Ok(updated); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/steps/{id}")]
        public IActionResult DeleteStep(int id)
        {
            try { _service.DeleteStep(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}
