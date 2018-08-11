using CefSharp;

namespace Browser.Controls.Model.EventArgs
{
    public class NewWindowOpeningEventArgs : System.EventArgs
    {
        public NewWindowOpeningEventArgs(IWebBrowser browserControl, string targetUrl)
        {
            BrowserControl = browserControl;
            TargetUrl = targetUrl;
        }

        public IWebBrowser BrowserControl { get; }
        public string TargetUrl { get; }
    }
}