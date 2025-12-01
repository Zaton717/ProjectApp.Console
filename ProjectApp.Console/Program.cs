using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.Services;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.ConsoleApp
{
    class Program
    {
        // Używamy interfejsu (abstrakcji)
        static ISzkolaService _service;
        static Szkola _aktualnaSzkola;

        static void Main(string[] args)
        {
            // Konfiguracja (Dependency Injection)
            var unitOfWork = new UnitOfWorkMemory();
            _service = new SzkolaService(unitOfWork);

            ZainicjujDaneStartowe();

            bool running = true;
            while (running)
            {
                Console.Clear();
                Console.WriteLine("=============================================");
                Console.WriteLine($"   SYSTEM OBSŁUGI SZKOŁY: {_aktualnaSzkola.Nazwa.ToUpper()}");
                Console.WriteLine("=============================================");
                Console.WriteLine("1. Panel Właściciela");
                Console.WriteLine("2. Panel Menadżera");
                Console.WriteLine("3. Panel Instruktora");
                Console.WriteLine("4. Panel Kursanta");
                Console.WriteLine("0. Wyjście");
                Console.Write("\nTwój wybór: ");

                var wybor = Console.ReadLine();

                switch (wybor)
                {
                    case "1": PanelWlasciciela(); break;
                    case "2": PanelMenadzera(); break;
                    case "3": PanelInstruktora(); break;
                    case "4": PanelKursanta(); break;
                    case "0": running = false; break;
                    default: Komunikat("Niepoprawna opcja."); break;
                }
            }
        }

        static void PanelWlasciciela()
        {
            Console.Clear();
            Console.WriteLine("--- PANEL WŁAŚCICIELA ---");
            Console.WriteLine("1. Raport finansowy");
            Console.WriteLine("0. Powrót");

            if (Console.ReadLine() == "1")
            {
                int oplaceni = _aktualnaSzkola.Kursanci.Count(k => k.Oplacony);
                Console.WriteLine($"Liczba opłaconych kursantów: {oplaceni}");
                Console.WriteLine($"Szacowany przychód (2500 zł/os): {oplaceni * 2500} PLN");
                Komunikat("");
            }
        }

        static void PanelMenadzera()
        {
            bool logIn = true;
            while (logIn)
            {
                Console.Clear();
                Console.WriteLine($"--- PANEL MENADŻERA: {_aktualnaSzkola.Menadzer.Imie} ---");
                Console.WriteLine("1. Utwórz nowy Kurs");
                Console.WriteLine("2. Przypisz Instruktora do Kursu");
                Console.WriteLine("3. Lista Kursów");
                Console.WriteLine("0. Wyloguj");
                Console.Write("Opcja: ");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("\nTworzenie kursu (0: A, 1: B, 2: C): ");
                        if (int.TryParse(Console.ReadLine(), out int kat))
                        {
                            var nowyKurs = new Kurs
                            {
                                IdKursu = (uint)(_aktualnaSzkola.Kursy.Count + 100),
                                Kategoria = (KategoriaPrawaJazdy)kat,
                                Harmonogram = new Harmonogram { Terminy = new List<string>() },
                                Uczestnicy = new List<Kursant>()
                            };
                            _service.DodajKurs(_aktualnaSzkola, nowyKurs);
                            Komunikat($"Utworzono kurs kat. {nowyKurs.Kategoria}");
                        }
                        break;

                    case "2":
                        Console.WriteLine("\nDostępne kursy bez instruktora:");
                        foreach (var k in _aktualnaSzkola.Kursy.Where(x => x.Instruktor == null))
                            Console.WriteLine($"ID: {k.IdKursu} | Kat: {k.Kategoria}");

                        Console.Write("Podaj ID Kursu: ");
                        uint kId = uint.Parse(Console.ReadLine());
                        var kurs = _aktualnaSzkola.Kursy.FirstOrDefault(x => x.IdKursu == kId);

                        Console.WriteLine("\nInstruktorzy:");
                        foreach (var i in _aktualnaSzkola.Instruktorzy)
                            Console.WriteLine($"ID: {i.IdInstruktora} | {i.Nazwisko} | Uprawnienia: {string.Join(", ", i.Uprawnienia)}");

                        Console.Write("Podaj ID Instruktora: ");
                        uint iId = uint.Parse(Console.ReadLine());
                        var instr = _aktualnaSzkola.Instruktorzy.FirstOrDefault(x => x.IdInstruktora == iId);

                        // UŻYCIE SERWISU DO LOGIKI
                        _service.PrzypiszInstruktoraDoKursu(kurs, instr);
                        Komunikat("");
                        break;

                    case "3":
                        foreach (var k in _aktualnaSzkola.Kursy)
                            Console.WriteLine($"Kurs #{k.IdKursu} ({k.Kategoria}) - Instr: {(k.Instruktor?.Nazwisko ?? "BRAK")}");
                        Komunikat("");
                        break;

                    case "0": logIn = false; break;
                }
            }
        }

        static void PanelInstruktora()
        {
            Console.Write("Podaj ID Instruktora (1): ");
            uint.TryParse(Console.ReadLine(), out uint id);

            // Pobieramy kursy przez serwis (opcjonalnie) lub filtrujemy lokalnie
            var mojeKursy = _service.PobierzKursyInstruktora(_aktualnaSzkola, id);

            if (mojeKursy.Count == 0)
            {
                Komunikat("Brak przypisanych kursów lub złe ID.");
                return;
            }

            Console.WriteLine($"\nMasz {mojeKursy.Count} kursów.");
            Console.WriteLine("1. Dodaj termin zajęć");
            Console.WriteLine("0. Powrót");

            if (Console.ReadLine() == "1")
            {
                var k = mojeKursy.First();
                Console.Write($"Podaj termin dla kursu {k.IdKursu}: ");
                string termin = Console.ReadLine();
                _service.DodajTerminDoHarmonogramu(k, termin);
                Komunikat("Dodano termin.");
            }
        }

        static void PanelKursanta()
        {
            Console.Write("Podaj ID Kursanta (1): ");
            uint.TryParse(Console.ReadLine(), out uint id);
            var kursant = _aktualnaSzkola.Kursanci.FirstOrDefault(k => k.IdKursanta == id);

            if (kursant == null) { Komunikat("Nie znaleziono kursanta."); return; }

            bool logIn = true;
            while (logIn)
            {
                Console.Clear();
                Console.WriteLine($"--- PANEL KURSANTA: {kursant.Imie} ---");
                Console.WriteLine($"Status: {(kursant.Oplacony ? "OPŁACONY" : "NIEOPŁACONY")}");
                Console.WriteLine("1. Zapisz się na kurs");
                Console.WriteLine("2. Opłać kurs");
                Console.WriteLine("0. Wyloguj");

                switch (Console.ReadLine())
                {
                    case "1":
                        Console.WriteLine("Dostępne kursy:");
                        foreach (var k in _aktualnaSzkola.Kursy) Console.WriteLine($"ID: {k.IdKursu} Kat: {k.Kategoria}");
                        Console.Write("Wybierz ID: ");
                        if (uint.TryParse(Console.ReadLine(), out uint kid))
                        {
                            var kurs = _aktualnaSzkola.Kursy.FirstOrDefault(x => x.IdKursu == kid);
                            // UŻYCIE SERWISU
                            _service.ZapiszKursantaNaKurs(kursant, kurs);
                            Komunikat("");
                        }
                        break;
                    case "2":
                        if (kursant.Oplacony) Komunikat("Już opłacono.");
                        else
                        {
                            var platnosc = new Platnosc { IdPlatnosci = 123, Kursant = kursant, Kwota = 2500 };
                            // UŻYCIE SERWISU
                            _service.ZatwierdzPlatnosc(platnosc);
                            Komunikat("");
                        }
                        break;
                    case "0": logIn = false; break;
                }
            }
        }

        static void ZainicjujDaneStartowe()
        {
            _service.UtworzNowaSzkole("Szkoła Mistrzów", "Warszawa", "Prezes");
            _aktualnaSzkola = _service.PobierzWszystkieSzkoly()[0];

            // Instruktor
            var instr = new Instruktor { IdInstruktora = 1, Imie = "Marek", Nazwisko = "Instruktor", Uprawnienia = new List<KategoriaPrawaJazdy> { KategoriaPrawaJazdy.B } };
            _service.DodajInstruktora(_aktualnaSzkola, instr);

            // Kursant
            var kursant = new Kursant { IdKursanta = 1, Imie = "Kamil", Nazwisko = "Uczeń", Kategoria = KategoriaPrawaJazdy.B };
            _service.RejestrujKursanta(_aktualnaSzkola, kursant);

            // Kurs
            var kurs = new Kurs
            {
                IdKursu = 101,
                Kategoria = KategoriaPrawaJazdy.B,
                Harmonogram = new Harmonogram { Terminy = new List<string>() },
                Uczestnicy = new List<Kursant>()
            };
            _service.DodajKurs(_aktualnaSzkola, kurs);
        }

        static void Komunikat(string tekst)
        {
            if (!string.IsNullOrEmpty(tekst)) Console.WriteLine(tekst);
            Console.WriteLine("Naciśnij ENTER...");
            Console.ReadLine();
        }
    }
}