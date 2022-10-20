namespace CodeGen.Generation;

public record CodeGenResult(ICollection<CodeGenControllerResult> Controllers, ICollection<string> DefinitionCodes,
    ICollection<string> ConverterCodes, ICollection<string> UrlBuilderCodes);