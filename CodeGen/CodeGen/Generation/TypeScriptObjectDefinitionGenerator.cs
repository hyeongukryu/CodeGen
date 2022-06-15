namespace CodeGen.Generation;

public class TypeScriptObjectDefinitionGenerator : IObjectDefinitionGenerator
{
    private readonly ITypeScriptTypeConverter _typeConverter;

    public TypeScriptObjectDefinitionGenerator(ITypeScriptTypeConverter typeConverter)
    {
        _typeConverter = typeConverter;
    }
}