using System;
using CefSharp;
using NewWindowOpeningEventArgs = Browser.Controls.Model.EventArgs.NewWindowOpeningEventArgs;

namespace Browser.Controls.Handlers
{
    public class LifeSpanHandler : ILifeSpanHandler
    {
        public event EventHandler<NewWindowOpeningEventArgs> NewWindowOpening;

        protected virtual void OnNewWindowOpening(NewWindowOpeningEventArgs e)
        {
            NewWindowOpening?.Invoke(this, e);
        }

        #region ILifeSpanHandler implemention

        bool ILifeSpanHandler.DoClose(IWebBrowser browserControl, IBrowser browser)
        {
            return true;
        }

        void ILifeSpanHandler.OnAfterCreated(IWebBrowser browserControl, IBrowser browser)
        {
        }

        void ILifeSpanHandler.OnBeforeClose(IWebBrowser browserControl, IBrowser browser)
        {
        }

        bool ILifeSpanHandler.OnBeforePopup(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture,
            IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings,
            ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            newBrowser = null;
            OnNewWindowOpening(new NewWindowOpeningEventArgs(browserControl, targetUrl));

            return true;
        }

        #endregion ILifeSpanHandler implemention

        
    }
}