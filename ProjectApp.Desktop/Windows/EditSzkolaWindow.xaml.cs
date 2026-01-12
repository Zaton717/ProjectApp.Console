using ProjectApp.Desktop.ViewModels;
using System.Windows;

namespace ProjectApp.Desktop.Windows
{
    public partial class EditSzkolaWindow : Window
    {
        public EditSzkolaWindow(EditSzkolaViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.CloseRequested += (s, result) => { DialogResult = result; Close(); };
        }
    }
}