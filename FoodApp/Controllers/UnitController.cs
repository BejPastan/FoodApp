using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    public class UnitController : Controller
    {
        private readonly IUnitService _service;
        private readonly IAuthService _auth;
        public UnitController(IUnitService service, IAuthService auth) 
        {
            _service = service;
            _auth = auth;
        }


        [HttpGet("api/units")]
        public IActionResult GetUnits([FromQuery] string name = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            try 
            { 
                return Ok(_service.GetUnits(name, page, perPage)); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpGet("api/units/{id}")]
        public IActionResult GetUnit(int id)
        {
            try { 
                var item = _service.GetUnitById(id);
                if (item == null)
                {
                    return NotFound();
                }
                return Ok(item); 
            }
            catch (Exception ex) { return StatusCode(500, new { error = ex.Message }); }
        }

        [HttpPost("api/units")]
        public IActionResult PostUnit([FromBody] Unit request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try 
            { 
                var created = _service.CreateUnit(request);
                return Ok(created); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpPatch("api/units/{id}")]
        public IActionResult PatchUnit(int id, [FromBody] Unit request, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try 
            { 
                var updated = _service.UpdateUnit(request); 
                if (updated == null)
                {
                    return BadRequest(new { error = "No fields or not found" });
                } 
                return Ok(updated);
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message }); 
            }
        }

        [HttpDelete("api/units/{id}")]
        public IActionResult DeleteUnit(int id, [FromHeader(Name = "Authorization")] string authorization)
        {
            _auth.CheckPermissions(authorization, [Roles.admin]);

            try 
            {
                _service.DeleteUnit(id); 
                return Ok(new { deleted = true }); 
            }
            catch (Exception ex) 
            { 
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
