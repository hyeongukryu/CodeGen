using System.Text.Json;

namespace CodeGen.Generation;

public class TypeScriptHelper
{
    public static string GetWebAppTypeName(Type type)
    {
        switch (type.FullName)
        {
            case "System.String": return "string";
            case "System.Int16": return "number";
            case "System.Int32": return "number";
            case "System.Int64": return "bigint";
            case "System.Boolean": return "boolean";
            case "System.Double": return "number";
            case "System.Single": return "number";
            case "NodaTime.Instant": return "_Dayjs";
            default:
                return type.Name;
        }
    }

    public static string GetPayloadTypeName(Type type)
    {
        switch (type.FullName)
        {
            case "System.String": return "string";
            case "System.Int16": return "string";
            case "System.Int32": return "string";
            case "System.Int64": return "string";
            case "System.Boolean": return "boolean";
            case "System.Double": return "string";
            case "System.Single": return "string";
            case "NodaTime.Instant": return "string";
            default:
                return "_" + type.Name;
        }
    }

    public static string GetFullWebAppTypeName(Type type, bool isArray, bool isNullable)
    {
        return GetWebAppTypeName(type) +
               (isArray ? "[]" : "") +
               (isNullable ? " | null" : "");
    }

    public static string GetFullPayloadTypeName(Type type, bool isArray, bool isNullable)
    {
        return GetPayloadTypeName(type) +
               (isArray ? "[]" : "") +
               (isNullable ? " | null" : "");
    }

    public static string GetConverterName(Type type, bool isArray, bool isNullable)
    {
        return $"_convert_{GetPayloadTypeName(type)}_{GetWebAppTypeName(type)}" +
               (isArray ? "_Array" : "") +
               (isNullable ? "_Nullable" : "");
    }

    public static string GetPropertyName(string name)
    {
        return JsonNamingPolicy.CamelCase.ConvertName(name);
    }

    public static string GetUrlName(CodeGenAction action)
    {
        return $"_{action.Controller.Name}_{action.HttpMethod}_{action.Name}_url";
    }
}