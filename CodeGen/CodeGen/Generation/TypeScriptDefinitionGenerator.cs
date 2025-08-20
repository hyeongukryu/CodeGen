using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CodeGen.Generation;

public class TypeScriptDefinitionGenerator
{
    private readonly ICollection<string> _definitionNames;
    private readonly ICollection<string> _errorMessages;
    private readonly ICollection<string> _definitionCodes;
    private readonly TypeScriptNameResolver _typeNameResolver;

    public TypeScriptDefinitionGenerator(ICollection<string> definitionNames,
        ICollection<string> definitionCodes,
        ICollection<string> errorMessages,
        TypeScriptNameResolver typeNameResolver)
    {
        _definitionNames = definitionNames;
        _definitionCodes = definitionCodes;
        _errorMessages = errorMessages;
        _typeNameResolver = typeNameResolver;
    }

    private void Generate(CodeGenType type, bool generatePayloadName)
    {
        var properties = type.BaseType.GetProperties();
        var builder = new StringBuilder();
        var typeName = generatePayloadName
            ? _typeNameResolver.GetPayloadTypeName(type)
            : _typeNameResolver.GetWebAppTypeName(type);
        var fullName = type.BaseType.AssemblyQualifiedName;
        if (fullName == null)
        {
            _errorMessages.Add("AssemblyQualifiedName " + typeName);
        }

        if (_definitionNames.Contains(typeName))
        {
            return;
        }

        _definitionNames.Add(typeName);

        builder.AppendLine("export interface " + typeName + " {");

        foreach (var property in properties)
        {
            if (property.GetCustomAttribute(typeof(JsonIgnoreAttribute)) != null)
            {
                continue;
            }

            var propertyType = property.ToCodeGenType();

            var propertyTypeName = generatePayloadName
                ? _typeNameResolver.GetFullPayloadTypeName(propertyType)
                : _typeNameResolver.GetFullWebAppTypeName(propertyType);

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
