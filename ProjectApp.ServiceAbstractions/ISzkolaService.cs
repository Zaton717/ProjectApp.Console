using ProjectApp.DataModel;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface ISzkolaService
    {
        // --- TWORZENIE ---
        void UtworzNowaSzkole(string nazwa, string adres, string wlascicielImie);
        void DodajInstruktora(Szkola szkola, Instruktor instruktor);
        void DodajPojazd(Szkola szkola, Pojazd pojazd);
        void DodajKurs(Szkola szkola, Kurs kurs);
        void RejestrujKursanta(Szkola szkola, Kursant kursant);

        // --- ZARZĄDZANIE (WŁAŚCICIEL) --- NOWE!
        void ZamknijSzkole(Szkola szkola);
        void ZwolnijInstruktora(Szkola szkola, uint idInstruktora);
        void ZmienMenadzera(Szkola szkola, string noweImie, string noweNazwisko);

        // --- POBIERANIE ---
        List<Szkola> PobierzWszystkieSzkoly();
        Szkola PobierzSzkole(uint id);
        List<Kurs> PobierzKursyInstruktora(Szkola szkola, uint idInstruktora);
        Kurs PobierzKursKursanta(Szkola szkola, uint idKursanta);

        // --- AKCJE ---
        void ZapiszKursantaNaKurs(Kursant kursant, Kurs kurs);
        void PrzypiszInstruktoraDoKursu(Kurs kurs, Instruktor instruktor);
        void PrzypiszInstruktoraDoPojazdu(Pojazd pojazd, Instruktor instruktor);
        void ZmienStatusPojazdu(Pojazd pojazd, StatusPojazdu status);
        void ZatwierdzPlatnosc(Platnosc platnosc);
        void DodajTerminDoHarmonogramu(Kurs kurs, string termin);
    }
}