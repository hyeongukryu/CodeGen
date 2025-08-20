using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NodaTime;

namespace CodeGen.Tests.Generation.TestControllers.CollisionA
{
    [ApiController]
    [Route("collision-a")]
    public class CollisionController : ControllerBase
    {
        [HttpGet]
        public ActionResult<DuplicateDto> Get()
        {
            return Ok(new DuplicateDto("a"));
        }
    }

    public record DuplicateDto(string Value);
}

namespace CodeGen.Tests.Generation.TestControllers.CollisionB
{
    [ApiController]
    [Route("collision-b")]
    public class CollisionController : ControllerBase
    {
        [HttpGet]
        public ActionResult<DuplicateDto> Get()
        {
            return Ok(new DuplicateDto(1));
        }
    }

    public record DuplicateDto(int Value);
}

namespace CodeGen.Tests.Generation.TestControllers.Invalid
{
    [ApiController]
    [Route("invalid-definition")]
    public class InvalidDefinitionController : ControllerBase
    {
        [HttpGet("invalid-response")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status202Accepted)]
        public ActionResult<string> InvalidResponse()
        {
            return Ok("invalid");
        }
    }
}

namespace CodeGen.Tests.Generation.TestControllers.PrefixA.Shared
{
    [ApiController]
    [Route("prefix-a-shared-duplicate")]
    public class DuplicateController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "a";
        }
    }
}

namespace CodeGen.Tests.Generation.TestControllers.PrefixB.Shared
{
    [ApiController]
    [Route("prefix-b-shared-duplicate")]
    public class DuplicateController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "b";
        }
    }
}

namespace CodeGen.Tests.Generation.TestControllers.Mangle
{
    public static class First
    {
        [ApiController]
        [Route("mangle/first")]
        public class DuplicateController : ControllerBase
        {
            [HttpGet]
            public DuplicateDto Get()
            {
                return new DuplicateDto("first");
            }
        }

        public record DuplicateDto(string Value);
    }

    public static class Second
    {
        [ApiController]
        [Route("mangle/second")]
        public class DuplicateController : ControllerBase
        {
            [HttpGet]
            public DuplicateDto Get()
            {
                return new DuplicateDto(2);
            }
        }

        public record DuplicateDto(int Value);
    }
}

namespace CodeGen.Tests.Generation.TestControllers.BuiltIns
{
    [ApiController]
    [Route("built-ins")]
    public class BuiltInController : ControllerBase
    {
        [HttpPost("echo")]
        public ActionResult<BuiltInDto> Echo([FromBody] BuiltInDto request)
        {
            return Ok(request);
        }
    }

    public class BuiltInDto
    {
        public required string StringValue { get; init; }
        public bool BooleanValue { get; init; }
        public byte ByteValue { get; init; }
        public sbyte SignedByteValue { get; init; }
        public short Int16Value { get; init; }
        public int Int32Value { get; init; }
        public long Int64Value { get; init; }
        public ushort UnsignedInt16Value { get; init; }
        public uint UnsignedInt32Value { get; init; }
        public ulong UnsignedInt64Value { get; init; }
        public float SingleValue { get; init; }
        public double DoubleValue { get; init; }
        public decimal DecimalValue { get; init; }
        public DateTime DateTimeValue { get; init; }
        public DateTimeOffset DateTimeOffsetValue { get; init; }
        public DateOnly DateOnlyValue { get; init; }
        public TimeOnly TimeOnlyValue { get; init; }
        public Guid GuidValue { get; init; }
        public required Uri UriValue { get; init; }
        public Instant InstantValue { get; init; }
        public LocalDate LocalDateValue { get; init; }
        public LocalTime LocalTimeValue { get; init; }
        public LocalDateTime LocalDateTimeValue { get; init; }
        public required byte[] Bytes { get; init; }
    }
}
