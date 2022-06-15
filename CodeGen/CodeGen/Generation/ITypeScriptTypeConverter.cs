namespace CodeGen.Generation;

public interface ITypeScriptTypeConverter
{
    string Convert(Type from);
}