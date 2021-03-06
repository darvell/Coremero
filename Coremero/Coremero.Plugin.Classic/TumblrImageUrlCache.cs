﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Coremero.Plugin.Classic.TumblrJson;
using Coremero.Utilities;
using Newtonsoft.Json;

namespace Coremero.Plugin.Classic
{
    public class TumblrImageUrlCache
    {
        private readonly string _username;
        private readonly string _apiKey;
        private DateTime _lastUpdate = DateTime.MinValue;
        private readonly TimeSpan _cacheInvalidationTime;
        private List<string> _lastCache = new List<string>();

        public TumblrImageUrlCache(string username, string apiKey, TimeSpan cacheInvalidationTime)
        {
            _username = username;
            _apiKey = apiKey;
            _cacheInvalidationTime = cacheInvalidationTime;
        }

        public async Task<IEnumerable<string>> GetImagesAsync()
        {
            if (_lastCache.Count == 0 || (DateTime.Now - _lastUpdate) > _cacheInvalidationTime)
            {
                await FillCache();
            }

            lock (_lastCache)
            {
                return _lastCache.ToImmutableList();
            }
        }

        public async Task<string> Pop()
        {
            if (_lastCache.Count == 0 || (DateTime.Now - _lastUpdate) > _cacheInvalidationTime)
            {
                await FillCache();
            }

            lock (_lastCache)
            {
                string result = _lastCache[0];
                _lastCache.RemoveAt(0);
                return result;
            }
        }

        public async Task FillCache()
        {
            List<string> newUrls = new List<string>();
            using (HttpClient httpClient = new HttpClient())
            {
                for (int i = 0; i < 200; i += 20)
                {
                    try
                    {
                        string blogJson =
                            await httpClient.GetStringAsync(
                                $"http://api.tumblr.com/v2/blog/{_username}.tumblr.com/posts?api_key={_apiKey}&type=photo&offset={i}");
                        var root = JsonConvert.DeserializeObject<Rootobject>(blogJson);
                        newUrls.AddRange(root.response.posts.Where(x => x?.photos != null).SelectMany(x => x.photos)
                            .Select(x => x?.original_size?.url).Where(x => !string.IsNullOrEmpty(x)));
                        if (newUrls.Count > 200)
                            break;
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            lock (_lastCache)
            {
                _lastCache.Clear();
                _lastCache.AddRange(newUrls.Shuffle());
            }

            _lastUpdate = DateTime.Now;
        }
    }
}