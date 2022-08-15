using System.Reflection;
using System.Runtime.InteropServices;

namespace API.Utilities
{
    public static class AssemblyInfo
    {
        public static readonly string AssemblyVersion;

        static AssemblyInfo()
        {
            AssemblyVersion = typeof(RuntimeEnvironment).GetTypeInfo()
                .Assembly.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;
        }
    }
}