using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coremero.Messages
{
    public class FileAttachment : IAttachment
    {
        public FileAttachment(string filePath)
        {
            _filePath = filePath;
        }

        private string _filePath;

        public string Name
        {
            get
            {
                return _filePath;
            }
        }

        public Stream Contents
        {
            get
            {
                // TODO: Possible handle leak.
                return new FileStream(_filePath, FileMode.Open);
            }
        }
    }
}
