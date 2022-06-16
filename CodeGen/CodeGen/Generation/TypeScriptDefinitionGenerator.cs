using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CodeGen.Generation;

public class TypeScriptDefinitionGenerator
{
    private readonly ICollection<string> _definitionNames;
    private readonly ISet<Tuple<string, string>> _definitionFullNames;
    private readonly ICollection<string> _errorMessages;
    private readonly ICollection<string> _definitionCodes;

    public TypeScriptDefinitionGenerator(ICollection<string> definitionNames,
        ICollection<string> definitionCodes,
        ISet<Tuple<string, string>> definitionFullNames,
        ICollection<string> errorMessages)
    {
        _definitionNames = definitionNames;
        _definitionCodes = definitionCodes;
        _definitionFullNames = definitionFullNames;
        _errorMessages = errorMessages;
    }

    private void Generate(CodeGenType type, bool generatePayloadName)
    {
        var properties = type.BaseType.GetProperties();
        var builder = new StringBuilder();
        var typeName = generatePayloadName ? type.GetPayloadTypeName() : type.GetWebAppTypeName();
        var fullName = type.BaseType.AssemblyQualifiedName;
        if (fullName == null)
        {
            _errorMessages.Add("AssemblyQualifiedName " + typeName);
        }

        if (_definitionFullNames.Any(typeNameAndFullNAme =>
                typeNameAndFullNAme.Item1 == typeName && typeNameAndFullNAme.Item2 != fullName))
        {
            _errorMessages.Add("DefinitionFullNames " + typeName);
        }

        if (_definitionNames.Contains(typeName))
        {
            return;
        }

        _definitionNames.Add(typeName);
        _definitionFullNames.Add(Tuple.Create(typeName, fullName)!);

        builder.AppendLine("export interface " + typeName + " {");

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null)
            {
                continue;
            }

            var propertyType = property.ToCodeGenType();

            var propertyTypeName = generatePayloadName
                ? propertyType.GetFullPayloadTypeName()
                : propertyType.GetFullWebAppTypeName();

            GenerateIfNotExists(propertyType);
            builder.AppendLine($"    {property.Name.ToCamelCase()}: {propertyTypeName};");
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