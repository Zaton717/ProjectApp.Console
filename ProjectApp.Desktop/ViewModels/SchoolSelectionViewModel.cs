using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Desktop.Services; // Potrzebne do Dialogów
using ProjectApp.Wpf.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjectApp.Desktop.ViewModels
{
    public class SchoolSelectionViewModel : BaseViewModel
    {
        private readonly ISzkolaService _szkolaSvc;
        private readonly ISzkolaDialogService _dialogs;
        private readonly INotificationService _notify;

        // Akcje nawigacji
        public Action<Guid, string>? RequestOpenManager;
        public Action<Guid>? RequestOpenOwner;
        public Action<Guid>? RequestOpenInstruktor;
        public Action<Guid>? RequestOpenKursant;

        public ObservableCollection<Szkola> Szkoly { get; } = new();

        private Szkola? _selectedSzkola;
        public Szkola? SelectedSzkola
        {
            get => _selectedSzkola;
            set
            {
                if (Set(ref _selectedSzkola, value))
                {
                    // Odświeżamy dostępność przycisków
                    OpenManagerCommand.RaiseCanExecuteChanged();
                    OpenOwnerCommand.RaiseCanExecuteChanged();
                    OpenInstruktorCommand.RaiseCanExecuteChanged();
                    OpenKursantCommand.RaiseCanExecuteChanged();

                    // CRUD
                    EditCommand.RaiseCanExecuteChanged();
                    DeleteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // --- KOMENDY NAWIGACJI ---
        public RelayCommand OpenManagerCommand { get; }
        public RelayCommand OpenOwnerCommand { get; }
        public RelayCommand OpenInstruktorCommand { get; }
        public RelayCommand OpenKursantCommand { get; }

        // --- KOMENDY CRUD ---
        public RelayCommand AddCommand { get; }
        public RelayCommand EditCommand { get; }
        public RelayCommand DeleteCommand { get; }

        public SchoolSelectionViewModel(ISzkolaService s, ISzkolaDialogService dialogs, INotificationService notify)
        {
            _szkolaSvc = s;
            _dialogs = dialogs;
            _notify = notify;

            RefreshList();

            // Nawigacja
            OpenManagerCommand = new RelayCommand(() => RequestOpenManager?.Invoke(SelectedSzkola!.Id, SelectedSzkola.Nazwa), () => SelectedSzkola != null);
            OpenOwnerCommand = new RelayCommand(() => RequestOpenOwner?.Invoke(SelectedSzkola!.Id), () => SelectedSzkola != null);
            OpenInstruktorCommand = new RelayCommand(() => RequestOpenInstruktor?.Invoke(SelectedSzkola!.Id), () => SelectedSzkola != null);
            OpenKursantCommand = new RelayCommand(() => RequestOpenKursant?.Invoke(SelectedSzkola!.Id), () => SelectedSzkola != null);

            // CRUD
            AddCommand = new RelayCommand(AddSzkola);
            EditCommand = new RelayCommand(EditSzkola, () => SelectedSzkola != null);
            DeleteCommand = new RelayCommand(DeleteSzkola, () => SelectedSzkola != null);
        }

        private void RefreshList()
        {
            Szkoly.Clear();
            foreach (var szkol in _szkolaSvc.GetAll()) Szkoly.Add(szkol);
        }

        // --- IMPLEMENTACJA CRUD ---

        // 1. CREATE
        private void AddSzkola()
        {
            var data = _dialogs.PromptAddSzkola();
            if (data == null) return;

            var (nazwa, adres, imie, nazwisko) = data.Value;
            try
            {
                _szkolaSvc.Create(nazwa, adres, imie, nazwisko);
                RefreshList();
                _notify.Info("Dodano nową szkołę.", "Sukces");
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message, "Błąd");
            }
        }

        // 2. UPDATE
        private void EditSzkola()
        {
            if (SelectedSzkola == null) return;

            var data = _dialogs.PromptEditSzkola(SelectedSzkola.Nazwa, SelectedSzkola.Adres);
            if (data == null) return;

            var (newNazwa, newAdres) = data.Value;
            try
            {
                _szkolaSvc.Update(SelectedSzkola.Id, newNazwa, newAdres);
                RefreshList();
                _notify.Info("Zaktualizowano dane szkoły.", "Sukces");
            }
            catch (Exception ex)
            {
                _notify.Error(ex.Message, "Błąd");
            }
        }

        // 3. DELETE
        private void DeleteSzkola()
        {
            if (SelectedSzkola == null) return;

            if (_dialogs.Confirm($"Czy na pewno usunąć szkołę {SelectedSzkola.Nazwa}?\nUsunie to również wszystkich pracowników i uczniów!", "Potwierdź usunięcie"))
            {
                try
                {
                    _szkolaSvc.Delete(SelectedSzkola.Id);
                    RefreshList();
                    _notify.Info("Szkoła została usunięta.", "Info");
                }
                catch (Exception ex)
                {
                    _notify.Error(ex.Message, "Błąd");
                }
            }
        }
    }
}