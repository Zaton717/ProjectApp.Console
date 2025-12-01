using ProjectApp.DataModel;
using System.Collections.Generic;

namespace ProjectApp.ServiceAbstractions
{
    public interface ISzkolaService
    {
        // --- INICJALIZACJA ---
        void UtworzNowaSzkole(string nazwa, string adres, string wlascicielImie);
        void DodajInstruktora(Szkola szkola, Instruktor instruktor);
        void DodajKurs(Szkola szkola, Kurs kurs);
        void RejestrujKursanta(Szkola szkola, Kursant kursant);

        // --- POBIERANIE DANYCH ---
        List<Szkola> PobierzWszystkieSzkoly();
        Szkola PobierzSzkole(uint id);
        List<Kurs> PobierzKursyInstruktora(Szkola szkola, uint idInstruktora);

        // --- LOGIKA BIZNESOWA (AKCJE) ---
        void ZapiszKursantaNaKurs(Kursant kursant, Kurs kurs);
        void PrzypiszInstruktoraDoKursu(Kurs kurs, Instruktor instruktor);
        void ZatwierdzPlatnosc(Platnosc platnosc);
        void DodajTerminDoHarmonogramu(Kurs kurs, string termin);
    }
}