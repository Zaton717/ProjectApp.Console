using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Linq;
using Xunit;

namespace ProjectApp.Test
{
    public class LogikaBiznesowaTests : IClassFixture<InMemoryServicesFixture>
    {
        private readonly ISzkolaService _szkolaSvc;
        private readonly IKursService _kursSvc;
        private readonly IKursantService _studentSvc;

        public LogikaBiznesowaTests(InMemoryServicesFixture fixture)
        {
            _szkolaSvc = fixture.SzkolaService;
            _kursSvc = fixture.KursService;
            _studentSvc = fixture.KursantService;
        }

        [Fact]
        public void UtworzKurs_PowinienZapisacCeneINumer()
        {

            var szkola = _szkolaSvc.GetAll().First();
            decimal cenaKursu = 4500m;


            var idKursu = _kursSvc.UtworzKurs(szkola.Id, KategoriaPrawaJazdy.C, cenaKursu);


            var kurs = _kursSvc.GetAll(szkola.Id).FirstOrDefault(k => k.Id == idKursu);
            Assert.NotNull(kurs);
            Assert.Equal(cenaKursu, kurs.Cena);
            Assert.True(kurs.Numer > 0);
        }

        [Fact]
        public void OplacKurs_PowinienZmienicStatusNaOplacony()
        {
            var kId = _studentSvc.Create("Jan", "Testowy", DateTime.Now);

            var kursantPrzed = _studentSvc.Get(kId);
            Assert.False(kursantPrzed!.Oplacony);

            bool wynik = _studentSvc.OplacKurs(kId);

            Assert.True(wynik);
            var kursantPo = _studentSvc.Get(kId);
            Assert.True(kursantPo!.Oplacony);
        }

        [Fact]
        public void ZapisNaKurs_PowinienPrzypisacKursantaDoSzkolyIKursu()
        {
            var sId = _szkolaSvc.Create("Szkoła Testowa", "Testowo", "Admin", "Admin");
            var kId = _studentSvc.Create("Adam", "Nowy", DateTime.Now.AddYears(-20));
            var kursId = _kursSvc.UtworzKurs(sId, KategoriaPrawaJazdy.A, 1000m);

            var wynik = _kursSvc.ZapiszKursanta(sId, kursId, kId);

            Assert.True(wynik);

            var kurs = _kursSvc.GetAll(sId).FirstOrDefault(k => k.Id == kursId);
            Assert.Contains(kurs!.Uczestnicy, u => u.Id == kId);

            var szkola = _szkolaSvc.Get(sId);
            Assert.Contains(szkola!.Kursanci, u => u.Id == kId);
        }
    }
}