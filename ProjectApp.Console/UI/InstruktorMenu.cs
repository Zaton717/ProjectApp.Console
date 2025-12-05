using ProjectApp.ServiceAbstractions;
using ProjectApp.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectApp.ConsoleApp.Helpers;
using ProjectApp.ConsoleApp.UIDictionary;

namespace ProjectApp.ConsoleApp.UI
{
    public class InstruktorMenu : MenuBase
    {
        private readonly Guid _szkolaId;
        private readonly Instruktor _instruktor;
        private readonly IPojazdService _pSvc;
        private readonly IKursService _kSvc;

        public InstruktorMenu(Guid szkolaId, Instruktor instruktor, IPojazdService p, IKursService k)
        {
            _szkolaId = szkolaId;
            _instruktor = instruktor;
            _pSvc = p;
            _kSvc = k;
        }

        protected override string Title => $"PANEL INSTRUKTORA: {_instruktor.PelneImie}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Mój Pojazd", PokazPojazd),
            ['2'] = new("Moje Kursy i Harmonogram", PokazKursy),
            ['0'] = new("Wyloguj", null)
        };

        private void PokazPojazd()
        {
            var pojazdy = _pSvc.GetAll(_szkolaId)
                .Where(p => p.PrzypisanyInstruktorId == _instruktor.Id)
                .ToList();

            if (!pojazdy.Any())
            {
                Console.WriteLine("Nie masz przypisanego pojazdu.");
            }
            else
            {
                Console.WriteLine("--- TWOJE POJAZDY ---");
                foreach (var p in pojazdy)
                {
                    Console.WriteLine($"- {p.Marka} {p.Model} ({p.NrRejestracyjny}) [Status: {p.Status}]");
                }
            }
            ConsoleHelpers.Pause();
        }

        private void PokazKursy()
        {
            var kursy = _kSvc.GetAll(_szkolaId)
                .Where(k => k.InstruktorId == _instruktor.Id)
                .ToList();

            if (!kursy.Any())
            {
                Console.WriteLine("Nie prowadzisz obecnie żadnych kursów.");
            }
            else
            {
                Console.WriteLine($"--- TWOJE KURSY (Liczba: {kursy.Count}) ---");
                foreach (var k in kursy)
                {
                    // TUTAJ ZMIANA: Wyświetlamy Numer
                    Console.WriteLine($"\n[KURS NR {k.Numer}] - Kategoria {k.Kategoria}");
                    Console.WriteLine($"Liczba kursantów: {k.Uczestnicy.Count}");

                    if (k.Harmonogram.Any())
                    {
                        Console.WriteLine("Harmonogram:");
                        foreach (var t in k.Harmonogram) Console.WriteLine($"  * {t}");
                    }
                    else
                    {
                        Console.WriteLine(" (Brak ustalonych terminów)");
                    }
                }
            }
            ConsoleHelpers.Pause();
        }
    }
}