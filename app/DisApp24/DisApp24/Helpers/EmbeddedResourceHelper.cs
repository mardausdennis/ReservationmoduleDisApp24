using System.IO;
using System.Reflection;

namespace DisApp24.Helpers
{
    public static class EmbeddedResourceHelper
    {
        public static Stream GetResourceStream(Assembly assembly, string resourceName)
        {
            return assembly.GetManifestResourceStream(resourceName);
        }

        public static string GetResourceText(Assembly assembly, string resourceName)
        {
            using var stream = GetResourceStream(assembly, resourceName);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
