using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationTest
{
    public static class TestUtils
    {
		public static string GetPathPrefix()
		{
			var path = Environment.CurrentDirectory;
			if (Environment.CurrentDirectory.EndsWith("integration", StringComparison.InvariantCultureIgnoreCase))
				path = "..";
			else if (string.IsNullOrEmpty(path))				
				path = ".";
			return path + "/";
		}
	}
}
