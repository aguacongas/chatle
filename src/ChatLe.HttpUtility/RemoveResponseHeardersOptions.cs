using System;
using System.Collections.Generic;

namespace ChatLe.HttpUtility
{
    public class RemoveResponseHeardersOptions
    {
        public IEnumerable<string> Headers { get; set; }=new List<string>() { "Server", "X-ProvidedBy" };
    }
}