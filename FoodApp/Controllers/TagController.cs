using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodApp.Controllers
{
    [ApiController]
    public class TagController : Controller
    {
        private readonly ITagService _service;
        private readonly IAuthService _auth;
        public TagController(ITagService service, IAuthService auth) 
        {
            _service = service;
            _auth = auth;
        }

        [HttpGet("api/tags")]
        public IActionResult GetTags([FromQuery] string name = "")
        {
            try { return Ok(_service.GetTags(name)); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpGet("api/tags/{id}")]
        public IActionResult GetTag(int id)
        {
            try { var item = _service.GetTagById(id); if (item == null) return NotFound(); return Ok(item); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPost("api/tags")]
        public IActionResult PostTag([FromBody] Tag tagRequest, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try 
            {
                var created = _service.CreateTag(tagRequest); 
                return Ok(created); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpPatch("api/tags/{id}")]
        public IActionResult PatchTag(int id, [FromBody] Tag request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);


            try 
            { 
                var updated = _service.UpdateTag(request);
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                }
                return Ok(updated); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpDelete("api/tags/{id}")]
        public IActionResult DeleteTag(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);
            try { _service.DeleteTag(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}
