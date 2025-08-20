using System.Text;

namespace CodeGen.Generation;

public class ControllerFileNameResolver
{
    private readonly IDictionary<CodeGenController, string> _fileNames =
        new Dictionary<CodeGenController, string>();

    public void ResolveNames(IEnumerable<CodeGenController> controllers)
    {
        var controllerList = controllers.ToList();

        foreach (var controller in controllerList)
        {
            _fileNames[controller] = BuildBaseFileName(controller);
        }

        var collisions = controllerList
            .GroupBy(controller => _fileNames[controller], StringComparer.OrdinalIgnoreCase)
            .Where(grouping => grouping.Count() > 1)
            .Select(grouping => grouping.ToList())
            .ToList();

        foreach (var collisionGroup in collisions)
        {
            foreach (var controller in collisionGroup)
            {
                _fileNames[controller] =
                    $"{_fileNames[controller]}_{NonCryptographicFileNameMangler.Mangle(controller.TypeIdentity)}";
            }
        }
    }

    public string GetFileName(CodeGenController controller)
    {
        return _fileNames[controller];
    }

    private static string BuildBaseFileName(CodeGenController controller)
    {
        var typeName = controller.ControllerType.FullName ?? controller.TypeIdentity;
        var builder = new StringBuilder(typeName.Length);
        var previousWasUnderscore = false;

        foreach (var character in typeName)
        {
            if (char.IsLetterOrDigit(character) || character == '.')
            {
                builder.Append(character);
                previousWasUnderscore = false;
                continue;
            }

            if (previousWasUnderscore)
            {
                continue;
            }

            builder.Append('_');
            previousWasUnderscore = true;
        }

        return builder.ToString().Trim('_');
    }
}
