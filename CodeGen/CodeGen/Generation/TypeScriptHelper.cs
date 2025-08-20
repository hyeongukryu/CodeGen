using System.Reflection;
using System.Text.Json;
using CodeGen.Analysis;

namespace CodeGen.Generation;

public static class TypeScriptHelper
{
    public static CodeGenType ToCodeGenType(this PropertyInfo propertyInfo)
    {
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

    public static string GetWebAppTypeName(this CodeGenType type)
    {
        return type.BaseType.FullName switch
        {
            "System.String" => "string",
            "System.Int16" => "number",
            "System.Int32" => "number",
            "System.Int64" => "bigint",
            "System.Boolean" => "boolean",
            "System.Double" => "number",
            "System.Single" => "number",
            "NodaTime.Instant" => "_Dayjs",
            "NodaTime.LocalDate" => "string",
            "NodaTime.LocalTime" => "string",
            "NodaTime.LocalDateTime" => "string",
            "System.DateTime" => "string",
            _ => type.BaseType.Name
        };
    }

    public static string GetPayloadTypeName(this CodeGenType type)
    {
        return type.BaseType.FullName switch
        {
            "System.String" => "string",
            "System.Int16" => "string",
            "System.Int32" => "string",
            "System.Int64" => "string",
            "System.Boolean" => "boolean",
            "System.Double" => "string",
            "System.Single" => "string",
            "NodaTime.Instant" => "string",
            "NodaTime.LocalDate" => "string",
            "NodaTime.LocalTime" => "string",
            "NodaTime.LocalDateTime" => "string",
            "System.DateTime" => "string",
            _ => "_api_" + type.BaseType.Name
        };
    }

    public static string GetFullWebAppTypeName(this CodeGenType type)
    {
        return type.GetWebAppTypeName() +
               (type.IsEnumerable ? "[]" : "") +
               (type.IsNullable ? " | null" : "");
    }

    public static string GetFullPayloadTypeName(this CodeGenType type)
    {
        return type.GetPayloadTypeName() +
               (type.IsEnumerable ? "[]" : "") +
               (type.IsNullable ? " | null" : "");
    }

    public static string GetConverterName(this CodeGenType type, bool convertClientToServer)
    {
        if (convertClientToServer)
        {
            return $"_convert_{type.GetWebAppTypeName()}_TO_{type.GetPayloadTypeName()}" +
                   (type.IsEnumerable ? "_Array" : "") +
                   (type.IsNullable ? "_Nullable" : "");
        }

        return $"_convert_{type.GetPayloadTypeName()}_TO_{type.GetWebAppTypeName()}" +
               (type.IsEnumerable ? "_Array" : "") +
               (type.IsNullable ? "_Nullable" : "");
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
        return $"_{action.Controller.Name}_{action.HttpMethod}_{action.Name}_url";
    }
}