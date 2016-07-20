using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTest
{
    public class ChatLeFixture: IDisposable
    {
		Process _serverProcess;

		public ChatLeFixture()
		{
			var processStartInfo = new ProcessStartInfo()
			{
				Arguments = "-p " + TestUtils.GetPathPrefix() + "src/chatle web",
				CreateNoWindow = true,
				FileName = "dnx",
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false
			};

			_serverProcess = Process.Start(processStartInfo);
			bool ready = false;
			_serverProcess.OutputDataReceived += (o, args) =>
			{
				var data = args.Data;
				if (data == null)
					return;
				Trace.WriteLine(data);
				Console.WriteLine(data);
				if (data.StartsWith("Application started."))
					ready = true;
			};

			_serverProcess.ErrorDataReceived += (o, args) =>
			{
				var data = args.Data;
				if (data == null)
					return;

				Trace.WriteLine(data);
				Console.WriteLine(data);
				ready = true;
			};

			_serverProcess.BeginErrorReadLine();
			_serverProcess.BeginOutputReadLine();
			
			while(!ready)
			{
				Thread.Sleep(50);
			}
		}

		public void Dispose()
		{
			Dispose(false);
		}

		protected virtual void Dispose(bool disposed)
		{
			if(!disposed)
			{
				if(_serverProcess != null)
				{
					_serverProcess.Kill();
					_serverProcess.Close();
					_serverProcess = null;
				}
			}
		}
    }

	[CollectionDefinition(ChatLeCollectionFixture.Definition)]
	public class ChatLeCollectionFixture: ICollectionFixture<ChatLeFixture>
	{
		public const string Definition = "ChatLe server";
	}
}
