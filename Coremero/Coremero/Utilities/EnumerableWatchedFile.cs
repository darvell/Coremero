using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coremero.Utilities
{
    public class EnumerableWatchedFile : IEnumerable<string>, IDisposable
    {
        private readonly List<string> _cachedContents = new List<string>();
        private readonly char _fileSeparator;
        private readonly FileSystemWatcher _fileSystemWatcher;
        
        public IEnumerator<string> GetEnumerator()
        {
            return _cachedContents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        /// <summary>
        /// Creates an IEnumerable<string> from a file that is backed in to RAM and updated on file changes.
        /// </summary>
        /// <param name="filePath">The path of the file to watch.</param>
        /// <param name="fileSeperator">How to split the file in to the enumerable.</param>
        public EnumerableWatchedFile(string filePath, char fileSeparator = '\n')
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException(filePath);
            }
            _fileSeparator = fileSeparator;
            _fileSystemWatcher = new FileSystemWatcher(Path.GetDirectoryName(filePath), Path.GetFileName(filePath))
            {
                EnableRaisingEvents = true
            };
            _fileSystemWatcher.Changed += FileSystemWatcherOnChanged;
            UpdateCacheFromFile(filePath);
        }

        private void FileSystemWatcherOnChanged(object sender, FileSystemEventArgs e)
        {
            UpdateCacheFromFile(e.FullPath);
        }

        private void UpdateCacheFromFile(string filePath)
        {
            var newContents = File.ReadAllText(filePath).Split(_fileSeparator);

            lock (_cachedContents)
            {
                _cachedContents.Clear();
                _cachedContents.AddRange(newContents);
            }
        }

        public void Dispose()
        {
            _fileSystemWatcher?.Dispose();
        }
    }
}
