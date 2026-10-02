using Microsoft.AspNetCore.Mvc;

namespace FoodApp.Controllers
{
    /// <summary>
    /// Health-check controller that exposes an endpoint for liveness and readiness probes.
    /// </summary>
    [ApiController]
    [Route("/api")]
    public class Ping : Controller
    {

        /// <summary>
        /// Endpoint to check if the service is alive.
        /// </summary>
        /// <returns>An HTTP 200 OK response confirming the service is running.</returns>
        [HttpGet]
        public IActionResult PingService()
        {
            return Ok();
        }
    }
}
