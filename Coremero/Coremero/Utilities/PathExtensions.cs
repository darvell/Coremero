using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.PlatformAbstractions;

namespace Coremero.Utilities
{
    public static class PathExtensions
    {
        private static string _pluginDirCache;

        public static string PluginDir
        {
            get
            {
                if (_pluginDirCache == null)
                {
                    string pluginDir = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Plugins");
                    if (Directory.Exists(pluginDir))
                    {
                        _pluginDirCache = pluginDir;
                    }
                    else
                    {
                        Log.Debug("No plugin directory found, reverting to application directory.");
                        _pluginDirCache = AppDir;
                    }
                }
                return _pluginDirCache;
            }
        }

        public static string AppDir
        {
            get { return PlatformServices.Default.Application.ApplicationBasePath; }
        }

        public static string ResourceDir
        {
            get { return Path.Combine(PluginDir, "Resources"); }
        }
    }
}