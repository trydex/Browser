using Browser.Controls.Interfaces;
using MahApps.Metro.Controls;
using Browser.Controls.ViewModel;

namespace Browser.Controls.View
{
    public partial class BrowserWindow : MetroWindow
    {
        private MainVm MainViewModel { get; } = new MainVm();
        
        public BrowserWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel;
        }

        public IInstance Instance => MainViewModel.Instance;

    }
}
