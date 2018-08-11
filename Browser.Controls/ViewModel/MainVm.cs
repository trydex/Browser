using System;
using System.IO;

namespace Browser.Controls.ViewModel
{
    public class MainVm
    {
        private static Random _random = new Random();

        public MainVm()
        {
            string cachePath = @"C:\!Trash\Cache" + _random.Next(10000);
            Instance = new Controls.ViewModel.InstanceVm(cachePath);
        }

        public Controls.ViewModel.InstanceVm Instance { get; }

        public string GetTemporaryDirectory()
        {
            while (true)
            {
                string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                if (!Directory.Exists(tempDirectory))
                {
                    return tempDirectory;
                }
            }
        }
    }
}