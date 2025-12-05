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
        public void ZapisNaKurs_PowinienDodacKursantaDoListy()
        {
            // Arrange - przygotowanie danych
            // 1. Tworzymy nową szkołę (żeby test był czysty)
            var sId = _szkolaSvc.Create("Testowa Szkoła", "Testowo", "Admin", "Admin");

            // 2. Tworzymy kursanta
            var kId = _studentSvc.Create("Test", "Student", DateTime.Now.AddYears(-20));

            // 3. Tworzymy kurs
            var kursId = _kursSvc.UtworzKurs(sId, KategoriaPrawaJazdy.A);

            // Act - wykonanie akcji (Core Logic)
            var wynik = _kursSvc.ZapiszKursanta(sId, kursId, kId);

            // Assert - sprawdzenie
            Assert.True(wynik, "ZapiszKursanta zwróciło false");

            var kurs = _kursSvc.GetAll(sId).FirstOrDefault(k => k.Id == kursId);
            Assert.NotNull(kurs);

            // Sprawdzamy czy kursant faktycznie jest na liście uczestników kursu
            Assert.Contains(kurs!.Uczestnicy, u => u.Id == kId);
        }

        [Fact]
        public void UtworzKurs_PowinienZwrocicPoprawneId()
        {
            var szkola = _szkolaSvc.GetAll().First();

            var idKursu = _kursSvc.UtworzKurs(szkola.Id, KategoriaPrawaJazdy.C);

            Assert.NotEqual(Guid.Empty, idKursu);

            var kursy = _kursSvc.GetAll(szkola.Id);
            Assert.Contains(kursy, k => k.Id == idKursu && k.Kategoria == KategoriaPrawaJazdy.C);
        }
    }
}