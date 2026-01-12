using ProjectApp.Desktop.ViewModels;
using System.Windows;

namespace ProjectApp.Desktop.Windows
{
    public partial class AddSzkolaWindow : Window
    {
        public AddSzkolaWindow(AddSzkolaViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
            vm.CloseRequested += (s, result) => { DialogResult = result; Close(); };
        }
    }
}