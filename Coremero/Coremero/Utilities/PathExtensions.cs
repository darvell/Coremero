using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.PlatformAbstractions;

namespace Coremero.Utilities
{
    public static class PathExtensions
    {
        public static string PluginDir
        {
            get
            {
                string pluginDir = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Plugins");
                if (Directory.Exists(pluginDir))
                {
                    return pluginDir;
                }
                return AppDir;
            }
        }

        public static string AppDir
        {
            get { return PlatformServices.Default.Application.ApplicationBasePath; }
        }
    }
}
