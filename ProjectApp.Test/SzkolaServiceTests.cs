using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System.Linq;
using Xunit;

namespace ProjectApp.Test
{
    // Klasa korzysta z Fixture, więc nie musi tworzyć bazy od nowa dla każdego testu
    public class SzkolaServiceTests : IClassFixture<InMemoryServicesFixture>
    {
        private readonly ISzkolaService _szkolaSvc;
        private readonly IInstruktorService _instrSvc;
        private readonly IPojazdService _pojazdSvc;

        public SzkolaServiceTests(InMemoryServicesFixture fixture)
        {
            _szkolaSvc = fixture.SzkolaService;
            _instrSvc = fixture.InstruktorService;
            _pojazdSvc = fixture.PojazdService;
        }

        [Fact]
        public void Seed_PowinienUtworzycSzkoly()
        {
            // Act
            var szkoly = _szkolaSvc.GetAll();

            // Assert
            Assert.NotEmpty(szkoly);
            Assert.Contains(szkoly, s => s.Nazwa == "Auto-Mistrz");
        }

        [Fact]
        public void InstruktorService_PowinienWidziecZatrudnionychWSeedzie()
        {
            // Arrange
            var szkola = _szkolaSvc.GetAll().FirstOrDefault(s => s.Nazwa == "Auto-Mistrz");
            Assert.NotNull(szkola);

            // Act
            var instruktorzy = _instrSvc.GetAll(szkola!.Id);

            // Assert
            Assert.NotEmpty(instruktorzy);
            Assert.Contains(instruktorzy, i => i.Imie == "Marek");
        }

        [Fact]
        public void PojazdService_PowinienDodacNoweAuto()
        {
            // Arrange
            var szkola = _szkolaSvc.GetAll().First();
            int iloscPrzed = _pojazdSvc.GetAll(szkola.Id).Count;

            // Act
            bool sukces = _pojazdSvc.DodajPojazd(szkola.Id, "Test", "Auto", "T1", TypPojazdu.Motocykl);

            // Assert
            Assert.True(sukces);
            Assert.Equal(iloscPrzed + 1, _pojazdSvc.GetAll(szkola.Id).Count);
        }
    }
}