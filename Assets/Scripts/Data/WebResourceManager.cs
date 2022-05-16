using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Utility;

namespace Data
{
    public class WebResourceManager: IWebResourceManager {
        [Serializable]
        public class CacheEntry {
            public Dictionary<string, string> Headers {get; set; }
        }
        [Serializable]
        public class CacheIndex {
            public Dictionary<string, CacheEntry> EntriesByUrl { get; set; }

            public CacheIndex(){
                EntriesByUrl = new Dictionary<string, CacheEntry>();
            }

            public string GetHeader(string url, string headerName)
            {
                CacheEntry entry = null;
                if (EntriesByUrl.TryGetValue(url, out entry)) {
                    string headerValue = null;
                    if (entry.Headers.TryGetValue(headerName, out headerValue)) {
                        return headerValue;
                    }

                }
                return null;
            }
        }

        private static object _lock = new object();
        private CacheIndex _cacheIndex = null;
        public WebResourceManager(string tempPath) {
            TempPath = tempPath;
        }

        public string TempPath { get; }
        public string IndexPath { get { return Path.Combine(TempPath, "cache.index.json"); } }

        public async Task<string> GetResourcePath (string url) {
            var cachePath = GetCachePath(url);
            if (string.IsNullOrEmpty(cachePath)) {
                return null;
            }
            var cacheExists = File.Exists(cachePath);
            var request = CreateRequest(url, cacheExists);
            try {
                using (var response = await request.GetResponseAsync()) {
                    var stream = response.GetResponseStream();
                    using (var dest = new FileStream(cachePath, FileMode.OpenOrCreate)) {
                        stream.CopyTo(dest);
                    }
                    UpdateCacheIndex(url, response);
                    return cachePath; 
                }
            } catch (WebException e) {
                var statusCode = (e?.Response as HttpWebResponse)?.StatusCode;
                switch (statusCode) {
                    case HttpStatusCode.NotModified: {
                        Debug.Log($"[304 Not Modified]: {url} -> {cachePath}");
                        return cachePath;
                    }
                    default: {
                        throw e;
                    }
                }
            }
        }

        public async Task<Dictionary<string, string>> GetResourcePaths(IEnumerable<string> urls)
        {
            var uniqueUrls = urls
                .Where(url => !string.IsNullOrEmpty(url))
                .UniqueBy(url => url)
                .ToList();
                
            Debug.Log($"[WebResourceManager] the urls are...");   
            foreach (var url in uniqueUrls) {
                Debug.Log($"[WebResourceManager] -- {url}");   
            }

            var mapping = new Dictionary<string, string>();
            await Task.WhenAll(uniqueUrls.Select(async url => {
                Debug.Log($"[WebResourceManager] fetching {url}...");
                var path = await GetResourcePath(url);
                Debug.Log($"[WebResourceManager] done fetching {url}");
                lock (mapping) {
                    mapping[url] = path;
                }
                Debug.Log($"[WebResourceManager] {url} -> {path}");
            }));

            return mapping;
        }

        protected string GetCachePath (string url ) {
            Uri validatedUri;
            if (!Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out validatedUri)) {
                return null;
            };
            var name = new UriBuilder(url).Path.Split('/').Last();
            var hash = GetHash(url);
            var fileName = Path.Combine(TempPath, $"{hash}.{name}");
            return fileName;
        }
 
        protected virtual string GetHash (string s) {
            using (var alg = MD5.Create())
            {
                return BitConverter.ToString(alg.ComputeHash(Encoding.UTF8.GetBytes(s))).Replace("-", "");
            }
        }

        protected WebRequest CreateRequest(string url, bool allowCaching) {
            var cacheIndex = GetCacheIndex();
            var etag = cacheIndex?.GetHeader(url, "ETag");
            var lastModified = cacheIndex?.GetHeader(url, "Last-Modified");
            var request = WebRequest.Create(url);
            if (allowCaching) {
                if (!string.IsNullOrEmpty(etag)) {
                    request.Headers.Set("If-None-Match", etag);
                }
                if (string.IsNullOrEmpty(lastModified)) {
                    request.Headers.Set("If-Modified-Since", lastModified);
                }
            }
            return request;
        }

        
        protected CacheIndex GetCacheIndex () {
            return LockCacheIndex(index => index);
        }

        private void UpdateCacheIndex(string url, WebResponse response)
        {
            LockCacheIndex(index => {
                var newIndex = DeepCopy(index);
                var headers = response.Headers;
                var entry = new CacheEntry{
                    Headers = headers.AllKeys.ToDictionary(key => key, key => headers.Get(key))
                };
                newIndex.EntriesByUrl[url] = entry;
                File.WriteAllText(IndexPath, JsonConvert.SerializeObject(newIndex, Formatting.Indented));
                return newIndex;
            });
        }

        protected CacheIndex LockCacheIndex (Func<CacheIndex, CacheIndex> fn) {
            lock(_lock) {
                if (_cacheIndex == null) {
                    try {
                        var text = File.ReadAllText(IndexPath);
                        _cacheIndex = JsonConvert.DeserializeObject<CacheIndex>(text);
                    } catch (FileNotFoundException) {
                        _cacheIndex = new CacheIndex();
                    }
                }
                _cacheIndex = fn(_cacheIndex);
                return _cacheIndex;
            }
        }


        protected static T DeepCopy<T>(T source)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, source);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}