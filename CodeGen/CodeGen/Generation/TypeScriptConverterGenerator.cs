using System.Reflection;
using System.Text.Json.Serialization;

namespace CodeGen.Generation;

public class TypeScriptConverterGenerator
{
    private readonly ICollection<string> _converterNames;
    private readonly ICollection<string> _converterCodes;
    private readonly TypeScriptDefinitionGenerator _definitionGenerator;
    private readonly IReferenceHandlerConfiguration _referenceHandlerConfiguration;
    private readonly IDictionary<string, ISet<string>> _dependencies = new Dictionary<string, ISet<string>>();

    public TypeScriptConverterGenerator(ICollection<string> converterNames, ICollection<string> converterCodes,
        TypeScriptDefinitionGenerator definitionGenerator, IReferenceHandlerConfiguration referenceHandlerConfiguration)
    {
        _converterNames = converterNames;
        _converterCodes = converterCodes;
        _definitionGenerator = definitionGenerator;
        _referenceHandlerConfiguration = referenceHandlerConfiguration;
    }

    public void GenerateIfNotExists(CodeGenType type)
    {
        DoGenerateIfNotExists(type, true);
        DoGenerateIfNotExists(type, false);
    }

    private void AddDependency(CodeGenType from, CodeGenType to)
    {
        if (_referenceHandlerConfiguration.PreserveReferences)
        {
            return;
        }

        _dependencies.AddDependency(from.GetWebAppTypeName(), to.GetWebAppTypeName());
    }

    public string? CheckDependencyErrors()
    {
        var cycle = _dependencies.FindCycleIfAny();
        return cycle == null
            ? null
            : @$"There is a cycle in the dependencies of the generated converters. The cycle starts with {cycle}.";
    }

    private void DoGenerateIfNotExists(CodeGenType type, bool convertClientToServer)
    {
        var converterName = type.GetConverterName(convertClientToServer);
        if (_converterNames.Contains(converterName))
        {
            return;
        }

        _converterNames.Add(converterName);
        _definitionGenerator.GenerateIfNotExists(type);

        var fullPayloadTypeName = type.GetFullPayloadTypeName();
        var fullWebAppTypeName = type.GetFullWebAppTypeName();
        var fromType = convertClientToServer ? fullWebAppTypeName : fullPayloadTypeName;
        var toType = convertClientToServer ? fullPayloadTypeName : fullWebAppTypeName;

        if (type.IsNullable)
        {
            var nonNullableType = new CodeGenType(type.BaseType, type.IsEnumerable, false);
            var delegateConverterName = nonNullableType.GetConverterName(convertClientToServer);
            DoGenerateIfNotExists(nonNullableType, convertClientToServer);
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
            var elementConverterName = elementType.GetConverterName(convertClientToServer);
            DoGenerateIfNotExists(elementType, convertClientToServer);

            _converterCodes.Add(
                @$"function {converterName}(from: {fromType}): {toType} {{
    if (_hasOwnPropertyRef(from)) {{
        return from as any;
    }}
    if (_hasOwnPropertyValues(from)) {{
        const values: {fromType} = (from as any).$values;
        const to: {toType} = values.map(element => {elementConverterName}(element));
        return {{ ...from, $values: to }} as any;
    }}
    const to: {toType} = from.map(element => {elementConverterName}(element));
    return to;
}}");
            return;
        }

        var propertyMappings = string.Join(Environment.NewLine,
            type.BaseType.GetProperties()
                .Where(property => property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) == null)
                .Select(property =>
                {
                    var propertyType = property.ToCodeGenType();
                    var propertyConvertMethodName = propertyType.GetConverterName(convertClientToServer);
                    DoGenerateIfNotExists(propertyType, convertClientToServer);
                    AddDependency(type, propertyType);
                    var propertyName = property.Name.ToCamelCase();
                    return $"        {propertyName}: {propertyConvertMethodName}(from.{propertyName}),";
                }));

        _converterCodes.Add(
            @$"function {converterName}(from: {fromType}): {toType} {{
    if (_hasOwnPropertyRef(from)) {{
        return from as any;
    }}
    const to: {toType} = {{
{propertyMappings}
    }};
    return {{ ...from, ...to }};
}}");
    }
}