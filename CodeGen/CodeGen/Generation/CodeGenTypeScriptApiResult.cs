namespace CodeGen.Generation;

public record CodeGenTypeScriptApiResult(ICollection<CodeGenControllerResult> Controllers,
    ICollection<string> DefinitionCodes, ICollection<string> DefinitionNames,
    ICollection<string> ConverterCodes, ICollection<string> ConverterNames,
    ICollection<string> UrlBuilderCodes, ICollection<string> UrlBuilderNames);