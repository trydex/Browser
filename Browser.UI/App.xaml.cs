using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Browser.ViewModel;
using CefSharp;

namespace Browser
{
   public partial class App : Application
   {
        private Timer _timer;
        private List<MainVm> vms = new List<MainVm>();
        private void TimerCallback(object state)
        {
            foreach (var vm in vms)
            {
                Task.Run(() => { vms[0].Instance.SendText("Очень-очень супер длинный текст. Проверка связи.", 300);});
                Task.Run(() => { vms[1].Instance.SendText("Инстанс 2. Инстанс 2. Инстанс 2. Инстанс 2. Инстанс 2. Инстанс 2.", 100);});
                break;
            }
            
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var settings = new CefSettings();
            settings.CefCommandLineArgs.Add("no-sandbox", "1");

            Cef.Initialize(settings);
            base.OnStartup(e);
            
            CreateWindowWithDataContext();
            CreateWindowWithDataContext();

            var files = Directory.GetFiles(@"C:\TempFiles").ToList();
            vms[1].Instance.SetFilesForUpload(files);
            //_timer = new Timer(TimerCallback, null, 0, 20000);
        }

        private void CreateWindowWithDataContext()
        {
            var window = new View.MainWindow();
            var vm = new MainVm();
            vms.Add(vm);
            window.DataContext = vm;
            window.Show();
        }

        
    }
}
