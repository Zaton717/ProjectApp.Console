using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Abstractions;
using ProjectApp.Services;
using ProjectApp.ServiceAbstractions;
using System;

namespace ProjectApp.Test
{
    public class InMemoryServicesFixture : IDisposable
    {
        // Udostêpniamy wszystkie serwisy do testów
        public ISzkolaService SzkolaService { get; }
        public IInstruktorService InstruktorService { get; }
        public IPojazdService PojazdService { get; }
        public IKursService KursService { get; }
        public IKursantService KursantService { get; }

        public InMemoryServicesFixture()
        {
            // 1. Baza danych w pamiêci
            var db = new MemoryDbContext();

            // 2. Repozytoria
            ISzkolaRepository szkolaRepo = new SzkolaRepositoryMemory(db);
            IKursantRepository kursantRepo = new KursantRepositoryMemory(db);

            // 3. Unit of Work
            IUnitOfWork uow = new UnitOfWorkMemory(db);

            // 4. Tworzenie serwisów (Wstrzykiwanie zale¿noœci)
            SzkolaService = new SzkolaService(szkolaRepo, uow);
            InstruktorService = new InstruktorService(szkolaRepo, uow);
            PojazdService = new PojazdService(szkolaRepo, uow);
            KursService = new KursService(szkolaRepo, kursantRepo, uow);
            KursantService = new KursantService(kursantRepo, uow);

            // 5. Uruchomienie Seedera (¿eby testy mia³y na czym pracowaæ)
            var seeder = new DataSeeder(SzkolaService, InstruktorService, PojazdService, KursService, KursantService);
            seeder.Seed();
        }

        public void Dispose()
        {
            // Sprz¹tanie po testach (niepotrzebne przy Memory, ale wymagane przez interfejs)
        }
    }
}