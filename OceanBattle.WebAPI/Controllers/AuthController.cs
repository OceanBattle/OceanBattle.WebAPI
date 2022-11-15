using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OceanBattle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }
    }
}
