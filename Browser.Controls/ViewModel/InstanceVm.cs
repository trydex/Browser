using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Browser.Controls.Handlers;
using CefSharp;
using Dragablz;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using IInstance = Browser.Controls.Interfaces.IInstance;
using ITab = Browser.Controls.Interfaces.ITab;
using ProxyInfo = Browser.Controls.Model.ProxyInfo;


namespace Browser.Controls.ViewModel
{
    public class InstanceVm : ViewModelBase, IInstance
    {
        private bool _canCommandExecute = true; // Чтобы команда не срабатывала много раз при зажатии горячих клавиш.
        private bool _isNeedCancelFileDialog;
        private ITab _activeTab;
        private RelayCommand _newTabCommand;
        private RelayCommand _closeActiveTabCommand;
        private RelayCommand _resetCommandsCanExecuteCommand;

        private List<string> _uploadFilePaths;

        public InstanceVm(string cachePath)
        {
            CachePath = cachePath;
            NewTab();
        }

        #region dragablz properties

        public Func<ITab> Factory => () =>
        {
            var t = NewTab();
            Tabs.Remove(t);
            return t;
        };

        public ItemActionCallback ClosingTabItemHandler => ClosingTabItemHandlerImpl;

        private static void ClosingTabItemHandlerImpl(ItemActionCallbackArgs<TabablzControl> args)
        {
            if (args.DragablzItem.DataContext is TabVm tab)
            {
                tab.Close();
            }
        }

        #endregion

        #region properties

        public string CachePath { get; }

        public ObservableCollection<ITab> Tabs { get; } = new ObservableCollection<ITab>();

        public ITab ActiveTab
        {
            get => _activeTab;
            set => Set(ref _activeTab, value);
        }

        public bool IsNeedCancelFileDialog
        {
            get => _isNeedCancelFileDialog;
            set => Set(ref _isNeedCancelFileDialog, value);
        }

        public List<string> UploadFilePaths
        {
            get => _uploadFilePaths;
            set => Set(ref _uploadFilePaths, value);
        }

        public RequestHandler RequestHandler { get; } = new RequestHandler();

        #endregion properties

        #region commands

        public RelayCommand NewTabCommand => _newTabCommand ??
                                             (_newTabCommand = new RelayCommand(() =>
                                             {
                                                 NewTab();
                                                 _canCommandExecute = false;
                                             }, () => _canCommandExecute));

        public RelayCommand CloseActiveTabCommand => _closeActiveTabCommand ??
                                                     (_closeActiveTabCommand = new RelayCommand(() =>
                                                     {
                                                         ActiveTab.Close();
                                                         _canCommandExecute = false;
                                                     }, () => _canCommandExecute));

        public RelayCommand ResetCommandsCanExecuteCommand => _resetCommandsCanExecuteCommand ??
                                                              (_resetCommandsCanExecuteCommand =
                                                                  new RelayCommand(ResetCommandsCanExecute));

        public RelayCommand TempCommand => new RelayCommand(() => SetProxy(null));
        public RelayCommand Temp2Command => new RelayCommand(ClearProxy);

        private void ResetCommandsCanExecute()
        {
            _canCommandExecute = true;
            NewTabCommand.RaiseCanExecuteChanged();
            CloseActiveTabCommand.RaiseCanExecuteChanged();
        }

        #endregion commands

        #region  methods

        public ITab NewTab(string name = "New Tab")
        {
            var tab = new TabVm(this) {Title = name};
            Tabs.Add(tab);
            tab.SetActive();

            return tab;
        }

        public bool ClearCache()
        {
            try
            {
                if (Directory.Exists(CachePath))
                {
                    Directory.Delete(CachePath, true);
                    return true;
                }
            }
            catch
            {
            }

            return false;
        }

        public void SetProxy(ProxyInfo proxy)
        {
            proxy = new ProxyInfo("http", "185.147.130.83", 65233, "k0de", "B3z5DoD");

            SetProxyForHost(proxy);

            RequestHandler.InstanceProxy = proxy;
        }

        private void SetProxyForHost(ProxyInfo proxy)
        {
            if ( proxy is null)
                return;

            //TODO: Если прокси кривой

            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var tab = (TabVm)ActiveTab;
                var rc = tab.WebBrowser.GetBrowser().GetHost().RequestContext;
                
                var v = new Dictionary<string, object>
                {
                    ["mode"] = "fixed_servers",
                    ["server"] = $"{proxy.Scheme}://{proxy.Ip}:{proxy.Port}",
                    ["webrtc.ip_handling_policy"] = "disable_non_proxied_udp" //Выключает определение оригинального IP через WebRtc
                };

                bool success = rc.SetPreference("proxy", v, out string error);
            });
        }

        public void SetUserAgent(string userAgentString)
        {
            if (string.IsNullOrWhiteSpace(userAgentString))
                return;
            RequestHandler.UserAgent = userAgentString;
        }

        public void ClearProxy()
        {
            RequestHandler.InstanceProxy = null;

            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                var tab = (TabVm)ActiveTab;
                var rc = tab.WebBrowser.GetBrowser().GetHost().RequestContext;
                var v = new Dictionary<string, object>
                {
                    ["mode"] = "direct"
                };

                bool success = rc.SetPreference("proxy", v, out string error);
            });
        }

       public void SetFileUploadPolicy(bool cancelFileDialog)
        {
            IsNeedCancelFileDialog = cancelFileDialog;
        }

        public void SetFilesForUpload(List<string> filePaths)
        {
            UploadFilePaths = filePaths;
        }

        #endregion methods
    }
}