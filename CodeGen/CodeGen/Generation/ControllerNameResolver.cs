namespace CodeGen.Generation;

public class ControllerNameResolver
{
    public void ResolveNames(IEnumerable<CodeGenController> controllers)
    {
        var controllerList = controllers.ToList();

        foreach (var controller in controllerList)
        {
            controller.GeneratedName = controller.ControllerName;
        }

        foreach (var group in controllerList.GroupBy(controller => controller.ControllerName))
        {
            var groupList = group.ToList();
            if (groupList.Count == 1)
            {
                continue;
            }

            var prefixDepthByController = groupList.ToDictionary(
                controller => controller,
                controller => controller.NamespaceParts.Length > 0 ? 1 : 0);

            while (true)
            {
                foreach (var controller in groupList)
                {
                    controller.GeneratedName = BuildName(controller, prefixDepthByController[controller]);
                }

                var collisions = groupList
                    .GroupBy(controller => controller.GeneratedName)
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
                    foreach (var controller in collisionGroup)
                    {
                        if (prefixDepthByController[controller] >= controller.NamespaceParts.Length)
                        {
                            continue;
                        }

                        prefixDepthByController[controller]++;
                        progressed = true;
                    }
                }

                if (!progressed)
                {
                    break;
                }
            }
        }

        var globalCollisions = controllerList
            .GroupBy(controller => controller.GeneratedName)
            .Where(grouping => grouping.Count() > 1)
            .Select(grouping => grouping.ToList())
            .ToList();

        foreach (var collisionGroup in globalCollisions)
        {
            foreach (var controller in collisionGroup)
            {
                controller.GeneratedName =
                    $"{controller.GeneratedName}_{NonCryptographicFileNameMangler.Mangle(controller.TypeIdentity)}";
            }
        }
    }

    private static string BuildName(CodeGenController controller, int prefixDepth)
    {
        if (prefixDepth <= 0)
        {
            return controller.ControllerName;
        }

        var prefix = string.Concat(controller.NamespaceParts.TakeLast(prefixDepth));
        return prefix + controller.ControllerName;
    }
}
