using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Browser.Controls.View
{
    public partial class TabControlPanel : UserControl
    {
        public TabControlPanel()
        {
            InitializeComponent();
        }

		private void txtBoxAddress_PreviewKeyDown(object sender, KeyEventArgs e)
		{
		    if (e.Key == Key.Enter)
		    {
		        var binding = BindingOperations.GetBindingExpression(txtBoxAddress, TextBox.TextProperty);

		        binding?.UpdateSource();
		        Keyboard.ClearFocus();
            }
		}

        private void TxtBoxAddress_OnLoaded(object sender, RoutedEventArgs e)
        {
            txtBoxAddress.Focus();
        }
    }
}
