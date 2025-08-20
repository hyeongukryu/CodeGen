namespace CodeGen.Generation;

public class TypeScriptApiResult
{
    public required string TypeScriptApi { get; set; }
    public required IEnumerable<TypeScriptApiFile> Files { get; set; }
    public required IEnumerable<string> ErrorMessages { get; set; }
}
