using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyModel;

namespace Coremero
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        protected override Assembly Load(AssemblyName assemblyName)
        {
            var deps = DependencyContext.Default;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (res.Count > 0)
            {
                var assembly = Assembly.Load(new AssemblyName(res.First().Name));
                return assembly;
            }

            var runtimeRes = deps.RuntimeLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (runtimeRes.Count > 0)
            {
                var assembly = Assembly.Load(new AssemblyName(res.First().Name));
                return assembly;
            }

            return null;
        }
    }
}
