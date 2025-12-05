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
            ['1'] = new("Raport Generalny (Finanse)", Raport),
            ['2'] = new("Lista Pracowników (Menadżer + Kadra)", PokazKadre),
            ['3'] = new("Zatrudnij Instruktora (Wiele kat.)", Zatrudnij), // <--- Zmiana
            ['4'] = new("Zwolnij Instruktora", Zwolnij),
            ['5'] = new("Zamknij/Otwórz tę placówkę", Toggle),
            ['6'] = new("OTWÓRZ NOWĄ PLACÓWKĘ", DodajNowaSzkole),
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
            Console.Write("Imię: "); var i = Console.ReadLine();
            Console.Write("Nazwisko: "); var n = Console.ReadLine();

            Console.WriteLine("Podaj kategorie oddzielone przecinkiem (np. A, B, C):");
            string inputKat = Console.ReadLine()?.ToUpper() ?? "";

            var wybraneKategorie = new List<KategoriaPrawaJazdy>();
            var czesci = inputKat.Split(',');

            foreach (var czesc in czesci)
            {
                if (Enum.TryParse(czesc.Trim(), out KategoriaPrawaJazdy kat))
                {
                    wybraneKategorie.Add(kat);
                }
            }

            if (wybraneKategorie.Count == 0)
            {
                Console.WriteLine("Nie podano poprawnych kategorii. Domyślnie przypisano B.");
                wybraneKategorie.Add(KategoriaPrawaJazdy.B);
            }

            _iSvc.Zatrudnij(_sId, i ?? "", n ?? "", wybraneKategorie);
            Console.WriteLine($"Zatrudniono instruktora z uprawnieniami: {string.Join(", ", wybraneKategorie)}");
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
            Console.Write("Nazwa szkoły (np. Auto-Mistrz Kraków): ");
            string nazwa = Console.ReadLine() ?? "";

            Console.Write("Adres (Miasto/Ulica): ");
            string adres = Console.ReadLine() ?? "";

            Console.WriteLine("\n--- MIANOWANIE MENADŻERA ---");
            Console.Write("Imię Menadżera: ");
            string imieM = Console.ReadLine() ?? "";

            Console.Write("Nazwisko Menadżera: ");
            string nazwM = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(nazwa) || string.IsNullOrWhiteSpace(imieM))
            {
                Console.WriteLine("Dane nie mogą być puste!");
                ConsoleHelpers.Pause();
                return;
            }

            var noweId = _sSvc.Create(nazwa, adres, imieM, nazwM);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nSUKCES! Utworzono nową szkołę: {nazwa}");
            Console.ResetColor();
            Console.WriteLine("Aby nią zarządzać, wróć do Menu Głównego i wybierz opcję 'Wybierz Szkołę'.");

            ConsoleHelpers.Pause();
        }
    }
}