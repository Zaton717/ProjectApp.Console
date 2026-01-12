using ProjectApp.Desktop.ViewModels;
using ProjectApp.Desktop.Windows;
using System;
using System.Windows;

namespace ProjectApp.Desktop.Services
{
    public interface ISzkolaDialogService
    {
        bool Confirm(string message, string title);
        (string, string, string, string)? PromptAddSzkola();
        (string, string)? PromptEditSzkola(string currentName, string currentAdres);
    }

    public class SzkolaDialogService : ISzkolaDialogService
    {
        private readonly Func<AddSzkolaWindow> _addFactory;
        private readonly Func<EditSzkolaWindow> _editFactory;

        public SzkolaDialogService(Func<AddSzkolaWindow> addFactory, Func<EditSzkolaWindow> editFactory)
        {
            _addFactory = addFactory;
            _editFactory = editFactory;
        }

        public bool Confirm(string message, string title)
        {
            return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public (string, string, string, string)? PromptAddSzkola()
        {
            var window = _addFactory();
            var vm = (AddSzkolaViewModel)window.DataContext;

            if (window.ShowDialog() == true)
            {
                return (vm.Nazwa, vm.Adres, vm.MenadzerImie, vm.MenadzerNazwisko);
            }
            return null;
        }

        public (string, string)? PromptEditSzkola(string currentName, string currentAdres)
        {
            var window = _editFactory();
            var vm = (EditSzkolaViewModel)window.DataContext;
            vm.UstawDaneStartowe(currentName, currentAdres);

            if (window.ShowDialog() == true)
            {
                return (vm.Nazwa, vm.Adres);
            }
            return null;
        }
    }
}