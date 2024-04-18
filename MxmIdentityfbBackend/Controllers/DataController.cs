using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MxmIdentityfbBackend.Controllers;

[ApiController]
[Route("api/data")]
[Authorize(AuthenticationSchemes = "Bearer")]

public class DataController : ControllerBase
{
    
    [HttpGet]
    public async Task<IActionResult> GetData()
    {
        var accountIdClaim = User.FindFirst("Id");
        if(accountIdClaim == null)
            return Unauthorized("Usuário não autenticado");
        var id = accountIdClaim.Value;

        return Ok(id);
    }
    
}
