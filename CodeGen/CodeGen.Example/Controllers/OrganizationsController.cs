using System.Text;
using CodeGen.Analysis;
using CodeGen.Example.Data;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace CodeGen.Example.Controllers;

[ApiController]
[Route("organizations")]
public class OrganizationsController : ControllerBase
{
    [HttpGet("empty")]
    [Command]
    public ActionResult ReturnsEmpty()
    {
        return Ok();
    }


    [HttpGet]
    public ActionResult<IEnumerable<Department>> GetAll()
    {
        var departments = new List<Department>
        {
            new()
            {
                Id = 100,
                Name = "부서 이름"
            },
            new()
            {
                Id = 200,
                Name = null
            }
        };
        departments[0].People = new Person[]
        {
            new(1, "a",
                Instant.FromUtc(2020, 1, 2, 3, 4), departments[0]),
            new(2, "b",
                Instant.FromUtc(2021, 1, 2, 3, 4), departments[0])
        };
        return Ok(departments);
    }

    [HttpPost]
    public ActionResult<EchoResponse> Echo([FromBody] EchoRequest request)
    {
        var builder = new StringBuilder();
        builder.AppendLine(request.A.ToString());
        builder.AppendLine(request.B.ToString());
        builder.AppendLine(request.C);
        builder.AppendLine(request.D.ToUnixTimeMilliseconds().ToString());
        return Ok(new EchoResponse(builder.ToString()));
    }
}