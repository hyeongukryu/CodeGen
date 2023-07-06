namespace CodeGen.Generation;

public static class DependencyHelper
{
    public static void AddDependency<T>(this IDictionary<T, ISet<T>> dependencies, T from, T to)
    {
        if (!dependencies.TryGetValue(from, out var edgeSet))
        {
            dependencies[from] = edgeSet = new HashSet<T>();
        }

        edgeSet.Add(to);
    }

    public static T? FindCycleIfAny<T>(this IDictionary<T, ISet<T>> dependencies)
        where T : class
    {
        var visited = new HashSet<T>();
        var stack = new HashSet<T>();

        void Search(T node)
        {
            if (stack.Contains(node))
            {
                throw new CycleFoundException<T>(node);
            }

            if (visited.Contains(node))
            {
                return;
            }

            visited.Add(node);
            stack.Add(node);

            if (dependencies.TryGetValue(node, out var edgeSet))
            {
                foreach (var edge in edgeSet)
                {
                    Search(edge);
                }
            }

            stack.Remove(node);
        }

        try
        {
            foreach (var node in dependencies.Keys)
            {
                Search(node);
            }
        }
        catch (CycleFoundException<T> e)
        {
            return e.Node;
        }

        return null;
    }

    private class CycleFoundException<T> : Exception
    {
        public CycleFoundException(T node) : base($"Cycle found at node {node}")
        {
            Node = node;
        }

        public T Node { get; }
    }
}