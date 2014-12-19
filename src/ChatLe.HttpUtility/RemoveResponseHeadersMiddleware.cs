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
using Microsoft.AspNet.Http.Security;
using System.Security.Claims;
using System.Net.WebSockets;
using System.Threading;

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
            //var responseFeature = context.GetFeature<IHttpResponseFeature>();
            
            //context.SetFeature(new HttpResponseFeature(responseFeature, _headersToRemove));
            //responseFeature.OnSendingHeaders(state =>
            //{
            //    Trace.TraceInformation("[IHttpResponseFeature] OnSendingHeaders callback " + context.Request.Path);
            //}, null);

            await _next.Invoke(new RemoveHeaderHttpContext(context, _headersToRemove));            
        }

        class RemoveHeaderHttpContext : HttpContext
        {
            readonly HttpContext _parent;
            readonly HttpResponse _response;
            public RemoveHeaderHttpContext(HttpContext parent, IEnumerable<string> headersToRemove)
            {
                _parent = parent;
                _response = new RemoveHeaderHttpResponse(this, parent.Response, headersToRemove);
            }

            public override IServiceProvider ApplicationServices
            {
                get
                {
                    return _parent.ApplicationServices;
                }

                set
                {
                    _parent.ApplicationServices = value;
                }
            }

            public override bool IsWebSocketRequest
            {
                get
                {
                    return _parent.IsWebSocketRequest;
                }
            }

            public override IDictionary<object, object> Items
            {
                get
                {
                    return _parent.Items;
                }
            }

            public override HttpRequest Request
            {
                get
                {
                    return _parent.Request;
                }
            }

            public override CancellationToken RequestAborted
            {
                get
                {
                    return _parent.RequestAborted;
                }
            }

            public override IServiceProvider RequestServices
            {
                get
                {
                    return _parent.RequestServices;
                }

                set
                {
                    _parent.RequestServices = value;
                }
            }

            public override HttpResponse Response
            {
                get
                {
                    return _response;
                }
            }

            public override ISessionCollection Session
            {
                get
                {
                    return _parent.Session;
                }
            }

            public override ClaimsPrincipal User
            {
                get
                {
                    return _parent.User;
                }

                set
                {
                    _parent.User = value;
                }
            }

            public override IList<string> WebSocketRequestedProtocols
            {
                get
                {
                    return _parent.WebSocketRequestedProtocols;
                }
            }

            public override void Abort()
            {
                _parent.Abort();
            }

            public override Task<WebSocket> AcceptWebSocketAsync(string subProtocol)
            {
                return _parent.AcceptWebSocketAsync(subProtocol);
            }

            public override IEnumerable<AuthenticationResult> Authenticate(IEnumerable<string> authenticationTypes)
            {
                return _parent.Authenticate(authenticationTypes);
            }

            public override Task<IEnumerable<AuthenticationResult>> AuthenticateAsync(IEnumerable<string> authenticationTypes)
            {
                return _parent.AuthenticateAsync(authenticationTypes);
            }

            public override void Dispose()
            {
                _parent.Dispose();
            }

            public override IEnumerable<AuthenticationDescription> GetAuthenticationTypes()
            {
                return _parent.GetAuthenticationTypes();
            }

            public override object GetFeature(Type type)
            {
                return _parent.GetFeature(type);
            }

            public override void SetFeature(Type type, object instance)
            {
                _parent.SetFeature(type, instance);
            }
        }

        class RemoveHeaderHttpResponse : HttpResponse
        {
            readonly HttpResponse _parent;
            readonly HttpContext _context;
            readonly IHeaderDictionary _headers;
            public RemoveHeaderHttpResponse(HttpContext context, HttpResponse parent, IEnumerable<string> headersToRemove)
            {
                _parent = parent;
                _context = context;
                _headers = new RemoveHeaderHeaderDictionary(parent.Headers, headersToRemove);
            }

            public override Stream Body
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

            public override long? ContentLength
            {
                get
                {
                    return _parent.ContentLength;
                }

                set
                {
                    _parent.ContentLength = value;
                }
            }

            public override string ContentType
            {
                get
                {
                    return _parent.ContentType;
                }

                set
                {
                    _parent.ContentType = value;
                }
            }

            public override IResponseCookies Cookies
            {
                get
                {
                    return _parent.Cookies;
                }
            }

            public override IHeaderDictionary Headers
            {
                get
                {
                    return _headers;
                }
            }

            public override HttpContext HttpContext
            {
                get
                {
                    return _context;
                }
            }

            public override int StatusCode
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

            public override void Challenge(AuthenticationProperties properties, IEnumerable<string> authenticationTypes)
            {
                _parent.Challenge(properties, authenticationTypes);
            }

            public override void OnSendingHeaders(Action<object> callback, object state)
            {
                _parent.OnSendingHeaders(callback, state);
            }

            public override void Redirect(string location, bool permanent)
            {
                _parent.Redirect(location, permanent);
            }

            public override void SignIn(AuthenticationProperties properties, IEnumerable<ClaimsIdentity> identities)
            {
                _parent.SignIn(properties, identities);
            }

            public override void SignOut(IEnumerable<string> authenticationTypes)
            {
                _parent.SignOut(authenticationTypes);
            }
        }

        class RemoveHeaderHeaderDictionary : IHeaderDictionary
        {
            readonly IHeaderDictionary _parent;
            readonly IEnumerable<string> _headersToRemove;
            public RemoveHeaderHeaderDictionary(IHeaderDictionary parent, IEnumerable<string> headersToRemove)
            {
                _parent = parent;
                _headersToRemove = headersToRemove;
                foreach (var header in headersToRemove)
                    parent.Remove(header);
            }

            bool IsAllowedHeader(string header)
            {
                var allowed = !_headersToRemove.Any(h => h == header);
                Trace.TraceInformation("[RemoveHeaderHeaderDictionary] {0} is {1}", header, allowed ? "allowed" : "not allowed");
                return allowed;
            }

            public string this[string key]
            {
                get
                {
                    return _parent[key];
                }

                set
                {
                    if (IsAllowedHeader(key))
                        _parent[key] = value;
                }
            }

            string[] IDictionary<string, string[]>.this[string key]
            {
                get
                {
                    return ((IDictionary<string, string[]>)_parent)[key];
                }

                set
                {
                    if (IsAllowedHeader(key))
                        ((IDictionary<string, string[]>)_parent)[key] = value;
                }
            }

            string IReadableStringCollection.this[string key]
            {
                get
                {
                    return ((IReadableStringCollection)_parent)[key];
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
                if (IsAllowedHeader(item.Key))
                    _parent.Add(item);
            }

            public void Add(string key, string[] value)
            {
                if (IsAllowedHeader(key))
                    _parent.Add(key, value);
            }

            public void Append(string key, string value)
            {
                if (IsAllowedHeader(key))
                    _parent.Append(key, value);
            }

            public void AppendCommaSeparatedValues(string key, params string[] values)
            {
                if (IsAllowedHeader(key))
                    _parent.AppendCommaSeparatedValues(key, values);
            }

            public void AppendValues(string key, params string[] values)
            {
                if (IsAllowedHeader(key))
                    _parent.AppendValues(key, values);
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

            public string Get(string key)
            {
                return _parent.Get(key);
            }

            public IList<string> GetCommaSeparatedValues(string key)
            {
                return _parent.GetCommaSeparatedValues(key);
            }

            public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
            {
                return _parent.GetEnumerator();
            }

            public IList<string> GetValues(string key)
            {
                return _parent.GetValues(key);
            }

            public bool Remove(KeyValuePair<string, string[]> item)
            {
                return _parent.Remove(item);
            }

            public bool Remove(string key)
            {
                return _parent.Remove(key);
            }

            public void Set(string key, string value)
            {
                if (IsAllowedHeader(key))
                    _parent.Set(key, value);
            }

            public void SetCommaSeparatedValues(string key, params string[] values)
            {
                if (IsAllowedHeader(key))
                    _parent.SetCommaSeparatedValues(key, values);
            }

            public void SetValues(string key, params string[] values)
            {
                if (IsAllowedHeader(key))
                    _parent.SetValues(key, values);
            }

            public bool TryGetValue(string key, out string[] value)
            {
                return _parent.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable)_parent).GetEnumerator();
            }
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