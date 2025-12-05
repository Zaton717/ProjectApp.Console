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

        public ManagerMenu(Guid id, IPojazdService p, IKursService k, IInstruktorService i, IKursantService st, ISzkolaService sSvc)
        {
            _sId = id; _pSvc = p; _kSvc = k; _iSvc = i; _studentSvc = st; _szkolaSvc = sSvc;
        }

        protected override string Title => "PANEL MENADŻERA";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Flota - Lista", ListaPojazdow),
            ['2'] = new("Flota - Dodaj Pojazd", DodajPojazd),
            ['3'] = new("Flota - Przypisz Instruktora", PrzypiszPojazdDoInstruktora),
            ['4'] = new("Flota - Zgłoś Awarię/Serwis", ZmienStatus),
            ['5'] = new("Kursy - Utwórz nowy (Cena)", DodajKurs),
            ['6'] = new("Kursy - Przypisz Instruktora", PrzypiszInstruktoraDoKursu),
            ['7'] = new("Kursy - Zapisz Kursanta", ZapiszKursantaNaKurs),
            ['8'] = new("Kursanci - Utwórz konto i przypisz do szkoły", DodajNowegoKursanta),
            ['9'] = new("Kursy - USUŃ KURS", UsunKurs), // <--- NOWE
            ['0'] = new("Powrót", null)
        };

        // --- FLOTA ---
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
            Console.Write("Marka: "); var m = Console.ReadLine();
            Console.Write("Rej: "); var r = Console.ReadLine();
            _pSvc.DodajPojazd(_sId, m ?? "", "Standard", r ?? "", TypPojazdu.SamochodOsobowy);
            Console.WriteLine("Dodano."); ConsoleHelpers.Pause();
        }

        private void PrzypiszPojazdDoInstruktora()
        {
            var pojazdy = _pSvc.GetAll(_sId);
            if (!pojazdy.Any()) { Console.WriteLine("Brak aut."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < pojazdy.Count; i++) Console.WriteLine($"{i + 1}) {pojazdy[i].Marka} ({pojazdy[i].NrRejestracyjny})");
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
            if (!list.Any()) { Console.WriteLine("Brak aut."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < list.Count; i++) Console.WriteLine($"{i + 1}) {list[i].Marka} [{list[i].Status}]");
            int idx = ConsoleHelpers.ReadIndex("Wybierz pojazd: ", list.Count);
            if (idx < 0) return;

            Console.WriteLine("Nowy status: 0-Sprawny, 1-Awaria, 2-WSerwisie");
            if (int.TryParse(Console.ReadLine(), out int st) && Enum.IsDefined(typeof(StatusPojazdu), st))
            {
                _pSvc.ZmienStatus(_sId, list[idx].Id, (StatusPojazdu)st);
                Console.WriteLine("Zapisano.");
            }
            ConsoleHelpers.Pause();
        }

        // --- KURSY I KURSANCI ---

        private void DodajKurs()
        {
            Console.Write("Kategoria (0:A, 1:B...): "); int.TryParse(Console.ReadLine(), out int k);
            Console.Write("Cena (PLN): "); decimal.TryParse(Console.ReadLine(), out decimal c);
            _kSvc.UtworzKurs(_sId, (KategoriaPrawaJazdy)k, c);
            Console.WriteLine($"Utworzono kurs za {c} PLN.");
            ConsoleHelpers.Pause();
        }

        private void PrzypiszInstruktoraDoKursu()
        {
            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) { Console.WriteLine("Brak kursów."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kursy.Count; i++) Console.WriteLine($"{i + 1}) Kurs Nr {kursy[i].Numer} (Kat. {kursy[i].Kategoria})");
            int idxK = ConsoleHelpers.ReadIndex("Wybierz kurs: ", kursy.Count);
            if (idxK < 0) return;

            var kadra = _iSvc.GetAll(_sId).ToList();
            if (!kadra.Any()) { Console.WriteLine("Brak instruktorów."); ConsoleHelpers.Pause(); return; }
            for (int i = 0; i < kadra.Count; i++)
            {
                string upr = string.Join(",", kadra[i].Uprawnienia);
                Console.WriteLine($"{i + 1}) {kadra[i].PelneImie} [{upr}]");
            }
            int idxI = ConsoleHelpers.ReadIndex("Wybierz instruktora: ", kadra.Count);
            if (idxI < 0) return;

            bool sukces = _kSvc.PrzypiszInstruktora(_sId, kursy[idxK].Id, kadra[idxI].Id);
            if (sukces) Console.WriteLine("Przypisano.");
            else Console.WriteLine("Błąd! Instruktor może nie mieć odpowiednich uprawnień.");

            ConsoleHelpers.Pause();
        }

        private void DodajNowegoKursanta()
        {
            var noweId = ConsoleHelpers.AddKursantAndReturnId(_studentSvc);
            if (noweId != Guid.Empty)
            {
                _szkolaSvc.AddExistingKursant(_sId, noweId);
                Console.WriteLine("Kursant utworzony i dodany do listy uczniów tej szkoły.");
            }
            ConsoleHelpers.Pause();
        }

        private void ZapiszKursantaNaKurs()
        {
            var szkola = _szkolaSvc.Get(_sId);
            if (szkola == null) return;
            var uczniowieSzkoly = szkola.Kursanci.ToList();

            if (!uczniowieSzkoly.Any()) { Console.WriteLine("Ta szkoła nie ma jeszcze żadnych uczniów."); ConsoleHelpers.Pause(); return; }

            Console.WriteLine("--- WYBIERZ UCZNIA ---");
            for (int i = 0; i < uczniowieSzkoly.Count; i++) Console.WriteLine($"{i + 1}) {uczniowieSzkoly[i].PelneImie}");
            int idxS = ConsoleHelpers.ReadIndex("Numer: ", uczniowieSzkoly.Count);
            if (idxS < 0) return;

            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) { Console.WriteLine("Brak kursów."); ConsoleHelpers.Pause(); return; }

            Console.WriteLine("--- WYBIERZ KURS ---");
            for (int i = 0; i < kursy.Count; i++) Console.WriteLine($"{i + 1}) Kurs Nr {kursy[i].Numer} (Kat. {kursy[i].Kategoria}) Cena: {kursy[i].Cena}");
            int idxK = ConsoleHelpers.ReadIndex("Numer: ", kursy.Count);
            if (idxK < 0) return;

            _kSvc.ZapiszKursanta(_sId, kursy[idxK].Id, uczniowieSzkoly[idxS].Id);
            Console.WriteLine("Zapisano kursanta na kurs.");
            ConsoleHelpers.Pause();
        }

        private void UsunKurs()
        {
            var kursy = _kSvc.GetAll(_sId).ToList();
            if (!kursy.Any()) { Console.WriteLine("Brak kursów do usunięcia."); ConsoleHelpers.Pause(); return; }

            Console.WriteLine("--- USUWANIE KURSU ---");
            for (int i = 0; i < kursy.Count; i++)
                Console.WriteLine($"{i + 1}) Kurs Nr {kursy[i].Numer} (Kat. {kursy[i].Kategoria}) - Uczestników: {kursy[i].Uczestnicy.Count}");

            int idx = ConsoleHelpers.ReadIndex("Wybierz kurs do usunięcia: ", kursy.Count);
            if (idx < 0) return;

            Console.WriteLine($"Czy na pewno usunąć kurs nr {kursy[idx].Numer}? (tak/nie)");
            if (Console.ReadLine() == "tak")
            {
                _kSvc.UsunKurs(_sId, kursy[idx].Id);
                Console.WriteLine("Kurs usunięty.");
            }
            ConsoleHelpers.Pause();
        }
    }
}