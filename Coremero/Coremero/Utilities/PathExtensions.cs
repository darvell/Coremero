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
                return Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "Plugins");
            }
        }

        public static string AppDir
        {
            get { return PlatformServices.Default.Application.ApplicationBasePath; }
        }
    }
}
