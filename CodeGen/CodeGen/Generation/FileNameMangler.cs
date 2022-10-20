using System.Security.Cryptography;
using System.Text;

namespace CodeGen.Generation;

public static class NonCryptographicFileNameMangler
{
    public static string Mangle(string name)
    {
        return string.Join("", SHA256.Create().ComputeHash(
            Encoding.UTF8.GetBytes(name)).Select(b => b.ToString("x2")));
    }
}