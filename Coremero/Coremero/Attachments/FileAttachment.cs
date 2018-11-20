using System.IO;

namespace Coremero.Attachments
{
    public class FileAttachment : IAttachment
    {
        public FileAttachment(string filePath)
        {
            _filePath = filePath;
        }

        private readonly string _filePath;

        public string Name
        {
            get { return _filePath; }
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