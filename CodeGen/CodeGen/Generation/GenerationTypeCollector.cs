using System.Reflection;
using System.Text.Json.Serialization;
using CodeGen.Analysis;

namespace CodeGen.Generation;

public static class GenerationTypeCollector
{
    public static ISet<Type> CollectTypes(IEnumerable<CodeGenController> controllers, string? tag)
    {
        var types = new HashSet<Type>();

        foreach (var action in controllers.SelectMany(controller => controller.Actions))
        {
            if (tag != null && !action.Tags.Contains(tag))
            {
                continue;
            }

            AddType(action.ResponseType, types);

            if (action.BodyParameter != null)
            {
                AddType(action.BodyParameter.ParameterInfo.ParameterType, types);
            }

            foreach (var parameter in action.PathParameters.Concat(action.QueryParameters))
            {
                AddType(parameter.ParameterInfo.ParameterType, types);
            }
        }

        return types;
    }

    private static void AddType(Type? type, ISet<Type> types)
    {
        if (type == null)
        {
            return;
        }

        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        if (KnownCodeGenTypes.IsBuiltIn(underlyingType))
        {
            return;
        }

        if (underlyingType.IsEnumerable())
        {
            AddType(underlyingType.GetEnumerableElementType(), types);
            return;
        }

        if (!types.Add(underlyingType) || underlyingType.IsEnum)
        {
            return;
        }

        foreach (var property in underlyingType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
        {
            if (property.GetMethod == null || property.GetMethod.GetParameters().Length > 0)
            {
                continue;
            }

            if (property.GetCustomAttribute<JsonIgnoreAttribute>() != null)
            {
                continue;
            }

            AddType(property.PropertyType, types);
        }
    }
}
