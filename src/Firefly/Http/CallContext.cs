using Firefly.Streams;
using Microsoft.AspNet.HttpFeature;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Firefly
{
    /// <summary>
    /// Summary description for CallContext
    /// </summary>
    public class CallContext : 
        IHttpRequestFeature, 
        IHttpResponseFeature,
        IHttpUpgradeFeature
    {
        public CallContext()
        {
            ((IHttpResponseFeature)this).StatusCode = 200;
        }

        Stream IHttpResponseFeature.Body { get; set; }

        Stream IHttpRequestFeature.Body { get; set; }

        IDictionary<string, string[]> IHttpResponseFeature.Headers { get; set; }

        IDictionary<string, string[]> IHttpRequestFeature.Headers { get; set; }


        string IHttpRequestFeature.Method { get; set; }

        string IHttpRequestFeature.Path { get; set; }

        string IHttpRequestFeature.PathBase { get; set; }

        string IHttpRequestFeature.Protocol { get; set; }

        string IHttpRequestFeature.QueryString { get; set; }

        string IHttpResponseFeature.ReasonPhrase { get; set; }

        string IHttpRequestFeature.Scheme { get; set; }

        int IHttpResponseFeature.StatusCode { get; set; }


        bool _headersSent;
        bool IHttpResponseFeature.HeadersSent
        {
            get
            {
                return _headersSent;
            }
        }

        BlockingCollection<KeyValuePair<Action<object>, object>> _sendingHeaders = new BlockingCollection<KeyValuePair<Action<object>, object>>();
        void IHttpResponseFeature.OnSendingHeaders(Action<object> callback, object state)
        {
            _sendingHeaders.Add(new KeyValuePair<Action<object>, object>(callback, state));
        }

        internal void SendingHeaders()
        {
            foreach (var kv in _sendingHeaders)
                kv.Key.Invoke(kv.Value);
        }
        internal void HeadersSent()
        {
            _headersSent = true;
        }

        bool IHttpUpgradeFeature.IsUpgradableRequest
        {
            get
            {
                string[] values;
                var feature = this as IHttpRequestFeature;
                if (feature.Headers.TryGetValue("Connection", out values))
                {
                    return values.Any(value => value.IndexOf("upgrade", StringComparison.OrdinalIgnoreCase) != -1);
                }
                return false;
            }
        }

        Task<Stream> IHttpUpgradeFeature.UpgradeAsync()
        {
            var feature = this as IHttpResponseFeature;
            feature.StatusCode = 101;
            feature.ReasonPhrase = "Switching Protocols";
            var headers = feature.Headers;
            headers["Connection"] = new string[] { "Upgrade" };
            if (!headers.ContainsKey("Upgrade"))
            {
                string[] values;
                if (headers.TryGetValue("Upgrade", out values))
                    headers["Upgrade"] = values;
            }
            return Task.FromResult<Stream>(new DuplexStream(((IHttpRequestFeature)this).Body, feature.Body));
        }
    }
}