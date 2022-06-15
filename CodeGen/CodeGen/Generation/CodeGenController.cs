namespace CodeGen.Generation;

public class CodeGenController
{
    public CodeGenController(string name)
    {
        Name = name;
    }

    public string Name { get; }
    public List<CodeGenAction> Actions { get; } = new();
}