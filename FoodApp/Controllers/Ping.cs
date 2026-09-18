using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    [ApiController]
    [Route("/api")]
    public class Ping : Controller
    {

        /// <summary>
        /// Endpioint to check if service is alive
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PingService()
        {
            return Ok();
        }
    }
}
