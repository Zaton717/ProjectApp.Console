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
        private readonly ISzkolaService _szkolaSvc;

        public ManagerMenu(Guid id, IPojazdService p, IKursService k, IInstruktorService i, IKursantService s, ISzkolaService sSvc)
        {
            _sId = id; _pSvc = p; _kSvc = k; _iSvc = i; _studentSvc = s; _szkolaSvc = sSvc;
        }

        protected override string Title => "PANEL MENADŻERA";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Flota - Lista i Statusy", ListaPojazdow),
            ['2'] = new("Flota - Dodaj Pojazd", DodajPojazd),
            ['3'] = new("Flota - Przypisz Instruktora", PrzypiszPojazdDoInstruktora),
            ['4'] = new("Flota - Zgłoś Awarię/Serwis", ZmienStatus),

            ['5'] = new("Kursy - Utwórz nowy", DodajKurs),
            ['6'] = new("Kursy - Przypisz Instruktora", PrzypiszInstruktoraDoKursu),
            ['7'] = new("Kursy - Zapisz Kursanta", ZapiszKursantaNaKurs),
            ['8'] = new("Kursy - Usuń Kurs", UsunKurs),

            ['9'] = new("Kursanci - Utwórz konto", DodajNowegoKursanta),

            ['k'] = new("Kadry - Zatrudnij / Zwolnij", MenuKadry),

            ['0'] = new("Powrót", null)
        };

        private void MenuKadry()
        {
            Console.Clear();
            Console.WriteLine("--- ZARZĄDZANIE KADRĄ ---");
            Console.WriteLine("1. Lista Instruktorów");
            Console.WriteLine("2. Zatrudnij Instruktora (Wiele kategorii)");
            Console.WriteLine("3. Zwolnij Instruktora");
            Console.WriteLine("0. Powrót");
            Console.Write("\nOpcja: ");

            switch (Console.ReadLine())
            {
                case "1": PokazListeInstruktorow(); break;
                case "2": ZatrudnijInstruktora(); break;
                case "3": ZwolnijInstruktora(); break;
                case "0": return;
            }
        }

        private void PokazListeInstruktorow()
        {
            var list = _iSvc.GetAll(_sId);
            Console.WriteLine("\n--- LISTA PRACOWNIKÓW ---");
            if (!list.Any()) Console.WriteLine("Brak instruktorów.");
            foreach (var i in list) Console.WriteLine($"- {i.PelneImie} [Uprawnienia: {string.Join(", ", i.Uprawnienia)}]");
            ConsoleHelpers.Pause();
        }

        private void ZatrudnijInstruktora()
        {
            Console.WriteLine("\n--- REKRUTACJA ---");
            string imie;
            do { Console.Write("Imię: "); imie = Console.ReadLine()?.Trim() ?? ""; } while (string.IsNullOrWhiteSpace(imie));

            string nazwisko;
            do { Console.Write("Nazwisko: "); nazwisko = Console.ReadLine()?.Trim() ?? ""; } while (string.IsNullOrWhiteSpace(nazwisko));

            Console.WriteLine("Kategorie po przecinku (A, B, C...):");
            string input = Console.ReadLine()?.ToUpper() ?? "";
            var katList = new List<KategoriaPrawaJazdy>();
            foreach (var s in input.Split(',')) if (Enum.TryParse(s.Trim(), out KategoriaPrawaJazdy k)) katList.Add(k);

            if (katList.Count == 0)
            {
                Console.WriteLine("Nie podano kategorii, domyślnie B.");
                katList.Add(KategoriaPrawaJazdy.B);
            }

            _iSvc.Zatrudnij(_sId, imie, nazwisko, katList);
            Console.WriteLine("Zatrudniono.");
            ConsoleHelpers.Pause();
        }

        private void ZwolnijInstruktora()
        {
            var list = _iSvc.GetAll(_sId).ToList();
            if (!list.Any()) { Console.WriteLine("Brak."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}) {list[i].PelneImie}");
            int idx = ConsoleHelpers.ReadIndex("Kogo zwolnić: ", list.Count);
            if (idx >= 0) _iSvc.Zwolnij(_sId, list[idx].Id);
        }

        private void ListaPojazdow()
        {
            var list = _pSvc.GetAll(_sId);
            Console.WriteLine("--- FLOTA ---");
            foreach (var p in list)
            {
                var col = p.Status == StatusPojazdu.Sprawny ? ConsoleColor.Green : ConsoleColor.Red;
                string kierowca = "Brak";
                if (p.PrzypisanyInstruktorId.HasValue)
                {
                    var instr = _iSvc.GetAll(_sId).FirstOrDefault(i => i.Id == p.PrzypisanyInstruktorId);
                    if (instr != null) kierowca = instr.Nazwisko;
                }
                Console.Write($"- {p.Marka} {p.Model} ({p.NrRejestracyjny}) [Kierowca: {kierowca}] -> ");
                Console.ForegroundColor = col; Console.WriteLine(p.Status); Console.ResetColor();
            }
            ConsoleHelpers.Pause();
        }

        private void DodajPojazd()
        {
            string marka;
            do { Console.Write("Marka: "); marka = Console.ReadLine()?.Trim() ?? ""; } while (string.IsNullOrWhiteSpace(marka));

            string model;
            do { Console.Write("Model: "); model = Console.ReadLine()?.Trim() ?? ""; } while (string.IsNullOrWhiteSpace(model));

            string rej;
            do { Console.Write("Rej: "); rej = Console.ReadLine()?.Trim() ?? ""; } while (string.IsNullOrWhiteSpace(rej));

            _pSvc.DodajPojazd(_sId, marka, model, rej, TypPojazdu.SamochodOsobowy);
            Console.WriteLine("Dodano."); ConsoleHelpers.Pause();
        }

        private void PrzypiszPojazdDoInstruktora()
        {
            var pojazdy = _pSvc.GetAll(_sId);
            if (!pojazdy.Any()) { Console.WriteLine("Brak aut."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < pojazdy.Count; i++) Console.WriteLine($"{i + 1}) {pojazdy[i].Marka}");
            int idxP = ConsoleHelpers.ReadIndex("Pojazd: ", pojazdy.Count);
            if (idxP < 0) return;

            var kadra = _iSvc.GetAll(_sId).ToList();
            if (!kadra.Any()) { Console.WriteLine("Brak kadry."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kadra.Count; i++) Console.WriteLine($"{i + 1}) {kadra[i].PelneImie}");
            int idxI = ConsoleHelpers.ReadIndex("Instruktor: ", kadra.Count);
            if (idxI < 0) return;

            _pSvc.PrzypiszInstruktora(_sId, pojazdy[idxP].Id, kadra[idxI].Id);
            Console.WriteLine("Przypisano."); ConsoleHelpers.Pause();
        }

        private void ZmienStatus()
        {
            var list = _pSvc.GetAll(_sId);
            if (!list.Any()) return;
            for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}) {list[i].Marka} [{list[i].Status}]");
            int idx = ConsoleHelpers.ReadIndex("Pojazd: ", list.Count);
            if (idx < 0) return;
            Console.WriteLine("0-Sprawny, 1-Awaria, 2-WSerwisie");
            int.TryParse(Console.ReadLine(), out int st);
            _pSvc.ZmienStatus(_sId, list[idx].Id, (StatusPojazdu)st);
            ConsoleHelpers.Pause();
        }

        private void DodajKurs()
        {
            Console.Write("Kat (0:A, 1:B, 2:C): ");
            int k;
            while (!int.TryParse(Console.ReadLine(), out k) || !Enum.IsDefined(typeof(KategoriaPrawaJazdy), k))
                Console.WriteLine("Błędna kategoria. Spróbuj ponownie:");

            Console.Write("Cena: ");
            decimal c;
            while (!decimal.TryParse(Console.ReadLine(), out c) || c < 0)
                Console.WriteLine("Błędna cena. Spróbuj ponownie:");

            _kSvc.UtworzKurs(_sId, (KategoriaPrawaJazdy)k, c);
            Console.WriteLine("Utworzono."); ConsoleHelpers.Pause();
        }

        private void PrzypiszInstruktoraDoKursu()
        {
            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) { Console.WriteLine("Brak kursów."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kursy.Count; i++) Console.WriteLine($"{i + 1}) Kurs {kursy[i].Numer} (Kat. {kursy[i].Kategoria})");
            int idxK = ConsoleHelpers.ReadIndex("Kurs: ", kursy.Count);
            if (idxK < 0) return;

            var kadra = _iSvc.GetAll(_sId).ToList();
            if (!kadra.Any()) { Console.WriteLine("Brak kadry."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kadra.Count; i++) Console.WriteLine($"{i + 1}) {kadra[i].PelneImie} [{string.Join(",", kadra[i].Uprawnienia)}]");
            int idxI = ConsoleHelpers.ReadIndex("Instruktor: ", kadra.Count);
            if (idxI < 0) return;

            if (_kSvc.PrzypiszInstruktora(_sId, kursy[idxK].Id, kadra[idxI].Id)) Console.WriteLine("Sukces.");
            else Console.WriteLine("Błąd (brak uprawnień).");
            ConsoleHelpers.Pause();
        }

        private void DodajNowegoKursanta()
        {
            var noweId = ConsoleHelpers.AddKursantAndReturnId(_studentSvc);
            if (noweId != Guid.Empty)
            {
                _szkolaSvc.AddExistingKursant(_sId, noweId);
                Console.WriteLine("Utworzono i przypisano.");
            }
            ConsoleHelpers.Pause();
        }

        private void ZapiszKursantaNaKurs()
        {
            var szkola = _szkolaSvc.Get(_sId);
            if (szkola == null) return;
            var uczniowie = szkola.Kursanci.ToList();
            if (!uczniowie.Any()) { Console.WriteLine("Brak uczniów w szkole."); ConsoleHelpers.Pause(); return; }

            for (int i = 0; i < uczniowie.Count; i++) Console.WriteLine($"{i + 1}) {uczniowie[i].PelneImie}");
            int idxS = ConsoleHelpers.ReadIndex("Uczeń: ", uczniowie.Count);
            if (idxS < 0) return;
            var wybranyUcen = uczniowie[idxS];

            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) { Console.WriteLine("Brak kursów."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kursy.Count; i++) Console.WriteLine($"{i + 1}) Kurs {kursy[i].Numer} (Kat. {kursy[i].Kategoria}) Cena: {kursy[i].Cena}");
            int idxK = ConsoleHelpers.ReadIndex("Kurs docelowy: ", kursy.Count);
            if (idxK < 0) return;
            var nowyKurs = kursy[idxK];

            var staryKurs = kursy.FirstOrDefault(k =>
                k.Kategoria == nowyKurs.Kategoria &&
                k.Id != nowyKurs.Id &&
                k.Uczestnicy.Any(u => u.Id == wybranyUcen.Id)
            );

            if (staryKurs != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nUWAGA! Kursant jest już na kursie Kat. {staryKurs.Kategoria} (Nr {staryKurs.Numer}).");
                Console.ResetColor();

                string statusOplaty = wybranyUcen.Oplacony ? "OPŁACONY" : "NIEOPŁACONY";
                Console.WriteLine($"Obecny status płatności kursanta: {statusOplaty}");
                Console.WriteLine("Status ten zostanie PRZENIESIONY na nowy kurs.");

                Console.WriteLine("\nCzy przepisać ucznia? (tak/nie)");
                if (Console.ReadLine()?.ToLower() == "tak")
                {
                    _kSvc.WypiszKursanta(_sId, staryKurs.Id, wybranyUcen.Id);
                    Console.WriteLine($"Wypisano z kursu nr {staryKurs.Numer}.");
                }
                else
                {
                    Console.WriteLine("Anulowano.");
                    ConsoleHelpers.Pause();
                    return;
                }
            }

            _kSvc.ZapiszKursanta(_sId, nowyKurs.Id, wybranyUcen.Id);
            Console.WriteLine("Zapisano pomyślnie. Status płatności został zachowany.");
            ConsoleHelpers.Pause();
        }

        private void UsunKurs()
        {
            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) { Console.WriteLine("Brak kursów."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kursy.Count; i++) Console.WriteLine($"{i + 1}) Kurs {kursy[i].Numer} (Kat. {kursy[i].Kategoria})");
            int idx = ConsoleHelpers.ReadIndex("Usuń: ", kursy.Count);
            if (idx < 0) return;

            Console.WriteLine("Potwierdź (tak): ");
            if (Console.ReadLine() == "tak") _kSvc.UsunKurs(_sId, kursy[idx].Id);
            ConsoleHelpers.Pause();
        }
    }
}