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
            // Arrange
            var szkola = _szkolaSvc.GetAll().First();
            decimal cenaKursu = 4500m;

            // Act
            var idKursu = _kursSvc.UtworzKurs(szkola.Id, KategoriaPrawaJazdy.C, cenaKursu);

            // Assert
            var kurs = _kursSvc.GetAll(szkola.Id).FirstOrDefault(k => k.Id == idKursu);
            Assert.NotNull(kurs);
            Assert.Equal(cenaKursu, kurs.Cena);
            Assert.True(kurs.Numer > 0); // Sprawdzamy czy auto-numeracja działa
        }

        [Fact]
        public void OplacKurs_PowinienZmienicStatusNaOplacony()
        {
            // Arrange
            var kId = _studentSvc.Create("Jan", "Testowy", DateTime.Now);

            // Sprawdzamy stan przed
            var kursantPrzed = _studentSvc.Get(kId);
            Assert.False(kursantPrzed!.Oplacony);

            // Act
            bool wynik = _studentSvc.OplacKurs(kId);

            // Assert
            Assert.True(wynik);
            var kursantPo = _studentSvc.Get(kId);
            Assert.True(kursantPo!.Oplacony);
        }

        [Fact]
        public void ZapisNaKurs_PowinienPrzypisacKursantaDoSzkolyIKursu()
        {
            // Arrange
            var sId = _szkolaSvc.Create("Szkoła Testowa", "Testowo", "Admin", "Admin");
            var kId = _studentSvc.Create("Adam", "Nowy", DateTime.Now.AddYears(-20));
            var kursId = _kursSvc.UtworzKurs(sId, KategoriaPrawaJazdy.A, 1000m);

            // Act
            var wynik = _kursSvc.ZapiszKursanta(sId, kursId, kId);

            // Assert
            Assert.True(wynik);

            // 1. Sprawdź czy jest w kursie
            var kurs = _kursSvc.GetAll(sId).FirstOrDefault(k => k.Id == kursId);
            Assert.Contains(kurs!.Uczestnicy, u => u.Id == kId);

            // 2. Sprawdź czy jest na liście uczniów szkoły (Menadżer musi go widzieć)
            var szkola = _szkolaSvc.Get(sId);
            Assert.Contains(szkola!.Kursanci, u => u.Id == kId);
        }
    }
}