namespace CodeGen.Generation;

public static class KnownCodeGenTypes
{
    public static bool IsBuiltIn(Type type)
    {
        return TryGetTypeScriptWebAppTypeName(type, out _) &&
               TryGetTypeScriptPayloadTypeName(type, out _);
    }

    public static bool TryGetTypeScriptWebAppTypeName(Type type, out string typeName)
    {
        if (type == typeof(byte[]))
        {
            typeName = "Uint8Array";
            return true;
        }

        typeName = type.FullName switch
        {
            "System.String" => "string",
            "System.Byte" => "number",
            "System.SByte" => "number",
            "System.Int16" => "number",
            "System.Int32" => "number",
            "System.Int64" => "bigint",
            "System.UInt16" => "number",
            "System.UInt32" => "number",
            "System.UInt64" => "bigint",
            "System.Boolean" => "boolean",
            "System.Double" => "number",
            "System.Single" => "number",
            "System.Decimal" => "number",
            "System.DateTime" => "string",
            "System.DateTimeOffset" => "string",
            "System.DateOnly" => "string",
            "System.TimeOnly" => "string",
            "System.Guid" => "string",
            "System.Uri" => "string",
            "NodaTime.Instant" => "_Dayjs",
            "NodaTime.LocalDate" => "string",
            "NodaTime.LocalTime" => "string",
            "NodaTime.LocalDateTime" => "string",
            _ => ""
        };

        return typeName.Length > 0;
    }

    public static bool TryGetTypeScriptPayloadTypeName(Type type, out string typeName)
    {
        if (type == typeof(byte[]))
        {
            typeName = "string";
            return true;
        }

        typeName = type.FullName switch
        {
            "System.String" => "string",
            "System.Byte" => "string",
            "System.SByte" => "string",
            "System.Int16" => "string",
            "System.Int32" => "string",
            "System.Int64" => "string",
            "System.UInt16" => "string",
            "System.UInt32" => "string",
            "System.UInt64" => "string",
            "System.Boolean" => "boolean",
            "System.Double" => "string",
            "System.Single" => "string",
            "System.Decimal" => "string",
            "System.DateTime" => "string",
            "System.DateTimeOffset" => "string",
            "System.DateOnly" => "string",
            "System.TimeOnly" => "string",
            "System.Guid" => "string",
            "System.Uri" => "string",
            "NodaTime.Instant" => "string",
            "NodaTime.LocalDate" => "string",
            "NodaTime.LocalTime" => "string",
            "NodaTime.LocalDateTime" => "string",
            _ => ""
        };

        return typeName.Length > 0;
    }
}
