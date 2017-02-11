using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Coremero.Messages
{
    public class UrlAttachment : IAttachment
    {
        private HttpClient _httpClient = new HttpClient();
        private string _url;
        public UrlAttachment(string url)
        {
            _url = url;
        }

        public string Name
        {
            get { return Path.GetFileName(_url); }
        }

        public Stream Contents
        {
            get
            {
                return _httpClient.GetStreamAsync(_url).Result;
            }
        }
    }
}
