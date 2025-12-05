using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic; // Potrzebne do List<>
using System.Linq;

namespace ProjectApp.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly ISzkolaService _sSvc;
        private readonly IInstruktorService _iSvc;
        private readonly IPojazdService _pSvc;
        private readonly IKursService _kSvc;
        private readonly IKursantService _studentSvc;

        public DataSeeder(ISzkolaService s, IInstruktorService i, IPojazdService p, IKursService k, IKursantService st)
        {
            _sSvc = s; _iSvc = i; _pSvc = p; _kSvc = k; _studentSvc = st;
        }

        public void Seed()
        {
            if (_sSvc.GetAll().Any()) return;

            // SZKOŁA 1: WARSZAWA
            var s1 = _sSvc.Create("Auto-Mistrz", "Warszawa, Centrum", "Jan", "Kierownik");

            // ZMIANA: Przekazujemy listę uprawnień (B)
            _iSvc.Zatrudnij(s1, "Marek", "Drogowy", new List<KategoriaPrawaJazdy> { KategoriaPrawaJazdy.B });
            var i1 = _iSvc.GetAll(s1).First();

            _pSvc.DodajPojazd(s1, "Toyota", "Yaris", "WA 12345", TypPojazdu.SamochodOsobowy);
            var p1 = _pSvc.GetAll(s1).First();
            _pSvc.PrzypiszInstruktora(s1, p1.Id, i1.Id);

            var k1_1 = _studentSvc.Create("Kamil", "Uczeń", new DateTime(2005, 5, 10));
            var k1_2 = _studentSvc.Create("Anna", "Nowa", new DateTime(2000, 1, 15));

            var kurs1 = _kSvc.UtworzKurs(s1, KategoriaPrawaJazdy.B, 3200m);
            _kSvc.PrzypiszInstruktora(s1, kurs1, i1.Id);
            _kSvc.DodajTermin(s1, kurs1, "Poniedziałek 10:00");
            _kSvc.DodajTermin(s1, kurs1, "Środa 14:00");
            _kSvc.ZapiszKursanta(s1, kurs1, k1_1);
            _kSvc.ZapiszKursanta(s1, kurs1, k1_2);


            // SZKOŁA 2: KRAKÓW
            var s2 = _sSvc.Create("Moto-Krak", "Kraków, Rynek", "Piotr", "Biker");

            // ZMIANA: Przekazujemy listę uprawnień (A oraz B)
            _iSvc.Zatrudnij(s2, "Krzysztof", "Ścigacz", new List<KategoriaPrawaJazdy> { KategoriaPrawaJazdy.A, KategoriaPrawaJazdy.B });
            var i2 = _iSvc.GetAll(s2).First();

            _pSvc.DodajPojazd(s2, "Yamaha", "MT-07", "KR MOTO1", TypPojazdu.Motocykl);
            var p2 = _pSvc.GetAll(s2).First();
            _pSvc.PrzypiszInstruktora(s2, p2.Id, i2.Id);

            var k2_1 = _studentSvc.Create("Tomasz", "Szybki", new DateTime(1995, 10, 12));
            var k2_2 = _studentSvc.Create("Ewa", "Zwinna", new DateTime(1998, 3, 30));

            var kurs2 = _kSvc.UtworzKurs(s2, KategoriaPrawaJazdy.A, 1800m);
            _kSvc.PrzypiszInstruktora(s2, kurs2, i2.Id);
            _kSvc.DodajTermin(s2, kurs2, "Wtorek 17:00");
            _kSvc.ZapiszKursanta(s2, kurs2, k2_1);
            _kSvc.ZapiszKursanta(s2, kurs2, k2_2);
            _studentSvc.OplacKurs(k2_1);


            // SZKOŁA 3: GDAŃSK
            var s3 = _sSvc.Create("Trans-Port", "Gdańsk, Portowa", "Zdzisław", "Tir");

            // ZMIANA: Przekazujemy listę uprawnień (C)
            _iSvc.Zatrudnij(s3, "Bogdan", "Duży", new List<KategoriaPrawaJazdy> { KategoriaPrawaJazdy.C });
            var i3 = _iSvc.GetAll(s3).First();

            _pSvc.DodajPojazd(s3, "MAN", "TGA", "GD BIG01", TypPojazdu.Ciezarowka);
            var p3 = _pSvc.GetAll(s3).First();
            _pSvc.PrzypiszInstruktora(s3, p3.Id, i3.Id);

            var k3_1 = _studentSvc.Create("Olek", "Mocny", new DateTime(1980, 12, 12));
            var k3_2 = _studentSvc.Create("Bartek", "Kierowca", new DateTime(1990, 7, 7));

            var kurs3 = _kSvc.UtworzKurs(s3, KategoriaPrawaJazdy.C, 5500m);
            _kSvc.PrzypiszInstruktora(s3, kurs3, i3.Id);
            _kSvc.DodajTermin(s3, kurs3, "Weekend 08:00 - 16:00");
            _kSvc.ZapiszKursanta(s3, kurs3, k3_1);
            _kSvc.ZapiszKursanta(s3, kurs3, k3_2);
        }
    }
}