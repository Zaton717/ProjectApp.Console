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

        public void UtworzNowaSzkole(string nazwa, string adres, string wlascicielImie)
        {
            // Tworzymy obiekty zależne
            var wlasciciel = new Wlasciciel { IdWlasciciela = 1, Imie = wlascicielImie, Nazwisko = "Kowalski" };
            var menadzer = new Menadzer { IdMenadzera = 1, Imie = "Jan", Nazwisko = "Zarządca" };

            var szkola = new Szkola
            {
                IdSzkoly = (uint)(_unitOfWork.SzkolaRepository.GetAll().Count + 1),
                Nazwa = nazwa,
                Adres = adres,
                Menadzer = menadzer,
                // Ważne: Inicjalizacja list, aby uniknąć NullReferenceException
                Instruktorzy = new List<Instruktor>(),
                Kursanci = new List<Kursant>(),
                Kursy = new List<Kurs>(),
                Pojazdy = new List<Pojazd>()
            };

            _unitOfWork.SzkolaRepository.Add(szkola);
            _unitOfWork.Save();
            Console.WriteLine($"[SYSTEM] Utworzono szkołę: {nazwa}");
        }

        public void DodajInstruktora(Szkola szkola, Instruktor instruktor)
        {
            if (szkola != null && instruktor != null)
            {
                szkola.Instruktorzy.Add(instruktor);
                _unitOfWork.Save();
            }
        }

        public void DodajKurs(Szkola szkola, Kurs kurs)
        {
            if (szkola != null && kurs != null)
            {
                szkola.Kursy.Add(kurs);
                _unitOfWork.Save();
            }
        }

        public void RejestrujKursanta(Szkola szkola, Kursant kursant)
        {
            if (szkola != null && kursant != null)
            {
                szkola.Kursanci.Add(kursant);
                _unitOfWork.Save();
            }
        }

        public List<Szkola> PobierzWszystkieSzkoly() => _unitOfWork.SzkolaRepository.GetAll();
        public Szkola PobierzSzkole(uint id) => _unitOfWork.SzkolaRepository.Get(id);

        public List<Kurs> PobierzKursyInstruktora(Szkola szkola, uint idInstruktora)
        {
            return szkola.Kursy
                .Where(k => k.Instruktor != null && k.Instruktor.IdInstruktora == idInstruktora)
                .ToList();
        }

        // --- Logika przeniesiona z DataModel ---

        public void ZapiszKursantaNaKurs(Kursant kursant, Kurs kurs)
        {
            if (kursant == null || kurs == null) return;

            if (kursant.Kategoria != kurs.Kategoria)
            {
                Console.WriteLine($"[BŁĄD] Kursant ma kategorię {kursant.Kategoria}, a kurs jest na {kurs.Kategoria}.");
                return;
            }

            if (!kurs.Uczestnicy.Contains(kursant))
            {
                kurs.Uczestnicy.Add(kursant);
                _unitOfWork.Save();
                Console.WriteLine($"[SUKCES] Zapisano kursanta {kursant.Nazwisko} na kurs ID {kurs.IdKursu}.");
            }
            else
            {
                Console.WriteLine("[INFO] Kursant już jest na liście.");
            }
        }

        public void PrzypiszInstruktoraDoKursu(Kurs kurs, Instruktor instruktor)
        {
            if (kurs == null || instruktor == null) return;

            if (instruktor.Uprawnienia.Contains(kurs.Kategoria))
            {
                kurs.Instruktor = instruktor;
                _unitOfWork.Save();
                Console.WriteLine($"[SUKCES] Przypisano instruktora {instruktor.Nazwisko} do kursu.");
            }
            else
            {
                Console.WriteLine($"[BŁĄD] Instruktor nie posiada uprawnień na kategorię {kurs.Kategoria}.");
            }
        }

        public void ZatwierdzPlatnosc(Platnosc platnosc)
        {
            if (platnosc == null) return;

            platnosc.Status = true;
            if (platnosc.Kursant != null)
            {
                platnosc.Kursant.Oplacony = true;
            }
            _unitOfWork.Save();
            Console.WriteLine($"[FINANSE] Płatność zatwierdzona. Status kursanta: Opłacony.");
        }

        public void DodajTerminDoHarmonogramu(Kurs kurs, string termin)
        {
            if (kurs != null)
            {
                if (kurs.Harmonogram == null) kurs.Harmonogram = new Harmonogram();
                kurs.Harmonogram.Terminy.Add(termin);
                _unitOfWork.Save();
                Console.WriteLine($"[TERMIN] Dodano: {termin}");
            }
        }
    }
}