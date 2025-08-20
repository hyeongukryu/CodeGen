namespace CodeGen.Generation;

public class CodeGenController
{
    public CodeGenController(Type controllerType, string controllerName)
    {
        ControllerType = controllerType;
        ControllerName = controllerName;
        GeneratedName = controllerName;
    }

    public Type ControllerType { get; }
    public string ControllerName { get; }
    public string GeneratedName { get; set; }
    public string TypeIdentity => ControllerType.FullName ?? ControllerType.AssemblyQualifiedName ?? ControllerName;
    public string[] NamespaceParts => ControllerType.Namespace?.Split('.') ?? [];
    public List<CodeGenAction> Actions { get; } = new();
}
