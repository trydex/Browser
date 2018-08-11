using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Browser.Controls.View;

namespace TestLauncher
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var window = new MainWindow();
            window.ShowDialog();

            Console.ReadKey();
        }
    }
}
