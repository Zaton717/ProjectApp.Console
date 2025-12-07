using ProjectApp.ServiceAbstractions;
using ProjectApp.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectApp.ConsoleApp.Helpers;
using ProjectApp.ConsoleApp.UIDictionary;

namespace ProjectApp.ConsoleApp.UI
{
    public class OwnerMenu : MenuBase
    {
        private readonly Guid _sId;
        private readonly ISzkolaService _sSvc;
        private readonly IInstruktorService _iSvc;

        public OwnerMenu(Guid id, ISzkolaService s, IInstruktorService i)
        {
            _sId = id;
            _sSvc = s;
            _iSvc = i;
        }

        protected override string Title => "PANEL WŁAŚCICIELA";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Raport Generalny", Raport),
            ['2'] = new("Lista Pracowników", PokazKadre),
            ['3'] = new("Zatrudnij Instruktora", Zatrudnij),
            ['4'] = new("Zwolnij Instruktora", Zwolnij),
            ['5'] = new("Zamknij/Otwórz tę placówkę", Toggle),
            ['6'] = new("Otwórz nową placówkę", DodajNowaSzkole),
            ['0'] = new("Powrót", null)
        };

        private void Raport()
        {
            var s = _sSvc.Get(_sId);
            if (s == null) return;
            Console.WriteLine($"Szkoła: {s.Nazwa}\nStatus: {(s.CzyAktywna ? "Aktywna" : "Zamknięta")}");
            Console.WriteLine($"Instruktorów: {s.Instruktorzy.Count}");
            Console.WriteLine($"Pojazdów: {s.Pojazdy.Count}");
            Console.WriteLine($"Kursów: {s.Kursy.Count}");

            decimal przychod = 0;
            int oplaceni = 0;
            foreach (var k in s.Kursy)
            {
                int zaplacili = k.Uczestnicy.Count(u => u.Oplacony);
                oplaceni += zaplacili;
                przychod += zaplacili * k.Cena;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n--- FINANSE ---");
            Console.WriteLine($"Opłaconych kursów: {oplaceni}");
            Console.WriteLine($"Przychód całkowity: {przychod:C} PLN");
            Console.ResetColor();

            ConsoleHelpers.Pause();
        }

        private void PokazKadre()
        {
            var szkola = _sSvc.Get(_sId);
            if (szkola == null) return;

            Console.WriteLine("--- ZARZĄD ---");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($" {szkola.Menadzer.PelneImie.ToUpper()} (Menadżer)");
            Console.ResetColor();

            var list = _iSvc.GetAll(_sId);
            Console.WriteLine("\n--- INSTRUKTORZY ---");
            if (!list.Any()) Console.WriteLine(" Brak.");
            foreach (var i in list)
            {
                Console.WriteLine($" - {i.PelneImie} [Uprawnienia: {string.Join(", ", i.Uprawnienia)}]");
            }
            ConsoleHelpers.Pause();
        }

        private void Zatrudnij()
        {
            Console.WriteLine("\n--- REKRUTACJA ---");
            string imie;
            do { Console.Write("Imię: "); imie = Console.ReadLine()?.Trim() ?? ""; } while (string.IsNullOrWhiteSpace(imie));
            string nazwisko;
            do { Console.Write("Nazwisko: "); nazwisko = Console.ReadLine()?.Trim() ?? ""; } while (string.IsNullOrWhiteSpace(nazwisko));

            Console.WriteLine("Kategorie po przecinku (A, B...):");
            string input = Console.ReadLine()?.ToUpper() ?? "";
            var katList = new List<KategoriaPrawaJazdy>();
            foreach (var s in input.Split(','))
                if (Enum.TryParse(s.Trim(), out KategoriaPrawaJazdy k)) katList.Add(k);

            if (katList.Count == 0) katList.Add(KategoriaPrawaJazdy.B);

            _iSvc.Zatrudnij(_sId, imie, nazwisko, katList);
            Console.WriteLine("Zatrudniono.");
            ConsoleHelpers.Pause();
        }

        private void Zwolnij()
        {
            var list = _iSvc.GetAll(_sId);
            for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}) {list[i].PelneImie}");
            int idx = ConsoleHelpers.ReadIndex("Kogo zwolnić: ", list.Count);
            if (idx >= 0) _iSvc.Zwolnij(_sId, list[idx].Id);
        }

        private void Toggle()
        {
            var s = _sSvc.Get(_sId);
            if (s!.CzyAktywna) _sSvc.ZamknijSzkole(_sId); else _sSvc.OtworzSzkole(_sId);
            Console.WriteLine("Zmieniono."); ConsoleHelpers.Pause();
        }

        private void DodajNowaSzkole()
        {
            Console.WriteLine("\n--- TWORZENIE NOWEJ PLACÓWKI ---");

            string nazwa;
            do
            {
                Console.Write("Nazwa szkoły (np. Auto-Mistrz Kraków): ");
                nazwa = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nazwa)) Console.WriteLine("Błąd: Nazwa jest wymagana.");
            } while (string.IsNullOrWhiteSpace(nazwa));

            string adres;
            do
            {
                Console.Write("Adres (Miasto/Ulica): ");
                adres = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(adres)) Console.WriteLine("Błąd: Adres jest wymagany.");
            } while (string.IsNullOrWhiteSpace(adres));

            Console.WriteLine("\n--- MIANOWANIE MENADŻERA ---");

            string imieM;
            do
            {
                Console.Write("Imię Menadżera: ");
                imieM = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(imieM)) Console.WriteLine("Błąd: Imię jest wymagane.");
            } while (string.IsNullOrWhiteSpace(imieM));

            string nazwM;
            do
            {
                Console.Write("Nazwisko Menadżera: ");
                nazwM = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(nazwM)) Console.WriteLine("Błąd: Nazwisko jest wymagane.");
            } while (string.IsNullOrWhiteSpace(nazwM));

            _sSvc.Create(nazwa, adres, imieM, nazwM);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSUKCES! Utworzono nową szkołę: {nazwa}");
            Console.ResetColor();
            ConsoleHelpers.Pause();
        }
    }
}