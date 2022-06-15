namespace CodeGen.Generation;

public class CodeGenType
{
    public CodeGenType(Type baseType, bool isEnumerable, bool isNullable)
    {
        BaseType = baseType;
        IsEnumerable = isEnumerable;
        IsNullable = isNullable;
    }

    public Type BaseType { get; }
    public bool IsEnumerable { get; }
    public bool IsNullable { get; }
}