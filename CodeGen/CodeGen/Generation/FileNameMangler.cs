using System.Security.Cryptography;
using System.Text;

namespace CodeGen.Generation;

public static class NonCryptographicFileNameMangler
{
    private static readonly object Lock = new();
    private static readonly Dictionary<string, string> Table = new();

    public static string Mangle(string name)
    {
        lock (Lock)
        {
            if (Table.ContainsKey(name))
            {
                return Table[name];
            }

            var hash = string.Join("", SHA256.Create().ComputeHash(
                Encoding.UTF8.GetBytes(name)).Select(b => b.ToString("x2")));

            for (var length = 1; length <= hash.Length; length++)
            {
                var substring = "M" + hash[..length];
                if (Table.ContainsValue(substring))
                {
                    continue;
                }

                Table.Add(name, substring);
                return substring;
            }

            throw new Exception("Hash collision");
        }
    }
}