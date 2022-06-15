using CodeGen.Analysis;

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

    public void GenerateIfNotExists(Type type, bool isArray, bool isNullable)
    {
        var converterName = TypeScriptHelper.GetConverterName(type, isArray, isNullable);
        if (_converterNames.Contains(converterName))
        {
            return;
        }

        _converterNames.Add(converterName);
        _definitionGenerator.GenerateIfNotExists(type);

        var fullPayloadTypeName = TypeScriptHelper.GetFullPayloadTypeName(type, isArray, isNullable);
        var fullWebAppTypeName = TypeScriptHelper.GetFullWebAppTypeName(type, isArray, isNullable);

        if (isNullable)
        {
            var delegateConverterName = TypeScriptHelper.GetConverterName(type, isArray, false);
            GenerateIfNotExists(type, isArray, false);

            _converterCodes.Add(
                @$"function {converterName}(from: {fullPayloadTypeName}): {fullWebAppTypeName} {{
    if (from === null) {{
        return null;
    }}
    return {delegateConverterName}(from);
}}");
            return;
        }

        if (isArray)
        {
            var elementConverterName = TypeScriptHelper.GetConverterName(type, false, false);
            GenerateIfNotExists(type, false, false);

            _converterCodes.Add(
                @$"function {converterName}(from: {fullPayloadTypeName}): {fullWebAppTypeName} {{
    if (from.hasOwnProperty('$ref')) {{
        return from as any;
    }}
    if (from.hasOwnProperty('$values')) {{
        from = (from as any).$values;
        const to: {fullWebAppTypeName} = from.map(element => {elementConverterName}(element));
        return {{ ...from, $values: to }} as any;
    }}
    const to: {fullWebAppTypeName} = from.map(element => {elementConverterName}(element));
    return to;
}}");
            return;
        }

        var propertyMappings = string.Join(Environment.NewLine,
            type.GetProperties().Select(property =>
            {
                var propertyIsEnumerable = property.PropertyType.IsEnumerable();
                var propertyIsNullable = property.IsNullable();
                var propertyType = property.PropertyType.IsEnumerable()
                    ? property.PropertyType.GetEnumerableElementType()!
                    : (propertyIsNullable
                        ? property.PropertyType.GetNullableElementType() ?? property.PropertyType
                        : property.PropertyType);

                var propertyConvertMethodName =
                    TypeScriptHelper.GetConverterName(propertyType, propertyIsEnumerable, propertyIsNullable);
                GenerateIfNotExists(propertyType, propertyIsEnumerable, propertyIsNullable);
                var propertyName = TypeScriptHelper.GetPropertyName(property.Name);
                return $"        {propertyName}: {propertyConvertMethodName}(from.{propertyName}),";
            }));

        _converterCodes.Add(
            @$"function {converterName}(from: {fullPayloadTypeName}): {fullWebAppTypeName} {{
    if (from.hasOwnProperty('$ref')) {{
        return from as any;
    }}
    const to: {fullWebAppTypeName} = {{
{propertyMappings}
    }};
    return {{ ...from, ...to }};
}}");
    }
}