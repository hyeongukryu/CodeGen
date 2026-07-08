using NodaTime;

namespace CodeGen.Example.Controllers;

public class BuiltInsDto
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
