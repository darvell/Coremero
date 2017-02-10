using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Coremero.Utilities;
using Microsoft.Extensions.DependencyModel;

namespace Coremero
{
    public class AssemblyLoader : AssemblyLoadContext
    {
        private string _path;
        private DependencyContext _dependencyContext;
        public AssemblyLoader(string path = null, DependencyContext dependencyContext = null)
        {
            if (path == null)
            {
                _path = PathExtensions.AppDir;
            }
        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            var deps = DependencyContext.Default;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (res.Count > 0)
            {
                var assembly = Assembly.Load(new AssemblyName(res.First().Name));
                return assembly;
            }
            /*
            else if (_dependencyContext != null)
            {
                _dependencyContext.CompileLibraries.Where()
            }
            */
            else
            { 
                var apiApplicationFileInfo = new FileInfo($"{PathExtensions.PluginDir}{Path.DirectorySeparatorChar}{assemblyName.Name}.dll");
                if (File.Exists(apiApplicationFileInfo.FullName))
                {
                    // Check if there's a depedency context there.
                    DependencyContext.Load()
                    var asl = new AssemblyLoader(apiApplicationFileInfo.DirectoryName);
                    return asl.LoadFromAssemblyPath(apiApplicationFileInfo.FullName);
                }
            }
            return null;
        }
    }
}
