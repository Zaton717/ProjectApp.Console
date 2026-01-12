using ProjectApp.Wpf.Infrastructure;
using System;

namespace ProjectApp.Desktop.ViewModels
{
    public class EditSzkolaViewModel : BaseViewModel
    {
        private string _nazwa = "";
        private string _adres = "";

        public string Nazwa { get => _nazwa; set => Set(ref _nazwa, value); }
        public string Adres { get => _adres; set => Set(ref _adres, value); }

        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event EventHandler<bool>? CloseRequested;

        public EditSzkolaViewModel()
        {
            ConfirmCommand = new RelayCommand(() => CloseRequested?.Invoke(this, true));
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, false));
        }

        public void UstawDaneStartowe(string nazwa, string adres)
        {
            Nazwa = nazwa;
            Adres = adres;
        }
    }
}