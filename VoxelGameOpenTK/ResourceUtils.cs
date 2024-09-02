using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGameOpenTK;

internal static class ResourceUtils
{
    public static string ReadResource(string name)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(typeof(ResourceUtils), name) ?? throw new Exception($"Resource not found: '{name}'");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
