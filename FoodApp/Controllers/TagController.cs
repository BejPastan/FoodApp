using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class TagController : Controller
    {
        private readonly ITagService _service;
        private readonly IAuthService _auth;
        public TagController(ITagService service, IAuthService authService)
        {
            _service = service;
            _auth = authService;
        }

        [HttpGet("api/tags")]
        public IActionResult GetTags([FromQuery] string name = "")
        {
            Authentication.ValidateToken(Request);
            return Ok(_service.GetTags(name));
        }

        [HttpGet("api/tags/{id}")]
        public IActionResult GetTag(int id)
        {
            Authentication.ValidateToken(Request);
            var item = _service.GetTagById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("api/tags")]
        public IActionResult PostTag([FromBody] Tag tagRequest)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateTag(tagRequest);
            return Ok(created);
        }

        [HttpPatch("api/tags/{id}")]
        public IActionResult PatchTag(int id, [FromBody] Tag request, [FromHeader(Name = "Authorization")] string authorization)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            var updated = _service.UpdateTag(request);
            if (updated == null)
            {
                return BadRequest(new { error = "No fields or not found" });
            }
            return Ok(updated);
        }

        [HttpDelete("api/tags/{id}")]
        public IActionResult DeleteTag(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            var userId = _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteTag(id);
            return Ok(new { deleted = true });
        }
    }
}
