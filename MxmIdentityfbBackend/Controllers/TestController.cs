using Microsoft.AspNetCore.Mvc;

namespace MxmIdentityfbBackend.Controllers;

[ApiController]
[Route("api/test")]
public class TestController : ControllerBase
{


        [HttpGet]
        public async Task<IActionResult> GetTest()
        {

            return Ok("deu certo");
        }

}
