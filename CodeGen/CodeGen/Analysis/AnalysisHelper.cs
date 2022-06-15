using System.Collections;
using System.Reflection;

namespace CodeGen.Analysis;

public static class AnalysisHelper
{
    public static bool IsNullable(this PropertyInfo propertyInfo)
    {
        var nullabilityInfoContext = new NullabilityInfoContext();
        return nullabilityInfoContext.Create(propertyInfo).ReadState != NullabilityState.NotNull;
    }

    public static bool IsNullable(this ParameterInfo parameterInfo)
    {
        var nullabilityInfoContext = new NullabilityInfoContext();
        return nullabilityInfoContext.Create(parameterInfo).ReadState != NullabilityState.NotNull;
    }

    public static bool IsEnumerable(this Type type)
    {
        if (type == typeof(string))
        {
            return false;
        }

        return typeof(IEnumerable).IsAssignableFrom(type);
    }

    public static Type? GetEnumerableElementType(this Type type)
    {
        if (type.IsEnumerable() == false)
        {
            return null;
        }

        if (type.IsArray)
        {
            return type.GetElementType();
        }

        return type.GenericTypeArguments.FirstOrDefault();
    }
    
    public static Type? GetNullableElementType(this Type type)
    {
        return type.GenericTypeArguments.FirstOrDefault();
    }
}