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
        switch (type.BaseType.FullName)
        {
            case "System.String": return "string";
            case "System.Int16": return "number";
            case "System.Int32": return "number";
            case "System.Int64": return "bigint";
            case "System.Boolean": return "boolean";
            case "System.Double": return "number";
            case "System.Single": return "number";
            case "NodaTime.Instant": return "_Dayjs";
            case "NodaTime.LocalDate": return "_Dayjs";
            case "NodaTime.LocalTime": return "_Dayjs";
            case "Microsoft.AspNetCore.Mvc.FileContentResult": return "";
            default:
                return type.BaseType.Name;
        }
    }

    public static string GetPayloadTypeName(this CodeGenType type)
    {
        switch (type.BaseType.FullName)
        {
            case "System.String": return "string";
            case "System.Int16": return "string";
            case "System.Int32": return "string";
            case "System.Int64": return "string";
            case "System.Boolean": return "boolean";
            case "System.Double": return "string";
            case "System.Single": return "string";
            case "NodaTime.Instant": return "string";
            case "NodaTime.LocalDate": return "string";
            case "NodaTime.LocalTime": return "string";
            default:
                return "_api_" + type.BaseType.Name;
        }
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

    public static string GetConverterName(this CodeGenType type, bool reverse)
    {
        if (reverse)
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

    public static string GetUrlName(this CodeGenAction action)
    {
        return $"_{action.Controller.Name}_{action.HttpMethod}_{action.Name}_url";
    }
}