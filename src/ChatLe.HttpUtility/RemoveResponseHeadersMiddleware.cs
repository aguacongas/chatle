using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.HttpFeature;
using Microsoft.Framework.OptionsModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections;
using System.Linq;
using System.IO;

namespace ChatLe.HttpUtility
{
    public class RemoveResponseHeadersMiddleware
    {
        readonly IEnumerable<string> _headersToRemove;
        readonly RequestDelegate _next;
        public RemoveResponseHeadersMiddleware(RequestDelegate next, IOptions<RemoveResponseHeardersOptions> optionsAccessor)
        {
            Trace.TraceInformation("[RemoveResponseHeadersMiddleware] constructor");
            if (next == null)
                throw new ArgumentNullException("next");
            if (optionsAccessor == null || optionsAccessor.Options == null)
                throw new ArgumentNullException("optionsAccessor");
            _next = next;
            _headersToRemove = optionsAccessor.Options.Headers;
        }

        public async Task Invoke(HttpContext context)
        {
            Trace.TraceInformation("[RemoveResponseHeadersMiddleware] Invoke " + context.Request.Path);
            var responseFeature = context.GetFeature<IHttpResponseFeature>();
            context.SetFeature(new HttpResponseFeature(responseFeature, _headersToRemove));
            responseFeature.OnSendingHeaders(state =>
            {
                Trace.TraceInformation("[IHttpResponseFeature] OnSendingHeaders callback " + context.Request.Path);
            }, null);

            await _next.Invoke(context);            
        }

        class HttpResponseFeature : IHttpResponseFeature
        {
            readonly IHttpResponseFeature _parent;
            readonly IDictionary<string, string[]> _headers;
            public HttpResponseFeature(IHttpResponseFeature parent, IEnumerable<string> headersToRemove)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");
                if (headersToRemove == null)
                    throw new ArgumentNullException("headersToRemove");
                _parent = parent;
                _headers = new HeaderDictionary(parent.Headers, headersToRemove);
            }

            public Stream Body
            {
                get
                {
                    return _parent.Body;
                }

                set
                {
                    _parent.Body = value;
                }
            }

            public IDictionary<string, string[]> Headers
            {
                get
                {
                    return _headers;
                }

                set
                {
                    throw new NotSupportedException();
                }
            }

            public string ReasonPhrase
            {
                get
                {
                    return _parent.ReasonPhrase;
                }

                set
                {
                    _parent.ReasonPhrase = value;
                }
            }

            public int StatusCode
            {
                get
                {
                    return _parent.StatusCode;
                }

                set
                {
                    _parent.StatusCode = value;
                }
            }

            public void OnSendingHeaders(Action<object> callback, object state)
            {
                _parent.OnSendingHeaders(callback, state);
            }
        }

        class HeaderDictionary : IDictionary<string, string[]>
        {
            readonly IDictionary<string, string[]> _parent;
            readonly IEnumerable<string> _headersToRemove;
            public HeaderDictionary(IDictionary<string, string[]> parent, IEnumerable<string> headersToRemove)
            {
                if (parent == null)
                    throw new ArgumentNullException("parent");
                if (headersToRemove == null)
                    throw new ArgumentNullException("headersToRemove");

                _parent = parent;
                _headersToRemove = headersToRemove;
                RemoveHeaders(_parent);
            }

            public string[] this[string key]
            {
                get
                {
                    return _parent[key];
                }
                set
                {
                    if (!_headersToRemove.Any(h => h == key))
                        _parent[key] = value;
                    else
                        Trace.TraceInformation("[RemoveResponseHeadersMiddleware.HeaderDictionary] not set header[ " + key + "]");
                }
            }

            public int Count
            {
                get
                {
                    return _parent.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return _parent.IsReadOnly;
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    return _parent.Keys;
                }
            }

            public ICollection<string[]> Values
            {
                get
                {
                    return _parent.Values;
                }
            }

            public void Add(KeyValuePair<string, string[]> item)
            {
                Add(item.Key, item.Value);
            }

            public void Add(string key, string[] value)
            {
                if (!_headersToRemove.Any(h => h == key))
                    _parent.Add(key, value);
                else
                    Trace.TraceInformation("[RemoveResponseHeadersMiddleware.HeaderDictionary] not Add header " + key);
            }

            public void Clear()
            {
                _parent.Clear();
            }

            public bool Contains(KeyValuePair<string, string[]> item)
            {
                return _parent.Contains(item);
            }

            public bool ContainsKey(string key)
            {
                return _parent.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
            {
                _parent.CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
            {
                return _parent.GetEnumerator();
            }

            public bool Remove(KeyValuePair<string, string[]> item)
            {
                return _parent.Remove(item);
            }

            public bool Remove(string key)
            {
                return _parent.Remove(key);
            }

            public bool TryGetValue(string key, out string[] value)
            {
                return _parent.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_parent).GetEnumerator();
            }

            private void RemoveHeaders(IDictionary<string, string[]> headers)
            {
                foreach (var header in _headersToRemove)
                {
                    if (headers.ContainsKey(header))
                    {
                        Trace.TraceInformation("[RemoveResponseHeadersMiddleware.HeaderDictionary] RemoveHeaders header: {0}={1} removed", header, string.Join(", ", headers[header]));
                        headers.Remove(header);
                    }
                }
            }
        }
    }
}