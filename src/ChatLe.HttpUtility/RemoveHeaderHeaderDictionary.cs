using Microsoft.AspNet.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ChatLe.HttpUtility
{
    /// <summary>
    /// <see cref="IHeaderDictionary"/> decorator to remove unwanted HTTP response header added by other middleware
    /// </summary>
    public class RemoveHeaderHeaderDictionary : IHeaderDictionary
    {
        readonly IHeaderDictionary _parent;
        readonly IEnumerable<string> _headersToRemove;
        /// <summary>
        /// Create an instance of <see cref="RemoveHeaderHeaderDictionary"/>
        /// </summary>
        /// <param name="parent">the <see cref="IHeaderDictionary"/> to decorate</param>
        /// <param name="headersToRemove">a list of unwanted header</param>
        public RemoveHeaderHeaderDictionary(IHeaderDictionary parent, IEnumerable<string> headersToRemove)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");
            if (headersToRemove == null)
                throw new ArgumentNullException("headersToRemove");
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
}