using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Wpf.Infrastructure;
using System;
using System.Collections.ObjectModel;

namespace ProjectApp.Desktop.ViewModels
{
    public class ManagerPanelViewModel : BaseViewModel
    {
        private readonly IPojazdService _pSvc;
        private readonly IKursService _kSvc;
        private readonly IInstruktorService _iSvc;

        private Guid _schoolId;

        public string Title { get; private set; } = "";
        public Action? RequestGoBack;

        public ObservableCollection<Pojazd> Flota { get; } = new();
        public ObservableCollection<Kurs> Kursy { get; } = new();
        public ObservableCollection<Instruktor> Kadra { get; } = new();

        public RelayCommand WrocCommand { get; }

        public ManagerPanelViewModel(IPojazdService p, IKursService k, IInstruktorService i)
        {
            _pSvc = p; _kSvc = k; _iSvc = i;
            WrocCommand = new RelayCommand(() => RequestGoBack?.Invoke());
        }

        public void ZaladujDane(Guid schoolId, string schoolName)
        {
            _schoolId = schoolId;
            Title = $"PANEL MENADŻERA: {schoolName}";
            OnPropertyChanged(nameof(Title));

            Flota.Clear();
            foreach (var x in _pSvc.GetAll(_schoolId)) Flota.Add(x);
            foreach (var x in _kSvc.GetAll(_schoolId)) Kursy.Add(x);
            foreach (var x in _iSvc.GetAll(_schoolId)) Kadra.Add(x);
        }
    }
}