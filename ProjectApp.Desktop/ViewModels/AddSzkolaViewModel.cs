using ProjectApp.Wpf.Infrastructure;
using System;

namespace ProjectApp.Desktop.ViewModels
{
    public class AddSzkolaViewModel : BaseViewModel
    {
        private string _nazwa = "";
        private string _adres = "";
        private string _menadzerImie = "";
        private string _menadzerNazwisko = "";

        public string Nazwa { get => _nazwa; set => Set(ref _nazwa, value); }
        public string Adres { get => _adres; set => Set(ref _adres, value); }
        public string MenadzerImie { get => _menadzerImie; set => Set(ref _menadzerImie, value); }
        public string MenadzerNazwisko { get => _menadzerNazwisko; set => Set(ref _menadzerNazwisko, value); }

        public RelayCommand ConfirmCommand { get; }
        public RelayCommand CancelCommand { get; }

        public event EventHandler<bool>? CloseRequested;

        public AddSzkolaViewModel()
        {
            ConfirmCommand = new RelayCommand(() => CloseRequested?.Invoke(this, true));
            CancelCommand = new RelayCommand(() => CloseRequested?.Invoke(this, false));
        }
    }
}