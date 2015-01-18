using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace ChatLe.Hosting.FastCGI
{
    public interface IListener<T> : IListener where T : EndPoint
    {
        void Start(T endpoint);
    }

    public interface IListener : IDisposable
    {
        IListernerConfiguration Configuration { get; }
        Func<object, Task> App { get; }
    }
}