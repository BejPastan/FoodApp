using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class StepController : Controller
    {
        private readonly IStepService _service;
        private readonly IAuthService _auth;
        
        public StepController(IStepService service, IAuthService auth) 
        {
            _service = service;
            _auth = auth;
        }

        [HttpGet("api/steps")]
        public IActionResult GetSteps([FromQuery] int? recipeId = null, [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            try 
            { 
                return Ok(_service.GetSteps(recipeId, page, perPage)); 
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
        public IActionResult PostStep([FromBody] Step request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try { var created = _service.CreateStep(request); return Ok(created); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPatch("api/steps/{id}")]
        public IActionResult PatchStep(int id, [FromBody] Step stepRequest, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

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
        public IActionResult DeleteStep(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try { _service.DeleteStep(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}
