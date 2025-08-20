using System.Reflection;
using System.Text.Json;
using CodeGen.Analysis;

namespace CodeGen.Generation;

public static class TypeScriptHelper
{
    public static CodeGenType ToCodeGenType(this PropertyInfo propertyInfo)
    {
        if (KnownCodeGenTypes.IsBuiltIn(propertyInfo.PropertyType))
        {
            return new CodeGenType(propertyInfo.PropertyType, false, propertyInfo.IsNullable());
        }

        if (propertyInfo.PropertyType.IsEnumerable())
        {
            return new CodeGenType(propertyInfo.PropertyType.GetEnumerableElementType()!, true,
                propertyInfo.IsNullable());
        }

        return new CodeGenType(propertyInfo.PropertyType.GetNullableElementType() ?? propertyInfo.PropertyType,
            false,
            propertyInfo.IsNullable());
    }

    public static CodeGenType ToCodeGenType(this ParameterInfo parameterInfo)
    {
        if (KnownCodeGenTypes.IsBuiltIn(parameterInfo.ParameterType))
        {
            return new CodeGenType(parameterInfo.ParameterType, false, parameterInfo.IsNullable());
        }

        if (parameterInfo.ParameterType.IsEnumerable())
        {
            return new CodeGenType(parameterInfo.ParameterType.GetEnumerableElementType()!, true,
                parameterInfo.IsNullable());
        }

        return new CodeGenType(parameterInfo.ParameterType.GetNullableElementType() ?? parameterInfo.ParameterType,
            false,
            parameterInfo.IsNullable());
    }

    public static CodeGenType ToCodeGenType(this Type type)
    {
        if (KnownCodeGenTypes.IsBuiltIn(type))
        {
            return new CodeGenType(type, false, false);
        }

        if (type.IsEnumerable())
        {
            return new CodeGenType(type.GetEnumerableElementType()!, true, false);
        }

        if (type.GetEnumerableElementType() != null)
        {
            throw new NotImplementedException();
        }

        return new CodeGenType(type, false, false);
    }

    public static string ToCamelCase(this string name)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(name);
    }

    public static string ToPascalCase(this string name)
    {
        var pascal = JsonNamingPolicy.CamelCase.ConvertName(name);
        return pascal[..1].ToUpper() + pascal[1..];
    }

    public static string GetUrlName(this CodeGenAction action)
    {
        return $"_{action.Controller.GeneratedName}_{action.HttpMethod}_{action.Name}_url";
    }
}
