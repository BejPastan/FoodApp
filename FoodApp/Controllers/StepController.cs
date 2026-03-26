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
        public StepController(IStepService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/steps")]
        public IActionResult GetSteps([FromQuery] int? recipeId = null, [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetSteps(recipeId, page, perPage));
        }

        [HttpGet("api/steps/{id}")]
        public IActionResult GetStep(int id)
        {
            Authentication.ValidateToken(Request);
            var item = _service.GetStepById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("api/steps")]
        public IActionResult PostStep([FromBody] Step request)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateStep(request);
            return Ok(created);
        }

        [HttpPatch("api/steps/{id}")]
        public IActionResult PatchStep(int id, [FromBody] Step stepRequest)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var updated = _service.UpdateStep(stepRequest);
            if (updated == null)
            {
                return BadRequest(new { error = "No fields or not found" });
            }
            return Ok(updated);
        }

        [HttpDelete("api/steps/{id}")]
        public IActionResult DeleteStep(int id)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteStep(id);
            return Ok(new { deleted = true });
        }
    }
}
