using Microsoft.AspNetCore.Mvc;

namespace GameOfLife.Controllers
{
    /// <summary>
    /// Endpoint for System.
    /// This is useful to check Api health state or get some Api info
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/system")]
    [ApiVersion("1.0")]
    public class SystemController : Controller
    {
        /// <summary>
        /// Get an ACK string response to know if API is alive.
        /// </summary>
        /// <returns>A 'Pong' string message</returns>
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Pong");
        }
    }
}
