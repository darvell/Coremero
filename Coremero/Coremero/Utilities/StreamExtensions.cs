using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Coremero.Utilities
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads and copies a stream to a MemoryStream asynchronously.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="disposeOriginalStream">Disposes the original stream after copying if true.</param>
        public static async Task<MemoryStream> CopyToMemoryStreamAsync(this Stream stream, bool disposeOriginalStream = true)
        {
            MemoryStream result = new MemoryStream();
            await stream.CopyToAsync(result);

            if (disposeOriginalStream)
            {
                stream.Dispose();
            }

            // Seek to start like it's brand new.
            result.Seek(0, SeekOrigin.Begin);

            return result;
        }

        /// <summary>
        /// Reads and copies a stream to a MemoryStream.
        /// </summary>
        /// <param name="stream">Input stream.</param>
        /// <param name="disposeOriginalStream">Disposes the original stream after copying if true.</param>
        public static MemoryStream CopyToMemoryStream(this Stream stream, bool disposeOriginalStream = true)
        {
            return CopyToMemoryStreamAsync(stream, disposeOriginalStream).Result;
        }
    }
}
