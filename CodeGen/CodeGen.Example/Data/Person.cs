using NodaTime;

namespace CodeGen.Example.Data;

public record Person(long Id, string Name, Instant Registered, Department Department);