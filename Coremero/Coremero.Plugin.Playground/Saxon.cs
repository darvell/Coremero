using System.IO;
using Coremero.Commands;
using Coremero.Utilities;

namespace Coremero.Plugin.Playground
{
    public class Saxon : IPlugin
    {
        private readonly string _path = Path.Combine(PathExtensions.ResourceDir, "saxon.txt");
        private FileIndex _fileIndex;

        public Saxon()
        {
            _fileIndex = new FileIndex(_path);
        }

        [Command("saxon")]
        public string SaySaxon()
        {
            return _fileIndex.GetRandomLine();
        }
    }
}