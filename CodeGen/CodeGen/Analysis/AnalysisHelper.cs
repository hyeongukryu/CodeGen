using System.Reflection;

namespace CodeGen.Analysis;

public static class AnalysisHelper
{
    public static bool IsNullableType(this Type type)
    {
        return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }
}