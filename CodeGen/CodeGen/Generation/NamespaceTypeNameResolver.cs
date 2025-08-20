namespace CodeGen.Generation;

public class NamespaceTypeNameResolver(string preservedPrefix)
{
    private readonly IDictionary<Type, string> _typeNames = new Dictionary<Type, string>();

    public void ResolveNames(IEnumerable<Type> types)
    {
        var typeList = types.Distinct().ToList();

        foreach (var type in typeList)
        {
            _typeNames[type] = preservedPrefix + type.Name;
        }

        foreach (var group in typeList.GroupBy(type => type.Name))
        {
            var groupList = group.ToList();
            if (groupList.Count == 1)
            {
                continue;
            }

            var prefixDepthByType = groupList.ToDictionary(
                type => type,
                type => GetNamespaceParts(type).Length > 0 ? 1 : 0);

            while (true)
            {
                foreach (var type in groupList)
                {
                    _typeNames[type] = BuildName(type, prefixDepthByType[type]);
                }

                var collisions = groupList
                    .GroupBy(type => _typeNames[type])
                    .Where(grouping => grouping.Count() > 1)
                    .Select(grouping => grouping.ToList())
                    .ToList();
                if (collisions.Count == 0)
                {
                    break;
                }

                var progressed = false;
                foreach (var collisionGroup in collisions)
                {
                    foreach (var type in collisionGroup)
                    {
                        if (prefixDepthByType[type] >= GetNamespaceParts(type).Length)
                        {
                            continue;
                        }

                        prefixDepthByType[type]++;
                        progressed = true;
                    }
                }

                if (!progressed)
                {
                    break;
                }
            }
        }

        var globalCollisions = typeList
            .GroupBy(type => _typeNames[type])
            .Where(grouping => grouping.Count() > 1)
            .Select(grouping => grouping.ToList())
            .ToList();

        foreach (var collisionGroup in globalCollisions)
        {
            foreach (var type in collisionGroup)
            {
                var typeIdentity = type.FullName ?? type.AssemblyQualifiedName ?? type.Name;
                _typeNames[type] = $"{_typeNames[type]}_{NonCryptographicFileNameMangler.Mangle(typeIdentity)}";
            }
        }
    }

    public string GetName(Type type)
    {
        return _typeNames[type];
    }

    private string BuildName(Type type, int prefixDepth)
    {
        if (prefixDepth <= 0)
        {
            return preservedPrefix + type.Name;
        }

        var prefix = string.Concat(GetNamespaceParts(type).TakeLast(prefixDepth));
        return preservedPrefix + prefix + type.Name;
    }

    private static string[] GetNamespaceParts(Type type)
    {
        return type.Namespace?.Split('.') ?? [];
    }
}
