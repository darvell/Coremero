using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Coremero.Messages
{
    public class StreamAttachment : IAttachment
    {
        public StreamAttachment(Stream stream, string fileName)
        {
            Name = fileName;
            Contents = stream;
        }

        public string Name { get; private set; }
        public Stream Contents { get; private set; }
    }
}
