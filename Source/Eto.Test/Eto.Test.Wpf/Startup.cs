using Eto.Wpf.Forms.Controls;
using System;
using System.Reflection;

namespace Eto.Test.Wpf
{
	class Startup
	{
		[STAThread]
		static void Main(string[] args)
		{
			var platform = new Eto.Wpf.Platform();
			//platform.LoadAssembly(AssemblyName.GetAssemblyName("Eto.Wpf.CefSharp.dll"));
			//platform.LoadAssembly(typeof(Eto.Wpf.CefSharp.PlatformExtension).Assembly);

			var app = new TestApplication(platform);
			app.TestAssemblies.Add(typeof(Startup).Assembly);
			app.Run();
		}
	}
}

