using Microsoft.AspNetCore.Mvc;
using FoodApp.Services;
using FoodApp.Models;

namespace FoodApp.Controllers
{
    [ApiController]
    public class TagController : Controller
    {
        private readonly ITagService _service;
        public TagController(ITagService service) { _service = service; }

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
        public IActionResult PostTag([FromBody] Tag tagRequest)
        {
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
        public IActionResult PatchTag(int id, [FromBody] Tag request)
        {
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
        public IActionResult DeleteTag(int id)
        {
            try { _service.DeleteTag(id); return Ok(new { deleted = true }); }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }
    }
}
