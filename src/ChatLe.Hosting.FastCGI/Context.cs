using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.AspNet.FeatureModel;
using Microsoft.AspNet.Http;

namespace ChatLe.Hosting.FastCGI
{
    public class Context : IHttpRequestFeature, IHttpResponseFeature, /*IHttpUpgradeFeature,*/ IDisposable
    {
        public IFeatureCollection Features { private set; get; } = new FeatureCollection();
        public byte Version { get; private set; }
        public ushort Id { get; private set; }

        public bool KeepAlive { get; set; }

        public State State { get; private set; }

        public bool Called { get; set; }
        public Context(byte version, ushort id, bool keepAlive, State state)
        {
            Version = version;
            Id = id;
            KeepAlive = keepAlive;
            State = state;
            ((IHttpResponseFeature)this).Body = new ResponseStream(this);
            _featureStream = _requestStream;
            Features.Add(typeof(IHttpRequestFeature), this);
            Features.Add(typeof(IHttpResponseFeature), this);
            //Features.Add(typeof(IHttpUpgradeFeature), this);
        }

        internal RequestStream _requestStream = new RequestStream();
        Stream _featureStream;
        Stream IHttpRequestFeature.Body
        {
            get { return _featureStream; }
            set { _featureStream = value; }
        }

        IDictionary<string, string[]> IHttpRequestFeature.Headers { get; set; } = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        string IHttpRequestFeature.Method
        {
            get;
            set;
        }

        string IHttpRequestFeature.Path
        {
            get;
            set;
        }

        string IHttpRequestFeature.PathBase
        {
            get;
            set;
        }

        string IHttpRequestFeature.Protocol { get; set; } = "http";

        string IHttpRequestFeature.QueryString
        {
            get;
            set;
        }

        string IHttpRequestFeature.Scheme
        {
            get;
            set;
        }

        int IHttpResponseFeature.StatusCode { get; set; }

        string IHttpResponseFeature.ReasonPhrase { get; set; }

        IDictionary<string, string[]> IHttpResponseFeature.Headers { get; set; } = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        Stream IHttpResponseFeature.Body { get; set; }

        BlockingCollection<KeyValuePair<Action<object>, object>> _sendingHeaders = new BlockingCollection<KeyValuePair<Action<object>, object>>();
        void IHttpResponseFeature.OnSendingHeaders(Action<object> callback, object state)
        {
            _sendingHeaders.Add(new KeyValuePair<Action<object>, object>(callback, state));
        }

        internal void SendingsHeader()
        {
            foreach (var kv in _sendingHeaders)
                kv.Key.Invoke(kv.Value);
        }
        internal void HeadersSent()
        {            
            _headersSent = true;
        }

        private bool _headersSent;
        bool IHttpResponseFeature.HeadersSent
        {
            get
            {
                return _headersSent;
            }
        }


        public CancellationTokenSource CancellationTokenSource { get; } = new CancellationTokenSource();


        //bool IHttpUpgradeFeature.IsUpgradableRequest
        //{
        //    get
        //    {
        //        string[] values;
        //        var feature = this as IHttpRequestFeature;
        //        if (feature.Headers.TryGetValue("Connection", out values))
        //        {
        //            return values.Any(value => value.IndexOf("upgrade", StringComparison.OrdinalIgnoreCase) != -1);
        //        }
        //        return false;
        //    }
        //}
        //Task<Stream> IHttpUpgradeFeature.UpgradeAsync()
        //{
        //    var feature = this as IHttpResponseFeature;
        //    feature.StatusCode = 101;
        //    feature.ReasonPhrase = "Switching Protocols";
        //    var headers = feature.Headers;
        //    headers["Connection"] = new string[] { "Upgrade" };
        //    if (!headers.ContainsKey("Upgrade"))
        //    {
        //        string[] values;
        //        if (headers.TryGetValue("Upgrade", out values))
        //            headers["Upgrade"] = values;
        //    }
        //    return Task.FromResult<Stream>(new UpgradeStream(((IHttpRequestFeature)this).Body, feature.Body));
        //}

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _requestStream.Dispose();
                    ((IHttpRequestFeature)this).Body.Dispose();
                    ((IHttpResponseFeature)this).Body.Dispose();
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        public void OnResponseCompleted(Action<object> callback, object state)
        {
            callback(state);
        }
        #endregion
    }
}