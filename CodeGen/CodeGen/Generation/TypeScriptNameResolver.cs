namespace CodeGen.Generation;

public class TypeScriptNameResolver
{
    private readonly NamespaceTypeNameResolver _webTypeNameResolver = new("");
    private readonly NamespaceTypeNameResolver _payloadTypeNameResolver = new("_api_");

    public TypeScriptNameResolver(IEnumerable<Type> customTypes)
    {
        var typeList = customTypes.Distinct().ToList();
        _webTypeNameResolver.ResolveNames(typeList);
        _payloadTypeNameResolver.ResolveNames(typeList);
    }

    public string GetWebAppTypeName(CodeGenType type)
    {
        return KnownCodeGenTypes.TryGetTypeScriptWebAppTypeName(type.BaseType, out var builtInTypeName)
            ? builtInTypeName
            : _webTypeNameResolver.GetName(type.BaseType);
    }

    public string GetPayloadTypeName(CodeGenType type)
    {
        return KnownCodeGenTypes.TryGetTypeScriptPayloadTypeName(type.BaseType, out var builtInTypeName)
            ? builtInTypeName
            : _payloadTypeNameResolver.GetName(type.BaseType);
    }

    public string GetFullWebAppTypeName(CodeGenType type)
    {
        return GetWebAppTypeName(type) +
               (type.IsEnumerable ? "[]" : "") +
               (type.IsNullable ? " | null" : "");
    }

    public string GetFullPayloadTypeName(CodeGenType type)
    {
        return GetPayloadTypeName(type) +
               (type.IsEnumerable ? "[]" : "") +
               (type.IsNullable ? " | null" : "");
    }

    public string GetConverterName(CodeGenType type, bool convertClientToServer)
    {
        if (convertClientToServer)
        {
            return $"_convert_{GetWebAppTypeName(type)}_TO_{GetPayloadTypeName(type)}" +
                   (type.IsEnumerable ? "_Array" : "") +
                   (type.IsNullable ? "_Nullable" : "");
        }

        return $"_convert_{GetPayloadTypeName(type)}_TO_{GetWebAppTypeName(type)}" +
               (type.IsEnumerable ? "_Array" : "") +
               (type.IsNullable ? "_Nullable" : "");
    }

}
