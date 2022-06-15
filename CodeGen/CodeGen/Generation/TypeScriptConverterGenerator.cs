namespace CodeGen.Generation;

public class TypeScriptConverterGenerator
{
    private readonly ICollection<string> _converterNames;
    private readonly ICollection<string> _converterCodes;
    private readonly TypeScriptDefinitionGenerator _definitionGenerator;

    public TypeScriptConverterGenerator(ICollection<string> converterNames, ICollection<string> converterCodes,
        TypeScriptDefinitionGenerator definitionGenerator)
    {
        _converterNames = converterNames;
        _converterCodes = converterCodes;
        _definitionGenerator = definitionGenerator;
    }

    public void GenerateIfNotExists(CodeGenType type)
    {
        DoGenerateIfNotExists(type, true);
        DoGenerateIfNotExists(type, false);
    }

    private void DoGenerateIfNotExists(CodeGenType type, bool reverse)
    {
        var converterName = type.GetConverterName(reverse);
        if (_converterNames.Contains(converterName))
        {
            return;
        }

        _converterNames.Add(converterName);
        _definitionGenerator.GenerateIfNotExists(type);

        var fullPayloadTypeName = type.GetFullPayloadTypeName();
        var fullWebAppTypeName = type.GetFullWebAppTypeName();
        var fromType = !reverse ? fullPayloadTypeName : fullWebAppTypeName;
        var toType = !reverse ? fullWebAppTypeName : fullPayloadTypeName;

        if (type.IsNullable)
        {
            var nonNullableType = new CodeGenType(type.BaseType, type.IsEnumerable, false);
            var delegateConverterName = nonNullableType.GetConverterName(reverse);
            DoGenerateIfNotExists(nonNullableType, reverse);
            _converterCodes.Add(
                @$"function {converterName}(from: {fromType}): {toType} {{
    if (from === null) {{
        return null;
    }}
    return {delegateConverterName}(from);
}}");
            return;
        }

        if (type.IsEnumerable)
        {
            var elementType = new CodeGenType(type.BaseType, false, false);
            var elementConverterName = elementType.GetConverterName(reverse);
            DoGenerateIfNotExists(elementType, reverse);

            _converterCodes.Add(
                @$"function {converterName}(from: {fromType}): {toType} {{
    if (from.hasOwnProperty('$ref')) {{
        return from as any;
    }}
    if (from.hasOwnProperty('$values')) {{
        from = (from as any).$values;
        const to: {toType} = from.map(element => {elementConverterName}(element));
        return {{ ...from, $values: to }} as any;
    }}
    const to: {toType} = from.map(element => {elementConverterName}(element));
    return to;
}}");
            return;
        }

        var propertyMappings = string.Join(Environment.NewLine,
            type.BaseType.GetProperties().Select(property =>
            {
                var propertyType = property.ToCodeGenType();
                var propertyConvertMethodName = propertyType.GetConverterName(reverse);
                DoGenerateIfNotExists(propertyType, reverse);
                var propertyName = property.Name.ToCamelCase();
                return $"        {propertyName}: {propertyConvertMethodName}(from.{propertyName}),";
            }));

        _converterCodes.Add(
            @$"function {converterName}(from: {fromType}): {toType} {{
    if (from.hasOwnProperty('$ref')) {{
        return from as any;
    }}
    const to: {toType} = {{
{propertyMappings}
    }};
    return {{ ...from, ...to }};
}}");
    }
}