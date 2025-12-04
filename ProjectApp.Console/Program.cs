using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.Services;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProjectApp.ConsoleApp
{
    class Program
    {
        // Globalne serwisy
        static ISzkolaService _service;
        static Szkola _aktualnaSzkola;

        static void Main(string[] args)
        {
            // 1. Inicjalizacja
            var unitOfWork = new UnitOfWorkMemory();
            _service = new SzkolaService(unitOfWork);

            // 2. Generator danych (jeśli pusto)
            GenerujBazeJezeliPusta();

            // 3. Sprawdzenie krytyczne
            if (_service.PobierzWszystkieSzkoly().Count == 0)
            {
                Console.WriteLine("[!] BŁĄD KRYTYCZNY: Brak szkół w bazie. Usuń plik .json i zrestartuj aplikację.");
                return;
            }

            // 4. Wybór szkoły na start
            WybierzSzkole();

            // 5. Główna pętla programu
            bool running = true;
            while (running)
            {
                Console.Clear();
                Naglowek("MENU GŁÓWNE");
                Console.WriteLine($"Wybrana placówka: {_aktualnaSzkola.Nazwa.ToUpper()}");
                Console.WriteLine($"Adres:            {_aktualnaSzkola.Adres}");
                Console.WriteLine($"Status:           {(_aktualnaSzkola.CzyAktywna ? "AKTYWNA" : "ZAMKNIĘTA")}");
                Console.WriteLine("---------------------------------------------");

                Console.WriteLine("1. Panel Właściciela");

                if (_aktualnaSzkola.CzyAktywna)
                {
                    Console.WriteLine("2. Panel Menadżera");
                    Console.WriteLine("3. Panel Instruktora");
                    Console.WriteLine("4. Panel Kursanta");
                }
                else
                {
                    Console.WriteLine("   (Panele personelu zablokowane - placówka zamknięta)");
                }

                Console.WriteLine("9. Zmień Placówkę");
                Console.WriteLine("0. Wyjście");
                Console.WriteLine("---------------------------------------------");
                Console.Write("Twój wybór: ");

                var opcja = Console.ReadLine();
                switch (opcja)
                {
                    case "1": PanelWlasciciela(); break;
                    case "2": if (_aktualnaSzkola.CzyAktywna) PanelMenadzera(); break;
                    case "3": if (_aktualnaSzkola.CzyAktywna) PanelInstruktora(); break;
                    case "4": if (_aktualnaSzkola.CzyAktywna) PanelKursanta(); break;
                    case "9": WybierzSzkole(); break;
                    case "0": running = false; break;
                }
            }
        }

        #region PANELE GŁÓWNE

        static void PanelWlasciciela()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Naglowek("PANEL WŁAŚCICIELA");
                Console.WriteLine("1. Raport Generalny (Finanse i Statystyki)");
                Console.WriteLine("2. Zarządzanie Kadrą (Zatrudnij / Zwolnij)");
                Console.WriteLine("3. Dodaj nową szkołę (Globalne)");
                Console.WriteLine($"4. {(_aktualnaSzkola.CzyAktywna ? "Zamknij" : "Otwórz")} obecną placówkę");
                Console.WriteLine("0. Powrót");
                Console.Write("\nOpcja: ");

                switch (Console.ReadLine())
                {
                    case "1": RaportGeneralny(); break;
                    case "2": MenuKadry("WŁAŚCICIEL"); break;
                    case "3":
                        Podnaglowek("NOWA SZKOŁA");
                        Console.Write("Podaj nazwę: "); string n = Console.ReadLine();
                        Console.Write("Podaj adres: "); string a = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(n) && !string.IsNullOrWhiteSpace(a))
                        {
                            _service.UtworzNowaSzkole(n, a, "Właściciel");
                            KomunikatSukces("Utworzono nową placówkę.");
                        }
                        break;
                    case "4":
                        if (_aktualnaSzkola.CzyAktywna)
                        {
                            Console.Write("Czy na pewno chcesz ZAMKNĄĆ placówkę? (tak/nie): ");
                            if (Console.ReadLine().ToLower() == "tak") _service.ZamknijSzkole(_aktualnaSzkola);
                        }
                        else
                        {
                            _aktualnaSzkola.CzyAktywna = true;
                            KomunikatSukces("Szkoła została ponownie otwarta.");
                        }
                        break;
                    case "0": loop = false; break;
                }
            }
        }

        static void PanelMenadzera()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Naglowek($"PANEL MENADŻERA: {_aktualnaSzkola.Menadzer.Imie} {_aktualnaSzkola.Menadzer.Nazwisko}");
                Console.WriteLine("1. Zarządzanie Flotą (Pojazdy)");
                Console.WriteLine("2. Zarządzanie Kursami (Planowanie, Przypisywanie)");
                Console.WriteLine("3. Kadry (Zatrudnij / Zwolnij)");
                Console.WriteLine("0. Powrót");
                Console.Write("\nOpcja: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuFloty(); break;
                    case "2": MenuKursow(); break;
                    case "3": MenuKadry("MENADŻER"); break;
                    case "0": loop = false; break;
                }
            }
        }

        #endregion

        #region PODMENU SZCZEGÓŁOWE (Flota, Kursy, Kadry)

        static void MenuFloty()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Naglowek("ZARZĄDZANIE FLOTĄ POJAZDÓW");

                // 1. Wyświetlanie listy - ZAWSZE AKTUALNE
                if (_aktualnaSzkola.Pojazdy.Count == 0)
                {
                    Console.WriteLine("[INFO] Brak pojazdów w systemie.");
                }
                else
                {
                    Console.WriteLine(String.Format("{0,-4} | {1,-15} | {2,-10} | {3,-12} | {4,-15}", "ID", "POJAZD", "REJESTR.", "STATUS", "INSTRUKTOR"));
                    Console.WriteLine(new string('-', 70));
                    foreach (var p in _aktualnaSzkola.Pojazdy)
                    {
                        string instr = p.PrzypisanyInstruktor != null ? p.PrzypisanyInstruktor.Nazwisko : "---";
                        Console.WriteLine(String.Format("{0,-4} | {1,-15} | {2,-10} | {3,-12} | {4,-15}",
                            p.IdPojazdu,
                            $"{p.Marka} {p.Model}",
                            p.NrRejestracyjny,
                            p.Status,
                            instr));
                    }
                }
                Console.WriteLine(new string('-', 70));

                Console.WriteLine("1. Dodaj nowy pojazd");
                Console.WriteLine("2. Zmień status (Awaria/Serwis)");
                Console.WriteLine("3. Przypisz instruktora do pojazdu");
                Console.WriteLine("0. Powrót");
                Console.Write("\nOpcja: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        DodajPojazdInteraktywnie();
                        break;
                    case "2":
                        Console.Write("Podaj ID pojazdu: ");
                        if (uint.TryParse(Console.ReadLine(), out uint pid))
                        {
                            var p = _aktualnaSzkola.Pojazdy.FirstOrDefault(x => x.IdPojazdu == pid);
                            if (p != null)
                            {
                                Console.WriteLine("Wybierz status: 0-Sprawny, 1-Awaria, 2-W Serwisie");
                                if (int.TryParse(Console.ReadLine(), out int st))
                                    _service.ZmienStatusPojazdu(p, (StatusPojazdu)st);
                            }
                            else KomunikatBlad("Nie znaleziono pojazdu.");
                        }
                        break;
                    case "3":
                        Console.Write("Podaj ID Pojazdu: ");
                        uint.TryParse(Console.ReadLine(), out uint vId);
                        Console.Write("Podaj ID Instruktora: ");
                        uint.TryParse(Console.ReadLine(), out uint iId);

                        var v = _aktualnaSzkola.Pojazdy.FirstOrDefault(x => x.IdPojazdu == vId);
                        var i = _aktualnaSzkola.Instruktorzy.FirstOrDefault(x => x.IdInstruktora == iId);

                        if (v != null && i != null) _service.PrzypiszInstruktoraDoPojazdu(v, i);
                        else KomunikatBlad("Błędne ID pojazdu lub instruktora.");
                        break;
                    case "0": loop = false; break;
                }
            }
        }

        static void MenuKursow()
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Naglowek("ZARZĄDZANIE KURSAMI");

                if (_aktualnaSzkola.Kursy.Count == 0)
                {
                    Console.WriteLine("[INFO] Brak utworzonych kursów.");
                }
                else
                {
                    Console.WriteLine(String.Format("{0,-4} | {1,-5} | {2,-20} | {3,-10}", "ID", "KAT", "INSTRUKTOR", "KURSANCI"));
                    Console.WriteLine(new string('-', 50));
                    foreach (var k in _aktualnaSzkola.Kursy)
                    {
                        string instr = k.Instruktor != null ? k.Instruktor.Nazwisko : "--- BRAK ---";
                        Console.WriteLine(String.Format("{0,-4} | {1,-5} | {2,-20} | {3,-10}",
                            k.IdKursu, k.Kategoria, instr, k.Uczestnicy.Count));
                    }
                }
                Console.WriteLine(new string('-', 50));

                Console.WriteLine("1. Utwórz nowy kurs");
                Console.WriteLine("2. Przypisz Instruktora do kursu");
                Console.WriteLine("3. SZCZEGÓŁY KURSU (Harmonogram i Lista)");
                Console.WriteLine("0. Powrót");
                Console.Write("\nOpcja: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.Write("Kategoria (0:A, 1:B, 2:C, 3:D): ");
                        if (int.TryParse(Console.ReadLine(), out int katInt) && Enum.IsDefined(typeof(KategoriaPrawaJazdy), katInt))
                        {
                            var nowyKurs = new Kurs
                            {
                                IdKursu = (uint)(_aktualnaSzkola.Kursy.Count + 100),
                                Kategoria = (KategoriaPrawaJazdy)katInt,
                                Harmonogram = new Harmonogram { Terminy = new List<string>() },
                                Uczestnicy = new List<Kursant>()
                            };
                            _service.DodajKurs(_aktualnaSzkola, nowyKurs);
                            KomunikatSukces($"Utworzono kurs kategorii {(KategoriaPrawaJazdy)katInt}");
                        }
                        else KomunikatBlad("Nieprawidłowa kategoria.");
                        break;

                    case "2":
                        Console.Write("Podaj ID Kursu: ");
                        uint.TryParse(Console.ReadLine(), out uint kId);
                        var kurs = _aktualnaSzkola.Kursy.FirstOrDefault(x => x.IdKursu == kId);

                        if (kurs == null) { KomunikatBlad("Nie znaleziono kursu."); break; }

                        Console.WriteLine("\n--- Dostępni Instruktorzy ---");
                        foreach (var inst in _aktualnaSzkola.Instruktorzy)
                            Console.WriteLine($"ID: {inst.IdInstruktora} | {inst.Imie} {inst.Nazwisko} | Uprawnienia: {string.Join(",", inst.Uprawnienia)}");

                        Console.Write("Podaj ID Instruktora: ");
                        uint.TryParse(Console.ReadLine(), out uint iId);
                        var instr = _aktualnaSzkola.Instruktorzy.FirstOrDefault(x => x.IdInstruktora == iId);

                        if (instr != null) _service.PrzypiszInstruktoraDoKursu(kurs, instr);
                        else KomunikatBlad("Nie znaleziono instruktora.");
                        break;

                    case "3":
                        Console.Write("Podaj ID Kursu do podglądu: ");
                        if (uint.TryParse(Console.ReadLine(), out uint viewId))
                        {
                            var k = _aktualnaSzkola.Kursy.FirstOrDefault(x => x.IdKursu == viewId);
                            if (k != null)
                            {
                                Console.Clear();
                                Podnaglowek($"SZCZEGÓŁY KURSU #{k.IdKursu} ({k.Kategoria})");
                                Console.WriteLine($"Prowadzący:  {(k.Instruktor != null ? k.Instruktor.Imie + " " + k.Instruktor.Nazwisko : "BRAK")}");
                                Console.WriteLine($"Liczba osób: {k.Uczestnicy.Count}");

                                Console.WriteLine("\n--- LISTA KURSANTÓW ---");
                                if (k.Uczestnicy.Count == 0) Console.WriteLine("(Brak kursantów)");
                                else foreach (var u in k.Uczestnicy) Console.WriteLine($"- {u.Imie} {u.Nazwisko} (Opłacony: {u.Oplacony})");

                                Console.WriteLine("\n--- HARMONOGRAM ---");
                                if (k.Harmonogram.Terminy.Count == 0) Console.WriteLine("(Brak terminów)");
                                else foreach (var t in k.Harmonogram.Terminy) Console.WriteLine($"[ ] {t}");

                                KomunikatInfo("");
                            }
                            else KomunikatBlad("Błędne ID.");
                        }
                        break;
                    case "0": loop = false; break;
                }
            }
        }

        static void MenuKadry(string rola)
        {
            bool loop = true;
            while (loop)
            {
                Console.Clear();
                Naglowek($"ZARZĄDZANIE KADRĄ ({rola})");

                // Zawsze wyświetlaj aktualną listę
                if (_aktualnaSzkola.Instruktorzy.Count == 0) Console.WriteLine("[INFO] Brak instruktorów.");
                else
                {
                    Console.WriteLine(String.Format("{0,-4} | {1,-20} | {2,-15} | {3,-10}", "ID", "IMIĘ I NAZWISKO", "UPRAWNIENIA", "KURSY"));
                    Console.WriteLine(new string('-', 60));
                    foreach (var i in _aktualnaSzkola.Instruktorzy)
                    {
                        int ileKursow = _service.PobierzKursyInstruktora(_aktualnaSzkola, i.IdInstruktora).Count;
                        Console.WriteLine(String.Format("{0,-4} | {1,-20} | {2,-15} | {3,-10}",
                            i.IdInstruktora, $"{i.Imie} {i.Nazwisko}", string.Join(",", i.Uprawnienia), ileKursow));
                    }
                }
                Console.WriteLine(new string('-', 60));

                Console.WriteLine("1. Zatrudnij Instruktora");
                Console.WriteLine("2. Zwolnij Instruktora");
                if (rola == "WŁAŚCICIEL") Console.WriteLine("3. Zmień Menadżera Szkoły");
                Console.WriteLine("0. Powrót");
                Console.Write("\nOpcja: ");

                var opcja = Console.ReadLine();
                switch (opcja)
                {
                    case "1":
                        Podnaglowek("REKRUTACJA");
                        Console.Write("Imię: "); string im = Console.ReadLine();
                        Console.Write("Nazwisko: "); string naz = Console.ReadLine();
                        Console.Write("Uprawnienia (0:A, 1:B, 2:C): ");
                        if (int.TryParse(Console.ReadLine(), out int u))
                        {
                            uint newId = (_aktualnaSzkola.Instruktorzy.Count > 0) ? _aktualnaSzkola.Instruktorzy.Max(x => x.IdInstruktora) + 1 : 1;
                            var instr = new Instruktor { IdInstruktora = newId, Imie = im, Nazwisko = naz, Uprawnienia = { (KategoriaPrawaJazdy)u } };
                            _service.DodajInstruktora(_aktualnaSzkola, instr);
                            KomunikatSukces($"Zatrudniono: {im} {naz}");
                        }
                        break;
                    case "2":
                        Console.Write("Podaj ID instruktora do zwolnienia: ");
                        if (uint.TryParse(Console.ReadLine(), out uint delId))
                        {
                            Console.Write("Potwierdź (tak/nie): ");
                            if (Console.ReadLine() == "tak") _service.ZwolnijInstruktora(_aktualnaSzkola, delId);
                        }
                        break;
                    case "3":
                        if (rola == "WŁAŚCICIEL")
                        {
                            Console.Write("Nowe Imię: "); string ni = Console.ReadLine();
                            Console.Write("Nowe Nazwisko: "); string nn = Console.ReadLine();
                            _service.ZmienMenadzera(_aktualnaSzkola, ni, nn);
                            KomunikatSukces("Zmieniono menadżera.");
                        }
                        break;
                    case "0": loop = false; break;
                }
            }
        }

        #endregion

        #region PANELE UŻYTKOWNIKA (Instruktor, Kursant)

        static void PanelInstruktora()
        {
            Console.Clear();
            Naglowek("PANEL INSTRUKTORA");
            Console.Write("Podaj swoje ID: ");
            if (uint.TryParse(Console.ReadLine(), out uint id))
            {
                var instr = _aktualnaSzkola.Instruktorzy.FirstOrDefault(x => x.IdInstruktora == id);
                if (instr == null) { KomunikatBlad("Brak instruktora o takim ID."); return; }

                Console.WriteLine($"Witaj, {instr.Imie} {instr.Nazwisko}!");
                var kursy = _service.PobierzKursyInstruktora(_aktualnaSzkola, id);

                Console.WriteLine($"\n--- TWOJE KURSY ({kursy.Count}) ---");
                foreach (var k in kursy)
                {
                    Console.WriteLine($"Kurs ID: {k.IdKursu} | Kat: {k.Kategoria} | Kursantów: {k.Uczestnicy.Count}");
                    if (k.Harmonogram != null)
                    {
                        foreach (var t in k.Harmonogram.Terminy) Console.WriteLine($"  -> Termin: {t}");
                    }
                }

                // Sekcja Pojazdu
                var pojazd = _aktualnaSzkola.Pojazdy.FirstOrDefault(p => p.PrzypisanyInstruktor == instr);
                Console.WriteLine("\n--- TWÓJ POJAZD ---");
                if (pojazd != null) Console.WriteLine($"{pojazd.Marka} {pojazd.Model} ({pojazd.NrRejestracyjny}) - Status: {pojazd.Status}");
                else Console.WriteLine("(Brak przypisanego pojazdu)");
            }
            KomunikatInfo("Naciśnij ENTER, aby wrócić.");
        }

        static void PanelKursanta()
        {
            Console.Clear();
            Naglowek("PANEL KURSANTA");
            Console.Write("Podaj swoje ID: ");
            if (uint.TryParse(Console.ReadLine(), out uint id))
            {
                var kurs = _service.PobierzKursKursanta(_aktualnaSzkola, id);
                var kursant = _aktualnaSzkola.Kursanci.FirstOrDefault(k => k.IdKursanta == id);

                if (kursant == null) { KomunikatBlad("Nie ma takiego kursanta."); return; }

                Console.WriteLine($"Witaj, {kursant.Imie} {kursant.Nazwisko}.");

                if (kurs != null)
                {
                    Console.WriteLine($"\nJesteś zapisany na kurs Kat. {kurs.Kategoria}");
                    Console.WriteLine($"Prowadzący: {(kurs.Instruktor != null ? kurs.Instruktor.Nazwisko : "Brak")}");
                    Console.WriteLine($"Status opłaty: {(kursant.Oplacony ? "OPŁACONY" : "DO ZAPŁATY")}");

                    Console.WriteLine("\n--- HARMONOGRAM ZAJĘĆ ---");
                    if (kurs.Harmonogram != null && kurs.Harmonogram.Terminy.Any())
                        foreach (var t in kurs.Harmonogram.Terminy) Console.WriteLine($"[ ] {t}");
                    else Console.WriteLine("Brak terminów.");
                }
                else
                {
                    Console.WriteLine("\nNie jesteś zapisany na żaden kurs.");
                    Console.WriteLine("Skontaktuj się z Menadżerem, aby się zapisać.");
                }
            }
            KomunikatInfo("Naciśnij ENTER, aby wrócić.");
        }

        #endregion

        #region POMOCNIKI UI (FORMATOWANIE)

        static void DodajPojazdInteraktywnie()
        {
            Podnaglowek("DODAWANIE NOWEGO POJAZDU");

            Console.Write("Marka (np. Toyota): ");
            string marka = Console.ReadLine();

            Console.Write("Model (np. Yaris): ");
            string model = Console.ReadLine();

            Console.Write("Nr Rejestracyjny: ");
            string rej = Console.ReadLine().ToUpper(); // Auto uppercase

            Console.WriteLine("Typ (0:Motocykl, 1:Osobowy, 2:Ciężarówka, 3:Autobus): ");
            if (int.TryParse(Console.ReadLine(), out int typ) && Enum.IsDefined(typeof(TypPojazdu), typ))
            {
                var p = new Pojazd
                {
                    IdPojazdu = (uint)(_aktualnaSzkola.Pojazdy.Count + 1),
                    Marka = marka,
                    Model = model,
                    NrRejestracyjny = rej,
                    Typ = (TypPojazdu)typ,
                    Status = StatusPojazdu.Sprawny
                };
                _service.DodajPojazd(_aktualnaSzkola, p);
                KomunikatSukces($"Dodano pojazd: {marka} {model}");
            }
            else KomunikatBlad("Błędny typ pojazdu.");
        }

        static void RaportGeneralny()
        {
            Console.Clear();
            Naglowek($"RAPORT: {_aktualnaSzkola.Nazwa}");

            int iloscPojazdow = _aktualnaSzkola.Pojazdy.Count;
            int iloscSprawnych = _aktualnaSzkola.Pojazdy.Count(p => p.Status == StatusPojazdu.Sprawny);

            int iloscKursantow = _aktualnaSzkola.Kursanci.Count;
            int oplaceni = _aktualnaSzkola.Kursanci.Count(k => k.Oplacony);
            double przychod = oplaceni * 2500; // Symulacja stawki

            Console.WriteLine("--- ZASOBY ---");
            Console.WriteLine($"Instruktorzy:    {_aktualnaSzkola.Instruktorzy.Count}");
            Console.WriteLine($"Flota Pojazdów:  {iloscPojazdow} (W tym sprawnych: {iloscSprawnych})");

            Console.WriteLine("\n--- DYDAKTYKA ---");
            Console.WriteLine($"Aktywne Kursy:   {_aktualnaSzkola.Kursy.Count}");
            Console.WriteLine($"Liczba Kursantów:{iloscKursantow}");

            Console.WriteLine("\n--- FINANSE (Szacunek) ---");
            Console.WriteLine($"Opłacone kursy:  {oplaceni}");
            Console.WriteLine($"Przychód brutto: {przychod:C}"); // Format walutowy

            KomunikatInfo("");
        }

        static void WybierzSzkole()
        {
            while (true)
            {
                Console.Clear();
                Naglowek("WYBÓR PLACÓWKI");
                var szkoly = _service.PobierzWszystkieSzkoly();
                foreach (var s in szkoly)
                {
                    string status = s.CzyAktywna ? "" : " [ZAMKNIĘTA]";
                    Console.WriteLine($"ID: {s.IdSzkoly} | {s.Nazwa} ({s.Adres}){status}");
                }
                Console.Write("\nPodaj ID placówki: ");
                if (uint.TryParse(Console.ReadLine(), out uint id))
                {
                    var s = _service.PobierzSzkole(id);
                    if (s != null)
                    {
                        _aktualnaSzkola = s;
                        return;
                    }
                }
                KomunikatBlad("Nieprawidłowe ID. Spróbuj ponownie.");
            }
        }

        // --- Generator Danych ---
        static void GenerujBazeJezeliPusta()
        {
            if (_service.PobierzWszystkieSzkoly().Any()) return;

            Console.WriteLine(">> Generowanie danych startowych...");
            // Warszawa
            _service.UtworzNowaSzkole("Auto-Mistrz", "Warszawa", "Jan Kowalski");
            var s1 = _service.PobierzWszystkieSzkoly().Last();
            var i1 = new Instruktor { IdInstruktora = 1, Imie = "Adam", Nazwisko = "Nowak", Uprawnienia = { KategoriaPrawaJazdy.B } };
            _service.DodajInstruktora(s1, i1);
            _service.DodajPojazd(s1, new Pojazd { IdPojazdu = 1, Marka = "Toyota", Model = "Yaris", NrRejestracyjny = "WA 12345", Typ = TypPojazdu.SamochodOsobowy });

            // Kraków
            _service.UtworzNowaSzkole("Moto-Krak", "Kraków", "Anna Nowak");

            Console.WriteLine(">> Gotowe.");
        }

        // --- Narzędzia formatowania tekstu ---
        static void Naglowek(string t)
        {
            Console.WriteLine("=============================================");
            Console.WriteLine($"   {t.ToUpper()}");
            Console.WriteLine("=============================================");
        }
        static void Podnaglowek(string t)
        {
            Console.WriteLine($"\n--- {t} ---");
        }
        static void KomunikatSukces(string t)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n[SUKCES] {t}");
            Console.ResetColor();
            Console.WriteLine("Naciśnij ENTER...");
            Console.ReadLine();
        }
        static void KomunikatBlad(string t)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n[!] BŁĄD: {t}");
            Console.ResetColor();
            Console.WriteLine("Naciśnij ENTER...");
            Console.ReadLine();
        }
        static void KomunikatInfo(string t)
        {
            if (!string.IsNullOrEmpty(t)) Console.WriteLine($"\n[INFO] {t}");
            Console.WriteLine("Naciśnij ENTER...");
            Console.ReadLine();
        }

        #endregion
    }
}