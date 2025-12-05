using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System.Linq;
using Xunit;

namespace ProjectApp.Test
{
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
            var szkoly = _szkolaSvc.GetAll();
            Assert.NotEmpty(szkoly);
            Assert.Contains(szkoly, s => s.Nazwa == "Auto-Mistrz");
        }

        [Fact]
        public void InstruktorService_PowinienWidziecZatrudnionych()
        {
            var szkola = _szkolaSvc.GetAll().FirstOrDefault(s => s.Nazwa == "Auto-Mistrz");
            Assert.NotNull(szkola);

            var instruktorzy = _instrSvc.GetAll(szkola!.Id);
            Assert.NotEmpty(instruktorzy);
            Assert.Contains(instruktorzy, i => i.Imie == "Marek");
        }

        [Fact]
        public void PojazdService_PowinienDodacNoweAuto()
        {
            var szkola = _szkolaSvc.GetAll().First();
            int iloscPrzed = _pojazdSvc.GetAll(szkola.Id).Count;

            bool sukces = _pojazdSvc.DodajPojazd(szkola.Id, "Test", "Auto", "T1", TypPojazdu.Motocykl);

            Assert.True(sukces);
            Assert.Equal(iloscPrzed + 1, _pojazdSvc.GetAll(szkola.Id).Count);
        }
    }
}