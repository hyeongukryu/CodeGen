using System.Text;
using CodeGen.Analysis;

namespace CodeGen.Generation;

public class TypeScriptDefinitionGenerator
{
    private readonly ICollection<string> _definitionNames;
    private readonly ICollection<string> _definitionCodes;

    public TypeScriptDefinitionGenerator(ICollection<string> definitionNames, ICollection<string> definitionCodes)
    {
        _definitionNames = definitionNames;
        _definitionCodes = definitionCodes;
    }

    private void Generate(Type type, bool generatePayloadName)
    {
        var properties = type.GetProperties();
        var builder = new StringBuilder();
        var typeName = generatePayloadName
            ? TypeScriptHelper.GetPayloadTypeName(type)
            : TypeScriptHelper.GetWebAppTypeName(type);

        if (_definitionNames.Contains(typeName))
        {
            return;
        }

        _definitionNames.Add(typeName);

        builder.AppendLine("export interface " + typeName + " {");

        foreach (var property in properties)
        {
            var propertyIsEnumerable = property.PropertyType.IsEnumerable();
            var propertyIsNullable = property.IsNullable();
            var propertyType = property.PropertyType.IsEnumerable()
                ? property.PropertyType.GetEnumerableElementType()!
                : (propertyIsNullable
                    ? property.PropertyType.GetNullableElementType() ?? property.PropertyType
                    : property.PropertyType);

            var propertyTypeName = generatePayloadName
                ? TypeScriptHelper.GetFullPayloadTypeName(propertyType, propertyIsEnumerable, propertyIsNullable)
                : TypeScriptHelper.GetFullWebAppTypeName(propertyType, propertyIsEnumerable, propertyIsNullable);

            GenerateIfNotExists(propertyType);
            builder.AppendLine($"        {TypeScriptHelper.GetPropertyName(property.Name)}: {propertyTypeName};");
        }

        builder.Append('}');
        _definitionCodes.Add(builder.ToString());
    }

    public void GenerateIfNotExists(Type type)
    {
        Generate(type, false);
        Generate(type, true);
    }
}