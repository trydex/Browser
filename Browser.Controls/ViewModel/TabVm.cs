using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Browser.Controls.Interfaces;
using Browser.Controls.Model;
using CefSharp;
using CefSharp.Wpf;
using GalaSoft.MvvmLight;
using ApplicationUtils = Browser.Controls.Resources.Utils.ApplicationUtils;
using ITab = Browser.Controls.Interfaces.ITab;
using NewWindowOpeningEventArgs = Browser.Controls.Model.EventArgs.NewWindowOpeningEventArgs;

namespace Browser.Controls.ViewModel
{
    public class TabVm : ViewModelBase, ITab
    {
        #region fields

        private string _title;
        private string _address;
        private bool _isBusy;
        private IWpfWebBrowser _webBrowser;

        #endregion fields

        public TabVm(Controls.ViewModel.InstanceVm instance)
        {
            Instance = instance;
        }

        #region properties

        public Controls.ViewModel.InstanceVm Instance { get; }

        public string Title
        {
            get => _title;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    Set(ref _title, value);
                }
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                _address = value;
                RaisePropertyChanged();
                IsBusy = true;
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => Set(ref _isBusy, value);
        }

        public IWpfWebBrowser WebBrowser
        {
            get => _webBrowser;
            set
            {
                Set(ref _webBrowser, value);
                SetHandlers();
                SubscribeOnWebBrowserEvents();
            }
        }

        private void SetHandlers()
        {
            WebBrowser.RequestHandler = Instance.RequestHandler;
        }

        #endregion properties

        #region methods

        private void UpdateProperties()
        {
            RaisePropertyChanged(nameof(Address));
        }

        private void SubscribeOnWebBrowserEvents()
        {
            WebBrowser.FrameLoadStart += (sender, args) => UpdateProperties();
            WebBrowser.FrameLoadEnd += (sender, args) =>
            {
                IsBusy = args.Browser.IsLoading;
                UpdateProperties();
            };

            WebBrowser.LoadingStateChanged += (sender, args) =>
            {
                IsBusy = args.IsLoading;
                UpdateProperties();
            };
        }
        
        public void Navigate(string url, string referer = "")
        {
            WaitBrowserLoaded();
            SetReferer(referer);

            Address = url;
        }

        private void SetReferer(string referer)
        {
            Instance.RequestHandler.Referer = referer;

            WebBrowser.FrameLoadEnd += ClearReferer;

            void ClearReferer(object sender, FrameLoadEndEventArgs e)
            {
                Instance.RequestHandler.Referer = null;
                WebBrowser.FrameLoadEnd -= ClearReferer;
            };
        }

        public void SetActive()
        {
            Instance.ActiveTab = this;
        }

        public void Close()
        {
            Instance.Tabs.Remove(this);

            if (Instance.Tabs.Count == 0)
            {
                Instance.NewTab();
            }

            WebBrowser.GetBrowser().CloseBrowser(true);
        }

        public void WaitBrowserLoaded()
        {
            while (WebBrowser is null)
            {   
                ApplicationUtils.DoEvents();
            }
        }

        public void WaitDownloading()
        {
            while (IsBusy || IsBrowserLoading())
            {
                ApplicationUtils.DoEvents();
            }
        }

        private bool IsBrowserLoading()
        {
            bool isLoading = false;
            WebBrowser.Dispatcher.Invoke(() => isLoading = WebBrowser.IsLoading);

            return isLoading;
        }

        public void SendText(string text, Model.Range latency)
        {
            
            var r = new Random();
            
            var tab = this;

            var task = Task.Run(() => //TODO: Оборачивать в Task или нет? 
            {
                foreach (char c in text)
                {
                    int threadLatency = r.Next(latency.From, latency.To);
                    var keyEvent = new KeyEvent
                    {
                        WindowsKeyCode = c,
                        FocusOnEditableField = true,
                        IsSystemKey = false,
                        Type = KeyEventType.Char
                    };

                    tab.WebBrowser.GetBrowser().GetHost().SendKeyEvent(keyEvent);

                    System.Threading.Thread
                        .Sleep(threadLatency); 
                }
            });

            while (!task.IsCompleted && !task.IsFaulted)
            {
                ApplicationUtils.DoEvents();
            }
        }

        public async Task<IWebElement> FindElementByXPathAsync(string xpath)
        {
            var element = new WebElement(WebBrowser, xpath);
            await element.FindAsync().ConfigureAwait(false);
            
            return element;
        }

        public Task<List<IWebElement>> FindElementsByXPathAsync(string xpath)
        {
            //При поиске детей используется родительский XPath, поэтому он опускается
            var element = new WebElement(WebBrowser, "");
            return element.FindChildrenByXPathAsync(xpath);
        }

        #endregion methods

        #region event handlers

        public Action<NewWindowOpeningEventArgs> NewWindowOpeningHandler => NewWindowOpeningHandlerImpl;

        private void NewWindowOpeningHandlerImpl(NewWindowOpeningEventArgs e)
        {
            var tab = Instance.NewTab();
            tab.Navigate(e.TargetUrl);
        }

        #endregion
    }
}