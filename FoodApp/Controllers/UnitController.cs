using FoodApp.Models;
using FoodApp.Services;
using FoodApp.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FoodApp.Controllers
{
    [ApiController]
    public class UnitController(IUnitService service, IAuthService auth) : Controller
    {
        private readonly IUnitService _service = service;
        private readonly IAuthService _auth = auth;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ids">ids filter, if given will return only Units withthis Ids</param>
        /// <param name="search">search filter</param>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <returns>return unit objects</returns>
        [HttpGet("api/units")]
        public IActionResult GetUnits([FromQuery] int[] ids, [FromQuery] string search = "", [FromQuery] int page = 1, [FromQuery] int perPage = 25)
        {
            _auth.CheckPermissions(Request, [Roles.user, Roles.admin]);
            Console.WriteLine(ids.Length);
            return Ok(_service.GetUnits(search, ids, page, perPage));
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
        public IActionResult PostUnit([FromBody] UnitCreateRequest request)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            var created = _service.CreateUnit(request);
            return Ok(created);
        }

        [HttpPatch("api/units/{id}")]
        public IActionResult PatchUnit(int id, [FromBody] UnitUpdateRequest request)
        {
            _auth.CheckPermissions(Request, [Roles.admin]);
            var updated = _service.UpdateUnit(id, request);
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
