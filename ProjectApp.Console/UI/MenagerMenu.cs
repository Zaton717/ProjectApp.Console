using ProjectApp.ServiceAbstractions;
using ProjectApp.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using ProjectApp.ConsoleApp.Helpers;
using ProjectApp.ConsoleApp.UIDictionary;

namespace ProjectApp.ConsoleApp.UI
{
    public class ManagerMenu : MenuBase
    {
        private readonly Guid _sId;
        private readonly IPojazdService _pSvc;
        private readonly IKursService _kSvc;
        private readonly IInstruktorService _iSvc;
        private readonly IKursantService _studentSvc;

        public ManagerMenu(Guid id, IPojazdService p, IKursService k, IInstruktorService i, IKursantService s)
        {
            _sId = id; _pSvc = p; _kSvc = k; _iSvc = i; _studentSvc = s;
        }

        protected override string Title => "PANEL MENADŻERA";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Flota - Lista", ListaPojazdow),
            ['2'] = new("Flota - Dodaj Pojazd", DodajPojazd),
            ['3'] = new("Flota - Przypisz Instruktora", PrzypiszPojazdDoInstruktora),
            ['4'] = new("Flota - Zgłoś Awarię/Serwis", ZmienStatus),
            ['5'] = new("Kursy - Utwórz nowy", DodajKurs),
            ['6'] = new("Kursy - Przypisz Instruktora", PrzypiszInstruktoraDoKursu),
            ['7'] = new("Kursy - Zapisz Kursanta", ZapiszKursantaNaKurs),
            ['8'] = new("KADRY - Zatrudnij/Zwolnij", ZarzadzajKadra), // <--- NOWE PODMENU
            ['9'] = new("Kursanci - Utwórz konto", DodajNowegoKursanta),
            ['0'] = new("Powrót", null)
        };

        // --- KADRY (NOWE) ---
        private void ZarzadzajKadra()
        {
            Console.Clear();
            Console.WriteLine("--- ZARZĄDZANIE KADRĄ ---");
            Console.WriteLine("1. Lista Instruktorów");
            Console.WriteLine("2. Zatrudnij Instruktora (Wiele kategorii)");
            Console.WriteLine("3. Zwolnij Instruktora");
            Console.WriteLine("0. Powrót");

            var opt = Console.ReadLine();

            if (opt == "1")
            {
                var list = _iSvc.GetAll(_sId);
                foreach (var i in list) Console.WriteLine($"- {i.PelneImie} [{string.Join(",", i.Uprawnienia)}]");
                ConsoleHelpers.Pause();
            }
            else if (opt == "2")
            {
                Console.Write("Imię: "); var i = Console.ReadLine();
                Console.Write("Nazwisko: "); var n = Console.ReadLine();

                Console.WriteLine("Kategorie po przecinku (A, B, C...):");
                string input = Console.ReadLine()?.ToUpper() ?? "";
                var katList = new List<KategoriaPrawaJazdy>();
                foreach (var s in input.Split(','))
                    if (Enum.TryParse(s.Trim(), out KategoriaPrawaJazdy k)) katList.Add(k);

                if (katList.Count == 0) katList.Add(KategoriaPrawaJazdy.B); // Domyślnie B

                _iSvc.Zatrudnij(_sId, i ?? "", n ?? "", katList);
                Console.WriteLine("Zatrudniono.");
                ConsoleHelpers.Pause();
            }
            else if (opt == "3")
            {
                var list = _iSvc.GetAll(_sId);
                for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}) {list[i].PelneImie}");
                int idx = ConsoleHelpers.ReadIndex("Kogo zwolnić: ", list.Count);
                if (idx >= 0) _iSvc.Zwolnij(_sId, list[idx].Id);
            }
        }

        // --- FLOTA ---
        private void ListaPojazdow()
        {
            var list = _pSvc.GetAll(_sId);
            foreach (var p in list)
            {
                var col = p.Status == StatusPojazdu.Sprawny ? ConsoleColor.Green : ConsoleColor.Red;
                string kierowca = "Brak";
                if (p.PrzypisanyInstruktorId.HasValue)
                {
                    var instr = _iSvc.GetAll(_sId).FirstOrDefault(i => i.Id == p.PrzypisanyInstruktorId);
                    if (instr != null) kierowca = instr.Nazwisko;
                }
                Console.Write($"- {p.Marka} {p.Model} [{p.NrRejestracyjny}] ({kierowca}) -> ");
                Console.ForegroundColor = col; Console.WriteLine(p.Status); Console.ResetColor();
            }
            ConsoleHelpers.Pause();
        }

        private void DodajPojazd()
        {
            Console.Write("Marka: "); var m = Console.ReadLine();
            Console.Write("Rej: "); var r = Console.ReadLine();
            _pSvc.DodajPojazd(_sId, m ?? "", "Standard", r ?? "", TypPojazdu.SamochodOsobowy);
            Console.WriteLine("Dodano."); ConsoleHelpers.Pause();
        }

        private void PrzypiszPojazdDoInstruktora()
        {
            var pojazdy = _pSvc.GetAll(_sId);
            if (!pojazdy.Any()) { Console.WriteLine("Brak aut."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < pojazdy.Count; i++) Console.WriteLine($"{i + 1}) {pojazdy[i].Marka}");
            int idxP = ConsoleHelpers.ReadIndex("Wybierz pojazd: ", pojazdy.Count);
            if (idxP < 0) return;

            var kadra = _iSvc.GetAll(_sId).ToList();
            if (!kadra.Any()) { Console.WriteLine("Brak instruktorów."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kadra.Count; i++) Console.WriteLine($"{i + 1}) {kadra[i].PelneImie}");
            int idxI = ConsoleHelpers.ReadIndex("Wybierz instruktora: ", kadra.Count);
            if (idxI < 0) return;

            _pSvc.PrzypiszInstruktora(_sId, pojazdy[idxP].Id, kadra[idxI].Id);
            Console.WriteLine("Przypisano."); ConsoleHelpers.Pause();
        }

        private void ZmienStatus()
        {
            var list = _pSvc.GetAll(_sId);
            if (!list.Any()) return;
            for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}) {list[i].Marka} [{list[i].Status}]");
            int idx = ConsoleHelpers.ReadIndex("Wybierz: ", list.Count);
            if (idx < 0) return;
            Console.WriteLine("0-Sprawny, 1-Awaria, 2-WSerwisie");
            int.TryParse(Console.ReadLine(), out int st);
            _pSvc.ZmienStatus(_sId, list[idx].Id, (StatusPojazdu)st);
            ConsoleHelpers.Pause();
        }

        // --- KURSY ---
        private void DodajKurs()
        {
            Console.Write("Kategoria (0:A, 1:B...): "); int.TryParse(Console.ReadLine(), out int k);
            Console.Write("Cena: "); decimal.TryParse(Console.ReadLine(), out decimal c);
            _kSvc.UtworzKurs(_sId, (KategoriaPrawaJazdy)k, c);
            Console.WriteLine("Utworzono."); ConsoleHelpers.Pause();
        }

        private void PrzypiszInstruktoraDoKursu()
        {
            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) return;
            for (int i = 0; i < kursy.Count; i++) Console.WriteLine($"{i + 1}) Kurs {kursy[i].Numer} (Kat. {kursy[i].Kategoria})");
            int idxK = ConsoleHelpers.ReadIndex("Kurs: ", kursy.Count);
            if (idxK < 0) return;

            var kadra = _iSvc.GetAll(_sId).ToList();
            for (int i = 0; i < kadra.Count; i++) Console.WriteLine($"{i + 1}) {kadra[i].PelneImie} [{string.Join(",", kadra[i].Uprawnienia)}]");
            int idxI = ConsoleHelpers.ReadIndex("Instruktor: ", kadra.Count);
            if (idxI < 0) return;

            if (_kSvc.PrzypiszInstruktora(_sId, kursy[idxK].Id, kadra[idxI].Id)) Console.WriteLine("Sukces.");
            else Console.WriteLine("Błąd (brak uprawnień?).");
            ConsoleHelpers.Pause();
        }

        private void DodajNowegoKursanta()
        {
            ConsoleHelpers.AddKursantAndReturnId(_studentSvc);
            ConsoleHelpers.Pause();
        }

        private void ZapiszKursantaNaKurs()
        {
            var wszyscy = _studentSvc.GetAll().ToList();
            if (!wszyscy.Any()) return;
            Console.WriteLine("--- KURSANCI ---");
            for (int i = 0; i < wszyscy.Count; i++) Console.WriteLine($"{i + 1}) {wszyscy[i].PelneImie}");
            int idxS = ConsoleHelpers.ReadIndex("Wybierz: ", wszyscy.Count);
            if (idxS < 0) return;

            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) return;
            Console.WriteLine("--- KURSY ---");
            for (int i = 0; i < kursy.Count; i++) Console.WriteLine($"{i + 1}) Kurs {kursy[i].Numer} (Kat. {kursy[i].Kategoria})");
            int idxK = ConsoleHelpers.ReadIndex("Wybierz: ", kursy.Count);
            if (idxK < 0) return;

            _kSvc.ZapiszKursanta(_sId, kursy[idxK].Id, wszyscy[idxS].Id);
            Console.WriteLine("Zapisano.");
            ConsoleHelpers.Pause();
        }
    }
}