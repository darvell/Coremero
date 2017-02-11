using System;
using System.Collections.Generic;
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
        private Dictionary<AssemblyName, Assembly> _assemblyCache = new Dictionary<AssemblyName, Assembly>();
        public AssemblyLoader()
        {
            this.Resolving += AssemblyLoader_Resolving;
        }

        private Assembly AssemblyLoader_Resolving(AssemblyLoadContext arg1, AssemblyName arg2)
        {
            if (_assemblyCache.ContainsKey(arg2))
            {
                return _assemblyCache[arg2];
            }

            string appPath = $"{Path.Combine(PathExtensions.AppDir, arg2.Name)}.dll";
            string pluginPath = $"{Path.Combine(PathExtensions.PluginDir, arg2.Name)}.dll";

            // Check the plugin folder and then local.
            if (File.Exists(appPath))
            {
                Assembly ass = LoadFromPath(appPath);
                if (ass != null)
                {
                    _assemblyCache[arg2] = ass;
                    return ass;
                }
            }
            else if (File.Exists(pluginPath))
            {
                Assembly ass = LoadFromPath(pluginPath);
                if (ass != null)
                {
                    _assemblyCache[arg2] = ass;
                    return ass;
                }
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
