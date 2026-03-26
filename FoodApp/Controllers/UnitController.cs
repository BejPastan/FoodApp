using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
            _auth.CheckPermissions(Request, [Roles.user, Roles.admin]);
            return Ok(_service.GetUnits(name, page, perPage));
        }

        [HttpGet("api/units/{id}")]
        public IActionResult GetUnit(int id)
        {
            _auth.CheckPermissions(Request, [Roles.user, Roles.admin]);
            var item = _service.GetUnitById(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost("api/units")]
        public IActionResult PostUnit([FromBody] Unit request)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateUnit(request);
            return Ok(created);
        }

        [HttpPatch("api/units/{id}")]
        public IActionResult PatchUnit(int id, [FromBody] Unit request)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            var updated = _service.UpdateUnit(request);
            if (updated == null)
            {
                return BadRequest(new { error = "No fields or not found" });
            }
            return Ok(updated);
        }

        [HttpDelete("api/units/{id}")]
        public IActionResult DeleteUnit(int id)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            _service.DeleteUnit(id);
            return Ok(new { deleted = true });
        }

        /// <summary>
        /// make conversion between 2 units, and return new unit and it's amount
        /// </summary>
        /// <param name="oldId"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        [HttpGet("api/units/convert")]
        public IActionResult ConvertUnit([FromQuery] string oldId, [FromQuery] string newId, [FromQuery] float originalAmount)
        {
            _auth.CheckPermissions(Request, [Roles.user, Roles.admin]);

            int oldUnitId = int.Parse(oldId);
            int newUnitId = int.Parse(newId);

            return Ok(_service.ConvertUnit(oldUnitId, newUnitId, originalAmount));
        }
    }
}
