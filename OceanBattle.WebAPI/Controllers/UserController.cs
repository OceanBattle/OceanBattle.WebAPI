using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace OceanBattle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public async Task<IActionResult> PostRegister()
        {

            return Ok();
        }
    }
}
