using ChatLe.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using Xunit;

namespace ChatLe.Repository.Test
{
    public class PageTest
    {
        [Fact]
        public void Constructor_should_throw_ArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Page<string>(null, 0, 0));
        }

        [Fact]
        public void Page_Implement_IEnumerable()
        {
            ((IEnumerable)new Page<string>(new List<string>(), 0, 0)).GetEnumerator();
        }
    }
}
