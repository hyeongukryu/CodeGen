using CodeGen.Analysis;
using NodaTime;

namespace CodeGen.Tests.Analysis;

public class AnalysisHelperTest
{
    [Fact]
    public void IsNullablePropertyInfo()
    {
        Assert.False(typeof(IsNullableTestTarget).GetProperty("A")!.IsNullable());
        Assert.True(typeof(IsNullableTestTarget).GetProperty("B")!.IsNullable());
        Assert.False(typeof(IsNullableTestTarget).GetProperty("C")!.IsNullable());
        Assert.True(typeof(IsNullableTestTarget).GetProperty("D")!.IsNullable());
        Assert.False(typeof(IsNullableTestTarget).GetProperty("E")!.IsNullable());
        Assert.True(typeof(IsNullableTestTarget).GetProperty("F")!.IsNullable());
        Assert.False(typeof(IsNullableTestTarget).GetProperty("G")!.IsNullable());
        Assert.True(typeof(IsNullableTestTarget).GetProperty("H")!.IsNullable());
    }

    [Fact]
    public void IsEnumerable()
    {
        Assert.False(typeof(int).IsEnumerable());
        Assert.True(typeof(IEnumerable<int>).IsEnumerable());
        Assert.True(typeof(IList<int>).IsEnumerable());
        Assert.True(typeof(List<int>).IsEnumerable());
        Assert.True(typeof(int[]).IsEnumerable());
        Assert.True(typeof(IQueryable<int>).IsEnumerable());

        Assert.False(typeof(string).IsEnumerable());
        Assert.True(typeof(IEnumerable<string>).IsEnumerable());
        Assert.True(typeof(IEnumerable<string?>).IsEnumerable());
        Assert.False(typeof(Instant).IsEnumerable());
        Assert.True(typeof(IEnumerable<Instant>).IsEnumerable());
        Assert.False(typeof(Instant?).IsEnumerable());
        Assert.True(typeof(IEnumerable<Instant?>).IsEnumerable());
        Assert.False(typeof(IsNullableTestTarget).IsEnumerable());
        Assert.True(typeof(IEnumerable<IsNullableTestTarget>).IsEnumerable());
        Assert.True(typeof(IEnumerable<IsNullableTestTarget?>).IsEnumerable());
    }

    [Fact]
    public void GetEnumerableElementType()
    {
        Assert.Equal(typeof(int), typeof(IEnumerable<int>).GetEnumerableElementType());
        Assert.Equal(typeof(int), typeof(IList<int>).GetEnumerableElementType());
        Assert.Equal(typeof(int), typeof(List<int>).GetEnumerableElementType());
        Assert.Equal(typeof(int), typeof(int[]).GetEnumerableElementType());
        Assert.Equal(typeof(int), typeof(IQueryable<int>).GetEnumerableElementType());
        Assert.Equal(typeof(string), typeof(IEnumerable<string>).GetEnumerableElementType());
        Assert.Equal(typeof(string), typeof(IEnumerable<string?>).GetEnumerableElementType());
        Assert.Equal(typeof(Instant), typeof(IEnumerable<Instant>).GetEnumerableElementType());
        Assert.Equal(typeof(Instant?), typeof(IEnumerable<Instant?>).GetEnumerableElementType());
        Assert.Equal(typeof(IsNullableTestTarget), typeof(IEnumerable<IsNullableTestTarget>).GetEnumerableElementType());
        Assert.Equal(typeof(IsNullableTestTarget), typeof(IEnumerable<IsNullableTestTarget?>).GetEnumerableElementType());
    }
}