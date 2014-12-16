using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System.Collections.Generic;
using System;
using Microsoft.Framework.OptionsModel;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Security;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Threading;
using System.IO;
using System.Diagnostics;
using System.Collections;
using Microsoft.AspNet.HttpFeature;

namespace ChatLe.HttpUtility
{
    public class RemoveResponseHeadersMiddleware
    {
        IEnumerable<string> _headersToRemove;
        RequestDelegate _next;
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
            Trace.TraceInformation("[RemoveResponseHeadersMiddleware] Invoke");
            var responseFeature = context.GetFeature<IHttpResponseFeature>();
            context.SetFeature<IHttpResponseFeature>(new HttpResponseFeature(responseFeature));
            await _next.Invoke(new HttpContextDecorator(context));
        }

        class HttpResponseFeature : IHttpResponseFeature
        {
            readonly IHttpResponseFeature _feature;
            IDictionary<string, string[]> _headers;
            public HttpResponseFeature(IHttpResponseFeature feature)
            {
                Trace.TraceInformation("[HttpResponseFeature] constructor");
                _feature = feature;
                _headers = new HeaderStore(feature.Headers);
            }
            public Stream Body
            {
                get
                {
                    return _feature.Body;
                }

                set
                {
                    _feature.Body = value;
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
                    _headers = value;
                }
            }

            public string ReasonPhrase
            {
                get
                {
                    return _feature.ReasonPhrase;
                }

                set
                {
                    _feature.ReasonPhrase = value;
                }
            }

            public int StatusCode
            {
                get
                {
                    return _feature.StatusCode;
                }

                set
                {
                    _feature.StatusCode = value;
                }
            }

            public void OnSendingHeaders(Action<object> callback, object state)
            {
                Trace.TraceInformation("[HttpResponseFeature] OnSendingHeaders");
                _feature.OnSendingHeaders(callback, state);
            }
        }

        class HeaderStore : IDictionary<string, string[]>
        {
            readonly IDictionary<string, string[]> _store;
            public HeaderStore(IDictionary<string, string[]> store)
            {
                Trace.TraceInformation("[HeaderStore] constructor");
                _store = store;
            }

            public string[] this[string key]
            {
                get
                {
                    Trace.TraceInformation("[HeaderStore] get this[" + key + "]");
                    return _store[key];
                }

                set
                {
                    Trace.TraceInformation("[HeaderStore] set this[{0}]: {1}", key, value);
                    _store[key] = value;
                }
            }

            public int Count
            {
                get
                {
                    Trace.TraceInformation("[HeaderStore] get Count");
                    return _store.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    Trace.TraceInformation("[HeaderStore] get IsReadOnly");
                    return _store.IsReadOnly;
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    Trace.TraceInformation("[HeaderStore] get Keys");
                    return _store.Keys;
                }
            }

            public ICollection<string[]> Values
            {
                get
                {
                    Trace.TraceInformation("[HeaderStore] get Values");
                    return _store.Values;
                }
            }

            public void Add(KeyValuePair<string, string[]> item)
            {
                Add(item.Key, item.Value);
            }

            public void Add(string key, string[] value)
            {
                Trace.TraceInformation("[HeaderStore] Add key: {0}, value: {1}", key, value);
                _store.Add(key, value);
            }

            public void Clear()
            {
                Trace.TraceInformation("[HeaderStore] Clear");
                _store.Clear();
            }

            public bool Contains(KeyValuePair<string, string[]> item)
            {
                return ContainsKey(item.Key);
            }

            public bool ContainsKey(string key)
            {
                Trace.TraceInformation("[HeaderStore] ContainsKey " + key);
                return _store.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
            {
                Trace.TraceInformation("[HeaderStore] CopyTo at index " + arrayIndex);
                _store.CopyTo(array, arrayIndex);
            }

            public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
            {
                Trace.TraceInformation("[HeaderStore] GetEnumerator");
                return _store.GetEnumerator();
            }

            public bool Remove(KeyValuePair<string, string[]> item)
            {
                return Remove(item.Key);
            }

            public bool Remove(string key)
            {
                Trace.TraceInformation("[HeaderStore] Remove key: " + key);
                return _store.Remove(key);
            }

            public bool TryGetValue(string key, out string[] value)
            {
                Trace.TraceInformation("[HeaderStore] TryGetValue key: " + key);
                return _store.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        class HeaderDictionaryDecorator : IHeaderDictionary
        {
            private readonly IHeaderDictionary _dictionary;

            public HeaderDictionaryDecorator(IHeaderDictionary dictionary)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] constructor");
                _dictionary = dictionary;
            }
            public string this[string key]
            {
                get
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] get this[" + key + "]");
                    return _dictionary[key];
                }
                set
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] set this[{0}]: {1}", key, value);
                    _dictionary[key] = value;
                }
            }

            string[] IDictionary<string, string[]>.this[string key]
            {
                get
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] get this[" + key + "]");
                    return ((IDictionary<string, string[]>)_dictionary)[key];
                }

                set
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] set this[{0}]: {1}", key, value);
                    ((IDictionary<string, string[]>)_dictionary)[key] = value;
                }
            }

            string IReadableStringCollection.this[string key]
            {
                get
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] get this[" + key + "]");
                    return ((IReadableStringCollection)_dictionary)[key];
                }
            }

            public int Count
            {
                get
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] get Count");
                    return _dictionary.Count;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] get IsReadOnly");
                    return _dictionary.IsReadOnly;
                }
            }

            public ICollection<string> Keys
            {
                get
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] get Keys");
                    return _dictionary.Keys;
                }
            }

            public ICollection<string[]> Values
            {
                get
                {
                    Trace.TraceInformation("[HeaderDictionaryDecorator] get Values");
                    return _dictionary.Values;
                }
            }

            public void Add(KeyValuePair<string, string[]> item)
            {
                Add(item.Key, item.Value);
            }

            public void Add(string key, string[] value)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] Add key: {0}, value: {1}", key, value);
                _dictionary.Add(key, value);
            }

            public void Append(string key, string value)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] Append key: {0}, value: {1}", key, value);
                _dictionary.Append(key, value);
            }

            public void AppendCommaSeparatedValues(string key, params string[] values)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] AppendCommaSeparatedValues key: {0}, value: {1}", key, values);
                _dictionary.AppendCommaSeparatedValues(key, values);
            }

            public void AppendValues(string key, params string[] values)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] AppendValues key: {0}, value: {1}", key, values);
                _dictionary.AppendValues(key, values);
            }

            public void Clear()
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] Clear");
                _dictionary.Clear();
            }

            public bool Contains(KeyValuePair<string, string[]> item)
            {
                return ContainsKey(item.Key);
            }

            public bool ContainsKey(string key)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] ContainsKey " + key);
                return _dictionary.ContainsKey(key);
            }

            public void CopyTo(KeyValuePair<string, string[]>[] array, int arrayIndex)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] CopyTo at index " + arrayIndex);
                _dictionary.CopyTo(array, arrayIndex);
            }

            public string Get(string key)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] Get key: " + key);
                return _dictionary.Get(key);
            }

            public IList<string> GetCommaSeparatedValues(string key)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] GetCommaSeparatedValues key: " + key);
                return _dictionary.GetCommaSeparatedValues(key);
            }

            public IEnumerator<KeyValuePair<string, string[]>> GetEnumerator()
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] GetEnumerator");
                return _dictionary.GetEnumerator();
            }

            public IList<string> GetValues(string key)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] GetValues key: " + key);
                return _dictionary.GetValues(key);
            }

            public bool Remove(KeyValuePair<string, string[]> item)
            {
                return Remove(item.Key);
            }

            public bool Remove(string key)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] Remove key: " + key);
                return _dictionary.Remove(key);
            }

            public void Set(string key, string value)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] Set key: {0}, value: {1}", key, value);
                _dictionary.Set(key, value);
            }

            public void SetCommaSeparatedValues(string key, params string[] values)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] SetCommaSeparatedValues key: " + key);
                _dictionary.SetCommaSeparatedValues(key, values);
            }

            public void SetValues(string key, params string[] values)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] SetValues key: " + key);
                _dictionary.SetValues(key, values);
            }

            public bool TryGetValue(string key, out string[] value)
            {
                Trace.TraceInformation("[HeaderDictionaryDecorator] TryGetValue key: " + key);
                return _dictionary.TryGetValue(key, out value);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        class HttpResponseDecorator : HttpResponse
        {
            private readonly HttpContext _context;
            private readonly IHeaderDictionary _headers;

            public HttpResponseDecorator(HttpContext context, HttpResponse response)
            {
                Trace.TraceInformation("[HttpResponseDecorator] constructor");
                Response = response;
                _context = context;
                _headers = new HeaderDictionaryDecorator(response.Headers);
            }
            public override Stream Body
            {
                get
                {
                    return Response.Body;
                }
                set
                {
                    Response.Body = value;
                }
            }

            public override long? ContentLength
            {
                get
                {
                    return Response.ContentLength;
                }
                set
                {
                    Response.ContentLength = value;
                }
            }

            public override string ContentType
            {
                get
                {
                    return Response.ContentType;
                }
                set
                {
                    Response.ContentType = value;
                }
            }

            public override IResponseCookies Cookies
            {
                get
                {
                    return Response.Cookies;
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

            public HttpResponse Response { get; private set; }

            public override int StatusCode
            {
                get
                {
                    return Response.StatusCode;
                }
                set
                {
                    Response.StatusCode = value;
                }
            }

            public override void Challenge(AuthenticationProperties properties, IEnumerable<string> authenticationTypes)
            {
                Response.Challenge(properties, authenticationTypes);
            }

            public override void OnSendingHeaders(Action<object> callback, object state)
            {
                Response.OnSendingHeaders(callback, state);
            }

            public override void Redirect(string location, bool permanent)
            {
                Response.Redirect(location, permanent);
            }

            public override void SignIn(AuthenticationProperties properties, IEnumerable<ClaimsIdentity> identities)
            {
                Response.SignIn(properties, identities);
            }

            public override void SignOut(IEnumerable<string> authenticationTypes)
            {
                Response.SignOut(authenticationTypes);
            }
        }
        class HttpContextDecorator : HttpContext
        {
            private readonly HttpResponseDecorator _response;

            public HttpContext Context { get; private set; }

            public HttpContextDecorator(HttpContext context)
            {
                Trace.TraceInformation("[HttpContextDecorator] constructor");
                Context = context;
                _response = new HttpResponseDecorator(this, context.Response);
            }
            public override IServiceProvider ApplicationServices
            {
                get
                {
                    return Context.ApplicationServices;
                }

                set
                {
                    Context.ApplicationServices = value;
                }
            }
            public override bool IsWebSocketRequest
            {
                get
                {
                    return Context.IsWebSocketRequest;
                }
            }

            public override IDictionary<object, object> Items
            {
                get
                {
                    return Context.Items;
                }
            }

            public override HttpRequest Request
            {
                get
                {
                    return Context.Request;
                }
            }

            public override CancellationToken RequestAborted
            {
                get
                {
                    return Context.RequestAborted;
                }
            }

            public override IServiceProvider RequestServices
            {
                get
                {
                    return Context.RequestServices;
                }
                set
                {
                    Context.RequestServices = value;
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
                    return Context.Session;
                }
            }

            public override ClaimsPrincipal User
            {
                get
                {
                    return Context.User;
                }
                set
                {
                    Context.User = value;
                }
            }

            public override IList<string> WebSocketRequestedProtocols
            {
                get
                {
                    return Context.WebSocketRequestedProtocols;
                }
            }

            public override void Abort()
            {
                Context.Abort();
            }

            public override Task<WebSocket> AcceptWebSocketAsync(string subProtocol)
            {
                return Context.AcceptWebSocketAsync(subProtocol);
            }

            public override IEnumerable<AuthenticationResult> Authenticate(IEnumerable<string> authenticationTypes)
            {
                return Context.Authenticate(authenticationTypes);
            }

            public override Task<IEnumerable<AuthenticationResult>> AuthenticateAsync(IEnumerable<string> authenticationTypes)
            {
                return Context.AuthenticateAsync(authenticationTypes);
            }

            public override void Dispose()
            {
                Context.Dispose();
            }

            public override IEnumerable<AuthenticationDescription> GetAuthenticationTypes()
            {
                return Context.GetAuthenticationTypes();
            }

            public override object GetFeature(Type type)
            {
                return Context.GetFeature(type);
            }

            public override void SetFeature(Type type, object instance)
            {
                Context.SetFeature(type, instance);
            }
        }
    }
}