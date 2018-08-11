using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using Browser.Controls.Model;
using Browser.Controls.View;
using Browser.Controls.ViewModel;

namespace Launcher
{
    public partial class MainWindow : Window
    {
        private List<BrowserWindow> _browserWindows = new List<BrowserWindow>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateInstance_Click(object sender, RoutedEventArgs e)
        {
            var window = new BrowserWindow
            {
                Width = 600,
                Height = 600
            };

            window.Closing += WindowOnClosing;

            _browserWindows.Add(window);
            window.Visibility = Visibility.Hidden;
            window.Show();
            var tab = window.Instance.ActiveTab as TabVm;
            tab.WaitBrowserLoaded();
            window.Instance.SetUserAgent("Mozilla/5.0 (Macintosh; U; PPC Mac OS X; fi-fi) AppleWebKit/420+ (KHTML, like Gecko) Safari/419.3");
          
        }

        private void WindowOnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            (sender as Window).Hide();
        }

        private void Navigate_Click(object sender, RoutedEventArgs e)
        {
            foreach (var window in _browserWindows)
            {
                window.Instance.NewTab();
                window.Instance.ActiveTab.Navigate("https://ya.ru", "https://trydex.ru");
            }
        }

        private void SendText_Click(object sender, RoutedEventArgs e)
        {
            foreach (var window in _browserWindows)
            {
                Task.Run(() =>
                {
                    window.Instance.ActiveTab.WaitDownloading();
                    window.Instance.ActiveTab.SendText(
                        @"Tarantool — это не просто база данных. ",
                        new Range(200, 400));
                });
            }
        }

        private void ShowInstances_Click(object sender, RoutedEventArgs e)
        {
            foreach (var window in _browserWindows)
            {
                window.Show();
            }
        }

        private void HideInstances_Click(object sender, RoutedEventArgs e)
        {
            foreach (var window in _browserWindows)
            {
                window.Hide();
            }
        }

        private async void FindElement_Click(object sender, RoutedEventArgs e)
        {
            var tasks = new List<Task>();

            foreach (var window in _browserWindows)
            {
                var tab = window.Instance.ActiveTab;
                var elements = await tab.FindElementsByXPathAsync("//*[@id='index_login_form']//input")
                    .ConfigureAwait(false);
                var form = await tab.FindElementByXPathAsync("//*[@id='index_login_form']").ConfigureAwait(false);
                var children = await form.FindChildrenByXPathAsync("//input").ConfigureAwait(false);

                MessageBox.Show(elements.Count.ToString(), "", MessageBoxButton.OK);
                //email.DrawToBitmap();
                var task = Task.Run(async () =>
                {
                    var tab = window.Instance.ActiveTab;

                    tab.Navigate("vk.com");
                    tab.WaitDownloading();

                    var email = await tab.FindElementByXPathAsync("//*[@id='index_email']").ConfigureAwait(false);
                   // email.DrawToBitmap();
                    await email.RiseEventAsync("focus").ConfigureAwait(false);
                    tab.SendText("login", new Range(100, 500));

                    var password = await tab.FindElementByXPathAsync("//*[@id='index_pass']").ConfigureAwait(false);
                    await password.RiseEventAsync("focus").ConfigureAwait(false);
                    tab.SendText("password", new Range(100, 500));

                    var enterButton = await tab.FindElementByXPathAsync("//*[@id='index_login_button']")
                        .ConfigureAwait(false);
                    await enterButton.ScrollIntoViewAsync().ConfigureAwait(false);
                    await Task.Delay(2000).ConfigureAwait(false);
                    await enterButton.RiseEventAsync("click").ConfigureAwait(false);

                });
                tasks.Add(task);
            }

            await Task.WhenAll(tasks).ConfigureAwait(false);
        }
    }
}