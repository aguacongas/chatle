using Microsoft.AspNet.HttpFeature;
using System;
using System.Collections.Generic;
using System.IO;

namespace ChatLe.Hosting.FastCGI
{
    public class Context : IHttpRequestFeature
    {
        public ushort Id { get; private set; }

        public bool KeepAlive { get; private set; }

        public Context(ushort id, bool keepAlive)
        {
            Id = id;
            KeepAlive = keepAlive;
        }

        Stream IHttpRequestFeature.Body { get; set; } = new RequestStream();

        IDictionary<string, string[]> IHttpRequestFeature.Headers
        {
            get;
            set;
        }

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

        string IHttpRequestFeature.Protocol
        {
            get;
            set;
        }

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
    }
}