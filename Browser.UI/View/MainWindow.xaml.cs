using Browser.ViewModel;
using MahApps.Metro.Controls;

namespace Browser.View
{
    public partial class MainWindow : MetroWindow
    {
        private MainVm MainViewModel { get; } = new MainVm();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = MainViewModel;
        }
	}
}
