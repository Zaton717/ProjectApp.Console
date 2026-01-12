using ProjectApp.Desktop.Services;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Wpf.Infrastructure;
using System;
using System.Collections.ObjectModel;

namespace ProjectApp.Desktop.ViewModels
{
    public class SzkolyViewModel : BaseViewModel
    {
        private readonly ISzkolaService _szkolaSvc;
        private readonly ISzkolaDialogService _dialogs;
        private readonly INotificationService _notify;

        public ObservableCollection<SzkolaListItemVm> Szkoly { get; } = new();

        private SzkolaListItemVm? _selectedSzkola;
        public SzkolaListItemVm? SelectedSzkola
        {
            get => _selectedSzkola;
            set
            {
                if (Set(ref _selectedSzkola, value))
                {
                    EditCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public RelayCommand LoadCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public SzkolyViewModel(ISzkolaService s, ISzkolaDialogService d, INotificationService n)
        {
            _szkolaSvc = s; _dialogs = d; _notify = n;

            LoadCommand = new RelayCommand(LoadData);
            AddCommand = new RelayCommand(AddSzkola);
            EditCommand = new RelayCommand(EditSzkola, () => SelectedSzkola != null);
            DeleteCommand = new RelayCommand(DeleteSzkola, () => SelectedSzkola != null);
        }

        private void LoadData()
        {
            Szkoly.Clear();
            var dane = _szkolaSvc.GetAll();
            foreach (var s in dane)
            {
                Szkoly.Add(new SzkolaListItemVm
                {
                    Id = s.Id,
                    Nazwa = s.Nazwa,
                    Adres = s.Adres,
                    Menadzer = s.Menadzer?.PelneImie ?? "Brak",
                    CzyAktywna = s.CzyAktywna
                });
            }
        }

        private void AddSzkola()
        {
            var result = _dialogs.PromptAddSzkola();
            if (result == null) return;

            var (n, a, im, nm) = result.Value;
            try
            {
                // Zakładam, że masz metodę Create(string, string, string, string)
                _szkolaSvc.Create(n, a, im, nm);
                LoadData();
                _notify.Info($"Dodano szkołę: {n}", "Sukces");
            }
            catch (Exception ex) { _notify.Error(ex.Message, "Błąd dodawania"); }
        }

        private void EditSzkola()
        {
            if (SelectedSzkola == null) return;

            var result = _dialogs.PromptEditSzkola(SelectedSzkola.Nazwa, SelectedSzkola.Adres);
            if (result == null) return;

            var (newNazwa, newAdres) = result.Value;
            try
            {
                // Tu korzystamy z nowej metody Update w serwisie
                _szkolaSvc.Update(SelectedSzkola.Id, newNazwa, newAdres);
                LoadData();
                _notify.Info("Zaktualizowano dane.", "Sukces");
            }
            catch (Exception ex) { _notify.Error(ex.Message, "Błąd edycji"); }
        }

        private void DeleteSzkola()
        {
            if (SelectedSzkola == null) return;

            if (_dialogs.Confirm($"Czy na pewno usunąć szkołę {SelectedSzkola.Nazwa}?", "Usuwanie"))
            {
                try
                {
                    _szkolaSvc.Delete(SelectedSzkola.Id);
                    LoadData();
                    _notify.Info("Szkoła usunięta.", "Info");
                }
                catch (Exception ex) { _notify.Error(ex.Message, "Błąd usuwania"); }
            }
        }
    }
}