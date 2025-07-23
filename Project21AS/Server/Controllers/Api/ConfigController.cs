using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Project21AS.Server.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfigController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ConfigController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("fingerprint-sensor")]
        [AllowAnonymous]
        public ActionResult<string> GetFingerprintSensor()
        {
            return _configuration.GetValue<string>("FingerprintSensor") ?? "MFS100";
        }
    }
}
