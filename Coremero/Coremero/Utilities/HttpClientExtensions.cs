using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Coremero.Utilities
{
    public static class HttpClientExtensions
    {
        public static async Task<MemoryStream> GetStreamAndBufferToMemory(this HttpClient client, string url)
        {
            Stream responseStream = await client.GetStreamAsync(url);
            return await responseStream.CopyToMemoryStreamAsync();
        }
    }
}
