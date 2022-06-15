using System.Text;

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

    private void Generate(CodeGenType type, bool generatePayloadName)
    {
        var properties = type.BaseType.GetProperties();
        var builder = new StringBuilder();
        var typeName = generatePayloadName ? type.GetPayloadTypeName() : type.GetWebAppTypeName();

        if (_definitionNames.Contains(typeName))
        {
            return;
        }

        _definitionNames.Add(typeName);

        builder.AppendLine("export interface " + typeName + " {");

        foreach (var property in properties)
        {
            var propertyType = property.ToCodeGenType();

            var propertyTypeName = generatePayloadName
                ? propertyType.GetFullPayloadTypeName()
                : propertyType.GetFullWebAppTypeName();

            GenerateIfNotExists(propertyType);
            builder.AppendLine($"        {property.Name.ToCamelCase()}: {propertyTypeName};");
        }

        builder.Append('}');
        _definitionCodes.Add(builder.ToString());
    }

    public void GenerateIfNotExists(CodeGenType type)
    {
        Generate(type, false);
        Generate(type, true);
    }
}