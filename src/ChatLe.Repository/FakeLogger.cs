﻿using Microsoft.Extensions.Logging;
using System;

namespace ChatLe.Models
{
    internal class FakeLogger : ILogger
    {
        class FakeState : IDisposable
        {
            #region IDisposable Support
            private bool disposedValue = false;

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    disposedValue = true;
                }
            }

            // This code added to correctly implement the disposable pattern.
            public void Dispose()
            {
                Dispose(true);
            }
            #endregion

        }

        private readonly static FakeState _state = new FakeState();
        public IDisposable BeginScopeImpl(object state)
        {
            return _state;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
