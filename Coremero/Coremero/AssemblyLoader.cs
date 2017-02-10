using System;
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
        // Since writing our own dependency context is probably a bit far off, let's setup our own.
        private static DependencyContext Context = DependencyContext.Default;

        public AssemblyLoader()
        {
            this.Resolving += AssemblyLoader_Resolving;
        }

        private Assembly AssemblyLoader_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            string appPath = $"{Path.Combine(PathExtensions.AppDir, arg2.Name)}.dll";
            string pluginPath = $"{Path.Combine(PathExtensions.PluginDir, arg2.Name)}.dll";

            // Check the plugin folder and then local.
            if (File.Exists(appPath))
            {
                return LoadFromPath(appPath);
            }
            else if (File.Exists(pluginPath))
            {
                return LoadFromPath(pluginPath);
            }
            return null;
        }

        protected override Assembly Load(AssemblyName assemblyName)
        {
            var deps = Context;
            var res = deps.CompileLibraries.Where(d => d.Name.Contains(assemblyName.Name)).ToList();
            if (res.Count > 0)
            {
                var assembly = Assembly.Load(new AssemblyName(res.First().Name));
                var depContext = DependencyContext.Load(assembly);
                Context = DependencyContext.Default.Merge(depContext);

                return assembly;
            }
            return null;
        }

        public Assembly LoadFromPath(string path)
        {
            Assembly assembly = LoadFromAssemblyPath(path);
            var depContext = DependencyContext.Load(assembly);
            if (depContext != null)
            {
                Context = DependencyContext.Default.Merge(depContext);
            }
            return assembly;
        }
    }
}
