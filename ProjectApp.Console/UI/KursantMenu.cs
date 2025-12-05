using ProjectApp.ServiceAbstractions;
using ProjectApp.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectApp.ConsoleApp.Helpers;
using ProjectApp.ConsoleApp.UIDictionary;

namespace ProjectApp.ConsoleApp.UI
{
    public class KursantMenu : MenuBase
    {
        private readonly Guid _szkolaId;
        private readonly Kursant _kursant;
        private readonly IKursService _kSvc;
        private readonly IInstruktorService _iSvc;
        private readonly IKursantService _studentSvc;

        public KursantMenu(Guid sId, Kursant k, IKursService ks, IInstruktorService Is, IKursantService stSvc)
        {
            _szkolaId = sId; _kursant = k; _kSvc = ks; _iSvc = Is; _studentSvc = stSvc;
        }

        protected override string Title => $"PANEL KURSANTA: {_kursant.PelneImie}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Mój Status / Płatności", PokazStatus),
            ['2'] = new("Szczegóły Kursu", PokazKurs),
            ['0'] = new("Wyloguj", null)
        };

        private void PokazStatus()
        {
            Console.WriteLine($"Imię i nazwisko: {_kursant.PelneImie}");

            var kurs = _kSvc.GetAll(_szkolaId).FirstOrDefault(x => x.Uczestnicy.Any(u => u.Id == _kursant.Id));
            decimal kwota = kurs?.Cena ?? 0;

            if (kurs != null) Console.WriteLine($"Kurs: Kat. {kurs.Kategoria} | Cena: {kwota} PLN");

            string status = _kursant.Oplacony ? "OPŁACONY" : "DO ZAPŁATY";
            Console.ForegroundColor = _kursant.Oplacony ? ConsoleColor.Green : ConsoleColor.Red;
            Console.WriteLine($"Status: {status}");
            Console.ResetColor();

            if (!_kursant.Oplacony)
            {
                Console.WriteLine("\n--- DANE DO PRZELEWU ---");
                Console.WriteLine("Odbiorca: Auto-Mistrz Sp. z o.o.");
                Console.WriteLine("Nr konta: 11 1020 3040 0000 9999 1234 5678");
                Console.WriteLine($"Tytuł:    Kurs ID {_kursant.Id}");
                Console.WriteLine($"Kwota:    {kwota} PLN");

                Console.WriteLine("\n[S] Symuluj przelew online");
                if (Console.ReadLine()?.ToUpper() == "S")
                {
                    _studentSvc.OplacKurs(_kursant.Id);
                    _kursant.Oplacony = true;
                    Console.WriteLine("Płatność przyjęta!");
                }
            }
            ConsoleHelpers.Pause();
        }

        private void PokazKurs()
        {
            var kurs = _kSvc.GetAll(_szkolaId).FirstOrDefault(x => x.Uczestnicy.Any(u => u.Id == _kursant.Id));
            if (kurs == null) { Console.WriteLine("Brak kursu."); ConsoleHelpers.Pause(); return; }

            string instr = "Brak";
            if (kurs.InstruktorId.HasValue)
                instr = _iSvc.GetAll(_szkolaId).FirstOrDefault(i => i.Id == kurs.InstruktorId)?.PelneImie ?? "Nieznany";

            Console.WriteLine($"Kurs Nr {kurs.Numer} (Kat. {kurs.Kategoria})");
            Console.WriteLine($"Instruktor: {instr}");
            Console.WriteLine("Terminy:");
            foreach (var t in kurs.Harmonogram) Console.WriteLine($"- {t}");
            ConsoleHelpers.Pause();
        }
    }
}