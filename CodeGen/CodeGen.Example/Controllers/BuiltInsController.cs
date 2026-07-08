using Microsoft.AspNetCore.Mvc;

namespace CodeGen.Example.Controllers;

[ApiController]
[Route("built-ins")]
public class BuiltInsController : ControllerBase
{
    [HttpPost("echo")]
    public ActionResult<BuiltInsDto> Echo([FromBody] BuiltInsDto request)
    {
        return Ok(request);
    }
}
