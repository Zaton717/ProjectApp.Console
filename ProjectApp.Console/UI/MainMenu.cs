using ProjectApp.ServiceAbstractions;
using ProjectApp.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectApp.ConsoleApp.Helpers;
using ProjectApp.ConsoleApp.UIDictionary;

namespace ProjectApp.ConsoleApp.UI
{
    public class MainMenu : MenuBase
    {
        private readonly ISzkolaService _sSvc;
        private readonly IInstruktorService _iSvc;
        private readonly IPojazdService _pSvc;
        private readonly IKursService _kSvc;
        private readonly IKursantService _studentSvc;

        private Szkola? _currentSzkola;

        public MainMenu(ISzkolaService s, IInstruktorService i, IPojazdService p, IKursService k, IKursantService st)
        {
            _sSvc = s; _iSvc = i; _pSvc = p; _kSvc = k; _studentSvc = st;
        }

        protected override string Title => $"MENU GŁÓWNE ({(_currentSzkola?.Nazwa ?? "BRAK WYBORU")})";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Wybierz Szkołę / Zmień Placówkę", WybierzSzkole),
            ['2'] = new("Panel Właściciela", PanelWlasciciela),
            ['3'] = new("Panel Menadżera", PanelMenadzera),
            ['4'] = new("Panel Instruktora", PanelInstruktora),
            ['5'] = new("Panel Kursanta", PanelKursanta),
            ['0'] = new("Wyjście", null)
        };

        private void WybierzSzkole()
        {
            var list = _sSvc.GetAll().ToList();
            for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}) {list[i].Nazwa}");
            int idx = ConsoleHelpers.ReadIndex("Wybierz: ", list.Count);
            if (idx >= 0) _currentSzkola = list[idx];
        }

        private void PanelWlasciciela()
        {
            if (_currentSzkola == null) { Console.WriteLine("Najpierw wybierz szkołę."); ConsoleHelpers.Pause(); return; }
            new OwnerMenu(_currentSzkola.Id, _sSvc, _iSvc).Run();
        }

        private void PanelMenadzera()
        {
            if (_currentSzkola == null) { Console.WriteLine("Najpierw wybierz szkołę."); ConsoleHelpers.Pause(); return; }
            if (!_currentSzkola.CzyAktywna) { Console.WriteLine("Zamknięte."); ConsoleHelpers.Pause(); return; }

            // WAŻNE: Przekazujemy wszystkie 5 serwisów do Managera
            new ManagerMenu(_currentSzkola.Id, _pSvc, _kSvc, _iSvc, _studentSvc).Run();
        }

        private void PanelInstruktora()
        {
            if (_currentSzkola == null) { Console.WriteLine("Najpierw wybierz szkołę."); ConsoleHelpers.Pause(); return; }
            if (!_currentSzkola.CzyAktywna) { Console.WriteLine("Zamknięte."); ConsoleHelpers.Pause(); return; }

            var instruktorzy = _iSvc.GetAll(_currentSzkola.Id).ToList();
            if (!instruktorzy.Any()) { Console.WriteLine("Brak instruktorów."); ConsoleHelpers.Pause(); return; }

            Console.WriteLine("--- WYBIERZ SWOJE KONTO ---");
            for (int i = 0; i < instruktorzy.Count; i++)
                Console.WriteLine($"{i + 1}) {instruktorzy[i].PelneImie}");

            int idx = ConsoleHelpers.ReadIndex("Twój wybór: ", instruktorzy.Count);
            if (idx >= 0)
            {
                new InstruktorMenu(_currentSzkola.Id, instruktorzy[idx], _pSvc, _kSvc).Run();
            }
        }

        private void PanelKursanta()
        {
            if (_currentSzkola == null) { Console.WriteLine("Najpierw wybierz szkołę."); ConsoleHelpers.Pause(); return; }
            if (!_currentSzkola.CzyAktywna) { Console.WriteLine("Zamknięte."); ConsoleHelpers.Pause(); return; }

            var kursanci = _currentSzkola.Kursanci.ToList();
            if (!kursanci.Any()) { Console.WriteLine("Brak kursantów."); ConsoleHelpers.Pause(); return; }

            Console.WriteLine("--- WYBIERZ SWOJE KONTO ---");
            for (int i = 0; i < kursanci.Count; i++)
                Console.WriteLine($"{i + 1}) {kursanci[i].PelneImie}");

            int idx = ConsoleHelpers.ReadIndex("Twój wybór: ", kursanci.Count);
            if (idx >= 0)
            {
                new KursantMenu(_currentSzkola.Id, kursanci[idx], _kSvc, _iSvc, _studentSvc).Run();
            }
        }
    }
}