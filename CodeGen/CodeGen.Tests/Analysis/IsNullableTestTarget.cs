namespace CodeGen.Tests.Analysis;

public record IsNullableTestTarget(
    string A, string? B,
    int C, int? D,
    IEnumerable<string> E, IEnumerable<string>? F,
    IsNullableTestTarget G, IsNullableTestTarget? H);