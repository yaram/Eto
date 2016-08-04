using Eto.CustomControls;
using Eto.Drawing;
using Eto.Forms;
using Eto.Wpf.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cs = global::CefSharp;

namespace Eto.Wpf.CefSharp
{
	public class CefSharpWebViewHandler : WpfControl<cs.Wpf.ChromiumWebBrowser, WebView, WebView.ICallback>, WebView.IHandler
	{
		public CefSharpWebViewHandler()
		{
			Control = new cs.Wpf.ChromiumWebBrowser();
		}
		public override System.Windows.Size GetPreferredSize(System.Windows.Size constraint)
		{
			return base.GetPreferredSize(new System.Windows.Size(100, 100));
		}

		protected override bool PreventUserResize { get { return true; } }

		public bool BrowserContextMenuEnabled { get; set; }

		public bool CanGoBack => Control.CanGoBack;

		public bool CanGoForward => Control.CanGoForward;

		public string DocumentTitle => Control.Title;

		public Uri Url
		{
			get
			{
				Uri url;
				if (Uri.TryCreate(Control.Address, UriKind.RelativeOrAbsolute, out url))
					return url;
				return null;
			}

			set
			{
				Control.Load(value?.AbsoluteUri);
			}
		}

		public string ExecuteScript(string script)
		{
			Control.GetBrowser()?.MainFrame.ExecuteJavaScriptAsync(script);
			return null;
		}

		public void GoBack()
		{
			Control.BackCommand.Execute(null);
		}

		public void GoForward()
		{
			Control.ForwardCommand.Execute(null);
		}

		HttpServer server;
		public void LoadHtml(string html, Uri baseUri)
		{
			if (server == null)
				server = new HttpServer();
			server.SetHtml(html, baseUri != null ? baseUri.LocalPath : null);
			if (Control.Address == server.Url.AbsoluteUri)
				Control.ReloadCommand.Execute(null);
			else
				Control.Address = server.Url.AbsoluteUri;
		}

		public void Reload()
		{
			Control.ReloadCommand.Execute(null);
		}

		public void ShowPrintDialog()
		{
			Control.PrintCommand.Execute(null);
		}

		public void Stop()
		{
			Control.StopCommand.Execute(null);
		}

		public override void AttachEvent(string id)
		{
			switch (id)
			{
				case WebView.DocumentLoadedEvent:
					Control.LoadingStateChanged += (sender, e) =>
					{
						Application.Instance.AsyncInvoke(() =>
						{
							if (!e.IsLoading && !string.IsNullOrEmpty(e.Browser.MainFrame.Url))
								Callback.OnDocumentLoaded(Widget, new WebViewLoadedEventArgs(new Uri(e.Browser.MainFrame.Url)));
						});
					};
					break;
				case WebView.DocumentLoadingEvent:
					Control.LoadingStateChanged += (sender, e) =>
					{
						Application.Instance.AsyncInvoke(() =>
						{
							if (e.IsLoading && !string.IsNullOrEmpty(e.Browser.MainFrame.Url))
								Callback.OnDocumentLoading(Widget, new WebViewLoadingEventArgs(new Uri(e.Browser.MainFrame.Url), true));
						});
					};
					break;
				case WebView.DocumentTitleChangedEvent:
					Control.TitleChanged += (sender, e) => Callback.OnDocumentTitleChanged(Widget, new WebViewTitleEventArgs(e.NewValue as string));
					break;
				case WebView.OpenNewWindowEvent:
				case WebView.NavigatedEvent:
					break;
				default:
					base.AttachEvent(id);
					break;
			}
		}
	}
}
