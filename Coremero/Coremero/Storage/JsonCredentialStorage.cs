using System.Diagnostics;
using System.IO;
using Coremero.Utilities;
using Newtonsoft.Json.Linq;

namespace Coremero.Storage
{
    public class JsonCredentialStorage : ICredentialStorage
    {
        private readonly string _secretsPath = Path.Combine(PathExtensions.AppDir, "secrets.json");

        public string GetKey(string keyName, string defaultKey = null)
        {
            if (!File.Exists(_secretsPath))
            {
                Debug.Fail("No secrets file found!");
                return defaultKey;
            }

            JObject config = JObject.Parse(File.ReadAllText(_secretsPath));
            return config.Value<string>(keyName) ?? defaultKey;
        }
    }
}