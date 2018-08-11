using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using Browser.Interfaces;
using Browser.Model;
using CefSharp;
using Dragablz;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;


namespace Browser.ViewModel
{
    public class InstanceVm : ViewModelBase, IInstance
    {
        private bool _canCommandExecute = true; // Чтобы команда не срабатывала много раз при зажатии горячих клавиш.
        private bool _isProxySetted;

        private ITab _activeTab;
        private RelayCommand _newTabCommand;
        private RelayCommand _closeActiveTabCommand;
        private RelayCommand _resetCommandsCanExecuteCommand;

        private ProxyInfo _instanceProxy;

        private bool _isNeedCancelFileDialog;
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

            if (_isProxySetted)
            {
                SetProxyForTab(tab, _instanceProxy);
            }

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

            foreach (var tab in Tabs)
            {
                SetProxyForTab((TabVm) tab, proxy);
            }

            _isProxySetted = true;
            _instanceProxy = proxy;
        }

        private void SetProxyForTab(TabVm tab, ProxyInfo proxy)
        {
            if (tab is null || proxy is null)
                return;

            //TODO: Если прокси кривой

            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                tab.WebBrowser.RequestHandler = new RequestHandler(proxy);
                var rc = tab.WebBrowser.GetBrowser().GetHost().RequestContext;

                var v = new Dictionary<string, object>
                {
                    ["mode"] = "fixed_servers",
                    ["server"] = $"{proxy.Scheme}://{proxy.Ip}:{proxy.Port}",
                    ["webrtc.ip_handling_policy"] =
                    "disable_non_proxied_udp" //Выключает определение оригинального IP через WebRtc
                };

                bool success = rc.SetPreference("proxy", v, out string error);
            });
        }

        public void ClearProxy()
        {
            _isProxySetted = false;
            _instanceProxy = null;

            Cef.UIThreadTaskFactory.StartNew(delegate
            {
                foreach (TabVm tab in Tabs)
                {
                    var rc = tab.WebBrowser.GetBrowser().GetHost().RequestContext;
                    var v = new Dictionary<string, object>
                    {
                        ["mode"] = "direct",
                    };

                    bool success = rc.SetPreference("proxy", v, out string error);
                }
            });
        }

        public void SendText(string text, int latency)
        {
            if (ActiveTab is null)
                return;

            var tab = (TabVm) ActiveTab;

            foreach (char c in text)
            {
                var keyEvent = new KeyEvent
                {
                    WindowsKeyCode = c,
                    FocusOnEditableField = true,
                    IsSystemKey = false,
                    Type = KeyEventType.Char
                };

                tab.WebBrowser.GetBrowser().GetHost().SendKeyEvent(keyEvent);

                System.Threading.Thread.Sleep(latency); // TODO: Сделать задержку рандомной? Сделать возможно отправлять клавиши? Типа {ENTER} и т.п.
            }
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