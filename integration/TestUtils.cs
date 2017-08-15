using System;
using System.IO;

namespace IntegrationTest
{
    public static class TestUtils
    {
		public static string GetPathPrefix()
		{
			var path = Directory.GetCurrentDirectory();
			if (path.EndsWith("integration", StringComparison.CurrentCultureIgnoreCase))
				path = "..";
			else if (string.IsNullOrEmpty(path))
				path = ".";
			return path + "/";
		}
	}
}
