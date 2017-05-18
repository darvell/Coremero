using System;
using System.Collections.Generic;
using System.IO;

namespace Coremero.Utilities
{
    public class FileIndex
    {
        private List<long> _indexes = new List<long>();
        private Random _rnd = new Random();
        private string _filePath;

        public FileIndex(string filePath)
        {
            _filePath = filePath;
            IndexFile();
        }

        public string GetRandomLine()
        {
            using (FileStream stream = File.OpenRead(_filePath))
            {
                stream.Position = _indexes[_rnd.Next(_indexes.Count)];
                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadLine();
                }
            }
        }
        
        private void IndexFile()
        {
            using (FileStream stream = File.OpenRead(_filePath))
            {
                _indexes.Add(stream.Position);

                int character;
                while ((character = stream.ReadByte()) != -1) // "-1" denotes the end of the file
                {
                    if (character == '\n')
                    {
                        _indexes.Add(stream.Position);
                    }
                }
            }
        }
    }
}
