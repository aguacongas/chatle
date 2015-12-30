using System;
using System.Collections;
using System.Collections.Generic;

namespace ChatLe.Models
{
    public class Page<T> :IEnumerable<T>
    {
        readonly IEnumerable<T> _values;
        public Page(IEnumerable<T> values, int pageIndex, int pageCount)
        {
            if (values == null)
                throw new ArgumentNullException("values");

            _values = values;
            PageIndex = pageIndex;
            PageCount = pageCount;
        }

        public int PageIndex { get; private set; }
        public int PageCount { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
    }
}