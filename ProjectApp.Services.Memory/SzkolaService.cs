using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Services
{
    public class SzkolaService : ISzkolaService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SzkolaService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- WŁAŚCICIEL: ZAMYKANIE I KADRY ---

        public void ZamknijSzkole(Szkola szkola)
        {
            if (szkola != null)
            {
                szkola.CzyAktywna = false;
                _unitOfWork.Save();
                Console.WriteLine($"[WŁAŚCICIEL] Szkoła '{szkola.Nazwa}' została zamknięta.");
            }
        }

        public void ZmienMenadzera(Szkola szkola, string noweImie, string noweNazwisko)
        {
            if (szkola != null && szkola.Menadzer != null)
            {
                szkola.Menadzer.Imie = noweImie;
                szkola.Menadzer.Nazwisko = noweNazwisko;
                _unitOfWork.Save();
                Console.WriteLine($"[KADRY] Nowym menadżerem jest {noweImie} {noweNazwisko}.");
            }
        }

        public void ZwolnijInstruktora(Szkola szkola, uint idInstruktora)
        {
            if (szkola == null) return;

            var instruktor = szkola.Instruktorzy.FirstOrDefault(i => i.IdInstruktora == idInstruktora);
            if (instruktor == null)
            {
                Console.WriteLine("[BŁĄD] Nie znaleziono instruktora.");
                return;
            }

            // 1. Usuń go z przypisanych Kursów
            foreach (var kurs in szkola.Kursy.Where(k => k.Instruktor == instruktor))
            {
                kurs.Instruktor = null;
            }

            // 2. Usuń go z przypisanych Pojazdów
            foreach (var pojazd in szkola.Pojazdy.Where(p => p.PrzypisanyInstruktor == instruktor))
            {
                pojazd.PrzypisanyInstruktor = null;
            }

            // 3. Usuń go z listy pracowników
            szkola.Instruktorzy.Remove(instruktor);
            _unitOfWork.Save();
            Console.WriteLine($"[KADRY] Instruktor {instruktor.Nazwisko} został zwolniony. Zasoby zwolnione.");
        }

        // --- RESZTA METOD (BEZ WIĘKSZYCH ZMIAN) ---

        public void UtworzNowaSzkole(string nazwa, string adres, string wlascicielImie)
        {
            var menadzer = new Menadzer { IdMenadzera = 1, Imie = "Jan", Nazwisko = "Kierownik" };
            var szkola = new Szkola
            {
                IdSzkoly = (uint)(_unitOfWork.SzkolaRepository.GetAll().Count + 1),
                Nazwa = nazwa,
                Adres = adres,
                CzyAktywna = true,
                Menadzer = menadzer,
                Instruktorzy = new List<Instruktor>(),
                Kursanci = new List<Kursant>(),
                Kursy = new List<Kurs>(),
                Pojazdy = new List<Pojazd>()
            };
            _unitOfWork.SzkolaRepository.Add(szkola);
            _unitOfWork.Save();
            Console.WriteLine($"[SYSTEM] Utworzono szkołę: {nazwa}");
        }

        public void DodajInstruktora(Szkola s, Instruktor i) { if (s != null) { s.Instruktorzy.Add(i); _unitOfWork.Save(); } }
        public void DodajPojazd(Szkola s, Pojazd p) { if (s != null) { s.Pojazdy.Add(p); _unitOfWork.Save(); } }
        public void DodajKurs(Szkola s, Kurs k) { if (s != null) { s.Kursy.Add(k); _unitOfWork.Save(); } }
        public void RejestrujKursanta(Szkola s, Kursant k) { if (s != null) { s.Kursanci.Add(k); _unitOfWork.Save(); } }

        public List<Szkola> PobierzWszystkieSzkoly() => _unitOfWork.SzkolaRepository.GetAll();
        public Szkola PobierzSzkole(uint id) => _unitOfWork.SzkolaRepository.Get(id);
        public List<Kurs> PobierzKursyInstruktora(Szkola s, uint id) => s.Kursy.Where(k => k.Instruktor?.IdInstruktora == id).ToList();
        public Kurs PobierzKursKursanta(Szkola s, uint id) => s.Kursy.FirstOrDefault(k => k.Uczestnicy.Any(u => u.IdKursanta == id));

        public void ZapiszKursantaNaKurs(Kursant k, Kurs kurs)
        {
            if (k.Kategoria == kurs.Kategoria && !kurs.Uczestnicy.Contains(k))
            {
                kurs.Uczestnicy.Add(k);
                _unitOfWork.Save();
                Console.WriteLine("Zapisano na kurs.");
            }
        }

        public void PrzypiszInstruktoraDoKursu(Kurs k, Instruktor i)
        {
            if (i.Uprawnienia.Contains(k.Kategoria))
            {
                k.Instruktor = i;
                _unitOfWork.Save();
                Console.WriteLine("Przypisano instruktora.");
            }
        }

        public void PrzypiszInstruktoraDoPojazdu(Pojazd p, Instruktor i)
        {
            if (p.Status == StatusPojazdu.Sprawny)
            {
                p.PrzypisanyInstruktor = i;
                _unitOfWork.Save();
                Console.WriteLine($"Przypisano {i.Nazwisko} do pojazdu {p.NrRejestracyjny}");
            }
            else Console.WriteLine("Nie można przypisać - pojazd niesprawny.");
        }

        public void ZmienStatusPojazdu(Pojazd p, StatusPojazdu s)
        {
            p.Status = s;
            if (s != StatusPojazdu.Sprawny) p.PrzypisanyInstruktor = null;
            _unitOfWork.Save();
        }

        public void ZatwierdzPlatnosc(Platnosc p)
        {
            p.Status = true;
            if (p.Kursant != null) p.Kursant.Oplacony = true;
            _unitOfWork.Save();
        }

        public void DodajTerminDoHarmonogramu(Kurs k, string t)
        {
            if (k.Harmonogram == null) k.Harmonogram = new Harmonogram { Terminy = new List<string>() };
            k.Harmonogram.Terminy.Add(t);
            _unitOfWork.Save();
        }
    }
}