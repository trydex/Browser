using System;
using System.Windows.Input;
using System.Windows.Threading;
using Browser.Interfaces;
using Browser.Resources.EventArgs;
using Browser.Resources.Utils;
using CefSharp.Wpf;
using GalaSoft.MvvmLight;

namespace Browser.ViewModel
{
    public class TabVm : ViewModelBase, ITab
    {
        #region fields

        private string _title;
        private string _address;
        private bool _isBusy;
        private IWpfWebBrowser _webBrowser;

        #endregion fields

        public TabVm(InstanceVm instance)
        {
            Instance = instance;
        }

        #region properties

        public InstanceVm Instance { get; }

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
            private set => Set(ref _isBusy, value);
        }

        public IWpfWebBrowser WebBrowser
        {
            get => _webBrowser;
            set
            {
                Set(ref _webBrowser, value);
                SubscribeOnWebBrowserEvents();
            }
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
            WebBrowser.FrameLoadEnd += (sender, args) => UpdateProperties();

            WebBrowser.LoadingStateChanged += (sender, args) =>
            {
                IsBusy = args.IsLoading;
                UpdateProperties();
            };
        }

        public void Navigate(string url, string referrer = "")
        {
            if (WebBrowser is null)
            {
                ApplicationUtils.DoEvents();
            }
            
            WebBrowser.Load(url);
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

        public void WaitDownloading()
        {
            while (IsBusy)
            {
                ApplicationUtils.DoEvents();
            }
        }

        public Action<NewWindowOpeningEventArgs> NewWindowOpeningHandler => NewWindowOpeningHandlerImpl;

        private void NewWindowOpeningHandlerImpl(NewWindowOpeningEventArgs e)
        {
            var tab = Instance.NewTab();
            tab.Navigate(e.TargetUrl);
        }

        #endregion methods
    }
}