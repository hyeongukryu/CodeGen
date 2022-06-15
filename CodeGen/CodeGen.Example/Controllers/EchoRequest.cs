using NodaTime;

namespace CodeGen.Example.Controllers;

public record EchoRequest(long A, int B, string C, Instant D);