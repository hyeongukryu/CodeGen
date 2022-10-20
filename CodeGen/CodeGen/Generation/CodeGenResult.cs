namespace CodeGen.Generation;

public record CodeGenResult(ICollection<string> ControllerCodes, ICollection<string> DefinitionCodes,
    ICollection<string> ConverterCodes, ICollection<string> UrlBuilderCodes);